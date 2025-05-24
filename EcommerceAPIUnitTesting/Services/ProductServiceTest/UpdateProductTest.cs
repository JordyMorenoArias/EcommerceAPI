using AutoFixture;
using AutoMapper;
using EcommerceAPI.Models;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Repositories;
using EcommerceAPI.Services.ElasticService.Interfaces;
using EcommerceAPI.Services.Infrastructure.Interfaces;
using EcommerceAPI.Services.Product;
using Moq;

namespace EcommerceAPIUnitTesting.Services.ProductServiceTest
{
    /// <summary>
    /// Unit tests for the UpdateProduct method in the ProductService class.
    /// </summary>
    public class UpdateProductTest
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IElasticGenericService<ProductElasticDto>> _mockElasticGenericService;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProductService _productService;
        private readonly Fixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateProductTest"/> class.
        /// </summary>
        public UpdateProductTest()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockElasticGenericService = new Mock<IElasticGenericService<ProductElasticDto>>();
            _mockCacheService = new Mock<ICacheService>();
            _mockMapper = new Mock<IMapper>();
            _fixture = new Fixture();

            _productService = new ProductService(
                _mockProductRepository.Object,
                elasticProductService: null!,
                _mockElasticGenericService.Object,
                _mockCacheService.Object,
                _mockMapper.Object
            );
        }

        /// <summary>
        /// Updates the product product exists and belongs to user update seccesfully.
        /// </summary>
        [Fact]
        public async Task UpdateProduct_ProductExistsAndBelongsToUser_UpdatesSuccessfully()
        {
            // Arrange
            var userId = 1;
            var productUpdate = _fixture.Create<ProductUpdateDto>();

            var productEntity = CreateProductEntity(productUpdate.Id, userId);
            var productDto = CreateProductDto(productUpdate.Id, userId);
            var productElasticDto = _fixture.Create<ProductElasticDto>();

            _mockProductRepository.Setup(r => r.GetProductById(productUpdate.Id))
                .ReturnsAsync(productEntity);

            _mockMapper.Setup(m => m.Map(It.IsAny<ProductUpdateDto>(), It.IsAny<ProductEntity>()))
                .Callback<ProductUpdateDto, ProductEntity>((src, dest) =>
                {
                    dest.Name = src.Name;
                    dest.Description = src.Description;
                    dest.Price = src.Price;
                    dest.Stock = src.Stock;
                });

            _mockProductRepository.Setup(r => r.UpdateProduct(It.IsAny<ProductEntity>()))
                .ReturnsAsync(productEntity);

            _mockMapper.Setup(m => m.Map<ProductElasticDto>(It.IsAny<ProductEntity>()))
                .Returns(productElasticDto);

            _mockElasticGenericService.Setup(e => e.Index(productElasticDto, It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _mockCacheService.Setup(c => c.Remove(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _mockMapper.Setup(m => m.Map<ProductDto>(It.IsAny<ProductEntity>()))
                .Returns(productDto);

            // Act
            var result = await _productService.UpdateProduct(userId, productUpdate);

            // Assert
            Assert.NotNull(result);
            _mockProductRepository.Verify(r => r.GetProductById(productUpdate.Id), Times.Once);
            _mockMapper.Verify(m => m.Map(It.IsAny<ProductUpdateDto>(), It.IsAny<ProductEntity>()), Times.Once);
            _mockProductRepository.Verify(r => r.UpdateProduct(It.IsAny<ProductEntity>()), Times.Once);
            _mockMapper.Verify(m => m.Map<ProductElasticDto>(It.IsAny<ProductEntity>()), Times.Once);
            _mockElasticGenericService.Verify(e => e.Index(productElasticDto, It.IsAny<string>()), Times.Once);
            _mockCacheService.Verify(c => c.Remove(It.IsAny<string>()), Times.Once);
            _mockMapper.Verify(m => m.Map<ProductDto>(It.IsAny<ProductEntity>()), Times.Once);
        }

        private ProductEntity CreateProductEntity(int id, int userId) =>
            _fixture.Build<ProductEntity>()
                .With(p => p.Id, id)
                .With(p => p.UserId, userId)
                .Without(p => p.ProductTags)
                .Without(p => p.Category)
                .Create();

        private ProductDto CreateProductDto(int id, int userId) =>
            _fixture.Build<ProductDto>()
                .With(p => p.Id, id)
                .With(p => p.UserId, userId)
                .Without(p => p.ProductTags)
                .Without(p => p.Category)
                .Create();

        /// <summary>
        /// Updates the product product does not exist throws key not found exception.
        /// </summary>
        [Fact]
        public async Task UpdateProduct_ProductDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            var userId = 1;
            var productUpdate = _fixture.Create<ProductUpdateDto>();

            _mockProductRepository.Setup(sp => sp.GetProductById(productUpdate.Id))
                .ReturnsAsync((ProductEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _productService.UpdateProduct(userId, productUpdate));
            _mockProductRepository.Verify(sp => sp.GetProductById(productUpdate.Id), Times.Once);
            _mockProductRepository.Verify(sp => sp.UpdateProduct(It.IsAny<ProductEntity>()), Times.Never);
            _mockMapper.Verify(sp => sp.Map(It.IsAny<ProductUpdateDto>(), It.IsAny<ProductEntity>()), Times.Never);
            _mockMapper.Verify(sp => sp.Map<ProductElasticDto>(It.IsAny<ProductEntity>()), Times.Never);
            _mockElasticGenericService.Verify(sp => sp.Index(It.IsAny<ProductElasticDto>(), It.IsAny<string>()), Times.Never);
            _mockCacheService.Verify(sp => sp.Remove(It.IsAny<string>()), Times.Never);
            _mockMapper.Verify(sp => sp.Map<ProductDto>(It.IsAny<ProductEntity>()), Times.Never);
        }

        /// <summary>
        /// Updates the product product not owned by user throws invalid operation exception.
        /// </summary>
        [Fact]
        public async Task UpdateProduct_ProductNotOwnedByUser_ThrowsInvalidOperationException()
        {
            // Arrange
            var userId = 1;
            var productUpdate = _fixture.Create<ProductUpdateDto>();

            var productEntity = CreateProductEntity(productUpdate.Id, 2);

            _mockProductRepository.Setup(sp => sp.GetProductById(productUpdate.Id))
                .ReturnsAsync(productEntity);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _productService.UpdateProduct(userId, productUpdate));
            _mockProductRepository.Verify(sp => sp.GetProductById(productUpdate.Id), Times.Once);
            _mockProductRepository.Verify(sp => sp.UpdateProduct(It.IsAny<ProductEntity>()), Times.Never);
            _mockMapper.Verify(sp => sp.Map(It.IsAny<ProductUpdateDto>(), It.IsAny<ProductEntity>()), Times.Never);
            _mockMapper.Verify(sp => sp.Map<ProductElasticDto>(It.IsAny<ProductEntity>()), Times.Never);
            _mockElasticGenericService.Verify(sp => sp.Index(It.IsAny<ProductElasticDto>(), It.IsAny<string>()), Times.Never);
            _mockCacheService.Verify(sp => sp.Remove(It.IsAny<string>()), Times.Never);
            _mockMapper.Verify(sp => sp.Map<ProductDto>(It.IsAny<ProductEntity>()), Times.Never);
        }
    }
}