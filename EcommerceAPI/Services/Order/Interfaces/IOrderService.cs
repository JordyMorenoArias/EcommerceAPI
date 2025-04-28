using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs;
using EcommerceAPI.Models.DTOs.Order;

namespace EcommerceAPI.Services.Order.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> AddOrder(int userId);
        Task<bool> DeleteOrder(int orderId);
        Task<OrderDto?> GetOrderById(int id);
        Task<PagedResult<OrderDto>> GetOrdersByDateRange(int page, int pageSize, DateTime? startDate = null, DateTime? endDate = null);
        Task<PagedResult<OrderDto>> GetOrdersBySeller(int sellerId, int page, int pageSize, DateTime? startDate = null, DateTime? endDate = null);
        Task<PagedResult<OrderDto>> GetOrdersByStatus(OrderStatus status, int page, int pageSize, DateTime? startDate = null, DateTime? endDate = null);
        Task<PagedResult<OrderDto>> GetOrdersByUserId(int userId, int page, int pageSize, DateTime? startDate = null, DateTime? endDate = null, bool isOwner = false);
        Task<OrderDto?> GetOrderWithDetails(int orderId);
        Task<OrderDto> UpdateAddressOrder(int orderId, int addressId);
        Task<OrderDto> UpdateAmountOrder(int orderId, decimal amount);
        Task<OrderDto> UpdateOrderStatus(int orderId, OrderStatus newStatus);
    }
}