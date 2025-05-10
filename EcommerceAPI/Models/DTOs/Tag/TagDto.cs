using EcommerceAPI.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.DTOs.Tag
{
    public class TagDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}
