using AutoMapper;
using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs.Generic;
using EcommerceAPI.Models.DTOs.Order;
using EcommerceAPI.Models.DTOs.OrderDetail;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.Order.Interfaces;
using EcommerceAPI.Services.OrderItem.Interfaces;
using EcommerceAPI.Services.OrderManagement.Interfaces;
using StackExchange.Redis;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderManagementService"/> class.
        /// </summary>
        /// <param name="orderDetailService">The order detail service.</param>
        /// <param name="orderService">The order service.</param>
        /// <param name="mapper">The mapper.</param>
        public OrderManagementService(IOrderDetailService orderDetailService, IOrderService orderService, IMapper mapper)
        {
            _orderItemService = orderDetailService;
            _orderService = orderService;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves an order with its associated details.
        /// </summary>
        /// <param name="userId">The identifier of the user requesting the order.</param>
        /// <param name="role">The role of the user (e.g., Admin, Customer, etc.).</param>
        /// <param name="orderId">The identifier of the order to retrieve.</param>
        /// <returns>
        /// The order with its associated details.
        /// </returns>
        public async Task<OrderDto> GetOrderWithDetails(int userId, UserRole role, int orderId)
        => await _orderService.GetOrderWithDetails(userId, role, orderId);

        /// <summary>
        /// Gets the orders.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Paged result of orders.</returns>
        public async Task<PagedResult<OrderDto>> GetOrders(int userId, UserRole role, OrderQueryParameters parameters)
            => await _orderService.GetOrders(userId, role, parameters);

        /// <summary>
        /// Gets the seller orders.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Paged result of seller orders.</returns>
        public async Task<PagedResult<OrderDto>> GetSellerOrders(int userId, UserRole role, OrderSellerQueryParameters parameters)
            => await _orderService.GetSellerOrders(userId, role, parameters);

        /// <summary>
        /// Creates the order with details.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        /// <param name="orderDetails">The order details.</param>
        /// <returns>
        /// The created order with its details.
        /// </returns>
        /// <exception cref="System.ArgumentException">Order details cannot be null or empty. - orderDetails</exception>
        public async Task<OrderDto> CreateOrderWithDetails(int userId, UserRole role,  IEnumerable<OrderDetailAddDto> orderDetails)
        {
            if (orderDetails == null || !orderDetails.Any())
                throw new ArgumentException("Order details cannot be null or empty.", nameof(orderDetails));

            var order = await _orderService.AddOrder(userId);
            var orderDetailEntities = _mapper.Map<IEnumerable<OrderDetailEntity>>(orderDetails);
            await _orderItemService.AddOrderDetails(order.Id, orderDetailEntities);

            order = await _orderService.GetOrderWithDetails(userId, role, order.Id);
            return order;
        }

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
        /// <exception cref="System.InvalidOperationException">You do not have permission to modify this order.</exception>
        public async Task<OrderDto> AddOrderDetailToOrder(int userId, UserRole role, int orderId, IEnumerable<OrderDetailAddDto> orderDetails)
        {
            var order = await _orderService.GetOrderById(orderId);

            if (order.UserId != userId)
                throw new InvalidOperationException("You do not have permission to modify this order.");

            var orderDetailEntities = _mapper.Map<IEnumerable<OrderDetailEntity>>(orderDetails);
            var addOrderDetailResult = await _orderItemService.AddOrderDetails(orderId, orderDetailEntities);

            var updatedOrder = await _orderService.GetOrderWithDetails(userId, role, order.Id);
            return updatedOrder;
        }

        /// <summary>
        /// Updates the order status.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="newStatus">The new status.</param>
        /// <returns>
        /// The updated order with the new status.
        /// </returns>
        public async Task<OrderDto> UpdateOrderStatus(int userId, UserRole role, int orderId, OrderStatus newStatus) 
            => await _orderService.UpdateOrderStatus(userId, role, orderId, newStatus);


        /// <summary>
        /// Updates the shipping address for an order.
        /// </summary>
        /// <param name="userId">The identifier of the user requesting the update.</param>
        /// <param name="orderId">The identifier of the order for which the address is being updated.</param>
        /// <param name="addressId">The identifier of the new address to be set for the order.</param>
        /// <returns>
        /// The updated order with the new address.
        /// </returns>
        public async Task<OrderDto> UpdateOrderAddress(int userId, int orderId, int addressId) 
            => await _orderService.UpdateOrderAddress(userId, orderId, addressId);

        /// <summary>
        /// Deletes the order.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains an OperationResult object indicating the success or failure of the operation.
        /// </returns>
        public async Task<OperationResult> DeleteOrder(int userId, UserRole role, int orderId)
        => await _orderService.DeleteOrder(userId, role, orderId);
    }
}