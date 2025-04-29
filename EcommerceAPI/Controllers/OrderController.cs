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
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : Controller
    {
        private readonly IOrderManagementService _orderManagementService;
        private readonly IUserService _userService;

        public OrderController(IOrderManagementService orderManagementService, IUserService userService)
        {
            _orderManagementService = orderManagementService;
            _userService = userService;
        }

        [HttpGet("{orderId}")]
        [AuthorizeRole(UserRole.Customer, UserRole.Admin)]
        public async Task<IActionResult> GetOrderWithDetails(int orderId)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);

            var order = await _orderManagementService.GetOrderWithDetails(userAuthenticated.Id, userAuthenticated.Role, orderId);
            return Ok(order);
        }

        [HttpGet]
        [AuthorizeRole(UserRole.Customer, UserRole.Admin)]
        public async Task<IActionResult> GetOrders([FromQuery] OrderQueryParameters parameters)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var orders = await _orderManagementService.GetOrders(userAuthenticated.Id, userAuthenticated.Role, parameters);
            return Ok(orders);
        }

        [HttpGet("seller")]
        [AuthorizeRole(UserRole.Seller, UserRole.Admin)]
        public async Task<IActionResult> GetSellerOrders([FromQuery] OrderSellerQueryParameters parameters)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var orders = await _orderManagementService.GetSellerOrders(userAuthenticated.Id, userAuthenticated.Role, parameters);
            return Ok(orders);
        }

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
