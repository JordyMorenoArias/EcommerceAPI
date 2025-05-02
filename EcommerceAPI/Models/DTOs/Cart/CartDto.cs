using EcommerceAPI.Models.DTOs.CartItem;
using EcommerceAPI.Models.DTOs.User;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Models.DTOs.Cart
{
    public class CartDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public UserDto User { get; set; } = null!;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<CartItemDto> CartItems { get; set; } = new List<CartItemDto>();

        public decimal Total { get; set; }
    }
}