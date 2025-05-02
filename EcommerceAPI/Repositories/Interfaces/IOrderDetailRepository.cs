using EcommerceAPI.Models.DTOs.OrderDetail;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Repositories.Interfaces
{
    /// <summary>
    /// Interface for order detail repository operations.
    /// </summary>
    public interface IOrderDetailRepository
    {
        /// <summary>
        /// Adds multiple order details to a specific order in the repository.
        /// </summary>
        /// <param name="orderId">The ID of the order to which the details will be added.</param>
        /// <param name="orderDetails">The collection of order details to be added.</param>
        /// <returns>A result indicating the success of the operation and details of the added order details.</returns>
        Task<AddOrderDetailResultDto> AddOrderDetails(int orderId, IEnumerable<OrderDetailEntity> orderDetails);

        /// <summary>
        /// Retrieves an order detail by its ID from the repository.
        /// </summary>
        /// <param name="id">The ID of the order detail to retrieve.</param>
        /// <returns>The order detail entity if found, otherwise null.</returns>
        Task<OrderDetailEntity?> GetOrderDetailById(int id);
    }
}