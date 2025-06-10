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
        /// Retrieves an order by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the order to be retrieved.</param>
        /// <returns>The order with the specified identifier, or null if not found.</returns>
        Task<OrderDto?> GetOrderById(int id);

        /// <summary>
        /// Retrieves an order with its details by its identifier.
        /// </summary>
        /// <param name="orderId">The identifier of the order to be retrieved.</param>
        /// <returns>The order with the specified identifier and its details, or null if not found.</returns>
        Task<OrderDto?> GetOrderWithDetails(int orderId);

        /// <summary>
        /// Retrieves a paged list of orders based on the provided query parameters.
        /// </summary>
        /// <param name="parameters">The query parameters for retrieving orders.</param>
        /// <returns>A paged result containing the orders matching the query parameters.</returns>
        Task<PagedResult<OrderDto>> GetOrders(OrderQueryParameters parameters);

        /// <summary>
        /// Retrieves a paged list of orders for a seller based on the provided query parameters.
        /// </summary>
        /// <param name="parameters">The query parameters for retrieving seller orders.</param>
        /// <returns>A paged result containing the seller's orders matching the query parameters.</returns>
        Task<PagedResult<OrderDto>> GetSellerOrders(OrderSellerQueryParameters parameters);

        /// <summary>
        /// Updates the address of an order.
        /// </summary>
        /// <param name="orderId">The identifier of the order to be updated.</param>
        /// <param name="addressId">The identifier of the new address for the order.</param>
        /// <returns>The updated order.</returns>
        Task<OrderDto> UpdateAddressOrder(int orderId, int addressId);

        /// <summary>
        /// Updates the total amount of an order.
        /// </summary>
        /// <param name="orderId">The identifier of the order to be updated.</param>
        /// <param name="amount">The new total amount for the order.</param>
        /// <returns>The updated order.</returns>
        Task<OrderDto> UpdateAmountOrder(int orderId, decimal amount);

        /// <summary>
        /// Updates the status of an order.
        /// </summary>
        /// <param name="orderId">The identifier of the order to be updated.</param>
        /// <param name="newStatus">The new status for the order.</param>
        /// <returns>The updated order.</returns>
        Task<OrderDto> UpdateOrderStatus(int orderId, OrderStatus newStatus);
    }
}