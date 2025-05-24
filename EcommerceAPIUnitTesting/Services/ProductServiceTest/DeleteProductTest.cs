using AutoFixture;
using EcommerceAPI.Constants;
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
    /// Unit tests for the DeleteProduct method in the ProductService class.
    /// </summary>
    public class DeleteProductTest
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IElasticGenericService<ProductElasticDto>> _mockElasticGenericService;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly ProductService _productService;
        private readonly Fixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteProductTest"/> class.
        /// </summary>
        public DeleteProductTest()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockElasticGenericService = new Mock<IElasticGenericService<ProductElasticDto>>();
            _mockCacheService = new Mock<ICacheService>();
            _fixture = new Fixture();

            _productService = new ProductService(
                _mockProductRepository.Object,
                elasticProductService: null!,
                _mockElasticGenericService.Object,
                _mockCacheService.Object,
                mapper: null!
            );
        }

        /// <summary>
        /// Deletes the product product exists and user authorized deletes successfully.
        /// </summary>
        [Fact]
        public async Task DeleteProduct_ProductExistsAndUserAuthorized_DeletesSuccessfully()
        {
            // Arrange
            var userId = 1;
            var productId = 1;
            var role = UserRole.Seller;

            var productEntity = CreateProductEntity(userId, productId);

            _mockProductRepository.Setup(sp => sp.GetProductById(It.IsAny<int>()))
                .ReturnsAsync(productEntity);

            _mockProductRepository.Setup(sp => sp.DeleteProduct(It.IsAny<int>()))
                .ReturnsAsync(true);

            _mockCacheService.Setup(sp => sp.Remove(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _mockElasticGenericService.Setup(e => e.Delete(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _productService.DeleteProduct(userId, role, productId);

            // Assert
            Assert.True(result);
            _mockProductRepository.Verify(sp => sp.GetProductById(It.IsAny<int>()), Times.Once);
            _mockProductRepository.Verify(sp => sp.DeleteProduct(It.IsAny<int>()), Times.Once);
            _mockCacheService.Verify(sp => sp.Remove(It.IsAny<string>()), Times.Once);
            _mockElasticGenericService.Verify(e => e.Delete(It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        /// Deletes the product product does not exist throws key not found exception.
        /// </summary>
        [Fact]
        public async Task DeleteProduct_ProductDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            var userId = 1;
            var productId = 1;
            var role = UserRole.Seller;

            _mockProductRepository.Setup(sp => sp.GetProductById(It.IsAny<int>()))
                .ReturnsAsync((ProductEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _productService.DeleteProduct(userId, role, productId));
            _mockProductRepository.Verify(sp => sp.GetProductById(It.IsAny<int>()), Times.Once);
            _mockProductRepository.Verify(sp => sp.DeleteProduct(It.IsAny<int>()), Times.Never);
            _mockCacheService.Verify(sp => sp.Remove(It.IsAny<string>()), Times.Never);
            _mockElasticGenericService.Verify(e => e.Delete(It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Deletes the product user not authorized throws invalid operation exception.
        /// </summary>
        [Fact]
        public async Task DeleteProduct_UserNotAuthorized_ThrowsInvalidOperationException()
        {
            // Arrange
            var userId = 1;
            var productId = 1;
            var role = UserRole.Customer;

            var productEntity = CreateProductEntity(userId, 2);

            _mockProductRepository.Setup(sp => sp.GetProductById(It.IsAny<int>()))
                .ReturnsAsync(productEntity);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _productService.DeleteProduct(userId, role, productId));
            _mockProductRepository.Verify(sp => sp.GetProductById(It.IsAny<int>()), Times.Once);
            _mockProductRepository.Verify(sp => sp.DeleteProduct(It.IsAny<int>()), Times.Never);
            _mockCacheService.Verify(sp => sp.Remove(It.IsAny<string>()), Times.Never);
            _mockElasticGenericService.Verify(e => e.Delete(It.IsAny<string>()), Times.Never);
        }

        private ProductEntity CreateProductEntity(int userId, int productId)
        {
            return _fixture.Build<ProductEntity>()
                .With(p => p.Id, userId)
                .With(p => p.UserId, productId)
                .Without(p => p.ProductTags)
                .Without(p => p.Category)
                .Create();
        }
    }
}