
using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs;
using EcommerceAPI.Models.DTOs.Product;

namespace EcommerceAPI.Services.Product.Interfaces
{
    public interface IProductService
    {
        Task<ProductDto> AddProduct(int userId, ProductAddDto productAdd);
        Task<ProductDto?> GetProductById(int productId);
        Task<ProductDto?> UpdateProduct(int userId, int productId, ProductUpdateDto productUpdate);
        Task<PagedResult<ProductDto>> GetActiveProductsByUserId(int userId, int page, int pageSize);
        Task<PagedResult<ProductDto>> GetProductsByUserId(int userId, int page, int pageSize);
        Task<PagedResult<ProductDto>> GetActiveProductsByCategory(CategoryProduct category, int page, int pageSize);
        Task<PagedResult<ProductDto>> GetProductsByCategory(CategoryProduct category, int page, int pageSize);
        Task<PagedResult<ProductDto>> GetActiveProducts(int page, int pageSize);
        Task<PagedResult<ProductDto>> GetProducts(int page, int pageSize);
        Task<PagedResult<ProductDto>> SearchProducts(string query, int page, int pageSize);
        Task<bool> DeleteProduct(int userId, UserRole role, int productId);
    }
}