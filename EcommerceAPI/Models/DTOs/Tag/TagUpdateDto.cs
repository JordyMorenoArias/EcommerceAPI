using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.DTOs.Tag
{
    public class TagUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
    }
}
