using AutoMapper;
using EcommerceAPI.Constants;
using EcommerceAPI.Data;
using EcommerceAPI.Models;
using EcommerceAPI.Models.DTOs;
using EcommerceAPI.Models.DTOs.Order;
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
        /// Gets the orders.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public async Task<PagedResult<OrderEntity>> GetOrders(OrderQueryParameters parameters)
        {
            var query = _context.Orders
                .Where(o => o.Status != OrderStatus.Draft);

            if (parameters.UserId.HasValue)
                query = query.Where(o => o.UserId == parameters.UserId.Value);

            if (parameters.Status.HasValue)
                query = query.Where(o => o.Status == parameters.Status.Value);

            if (parameters.StartDate.HasValue)
                query = query.Where(o => o.CreatedAt >= parameters.StartDate.Value);

            if (parameters.EndDate.HasValue)
                query = query.Where(o => o.CreatedAt <= parameters.EndDate.Value);

            var totalItems = await query.CountAsync();

            var orders = await query
                .Skip((parameters.Page - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
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
                Page = parameters.Page,
                PageSize = parameters.PageSize
            };
        }

        /// <summary>
        /// Gets the seller orders.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public async Task<PagedResult<OrderEntity>> GetSellerOrders(OrderSellerQueryParameters parameters)
        {
            var query = _context.Orders
                .Where(o => o.OrderDetails.Any(od => od.Product.UserId == parameters.SellerId)
                    && o.Status != OrderStatus.Draft);

            if (parameters.Status.HasValue)
                query = query.Where(o => o.Status == parameters.Status.Value);

            if (parameters.StartDate.HasValue)
                query = query.Where(o => o.CreatedAt >= parameters.StartDate.Value);

            if (parameters.EndDate.HasValue)
                query = query.Where(o => o.CreatedAt <= parameters.EndDate.Value);

            var totalItems = await query.CountAsync();

            var orders = await query
                .Skip((parameters.Page - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
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
                        .Where(od => od.Product.UserId == parameters.SellerId)
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
                Page = parameters.Page,
                PageSize = parameters.PageSize
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