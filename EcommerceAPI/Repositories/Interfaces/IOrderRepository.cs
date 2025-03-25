using EcommerceAPI.Constants;
using EcommerceAPI.Models;

namespace EcommerceAPI.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> AddOrder(Order order);
        Task<bool> DeleteOrder(int orderId);
        Task<IEnumerable<Order>> GetAllOrders();
        Task<IEnumerable<Order>> GetAllOrdersByUserId(int userId);
        Task<Order?> GetOrderById(int id);
        Task<IEnumerable<Order>> GetOrdersByStatus(OrderStatus status);
        Task<Order?> GetOrderWithDetails(int orderId);
        Task<decimal> GetTotalSales();
        Task<bool> UpdateOrder(Order order);
        Task<bool> UpdateOrderStatus(int orderId, OrderStatus newStatus);
    }
}