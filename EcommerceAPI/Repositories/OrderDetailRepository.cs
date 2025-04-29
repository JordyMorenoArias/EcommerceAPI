using EcommerceAPI.Data;
using EcommerceAPI.Models.DTOs.OrderDetail;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Repositories
{
    /// <summary>
    /// Repository for managing order details in the database.
    /// </summary>
    /// <seealso cref="EcommerceAPI.Repositories.Interfaces.IOrderDetailRepository" />
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly EcommerceContext _context;
        private readonly ILogger<OrderDetailRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderDetailRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public OrderDetailRepository(EcommerceContext context, ILogger<OrderDetailRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Gets the order detail by its identifier.
        /// </summary>
        /// <param name="id">The order detail identifier.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the
        /// <see cref="OrderDetailEntity"/> if found; otherwise, <c>null</c>.
        /// </returns>
        public async Task<OrderDetailEntity?> GetOrderDetailById(int id)
        {
            return await _context.OrderDetails
                .Include(od => od.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(od => od.Id == id);
        }

        /// <summary>
        /// Adds the order details to the specified order and updates the product stock and order total.
        /// </summary>
        /// <param name="orderId">The identifier of the order to which details will be added.</param>
        /// <param name="orderDetails">The collection of order details to add.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains an
        /// <see cref="AddOrderDetailResultDto"/> object indicating the result of the operation,
        /// including success status and any stock errors encountered.
        /// </returns>
        public async Task<AddOrderDetailResultDto> AddOrderDetails(int orderId, IEnumerable<OrderDetailEntity> orderDetails)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            var result = new AddOrderDetailResultDto();

            try
            {
                var stockErrors = new List<StockErrorDto>();
                var trackedDetails = new List<OrderDetailEntity>();

                foreach (var detail in orderDetails)
                {
                    detail.OrderId = orderId;

                    var product = await _context.Products
                        .Where(p => p.Id == detail.ProductId)
                        .AsTracking()
                        .FirstOrDefaultAsync();

                    if (product == null)
                    {
                        stockErrors.Add(new StockErrorDto
                        {
                            ProductId = detail.ProductId,
                            ProductName = "Unknown",
                            AvailableStock = 0,
                            RequestedQuantity = detail.Quantity,
                        });
                        continue;
                    }

                    if (product.Stock < detail.Quantity)
                    {
                        stockErrors.Add(new StockErrorDto
                        {
                            ProductId = product.Id,
                            ProductName = product.Name,
                            AvailableStock = product.Stock,
                            RequestedQuantity = detail.Quantity
                        });
                        continue;
                    }

                    product.Stock -= detail.Quantity;
                    detail.UnitPrice = product.Price;
                    trackedDetails.Add(detail);
                }

                if (stockErrors.Any())
                {
                    await transaction.RollbackAsync();
                    result.Success = false;
                    result.StockErrors = stockErrors;
                    return result;
                }

                await _context.OrderDetails.AddRangeAsync(trackedDetails);
                await _context.SaveChangesAsync();

                // Recalcular el TotalAmount
                var order = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order is not null)
                {
                    order.RecalculateTotalAmount();
                    _context.Orders.Update(order);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                result.Success = true;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing order details for Order ID {OrderId}", orderId);

                try
                {
                    await transaction.RollbackAsync();
                }
                catch (Exception rollbackEx)
                {
                    _logger.LogError(rollbackEx, "Rollback failed for Order ID {OrderId}", orderId);
                }

                try
                {
                    var order = await _context.Orders.FindAsync(orderId);

                    if (order != null)
                    {
                        _context.Orders.Remove(order);
                        await _context.SaveChangesAsync();
                        _logger.LogWarning("Order {OrderId} was deleted due to processing failure.", orderId);
                    }
                }
                catch (Exception deleteEx)
                {
                    _logger.LogError(deleteEx, "Failed to delete Order ID {OrderId} after error.", orderId);
                }

                result.Success = false;
                result.StockErrors = new List<StockErrorDto>
                {
                    new StockErrorDto
                    {
                        ProductId = 0,
                        ProductName = "N/A",
                        AvailableStock = 0,
                        RequestedQuantity = 0,
                    }
                };

                return result;
            }
        }
    }
}