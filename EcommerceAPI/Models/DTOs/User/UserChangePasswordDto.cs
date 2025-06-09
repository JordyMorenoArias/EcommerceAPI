using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.DTOs.User
{
    public class UserChangePasswordDto
    {
        [Required]
        public string OldPassword { get; set; } = string.Empty;

        [Required] 
        public string NewPassword { get; set; } = string.Empty;
    }
}