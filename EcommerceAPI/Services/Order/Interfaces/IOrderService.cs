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
        /// Deletes an order by its identifier.
        /// </summary>
        /// <param name="orderId">The identifier of the order to be deleted.</param>
        /// <returns>True if the order was successfully deleted, otherwise false.</returns>
        Task<bool> DeleteOrder(int orderId);

        /// <summary>
        /// Gets the orders.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A paginated list of orders matching the provided filters as <see cref="PagedResult{OrderDto}"/>.</returns>
        /// <exception cref="System.UnauthorizedAccessException">You do not have permission to access orders for this user.</exception>
        Task<PagedResult<OrderDto>> GetOrders(int userId, UserRole role, OrderQueryParameters parameters);

        /// <summary>
        /// Gets the seller orders.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A paginated list of orders for a specific seller as <see cref="PagedResult{OrderDto}"/>.</returns>
        /// <exception cref="System.UnauthorizedAccessException">
        /// You do not have permission to access orders for this seller.
        /// or
        /// You do not have permission to access seller orders.
        /// </exception>
        Task<PagedResult<OrderDto>> GetSellerOrders(int userId, UserRole role, OrderSellerQueryParameters parameters);

        /// <summary>
        /// Gets the order with details.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Order not found</exception>
        Task<OrderDto> GetOrderWithDetails(int orderId);

        /// <summary>
        /// Gets the order by identifier.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Order not found</exception>
        /// <exception cref="System.UnauthorizedAccessException">You do not have permission to access this order.</exception>
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
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">You do not have permission to update this order</exception>
        /// <exception cref="System.InvalidOperationException">Only draft orders can have their shipping address updated</exception>
        Task<OrderDto> UpdateOrderAddress(int userId, int orderId, int addressId);

        /// <summary>
        /// Updates the order status.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="newStatus">The new status.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Order not found</exception>
        /// <exception cref="System.UnauthorizedAccessException">You do not have permission to update order status</exception>
        Task<OrderDto> UpdateOrderStatus(int userId, UserRole role, int orderId, OrderStatus newStatus);
    }
}