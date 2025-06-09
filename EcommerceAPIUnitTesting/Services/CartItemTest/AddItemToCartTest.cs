using AutoFixture;
using AutoMapper;
using EcommerceAPI.Models;
using EcommerceAPI.Models.DTOs.Cart;
using EcommerceAPI.Models.DTOs.CartItem;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Repositories;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Services.CartItem;
using EcommerceAPI.Services.CartItem.Interfaces;
using EcommerceAPI.Services.Infrastructure.Interfaces;
using Moq;

namespace EcommerceAPIUnitTesting.Services.CartItemTest
{
    /// <summary>
    /// Unit tests for the AddItemToCart method in the CartItemService class.
    /// </summary>
    public class AddItemToCartTest
    {
        private readonly Mock<ICartRepository> _mockCartRepository;
        private readonly Mock<ICartItemRepository> _mockCartItemRepository;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ICartItemService _cartItemService;
        private readonly Fixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddItemToCartTest"/> class.
        /// </summary>
        public AddItemToCartTest()
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
        /// Adds the item to cart quantity less than or equal zero throws argument exception.
        /// </summary>
        [Fact]
        public async Task AddItemToCart_QuantityLessThanOrEqualZero_ThrowsArgumentException()
        {
            // Arrange
            var userId = _fixture.Create<int>();
            var cartItemAddDto = CreateCartItemAddDto(0);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _cartItemService.AddItemToCart(userId, cartItemAddDto));
            _mockCartRepository.Verify(repo => repo.GetCartByUserId(userId), Times.Never);
            _mockCartRepository.Verify(repo => repo.CreateCart(userId), Times.Never);
            _mockProductRepository.Verify(repo => repo.GetProductById(cartItemAddDto.ProductId), Times.Never);
            _mockCartItemRepository.Verify(repo => repo.GetCartItem(It.IsAny<int>(), cartItemAddDto.ProductId), Times.Never);
            _mockCartItemRepository.Verify(repo => repo.UpdateCartItem(It.IsAny<CartItemEntity>()), Times.Never);
            _mockCartItemRepository.Verify(repo => repo.CreateCartItem(It.IsAny<CartItemEntity>()), Times.Never);
            _mockCacheService.Verify(cache => cache.Remove(It.IsAny<string>()), Times.Never);
            _mockMapper.Verify(m => m.Map<CartItemDto>(It.IsAny<CartItemEntity>()), Times.Never);
        }

        /// <summary>
        /// Adds the item to cart cart does not exist creates new cart.
        /// </summary>
        [Fact]
        public async Task AddItemToCart_CartDoesNotExist_CreatesNewCart()
        {
            // Arrange
            var userId = 1;
            var cartItemDtoAdd = CreateCartItemAddDto(1);
            var cart = CreateCart(userId);
            var product = CreateProduct(cartItemDtoAdd.ProductId);
            var cartItemEntity = CreateCartItemEntity(cart.Id, cartItemDtoAdd.ProductId, cartItemDtoAdd.Quantity, product.Price);
            var cartItemDto = CreateCartItemDto(cart.Id, cartItemDtoAdd.ProductId, cartItemDtoAdd.Quantity, product.Price);


            _mockCartRepository.Setup(repo => repo.GetCartByUserId(userId))
                .ReturnsAsync((CartEntity?)null);

            _mockCartRepository.Setup(repo => repo.CreateCart(userId))
                .ReturnsAsync(cart);

            _mockProductRepository.Setup(repo => repo.GetProductById(cartItemDtoAdd.ProductId))
                .ReturnsAsync(product);

            _mockCartItemRepository.Setup(repo => repo.GetCartItem(It.IsAny<int>(), cartItemDtoAdd.ProductId))
                .ReturnsAsync((CartItemEntity?)null);

            _mockCartItemRepository.Setup(repo => repo.CreateCartItem(It.IsAny<CartItemEntity>()))
                .ReturnsAsync(cartItemEntity);

            _mockCacheService.Setup(cache => cache.Remove(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _mockMapper.Setup(m => m.Map<CartItemDto>(It.IsAny<CartItemEntity>()))
                .Returns(cartItemDto);

            // Act
            var result = await _cartItemService.AddItemToCart(userId, cartItemDtoAdd);

            // Assert
            Assert.NotNull(result);
            _mockCartRepository.Verify(repo => repo.GetCartByUserId(userId), Times.Once);
            _mockCartRepository.Verify(repo => repo.CreateCart(userId), Times.Once);
            _mockProductRepository.Verify(repo => repo.GetProductById(cartItemDtoAdd.ProductId), Times.Once);
            _mockCartItemRepository.Verify(repo => repo.GetCartItem(It.IsAny<int>(), cartItemDtoAdd.ProductId), Times.Once);
            _mockCartItemRepository.Verify(repo => repo.UpdateCartItem(It.IsAny<CartItemEntity>()), Times.Never);
            _mockCartItemRepository.Verify(repo => repo.CreateCartItem(It.IsAny<CartItemEntity>()), Times.Once);
            _mockCacheService.Verify(cache => cache.Remove(It.IsAny<string>()), Times.Exactly(2));
            _mockMapper.Verify(m => m.Map<CartItemDto>(It.IsAny<CartItemEntity>()), Times.Once);
        }

        /// <summary>
        /// Adds the item to cart product not found throws key not found exception.
        /// </summary>
        [Fact]
        public async Task AddItemToCart_ProductNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var userId = _fixture.Create<int>();
            var cartItemDto = CreateCartItemAddDto();
            var cart = CreateCart(userId);

            _mockCartRepository.Setup(repo => repo.GetCartByUserId(userId))
                .ReturnsAsync(cart);

            _mockProductRepository.Setup(repo => repo.GetProductById(cartItemDto.ProductId))
                .ReturnsAsync((ProductEntity?)null);

            // Act && Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _cartItemService.AddItemToCart(userId, cartItemDto));
            _mockCartRepository.Verify(repo => repo.GetCartByUserId(userId), Times.Once);
            _mockCartRepository.Verify(repo => repo.CreateCart(userId), Times.Never);
            _mockProductRepository.Verify(repo => repo.GetProductById(cartItemDto.ProductId), Times.Once);
            _mockCartItemRepository.Verify(repo => repo.GetCartItem(It.IsAny<int>(), cartItemDto.ProductId), Times.Never);
            _mockCartItemRepository.Verify(repo => repo.UpdateCartItem(It.IsAny<CartItemEntity>()), Times.Never);
            _mockCartItemRepository.Verify(repo => repo.CreateCartItem(It.IsAny<CartItemEntity>()), Times.Never);
            _mockCacheService.Verify(cache => cache.Remove(It.IsAny<string>()), Times.Never);
            _mockMapper.Verify(m => m.Map<CartItemDto>(It.IsAny<CartItemEntity>()), Times.Never);
        }

        /// <summary>
        /// Adds the item to cart product inactive throws invalid operation exception.
        /// </summary>
        [Fact]
        public async Task AddItemToCart_ProductInactive_ThrowsInvalidOperationException()
        {
            // Arrange
            var userId = 1;
            var cartItemDtoAdd = CreateCartItemAddDto();
            var cart = CreateCart(userId);
            var product = CreateProduct(cartItemDtoAdd.ProductId, isActive: false);

            _mockCartRepository.Setup(repo => repo.GetCartByUserId(userId))
                .ReturnsAsync(cart);

            _mockProductRepository.Setup(repo => repo.GetProductById(cartItemDtoAdd.ProductId))
                .ReturnsAsync(product);

            // Act && Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _cartItemService.AddItemToCart(userId, cartItemDtoAdd));
            _mockCartRepository.Verify(repo => repo.GetCartByUserId(userId), Times.Once);
            _mockCartRepository.Verify(repo => repo.CreateCart(userId), Times.Never);
            _mockProductRepository.Verify(repo => repo.GetProductById(cartItemDtoAdd.ProductId), Times.Once);
            _mockCartItemRepository.Verify(repo => repo.GetCartItem(It.IsAny<int>(), cartItemDtoAdd.ProductId), Times.Never);
            _mockCartItemRepository.Verify(repo => repo.UpdateCartItem(It.IsAny<CartItemEntity>()), Times.Never);
            _mockCartItemRepository.Verify(repo => repo.CreateCartItem(It.IsAny<CartItemEntity>()), Times.Never);
            _mockCacheService.Verify(cache => cache.Remove(It.IsAny<string>()), Times.Never);
            _mockMapper.Verify(m => m.Map<CartItemDto>(It.IsAny<CartItemEntity>()), Times.Never);
        }

        /// <summary>
        /// Adds the item to cart product out of stock throws invalid operation exception.
        /// </summary>
        [Fact]
        public async Task AddItemToCart_ProductOutOfStock_ThrowsInvalidOperationException()
        {
            // Arrange
            var userId = 1;
            var cartItemAddDto = CreateCartItemAddDto();
            var cart = CreateCart(userId);
            var product = CreateProduct(cartItemAddDto.ProductId, stock: 0);

            _mockCartRepository.Setup(sp => sp.GetCartByUserId(userId))
                .ReturnsAsync(cart);

            _mockProductRepository.Setup(repo => repo.GetProductById(cartItemAddDto.ProductId))
                .ReturnsAsync(product);

            // Act && Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _cartItemService.AddItemToCart(userId, cartItemAddDto));
            _mockCartRepository.Verify(repo => repo.GetCartByUserId(userId), Times.Once);
            _mockCartRepository.Verify(repo => repo.CreateCart(userId), Times.Never);
            _mockProductRepository.Verify(repo => repo.GetProductById(cartItemAddDto.ProductId), Times.Once);
            _mockCartItemRepository.Verify(repo => repo.GetCartItem(It.IsAny<int>(), cartItemAddDto.ProductId), Times.Never);
            _mockCartItemRepository.Verify(repo => repo.UpdateCartItem(It.IsAny<CartItemEntity>()), Times.Never);
            _mockCartItemRepository.Verify(repo => repo.CreateCartItem(It.IsAny<CartItemEntity>()), Times.Never);
            _mockCacheService.Verify(cache => cache.Remove(It.IsAny<string>()), Times.Never);
            _mockMapper.Verify(m => m.Map<CartItemDto>(It.IsAny<CartItemEntity>()), Times.Never);
        }

        /// <summary>
        /// Adds the item to cart stock insufficient throws invalid operation exception.
        /// </summary>
        [Fact]
        public async Task AddItemToCart_StockInsufficient_ThrowsInvalidOperationException()
        {
            // Arrange
            var userId = 1;
            var cartItemAddDto = CreateCartItemAddDto(10);
            var cart = CreateCart(userId);
            var product = CreateProduct(cartItemAddDto.ProductId, stock: 5);

            _mockCartRepository.Setup(repo => repo.GetCartByUserId(userId))
                .ReturnsAsync(cart);

            _mockProductRepository.Setup(repo => repo.GetProductById(cartItemAddDto.ProductId))
                .ReturnsAsync(product);

            _mockCartItemRepository.Setup(repo => repo.GetCartItem(It.IsAny<int>(), cartItemAddDto.ProductId))
                .ReturnsAsync((CartItemEntity?)null);

            // Act && Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _cartItemService.AddItemToCart(userId, cartItemAddDto));
            _mockCartRepository.Verify(repo => repo.GetCartByUserId(userId), Times.Once);
            _mockCartRepository.Verify(repo => repo.CreateCart(userId), Times.Never);
            _mockProductRepository.Verify(repo => repo.GetProductById(cartItemAddDto.ProductId), Times.Once);
            _mockCartItemRepository.Verify(repo => repo.GetCartItem(It.IsAny<int>(), cartItemAddDto.ProductId), Times.Once);
            _mockCartItemRepository.Verify(repo => repo.UpdateCartItem(It.IsAny<CartItemEntity>()), Times.Never);
            _mockCartItemRepository.Verify(repo => repo.CreateCartItem(It.IsAny<CartItemEntity>()), Times.Never);
            _mockCacheService.Verify(cache => cache.Remove(It.IsAny<string>()), Times.Never);
            _mockMapper.Verify(m => m.Map<CartItemDto>(It.IsAny<CartItemEntity>()), Times.Never);
        }

        /// <summary>
        /// Adds the item to cart item already in cart updates item.
        /// </summary>
        [Fact]
        public async Task AddItemToCart_ItemAlreadyInCart_UpdatesItem()
        {
            // Arrange
            var userId = 1;
            var cartItemAddDto = CreateCartItemAddDto(4);
            var cartEntity = CreateCart(userId);
            var productEntity = CreateProduct(cartItemAddDto.ProductId, stock: 20); 
            var cartItemEntity = CreateCartItemEntity(cartEntity.Id, cartItemAddDto.ProductId, cartItemAddDto.Quantity, productEntity.Price);
            var cartItemDto = CreateCartItemDto(cartEntity.Id, cartItemAddDto.ProductId, cartItemAddDto.Quantity, productEntity.Price);

            _mockCartRepository.Setup(sp => sp.GetCartByUserId(userId))
                .ReturnsAsync(cartEntity);

            _mockProductRepository.Setup(repo => repo.GetProductById(cartItemAddDto.ProductId))
                .ReturnsAsync(productEntity);

            _mockCartItemRepository.Setup(repo => repo.GetCartItem(It.IsAny<int>(), cartItemAddDto.ProductId))
                .ReturnsAsync(cartItemEntity);

            _mockCartItemRepository.Setup(repo => repo.UpdateCartItem(It.IsAny<CartItemEntity>()))
                .ReturnsAsync(cartItemEntity);

            _mockCacheService.Setup(cache => cache.Remove(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _mockMapper.Setup(m => m.Map<CartItemDto>(It.IsAny<CartItemEntity>()))
                .Returns(cartItemDto);

            // Act
            var result = await _cartItemService.AddItemToCart(userId, cartItemAddDto);

            // Assert
            Assert.NotNull(result);
            _mockCartRepository.Verify(repo => repo.GetCartByUserId(userId), Times.Once);
            _mockCartRepository.Verify(repo => repo.CreateCart(userId), Times.Never);
            _mockProductRepository.Verify(repo => repo.GetProductById(cartItemAddDto.ProductId), Times.Once);
            _mockCartItemRepository.Verify(repo => repo.GetCartItem(It.IsAny<int>(), cartItemAddDto.ProductId), Times.Once);
            _mockCartItemRepository.Verify(repo => repo.UpdateCartItem(It.IsAny<CartItemEntity>()), Times.Once);
            _mockCartItemRepository.Verify(repo => repo.CreateCartItem(It.IsAny<CartItemEntity>()), Times.Never);
            _mockCacheService.Verify(cache => cache.Remove(It.IsAny<string>()), Times.Exactly(2));
            _mockMapper.Verify(m => m.Map<CartItemDto>(It.IsAny<CartItemEntity>()), Times.Once);
        }

        /// <summary>
        /// Adds the item to cart item not in cart creates new item.
        /// </summary>
        [Fact]
        public async Task AddItemToCart_ItemNotInCart_CreatesNewItem()
        {
            // Arrange
            var userId = 1;
            var cartItemAddDto = CreateCartItemAddDto(1);
            var cartEntity = CreateCart(userId);
            var productEntity = CreateProduct(cartItemAddDto.ProductId);
            var cartItemEntity = CreateCartItemEntity(cartEntity.Id, cartItemAddDto.ProductId, cartItemAddDto.Quantity, productEntity.Price);
            var cartItemDto = CreateCartItemDto(cartEntity.Id, cartItemAddDto.ProductId, cartItemAddDto.Quantity, productEntity.Price);

            _mockCartRepository.Setup(sp => sp.GetCartByUserId(userId))
                .ReturnsAsync(cartEntity);

            _mockProductRepository.Setup(repo => repo.GetProductById(cartItemAddDto.ProductId))
                .ReturnsAsync(productEntity);

            _mockCartItemRepository.Setup(repo => repo.GetCartItem(It.IsAny<int>(), cartItemAddDto.ProductId))
                .ReturnsAsync((CartItemEntity?)null);

            _mockCartItemRepository.Setup(repo => repo.CreateCartItem(It.IsAny<CartItemEntity>()))
                .ReturnsAsync(cartItemEntity);

            _mockCacheService.Setup(cache => cache.Remove(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _mockMapper.Setup(m => m.Map<CartItemDto>(It.IsAny<CartItemEntity>()))
                .Returns(cartItemDto);

            // Act
            var result = await _cartItemService.AddItemToCart(userId, cartItemAddDto);

            // Assert
            Assert.NotNull(result);
            _mockCartRepository.Verify(repo => repo.GetCartByUserId(userId), Times.Once);
            _mockCartRepository.Verify(repo => repo.CreateCart(userId), Times.Never);
            _mockProductRepository.Verify(repo => repo.GetProductById(cartItemAddDto.ProductId), Times.Once);
            _mockCartItemRepository.Verify(repo => repo.GetCartItem(It.IsAny<int>(), cartItemAddDto.ProductId), Times.Once);
            _mockCartItemRepository.Verify(repo => repo.UpdateCartItem(It.IsAny<CartItemEntity>()), Times.Never);
            _mockCartItemRepository.Verify(repo => repo.CreateCartItem(It.IsAny<CartItemEntity>()), Times.Once);
            _mockCacheService.Verify(cache => cache.Remove(It.IsAny<string>()), Times.Exactly(2));
            _mockMapper.Verify(m => m.Map<CartItemDto>(It.IsAny<CartItemEntity>()), Times.Once);
        }

        private CartItemAddDto CreateCartItemAddDto(int quantity = 1)
        {
            return _fixture.Build<CartItemAddDto>()
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