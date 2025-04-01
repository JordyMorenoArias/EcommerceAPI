using EcommerceAPI.Constants;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "LastName is required")]
        public string? LastName { get; set; }

        [MaxLength(100)]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is required"), EmailAddress, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required, PasswordPropertyText]
        public string PasswordHash { get; set; } = string.Empty;

        public UserRole Role { get; set; } = UserRole.Customer;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsEmailConfirmed { get; set; } = false;

        [MaxLength(100)]
        public string? EmailConfirmedToken { get; set; }
        public int? ResetPasswordCode { get; set; }
        public DateTime? ResetTokenExpiresAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
