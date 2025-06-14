using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs.Generic;
using EcommerceAPI.Models.DTOs.Order;
using EcommerceAPI.Models.DTOs.OrderDetail;

namespace EcommerceAPI.Services.OrderManagement.Interfaces
{
    /// <summary>
    /// Interface for managing orders and related operations such as adding details, creating orders, and updating statuses.
    /// </summary>
    public interface IOrderManagementService
    {
        /// <summary>
        /// Adds the order detail to order.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="orderDetails">The order details.</param>
        /// <returns>
        /// The updated order with the added details.
        /// </returns>
        Task<OrderDto> AddOrderDetailToOrder(int userId, UserRole role, int orderId, IEnumerable<OrderDetailAddDto> orderDetails);

        /// <summary>
        /// Creates the order with details.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        /// <param name="orderDetails">The order details.</param>
        /// <returns>
        /// The created order with its details.
        /// </returns>
        Task<OrderDto> CreateOrderWithDetails(int userId, UserRole role, IEnumerable<OrderDetailAddDto> orderDetails);

        /// <summary>
        /// Deletes the order.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains an OperationResult object indicating the success or failure of the operation.
        /// </returns>
        Task<OperationResult> DeleteOrder(int userId, UserRole role, int orderId);

        /// <summary>
        /// Gets a paginated list of orders based on query parameters.
        /// </summary>
        /// <param name="userId">The identifier of the user requesting the orders.</param>
        /// <param name="userRole">The role of the user making the request (e.g., Admin, Customer, etc.).</param>
        /// <param name="parameters">The query parameters for filtering and paginating the orders.</param>
        /// <returns>A paginated result containing the orders matching the query parameters.</returns>
        Task<PagedResult<OrderDto>> GetOrders(int userId, UserRole userRole, OrderQueryParameters parameters);

        /// <summary>
        /// Retrieves an order with its associated details.
        /// </summary>
        /// <param name="userId">The identifier of the user requesting the order.</param>
        /// <param name="role">The role of the user (e.g., Admin, Customer, etc.).</param>
        /// <param name="orderId">The identifier of the order to retrieve.</param>
        /// <returns>The order with its associated details.</returns>
        Task<OrderDto> GetOrderWithDetails(int userId, UserRole role, int orderId);

        /// <summary>
        /// Gets a paginated list of orders for a seller based on query parameters.
        /// </summary>
        /// <param name="userId">The identifier of the user requesting the orders.</param>
        /// <param name="userRole">The role of the user making the request (e.g., Seller, Admin, etc.).</param>
        /// <param name="parameters">The query parameters for filtering and paginating the seller's orders.</param>
        /// <returns>A paginated result containing the seller's orders.</returns>
        Task<PagedResult<OrderDto>> GetSellerOrders(int userId, UserRole userRole, OrderSellerQueryParameters parameters);

        /// <summary>
        /// Updates the shipping address for an order.
        /// </summary>
        /// <param name="userId">The identifier of the user requesting the update.</param>
        /// <param name="orderId">The identifier of the order for which the address is being updated.</param>
        /// <param name="addressId">The identifier of the new address to be set for the order.</param>
        /// <returns>The updated order with the new address.</returns>
        Task<OrderDto> UpdateOrderAddress(int userId, int orderId, int addressId);

        /// <summary>
        /// Updates the order status.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="newStatus">The new status.</param>
        /// <returns>The updated order with the new status.</returns>
        Task<OrderDto> UpdateOrderStatus(int userId, UserRole role, int orderId, OrderStatus newStatus);
    }
}