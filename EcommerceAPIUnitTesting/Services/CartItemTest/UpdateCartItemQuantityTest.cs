using AutoFixture;
using AutoMapper;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Repositories;
using EcommerceAPI.Services.CartItem.Interfaces;
using EcommerceAPI.Services.CartItem;
using EcommerceAPI.Services.Infrastructure.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcommerceAPI.Models.DTOs.Cart;
using EcommerceAPI.Models.DTOs.CartItem;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Models;

namespace EcommerceAPIUnitTesting.Services.CartItemTest
{
    /// <summary>
    /// Unit tests for updating the quantity of a cart item in the shopping cart service.
    /// </summary>
    public class UpdateCartItemQuantityTest
    {
        private readonly Mock<ICartRepository> _mockCartRepository;
        private readonly Mock<ICartItemRepository> _mockCartItemRepository;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ICartItemService _cartItemService;
        private readonly Fixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCartItemQuantityTest"/> class.
        /// </summary>
        public UpdateCartItemQuantityTest()
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
        /// Updates the cart item quantity quantity less than or equal zero throws argument exception.
        /// </summary>
        [Fact]
        public async Task UpdateCartItemQuantity_QuantityLessThanOrEqualZero_ThrowsArgumentException()
        {
            // Arrange
            var userId = 1;
            var cartItemUpdateDto = CreateCartItemUpdateDto(quantity: 0);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _cartItemService.UpdateCartItemQuantity(userId, cartItemUpdateDto));
            _mockCartRepository.Verify(repo => repo.GetCartByUserId(userId), Times.Never);
            _mockProductRepository.Verify(repo => repo.GetProductById(It.IsAny<int>()), Times.Never);
            _mockCartItemRepository.Verify(repo => repo.GetCartItem(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            _mockCartItemRepository.Verify(repo => repo.UpdateCartItem(It.IsAny<CartItemEntity>()), Times.Never);
            _mockCacheService.Verify(cache => cache.Remove(It.IsAny<string>()), Times.Never);
            _mockMapper.Verify(mapper => mapper.Map<CartItemDto>(It.IsAny<CartItemEntity>()), Times.Never);
        }

        /// <summary>
        /// Updates the cart item quantity cart not found throws key not found exception.
        /// </summary>
        [Fact]
        public async Task UpdateCartItemQuantity_CartNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var userId = 1;
            var cartItemUpdateDto = CreateCartItemUpdateDto();

            _mockCartRepository.Setup(repo => repo.GetCartByUserId(userId))
                .ReturnsAsync((CartEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _cartItemService.UpdateCartItemQuantity(userId, cartItemUpdateDto));
            _mockCartRepository.Verify(repo => repo.GetCartByUserId(userId), Times.Once);
            _mockProductRepository.Verify(repo => repo.GetProductById(It.IsAny<int>()), Times.Never);
            _mockCartItemRepository.Verify(repo => repo.GetCartItem(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            _mockCartItemRepository.Verify(repo => repo.UpdateCartItem(It.IsAny<CartItemEntity>()), Times.Never);
            _mockCacheService.Verify(cache => cache.Remove(It.IsAny<string>()), Times.Never);
            _mockMapper.Verify(mapper => mapper.Map<CartItemDto>(It.IsAny<CartItemEntity>()), Times.Never);
        }

        /// <summary>
        /// Updates the cart item quantity product not found throws key not found exception.
        /// </summary>
        [Fact]
        public async Task UpdateCartItemQuantity_ProductNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var userId = 1;
            var cartItemUpdateDto = CreateCartItemUpdateDto();
            var cart = CreateCart(userId);

            _mockCartRepository.Setup(repo => repo.GetCartByUserId(userId))
                .ReturnsAsync(cart);

            _mockProductRepository.Setup(repo => repo.GetProductById(cartItemUpdateDto.ProductId))
                .ReturnsAsync((ProductEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _cartItemService.UpdateCartItemQuantity(userId, cartItemUpdateDto));
            _mockCartRepository.Verify(repo => repo.GetCartByUserId(userId), Times.Once);
            _mockProductRepository.Verify(repo => repo.GetProductById(cartItemUpdateDto.ProductId), Times.Once);
            _mockCartItemRepository.Verify(repo => repo.GetCartItem(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            _mockCartItemRepository.Verify(repo => repo.UpdateCartItem(It.IsAny<CartItemEntity>()), Times.Never);
            _mockCacheService.Verify(cache => cache.Remove(It.IsAny<string>()), Times.Never);
            _mockMapper.Verify(mapper => mapper.Map<CartItemDto>(It.IsAny<CartItemEntity>()), Times.Never);
        }

        /// <summary>
        /// Updates the cart item quantity product inactive throws invalid operation exception.
        /// </summary>
        [Fact]
        public async Task UpdateCartItemQuantity_ProductInactive_ThrowsInvalidOperationException()
        {
            // Arrange
            var userId = 1;
            var cartItemUpdateDto = CreateCartItemUpdateDto();
            var cart = CreateCart(userId);
            var product = CreateProduct(cartItemUpdateDto.ProductId, isActive: false);

            _mockCartRepository.Setup(repo => repo.GetCartByUserId(userId))
                .ReturnsAsync(cart);

            _mockProductRepository.Setup(repo => repo.GetProductById(cartItemUpdateDto.ProductId))
                .ReturnsAsync(product);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _cartItemService.UpdateCartItemQuantity(userId, cartItemUpdateDto));
            _mockCartRepository.Verify(repo => repo.GetCartByUserId(userId), Times.Once);
            _mockProductRepository.Verify(repo => repo.GetProductById(cartItemUpdateDto.ProductId), Times.Once);
            _mockCartItemRepository.Verify(repo => repo.GetCartItem(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            _mockCartItemRepository.Verify(repo => repo.UpdateCartItem(It.IsAny<CartItemEntity>()), Times.Never);
            _mockCacheService.Verify(cache => cache.Remove(It.IsAny<string>()), Times.Never);
            _mockMapper.Verify(mapper => mapper.Map<CartItemDto>(It.IsAny<CartItemEntity>()), Times.Never);
        }

        /// <summary>
        /// Updates the cart item quantity cart item not found throws key not found exception.
        /// </summary>
        [Fact]
        public async Task UpdateCartItemQuantity_CartItemNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var userId = 1;
            var cartItemUpdateDto = CreateCartItemUpdateDto();
            var cart = CreateCart(userId);
            var product = CreateProduct(cartItemUpdateDto.ProductId);

            _mockCartRepository.Setup(repo => repo.GetCartByUserId(userId))
                .ReturnsAsync(cart);

            _mockProductRepository.Setup(repo => repo.GetProductById(cartItemUpdateDto.ProductId))
                .ReturnsAsync(product);

            _mockCartItemRepository.Setup(repo => repo.GetCartItem(cart.Id, product.Id))
                .ReturnsAsync((CartItemEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _cartItemService.UpdateCartItemQuantity(userId, cartItemUpdateDto));
            _mockCartRepository.Verify(repo => repo.GetCartByUserId(userId), Times.Once);
            _mockProductRepository.Verify(repo => repo.GetProductById(cartItemUpdateDto.ProductId), Times.Once);
            _mockCartItemRepository.Verify(repo => repo.GetCartItem(cart.Id, product.Id), Times.Once);
            _mockCartItemRepository.Verify(repo => repo.UpdateCartItem(It.IsAny<CartItemEntity>()), Times.Never);
            _mockCacheService.Verify(cache => cache.Remove(It.IsAny<string>()), Times.Never);
            _mockMapper.Verify(mapper => mapper.Map<CartItemDto>(It.IsAny<CartItemEntity>()), Times.Never);
        }

        /// <summary>
        /// Updates the cart item quantity stock insufficient throws invalid operation exception.
        /// </summary>
        [Fact]
        public async Task UpdateCartItemQuantity_StockInsufficient_ThrowsInvalidOperationException()
        {
            // Arrange
            var userId = 1;
            var cartItemUpdateDto = CreateCartItemUpdateDto(quantity: 5);
            var cart = CreateCart(userId);
            var product = CreateProduct(cartItemUpdateDto.ProductId, stock: 3);
            var existingCartItem = CreateCartItemEntity(cart.Id, product.Id, 2, product.Price);

            _mockCartRepository.Setup(repo => repo.GetCartByUserId(userId))
                .ReturnsAsync(cart);

            _mockProductRepository.Setup(repo => repo.GetProductById(cartItemUpdateDto.ProductId))
                .ReturnsAsync(product);

            _mockCartItemRepository.Setup(repo => repo.GetCartItem(cart.Id, product.Id))
                .ReturnsAsync(existingCartItem);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _cartItemService.UpdateCartItemQuantity(userId, cartItemUpdateDto));
            _mockCartRepository.Verify(repo => repo.GetCartByUserId(userId), Times.Once);
            _mockProductRepository.Verify(repo => repo.GetProductById(cartItemUpdateDto.ProductId), Times.Once);
            _mockCartItemRepository.Verify(repo => repo.GetCartItem(cart.Id, product.Id), Times.Once);
            _mockCartItemRepository.Verify(repo => repo.UpdateCartItem(It.IsAny<CartItemEntity>()), Times.Never);
            _mockCacheService.Verify(cache => cache.Remove(It.IsAny<string>()), Times.Never);
            _mockMapper.Verify(mapper => mapper.Map<CartItemDto>(It.IsAny<CartItemEntity>()), Times.Never);
        }

        /// <summary>
        /// Updates the cart item quantity valid request updates cart item and refreshes cache.
        /// </summary>
        [Fact]
        public async Task UpdateCartItemQuantity_ValidRequest_UpdatesCartItemAndRefreshesCache()
        {
            // Arrange
            var userId = 1;
            var cartItemUpdateDto = CreateCartItemUpdateDto(quantity: 3);
            var cart = CreateCart(userId);
            var product = CreateProduct(cartItemUpdateDto.ProductId, stock: 10, price: 50.00m);
            var cartItemEntity = CreateCartItemEntity(cart.Id, product.Id, cartItemUpdateDto.Quantity, product.Price);
            var cartItemDto = CreateCartItemDto(cart.Id, product.Id, cartItemUpdateDto.Quantity, product.Price);

            _mockCartRepository.Setup(repo => repo.GetCartByUserId(userId))
                .ReturnsAsync(cart);

            _mockProductRepository.Setup(repo => repo.GetProductById(cartItemUpdateDto.ProductId))
                .ReturnsAsync(product);

            _mockCartItemRepository.Setup(repo => repo.GetCartItem(cart.Id, product.Id))
                .ReturnsAsync(cartItemEntity);

            _mockCartItemRepository.Setup(repo => repo.UpdateCartItem(cartItemEntity))
                .ReturnsAsync(cartItemEntity);

            _mockMapper.Setup(mapper => mapper.Map<CartItemDto>(cartItemEntity))
                .Returns(cartItemDto);

            // Act
            var result = await _cartItemService.UpdateCartItemQuantity(userId, cartItemUpdateDto);

            // Assert
            Assert.NotNull(result);
            _mockCartRepository.Verify(repo => repo.GetCartByUserId(userId), Times.Once);
            _mockProductRepository.Verify(repo => repo.GetProductById(cartItemUpdateDto.ProductId), Times.Once);
            _mockCartItemRepository.Verify(repo => repo.GetCartItem(cart.Id, product.Id), Times.Once);
            _mockCartItemRepository.Verify(repo => repo.UpdateCartItem(It.IsAny<CartItemEntity>()), Times.Once);
            _mockCacheService.Verify(cache => cache.Remove(It.IsAny<string>()), Times.Exactly(2));
            _mockMapper.Verify(mapper => mapper.Map<CartItemDto>(It.IsAny<CartItemEntity>()), Times.Once);
        }

        private CartItemUpdateDto CreateCartItemUpdateDto(int quantity = 1)
        {
            return _fixture.Build<CartItemUpdateDto>()
                           .With(c => c.Quantity, quantity)
                           .Create();
        }

        private CartEntity CreateCart(int userId)
        {
            return _fixture.Build<CartEntity>()
                           .With(c => c.UserId, userId)
                           .With(c => c.CartItems, new List<CartItemEntity>())
                           .Create();
        }

        private ProductEntity CreateProduct(int productId, bool isActive = true, int stock = 10, decimal price = 100.00m)
        {
            return _fixture.Build<ProductEntity>()
                           .With(p => p.Id, productId)
                           .With(p => p.IsActive, isActive)
                           .With(p => p.Stock, stock)
                           .With(p => p.Price, price)
                           .Without(p => p.Category)
                           .Without(p => p.ProductTags)
                           .Without(p => p.User)
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

        private CartItemDto CreateCartItemDto(int cartId, int productId, int quantity, decimal price)
        {
            return _fixture.Build<CartItemDto>()
                           .With(c => c.CartId, cartId)
                           .With(c => c.ProductId, productId)
                           .With(c => c.Quantity, quantity)
                           .With(c => c.UnitPrice, price)
                           .Without(c => c.Product)
                           .Create();
        }
    }
}