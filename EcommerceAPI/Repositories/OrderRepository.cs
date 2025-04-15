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
        public async Task<IEnumerable<OrderEntity>> GetAllOrders()
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
        public async Task<IEnumerable<OrderEntity>> GetAllOrdersByUserId(int userId)
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
        /// <returns>True if the order was added successfully; otherwise, false.</returns>
        public async Task<OrderEntity?> AddOrder(OrderEntity order)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();

                foreach (var orderDetail in order.OrderDetails)
                {
                    var product = await _context.Products
                        .Where(p => p.Id == orderDetail.ProductId)
                        .AsTracking()
                        .FirstOrDefaultAsync();

                    if (product is null)
                    {
                        await transaction.RollbackAsync();
                        return null;
                    }

                    product.Stock -= orderDetail.Quantity;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return order;
            }
            catch
            {
                await transaction.RollbackAsync();
                return null;
            }
        }

        /// <summary>
        /// Updates an existing order.
        /// </summary>
        /// <param name="order">The updated order data.</param>
        /// <returns>True if the update was successful; otherwise, false.</returns>
        public async Task<bool> UpdateOrder(OrderEntity order)
        {
            var existingOrder = await GetOrderById(order.Id);

            if (existingOrder is null) 
                return false;

            _context.Orders.Update(order);
            return await _context.SaveChangesAsync() > 0;
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
