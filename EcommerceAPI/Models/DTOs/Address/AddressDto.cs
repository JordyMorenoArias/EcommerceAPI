using EcommerceAPI.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.DTOs.Address
{
    public class AddressDto
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required, MaxLength(100)]
        public string StreetAddress { get; set; } = string.Empty;

        [MaxLength(100)]
        public string AddressLine2 { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string City { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string State { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string PostalCode { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Country { get; set; } = string.Empty;

        public bool IsDefault { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
