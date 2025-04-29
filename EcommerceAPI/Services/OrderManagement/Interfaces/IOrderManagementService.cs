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
        Task<bool> DeleteOrder(int userId, int orderId);
        Task<PagedResult<OrderDto>> GetOrders(int userId, UserRole userRole, OrderQueryParameters parameters);
        Task<OrderDto> GetOrderWithDetails(int userId, UserRole role, int orderId);
        Task<PagedResult<OrderDto>> GetSellerOrders(int userId, UserRole userRole, OrderSellerQueryParameters parameters);
        Task<OrderDto> UpdateOrderAddress(int userId, int orderId, int addressId);
        Task<OrderDto> UpdateOrderStatus(int orderId, OrderStatus newStatus);
    }
}