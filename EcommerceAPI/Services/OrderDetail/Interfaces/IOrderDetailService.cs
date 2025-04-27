using EcommerceAPI.Models.DTOs.OrderDetail;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.OrderItem.Interfaces
{
    public interface IOrderDetailService
    {
        Task<AddOrderDetailResultDto> AddOrderDetails(int orderId, IEnumerable<OrderDetailEntity> orderDetails);
        Task<OrderDetailDto> GetOrderDetailByI(int id);
    }
}