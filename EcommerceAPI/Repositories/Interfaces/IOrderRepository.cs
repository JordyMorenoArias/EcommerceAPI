using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs;
using EcommerceAPI.Models.DTOs.Order;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Repositories.Interfaces
{
    /// <summary>
    /// Interface for order repository operations.
    /// </summary>
    public interface IOrderRepository
    {
        /// <summary>
        /// Adds a new order to the repository.
        /// </summary>
        /// <param name="order">The order entity to be added.</param>
        /// <returns>The added order entity.</returns>
        Task<OrderEntity> AddOrder(OrderEntity order);

        /// <summary>
        /// Deletes an order from the repository by its ID.
        /// </summary>
        /// <param name="orderId">The ID of the order to be deleted.</param>
        /// <returns>True if the order was successfully deleted, otherwise false.</returns>
        Task<bool> DeleteOrder(int orderId);

        /// <summary>
        /// Retrieves an order by its ID from the repository.
        /// </summary>
        /// <param name="id">The ID of the order to retrieve.</param>
        /// <returns>The order entity if found, otherwise null.</returns>
        Task<OrderEntity?> GetOrderById(int id);

        /// <summary>
        /// Retrieves an order along with its details from the repository.
        /// </summary>
        /// <param name="orderId">The ID of the order to retrieve along with details.</param>
        /// <returns>The order entity with its details if found, otherwise null.</returns>
        Task<OrderEntity?> GetOrderWithDetails(int orderId);

        /// <summary>
        /// Updates the address associated with an order.
        /// </summary>
        /// <param name="orderId">The ID of the order to update.</param>
        /// <param name="addressId">The ID of the new address.</param>
        /// <returns>The updated order entity with the new address.</returns>
        Task<OrderEntity?> UpdateAddressOrder(int orderId, int addressId);

        /// <summary>
        /// Updates the status of an order.
        /// </summary>
        /// <param name="orderId">The ID of the order to update.</param>
        /// <param name="newStatus">The new status to set for the order.</param>
        /// <returns>The updated order entity with the new status.</returns>
        Task<OrderEntity?> UpdateOrderStatus(int orderId, OrderStatus newStatus);

        /// <summary>
        /// Updates the amount of an order.
        /// </summary>
        /// <param name="orderId">The ID of the order to update.</param>
        /// <param name="amount">The new amount to set for the order.</param>
        /// <returns>The updated order entity with the new amount.</returns>
        Task<OrderEntity?> UpdateAmountOrder(int orderId, decimal amount);

        /// <summary>
        /// Retrieves a paginated list of orders based on the provided parameters.
        /// </summary>
        /// <param name="parameters">The query parameters to filter and paginate the orders.</param>
        /// <returns>A paginated result set of orders.</returns>
        Task<PagedResult<OrderEntity>> GetOrders(OrderQueryParameters parameters);

        /// <summary>
        /// Retrieves a paginated list of orders for a specific seller based on the provided parameters.
        /// </summary>
        /// <param name="parameters">The query parameters to filter and paginate the orders for the seller.</param>
        /// <returns>A paginated result set of orders for the seller.</returns>
        Task<PagedResult<OrderEntity>> GetSellerOrders(OrderSellerQueryParameters parameters);
    }
}