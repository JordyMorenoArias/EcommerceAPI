using EcommerceAPI.Constants;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.DTOs.User
{
    public class UserDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "LastName is required")]
        public string? LastName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is required"), EmailAddress, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        public UserRole Role { get; set; } = UserRole.Customer;

        public DateTime CreatedAt { get; set; }
    }
}
