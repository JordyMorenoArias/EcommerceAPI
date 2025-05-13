using EcommerceAPI.Models.DTOs.Tag;
using EcommerceAPI.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.DTOs.ProductTags
{
    public class ProductTagDto
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public ProductEntity product { get; set; } = null!;

        public int TagId { get; set; }
        public TagDto Tag { get; set; } = null!;
    }
}