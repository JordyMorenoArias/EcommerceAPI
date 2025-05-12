using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.DTOs.Tag
{
    public class GetTagParameters
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
