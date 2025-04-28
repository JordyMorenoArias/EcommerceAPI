using AutoMapper;
using EcommerceAPI.Constants;
using EcommerceAPI.Data;
using EcommerceAPI.Models;
using EcommerceAPI.Models.DTOs;
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
        /// Retrieves an order by its unique identifier.
        /// </summary>
        /// <param name="id">Order ID.</param>
        /// <returns>The order if found; otherwise, null.</returns>
        public async Task<OrderEntity?> GetOrderById(int id)
        {
            return await _context.Orders.FindAsync(id);
        }

        /// <summary>
        /// Retrieves an order along with its related user, shipping address, and order details.
        /// </summary>
        /// <param name="orderId">Order ID.</param>
        /// <returns>The order with its details if found; otherwise, null.</returns>
        public async Task<OrderEntity?> GetOrderWithDetails(int orderId)
        {
            return await _context.Orders
                .Where(o => o.Id == orderId && o.Status != OrderStatus.Draft)
                .Include(o => o.User)
                .Include(o => o.ShippingAddress)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves orders between two dates with user and shipping address details.
        /// </summary>
        /// <param name="startDate">Start date (inclusive).</param>
        /// <param name="endDate">End date (inclusive).</param>
        /// <returns>A collection of orders between the given dates.</returns>
        public async Task<PagedResult<OrderEntity>> GetOrdersByDateRange(int page, int pageSize, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Orders
                .Where(o => o.Status != OrderStatus.Draft
                 && (startDate == null || o.CreatedAt >= startDate)
                 && (endDate == null || o.CreatedAt <= endDate));

            var totalItems = await query.CountAsync();
            var orders = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(o => o.User)
                .Include(o => o.ShippingAddress)
                .AsNoTracking()
                .ToListAsync();

            return new PagedResult<OrderEntity>
            {
                Items = orders,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Retrieves all orders associated with a specific user.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <returns>A collection of orders for the specified user.</returns>
        public async Task<PagedResult<OrderEntity>> GetOrdersByUserId(int userId, int page, int pageSize, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Orders
                .Where(o => o.UserId == userId && o.Status != OrderStatus.Draft
                 && (startDate == null || o.CreatedAt >= startDate)
                 && (endDate == null || o.CreatedAt <= endDate));

            var orders = await _context.Orders
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(o => o.ShippingAddress)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .AsNoTracking()
                .ToListAsync();

            return new PagedResult<OrderEntity>
            {
                Items = orders,
                TotalItems = await query.CountAsync(),
                Page = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Retrieves all orders with a specific status.
        /// </summary>
        /// <param name="status">Order status.</param>
        /// <returns>A collection of orders matching the status.</returns>
        public async Task<PagedResult<OrderEntity>> GetOrdersByStatus(OrderStatus status, int page, int pageSize, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Orders
                .Where(o => o.Status == status && o.Status != OrderStatus.Draft
                    && (startDate == null || o.CreatedAt >= startDate)
                    && (endDate == null || o.CreatedAt <= endDate));

            var totalItems = await query.CountAsync();
            var orders = await _context.Orders
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(o => o.User)
                .Include(o => o.ShippingAddress)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .AsNoTracking()
                .ToListAsync();

            return new PagedResult<OrderEntity>
            {
                Items = orders,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<OrderEntity>> GetOrdersBySeller(int sellerId, int page, int pageSize, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Orders
                .Where(o => o.OrderDetails.Any(od => od.Product.UserId == sellerId) && o.Status != OrderStatus.Draft
                    && (startDate == null || o.CreatedAt >= startDate)
                    && (endDate == null || o.CreatedAt <= endDate));

            var totalItems = await query.CountAsync();
            var orders = await _context.Orders
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderEntity
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    User = o.User,
                    ShippingAddressId = o.ShippingAddressId,
                    ShippingAddress = o.ShippingAddress,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,
                    OrderDetails = o.OrderDetails
                        .Where(od => od.Product.UserId == sellerId)
                        .Select(od => new OrderDetailEntity
                        {
                            Id = od.Id,
                            ProductId = od.ProductId,
                            Product = od.Product,
                            Quantity = od.Quantity,
                            UnitPrice = od.UnitPrice
                        }).ToList()

                })
                .AsNoTracking()
                .ToListAsync();

            return new PagedResult<OrderEntity>
            {
                Items = orders,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Adds a new order and updates product stock in a transaction.
        /// </summary>
        /// <param name="order">The order to add.</param>
        /// <returns>The order if found; otherwise, null.</returns>
        public async Task<OrderEntity> AddOrder(OrderEntity order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<OrderEntity?> UpdateAmountOrder(int orderId, decimal amount)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if (order == null)
                return null;

            order.TotalAmount = amount;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }

        /// <summary>
        /// Updates the address order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="addressId">The address identifier.</param>
        /// <returns></returns>
        public async Task<OrderEntity?> UpdateAddressOrder(int orderId, int addressId)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if (order == null)
                return null;

            order.ShippingAddressId = addressId;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }

        /// <summary>
        /// Updates the status of an order.
        /// </summary>
        /// <param name="orderId">Order ID.</param>
        /// <param name="newStatus">New order status.</param>
        /// <returns>True if the status update was successful; otherwise, false.</returns>
        public async Task<OrderEntity?> UpdateOrderStatus(int orderId, OrderStatus newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if (order == null)
                return null;

            order.Status = newStatus;
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
            var order = new OrderEntity { Id = orderId };
            _context.Orders.Attach(order);
            _context.Orders.Remove(order);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}