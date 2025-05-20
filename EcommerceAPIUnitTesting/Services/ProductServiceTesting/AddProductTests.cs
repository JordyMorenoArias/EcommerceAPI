using AutoMapper;
using EcommerceAPI.Models;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Repositories;
using EcommerceAPI.Services.ElasticService.Interfaces;
using EcommerceAPI.Services.Product;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace EcommerceAPIUnitTesting.Services.ProductServiceTesting
{
    /// <summary>
    /// Unit tests for the AddProduct method in the ProductService class.
    /// </summary>
    public class AddProductTests
    {
        public readonly Mock<IProductRepository> _mockProductRepository;
        public readonly Mock<IElasticGenericService<ProductElasticDto>> _mockElasticGenericService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProductService _productService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddProductTests"/> class.
        /// </summary>
        public AddProductTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockElasticGenericService = new Mock<IElasticGenericService<ProductElasticDto>>();
            _mockMapper = new Mock<IMapper>();

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
            var productAdd = new ProductAddDto
            {
                Name = "Test Product",
                Description = "Test Description",
                CategoryId = 1,
                Price = 99.99m,              
            };

            _mockMapper.Setup(sp => sp.Map<ProductEntity>(It.IsAny<ProductAddDto>()))
                .Returns(new ProductEntity
                {
                    Name = productAdd.Name,
                    Description = productAdd.Description,
                    CategoryId = productAdd.CategoryId,
                    Price = productAdd.Price,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });

            _mockProductRepository.Setup(sp => sp.AddProduct(It.IsAny<ProductEntity>()))
                .ReturnsAsync(new ProductEntity
                {
                    Id = 1,
                    Name = productAdd.Name,
                    Description = productAdd.Description,
                    CategoryId = productAdd.CategoryId,
                    Price = productAdd.Price,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });

            _mockElasticGenericService.Setup(sp => sp.Index(It.IsAny<ProductElasticDto>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _mockMapper.Setup(sp => sp.Map<ProductElasticDto>(It.IsAny<ProductEntity>()))
                .Returns(new ProductElasticDto
                {
                    Id = 1,
                    Name = productAdd.Name,
                    Description = productAdd.Description,
                    CategoryId = productAdd.CategoryId,
                    Price = double.Parse(productAdd.Price.ToString()),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });

            _mockMapper.Setup(sp => sp.Map<ProductDto>(It.IsAny<ProductEntity>()))
                .Returns(new ProductDto
                {
                    Id = 1,
                    Name = productAdd.Name,
                    Description = productAdd.Description,
                    CategoryId = productAdd.CategoryId,
                    Price = productAdd.Price,
                    CreatedAt = DateTime.UtcNow
                });

            // Act
            var result = await _productService.AddProduct(userId, productAdd);

            // Assert
            Assert.NotNull(result);
        }
    }
}
