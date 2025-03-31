using EcommerceAPI.Constants;
using EcommerceAPI.Models.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Required]
        public CategoryProduct Category { get; set; }

        [Required, Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; } = 0;

        [Required]
        public int Stock { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required, ForeignKey("User")]
        public int? UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
