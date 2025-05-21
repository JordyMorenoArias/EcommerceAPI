using AutoFixture;
using AutoMapper;
using EcommerceAPI.Models;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Repositories;
using EcommerceAPI.Services.ElasticService.Interfaces;
using EcommerceAPI.Services.Product;
using Moq;

namespace EcommerceAPIUnitTesting.Services.ProductServiceTest
{
    /// <summary>
    /// Unit tests for the AddProduct method in the ProductService class.
    /// </summary>
    public class AddProductTest
    {
        public readonly Mock<IProductRepository> _mockProductRepository;
        public readonly Mock<IElasticGenericService<ProductElasticDto>> _mockElasticGenericService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProductService _productService;
        private readonly Fixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddProductTest"/> class.
        /// </summary>
        public AddProductTest()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockElasticGenericService = new Mock<IElasticGenericService<ProductElasticDto>>();
            _mockMapper = new Mock<IMapper>();
            _fixture = new Fixture();

            _productService = new ProductService(
                _mockProductRepository.Object,
                elasticProductService: null!,
                _mockElasticGenericService.Object,
                cacheService: null!,
                _mockMapper.Object
            );
        }

        /// <summary>
        /// Adds the product valid data adds product and indexes in elastic.
        /// </summary>
        [Fact]
        public async Task AddProduct_ValidData_AddsProductAndIndexesInElastic()
        {
            // Arrange
            int userId = 1;
            var productAdd = _fixture.Create<ProductAddDto>();

            var productEntity = CreateProductEntity();
            var productElasticDto = _fixture.Create<ProductElasticDto>();
            var productDto = CreateProductDto();

            _mockMapper.Setup(sp => sp.Map<ProductEntity>(It.IsAny<ProductAddDto>()))
                .Returns(productEntity);

            _mockProductRepository.Setup(sp => sp.AddProduct(It.IsAny<ProductEntity>()))
                .ReturnsAsync(productEntity);

            _mockElasticGenericService.Setup(sp => sp.Index(It.IsAny<ProductElasticDto>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _mockMapper.Setup(sp => sp.Map<ProductElasticDto>(It.IsAny<ProductEntity>()))
                .Returns(productElasticDto);

            _mockMapper.Setup(sp => sp.Map<ProductDto>(It.IsAny<ProductEntity>()))
                .Returns(productDto);

            // Act
            var result = await _productService.AddProduct(userId, productAdd);

            // Assert
            Assert.NotNull(result);
            _mockMapper.Verify(m => m.Map<ProductEntity>(It.IsAny<ProductAddDto>()), Times.Once);
            _mockProductRepository.Verify(r => r.AddProduct(It.IsAny<ProductEntity>()), Times.Once);
            _mockElasticGenericService.Verify(e => e.Index(It.IsAny<ProductElasticDto>(), It.IsAny<string>()), Times.Once);
            _mockMapper.Verify(m => m.Map<ProductElasticDto>(It.IsAny<ProductEntity>()), Times.Once);
            _mockMapper.Verify(m => m.Map<ProductDto>(It.IsAny<ProductEntity>()), Times.Once);
        }

        private ProductEntity CreateProductEntity()
        {
            return _fixture.Build<ProductEntity>()
                .Without(p => p.ProductTags)
                .Without(p => p.Category)
                .Create();
        }

        private ProductDto CreateProductDto()
        {
            return _fixture.Build<ProductDto>()
                .Without(p => p.ProductTags)
                .Without(p => p.Category)
                .Create();
        }
    }
}
