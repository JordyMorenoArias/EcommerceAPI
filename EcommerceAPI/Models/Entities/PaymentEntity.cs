using EcommerceAPI.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Models.Entities
{
    public class PaymentEntity
    {
        [Key]
        public int Id { get; set; }

        [Required, ForeignKey("Order")]
        public int OrderId { get; set; }
        public OrderEntity Order { get; set; } = null!;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        [Required, MaxLength(3)]
        public string Currency { get; set; } = "USD";

        public PaymentMethod Method { get; set; } = PaymentMethod.Card;

        public CardProvider CardProvider { get; set; } = CardProvider.Unknown;

        public PaymentStatus Status { get; set; } = PaymentStatus.Processing;

        [MaxLength(4)]
        public string LastFourDigits { get; set; } = "";

        [MaxLength(100)]
        public string TransactionId { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
