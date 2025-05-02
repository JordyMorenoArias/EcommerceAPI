using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.DTOs.User
{
    public class UserVerifyResetCodeDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public int Code { get; set; }
    }
}
