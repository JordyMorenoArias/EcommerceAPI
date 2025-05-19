using AutoMapper;
using EcommerceAPI.Models;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Repositories;
using EcommerceAPI.Services.ElasticService.Interfaces;
using EcommerceAPI.Services.Infrastructure.Interfaces;
using EcommerceAPI.Services.Product;
using EcommerceAPI.Services.Product.Interfaces;
using Moq;

namespace EcommerceAPIUnitTesting.Services
{
    /// <summary>
    /// Unit tests for the ProductService class.
    /// </summary>
    public class ProductServiceTest
    {
        public readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IElasticProductService> _mockElasticProductService;
        private readonly Mock<IElasticGenericService<ProductElasticDto>> _mockElasticGenericService;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProductService _productService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductServiceTest"/> class.
        /// </summary>
        public ProductServiceTest()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockElasticProductService = new Mock<IElasticProductService>();
            _mockElasticGenericService = new Mock<IElasticGenericService<ProductElasticDto>>();
            _mockCacheService = new Mock<ICacheService>();
            _mockMapper = new Mock<IMapper>();

            _productService = new ProductService(
                _mockProductRepository.Object,
                _mockElasticProductService.Object,
                _mockElasticGenericService.Object,
                _mockCacheService.Object,
                _mockMapper.Object
        );
        }

        /// <summary>
        /// Gets the product by identifier product in cache returns product.
        /// </summary>
        [Fact]
        public async Task GetProductById_ProductInCache_ReturnsProduct()
        {
            // Arrange
            int productId = 1;

            _mockCacheService.Setup(sp => sp.Get<ProductDto>(It.IsAny<string>())).ReturnsAsync(new ProductDto
            {
                Id = productId,
                Name = "Test Product",
                Price = 100.00m,
                Description = "Test Description",
                Stock = 10
            });

            _mockMapper.Setup(sp => sp.Map<ProductDto>(It.IsAny<ProductEntity>()))
                .Returns(new ProductDto
                {
                    Id = productId,
                    Name = "Test Product",
                    Price = 100.00m,
                    Description = "Test Description",
                    Stock = 10
                });

            // Act
            var result = await _productService.GetProductById(productId);

            // Assert
            Assert.NotNull(result);
        }

        /// <summary>
        /// Gets the produc by identifier product not in cache but in database returns product.
        /// </summary>
        [Fact]
        public async Task GetProducById_ProductNotInCacheButInDb_ReturnsProduct()
        {
            // Arrange
            var productId = 1;

            _mockCacheService.Setup(sp => sp.Get<ProductDto>(It.IsAny<string>())).ReturnsAsync((ProductDto?)null);

            _mockProductRepository.Setup(sp => sp.GetProductById(productId)).ReturnsAsync(new ProductEntity
            {
                Id = productId,
                Name = "Test Product",
                Price = 100.00m,
                Description = "Test Description",
                Stock = 10
            });

            _mockMapper.Setup(sp => sp.Map<ProductDto>(It.IsAny<ProductEntity>()))
            .Returns(new ProductDto
            {
                Id = productId,
                Name = "Test Product",
                Price = 100.00m,
                Description = "Test Description",
                Stock = 10
            });

            // Act
            var result = await _productService.GetProductById(productId);

            // Asset
            Assert.NotNull(result);
        }

        /// <summary>
        /// Gets the product by identifier product does not exist exception thrown.
        /// </summary>
        [Fact]
        public async Task GetProductById_ProductDoesNotExist_ThrowsException()
        {
            // Arrange
            var productId = 1;

            _mockCacheService.Setup(sp => sp.Get<ProductDto>(It.IsAny<string>())).ReturnsAsync((ProductDto?)null);

            _mockProductRepository.Setup(sp => sp.GetProductById(productId)).ReturnsAsync((ProductEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _productService.GetProductById(productId));
        }

        [Fact]
        public async Task GetSuggestions_ValidQuery_ReturnsResults()
        {
            // Arrange
            var query = "test";

            _mockElasticProductService.Setup(sp => sp.GetSuggestionsProducts(query))
                .ReturnsAsync(new List<string> { "Test Product 1", "Test Product 2" });

            // Act
            var result = await _productService.GetSuggestionsProducts(query);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetSuggestions_EmptyOrNullQuery_ThrowsException()
        {
            // Arrange
            string query = string.Empty;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _productService.GetSuggestionsProducts(query));
        }
    }
}