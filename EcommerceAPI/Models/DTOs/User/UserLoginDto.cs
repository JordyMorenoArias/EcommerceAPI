using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.DTOs.User
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "Email is required"), EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "password is required"), PasswordPropertyText]
        public string Password { get; set; } = string.Empty;
    }
}
