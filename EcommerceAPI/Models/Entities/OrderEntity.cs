using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using EcommerceAPI.Constants;

namespace EcommerceAPI.Models.Entities
{
    public class OrderEntity
    {
        [Key]
        public int Id { get; set; }

        [Required, ForeignKey("User")]
        public int? UserId { get; set; }
        public UserEntity User { get; set; } = null!;

        [Required, ForeignKey("Address")]
        public int ShippingAddressId { get; set; }
        public AddressEntity ShippingAddress { get; set; } = null!;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<OrderDetailEntity> OrderDetails { get; set; } = new List<OrderDetailEntity>();
    }
}
