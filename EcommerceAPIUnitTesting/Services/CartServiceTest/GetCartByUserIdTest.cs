using AutoFixture;
using AutoMapper;
using EcommerceAPI.Models;
using EcommerceAPI.Models.DTOs.Cart;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Services.Cart;
using EcommerceAPI.Services.Infrastructure.Interfaces;
using Moq;

namespace EcommerceAPIUnitTesting.Services.CartServiceTest
{
    /// <summary>
    /// Unit tests for the CartService's GetCartByUserId method.
    /// </summary>
    public class GetCartByUserIdTest
    {
        private readonly Mock<ICartRepository> _mockCartRepository;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CartService _cartService;
        private readonly Fixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetCartByUserIdTest"/> class.
        /// </summary>
        public GetCartByUserIdTest()
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
        /// Gets the cart by user identifier returns cart from cache when cache exists.
        /// </summary>
        [Fact]
        public async Task GetCartByUserId_ReturnsCartFromCache_WhenCacheExists()
        {
            // Arrange
            int userId = 1;

            var cachedCart = CreateCartDto();

            _mockCacheService.Setup(sp => sp.Get<CartDto>(It.IsAny<string>()))
                .ReturnsAsync(cachedCart);

            // Act
            var result = await _cartService.GetCartByUserId(userId);

            // Assert
            Assert.NotNull(result);
            _mockCacheService.Verify(sp => sp.Get<CartDto>(It.IsAny<string>()), Times.Once);
            _mockCartRepository.Verify(sp => sp.GetCartByUserId(It.IsAny<int>()), Times.Never);
            _mockCartRepository.Verify(sp => sp.CreateCart(It.IsAny<int>()), Times.Never);
            _mockCartRepository.Verify(sp => sp.GetCartTotal(It.IsAny<int>()), Times.Never);
            _mockMapper.Verify(sp => sp.Map<CartDto>(It.IsAny<object>()), Times.Never);
            _mockCacheService.Verify(sp => sp.Set(It.IsAny<string>(), It.IsAny<CartDto>(), null), Times.Never);
        }

        /// <summary>
        /// Gets the cart by user identifier creates new cart when cart not exists.
        /// </summary>
        [Fact]
        public async Task GetCartByUserId_CreatesNewCart_WhenCartNotExists()
        {
            // Arrange
            int userId = 1;
            var cartEntity = CreateEntity();
            var cartDto = CreateCartDto();

            _mockCacheService.Setup(sp => sp.Get<CartDto>(It.IsAny<string>()))
                .ReturnsAsync((CartDto?)null);

            _mockCartRepository.Setup(sp => sp.GetCartByUserId(userId))
                .ReturnsAsync((CartEntity?)null);

            _mockCartRepository.Setup(sp => sp.CreateCart(userId))
                .ReturnsAsync(cartEntity);

            _mockCartRepository.Setup(sp => sp.GetCartTotal(cartEntity.Id))
                .ReturnsAsync(100.00m);

            _mockMapper.Setup(sp => sp.Map<CartDto>(cartEntity))
                .Returns(cartDto);

            // Act
            var result = await _cartService.GetCartByUserId(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(100.00m, result.Total);
            _mockCacheService.Verify(sp => sp.Get<CartDto>(It.IsAny<string>()), Times.Once);
            _mockCartRepository.Verify(sp => sp.GetCartByUserId(userId), Times.Once);
            _mockCartRepository.Verify(sp => sp.CreateCart(userId), Times.Once);
            _mockCartRepository.Verify(sp => sp.GetCartTotal(cartEntity.Id), Times.Once);
            _mockMapper.Verify(sp => sp.Map<CartDto>(cartEntity), Times.Once);
            _mockCacheService.Verify(sp => sp.Set(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>()), Times.Exactly(2));
        }
        /// <summary>
        /// Gets the cart by user identifier returns mapped cart with total when cart is retrieved from database.
        /// </summary>
        [Fact]
        public async Task GetCartByUserId_ReturnsMappedCartWithTotal_WhenCartIsRetrievedFromDatabase()
        {
            // Arrange
            int userId = 1;
            var cartEntity = CreateEntity();
            var cartDto = CreateCartDto();

            _mockCacheService.Setup(sp => sp.Get<CartDto>(It.IsAny<string>()))
                .ReturnsAsync((CartDto?)null);

            _mockCartRepository.Setup(sp => sp.GetCartByUserId(userId))
                .ReturnsAsync(cartEntity);

            _mockCartRepository.Setup(sp => sp.GetCartTotal(cartEntity.Id))
                .ReturnsAsync(100.00m);

            _mockMapper.Setup(sp => sp.Map<CartDto>(cartEntity))
                .Returns(cartDto);

            // Act
            var result = await _cartService.GetCartByUserId(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(100.00m, result.Total);
            _mockCacheService.Verify(sp => sp.Get<CartDto>(It.IsAny<string>()), Times.Once);
            _mockCartRepository.Verify(sp => sp.GetCartByUserId(userId), Times.Once);
            _mockCartRepository.Verify(sp => sp.GetCartTotal(cartEntity.Id), Times.Once);
            _mockMapper.Verify(sp => sp.Map<CartDto>(cartEntity), Times.Once);
            _mockCacheService.Verify(sp => sp.Set(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>()), Times.Exactly(2));
        }

        private CartDto CreateCartDto()
        {
            return _fixture.Build<CartDto>()
                .Without(cart => cart.CartItems)
                .Create();
        }

        private CartEntity CreateEntity()
        {
            return _fixture.Build<CartEntity>()
                .Without(cart => cart.CartItems)
                .Create();
        }
    }
}