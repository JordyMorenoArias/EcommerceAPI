using EcommerceAPI.Constants;
using EcommerceAPI.Data;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Repositories
{
    /// <summary>
    /// Repository for managing payments in the e-commerce system.
    /// </summary>
    public class PaymentRepository : IPaymentRepository
    {
        private readonly EcommerceContext _context;

        public PaymentRepository(EcommerceContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all payments.
        /// </summary>
        /// <returns>A collection of all payments.</returns>
        public async Task<IEnumerable<Payment>> GetAllPayments()
        {
            return await _context.Payments
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a payment by its unique identifier.
        /// </summary>
        /// <param name="id">The payment ID.</param>
        /// <returns>The payment if found, otherwise null.</returns>
        public async Task<Payment?> GetPaymentById(int id)
        {
            return await _context.Payments.FindAsync(id);
        }

        /// <summary>
        /// Retrieves all payments associated with a specific user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>A collection of payments made by the user.</returns>
        public async Task<IEnumerable<Payment>> GetPaymentsByUserId(int userId)
        {
            return await _context.Payments
                .Where(p => p.Order.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a payment along with its associated order details.
        /// </summary>
        /// <param name="id">The payment ID.</param>
        /// <returns>The payment including order details, or null if not found.</returns>
        public async Task<Payment?> GetPaymentByIdWithOrder(int id)
        {
            return await _context.Payments
                .Include(p => p.Order)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        /// <summary>
        /// Adds a new payment to the database.
        /// </summary>
        /// <param name="payment">The payment to add.</param>
        /// <returns>True if the payment was added successfully, otherwise false.</returns>
        public async Task<bool> AddPayment(Payment payment)
        {
            _context.Payments.Add(payment);
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Updates an existing payment.
        /// </summary>
        /// <param name="payment">The payment with updated details.</param>
        /// <returns>True if the update was successful, otherwise false.</returns>
        public async Task<bool> UpdatePayment(Payment payment)
        {
            var existingPayment = await GetPaymentById(payment.Id);

            if (existingPayment is null)
                return false;

            _context.Payments.Update(payment);
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Deletes a payment by its unique identifier.
        /// </summary>
        /// <param name="id">The payment ID.</param>
        /// <returns>True if the deletion was successful, otherwise false.</returns>
        public async Task<bool> DeletePayment(int id)
        {
            var payment = await GetPaymentById(id);

            if (payment is null)
                return false;

            _context.Payments.Remove(payment);
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Updates the status of a payment.
        /// </summary>
        /// <param name="id">The payment ID.</param>
        /// <param name="newStatus">The new status of the payment.</param>
        /// <returns>True if the update was successful, otherwise false.</returns>
        public async Task<bool> UpdatePaymentStatus(int id, PaymentStatus newStatus)
        {
            var payment = await GetPaymentById(id);
            if (payment is null)
                return false;

            payment.Status = newStatus;
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Retrieves all payments with a specific status.
        /// </summary>
        /// <param name="status">The payment status to filter by.</param>
        /// <returns>A collection of payments with the specified status.</returns>
        public async Task<IEnumerable<Payment>> GetPaymentsByStatus(PaymentStatus status)
        {
            return await _context.Payments
                .Where(p => p.Status == status)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all payments made using a specific payment method.
        /// </summary>
        /// <param name="method">The payment method to filter by.</param>
        /// <returns>A collection of payments made with the specified method.</returns>
        public async Task<IEnumerable<Payment>> GetPaymentsByMethod(PaymentMethod method)
        {
            return await _context.Payments
                .Where(p => p.PaymentMethod == method)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all payments related to a specific order.
        /// </summary>
        /// <param name="orderId">The order ID.</param>
        /// <returns>A collection of payments for the order.</returns>
        public async Task<IEnumerable<Payment>> GetPaymentsByOrderIdAsync(int orderId)
        {
            return await _context.Payments
                .Where(p => p.OrderId == orderId)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Calculates the total amount of payments made by a user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>The total amount of payments.</returns>
        public async Task<decimal> GetTotalPaymentsByUserId(int userId)
        {
            return await _context.Payments
                .Where(p => p.Order.UserId == userId)
                .AsNoTracking()
                .SumAsync(p => p.Amount);
        }

        /// <summary>
        /// Calculates the total amount of payments within a specific date range.
        /// </summary>
        /// <param name="startDate">The start date of the range.</param>
        /// <param name="endDate">The end date of the range.</param>
        /// <returns>The total amount of payments within the date range.</returns>
        public async Task<decimal> GetTotalPaymentsByDateRange(DateTime startDate, DateTime endDate)
        {
            return await _context.Payments
                .Where(p => p.CreatedAt >= startDate && p.CreatedAt <= endDate)
                .AsNoTracking()
                .SumAsync(p => p.Amount);
        }
    }
}
