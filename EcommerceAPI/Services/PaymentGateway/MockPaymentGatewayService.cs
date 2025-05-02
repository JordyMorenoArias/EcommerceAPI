using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs.Payment;
using EcommerceAPI.Services.PaymentGateway.Interfaces;

namespace EcommerceAPI.Services.PaymentGateway
{
    public class MockPaymentGatewayService : IPaymentGatewayService
    {
        public Task<PaymentResultDto> ProcessPayment(PaymentProcessDto dto, decimal amount)
        {
            var result = new PaymentResultDto
            {
                PaymentId = 0,
                TransactionId = Guid.NewGuid().ToString(),
                Status = PaymentStatus.Paid,
                Message = "Simulated payment successful",
                Amount = amount,
                Currency = "USD",
                PaymentMethod = dto.Method,
                CardProvider = CardProvider.Visa,
                LastFourDigits = dto.CardNumber[^4..],
                CreatedAt = DateTime.UtcNow
            };

            return Task.FromResult(result);
        }
    }
}