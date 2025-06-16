using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs.Generic;
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

        /// <summary>
        /// Updates the order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns>The updated <see cref="OrderEntity"/> after saving changes to the database.</returns>
        Task<OrderEntity> UpdateOrder(OrderEntity order);

        /// <summary>
        /// Deletes the order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns>A boolean indicating whether the deletion was successful (<c>true</c>) or not (<c>false</c>).</returns>
        Task<bool> DeleteOrder(OrderEntity order);
    }
}