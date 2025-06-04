using AutoFixture;
using AutoMapper;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Repositories;
using EcommerceAPI.Services.CartItem.Interfaces;
using EcommerceAPI.Services.CartItem;
using EcommerceAPI.Services.Infrastructure.Interfaces;
using Moq;
using EcommerceAPI.Models;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPIUnitTesting.Services.CartItemTest
{
    /// <summary>
    /// Unit tests for the DeleteItemFromCart method in the CartItemService class.
    /// </summary>
    public class DeleteItemFromCartTest
    {
        private readonly Mock<ICartRepository> _mockCartRepository;
        private readonly Mock<ICartItemRepository> _mockCartItemRepository;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ICartItemService _cartItemService;
        private readonly Fixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteItemFromCartTest"/> class.
        /// </summary>
        public DeleteItemFromCartTest()
        {
            _mockCartRepository = new Mock<ICartRepository>();
            _mockCartItemRepository = new Mock<ICartItemRepository>();
            _mockProductRepository = new Mock<IProductRepository>();
            _mockCacheService = new Mock<ICacheService>();
            _mockMapper = new Mock<IMapper>();

            _fixture = new Fixture();

            _cartItemService = new CartItemService(
                _mockCartRepository.Object,
                _mockCartItemRepository.Object,
                _mockProductRepository.Object,
                _mockCacheService.Object,
                _mockMapper.Object
            );
        }

        /// <summary>
        /// Deletes the item from cart cart not found throws key not found exception.
        /// </summary>
        [Fact]
        public async Task DeleteItemFromCart_CartNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            int userId = _fixture.Create<int>();
            int productId = _fixture.Create<int>();

            _mockCartRepository.Setup(repo => repo.GetCartByUserId(userId))
                .ReturnsAsync((CartEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _cartItemService.DeleteItemFromCart(userId, productId));
            _mockCartRepository.Verify(repo => repo.GetCartByUserId(userId), Times.Once);
            _mockCartItemRepository.Verify(repo => repo.GetCartItem(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            _mockCartItemRepository.Verify(repo => repo.DeleteCartItem(It.IsAny<int>()), Times.Never);
            _mockCacheService.Verify(service => service.Remove(It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Deletes the item from cart cart item not found throws key not found exception.
        /// </summary>
        [Fact]
        public async Task DeleteItemFromCart_CartItemNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            int userId = _fixture.Create<int>();
            int productId = _fixture.Create<int>();
            var cart = CreateCart(userId);

            _mockCartRepository.Setup(repo => repo.GetCartByUserId(userId))
                .ReturnsAsync(cart);

            _mockCartItemRepository.Setup(repo => repo.GetCartItem(cart.Id, productId))
                .ReturnsAsync((CartItemEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _cartItemService.DeleteItemFromCart(userId, productId));
            _mockCartRepository.Verify(repo => repo.GetCartByUserId(userId), Times.Once);
            _mockCartItemRepository.Verify(repo => repo.GetCartItem(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            _mockCartItemRepository.Verify(repo => repo.DeleteCartItem(It.IsAny<int>()), Times.Never);
            _mockCacheService.Verify(service => service.Remove(It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Deletes the item from cart valid request deletes item and refreshes cache.
        /// </summary>
        [Fact]
        public async Task DeleteItemFromCart_ValidRequest_DeletesItemAndRefreshesCache()
        {
            // Arrange
            int userId = _fixture.Create<int>();
            int productId = _fixture.Create<int>();
            var cart = CreateCart(userId);
            var cartItem = CreateCartItemEntity(cart.Id, productId, 2, 100.00m);

            _mockCartRepository.Setup(repo => repo.GetCartByUserId(userId))
                .ReturnsAsync(cart);

            _mockCartItemRepository.Setup(repo => repo.GetCartItem(cart.Id, productId))
                .ReturnsAsync(cartItem);

            _mockCartItemRepository.Setup(repo => repo.DeleteCartItem(cartItem.Id))
                .ReturnsAsync(true);

            // Act
            var result = await _cartItemService.DeleteItemFromCart(userId, productId);

            // Assert
            Assert.True(result);
            _mockCartRepository.Verify(repo => repo.GetCartByUserId(userId), Times.Once);
            _mockCartItemRepository.Verify(repo => repo.GetCartItem(cart.Id, productId), Times.Once);
            _mockCartItemRepository.Verify(repo => repo.DeleteCartItem(cartItem.Id), Times.Once);
            _mockCacheService.Verify(service => service.Remove(It.IsAny<string>()), Times.Exactly(2));
        }

        private CartEntity CreateCart(int userId)
        {
            return _fixture.Build<CartEntity>()
                           .With(c => c.UserId, userId)
                           .With(c => c.CartItems, new List<CartItemEntity>())
                           .Create();
        }

        private CartItemEntity CreateCartItemEntity(int cartId, int productId, int quantity, decimal price)
        {
            return _fixture.Build<CartItemEntity>()
                           .With(c => c.CartId, cartId)
                           .With(c => c.ProductId, productId)
                           .With(c => c.Quantity, quantity)
                           .With(c => c.UnitPrice, price)
                           .Without(c => c.Cart)
                           .Without(c => c.Product)
                           .Create();
        }
    }
}