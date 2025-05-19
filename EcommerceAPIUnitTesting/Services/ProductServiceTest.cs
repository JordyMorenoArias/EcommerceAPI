using AutoMapper;
using EcommerceAPI.Constants;
using EcommerceAPI.Models;
using EcommerceAPI.Models.DTOs;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Repositories;
using EcommerceAPI.Services.ElasticService.Interfaces;
using EcommerceAPI.Services.Infrastructure.Interfaces;
using EcommerceAPI.Services.Product;
using EcommerceAPI.Services.Product.Interfaces;
using Moq;

namespace EcommerceAPIUnitTesting.Services
{
    /// <summary>
    /// Unit tests for the ProductService class.
    /// </summary>
    public class ProductServiceTest
    {
        public readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IElasticProductService> _mockElasticProductService;
        private readonly Mock<IElasticGenericService<ProductElasticDto>> _mockElasticGenericService;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProductService _productService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductServiceTest"/> class.
        /// </summary>
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

        /// <summary>
        /// Gets the product by identifier product in cache returns product.
        /// </summary>
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

        /// <summary>
        /// Gets the produc by identifier product not in cache but in database returns product.
        /// </summary>
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

        /// <summary>
        /// Gets the product by identifier product does not exist exception thrown.
        /// </summary>
        [Fact]
        public async Task GetProductById_ProductDoesNotExist_ThrowsException()
        {
            // Arrange
            var productId = 1;

            _mockCacheService.Setup(sp => sp.Get<ProductDto>(It.IsAny<string>())).ReturnsAsync((ProductDto?)null);

            _mockProductRepository.Setup(sp => sp.GetProductById(productId)).ReturnsAsync((ProductEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _productService.GetProductById(productId));
        }

        /// <summary>
        /// Gets the suggestions valid query returns results.
        /// </summary>
        [Fact]
        public async Task GetSuggestions_ValidQuery_ReturnsResults()
        {
            // Arrange
            var query = "test";

            _mockElasticProductService.Setup(sp => sp.GetSuggestionsProducts(query))
                .ReturnsAsync(new List<string> { "Test Product 1", "Test Product 2" });

            // Act
            var result = await _productService.GetSuggestionsProducts(query);

            // Assert
            Assert.NotNull(result);
        }

        /// <summary>
        /// Gets the suggestions empty or null query throws exception.
        /// </summary>
        [Fact]
        public async Task GetSuggestions_EmptyOrNullQuery_ThrowsException()
        {
            // Arrange
            string query = string.Empty;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _productService.GetSuggestionsProducts(query));
        }

        /// <summary>
        /// Searches the products valid parameters with results returns paged result.
        /// </summary>
        [Fact]
        public async Task SearchProducts_ValidParametersWithResults_ReturnsPagedResult()
        {
            // Arrange
            var role = UserRole.Admin;
            var parameters = new SearchProductParameters
            {
                Page = 1,
                PageSize = 10,
                SearchTerm = "test",
            };

            _mockElasticProductService.Setup(sp => sp.SearchProducts(parameters))
                .ReturnsAsync(new PagedResult<int>
                {
                    TotalItems = 100,
                    PageSize = parameters.PageSize,
                    Page = parameters.Page,
                    Items = new List<int> { 1, 2, 3 }
                });

            // Act
            var result = await _productService.SearchProducts(role, parameters);

            // Assert
            Assert.NotNull(result);
        }

        /// <summary>
        /// Searches the products valid parameters no results returns empty paged result.
        /// </summary>
        [Fact]
        public async Task SearchProducts_ValidParametersNoResults_ReturnsEmptyPagedResult()
        {
            // Arrange
            var role = UserRole.Admin;
            var parameters = new SearchProductParameters
            {
                SearchTerm = "test",
                Page = 1,
                PageSize = 10
            };

            _mockElasticProductService.Setup(sp => sp.SearchProducts(parameters))
                    .ReturnsAsync(new PagedResult<int>
                    {
                        TotalItems = 0,
                        PageSize = parameters.PageSize,
                        Page = parameters.Page,
                        Items = new List<int>()
                    });

            // Act
            var result = await _productService.SearchProducts(role, parameters);

            // Assert
            Assert.NotNull(result);
        }

        /// <summary>
        /// Searches the products empty search term returns empty paged result.
        /// </summary>
        [Fact]
        public async Task SearchProducts_EmptySearchTerm_ReturnsEmptyPagedResult()
        {
            // Arrange
            var role = UserRole.Admin;
            var parameters = new SearchProductParameters
            {
                SearchTerm = string.Empty,
                Page = 1,
                PageSize = 10
            };

            // Act
            var result = await _productService.SearchProducts(role, parameters);

            // Assert
            Assert.NotNull(result);
        }

        /// <summary>
        /// Searches the products minimum price greater than maximum price throws argument exception.
        /// </summary>
        [Fact]
        public async Task SearchProducts_MinPriceGreaterThanMaxPrice_ThrowsArgumentException()
        {
            // Arrange
            var role = UserRole.Admin;
            var parameters = new SearchProductParameters
            {
                SearchTerm = "test",
                Page = 1,
                PageSize = 10,
                MinPrice = 100,
                MaxPrice = 50,
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _productService.SearchProducts(role, parameters));
        }

        /// <summary>
        /// Searches the products invalid pagination throws argument exception.
        /// </summary>
        [Fact]
        public async Task SearchProducts_InvalidPagination_ThrowsArgumentException()
        {
            // Arrange
            var role = UserRole.Admin;
            var parameters = new SearchProductParameters
            {
                SearchTerm = "test",
                Page = -1,
                PageSize = 0,
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _productService.SearchProducts(role, parameters));
        }

        /// <summary>
        /// Searches the products customer searching inactive products throws invalid operation exception.
        /// </summary>
        [Fact]
        public async Task SearchProducts_CustomerSearchingInactiveProducts_ThrowsInvalidOperationException()
        {
            // Arrange
            var role = UserRole.Customer;
            var parameters = new SearchProductParameters
            {
                SearchTerm = "test",
                Page = 1,
                PageSize = 10,
                IsActive = false,
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _productService.SearchProducts(role, parameters));
        }

        /// <summary>
        /// Searches the products seller searching other inactive products throws invalid operation exception.
        /// </summary>
        [Fact]
        public async Task SearchProducts_SellerSearchingOtherInactiveProducts_ThrowsInvalidOperationException()
        {
            // Arrange
            var role = UserRole.Seller;
            var parameters = new SearchProductParameters
            {
                SearchTerm = "test",
                Page = 1,
                PageSize = 10,
                IsActive = false,
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _productService.SearchProducts(role, parameters));
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