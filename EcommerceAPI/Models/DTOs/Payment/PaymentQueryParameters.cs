using EcommerceAPI.Constants;

namespace EcommerceAPI.Models.DTOs.Payment
{
    public class PaymentQueryParameters
    {
        public int? UserId { get; set; }
        public PaymentMethod? Method { get; set; }
        public PaymentStatus? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
