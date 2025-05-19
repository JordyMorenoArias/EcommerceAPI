using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.DTOs.Product
{
    public class SearchProductParameters
    {
        [Required]
        public string SearchTerm { get; set; } = string.Empty;

        public bool? IsActive { get; set; }

        public double? MinPrice { get; set; } = 0;
        public double? MaxPrice { get; set; } = double.MaxValue;

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}