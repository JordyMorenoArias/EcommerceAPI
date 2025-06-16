using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs.Generic;
using EcommerceAPI.Models.DTOs.Order;

namespace EcommerceAPI.Services.Order.Interfaces
{
    /// <summary>
    /// Interface for managing orders, including creating, updating, deleting, and retrieving orders.
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Adds a new order for the specified user.
        /// </summary>
        /// <param name="userId">The identifier of the user who is placing the order.</param>
        /// <returns>The created order.</returns>
        Task<OrderDto> AddOrder(int userId);


        /// <summary>
        /// Gets the orders.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A paginated list of orders matching the provided filters as <see cref="PagedResult{OrderDto}"/>.</returns>
        Task<PagedResult<OrderDto>> GetOrders(int userId, UserRole role, OrderQueryParameters parameters);

        /// <summary>
        /// Gets the seller orders.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A paginated list of orders for a specific seller as <see cref="PagedResult{OrderDto}"/>.</returns>
        Task<PagedResult<OrderDto>> GetSellerOrders(int userId, UserRole role, OrderSellerQueryParameters parameters);

        /// <summary>
        /// Gets the order with details.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <returns>A task that represents the asynchronous operation.The task result contains the OrderDto with order details.</returns>
        Task<OrderDto> GetOrderWithDetails(int userId, UserRole role, int orderId);

        /// <summary>
        /// Gets the order by identifier.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the OrderDto for the specified order ID.</returns>
        Task<OrderDto> GetOrderById(int id);

        /// <summary>
        /// Updates the address order.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="addressId">The address identifier.</param>
        /// <returns>The updated order with the new shipping address as <see cref="OrderDto"/>.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">
        /// Order not found
        /// or
        /// Address not found
        Task<OrderDto> UpdateOrderAddress(int userId, int orderId, int addressId);

        /// <summary>
        /// Updates the order status.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="newStatus">The new status.</param>
        /// <returns>A task that represents the asynchronous operation.The task result contains the updated OrderDto.</returns>
        Task<OrderDto> UpdateOrderStatus(int userId, UserRole role, int orderId, OrderStatus newStatus);

        /// <summary>
        /// Deletes the order.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        /// <param name="orderId">The order identifier.</param>
        ///  <returns>A task that represents the asynchronous operation. The task result contains an OperationResult object indicating the success or failure of the operation.</returns>
        Task<OperationResult> DeleteOrder(int userId, UserRole role, int orderId);
    }
}