using AutoFixture;
using AutoMapper;
using EcommerceAPI.Models;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Services.Cart;
using EcommerceAPI.Services.Infrastructure.Interfaces;
using Moq;

namespace EcommerceAPIUnitTesting.Services.CartServiceTest
{
    /// <summary>
    /// Unit tests for the CartService's GetCartTotal method.
    /// </summary>
    public class GetCartTotalTest
    {
        private readonly Mock<ICartRepository> _mockCartRepository;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CartService _cartService;
        private readonly Fixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetCartTotalTest"/> class.
        /// </summary>
        public GetCartTotalTest()
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
        /// Gets the cart total returns total from cache when cache exists.
        /// </summary>
        [Fact]
        public async Task GetCartTotal_ReturnsTotalFromCache_WhenCacheExists()
        {
            // Arrange
            int userId = 1;
            decimal cachedTotal = 100.00m;

            _mockCacheService.Setup(sp => sp.Get<decimal?>(It.IsAny<string>()))
                .ReturnsAsync(cachedTotal);

            // Act
            var result = await _cartService.GetCartTotal(userId);

            // Assert
            Assert.Equal(cachedTotal, result);
            _mockCacheService.Verify(sp => sp.Get<decimal?>(It.IsAny<string>()), Times.Once);
            _mockCartRepository.Verify(sp => sp.GetCartByUserId(It.IsAny<int>()), Times.Never);
            _mockCartRepository.Verify(sp => sp.GetCartTotal(It.IsAny<int>()), Times.Never);
            _mockCacheService.Verify(sp => sp.Set(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<TimeSpan>()), Times.Never);
        }

        /// <summary>
        /// Gets the cart total throws key not found exception when cart does not exist.
        /// </summary>
        [Fact]
        public async Task GetCartTotal_ThrowsKeyNotFoundException_WhenCartDoesNotExist()
        {
            // Arrange
            int userId = 1;

            _mockCacheService.Setup(sp => sp.Get<decimal?>(It.IsAny<string>()))
                .ReturnsAsync((decimal?)null);

            _mockCartRepository.Setup(sp => sp.GetCartByUserId(userId))
                .ReturnsAsync((CartEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _cartService.GetCartTotal(userId));
            _mockCacheService.Verify(sp => sp.Get<decimal?>(It.IsAny<string>()), Times.Once);
            _mockCartRepository.Verify(sp => sp.GetCartByUserId(userId), Times.Once);
            _mockCartRepository.Verify(sp => sp.GetCartTotal(It.IsAny<int>()), Times.Never);
            _mockCacheService.Verify(sp => sp.Set(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<TimeSpan>()), Times.Never);
        }

        /// <summary>
        /// Gets the cart total returns total from database when not in cache.
        /// </summary>
        [Fact]
        public async Task GetCartTotal_ReturnsTotalFromDatabase_WhenNotInCache()
        {
            // Arrange
            int userId = 1;
            var cart = CreateEntity();
            decimal totalFromDb = 150.00m;

            _mockCacheService.Setup(sp => sp.Get<decimal?>(It.IsAny<string>()))
                .ReturnsAsync((decimal?)null);

            _mockCartRepository.Setup(sp => sp.GetCartByUserId(userId))
                .ReturnsAsync(cart);

            _mockCartRepository.Setup(sp => sp.GetCartTotal(cart.Id))
                .ReturnsAsync(totalFromDb);

            // Act
            var result = await _cartService.GetCartTotal(userId);

            // Assert
            Assert.Equal(totalFromDb, result);
            _mockCacheService.Verify(sp => sp.Get<decimal?>(It.IsAny<string>()), Times.Once);
            _mockCartRepository.Verify(sp => sp.GetCartByUserId(userId), Times.Once);
            _mockCartRepository.Verify(sp => sp.GetCartTotal(cart.Id), Times.Once);
            _mockCacheService.Verify(sp => sp.Set(It.IsAny<string>(), totalFromDb, It.IsAny<TimeSpan>()), Times.Once);
        }

        /// <summary>
        /// Gets the cart total sets cache when total is retrieved from database.
        /// </summary>
        [Fact]
        public async Task GetCartTotal_SetsCache_WhenTotalIsRetrievedFromDatabase()
        {
            // Arrange
            int userId = 1;
            var cart = CreateEntity();
            decimal totalFromDb = 200.00m;

            _mockCacheService.Setup(sp => sp.Get<decimal?>(It.IsAny<string>()))
                .ReturnsAsync((decimal?)null);

            _mockCartRepository.Setup(sp => sp.GetCartByUserId(userId))
                .ReturnsAsync(cart);

            _mockCartRepository.Setup(sp => sp.GetCartTotal(cart.Id))
                .ReturnsAsync(totalFromDb);

            // Act
            var result = await _cartService.GetCartTotal(userId);

            // Assert
            Assert.Equal(totalFromDb, result);
            _mockCacheService.Verify(sp => sp.Get<decimal?>(It.IsAny<string>()), Times.Once);
            _mockCartRepository.Verify(sp => sp.GetCartByUserId(userId), Times.Once);
            _mockCartRepository.Verify(sp => sp.GetCartTotal(cart.Id), Times.Once);
            _mockCacheService.Verify(sp => sp.Set(It.IsAny<string>(), totalFromDb, It.IsAny<TimeSpan>()), Times.Once);
        }

        private CartEntity CreateEntity()
        {
            return _fixture.Build<CartEntity>()
                .Without(cart => cart.CartItems)
                .Create();
        }
    }
}