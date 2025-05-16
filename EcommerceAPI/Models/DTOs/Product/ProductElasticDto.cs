using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using EcommerceAPI.Models.DTOs.ProductTags;
using EcommerceAPI.Models.DTOs.Tag;

namespace EcommerceAPI.Models.DTOs.Product
{
    public class ProductElasticDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int CategoryId { get; set; }

        public double Price { get; set; } = 0;

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<TagDto> Tags { get; set; } = new List<TagDto>();
    }
}
