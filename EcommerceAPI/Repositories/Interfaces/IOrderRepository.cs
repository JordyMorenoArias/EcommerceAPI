using EcommerceAPI.Constants;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<OrderEntity> AddOrder(OrderEntity order);
        Task<bool> DeleteOrder(int orderId);
        Task<IEnumerable<OrderEntity>> GetOrdersByUserId(int userId);
        Task<OrderEntity?> GetOrderById(int id);
        Task<IEnumerable<OrderEntity>> GetOrdersByStatus(OrderStatus status);
        Task<OrderEntity?> GetOrderWithDetails(int orderId);
        Task<decimal> GetTotalSales();
        Task<IEnumerable<OrderEntity>> GetOrdersByDateRange(DateTime startDate, DateTime endDate);
        Task<OrderEntity?> UpdateAddressOrder(int orderId, int addressId);
        Task<OrderEntity?> UpdateOrderStatus(int orderId, OrderStatus newStatus);
        Task<OrderEntity?> UpdateAmountOrder(int orderId, decimal amount);
    }
}