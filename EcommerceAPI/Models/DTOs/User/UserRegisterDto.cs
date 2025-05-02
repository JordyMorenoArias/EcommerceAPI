using EcommerceAPI.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace EcommerceAPI.Models.DTOs.User
{
    public class UserRegisterDto
    {
        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "LastName is required")]
        public string? LastName { get; set; }

        [MaxLength(100)]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is required"), EmailAddress, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required, PasswordPropertyText]
        public string Password { get; set; } = string.Empty;
    }
}
