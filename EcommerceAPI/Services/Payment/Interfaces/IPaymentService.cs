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
        /// Processes a payment for a given order and user.
        /// </summary>
        /// <param name="userId">The identifier of the user making the payment.</param>
        /// <param name="orderId">The identifier of the order to pay for.</param>
        /// <param name="paymentDto">The data transfer object containing payment details.</param>
        /// <returns>
        /// A <see cref="PaymentResultDto"/> containing the result of the payment transaction.
        /// </returns>
        Task<PaymentResultDto> ProcessPayment(int userId, int orderId, PaymentProcessDto paymentDto);
    }

}