using EcommerceAPI.Constants;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using EcommerceAPI.Models.DTOs.Order;

namespace EcommerceAPI.Models.DTOs.Payment
{
    public class PaymentDto
    {
        public int Id { get; set; }

        [Required, ForeignKey("Order")]
        public int OrderId { get; set; }
        public OrderDto Order { get; set; } = null!;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        public PaymentMethod Method { get; set; }

        public CardProvider CardProvider { get; set; }

        public PaymentStatus Status { get; set; }

        [MaxLength(4)]
        public string LastFourDigits { get; set; } = "";

        [MaxLength(100)]
        public string TransactionId { get; set; } = "";

        public DateTime CreatedAt { get; set; }
    }
}
