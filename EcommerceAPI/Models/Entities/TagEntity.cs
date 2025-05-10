using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.Entities
{
    public class TagEntity
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(300)]
        public string Description { get; set; } = string.Empty;

        public ICollection<ProductTagEntity> ProductTags { get; set; } = new List<ProductTagEntity>();
    }
}