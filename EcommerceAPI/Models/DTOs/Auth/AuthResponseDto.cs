using EcommerceAPI.Models.DTOs.User;

namespace EcommerceAPI.Models.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime Expires { get; set; }
        public UserDto? User { get; set; }
    }
}
