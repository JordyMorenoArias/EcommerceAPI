using EcommerceAPI.Constants;

namespace EcommerceAPI.Models.DTOs.Product
{
    public class ProductQueryParameters
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool? IsActive { get; set; }
        public int? UserId { get; set; }
        public CategoryProduct? Category { get; set; }
    }
}
