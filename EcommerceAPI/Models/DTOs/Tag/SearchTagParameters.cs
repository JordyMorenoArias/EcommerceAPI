using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.DTOs.Tag
{
    public class SearchTagParameters
    {
        [Required]
        public string SearchTerm { get; set; } = string.Empty;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
