using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<OrderEntity> AddOrder(OrderEntity order);
        Task<bool> DeleteOrder(int orderId);
        Task<OrderEntity?> GetOrderById(int id);
        Task<OrderEntity?> GetOrderWithDetails(int orderId);
        Task<OrderEntity?> UpdateAddressOrder(int orderId, int addressId);
        Task<OrderEntity?> UpdateOrderStatus(int orderId, OrderStatus newStatus);
        Task<OrderEntity?> UpdateAmountOrder(int orderId, decimal amount);
        Task<PagedResult<OrderEntity>> GetOrdersByUserId(int userId, int page, int pageSize, DateTime? startDate = null, DateTime? endDate = null);
        Task<PagedResult<OrderEntity>> GetOrdersByStatus(OrderStatus status, int page, int pageSize, DateTime? startDate = null, DateTime? endDate = null);
        Task<PagedResult<OrderEntity>> GetOrdersBySeller(int sellerId, int page, int pageSize, DateTime? startDate = null, DateTime? endDate = null);
        Task<PagedResult<OrderEntity>> GetOrdersByDateRange(int page, int pageSize, DateTime? startDate = null, DateTime? endDate = null);
    }
}