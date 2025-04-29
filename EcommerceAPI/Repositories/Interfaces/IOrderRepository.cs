using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs;
using EcommerceAPI.Models.DTOs.Order;
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
        Task<PagedResult<OrderEntity>> GetOrders(OrderQueryParameters parameters);
        Task<PagedResult<OrderEntity>> GetSellerOrders(OrderSellerQueryParameters parameters);
    }
}