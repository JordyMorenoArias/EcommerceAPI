using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Models.Entities
{
    public class Address
    {
        [Key]
        public int Id { get; set; }

        [Required, ForeignKey("User")]
        public int UserId { get; set; }
        public UserEntity User { get; set; } = null!;

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
