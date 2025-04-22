using EcommerceAPI.Constants;
using EcommerceAPI.Filters;
using EcommerceAPI.Models.DTOs.Cart;
using EcommerceAPI.Services.CartItem.Interfaces;
using EcommerceAPI.Services.User.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{
    /// <summary>
    /// Controller for managing shopping cart item operations in the e-commerce system.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeRole(UserRole.Customer)]
    public class CartItemController : Controller
    {
        private readonly ICartItemService _cartItemService;
        private readonly IUserService _userService;

        public CartItemController(ICartItemService cartItemService, IUserService userService)
        {
            _cartItemService = cartItemService;
            _userService = userService;
        }

        /// <summary>
        /// Adds the item to cart.
        /// </summary>
        /// <param name="cartItem">The cart item.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddItemToCart([FromBody] CartItemAddDto cartItem)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var result = await _cartItemService.AddItemToCart(userAuthenticated.Id, cartItem);

            if (result == null)
                return BadRequest("Failed to add item to cart.");

            return Ok(result);
        }

        /// <summary>
        /// Updates the cart item.
        /// </summary>
        /// <param name="cartItem">The cart item.</param>
        /// <returns></returns>
        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateCartItem([FromBody] CartItemUpdateDto cartItem)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var result = await _cartItemService.UpdateCartItemQuantity(userAuthenticated.Id, cartItem);

            if (result == null)
                return BadRequest("Failed to update cart item.");

            return Ok(result);
        }

        /// <summary>
        /// Removes the item from cart.
        /// </summary>
        /// <param name="productId">The product identifier.</param>
        /// <returns></returns>
        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveItemFromCart(int productId)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var result = await _cartItemService.DeleteItemFromCart(userAuthenticated.Id, productId);

            if (!result)
                return BadRequest("Failed to remove item from cart.");

            return NoContent();
        }
    }
}