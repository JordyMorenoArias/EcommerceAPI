using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.DTOs.ProductTag
{
    public class ProductTagAddDto
    {

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int TagId { get; set; }
    }
}
