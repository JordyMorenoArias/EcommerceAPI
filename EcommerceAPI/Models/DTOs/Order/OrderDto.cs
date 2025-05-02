using EcommerceAPI.Constants;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Models.DTOs.OrderDetail;
using EcommerceAPI.Models.DTOs.User;
using EcommerceAPI.Models.DTOs.Address;

namespace EcommerceAPI.Models.DTOs.Order
{
    public class OrderDto
    {
        public int Id { get; set; }

        public int? UserId { get; set; }
        public UserDto User { get; set; } = null!;

        public int ShippingAddressId { get; set; }
        public AddressDto ShippingAddress { get; set; } = null!;

        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<OrderDetailDto> OrderDetails { get; set; } = new List<OrderDetailDto>();
    }
}
