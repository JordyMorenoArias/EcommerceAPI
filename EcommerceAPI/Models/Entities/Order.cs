using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using EcommerceAPI.Constants;

namespace EcommerceAPI.Models.Entities
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
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
