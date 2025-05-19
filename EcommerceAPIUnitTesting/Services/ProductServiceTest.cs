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
    public class ProductServiceTest
    {

        public readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IElasticProductService> _mockElasticProductService;
        private readonly Mock<IElasticGenericService<ProductElasticDto>> _mockElasticGenericService;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProductService _productService;

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

        [Fact]
        public async Task GetProductById_ProductDoesNotExist_ExceptionThrown()
        {
            // Arrange
            var productId = 1;

            _mockCacheService.Setup(sp => sp.Get<ProductDto>(It.IsAny<string>())).ReturnsAsync((ProductDto?)null);

            _mockProductRepository.Setup(sp => sp.GetProductById(productId)).ReturnsAsync((ProductEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _productService.GetProductById(productId));
        }
    }
}
