using EcommerceAPI.Constants;
using EcommerceAPI.Data;
using EcommerceAPI.Models.DTOs.Generic;
using EcommerceAPI.Models.DTOs.Payment;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public PaymentRepository(EcommerceContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a payment along with its associated order details.
        /// </summary>
        /// <param name="id">The payment ID.</param>
        /// <returns>The payment including order details, or null if not found.</returns>
        public async Task<PaymentEntity?> GetPaymentById(int id)
        {
            return await _context.Payments
                .Include(p => p.Order)
                    .Include(p => p.Order.User)
                    .Include(p => p.Order.OrderDetails)
                        .ThenInclude(i => i.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        /// <summary>
        /// Gets the payments.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>
        /// A <see cref="PagedResult{PaymentEntity}"/> object containing a paginated list of payments that match the given criteria.
        /// </returns>
        public async Task<PagedResult<PaymentEntity>> GetPayments(PaymentQueryParameters parameters)
        {
            var query = _context.Payments.AsQueryable();

            if (parameters.UserId.HasValue)
                query = query.Where(p => p.Order.UserId == parameters.UserId.Value);

            if (parameters.Status.HasValue)
                query = query.Where(p => p.Status == parameters.Status.Value);

            if (parameters.Method.HasValue)
                query = query.Where(p => p.Method == parameters.Method.Value);

            if (parameters.StartDate.HasValue)
                query = query.Where(p => p.CreatedAt >= parameters.StartDate.Value);

            if (parameters.EndDate.HasValue)
                query = query.Where(p => p.CreatedAt <= parameters.EndDate.Value);

            var totalItems = await query.CountAsync();
            var payments = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((parameters.Page - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .Include(p => p.Order)
                .AsNoTracking()
                .ToListAsync();

            return new PagedResult<PaymentEntity>
            {
                TotalItems = totalItems,
                PageSize = parameters.PageSize,
                Page = parameters.Page,
                Items = payments
            };
        }

        /// <summary>
        /// Adds a new payment to the database.
        /// </summary>
        /// <param name="payment">The payment to add.</param>
        /// <returns>
        /// A <see cref="PaymentEntity"/> object representing the newly added payment.
        /// </returns>
        public async Task<PaymentEntity> AddPayment(PaymentEntity payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        /// <summary>
        /// Updates an existing payment.
        /// </summary>
        /// <param name="payment">The payment with updated details.</param>
        /// <returns>
        /// A <see cref="PaymentEntity"/> object representing the updated payment.
        /// </returns>
        public async Task<PaymentEntity> UpdatePayment(PaymentEntity payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        /// <summary>
        /// Updates the status of a payment.
        /// </summary>
        /// <param name="id">The payment ID.</param>
        /// <param name="newStatus">The new status of the payment.</param>
        /// <returns>
        /// A <see cref="PaymentEntity"/> object with the updated status, or <c>null</c> if the payment was not found.
        /// </returns>
        public async Task<PaymentEntity?> UpdatePaymentStatus(int id, PaymentStatus newStatus)
        {
            var payment = await _context.Payments.FindAsync(id);

            if (payment is null)
                return null;

            payment.Status = newStatus;
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
            return payment;
        }
    }
}