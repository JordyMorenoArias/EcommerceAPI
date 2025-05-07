using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.DTOs.Product
{
    public class SearchParameters
    {
        [Required]
        public string SearchTerm { get; set; } = string.Empty;

        public bool? IsActive { get; set; }

        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}