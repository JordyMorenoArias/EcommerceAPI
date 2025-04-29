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
        /// <param name="context">The database context for e-commerce operations.</param>
        public OrderRepository(EcommerceContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves an order by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the order.</param>
        /// <returns>The order if found; otherwise, <c>null</c>.</returns>
        public async Task<OrderEntity?> GetOrderById(int id)
        {
            return await _context.Orders.FindAsync(id);
        }

        /// <summary>
        /// Retrieves an order along with its related user, shipping address, and order details.
        /// </summary>
        /// <param name="orderId">The ID of the order.</param>
        /// <returns>The order with details if found; otherwise, <c>null</c>.</returns>
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
        /// Retrieves a paginated list of orders based on query parameters.
        /// </summary>
        /// <param name="parameters">Query parameters for filtering and pagination.</param>
        /// <returns>A paged result containing the matching orders.</returns>
        public async Task<PagedResult<OrderEntity>> GetOrders(OrderQueryParameters parameters)
        {
            var query = _context.Orders.Where(o => o.Status != OrderStatus.Draft);

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
        /// Retrieves a paginated list of orders that include products sold by a specific seller.
        /// </summary>
        /// <param name="parameters">Query parameters for filtering and pagination.</param>
        /// <returns>A paged result containing seller-specific order details.</returns>
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
        /// Adds a new order to the database.
        /// </summary>
        /// <param name="order">The order entity to add.</param>
        /// <returns>The added order entity.</returns>
        public async Task<OrderEntity> AddOrder(OrderEntity order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return order;
        }

        /// <summary>
        /// Updates the total amount of a specific order.
        /// </summary>
        /// <param name="orderId">The ID of the order.</param>
        /// <param name="amount">The new total amount.</param>
        /// <returns>The updated order if found; otherwise, <c>null</c>.</returns>
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
        /// Updates the shipping address of an order.
        /// </summary>
        /// <param name="orderId">The ID of the order.</param>
        /// <param name="addressId">The ID of the new shipping address.</param>
        /// <returns>The updated order if found; otherwise, <c>null</c>.</returns>
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
        /// <param name="orderId">The ID of the order.</param>
        /// <param name="newStatus">The new status to assign to the order.</param>
        /// <returns>The updated order if found; otherwise, <c>null</c>.</returns>
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
        /// <param name="orderId">The ID of the order to delete.</param>
        /// <returns><c>true</c> if the order was deleted successfully; otherwise, <c>false</c>.</returns>
        public async Task<bool> DeleteOrder(int orderId)
        {
            var order = new OrderEntity { Id = orderId };
            _context.Orders.Attach(order);
            _context.Orders.Remove(order);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}