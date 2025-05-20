using EcommerceAPI.Constants;
using EcommerceAPI.Models;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Repositories;
using EcommerceAPI.Services.ElasticService.Interfaces;
using EcommerceAPI.Services.Infrastructure.Interfaces;
using EcommerceAPI.Services.Product;
using Moq;

namespace EcommerceAPIUnitTesting.Services.ProductServiceTesting
{
    public class DeleteProductTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IElasticGenericService<ProductElasticDto>> _mockElasticGenericService;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly ProductService _productService;

        public DeleteProductTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockElasticGenericService = new Mock<IElasticGenericService<ProductElasticDto>>();
            _mockCacheService = new Mock<ICacheService>();

            _productService = new ProductService(
                _mockProductRepository.Object,
                elasticProductService: null!,
                _mockElasticGenericService.Object,
                _mockCacheService.Object,
                mapper: null!
            );
        }

        [Fact]
        public async Task DeleteProduct_ProductExistsAndUserAuthorized_DeletesSuccessfully()
        {
            // Arrange
            var userId = 1;
            var role = UserRole.Seller;
            var productId = 1;

            _mockProductRepository.Setup(sp => sp.GetProductById(It.IsAny<int>()))
                .ReturnsAsync(new ProductEntity
                {
                    Id = productId,
                    UserId = userId,    
                });

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
        }

        [Fact]
        public async Task DeleteProduct_ProductDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            var userId = 1;
            var role = UserRole.Seller;
            var productId = 1;

            _mockProductRepository.Setup(sp => sp.GetProductById(It.IsAny<int>()))
                .ReturnsAsync((ProductEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _productService.DeleteProduct(userId, role, productId));
        }

        [Fact]
        public async Task DeleteProduct_UserNotAuthorized_ThrowsInvalidOperationException()
        {
            // Arrange
            var userId = 1;
            var role = UserRole.Customer;
            var productId = 1;

            _mockProductRepository.Setup(sp => sp.GetProductById(It.IsAny<int>()))
                .ReturnsAsync(new ProductEntity
                {
                    Id = productId,
                    UserId = 2,
                });

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _productService.DeleteProduct(userId, role, productId));
        }
    }
}
