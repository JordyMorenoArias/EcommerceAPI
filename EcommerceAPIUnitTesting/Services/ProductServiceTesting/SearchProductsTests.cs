using AutoMapper;
using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Repositories;
using EcommerceAPI.Services.ElasticService.Interfaces;
using EcommerceAPI.Services.Infrastructure.Interfaces;
using EcommerceAPI.Services.Product;
using Moq;

namespace EcommerceAPIUnitTesting.Services.ProductServiceTests
{
    public class SearchProductsTests
    {
        public readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IElasticProductService> _mockElasticProductService;
        private readonly Mock<IElasticGenericService<ProductElasticDto>> _mockElasticGenericService;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProductService _productService;

        public SearchProductsTests()
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
    }
}
