using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required, ForeignKey("User")]
        public int? UserId { get; set; }
        public User User { get; set; } = null!;

        [Required, ForeignKey("Address")]
        public int ShippingAddressId { get; set; }
        public Address ShippingAddress { get; set; } = null!;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }
        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; // can be "Pending", "Paid", "Shipped", "Cancelled"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
