using EcommerceAPI.Constants;
using EcommerceAPI.Filters;
using EcommerceAPI.Models.DTOs.Order;
using EcommerceAPI.Models.DTOs;
using EcommerceAPI.Models.DTOs.OrderDetail;
using EcommerceAPI.Services.OrderManagement.Interfaces;
using EcommerceAPI.Services.User.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{
    /// <summary>
    /// Controller responsible for managing order-related operations.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeRole(UserRole.Admin, UserRole.Seller, UserRole.Customer)]
    public class OrderController : Controller
    {
        private readonly IOrderManagementService _orderManagementService;
        private readonly IUserService _userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderController"/> class.
        /// </summary>
        /// <param name="orderManagementService">The service responsible for order management logic.</param>
        /// <param name="userService">The service responsible for user-related operations.</param>
        public OrderController(IOrderManagementService orderManagementService, IUserService userService)
        {
            _orderManagementService = orderManagementService;
            _userService = userService;
        }

        /// <summary>
        /// Gets the order with its details by ID.
        /// </summary>
        /// <param name="orderId">The identifier of the order.</param>
        /// <returns>An <see cref="IActionResult"/> containing the order with details.</returns>
        [HttpGet("{orderId}")]
        [AuthorizeRole(UserRole.Customer, UserRole.Admin)]
        public async Task<IActionResult> GetOrderWithDetails(int orderId)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var order = await _orderManagementService.GetOrderWithDetails(userAuthenticated.Id, userAuthenticated.Role, orderId);
            return Ok(order);
        }

        /// <summary>
        /// Gets the list of orders for the authenticated user based on query parameters.
        /// </summary>
        /// <param name="parameters">The filtering and paging parameters.</param>
        /// <returns>An <see cref="IActionResult"/> containing the list of orders.</returns>
        [HttpGet]
        [AuthorizeRole(UserRole.Customer, UserRole.Admin)]
        public async Task<IActionResult> GetOrders([FromQuery] OrderQueryParameters parameters)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var orders = await _orderManagementService.GetOrders(userAuthenticated.Id, userAuthenticated.Role, parameters);
            return Ok(orders);
        }

        /// <summary>
        /// Gets the list of orders for the seller based on query parameters.
        /// </summary>
        /// <param name="parameters">The filtering and paging parameters specific to the seller.</param>
        /// <returns>An <see cref="IActionResult"/> containing the list of seller orders.</returns>
        [HttpGet("seller")]
        [AuthorizeRole(UserRole.Seller, UserRole.Admin)]
        public async Task<IActionResult> GetSellerOrders([FromQuery] OrderSellerQueryParameters parameters)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var orders = await _orderManagementService.GetSellerOrders(userAuthenticated.Id, userAuthenticated.Role, parameters);
            return Ok(orders);
        }

        /// <summary>
        /// Creates a new order with the specified details.
        /// </summary>
        /// <param name="orderDetails">The collection of order details to include in the new order.</param>
        /// <returns>An <see cref="IActionResult"/> containing the created order.</returns>
        [HttpPost]
        [AuthorizeRole(UserRole.Customer)]
        public async Task<IActionResult> CreateOrderWithDetails([FromBody] IEnumerable<OrderDetailAddDto> orderDetails)
        {
            if (orderDetails == null || !orderDetails.Any())
                return BadRequest("Order details cannot be null or empty.");

            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var order = await _orderManagementService.CreateOrderWithDetails(userAuthenticated.Id, orderDetails);

            if (order == null)
                return BadRequest("Failed to create order.");

            return CreatedAtAction(nameof(GetOrderWithDetails), new { orderId = order.Id }, order);
        }

        /// <summary>
        /// Adds additional order details to an existing order.
        /// </summary>
        /// <param name="orderId">The identifier of the order to update.</param>
        /// <param name="orderDetails">The order details to add.</param>
        /// <returns>An <see cref="IActionResult"/> containing the updated order.</returns>
        [HttpPost("{orderId}/details")]
        [AuthorizeRole(UserRole.Customer)]
        public async Task<IActionResult> AddOrderDetailToOrder(int orderId, [FromBody] IEnumerable<OrderDetailAddDto> orderDetails)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var order = await _orderManagementService.AddOrderDetailToOrder(userAuthenticated.Id, orderId, orderDetails);

            if (order == null)
                return BadRequest("Failed to add order details.");

            return Ok(order);
        }

        /// <summary>
        /// Updates the address associated with a specific order.
        /// </summary>
        /// <param name="orderId">The identifier of the order to update.</param>
        /// <param name="addressId">The identifier of the new address.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the update operation.</returns>
        [HttpPut("{orderId}/address")]
        [AuthorizeRole(UserRole.Customer)]
        public async Task<IActionResult> UpdateOrderAddress([FromQuery] int orderId, [FromQuery] int addressId)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var order = await _orderManagementService.UpdateOrderAddress(userAuthenticated.Id, orderId, addressId);

            if (order == null)
                return BadRequest("Failed to update order address.");

            return Ok(order);
        }

        /// <summary>
        /// Deletes an order by its identifier.
        /// </summary>
        /// <param name="orderId">The identifier of the order to delete.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the deletion.</returns>
        [HttpDelete("{orderId}")]
        [AuthorizeRole(UserRole.Customer)]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var result = await _orderManagementService.DeleteOrder(userAuthenticated.Id, orderId);

            if (!result)
                return BadRequest("Failed to delete order.");

            return Ok("Order deleted successfully.");
        }
    }
}