using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs.Generic;
using EcommerceAPI.Models.DTOs.Payment;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.Payment.Interfaces
{
    /// <summary>
    /// Defines the contract for a payment service that handles retrieving and processing payments.
    /// </summary>
    public interface IPaymentService
    {
        /// <summary>
        /// Retrieves a specific payment by its ID, ensuring the user has the appropriate role and authorization.
        /// </summary>
        /// <param name="userId">The identifier of the requesting user.</param>
        /// <param name="role">The role of the user (e.g., Admin, Customer).</param>
        /// <param name="id">The identifier of the payment to retrieve.</param>
        /// <returns>
        /// A <see cref="PaymentEntity"/> representing the requested payment.
        /// </returns>
        Task<PaymentEntity> GetPaymentById(int userId, UserRole role, int id);

        /// <summary>
        /// Retrieves a paginated list of payments based on filtering parameters and user role.
        /// </summary>
        /// <param name="userId">The identifier of the requesting user.</param>
        /// <param name="role">The role of the user (e.g., Admin, Customer).</param>
        /// <param name="parameters">The filtering and pagination parameters.</param>
        /// <returns>
        /// A <see cref="PagedResult{PaymentProcessDto}"/> containing the paginated list of payments.
        /// </returns>
        Task<PagedResult<PaymentProcessDto>> GetPayments(int userId, UserRole role, PaymentQueryParameters parameters);

        /// <summary>
        /// Processes the payment.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="paymentDto">The payment dto.</param>
        /// <returns>
        /// A <see cref="PaymentResultDto"/> object containing the result of the payment transaction.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// Invalid card number.
        /// or
        /// Card is expired.
        /// </exception>
        /// <exception cref="System.Exception">
        /// Order not found
        /// or
        /// Order already paid
        /// or
        /// Order is canceled
        /// or
        /// User not authorized to pay for this order
        /// or
        /// Payment failed: {transaction.Message}
        Task<PaymentResultDto> ProcessPayment(int userId, UserRole role, int orderId, PaymentProcessDto paymentDto);
    }

}