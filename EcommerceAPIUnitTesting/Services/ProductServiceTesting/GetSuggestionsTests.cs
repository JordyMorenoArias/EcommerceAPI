using AutoMapper;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Repositories;
using EcommerceAPI.Services.ElasticService.Interfaces;
using EcommerceAPI.Services.Infrastructure.Interfaces;
using EcommerceAPI.Services.Product;
using Moq;

namespace EcommerceAPIUnitTesting.Services.ProductServiceTesting
{
    /// <summary>
    /// Unit tests for the GetSuggestions method in the ProductService class.
    /// </summary>
    public class GetSuggestionsTests
    {
        private readonly Mock<IElasticProductService> _mockElasticProductService;
        private readonly ProductService _productService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetSuggestionsTests"/> class.
        /// </summary>
        public GetSuggestionsTests()
        {
            _mockElasticProductService = new Mock<IElasticProductService>();

            _productService = new ProductService(
                productRepository: null!,
                _mockElasticProductService.Object,
                elasticGenericService: null!,
                cacheService: null!,
                mapper: null!
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
