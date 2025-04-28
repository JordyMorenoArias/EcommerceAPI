using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs;
using EcommerceAPI.Models.DTOs.Order;
using EcommerceAPI.Models.DTOs.OrderDetail;

namespace EcommerceAPI.Services.OrderManagement.Interfaces
{
    public interface IOrderManagementService
    {
        Task<OrderDto> AddOrderDetailToOrder(int userId, int orderId, IEnumerable<OrderDetailAddDto> orderDetails);
        Task<OrderDto> CreateOrderWithDetails(int userId, IEnumerable<OrderDetailAddDto> orderDetails);
        Task<bool> DeleteOrder(int orderId);
        Task<PagedResult<OrderDto>> GetOrdersByDateRange(int page, int pageSize, DateTime? startDate = null, DateTime? endDate = null);
        Task<PagedResult<OrderDto>> GetOrdersBySeller(int sellerId, int page, int pageSize, DateTime? startDate = null, DateTime? endDate = null);
        Task<PagedResult<OrderDto>> GetOrdersByStatus(OrderStatus status, int page, int pageSize, DateTime? startDate = null, DateTime? endDate = null);
        Task<PagedResult<OrderDto>> GetOrdersByUserId(int userId, int page, int pageSize, DateTime? startDate = null, DateTime? endDate = null);
        Task<OrderDto> GetOrderWithDetails(int userId, UserRole role, int orderId);
        Task<OrderDto> UpdateOrderAddress(int userId, int orderId, int addressId);
        Task<OrderDto> UpdateOrderStatus(int orderId, OrderStatus newStatus);
    }
}