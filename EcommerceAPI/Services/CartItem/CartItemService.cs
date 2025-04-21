using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Repositories;
using EcommerceAPI.Services.CartItem.Interfaces;
using EcommerceAPI.Services.Infrastructure.Interfaces;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Models.DTOs.Cart;
using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Services.CartItem
{
    /// <summary>
    /// Service class for managing cart items in the e-commerce system.
    /// </summary>
    /// <seealso cref="EcommerceAPI.Services.CartItem.Interfaces.ICartItemService" />
    public class CartItemService : ICartItemService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICacheService _cacheService;
        private readonly ILogger<CartItemService> _logger;

        public CartItemService(ICartRepository cartRepository, ICartItemRepository cartItemRepository, IProductRepository productRepository, ICacheService cacheService, ILogger<CartItemService> logger)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _productRepository = productRepository;
            _cacheService = cacheService;
            _logger = logger;
        }

        /// <summary>
        /// Adds the item to cart.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cartItem">The cart item.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Quantity must be greater than zero - Quantity</exception>
        public async Task<CartItemEntity?> AddItemToCart(int userId, CartItemAddDto cartItem)
        {
            if (cartItem.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(cartItem.Quantity));

            var cart = await GetOrCreateCartByUserId(userId);
            var product = await GetProductOrThrow(cartItem.ProductId);
            var existingItem = await _cartItemRepository.GetCartItem(cart.Id, cartItem.ProductId);

            ValidateStock(existingItem?.Quantity ?? 0, cartItem.Quantity, product.Stock);

            CartItemEntity? updatedItem;

            if (existingItem != null)
            {
                existingItem.Quantity = cartItem.Quantity;
                existingItem.UnitPrice = product.Price;

                updatedItem = await _cartItemRepository.UpdateCartItem(existingItem);
            }
            else
            {
                updatedItem = await CreateNewCartItem(cart.Id, cartItem, product.Price);
            }

            await RefreshCartCacheWithLatestCart(userId);

            return updatedItem;
        }

        /// <summary>
        /// Updates the cart item quantity.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cartItem">The cart item.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Quantity must be greater than zero - Quantity</exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">
        /// Cart not found
        /// or
        /// Cart item not found
        /// </exception>
        public async Task<CartItemEntity?> UpdateCartItemQuantity(int userId, CartItemUpdateDto cartItem)
        {
            if (cartItem.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(cartItem.Quantity));

            var cart = await _cartRepository.GetCartByUserId(userId)
                       ?? throw new KeyNotFoundException("Cart not found");

            var product = await GetProductOrThrow(cartItem.ProductId);
            var existingItem = await _cartItemRepository.GetCartItem(cart.Id, cartItem.ProductId)
                              ?? throw new KeyNotFoundException("Cart item not found");

            ValidateStock(existingItem.Quantity, cartItem.Quantity, product.Stock);

            var updatedItem = await UpdateCartItemQuantity(userId, cartItem);

            await RefreshCartCacheWithLatestCart(userId);

            return updatedItem;
        }

        /// <summary>
        /// Gets the or create cart by user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        private async Task<CartEntity> GetOrCreateCartByUserId(int userId)
        {
            return await _cartRepository.GetCartByUserId(userId)
                   ?? await _cartRepository.CreateCart(userId);
        }

        /// <summary>
        /// Gets the product or throw.
        /// </summary>
        /// <param name="productId">The product identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Product not found</exception>
        /// <exception cref="System.InvalidOperationException">
        /// Product is not active
        /// or
        /// Product is out of stock
        /// </exception>
        private async Task<ProductEntity> GetProductOrThrow(int productId)
        {
            var product = await _productRepository.GetProductById(productId);

            if (product == null)
                throw new KeyNotFoundException("Product not found");

            if (!product.IsActive)
                throw new InvalidOperationException("Product is not active");

            if (product.Stock <= 0)
                throw new InvalidOperationException("Product is out of stock");

            return product;
        }

        /// <summary>
        /// Validates the stock.
        /// </summary>
        /// <param name="currentQuantity">The current quantity.</param>
        /// <param name="quantityToAdd">The quantity to add.</param>
        /// <param name="stock">The stock.</param>
        /// <exception cref="System.InvalidOperationException">Not enough stock available</exception>
        private void ValidateStock(int currentQuantity, int quantityToAdd, int stock)
        {
            if (currentQuantity + quantityToAdd > stock)
                throw new InvalidOperationException("Not enough stock available");
        }

        /// <summary>
        /// Creates the new cart item.
        /// </summary>
        /// <param name="cartId">The cart identifier.</param>
        /// <param name="cartItem">The cart item.</param>
        /// <param name="unitPrice">The unit price.</param>
        /// <returns></returns>
        private async Task<CartItemEntity?> CreateNewCartItem(int cartId, CartItemAddDto cartItem, decimal unitPrice)
        {
            var newItem = new CartItemEntity
            {
                CartId = cartId,
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity,
                UnitPrice = unitPrice
            };

            return await _cartItemRepository.CreateCartItem(newItem);
        }

        /// <summary>
        /// Deletes the item from cart.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="productId">The product identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">
        /// Cart not found
        /// or
        /// Cart item not found
        /// </exception>
        public async Task<bool> DeleteItemFromCart(int userId, int productId)
        {
            var cart = await _cartRepository.GetCartByUserId(userId);

            if (cart is null)
                throw new KeyNotFoundException("Cart not found");

            var existingItem = await _cartItemRepository.GetCartItem(cart.Id, productId);

            if (existingItem is null)
                throw new KeyNotFoundException("Cart item not found");

            var result = await _cartItemRepository.DeleteCartItem(existingItem.Id);

            var cartUpdate = await _cartRepository.GetCartByUserId(userId);

            if (result)
            {
                await RefreshCartCache(userId, cartUpdate!);
            }

            return result;
        }

        /// <summary>
        /// Refreshes the cart cache with latest cart.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <exception cref="System.InvalidOperationException">Cart not found after update</exception>
        private async Task RefreshCartCacheWithLatestCart(int userId)
        {
            var updatedCart = await _cartRepository.GetCartByUserId(userId);

            if (updatedCart is null)
                throw new InvalidOperationException("Cart not found after update");

            await RefreshCartCache(userId, updatedCart);
        }

        /// <summary>
        /// Refreshes the cart cache.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cart">The cart.</param>
        private async Task RefreshCartCache(int userId, CartEntity cart)
        {
            var cacheKey = $"cart_{userId}";
            await _cacheService.Remove(cacheKey);
            await _cacheService.Set(cacheKey, cart, TimeSpan.FromMinutes(30));

            var cacheKeyTotal = $"cart_total_{userId}";
            await _cacheService.Remove(cacheKeyTotal);
            var total = await _cartRepository.GetCartTotal(cart.Id);
            await _cacheService.Set(cacheKeyTotal, total, TimeSpan.FromMinutes(30));
        }
    }
}