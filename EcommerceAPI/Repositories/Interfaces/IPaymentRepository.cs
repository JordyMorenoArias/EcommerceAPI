using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs.Generic;
using EcommerceAPI.Models.DTOs.Payment;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Repositories.Interfaces
{
    /// <summary>
    /// Interface for payment repository operations.
    /// </summary>
    public interface IPaymentRepository
    {
        /// <summary>
        /// Adds a new payment to the database.
        /// </summary>
        /// <param name="payment">The payment to add.</param>
        /// <returns>
        /// A <see cref="PaymentEntity"/> object representing the newly added payment.
        /// </returns>
        Task<PaymentEntity> AddPayment(PaymentEntity payment);

        /// <summary>
        /// Gets the payment by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task<PaymentEntity?> GetPaymentById(int id);

        /// <summary>
        /// Gets the payments.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>
        /// A <see cref="PagedResult{PaymentEntity}"/> object containing a paginated list of payments that match the given criteria.
        /// </returns>
        Task<PagedResult<PaymentEntity>> GetPayments(PaymentQueryParameters parameters);

        /// <summary>
        /// Updates an existing payment.
        /// </summary>
        /// <param name="payment">The payment with updated details.</param>
        /// <returns>
        /// A <see cref="PaymentEntity"/> object representing the updated payment.
        /// </returns>
        Task<PaymentEntity> UpdatePayment(PaymentEntity payment);

        /// <summary>
        /// Updates the status of a payment.
        /// </summary>
        /// <param name="id">The payment ID.</param>
        /// <param name="newStatus">The new status of the payment.</param>
        /// <returns>
        /// A <see cref="PaymentEntity"/> object with the updated status, or <c>null</c> if the payment was not found.
        /// </returns>
        Task<PaymentEntity?> UpdatePaymentStatus(int id, PaymentStatus newStatus);
    }
}