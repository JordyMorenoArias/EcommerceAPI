using EcommerceAPI.Constants;
using EcommerceAPI.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using EcommerceAPI.Models.DTOs.User;

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
        public CategoryProduct Category { get; set; }

        public decimal Price { get; set; } = 0;

        [Required]
        public int Stock { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; }

        [Required]
        public int? UserId { get; set; }
        public UserDto User { get; set; } = null!;
    }
}