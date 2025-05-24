using AutoFixture;
using AutoMapper;
using EcommerceAPI.Models;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Repositories;
using EcommerceAPI.Services.Infrastructure.Interfaces;
using EcommerceAPI.Services.Product;
using Moq;

namespace EcommerceAPIUnitTesting.Services.ProductServiceTest
{
    /// <summary>
    /// Unit tests for the GetProductById method in the ProductService class.
    /// </summary>
    public class GetProductByIdTest
    {
        public readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProductService _productService;
        private readonly Fixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetProductByIdTest"/> class.
        /// </summary>
        public GetProductByIdTest()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockCacheService = new Mock<ICacheService>();
            _mockMapper = new Mock<IMapper>();
            _fixture = new Fixture();

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

            var productDto = CreateProductDto(productId);

            _mockCacheService.Setup(sp => sp.Get<ProductDto>(It.IsAny<string>()))
                .ReturnsAsync(productDto);

            _mockMapper.Setup(sp => sp.Map<ProductDto>(It.IsAny<ProductEntity>()))
                .Returns(productDto);

            // Act
            var result = await _productService.GetProductById(productId);

            // Assert
            Assert.NotNull(result);
            _mockCacheService.Verify(sp => sp.Get<ProductDto>(It.IsAny<string>()));
            _mockProductRepository.Verify(sp => sp.GetProductById(It.IsAny<int>()), Times.Never);
            _mockMapper.Verify(sp => sp.Map<ProductDto>(It.IsAny<ProductEntity>()), Times.Never);
            _mockCacheService.Verify(sp => sp.Set(It.IsAny<string>(), It.IsAny<ProductDto>(), It.IsAny<TimeSpan>()), Times.Never);
        }

        /// <summary>
        /// Gets the produc by identifier product not in cache but in database returns product.
        /// </summary>
        [Fact]
        public async Task GetProductById_ProductNotInCacheButInDb_ReturnsProduct()
        {
            // Arrange
            var productId = 1;

            var productEntity = CreateProductEntity(productId);
            var productDto = CreateProductDto(productId);

            _mockCacheService.Setup(sp => sp.Get<ProductDto>(It.IsAny<string>()))
                .ReturnsAsync((ProductDto?)null);

            _mockProductRepository.Setup(sp => sp.GetProductById(productId))
                .ReturnsAsync(productEntity);

            _mockMapper.Setup(sp => sp.Map<ProductDto>(It.IsAny<ProductEntity>()))
                .Returns(productDto);

            _mockCacheService.Setup(sp => sp.Set(It.IsAny<string>(), It.IsAny<ProductDto>(), It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _productService.GetProductById(productId);

            // Asset
            Assert.NotNull(result);
            _mockCacheService.Verify(sp => sp.Get<ProductDto>(It.IsAny<string>()), Times.Once);
            _mockProductRepository.Verify(sp => sp.GetProductById(It.IsAny<int>()), Times.Once);
            _mockMapper.Verify(sp => sp.Map<ProductDto>(It.IsAny<ProductEntity>()), Times.Once);
            _mockCacheService.Verify(sp => sp.Set(It.IsAny<string>(), It.IsAny<ProductDto>(), It.IsAny<TimeSpan>()), Times.Once);
        }

        /// <summary>
        /// Gets the product by identifier product does not exist exception thrown.
        /// </summary>
        [Fact]
        public async Task GetProductById_ProductDoesNotExist_ThrowsException()
        {
            // Arrange
            var productId = 1;

            _mockCacheService.Setup(sp => sp.Get<ProductDto>(It.IsAny<string>()))
                .ReturnsAsync((ProductDto?)null);

            _mockProductRepository.Setup(sp => sp.GetProductById(It.IsAny<int>()))
                .ReturnsAsync((ProductEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _productService.GetProductById(productId));
            _mockCacheService.Verify(sp => sp.Get<ProductDto>(It.IsAny<string>()), Times.Once);
            _mockProductRepository.Verify(sp => sp.GetProductById(It.IsAny<int>()), Times.Once);
            _mockMapper.Verify(sp => sp.Map<ProductDto>(It.IsAny<ProductEntity>()), Times.Never);
            _mockCacheService.Verify(sp => sp.Set(It.IsAny<string>(), It.IsAny<ProductDto>(), It.IsAny<TimeSpan>()), Times.Never);
        }

        private ProductEntity CreateProductEntity(int productId)
        {
            return _fixture.Build<ProductEntity>()
                .With(p => p.Id, productId)
                .Without(p => p.Category)
                .Without(p => p.ProductTags)
                .Create();
        }

        private ProductDto CreateProductDto(int productId)
        {
            return _fixture.Build<ProductDto>()
             .With(p => p.Id, productId)
             .Without(p => p.Category)
             .Without(p => p.ProductTags)
             .Create();
        }
    }
}