using EcommerceAPI.Constants;

namespace EcommerceAPI.Models.DTOs.Payment
{
    public class PaymentResultDto
    {
        public int PaymentId { get; set; }

        public string TransactionId { get; set; } = string.Empty;

        public PaymentStatus Status { get; set; }

        public string Message { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string Currency { get; set; } = "USD";

        public PaymentMethod PaymentMethod { get; set; }

        public CardProvider CardProvider { get; set; } = CardProvider.Unknown;

        public string LastFourDigits { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }

}