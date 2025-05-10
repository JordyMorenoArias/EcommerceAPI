using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.Entities
{
    public class ProductTagEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }
        public ProductEntity product { get; set; } = null!;

        [Required]
        public int TagId { get; set; }
        public TagEntity Tag { get; set; } = null!;
    }
}