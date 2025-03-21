using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Models
{
    public class Cart
    {
        public int Id { get; set; }

        [Required, ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
