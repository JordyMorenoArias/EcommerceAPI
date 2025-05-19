using AutoMapper;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Repositories;
using EcommerceAPI.Services.ElasticService.Interfaces;
using EcommerceAPI.Services.Infrastructure.Interfaces;
using EcommerceAPI.Services.Product;
using Moq;

namespace EcommerceAPIUnitTesting.Services.ProductServiceTests
{
    public class GetSuggestionsTests
    {
        public readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IElasticProductService> _mockElasticProductService;
        private readonly Mock<IElasticGenericService<ProductElasticDto>> _mockElasticGenericService;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProductService _productService;

        public GetSuggestionsTests()
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
    }
}
