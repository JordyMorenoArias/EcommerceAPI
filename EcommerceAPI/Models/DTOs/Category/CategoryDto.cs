using EcommerceAPI.Models.DTOs.Product;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.DTOs.Category
{
    public class CategoryDto
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Description { get; set; }

        public ICollection<ProductDto> Products { get; set; } = new List<ProductDto>();
    }
}
