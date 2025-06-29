﻿using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Repositories;
using EcommerceAPI.Services.CartItem.Interfaces;
using EcommerceAPI.Services.Infrastructure.Interfaces;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Models.DTOs.Cart;
using EcommerceAPI.Models;
using AutoMapper;
using EcommerceAPI.Models.DTOs.CartItem;

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
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartItemService"/> class.
        /// </summary>
        /// <param name="cartRepository">The cart repository.</param>
        /// <param name="cartItemRepository">The cart item repository.</param>
        /// <param name="productRepository">The product repository.</param>
        /// <param name="cacheService">The cache service.</param>
        /// <param name="mapper">The mapper.</param>
        public CartItemService(ICartRepository cartRepository,  ICartItemRepository cartItemRepository, IProductRepository productRepository, ICacheService cacheService, IMapper mapper)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _productRepository = productRepository;
            _cacheService = cacheService;
            _mapper = mapper;
        }

        /// <summary>
        /// Adds the item to cart.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cartItem">The cart item.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the added or updated <see cref="CartItemEntity"/> 
        /// if successful; otherwise, <c>null</c>.
        /// </returns>
        /// <exception cref="System.ArgumentException">Quantity must be greater than zero - Quantity</exception>
        public async Task<CartItemDto?> AddItemToCart(int userId, CartItemAddDto cartItem)
        {
            if (cartItem.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(cartItem.Quantity));

            var cart = await GetOrCreateCartByUserId(userId);
            var product = await GetProductOrThrow(cartItem.ProductId);
            var existingItem = await _cartItemRepository.GetCartItem(cart.Id, cartItem.ProductId);

            if ((existingItem?.Quantity ?? 0) + cartItem.Quantity > product.Stock)
                throw new InvalidOperationException("Not enough stock available");

            var newCartItem = new CartItemEntity();

            if (existingItem is not null)
            {
                existingItem.Quantity = cartItem.Quantity;
                existingItem.UnitPrice = product.Price;

                newCartItem = await _cartItemRepository.UpdateCartItem(existingItem);
            }
            else
            {
                newCartItem = await CreateNewCartItem(cart.Id, cartItem, product.Price);
            }

            await RemoveCartCache(userId);

            var dto = _mapper.Map<CartItemDto>(newCartItem);
            return dto;
        }

        /// <summary>
        /// Updates the cart item quantity.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cartItem">The cart item.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the updated <see cref="CartItemEntity"/> 
        /// if successful; otherwise, <c>null</c>.
        /// </returns>
        /// <exception cref="System.ArgumentException">Quantity must be greater than zero - Quantity</exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">
        /// Cart not found
        /// or
        /// Cart item not found
        /// </exception>
        public async Task<CartItemDto?> UpdateCartItemQuantity(int userId, CartItemUpdateDto cartItem)
        {
            if (cartItem.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(cartItem.Quantity));

            var cart = await _cartRepository.GetCartByUserId(userId) ?? throw new KeyNotFoundException("Cart not found");

            var product = await GetProductOrThrow(cartItem.ProductId);

            var existingItem = await _cartItemRepository.GetCartItem(cart.Id, cartItem.ProductId);

            if (existingItem is null)
                throw new KeyNotFoundException("Cart item not found");

            if (cartItem.Quantity > product.Stock)
                throw new InvalidOperationException("Not enough stock available");

            existingItem.Quantity = cartItem.Quantity;
            existingItem.UnitPrice = product.Price;
            var updatedItem = await _cartItemRepository.UpdateCartItem(existingItem);

            await RemoveCartCache(userId);

            var dto = _mapper.Map<CartItemDto>(updatedItem);
            return dto;
        }

        /// <summary>
        /// Deletes the item from cart.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="productId">The product identifier.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result is <c>true</c> if the item was successfully deleted; 
        /// otherwise, <c>false</c>.
        /// </returns>
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

            await RemoveCartCache(userId);

            return result;
        }

        /// <summary>
        /// Gets the or create cart by user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the existing or newly created <see cref="CartEntity"/>.
        /// </returns>
        private async Task<CartEntity> GetOrCreateCartByUserId(int userId)
        {
            return await _cartRepository.GetCartByUserId(userId)
                   ?? await _cartRepository.CreateCart(userId);
        }

        /// <summary>
        /// Gets the product or throw.
        /// </summary>
        /// <param name="productId">The product identifier.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the <see cref="ProductEntity"/> 
        /// if found and valid.
        /// </returns>
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
        /// Creates the new cart item.
        /// </summary>
        /// <param name="cartId">The cart identifier.</param>
        /// <param name="cartItem">The cart item.</param>
        /// <param name="unitPrice">The unit price.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the newly created <see cref="CartItemEntity"/> 
        /// if successful; otherwise, <c>null</c>.
        /// </returns>
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

        private async Task RemoveCartCache(int userId)
        {
            var cacheKey = $"cart_{userId}";
            var cacheKeyTotal = $"cart_total_{userId}";

            await _cacheService.Remove(cacheKey);
            await _cacheService.Remove(cacheKeyTotal);
        }
    }
}