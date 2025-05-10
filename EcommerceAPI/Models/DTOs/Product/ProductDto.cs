using EcommerceAPI.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using EcommerceAPI.Models.DTOs.User;
using EcommerceAPI.Models.DTOs.ProductTags;

namespace EcommerceAPI.Models.DTOs.Product
{
    public class ProductDto
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public CategoryEntity Category { get; set; } = null!;

        [Required, Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; } = 0;

        [Required, MaxLength(3)]
        public string Currency { get; set; } = "USD";

        [Required]
        public int Stock { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; }

        [Required]
        public int? UserId { get; set; }
        public UserDto User { get; set; } = null!;

        public ICollection<ProductTagDto> ProductTags { get; set; } = new List<ProductTagDto>();
    }
}