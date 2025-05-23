﻿using AutoMapper;
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
        /// Gets the order with details.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <returns>The order with its details.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Order with ID {orderId} not found.</exception>
        /// <exception cref="System.InvalidOperationException">You do not have permission to access this order.</exception>
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
        /// Gets the orders.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The user role.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Paged result of orders.</returns>
        /// <exception cref="System.ArgumentException">
        /// Page and PageSize must be greater than 0.
        /// or
        /// StartDate must be less than or equal to EndDate.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">You do not have permission to view orders for this user.</exception>
        public async Task<PagedResult<OrderDto>> GetOrders(int userId, UserRole role, OrderQueryParameters parameters)
        {
            if (parameters.Page <= 0 || parameters.PageSize <= 0)
                throw new ArgumentException("Page and PageSize must be greater than 0.");

            if (parameters.StartDate.HasValue && parameters.EndDate.HasValue && parameters.StartDate > parameters.EndDate)
                throw new ArgumentException("StartDate must be less than or equal to EndDate.");

            if (role == UserRole.Customer && userId != parameters.UserId)
            {
                throw new InvalidOperationException("You do not have permission to view orders for this user.");
            }
            else if (role == UserRole.Seller && parameters.UserId != userId)
            {
                throw new InvalidOperationException("You do not have permission to view orders for this user.");
            }

            var orders = await _orderService.GetOrders(parameters);
            return orders;
        }

        /// <summary>
        /// Gets the seller orders.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The user role.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Paged result of seller orders.</returns>
        /// <exception cref="System.ArgumentException">
        /// Page and PageSize must be greater than 0.
        /// or
        /// StartDate must be less than or equal to EndDate.
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">Customers are not allowed to view seller orders.</exception>
        /// <exception cref="System.InvalidOperationException">You do not have permission to view orders for this seller.</exception>
        public async Task<PagedResult<OrderDto>> GetSellerOrders(int userId, UserRole role, OrderSellerQueryParameters parameters)
        {
            if (parameters.Page <= 0 || parameters.PageSize <= 0)
                throw new ArgumentException("Page and PageSize must be greater than 0.");

            if (parameters.StartDate.HasValue && parameters.EndDate.HasValue && parameters.StartDate > parameters.EndDate)
                throw new ArgumentException("StartDate must be less than or equal to EndDate.");

            if (role == UserRole.Customer)
            {
                throw new UnauthorizedAccessException("Customers are not allowed to view seller orders.");
            }
            else if (role == UserRole.Seller && parameters.SellerId != userId)
            {
                throw new InvalidOperationException("You do not have permission to view orders for this seller.");
            }

            var orders = await _orderService.GetSellerOrders(parameters);
            return orders;
        }

        /// <summary>
        /// Creates the order with details.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="orderDetails">The order details.</param>
        /// <returns>The newly created order with details.</returns>
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
        /// Adds the order detail to order.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="orderDetails">The order details.</param>
        /// <returns>The updated order with the added details.</returns>
        /// <exception cref="System.ArgumentException">Order details cannot be null or empty. - orderDetails</exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Order with ID {orderId} not found.</exception>
        /// <exception cref="System.InvalidOperationException">
        /// You do not have permission to modify this order.
        /// or
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
        /// <returns>The updated order with the new status.</returns>
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
        /// <returns>The updated order with the new address.</returns>
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
        /// <returns>True if the deletion was successful; otherwise, false.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Order with ID {orderId} not found.</exception>
        /// <exception cref="System.InvalidOperationException">Only draft orders can be deleted.</exception>
        public async Task<bool> DeleteOrder(int userId, int orderId)
        {
            var order = await _orderService.GetOrderById(orderId);

            if (order == null)
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");

            if (order.UserId != userId)
                throw new InvalidOperationException("You do not have permission to delete this order.");

            if (order.Status != OrderStatus.Draft)
                throw new InvalidOperationException("Only draft orders can be deleted.");

            return await _orderService.DeleteOrder(order.Id);
        }
    }
}