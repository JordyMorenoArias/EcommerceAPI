using EcommerceAPI.Constants;

namespace EcommerceAPI.Models.DTOs.Order
{
    public class OrderQueryParameters
    {
        public int? UserId { get; set; }
        public OrderStatus? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
