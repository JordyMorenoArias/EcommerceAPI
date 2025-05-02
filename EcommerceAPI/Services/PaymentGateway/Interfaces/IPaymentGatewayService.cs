using EcommerceAPI.Models.DTOs.Payment;

namespace EcommerceAPI.Services.PaymentGateway.Interfaces
{
    /// <summary>
    /// Service for processing payments using a payment gateway.
    /// </summary>
    public interface IPaymentGatewayService
    {
        /// <summary>
        /// Processes the payment.
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <param name="amount">The amount.</param>
        /// <returns>A task that represents the asynchronous operation, with a result containing the payment processing outcome.</returns>
        Task<PaymentResultDto> ProcessPayment(PaymentProcessDto dto, decimal amount);
    }
}