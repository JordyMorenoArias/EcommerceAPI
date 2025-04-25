using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Models.DTOs.OrderDetail
{
    public class AddOrderDetailResultDto
    {
        public bool Success { get; set; } = true;
        public List<StockErrorDto> StockErrors { get; set; } = new();
    }
}
