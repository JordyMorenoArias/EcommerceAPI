using EcommerceAPI.Constants;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.DTOs.Payment
{
    public class PaymentProcessDto
    {
        [Required]
        public string CardHolderName { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{13,19}$", ErrorMessage = "Invalid card number format. Use 13 to 19 digits.")]
        public string CardNumber { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^(0[1-9]|1[0-2])$", ErrorMessage = "Invalid month format. Use MM.")]
        public string ExpirationMonth { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Invalid year format. Use YYYY.")]
        public string ExpirationYear { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "Invalid CVC format. Use 3 or 4 digits.")]
        public string Cvc { get; set; } = string.Empty;

        [Required]
        public PaymentMethod Method { get; set; }
    }
}
