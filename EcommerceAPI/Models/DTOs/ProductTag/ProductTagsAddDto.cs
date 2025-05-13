using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.DTOs.ProductTag
{
    public class ProductTagsAddDto
    {

        [Required]
        public int ProductId { get; set; }

        [Required]
        public List<int> TagIds { get; set; } = new List<int>();
    }
}
