namespace EcommerceAPI.Models.DTOs.Product
{
    public class SearchParameters
    {
        public string SearchTerm { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
        public decimal? MaxPrice { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}