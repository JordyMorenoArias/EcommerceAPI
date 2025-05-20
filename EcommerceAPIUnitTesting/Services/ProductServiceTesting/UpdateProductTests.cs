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
    /// <summary>
    /// Unit tests for the UpdateProduct method in the ProductService class.
    /// </summary>
    public class UpdateProductTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IElasticGenericService<ProductElasticDto>> _mockElasticGenericService;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProductService _productService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateProductTests"/> class.
        /// </summary>
        public UpdateProductTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockElasticGenericService = new Mock<IElasticGenericService<ProductElasticDto>>();
            _mockCacheService = new Mock<ICacheService>();
            _mockMapper = new Mock<IMapper>();

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
        public async Task UpdateProduct_ProductExistsAndBelongsToUser_UpdateSeccesfully()
        {
            // Arrange
            var userId = 1;
            var productUpdate = new ProductUpdateDto
            {
                Id = 1,
                Name = "Updated Product",
                Description = "Updated Description",
                Price = 150.00m,
                Stock = 20
            };

            _mockProductRepository.Setup(sp => sp.GetProductById(It.IsAny<int>()))
                .ReturnsAsync(new ProductEntity
                {
                    Id = 1,
                    Name = "Test Product",
                    Description = "Test Description",
                    Price = 100.00m,
                    Stock = 10,
                    UserId = 1
                });


            _mockMapper.Setup(sp => sp.Map(It.IsAny<ProductUpdateDto>(), It.IsAny<ProductEntity>()))
                .Callback<ProductUpdateDto, ProductEntity>((src, dest) =>
                {
                    dest.Name = src.Name;
                    dest.Description = src.Description;
                    dest.Price = src.Price;
                    dest.Stock = src.Stock;
                });

            _mockProductRepository.Setup(sp => sp.UpdateProduct(It.IsAny<ProductEntity>()))
                .ReturnsAsync(new ProductEntity
                {
                    Id = 1,
                    Name = "Updated Product",
                    Description = "Updated Description",
                    Price = 150.00m,
                    Stock = 20,
                    UserId = userId
                });

            _mockMapper.Setup(sp => sp.Map<ProductElasticDto>(It.IsAny<ProductEntity>()))
                .Returns(new ProductElasticDto
                {
                    Id = 1,
                    Name = "Updated Product",
                    Description = "Updated Description",
                });

            _mockElasticGenericService.Setup(e => e.Index(It.IsAny<ProductElasticDto>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _mockCacheService.Setup(sp => sp.Remove(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _mockMapper.Setup(sp => sp.Map<ProductDto>(It.IsAny<ProductEntity>()))
                .Returns(new ProductDto
                {
                    Id = 1,
                    Name = "Updated Product",
                    Description = "Updated Description",
                    Price = 150.00m,
                    Stock = 20
                });

            // Act
            var result = await _productService.UpdateProduct(userId, productUpdate);

            // Assert
            Assert.NotNull(result);
        }

        /// <summary>
        /// Updates the product product does not exist throws key not found exception.
        /// </summary>
        [Fact]
        public async Task UpdateProduct_ProductDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            var userId = 1;
            var productUpdate = new ProductUpdateDto
            {
                Id = 1,
                Name = "Updated Product",
                Description = "Updated Description",
                Price = 150.00m,
                Stock = 20
            };

            _mockProductRepository.Setup(sp => sp.GetProductById(It.IsAny<int>()))
                .ReturnsAsync((ProductEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _productService.UpdateProduct(userId, productUpdate));
        }

        /// <summary>
        /// Updates the product product not owned by user throws invalid operation exception.
        /// </summary>
        [Fact]
        public async Task UpdateProduct_ProductNotOwnedByUser_ThrowsInvalidOperationException()
        {
            // Arrange
            var userId = 1;
            var productUpdate = new ProductUpdateDto
            {
                Id = 1,
                Name = "Updated Product",
                Description = "Updated Description",
                Price = 150.00m,
                Stock = 20,              
            };

            _mockProductRepository.Setup(sp => sp.GetProductById(It.IsAny<int>()))
                .ReturnsAsync(new ProductEntity
                {
                    Id = 1,
                    Name = "Test Product",
                    Description = "Test Description",
                    Price = 100.00m,
                    Stock = 10,
                    UserId = 2 // Different user ID
                });

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _productService.UpdateProduct(userId, productUpdate));
        }
    }
}
