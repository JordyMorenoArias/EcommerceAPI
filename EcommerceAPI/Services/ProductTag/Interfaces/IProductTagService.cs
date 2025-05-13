using EcommerceAPI.Models.DTOs.ProductTag;
using EcommerceAPI.Models.DTOs.ProductTags;

namespace EcommerceAPI.Services.ProductTag.Interfaces
{
    public interface IProductTagService
    {
        Task<ProductTagDto> AssignTagToProduct(int userId, ProductTagAddDto productTagAdd);
    }
}