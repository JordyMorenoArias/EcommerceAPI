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

        public OrderDetailRepository(EcommerceContext context, ILogger<OrderDetailRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Gets the order detail by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<OrderDetailEntity?> GetOrderDetailById(int id)
        {
            return await _context.OrderDetails
                .Include(od => od.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(od => od.Id == id);
        }

        /// <summary>
        /// Adds the order details.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="orderDetails">The order details.</param>
        /// <returns></returns>
        public async Task<AddOrderDetailResultDto?> AddOrderDetails(int orderId, IEnumerable<OrderDetailEntity> orderDetails)
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
                await transaction.CommitAsync();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing order details for Order ID {OrderId}", orderId);
                await transaction.RollbackAsync();
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