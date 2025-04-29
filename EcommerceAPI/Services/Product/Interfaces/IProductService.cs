
using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs;
using EcommerceAPI.Models.DTOs.Product;

namespace EcommerceAPI.Services.Product.Interfaces
{
    public interface IProductService
    {
        Task<ProductDto> AddProduct(int userId, ProductAddDto productAdd);
        Task<ProductDto?> GetProductById(int productId);
        Task<bool> DeleteProduct(int userId, UserRole role, int productId);
        Task<PagedResult<ProductDto>> GetProducts(int userId, UserRole role, ProductQueryParameters parameters);
        Task<ProductDto?> UpdateProduct(int userId, ProductUpdateDto productUpdate);
    }
}