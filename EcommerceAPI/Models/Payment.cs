using EcommerceAPI.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Required, ForeignKey("Order")]
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public PaymentStatus Status { get; set; } = PaymentStatus.Processing;

        [MaxLength(4)]
        public string LastFourDigits { get; set; } = "";

        [MaxLength(100)]
        public string TransactionId { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
