using EcommerceAPI.Constants;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.DTOs.Payment
{
    public class PaymentProcessDto
    {
        [Required(ErrorMessage = "Card holder name is required.")]
        [StringLength(100, ErrorMessage = "Card holder name is too long.")]
        public string CardHolderName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Card number is required.")]
        [RegularExpression(@"^\d{13,19}$", ErrorMessage = "Card number must be between 13 and 19 digits.")]
        public string CardNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Expiration month is required.")]
        [RegularExpression(@"^(0[1-9]|1[0-2])$", ErrorMessage = "Expiration month must be in MM format (01–12).")]
        public string ExpirationMonth { get; set; } = string.Empty;

        [Required(ErrorMessage = "Expiration year is required.")]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Expiration year must be in YYYY format.")]
        public string ExpirationYear { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "Invalid CVC format. Use 3 or 4 digits.")]
        public string Cvc { get; set; } = string.Empty;

        [Required(ErrorMessage = "Currency is required.")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be a 3-letter ISO code.")]
        public string Currency { get; set; } = "USD";

        public PaymentMethod Method { get; set; } = PaymentMethod.Card;
    }
}
