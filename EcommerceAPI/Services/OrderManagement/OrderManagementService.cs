using AutoMapper;
using EcommerceAPI.Models.DTOs.Order;
using EcommerceAPI.Models.DTOs.OrderDetail;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.Order.Interfaces;
using EcommerceAPI.Services.OrderItem;
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
        /// Creates the order with details.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="orderDetails">The order details.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Order details cannot be null or empty. - orderDetails</exception>
        /// <exception cref="System.InvalidOperationException">
        /// Failed to create order.
        /// or
        /// Failed to add order details.
        /// or
        /// Failed to retrieve the newly created order.
        /// </exception>
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
        public async Task<OrderDto> AddItemsToOrder(int orderId, IEnumerable<OrderDetailAddDto> orderDetails)
        {
            if (orderDetails == null || !orderDetails.Any())
                throw new ArgumentException("Order details cannot be null or empty.", nameof(orderDetails));

            var order = await _orderService.GetOrderById(orderId);

            if (order == null)
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");

            var orderDetailEntities = _mapper.Map<IEnumerable<OrderDetailEntity>>(orderDetails);
            var addOrderDetailResult = await _orderItemService.AddOrderDetails(orderId, orderDetailEntities);

            if (addOrderDetailResult == null)
                throw new InvalidOperationException("Failed to add order details.");

            var updatedOrder = await _orderService.GetOrderWithDetails(orderId);

            if (updatedOrder == null)
                throw new InvalidOperationException("Failed to retrieve the updated order.");

            return updatedOrder;
        }
    }
}