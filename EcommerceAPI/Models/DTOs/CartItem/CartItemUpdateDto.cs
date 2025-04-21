using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.DTOs.Cart
{
    public class CartItemUpdateDto
    {

        [Required]
        public int ProductId { get; set; }

        public int Quantity { get; set; } = 1;
    }
}
