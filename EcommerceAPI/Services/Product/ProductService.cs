using AutoMapper;
using EcommerceAPI.Constants;
using EcommerceAPI.Models;
using EcommerceAPI.Models.DTOs;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Repositories;
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
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, ICacheService cacheService, IMapper mapper)
        {
            _productRepository = productRepository;
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
        public async Task<PagedResult<ProductDto>> GetProducts(int userId, UserRole role, ProductQueryParameters parameters)
        {
            if (parameters.Page <= 0 || parameters.PageSize <= 0)
                throw new ArgumentException("Page and PageSize must be greater than 0.");

            switch (role)
            {
                case UserRole.Customer:
                    parameters.IsActive = true;
                    break;

                case UserRole.Seller:
                    if (!parameters.UserId.HasValue || parameters.UserId != userId)
                    {
                        parameters.IsActive = true;
                    }
                    break;

                case UserRole.Admin:
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

            var categoryPart = parameters.Category.HasValue
                ? parameters.Category.Value.ToString()
                : "all_categories";

            var cacheKey = $"Products_{categoryPart}_{userIdPart}_Page_{parameters.Page}_PageSize_{parameters.PageSize}_IsActive_{isActivePart}";

            var cachedProducts = await _cacheService.Get<PagedResult<ProductDto>>(cacheKey);

            if (cachedProducts != null)
                return cachedProducts;

            var products = await _productRepository.GetProducts(parameters);

            var productDtos = _mapper.Map<PagedResult<ProductDto>>(products);

            await _cacheService.Set(cacheKey, productDtos, TimeSpan.FromMinutes(5));
            return productDtos;
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

            await InvalidateProductCache(addedProduct);

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
            product = await _productRepository.UpdateProduct(product);
            
            await InvalidateProductCache(product);

            return _mapper.Map<ProductDto>(product);
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
                await InvalidateProductCache(product);

            return result;
        }

        /// <summary>
        /// Invalidates cache entries related to the given product.
        /// </summary>
        /// <param name="product">The product entity.</param>
        private async Task InvalidateProductCache(ProductEntity product)
        {
            // Invalidates the individual product
            await _cacheService.Remove($"Product_{product.Id}");
        }
    }
}