using AutoFixture;
using AutoMapper;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Services.Cart;
using EcommerceAPI.Services.Infrastructure.Interfaces;
using Moq;

namespace EcommerceAPIUnitTesting.Services.CartServiceTest
{
    /// <summary>
    /// Unit tests for the CartService's ClearCart method.
    /// </summary>
    public class ClearCartTest
    {
        private readonly Mock<ICartRepository> _mockCartRepository;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CartService _cartService;
        private readonly Fixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClearCartTest"/> class.
        /// </summary>
        public ClearCartTest()
        {
            _mockCartRepository = new Mock<ICartRepository>();
            _mockCacheService = new Mock<ICacheService>();
            _mockMapper = new Mock<IMapper>();
            _fixture = new Fixture();

            _cartService = new CartService(
                _mockCartRepository.Object,
                _mockCacheService.Object,
                _mockMapper.Object
            );
        }

        /// <summary>
        /// Clears the cart calls clear cart in repository.
        /// </summary>
        [Fact]
        public async Task ClearCart_CallsClearCartInRepository()
        {
            // Arrange
            int userId = 1;

            // Act
            await _cartService.ClearCart(userId);

            // Assert
            _mockCartRepository.Verify(sp => sp.ClearCart(userId), Times.Once);
            _mockCacheService.Verify(sp => sp.Remove($"cart_{userId}"), Times.Once);
            _mockCacheService.Verify(sp => sp.Remove($"cart_total_{userId}"), Times.Once);
        }
    }
}