using EcommerceAPI.Constants;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<OrderEntity?> AddOrder(OrderEntity order);
        Task<bool> DeleteOrder(int orderId);
        Task<IEnumerable<OrderEntity>> GetAllOrders();
        Task<IEnumerable<OrderEntity>> GetAllOrdersByUserId(int userId);
        Task<OrderEntity?> GetOrderById(int id);
        Task<IEnumerable<OrderEntity>> GetOrdersByStatus(OrderStatus status);
        Task<OrderEntity?> GetOrderWithDetails(int orderId);
        Task<decimal> GetTotalSales();
        Task<bool> UpdateOrder(OrderEntity order);
        Task<bool> UpdateOrderStatus(int orderId, OrderStatus newStatus);
    }
}