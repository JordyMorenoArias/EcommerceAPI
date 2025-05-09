using EcommerceAPI.Constants;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.DTOs.Product
{
    public class ProductUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required, Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; } = 0;

        [Required, MaxLength(3)]
        public string Currency { get; set; } = "USD";

        [Required]
        public int Stock { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }
    }
}
