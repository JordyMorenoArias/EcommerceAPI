using AutoMapper;
using EcommerceAPI.AutoMapper;
using EcommerceAPI.Models;
using EcommerceAPI.Models.DTOs.Cart;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Repositories;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Services.Cart.Interfaces;
using EcommerceAPI.Services.Infrastructure.Interfaces;

namespace EcommerceAPI.Services.Cart
{
    /// <summary>
    /// Service class for managing shopping carts in the e-commerce system.
    /// </summary>
    /// <seealso cref="EcommerceAPI.Services.Cart.Interfaces.ICartService" />
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartService"/> class.
        /// </summary>
        /// <param name="cartRepository">The cart repository.</param>
        /// <param name="cacheService">The cache service.</param>
        /// <param name="mapper">The mapper.</param>
        public CartService(ICartRepository cartRepository, ICacheService cacheService, IMapper mapper)
        {
            _cartRepository = cartRepository;
            _cacheService = cacheService;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets the cart by user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        /// A <see cref="CartDto"/> representing the user's cart, including items and total cost.
        /// </returns>
        public async Task<CartDto> GetCartByUserId(int userId)
        {
            var cacheKey = $"cart_{userId}";
            var cachedCart = await _cacheService.Get<CartDto>(cacheKey);

            if (cachedCart is not null)
                return cachedCart;

            var cart = await _cartRepository.GetCartByUserId(userId);

            if (cart is null)
                cart = await _cartRepository.CreateCart(userId);

            var total = await _cartRepository.GetCartTotal(cart.Id);

            var cartDto = _mapper.Map<CartDto>(cart);
            cartDto.Total = (decimal)total;

            await _cacheService.Set(cacheKey, cartDto, TimeSpan.FromMinutes(30));
            await _cacheService.Set($"cart_total_{userId}", total, TimeSpan.FromMinutes(30));
            return cartDto;
        }

        /// <summary>
        /// Clears the cart.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        public async Task ClearCart(int userId)
        {
            var cacheKey = $"cart_{userId}";
            var cachedTotal = $"cart_total_{userId}";

            await _cartRepository.ClearCart(userId);

            await _cacheService.Remove(cacheKey);
            await _cacheService.Remove(cachedTotal);
        }

        /// <summary>
        /// Gets the cart total.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        /// A <see cref="decimal"/> value representing the total cost of items in the user's cart,
        /// or <c>null</c> if no total is found in cache and the cart does not exist.
        /// </returns>
        /// <exception cref="KeyNotFoundException">Thrown if the cart does not exist for the specified user.</exception>
        public async Task<decimal?> GetCartTotal(int userId)
        {
            var cacheKey = $"cart_total_{userId}";
            var cachedTotal = await _cacheService.Get<decimal?>(cacheKey);

            if (cachedTotal is not null)
                return cachedTotal;

            var cart = await _cartRepository.GetCartByUserId(userId);

            if (cart is null)
                throw new KeyNotFoundException("Cart not found");

            var total = await _cartRepository.GetCartTotal(cart.Id);
            await _cacheService.Set(cacheKey, total, TimeSpan.FromMinutes(30));
            return total;
        }
    }
}