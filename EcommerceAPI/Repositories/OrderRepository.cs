using AutoMapper;
using EcommerceAPI.Constants;
using EcommerceAPI.Data;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EcommerceAPI.Repositories
{
    /// <summary>
    /// Repository for managing orders in the e-commerce system.
    /// </summary>
    public class OrderRepository : IOrderRepository
    {
        private readonly EcommerceContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderRepository"/> class.
        /// </summary>
        /// <param name="context">Database context for e-commerce operations.</param>
        public OrderRepository(EcommerceContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all orders with user and shipping address details.
        /// </summary>
        /// <returns>A collection of all orders.</returns>
        public async Task<IEnumerable<OrderEntity>> GetOrders()
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.ShippingAddress)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves an order by its unique identifier.
        /// </summary>
        /// <param name="id">Order ID.</param>
        /// <returns>The order if found; otherwise, null.</returns>
        public async Task<OrderEntity?> GetOrderById(int id)
        {
            return await _context.Orders.FindAsync(id);
        }

        /// <summary>
        /// Retrieves all orders associated with a specific user.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <returns>A collection of orders for the specified user.</returns>
        public async Task<IEnumerable<OrderEntity>> GetOrdersByUserId(int userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.User)
                .Include(o => o.ShippingAddress)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all orders with a specific status.
        /// </summary>
        /// <param name="status">Order status.</param>
        /// <returns>A collection of orders matching the status.</returns>
        public async Task<IEnumerable<OrderEntity>> GetOrdersByStatus(OrderStatus status)
        {
            return await _context.Orders
                .Where(o => o.Status == status)
                .Include(o => o.ShippingAddress)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves an order along with its related user, shipping address, and order details.
        /// </summary>
        /// <param name="orderId">Order ID.</param>
        /// <returns>The order with its details if found; otherwise, null.</returns>
        public async Task<OrderEntity?> GetOrderWithDetails(int orderId)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.ShippingAddress)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        /// <summary>
        /// Adds a new order and updates product stock in a transaction.
        /// </summary>
        /// <param name="order">The order to add.</param>
        /// <returns>The order if found; otherwise, null.</returns>
        public async Task<OrderEntity?> AddOrder(OrderEntity order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return order;
        }

        /// <summary>
        /// Updates an existing order.
        /// </summary>
        /// <param name="order">The updated order data.</param>
        /// <returns>True if the update was successful; otherwise, false.</returns>
        public async Task<OrderEntity> UpdateOrder(OrderEntity order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }

        /// <summary>
        /// Deletes an order by its ID.
        /// </summary>
        /// <param name="orderId">Order ID.</param>
        /// <returns>True if the order was deleted successfully; otherwise, false.</returns>
        public async Task<bool> DeleteOrder(int orderId)
        {
            var order = await GetOrderById(orderId);

            if (order is null)
                return false;

            _context.Orders.Remove(order);
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Updates the status of an order.
        /// </summary>
        /// <param name="orderId">Order ID.</param>
        /// <param name="newStatus">New order status.</param>
        /// <returns>True if the status update was successful; otherwise, false.</returns>
        public async Task<bool> UpdateOrderStatus(int orderId, OrderStatus newStatus)
        {
            var order = await GetOrderById(orderId);

            if (order is null)
                return false;

            order!.Status = newStatus;
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Calculates the total sales amount from paid orders.
        /// </summary>
        /// <returns>The total sales amount.</returns>
        public async Task<decimal> GetTotalSales()
        {
            var totalSales = await _context.Orders
                .Where(o => o.Status == OrderStatus.Paid)
                .AsNoTracking()
                .SumAsync(o => o.TotalAmount);

            return totalSales;
        }
    }
}