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

        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty; // can be "Credit Card", "Debit Card", "Paypal"
        public string Status { get; set; } = "Processing"; // can be "Processing ", "Paid", "Failed"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
