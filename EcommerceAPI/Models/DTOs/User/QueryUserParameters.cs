namespace EcommerceAPI.Models.DTOs.User
{
    public class QueryUserParameters
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool? IsActive { get; set; }
        public string? Role { get; set; }
        public string? Email { get; set; }
    }
}
