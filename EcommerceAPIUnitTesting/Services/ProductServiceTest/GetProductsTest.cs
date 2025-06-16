using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Models;
using AutoMapper;
using EcommerceAPI.Repositories;
using EcommerceAPI.Services.ElasticService.Interfaces;
using EcommerceAPI.Services.Infrastructure.Interfaces;
using Moq;
using EcommerceAPI.Services.Product;
using AutoFixture;
using EcommerceAPI.Models.DTOs.Generic;

namespace EcommerceAPIUnitTesting.Services.ProductServiceTest
{
    /// <summary>
    /// Unit tests for the GetProducts method in the ProductService class.
    /// </summary>
    public class GetProductsTest
    {
        public readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProductService _productService;
        private readonly Fixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetProductsTest"/> class.
        /// </summary>
        public GetProductsTest()
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

            var pagedResultProductEntity = CreatePagedResultProductEntity();
            var pagedResultProductDto = CreatePagedResultProductDto();

            _mockCacheService.Setup(sp => sp.Get<PagedResult<ProductDto>>(It.IsAny<string>())).ReturnsAsync((PagedResult<ProductDto>?)null);

            _mockProductRepository.Setup(sp => sp.GetProducts(It.IsAny<QueryProductParameters>()))
                .ReturnsAsync(pagedResultProductEntity);

            _mockMapper.Setup(sp => sp.Map<PagedResult<ProductDto>>(It.IsAny<PagedResult<ProductEntity>>()))
                .Returns(pagedResultProductDto);

            // Act
            var result = await _productService.GetProducts(userId, role, parameters);

            // Assert
            Assert.NotNull(result);
            _mockCacheService.Verify(sp => sp.Get<PagedResult<ProductDto>>(It.IsAny<string>()), Times.Once);
            _mockProductRepository.Verify(sp => sp.GetProducts(It.IsAny<QueryProductParameters>()), Times.Once);
            _mockMapper.Verify(sp => sp.Map<PagedResult<ProductDto>>(It.IsAny<PagedResult<ProductEntity>>()), Times.Once);
            _mockCacheService.Verify(sp => sp.Set(It.IsAny<string>(), It.IsAny<PagedResult<ProductDto>>(), It.IsAny<TimeSpan>()), Times.Once);
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

            var pagedResultProductDto = CreatePagedResultProductDto();

            _mockCacheService.Setup(sp => sp.Get<PagedResult<ProductDto>>(It.IsAny<string>()))
                .ReturnsAsync(pagedResultProductDto);

            // Act
            var result = await _productService.GetProducts(userId, role, parameters);

            // Assert
            Assert.NotNull(result);
            _mockCacheService.Verify(sp => sp.Get<PagedResult<ProductDto>>(It.IsAny<string>()), Times.Once);
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

            var pagedResultProductEntity = CreatePagedResultProductEntity();
            var pagedResultProductDto = CreatePagedResultProductDto();

            _mockCacheService.Setup(sp => sp.Get<PagedResult<ProductDto>>(It.IsAny<string>())).ReturnsAsync((PagedResult<ProductDto>?)null);

            _mockProductRepository.Setup(sp => sp.GetProducts(It.IsAny<QueryProductParameters>()))
                .ReturnsAsync(pagedResultProductEntity);

            _mockMapper.Setup(sp => sp.Map<PagedResult<ProductDto>>(It.IsAny<PagedResult<ProductEntity>>()))
                .Returns(pagedResultProductDto);

            // Act
            var result = await _productService.GetProducts(userId, role, parameters);

            // Assert
            Assert.NotNull(result);
            _mockCacheService.Verify(sp => sp.Get<PagedResult<ProductDto>>(It.IsAny<string>()), Times.Once);
            _mockProductRepository.Verify(sp => sp.GetProducts(It.IsAny<QueryProductParameters>()), Times.Once);
            _mockMapper.Verify(sp => sp.Map<PagedResult<ProductDto>>(It.IsAny<PagedResult<ProductEntity>>()), Times.Once);
            _mockCacheService.Verify(sp => sp.Set(It.IsAny<string>(), It.IsAny<PagedResult<ProductDto>>(), It.IsAny<TimeSpan>()), Times.Once);
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

            var pagedResultProductEntity = CreatePagedResultProductEntity();
            var pagedResultProductDto = CreatePagedResultProductDto();

            _mockCacheService.Setup(sp => sp.Get<PagedResult<ProductDto>>(It.IsAny<string>())).ReturnsAsync((PagedResult<ProductDto>?)null);

            _mockProductRepository.Setup(sp => sp.GetProducts(It.IsAny<QueryProductParameters>()))
                .ReturnsAsync(pagedResultProductEntity);

            _mockMapper.Setup(sp => sp.Map<PagedResult<ProductDto>>(It.IsAny<PagedResult<ProductEntity>>()))
                .Returns(pagedResultProductDto);

            // Act
            var result = await _productService.GetProducts(userId, role, parameters);

            // Assert
            Assert.NotNull(result);
            _mockCacheService.Verify(sp => sp.Get<PagedResult<ProductDto>>(It.IsAny<string>()), Times.Once);
            _mockProductRepository.Verify(sp => sp.GetProducts(It.IsAny<QueryProductParameters>()), Times.Once);
            _mockMapper.Verify(sp => sp.Map<PagedResult<ProductDto>>(It.IsAny<PagedResult<ProductEntity>>()), Times.Once);
            _mockCacheService.Verify(sp => sp.Set(It.IsAny<string>(), It.IsAny<PagedResult<ProductDto>>(), It.IsAny<TimeSpan>()), Times.Once);
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
            _mockCacheService.Verify(sp => sp.Get<PagedResult<ProductDto>>(It.IsAny<string>()), Times.Never);
            _mockProductRepository.Verify(sp => sp.GetProducts(It.IsAny<QueryProductParameters>()), Times.Never);
            _mockMapper.Verify(sp => sp.Map<PagedResult<ProductDto>>(It.IsAny<PagedResult<ProductEntity>>()), Times.Never);
            _mockCacheService.Verify(sp => sp.Set(It.IsAny<string>(), It.IsAny<PagedResult<ProductDto>>(), It.IsAny<TimeSpan>()), Times.Never);
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
            _mockCacheService.Verify(sp => sp.Get<PagedResult<ProductDto>>(It.IsAny<string>()), Times.Never);
            _mockProductRepository.Verify(sp => sp.GetProducts(It.IsAny<QueryProductParameters>()), Times.Never);
            _mockMapper.Verify(sp => sp.Map<PagedResult<ProductDto>>(It.IsAny<PagedResult<ProductEntity>>()), Times.Never);
            _mockCacheService.Verify(sp => sp.Set(It.IsAny<string>(), It.IsAny<PagedResult<ProductDto>>(), It.IsAny<TimeSpan>()), Times.Never);
        }

        private PagedResult<ProductEntity> CreatePagedResultProductEntity()
        {
            return _fixture.Build<PagedResult<ProductEntity>>()
                .With(p => p.TotalItems, 100)
                .With(p => p.PageSize, 10)
                .With(p => p.Page, 1)
                .With(p => p.Items, _fixture.Build<ProductEntity>()
                                            .Without(p => p.Category)
                                            .Without(p => p.ProductTags)
                                            .CreateMany(5).ToList())
                .Create();
        }

        private PagedResult<ProductDto> CreatePagedResultProductDto()
        {
            return _fixture.Build<PagedResult<ProductDto>>()
                .With(p => p.TotalItems, 100)
                .With(p => p.PageSize, 10)
                .With(p => p.Page, 1)
                .With(p => p.Items, _fixture.Build<ProductDto>()
                                            .Without(p => p.Category)
                                            .Without(p => p.ProductTags)
                                            .CreateMany(5).ToList())
                .Create();
        }
    }
}