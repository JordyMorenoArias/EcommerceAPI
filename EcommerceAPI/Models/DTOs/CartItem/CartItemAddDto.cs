using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Models.DTOs.Cart
{
    public class CartItemAddDto
    {

        [Required]
        public int ProductId { get; set; }

        public int Quantity { get; set; } = 1;
    }
}