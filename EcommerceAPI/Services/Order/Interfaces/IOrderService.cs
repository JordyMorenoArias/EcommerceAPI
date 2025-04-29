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
        Task<OrderDto?> GetOrderWithDetails(int orderId);
        Task<PagedResult<OrderDto>> GetOrders(OrderQueryParameters parameters);
        Task<PagedResult<OrderDto>> GetSellerOrders(OrderSellerQueryParameters parameters);
        Task<OrderDto> UpdateAddressOrder(int orderId, int addressId);
        Task<OrderDto> UpdateAmountOrder(int orderId, decimal amount);
        Task<OrderDto> UpdateOrderStatus(int orderId, OrderStatus newStatus);
    }
}