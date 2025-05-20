using AutoMapper;
using EcommerceAPI.Models;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Repositories;
using EcommerceAPI.Services.ElasticService.Interfaces;
using EcommerceAPI.Services.Infrastructure.Interfaces;
using EcommerceAPI.Services.Product;
using Moq;

namespace EcommerceAPIUnitTesting.Services.ProductServiceTesting
{
    public class GetProductByIdTests
    {
        public readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProductService _productService;

        public GetProductByIdTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockCacheService = new Mock<ICacheService>();
            _mockMapper = new Mock<IMapper>();

            _productService = new ProductService(
                _mockProductRepository.Object,
                elasticProductService: null!,
                elasticGenericService: null!,
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
                .Returns(GetTestProductDto());

            // Act
            var result = await _productService.GetProductById(productId);

            // Assert
            Assert.NotNull(result);
        }

        /// <summary>
        /// Gets the produc by identifier product not in cache but in database returns product.
        /// </summary>
        [Fact]
        public async Task GetProductById_ProductNotInCacheButInDb_ReturnsProduct()
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
            .Returns(GetTestProductDto());

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

        private ProductDto GetTestProductDto()
        {
            return new ProductDto
            {
                Id = 1,
                Name = "Test Product",
                Price = 100.00m,
                Description = "Test Description",
                Stock = 10
            };
        }
    }
}
