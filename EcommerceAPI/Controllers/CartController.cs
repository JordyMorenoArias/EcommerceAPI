using EcommerceAPI.Constants;
using EcommerceAPI.Filters;
using EcommerceAPI.Services.Cart.Interfaces;
using EcommerceAPI.Services.User.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{
    /// <summary>
    /// Controller for managing shopping cart operations in the e-commerce system.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeRole(UserRole.Customer)]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IUserService _userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartController"/> class.
        /// </summary>
        /// <param name="cartService">Service responsible for cart-related operations.</param>
        /// <param name="userService">Service responsible for user authentication and management.</param>
        public CartController(ICartService cartService, IUserService userService)
        {
            _cartService = cartService;
            _userService = userService;
        }

        /// <summary>
        /// Retrieves the shopping cart of the authenticated user.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the current shopping cart,
        /// or an empty cart if none exists.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var user = _userService.GetAuthenticatedUser(HttpContext);
            var cart = await _cartService.GetCartByUserId(user.Id);
            return Ok(cart);
        }

        /// <summary>
        /// Clears all items from the authenticated user's shopping cart.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// Returns <see cref="NoContentResult"/> on success.
        /// </returns>
        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var user = _userService.GetAuthenticatedUser(HttpContext);
            await _cartService.ClearCart(user.Id);
            return NoContent();
        }

        /// <summary>
        /// Retrieves the total cost of the authenticated user's shopping cart.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the total amount of the cart.
        /// </returns>
        [HttpGet("total")]
        public async Task<IActionResult> GetTotal()
        {
            var user = _userService.GetAuthenticatedUser(HttpContext);
            var total = await _cartService.GetCartTotal(user.Id);
            return Ok(total);
        }
    }
}