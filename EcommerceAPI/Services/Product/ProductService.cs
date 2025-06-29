﻿using AutoMapper;
using EcommerceAPI.Constants;
using EcommerceAPI.Models;
using EcommerceAPI.Models.DTOs.Generic;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Repositories;
using EcommerceAPI.Services.ElasticService.Interfaces;
using EcommerceAPI.Services.Infrastructure.Interfaces;
using EcommerceAPI.Services.Product.Interfaces;

namespace EcommerceAPI.Services.Product
{
    /// <summary>
    /// Provides operations for managing products, including retrieval, creation, updating,
    /// deletion, and caching of product data.
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IElasticProductService _elasticProductService;
        private readonly IElasticGenericService<ProductElasticDto> _elasticGenericService;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductService"/> class.
        /// </summary>
        /// <param name="productRepository">The product repository.</param>
        /// <param name="elasticProductService">The elastic product service.</param>
        /// <param name="elasticGenericService">The elastic generic service.</param>
        /// <param name="cacheService">The cache service.</param>
        /// <param name="mapper">The mapper.</param>
        public ProductService(IProductRepository productRepository, IElasticProductService elasticProductService, IElasticGenericService<ProductElasticDto> elasticGenericService, ICacheService cacheService, IMapper mapper)
        {
            _productRepository = productRepository;
            _elasticProductService = elasticProductService;
            _elasticGenericService = elasticGenericService;
            _cacheService = cacheService;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a product by its ID, using cache if available.
        /// </summary>
        /// <param name="productId">The product ID.</param>
        /// <returns>The product DTO or null if not found.</returns>
        public async Task<ProductDto?> GetProductById(int productId)
        {
            var cacheKey = $"Product_{productId}";

            var cachedProduct = await _cacheService.Get<ProductDto>(cacheKey);

            if (cachedProduct != null)
                return cachedProduct;

            var product = await _productRepository.GetProductById(productId);

            if (product == null)
                throw new KeyNotFoundException("Product not found");

            var productDto = _mapper.Map<ProductDto>(product);

            await _cacheService.Set(cacheKey, productDto, TimeSpan.FromMinutes(5));

            return productDto;
        }

        /// <summary>
        /// Gets the suggestions.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>A collection of suggested product names that match the query.</returns>
        /// <exception cref="System.ArgumentException">Query cannot be null or empty. - query</exception>
        public async Task<IEnumerable<string>> GetSuggestionsProducts(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("Query cannot be null or empty.", nameof(query));

            var response = await _elasticProductService.GetSuggestionsProducts(query);
            return response;
        }

        /// <summary>
        /// Searches the products.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="parameters">The search parameters.</param>
        /// <returns>
        /// A paginated result containing a list of <see cref="ProductDto"/> objects that match the search criteria.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// Search term cannot be null or empty. - SearchTerm
        /// or
        /// Page and PageSize must be greater than 0.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// Customers can only view active products.
        /// or
        /// Sellers can only view active products of other sellers.
        /// </exception>
        public async Task<PagedResult<ProductDto>> SearchProducts(UserRole role, SearchProductParameters parameters)
        {
            if (string.IsNullOrEmpty(parameters.SearchTerm))
            {
                return new PagedResult<ProductDto>
                {
                    Items = Enumerable.Empty<ProductDto>(),
                    TotalItems = 0,
                    Page = parameters.Page,
                    PageSize = parameters.PageSize
                };
            }

            if (parameters.MinPrice > parameters.MaxPrice)
                throw new ArgumentException("MinPrice cannot be greater than MaxPrice.");

            if (parameters.Page <= 0 || parameters.PageSize <= 0)
                throw new ArgumentException("Page and PageSize must be greater than 0.");

            if (role == UserRole.Customer || role == UserRole.Seller)
            {
                if (parameters.IsActive != null || parameters.IsActive == false)
                    throw new InvalidOperationException("Customers and sellers can only search active products.");
            }

            var productIds = await _elasticProductService.SearchProducts(parameters);

            if (!productIds.Items.Any())
            {
                return new PagedResult<ProductDto>
                {
                    Items = Enumerable.Empty<ProductDto>(),
                    TotalItems = 0,
                    Page = parameters.Page,
                    PageSize = parameters.PageSize
                };
            }

            var products = await _productRepository.GetProductsByIds(productIds.Items);

            return new PagedResult<ProductDto>
            {
                Items = _mapper.Map<IEnumerable<ProductDto>>(products),
                TotalItems = productIds.TotalItems,
                Page = productIds.Page,
                PageSize = productIds.PageSize
            };
        }

        /// <summary>
        /// Gets the products.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>
        /// A <see cref="PagedResult{ProductDto}"/> containing the list of products that match the specified criteria.
        /// The result may come from cache if previously requested.
        /// </returns>
        /// <exception cref="System.ArgumentException">Page and PageSize must be greater than 0.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">role - null</exception>
        public async Task<PagedResult<ProductDto>> GetProducts(int userId, UserRole role, QueryProductParameters parameters)
        {
            if (parameters.Page <= 0 || parameters.PageSize <= 0)
                throw new ArgumentException("Page and PageSize must be greater than 0.");

            switch (role)
            {
                case UserRole.Customer:
                    if (parameters.IsActive != null && parameters.IsActive == false)
                        throw new InvalidOperationException("Customers can only view active products.");
                    break;

                case UserRole.Seller:
                    if (!parameters.UserId.HasValue || parameters.UserId != userId)
                    {
                        if (parameters.IsActive != null && parameters.IsActive == false)
                            throw new InvalidOperationException("Sellers can only view active products of other sellers.");

                        parameters.IsActive = true; // Ensure only active products are fetched for other sellers
                    }
                    // else: seller viewing their own products; allow active and inactive
                    break;

                case UserRole.Admin:
                    // Admin has full access; no restrictions
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(role), role, null);
            }

            var userIdPart = parameters.UserId.HasValue
                ? parameters.UserId.Value.ToString()
                : "all_users";

            var isActivePart = parameters.IsActive.HasValue 
                ? parameters.IsActive.Value.ToString() 
                : "all";

            var categoryPart = parameters.CategoryId.HasValue
                ? parameters.CategoryId.Value.ToString()
                : "all_categories";

            var cacheKey = $"Products_{categoryPart}_{userIdPart}_Page_{parameters.Page}_PageSize_{parameters.PageSize}_IsActive_{isActivePart}";

            var cachedProducts = await _cacheService.Get<PagedResult<ProductDto>>(cacheKey);

            if (cachedProducts != null)
                return cachedProducts;

            var products = await _productRepository.GetProducts(parameters);
            var productsDtos = _mapper.Map<PagedResult<ProductDto>>(products);

            await _cacheService.Set(cacheKey, productsDtos, TimeSpan.FromMinutes(5));
            return productsDtos;
        }

        /// <summary>
        /// Adds a new product for the specified user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="productAdd">The product data to add.</param>
        /// <returns>The newly added product DTO.</returns>
        public async Task<ProductDto> AddProduct(int userId, ProductAddDto productAdd)
        {
            var product = _mapper.Map<ProductEntity>(productAdd);
            product.UserId = userId;

            var addedProduct = await _productRepository.AddProduct(product);

            var elasticProduct = _mapper.Map<ProductElasticDto>(addedProduct);
            await _elasticGenericService.Index(elasticProduct, elasticProduct.Id.ToString());

            return _mapper.Map<ProductDto>(addedProduct);
        }

        /// <summary>
        /// Updates an existing product, if the user owns it.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="productId">The product ID.</param>
        /// <param name="productUpdate">The updated product data.</param>
        /// <returns>The updated product DTO or null if not found.</returns>
        public async Task<ProductDto?> UpdateProduct(int userId, ProductUpdateDto productUpdate)
        {
            var product = await _productRepository.GetProductById(productUpdate.Id);

            if (product == null)
                throw new KeyNotFoundException("Product not found");

            if (product.UserId != userId)
                throw new InvalidOperationException("You are not authorized to update this product.");

            _mapper.Map(productUpdate, product);
            var updatedProduct = await _productRepository.UpdateProduct(product);

            var elasticProduct = _mapper.Map<ProductElasticDto>(updatedProduct);
            await _elasticGenericService.Index(elasticProduct, elasticProduct.Id.ToString());

            await _cacheService.Remove($"Product_{updatedProduct.Id}");
            return _mapper.Map<ProductDto>(updatedProduct);
        }

        /// <summary>
        /// Deletes a product owned by the specified user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="productId">The product ID.</param>
        /// <returns><c>true</c> if the product was deleted; otherwise, <c>false</c>.</returns>
        public async Task<bool> DeleteProduct(int userId, UserRole role, int productId)
        {
            var product = await _productRepository.GetProductById(productId);

            if (product == null)
                throw new KeyNotFoundException("Product not found");

            if (role != UserRole.Admin && product.UserId != userId)
                throw new InvalidOperationException("You are not authorized to delete this product.");

            var result = await _productRepository.DeleteProduct(productId);

            if (result)
            {
                await _cacheService.Remove($"Product_{product.Id}");

                await _elasticGenericService.Delete(product.Id.ToString());
            }

            return result;
        }
    }
}