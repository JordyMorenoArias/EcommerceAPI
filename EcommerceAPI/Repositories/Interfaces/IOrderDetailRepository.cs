using EcommerceAPI.Models.DTOs.OrderDetail;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Repositories.Interfaces
{
    public interface IOrderDetailRepository
    {
        Task<AddOrderDetailResultDto?> AddOrderDetails(int orderId, IEnumerable<OrderDetailEntity> orderDetails);
        Task<OrderDetailEntity?> GetOrderDetailById(int id);
    }
}