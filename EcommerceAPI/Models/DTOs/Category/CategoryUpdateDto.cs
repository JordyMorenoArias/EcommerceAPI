using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.DTOs.Category
{
    public class CategoryUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Description { get; set; }
    }
}
