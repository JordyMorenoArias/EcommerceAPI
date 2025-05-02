using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.DTOs.Payment
{
    public class PaymentRequestDto
    {
        [Required]
        public int OrderId { get; set; }
        public PaymentProcessDto PaymentDto { get; set; } = new PaymentProcessDto();
    }
}
