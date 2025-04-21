using EcommerceAPI.Models.DTOs.Product;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Models.DTOs.CartItem
{
    public class CartItemDto
    {
        public int Id { get; set; }

        public int CartId { get; set; }

        public int ProductId { get; set; }
        public ProductDto Product { get; set; } = null!;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}