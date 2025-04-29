using EcommerceAPI.Constants;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Repositories.Interfaces
{
    /// <summary>
    /// Interface for payment repository operations.
    /// </summary>
    public interface IPaymentRepository
    {
        /// <summary>
        /// Adds a new payment to the repository.
        /// </summary>
        /// <param name="payment">The payment entity to be added.</param>
        /// <returns>True if the payment was successfully added, otherwise false.</returns>
        Task<bool> AddPayment(PaymentEntity payment);

        /// <summary>
        /// Deletes a payment from the repository.
        /// </summary>
        /// <param name="id">The ID of the payment to be deleted.</param>
        /// <returns>True if the payment was successfully deleted, otherwise false.</returns>
        Task<bool> DeletePayment(int id);

        /// <summary>
        /// Retrieves all payments from the repository.
        /// </summary>
        /// <returns>A collection of all payment entities.</returns>
        Task<IEnumerable<PaymentEntity>> GetAllPayments();

        /// <summary>
        /// Retrieves a payment by its ID.
        /// </summary>
        /// <param name="id">The ID of the payment to retrieve.</param>
        /// <returns>The payment entity if found, otherwise null.</returns>
        Task<PaymentEntity?> GetPaymentById(int id);

        /// <summary>
        /// Retrieves a payment by its ID, including the associated order details.
        /// </summary>
        /// <param name="id">The ID of the payment to retrieve.</param>
        /// <returns>The payment entity with order details if found, otherwise null.</returns>
        Task<PaymentEntity?> GetPaymentByIdWithOrder(int id);

        /// <summary>
        /// Retrieves payments based on the payment method.
        /// </summary>
        /// <param name="method">The payment method used for filtering.</param>
        /// <returns>A collection of payments filtered by the specified method.</returns>
        Task<IEnumerable<PaymentEntity>> GetPaymentsByMethod(PaymentMethod method);

        /// <summary>
        /// Retrieves payments by the order ID.
        /// </summary>
        /// <param name="orderId">The ID of the order associated with the payments.</param>
        /// <returns>A collection of payments associated with the specified order ID.</returns>
        Task<IEnumerable<PaymentEntity>> GetPaymentsByOrderIdAsync(int orderId);

        /// <summary>
        /// Retrieves payments based on the payment status.
        /// </summary>
        /// <param name="status">The payment status used for filtering.</param>
        /// <returns>A collection of payments filtered by the specified status.</returns>
        Task<IEnumerable<PaymentEntity>> GetPaymentsByStatus(PaymentStatus status);

        /// <summary>
        /// Retrieves payments by the user ID.
        /// </summary>
        /// <param name="userId">The ID of the user associated with the payments.</param>
        /// <returns>A collection of payments associated with the specified user ID.</returns>
        Task<IEnumerable<PaymentEntity>> GetPaymentsByUserId(int userId);

        /// <summary>
        /// Calculates the total payments made within a specific date range.
        /// </summary>
        /// <param name="startDate">The start date of the date range.</param>
        /// <param name="endDate">The end date of the date range.</param>
        /// <returns>The total amount of payments within the specified date range.</returns>
        Task<decimal> GetTotalPaymentsByDateRange(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Retrieves the total amount of payments made by a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user for whom to calculate the total payments.</param>
        /// <returns>The total amount of payments made by the specified user.</returns>
        Task<decimal> GetTotalPaymentsByUserId(int userId);

        /// <summary>
        /// Updates an existing payment in the repository.
        /// </summary>
        /// <param name="payment">The payment entity with updated information.</param>
        /// <returns>True if the payment was successfully updated, otherwise false.</returns>
        Task<bool> UpdatePayment(PaymentEntity payment);

        /// <summary>
        /// Updates the status of a payment.
        /// </summary>
        /// <param name="id">The ID of the payment to update.</param>
        /// <param name="newStatus">The new payment status.</param>
        /// <returns>True if the payment status was successfully updated, otherwise false.</returns>
        Task<bool> UpdatePaymentStatus(int id, PaymentStatus newStatus);
    }
}