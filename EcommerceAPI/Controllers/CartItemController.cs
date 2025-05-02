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

        /// <summary>
        /// Initializes a new instance of the <see cref="CartItemController"/> class.
        /// </summary>
        /// <param name="cartItemService">The service responsible for cart item operations.</param>
        /// <param name="userService">The service responsible for user-related operations.</param>
        public CartItemController(ICartItemService cartItemService, IUserService userService)
        {
            _cartItemService = cartItemService;
            _userService = userService;
        }

        /// <summary>
        /// Adds an item to the authenticated user's shopping cart.
        /// </summary>
        /// <param name="cartItem">The cart item to be added.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the updated cart item,
        /// or a Bad Request response if the operation fails.
        /// </returns>
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
        /// Updates the quantity of a specific item in the authenticated user's cart.
        /// </summary>
        /// <param name="cartItem">The cart item with updated quantity information.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the updated cart item,
        /// or a Bad Request response if the update fails.
        /// </returns>
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
        /// Removes a specific item from the authenticated user's cart.
        /// </summary>
        /// <param name="productId">The identifier of the product to remove from the cart.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the deletion. Returns No Content on success.
        /// </returns>
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