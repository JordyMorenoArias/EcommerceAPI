using EcommerceAPI.Models.DTOs.OrderDetail;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.OrderItem.Interfaces
{
    /// <summary>
    /// Interface for managing order details, including adding new order details and retrieving specific details by identifier.
    /// </summary>
    public interface IOrderDetailService
    {
        /// <summary>
        /// Adds the details of an order to an existing order.
        /// </summary>
        /// <param name="orderId">The identifier of the order to which the details will be added.</param>
        /// <param name="orderDetails">The collection of order details to be added.</param>
        /// <returns>The result of the operation, containing information about the added order details.</returns>
        Task<AddOrderDetailResultDto> AddOrderDetails(int orderId, IEnumerable<OrderDetailEntity> orderDetails);

        /// <summary>
        /// Retrieves the details of a specific order detail by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the order detail to be retrieved.</param>
        /// <returns>The details of the order, if found, otherwise null.</returns>
        Task<OrderDetailDto> GetOrderDetailById(int id);
    }
}