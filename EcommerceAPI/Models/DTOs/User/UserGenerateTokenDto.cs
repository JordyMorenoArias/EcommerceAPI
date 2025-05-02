using EcommerceAPI.Constants;

namespace EcommerceAPI.Models.DTOs.User
{
    public class UserGenerateTokenDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }
    }
}
