using AutoMapper;
using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs;
using EcommerceAPI.Models.DTOs.Order;
using EcommerceAPI.Models.DTOs.OrderDetail;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.Order.Interfaces;
using EcommerceAPI.Services.OrderItem.Interfaces;
using EcommerceAPI.Services.OrderManagement.Interfaces;

namespace EcommerceAPI.Services.Order
{
    /// <summary>
    /// Service for managing orders and their details in the e-commerce system.
    /// </summary>
    /// <seealso cref="EcommerceAPI.Services.OrderManagement.Interfaces.IOrderManagementService" />
    public class OrderManagementService : IOrderManagementService
    {
        private readonly IOrderDetailService _orderItemService;
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrderManagementService(IOrderDetailService orderDetailService, IOrderService orderService, IMapper mapper)
        {
            _orderItemService = orderDetailService;
            _orderService = orderService;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets the order with details and products.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Order with ID {orderId} not found.</exception>
        public async Task<OrderDto> GetOrderWithDetails(int userId, UserRole role, int orderId)
        {
            var order = await _orderService.GetOrderWithDetails(orderId);

            if (order == null)
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");

            if (role != UserRole.Admin && order.UserId != userId)
                throw new InvalidOperationException("You do not have permission to access this order.");

            return order;
        }

        /// <summary>
        /// Gets the orders by date range.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns></returns>
        public async Task<PagedResult<OrderDto>> GetOrdersByDateRange(int page, int pageSize, DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _orderService.GetOrdersByDateRange(page, pageSize, startDate, endDate);
        }

        /// <summary>
        /// Gets the orders by user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns></returns>
        public async Task<PagedResult<OrderDto>> GetOrdersByUserId(int userId, int page, int pageSize, DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _orderService.GetOrdersByUserId(userId, page, pageSize, startDate, endDate);
        }

        /// <summary>
        /// Gets the orders by status.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns></returns>
        public async Task<PagedResult<OrderDto>> GetOrdersByStatus(OrderStatus status, int page, int pageSize, DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _orderService.GetOrdersByStatus(status, page, pageSize, startDate, endDate);
        }

        /// <summary>
        /// Gets the orders by seller.
        /// </summary>
        /// <param name="sellerId">The seller identifier.</param>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns></returns>
        public async Task<PagedResult<OrderDto>> GetOrdersBySeller(int sellerId, int page, int pageSize, DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _orderService.GetOrdersBySeller(sellerId, page, pageSize, startDate, endDate);
        }

        /// <summary>Creates the order with details.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="orderDetails">The order details.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        /// <exception cref="System.ArgumentException">Order details cannot be null or empty. - orderDetails</exception>
        /// <exception cref="System.InvalidOperationException">Failed to create order.
        /// or
        /// Failed to add order details.
        /// or
        /// Failed to retrieve the newly created order.</exception>
        public async Task<OrderDto> CreateOrderWithDetails(int userId, IEnumerable<OrderDetailAddDto> orderDetails)
        {
            if (orderDetails == null || !orderDetails.Any())
                throw new ArgumentException("Order details cannot be null or empty.", nameof(orderDetails));

            var order = await _orderService.AddOrder(userId);

            if (order == null)
                throw new InvalidOperationException("Failed to create order.");

            var orderDetailEntities = _mapper.Map<IEnumerable<OrderDetailEntity>>(orderDetails);
            var addOrderDetailResult = await _orderItemService.AddOrderDetails(order.Id, orderDetailEntities);

            if (addOrderDetailResult == null)
                throw new InvalidOperationException("Failed to add order details.");

            order = await _orderService.GetOrderWithDetails(order.Id);

            if (order == null)
                throw new InvalidOperationException("Failed to retrieve the newly created order.");

            return order;
        }

        /// <summary>
        /// Adds the items to order.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="orderDetails">The order details.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Order details cannot be null or empty. - orderDetails</exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Order with ID {orderId} not found.</exception>
        /// <exception cref="System.InvalidOperationException">
        /// Failed to add order details.
        /// or
        /// Failed to retrieve the updated order.
        /// </exception>
        public async Task<OrderDto> AddOrderDetailToOrder(int userId, int orderId, IEnumerable<OrderDetailAddDto> orderDetails)
        {
            if (orderDetails == null || !orderDetails.Any())
                throw new ArgumentException("Order details cannot be null or empty.", nameof(orderDetails));

            var order = await _orderService.GetOrderById(orderId);

            if (order == null)
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");

            if (order.UserId != userId)
                throw new InvalidOperationException("You do not have permission to modify this order.");

            var orderDetailEntities = _mapper.Map<IEnumerable<OrderDetailEntity>>(orderDetails);
            var addOrderDetailResult = await _orderItemService.AddOrderDetails(orderId, orderDetailEntities);

            if (addOrderDetailResult == null)
                throw new InvalidOperationException("Failed to add order details.");

            var updatedOrder = await _orderService.GetOrderWithDetails(orderId);

            if (updatedOrder == null)
                throw new InvalidOperationException("Failed to retrieve the updated order.");

            return updatedOrder;
        }

        /// <summary>
        /// Updates the order status.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="newStatus">The new status.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Order with ID {orderId} not found.</exception>
        public async Task<OrderDto> UpdateOrderStatus(int orderId, OrderStatus newStatus)
        {
            var order = await _orderService.GetOrderById(orderId);

            if (order == null)
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");

            return await _orderService.UpdateOrderStatus(order.Id, newStatus);
        }

        /// <summary>
        /// Updates the order address.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="addressId">The address identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Order with ID {orderId} not found.</exception>
        public async Task<OrderDto> UpdateOrderAddress(int userId, int orderId, int addressId)
        {
            var order = await _orderService.GetOrderById(orderId);

            if (order == null)
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");

            if (order.UserId != userId)
                throw new InvalidOperationException("You do not have permission to modify this order.");

            if (order.Status != OrderStatus.Draft)
                throw new InvalidOperationException("Only draft orders can have their address updated.");

            return await _orderService.UpdateAddressOrder(order.Id, addressId);
        }

        /// <summary>
        /// Deletes the order.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Order with ID {orderId} not found.</exception>
        /// <exception cref="System.InvalidOperationException">Only draft orders can be deleted.</exception>
        public async Task<bool> DeleteOrder(int orderId)
        {
            var order = await _orderService.GetOrderById(orderId);

            if (order == null)
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");

            if (order.Status != OrderStatus.Draft)
                throw new InvalidOperationException("Only draft orders can be deleted.");

            return await _orderService.DeleteOrder(order.Id);
        }
    }
}