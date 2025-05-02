using EcommerceAPI.Constants;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.DTOs.User
{
    public class UserUpdateDto
    {
        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "LastName is required")]
        public string? LastName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? PhoneNumber { get; set; }
    }
}
