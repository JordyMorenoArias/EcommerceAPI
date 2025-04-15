using EcommerceAPI.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Models
{
    public class CartEntity
    {
        public int Id { get; set; }

        [Required, ForeignKey("User")]
        public int UserId { get; set; }
        public UserEntity User { get; set; } = null!;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<CartItemEntity> CartItems { get; set; } = new List<CartItemEntity>();
    }
}
