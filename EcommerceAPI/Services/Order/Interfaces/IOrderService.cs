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
        /// Updates the address of an order.
        /// </summary>
        /// <param name="orderId">The identifier of the order to be updated.</param>
        /// <param name="addressId">The identifier of the new address for the order.</param>
        /// <returns>The updated order.</returns>
        Task<OrderDto> UpdateAddressOrder(int orderId, int addressId);

        /// <summary>
        /// Updates the status of an order.
        /// </summary>
        /// <param name="orderId">The identifier of the order to be updated.</param>
        /// <param name="newStatus">The new status for the order.</param>
        /// <returns>The updated order.</returns>
        Task<OrderDto> UpdateOrderStatus(int orderId, OrderStatus newStatus);

        /// <summary>
        /// Gets the order by identifier.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <param name="role">The role.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Order not found</exception>
        /// <exception cref="System.UnauthorizedAccessException">You do not have permission to access this order.</exception>
        Task<OrderDto?> GetOrderById(int userid, UserRole role, int id);

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
        /// <param name="role">The role.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Order not found</exception>
        Task<OrderDto?> GetOrderWithDetails(int userId, UserRole role, int orderId);
    }
}