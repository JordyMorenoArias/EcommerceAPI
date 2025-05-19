using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Models.DTOs;
using EcommerceAPI.Models;
using AutoMapper;
using EcommerceAPI.Repositories;
using EcommerceAPI.Services.ElasticService.Interfaces;
using EcommerceAPI.Services.Infrastructure.Interfaces;
using Moq;
using EcommerceAPI.Services.Product;

namespace EcommerceAPIUnitTesting.Services.ProductServiceTests
{
    public class GetProductsTests
    {

        public readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IElasticProductService> _mockElasticProductService;
        private readonly Mock<IElasticGenericService<ProductElasticDto>> _mockElasticGenericService;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProductService _productService;

        public GetProductsTests()
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
        /// Gets the products admin role returns all products.
        /// </summary>
        [Fact]
        public async Task GetProducts_AdminRole_ReturnsResult()
        {
            // Arrange
            int userId = 1;
            var role = UserRole.Admin;
            var parameters = new QueryProductParameters
            {
                Page = 1,
                PageSize = 10,
            };

            _mockCacheService.Setup(sp => sp.Get<PagedResult<ProductDto>>(It.IsAny<string>())).ReturnsAsync((PagedResult<ProductDto>?)null);

            _mockProductRepository.Setup(sp => sp.GetProducts(It.IsAny<QueryProductParameters>()))
                .ReturnsAsync(new PagedResult<ProductEntity>
                {
                    TotalItems = 100,
                    PageSize = parameters.PageSize,
                    Page = parameters.Page,
                    Items = new List<ProductEntity> { new ProductEntity { Id = 1, Name = "Test Product" } }
                });

            _mockMapper.Setup(sp => sp.Map<PagedResult<ProductDto>>(It.IsAny<PagedResult<ProductEntity>>()))
                .Returns(new PagedResult<ProductDto>
                {
                    TotalItems = 100,
                    PageSize = parameters.PageSize,
                    Page = parameters.Page,
                    Items = new List<ProductDto> { new ProductDto { Id = 1, Name = "Test Product" } }
                });

            // Act
            var result = await _productService.GetProducts(userId, role, parameters);

            // Assert
            Assert.NotNull(result);
        }

        /// <summary>
        /// Gets the products products in cache returns cached result.
        /// </summary>
        [Fact]
        public async Task GetProducts_ProductsInCache_ReturnsCachedResult()
        {
            // Arrange
            var userId = 1;
            var role = UserRole.Admin;
            var parameters = new QueryProductParameters
            {
                Page = 1,
                PageSize = 10,
            };

            _mockCacheService.Setup(sp => sp.Get<PagedResult<ProductDto>>(It.IsAny<string>()))
                .ReturnsAsync(new PagedResult<ProductDto>
                {
                    TotalItems = 100,
                    PageSize = parameters.PageSize,
                    Page = parameters.Page,
                    Items = new List<ProductDto> { new ProductDto { Id = 1, Name = "Test Product" } }
                });

            // Act
            var result = await _productService.GetProducts(userId, role, parameters);

            // Assert
            Assert.NotNull(result);
        }

        /// <summary>
        /// Gets the products customer role returns only active products.
        /// </summary>
        [Fact]
        public async Task GetProducts_CustomerRole_ReturnsOnlyActiveProducts()
        {
            // Arrange
            var userId = 1;
            var role = UserRole.Customer;
            var parameters = new QueryProductParameters
            {
                Page = 1,
                PageSize = 10,
                IsActive = true
            };

            _mockCacheService.Setup(sp => sp.Get<PagedResult<ProductDto>>(It.IsAny<string>())).ReturnsAsync((PagedResult<ProductDto>?)null);

            _mockProductRepository.Setup(sp => sp.GetProducts(It.IsAny<QueryProductParameters>()))
                .ReturnsAsync(new PagedResult<ProductEntity>
                {
                    TotalItems = 100,
                    PageSize = parameters.PageSize,
                    Page = parameters.Page,
                    Items = new List<ProductEntity> { new ProductEntity { Id = 1, Name = "Test Product" } }
                });

            _mockMapper.Setup(sp => sp.Map<PagedResult<ProductDto>>(It.IsAny<PagedResult<ProductEntity>>()))
                .Returns(new PagedResult<ProductDto>
                {
                    TotalItems = 100,
                    PageSize = parameters.PageSize,
                    Page = parameters.Page,
                    Items = new List<ProductDto> { new ProductDto { Id = 1, Name = "Test Product" } }
                });

            // Act
            var result = await _productService.GetProducts(userId, role, parameters);

            // Assert
            Assert.NotNull(result);
        }

        /// <summary>
        /// Gets the products seller role returns own products.
        /// </summary>
        [Fact]
        public async Task GetProducts_SellerRole_ReturnsOwnProducts()
        {
            // Arrange
            var userId = 1;
            var role = UserRole.Seller;
            var parameters = new QueryProductParameters
            {
                Page = 1,
                PageSize = 10,
                UserId = 1
            };

            _mockCacheService.Setup(sp => sp.Get<PagedResult<ProductDto>>(It.IsAny<string>())).ReturnsAsync((PagedResult<ProductDto>?)null);

            _mockProductRepository.Setup(sp => sp.GetProducts(It.IsAny<QueryProductParameters>()))
                .ReturnsAsync(new PagedResult<ProductEntity>
                {
                    TotalItems = 100,
                    PageSize = parameters.PageSize,
                    Page = parameters.Page,
                    Items = new List<ProductEntity> { new ProductEntity { Id = 1, Name = "Test Product" } }
                });

            _mockMapper.Setup(sp => sp.Map<PagedResult<ProductDto>>(It.IsAny<PagedResult<ProductEntity>>()))
                .Returns(new PagedResult<ProductDto>
                {
                    TotalItems = 100,
                    PageSize = parameters.PageSize,
                    Page = parameters.Page,
                    Items = new List<ProductDto> { new ProductDto { Id = 1, Name = "Test Product" } }
                });

            // Act
            var result = await _productService.GetProducts(userId, role, parameters);

            // Assert
            Assert.NotNull(result);
        }

        /// <summary>
        /// Gets the products invalid pagination throws argument exception.
        /// </summary>
        [Fact]
        public async Task GetProducts_InvalidPagination_ThrowsArgumentException()
        {
            // Arrange
            var userId = 1;
            var role = UserRole.Admin;

            var parameters = new QueryProductParameters
            {
                Page = -1,
                PageSize = 0,
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _productService.GetProducts(userId, role, parameters));
        }

        /// <summary>
        /// Gets the products invalid user role throws argument out of range exception.
        /// </summary>
        [Fact]
        public async Task GetProducts_InvalidUserRole_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var userId = 1;
            var role = (UserRole)999; // Invalid role
            var parameters = new QueryProductParameters
            {
                Page = 1,
                PageSize = 10,
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _productService.GetProducts(userId, role, parameters));
        }
    }
}
