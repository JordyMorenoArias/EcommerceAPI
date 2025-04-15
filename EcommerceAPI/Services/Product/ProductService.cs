using AutoMapper;
using EcommerceAPI.Constants;
using EcommerceAPI.Models;
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
        /// Retrieves all products, using cache if available.
        /// </summary>
        /// <returns>A collection of all product DTOs.</returns>
        public async Task<IEnumerable<ProductDto>> GetAllProducts()
        {
            var cacheKey = "AllProducts";
            var cachedProducts = await _cacheService.Get<IEnumerable<ProductDto>>(cacheKey);

            if (cachedProducts != null)
                return cachedProducts;

            var products = await _productRepository.GetAllProducts();
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

            await _cacheService.Set(cacheKey, productDtos, TimeSpan.FromMinutes(5));

            return productDtos;
        }

        /// <summary>
        /// Retrieves all active products, using cache if available.
        /// </summary>
        /// <returns>A collection of active product DTOs.</returns>
        public async Task<IEnumerable<ProductDto>> GetActiveProducts()
        {
            var cacheKey = "ActiveProducts";
            var cachedProducts = await _cacheService.Get<IEnumerable<ProductDto>>(cacheKey);

            if (cachedProducts != null)
                return cachedProducts;

            var products = await _productRepository.GetActiveProducts();
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

            await _cacheService.Set(cacheKey, productDtos, TimeSpan.FromMinutes(5));

            return productDtos;
        }

        /// <summary>
        /// Searches for products that match the given query string.
        /// </summary>
        /// <param name="query">The search query.</param>
        /// <returns>A collection of matching product DTOs.</returns>
        public async Task<IEnumerable<ProductDto>> SearchProducts(string query)
        {
            var products = await _productRepository.SearchProducts(query);

            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        /// <summary>
        /// Retrieves products by category, using cache if available.
        /// </summary>
        /// <param name="category">The category to filter products by.</param>
        /// <returns>A collection of product DTOs in the specified category.</returns>
        public async Task<IEnumerable<ProductDto>> GetProductsByCategory(CategoryProduct category)
        {
            var cacheKey = $"ProductsByCategory_{category.ToString()}";
            var cachedProducts = await _cacheService.Get<IEnumerable<ProductDto>>(cacheKey);

            if (cachedProducts != null)
                return cachedProducts;

            var products = await _productRepository.GetProductsByCategory(category);
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

            await _cacheService.Set(cacheKey, productDtos, TimeSpan.FromMinutes(5));

            return productDtos;
        }

        /// <summary>
        /// Retrieves active products by category, using cache if available.
        /// </summary>
        /// <param name="category">The category to filter products by.</param>
        /// <returns>A collection of active product DTOs in the specified category.</returns>
        public async Task<IEnumerable<ProductDto>> GetActiveProductsByCategory(CategoryProduct category)
        {
            var cacheKey = $"ActiveProductsByCategory_{category.ToString()}";
            var cachedProducts = await _cacheService.Get<IEnumerable<ProductDto>>(cacheKey);

            if (cachedProducts != null)
                return cachedProducts;

            var products = await _productRepository.GetActiveProductsByCategory(category);
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

            await _cacheService.Set(cacheKey, productDtos, TimeSpan.FromMinutes(5));

            return productDtos;
        }

        /// <summary>
        /// Retrieves active products for a specific user, using cache if available.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>A collection of the user's active product DTOs.</returns>
        public async Task<IEnumerable<ProductDto>> GetActiveProductsByUserId(int userId)
        {
            var cacheKey = $"ActiveProductsByUserId_{userId}";
            var cachedProducts = await _cacheService.Get<IEnumerable<ProductDto>>(cacheKey);

            if (cachedProducts != null)
                return cachedProducts;

            var products = await _productRepository.GetActiveProductsByUserId(userId);
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

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
        public async Task<ProductDto?> UpdateProduct(int userId, int productId, ProductUpdateDto productUpdate)
        {
            var product = await _productRepository.GetProductById(productId);

            if (product == null || product.UserId != userId)
                throw new UnauthorizedAccessException("You are not authorized to update this product.");

            var oldCategory = product.Category;
            var wasActive = product.IsActive;

            _mapper.Map(productUpdate, product);
            await _productRepository.UpdateProduct(product);

            await InvalidateProductCache(product);

            return _mapper.Map<ProductDto>(product);
        }

        /// <summary>
        /// Deletes a product owned by the specified user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="productId">The product ID.</param>
        /// <returns><c>true</c> if the product was deleted; otherwise, <c>false</c>.</returns>
        public async Task<bool> DeleteProduct(int userId, int productId)
        {
            var product = await _productRepository.GetProductById(productId);

            if (product == null || product.UserId != userId)
                throw new UnauthorizedAccessException("You are not authorized to delete this product.");

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

            // Invalidates general lists
            await _cacheService.Remove("AllProducts");
            await _cacheService.Remove("ActiveProducts");

            // Invalidates lists related to the user
            await _cacheService.Remove($"ActiveProductsByUserId_{product.UserId}");

            // Invalidates lists by category
            await _cacheService.Remove($"ProductsByCategory_{product.Category.ToString()}");
            await _cacheService.Remove($"ActiveProductsByCategory_{product.Category.ToString()}");
        }
    }
}
