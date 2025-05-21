using AutoFixture;
using AutoMapper;
using EcommerceAPI.Constants;
using EcommerceAPI.Models;
using EcommerceAPI.Models.DTOs;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Repositories;
using EcommerceAPI.Services.ElasticService.Interfaces;
using EcommerceAPI.Services.Infrastructure.Interfaces;
using EcommerceAPI.Services.Product;
using Moq;

namespace EcommerceAPIUnitTesting.Services.ProductServiceTesting
{
    /// <summary>
    /// Unit tests for the SearchProducts method in the ProductService class.
    /// </summary>
    public class SearchProductsTests
    {
        private readonly Mock<IElasticProductService> _mockElasticProductService;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProductService _productService;
        private readonly Fixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchProductsTests"/> class.
        /// </summary>
        public SearchProductsTests()
        {
            _mockElasticProductService = new Mock<IElasticProductService>();
            _mockProductRepository = new Mock<IProductRepository>();
            _mockMapper = new Mock<IMapper>();
            _fixture = new Fixture();

            _productService = new ProductService(
                _mockProductRepository.Object,
                _mockElasticProductService.Object,
                elasticGenericService: null!,
                cacheService: null!,
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

            var productsEntity = CreateProductsEntity();
            var productsDto = CreateProductsDto();

            _mockElasticProductService.Setup(sp => sp.SearchProducts(It.IsAny<SearchProductParameters>()))
                .ReturnsAsync(new PagedResult<int>
                {
                    TotalItems = 100,
                    PageSize = parameters.PageSize,
                    Page = parameters.Page,
                    Items = new List<int> { 1, 2, 3 }
                });

            _mockProductRepository.Setup(sp => sp.GetProductsByIds(It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(productsEntity);

            _mockMapper.Setup(sp => sp.Map<IEnumerable<ProductDto>>(It.IsAny<IEnumerable<ProductEntity>>()))
                .Returns(productsDto);

            // Act
            var result = await _productService.SearchProducts(role, parameters);

            // Assert
            Assert.NotNull(result);
            _mockElasticProductService.Verify(sp => sp.SearchProducts(It.IsAny<SearchProductParameters>()), Times.Once);
            _mockProductRepository.Verify(sp => sp.GetProductsByIds(It.IsAny<IEnumerable<int>>()), Times.Once);
            _mockMapper.Verify(sp => sp.Map<IEnumerable<ProductDto>>(It.IsAny<IEnumerable<ProductEntity>>()), Times.Once);
        }

        private IEnumerable<ProductEntity> CreateProductsEntity()
        {
            return _fixture.Build<ProductEntity>()
                .Without(p => p.Category)
                .Without(p => p.ProductTags)
                .CreateMany(5);
        }

        private IEnumerable<ProductDto> CreateProductsDto()
        {
            return _fixture.Build<ProductDto>()
                .Without(p => p.Category)
                .Without(p => p.ProductTags)
                .CreateMany(5);
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
            _mockElasticProductService.Verify(sp => sp.SearchProducts(parameters), Times.Once);
            _mockProductRepository.Verify(sp => sp.GetProductsByIds(It.IsAny<IEnumerable<int>>()), Times.Never);
            _mockMapper.Verify(sp => sp.Map<IEnumerable<ProductDto>>(It.IsAny<IEnumerable<ProductEntity>>()), Times.Never);
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
            _mockElasticProductService.Verify(sp => sp.SearchProducts(It.IsAny<SearchProductParameters>()), Times.Never);
            _mockProductRepository.Verify(sp => sp.GetProductsByIds(It.IsAny<IEnumerable<int>>()), Times.Never);
            _mockMapper.Verify(sp => sp.Map<IEnumerable<ProductDto>>(It.IsAny<IEnumerable<ProductEntity>>()), Times.Never);
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
            _mockElasticProductService.Verify(sp => sp.SearchProducts(It.IsAny<SearchProductParameters>()), Times.Never);
            _mockProductRepository.Verify(sp => sp.GetProductsByIds(It.IsAny<IEnumerable<int>>()), Times.Never);
            _mockMapper.Verify(sp => sp.Map<IEnumerable<ProductDto>>(It.IsAny<IEnumerable<ProductEntity>>()), Times.Never);
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
            _mockElasticProductService.Verify(sp => sp.SearchProducts(It.IsAny<SearchProductParameters>()), Times.Never);
            _mockProductRepository.Verify(sp => sp.GetProductsByIds(It.IsAny<IEnumerable<int>>()), Times.Never);
            _mockMapper.Verify(sp => sp.Map<IEnumerable<ProductDto>>(It.IsAny<IEnumerable<ProductEntity>>()), Times.Never);
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
            _mockElasticProductService.Verify(sp => sp.SearchProducts(It.IsAny<SearchProductParameters>()), Times.Never);
            _mockProductRepository.Verify(sp => sp.GetProductsByIds(It.IsAny<IEnumerable<int>>()), Times.Never);
            _mockMapper.Verify(sp => sp.Map<IEnumerable<ProductDto>>(It.IsAny<IEnumerable<ProductEntity>>()), Times.Never);
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
            _mockElasticProductService.Verify(sp => sp.SearchProducts(It.IsAny<SearchProductParameters>()), Times.Never);
            _mockProductRepository.Verify(sp => sp.GetProductsByIds(It.IsAny<IEnumerable<int>>()), Times.Never);
            _mockMapper.Verify(sp => sp.Map<IEnumerable<ProductDto>>(It.IsAny<IEnumerable<ProductEntity>>()), Times.Never);
        }
    }
}