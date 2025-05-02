using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs.Payment;
using EcommerceAPI.Services.PaymentGateway.Interfaces;
using Stripe;

namespace EcommerceAPI.Services.PaymentGateway
{
    /// <summary>
    /// Service for processing payments using Stripe.
    /// </summary>
    /// <seealso cref="EcommerceAPI.Services.PaymentGateway.Interfaces.IPaymentGatewayService" />
    public class StripePaymentGatewayService : IPaymentGatewayService
    {
        private readonly ILogger<StripePaymentGatewayService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StripePaymentGatewayService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public StripePaymentGatewayService(ILogger<StripePaymentGatewayService> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Processes the payment.
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <param name="amount">The amount.</param>
        /// <returns>A task that represents the asynchronous operation, with a result containing the payment processing outcome.</returns>
        public async Task<PaymentResultDto> ProcessPayment(PaymentProcessDto dto, decimal amount)
        {
            try
            {
                var options = new ChargeCreateOptions
                {
                    Amount = (long)(amount * 100), // Stripe works with cents,
                    Currency = dto.Currency.ToLower(),
                    Source = await CreateCardToken(dto),
                    Description = $"Charge for {dto.CardHolderName}"
                };

                var service = new ChargeService();
                var charge = await service.CreateAsync(options);

                var cardBrand = charge.PaymentMethodDetails?.Card?.Brand ?? "Unknown";
                var last4 = charge.PaymentMethodDetails?.Card?.Last4
                            ?? (dto.CardNumber.Length >= 4 ? dto.CardNumber[^4..] : "0000");

                logger.LogInformation($"Payment processed: {charge.Id}, Status: {charge.Status}");

                return new PaymentResultDto
                {
                    TransactionId = charge.Id,
                    Status = charge.Status == "succeeded" ? PaymentStatus.Paid : PaymentStatus.Failed,
                    Message = charge.Status == "succeeded" ? "Payment succeeded" : $"Payment failed: {charge.FailureMessage ?? "Unknown reason"}",
                    Amount = amount,
                    Currency = dto.Currency,
                    CreatedAt = DateTime.UtcNow,
                    CardProvider = Enum.TryParse<CardProvider>(cardBrand, true, out var provider) ? provider : CardProvider.Unknown,
                    LastFourDigits = last4
                };
            }
            catch (StripeException ex)
            {
                logger.LogError($"Stripe error: {ex.Message}, Code: {ex.StripeError?.Code}, Status: {ex.HttpStatusCode}");

                return new PaymentResultDto
                {
                    Status = PaymentStatus.Failed,
                    Message = ex.Message,
                    Amount = amount,
                    Currency = dto.Currency,
                    PaymentMethod = dto.Method,
                    CreatedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"Unexpected error: {ex.Message}");

                return new PaymentResultDto
                {
                    Status = PaymentStatus.Failed,
                    Message = "An unexpected error occurred: " + ex.Message,
                    Amount = amount,
                    Currency = dto.Currency,
                    PaymentMethod = dto.Method,
                    CreatedAt = DateTime.UtcNow
                };
            }
        }

        private async Task<string> CreateCardToken(PaymentProcessDto dto)
        {
            var tokenOptions = new TokenCreateOptions
            {
                Card = new TokenCardOptions
                {
                    Name = dto.CardHolderName,
                    Number = dto.CardNumber,
                    ExpMonth = dto.ExpirationMonth,
                    ExpYear = dto.ExpirationYear,
                    Cvc = dto.Cvc
                }
            };

            var tokenService = new TokenService();
            var token = await tokenService.CreateAsync(tokenOptions);
            return token.Id;
        }
    }
}