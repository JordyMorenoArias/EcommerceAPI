using EcommerceAPI.Constants;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "FirstName is required"), MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "LastName is required"), MaxLength(100)]
        public string? LastName { get; set; }

        [MaxLength(100)]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is required"), EmailAddress, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [PasswordPropertyText]
        public string? PasswordHash { get; set; } = null;

        public UserRole Role { get; set; } = UserRole.Customer;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public UserProvider Provider { get; set; } = UserProvider.local;

        [MaxLength(200)]
        public string? ProviderId { get; set; } = null;

        public bool IsEmailConfirmed { get; set; } = false;

        [MaxLength(200)]
        public string? EmailConfirmedToken { get; set; }
        public int? ResetPasswordCode { get; set; }
        public DateTime? ResetTokenExpiresAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
