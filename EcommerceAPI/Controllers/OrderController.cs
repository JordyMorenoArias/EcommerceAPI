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
        [AuthorizeRole(UserRole.Admin)]
        public async Task<IActionResult> GetOrdersByDateRange(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _orderManagementService.GetOrdersByDateRange(page, pageSize, startDate, endDate);
            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        [AuthorizeRole(UserRole.Customer, UserRole.Admin)]
        public async Task<IActionResult> GetOrdersByUserId(
            int userId,
            [FromQuery] int page,
            [FromQuery] int pageSize,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);

            if (userAuthenticated.Role != UserRole.Admin && userAuthenticated.Id != userId)
                return Forbid("You do not have permission to access this user's orders.");

            var orders = await _orderManagementService.GetOrdersByUserId(userId, page, pageSize, startDate, endDate);
            return Ok(orders);
        }

        [HttpGet("by-status")]
        [AuthorizeRole(UserRole.Admin)]
        public async Task<IActionResult> GetOrdersByStatus(
            [FromQuery] string status,
            [FromQuery] int page,
            [FromQuery] int pageSize,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);

            if (userAuthenticated.Role != UserRole.Admin)
                return Forbid("You do not have permission to access orders with this status.");

            if (!Enum.TryParse(typeof(OrderStatus), status, true, out var statusEnum))
            {
                return BadRequest("Invalid order status.");
            }

            var orders = await _orderManagementService.GetOrdersByStatus((OrderStatus)statusEnum, page, pageSize, startDate, endDate);

            return Ok(orders);
        }

        [HttpGet("by-seller")]
        [AuthorizeRole(UserRole.Seller, UserRole.Admin)]
        public async Task<IActionResult> GetOrdersBySeller(
            int sellerId,
            [FromQuery] int page,
            [FromQuery] int pageSize,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);

            if (userAuthenticated.Role != UserRole.Admin && userAuthenticated.Id != sellerId)
                return Forbid("You do not have permission to access this seller's orders.");

            var orders = await _orderManagementService.GetOrdersBySeller(sellerId, page, pageSize, startDate, endDate);
            return Ok(orders);
        }

        [HttpPost("create")]
        [AuthorizeRole(UserRole.Customer)]
        public async Task<IActionResult> CreateOrderWithDetails([FromBody] IEnumerable<OrderDetailAddDto> orderDetails)
        {
            if (orderDetails == null || !orderDetails.Any())
                return BadRequest("Order details cannot be null or empty.");
            
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var order = await _orderManagementService.CreateOrderWithDetails(userAuthenticated.Id, orderDetails);

            if (order == null)
                return BadRequest("Failed to create order.");

            return Ok(order);
        }

        [HttpPost("add/{orderId}")]
        [AuthorizeRole(UserRole.Customer)]
        public async Task<IActionResult> AddOrderDetailToOrder(int orderId, [FromBody] IEnumerable<OrderDetailAddDto> orderDetails)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var order = await _orderManagementService.AddOrderDetailToOrder(userAuthenticated.Id, orderId, orderDetails);

            if (order == null)
                return BadRequest("Failed to add order details.");

            return Ok(order);
        }

        [HttpPut("update-address")]
        [AuthorizeRole(UserRole.Customer)]
        public async Task<IActionResult> UpdateOrderAddress([FromQuery] int orderId, [FromQuery] int addressId)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var order = await _orderManagementService.UpdateOrderAddress(userAuthenticated.Id, orderId, addressId);

            if (order == null)
                return BadRequest("Failed to update order address.");

            return Ok(order);
        }

        [HttpDelete("delete/{orderId}")]
        [AuthorizeRole(UserRole.Customer)]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);

            var result = await _orderManagementService.DeleteOrder(orderId);

            if (!result)
                return BadRequest("Failed to delete order.");

            return Ok("Order deleted successfully.");
        }
    }
}
