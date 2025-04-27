using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs.Order;

namespace EcommerceAPI.Services.Order.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> AddOrder(int userId);
        Task<bool> DeleteOrder(int orderId);
        Task<OrderDto?> GetOrderById(int id);
        Task<IEnumerable<OrderDto>> GetOrdersByDateRange(DateTime startDate, DateTime endDate);
        Task<IEnumerable<OrderDto>> GetOrdersByStatus(OrderStatus status);
        Task<IEnumerable<OrderDto>> GetOrdersByUserId(int userId);
        Task<OrderDto?> GetOrderWithDetails(int orderId);
        Task<decimal> GetTotalSales();
        Task<OrderDto> UpdateAddressOrder(int orderId, int addressId);
        Task<OrderDto> UpdateAmountOrder(int orderId, decimal amount);
        Task<OrderDto> UpdateOrderStatus(int orderId, OrderStatus newStatus);
    }
}