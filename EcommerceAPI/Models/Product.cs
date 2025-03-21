using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public string Category { get; set; }

        [Required]
        public decimal Price { get; set; } = 0;

        public int Stock { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
