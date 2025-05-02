using EcommerceAPI.Constants;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.DTOs.User
{
    public class UserRoleDto
    {
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [Required]
        public UserRole Role { get; set; } = UserRole.Customer;
    }
}
