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

        public CartController(ICartService cartService, IUserService userService)
        {
            _cartService = cartService;
            _userService = userService;
        }

        /// <summary>
        /// Gets the cart.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var user = _userService.GetAuthenticatedUser(HttpContext);
            var cart = await _cartService.GetCartByUserId(user.Id);
            return Ok(cart);
        }

        /// <summary>
        /// Clears the cart.
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var user = _userService.GetAuthenticatedUser(HttpContext);
            await _cartService.ClearCart(user.Id);
            return NoContent();
        }

        /// <summary>
        /// Gets the total.
        /// </summary>
        /// <returns></returns>
        [HttpGet("total")]
        public async Task<IActionResult> GetTotal()
        {
            var user = _userService.GetAuthenticatedUser(HttpContext);
            var total = await _cartService.GetCartTotal(user.Id);
            return Ok(total);
        }
    }
}