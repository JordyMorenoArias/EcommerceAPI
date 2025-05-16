using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.DTOs.ProductTag
{
    public class AssignTagsDto
    {
        [Required]
        public int ProductId { get; set; }

        [MaxLength(10, ErrorMessage = "You can only assign a maximum of 10 tags")]
        public IEnumerable<int> TagIds { get; set; } = new List<int>();
    }
}
