using EcommerceAPI.Constants;
using EcommerceAPI.Models;
using EcommerceAPI.Models.DTOs;
using EcommerceAPI.Models.DTOs.Product;

namespace EcommerceAPI.Repositories
{
    public interface IProductRepository
    {
        Task<ProductEntity> AddProduct(ProductEntity product);
        Task<bool> DeleteProduct(int id);
        Task<ProductEntity?> GetProductById(int id);
        Task<ProductEntity> UpdateProduct(ProductEntity product);
        Task<PagedResult<ProductDto>> GetActiveProductsByCategory(CategoryProduct category, int page, int pageSize);
        Task<PagedResult<ProductDto>> GetProductsByCategory(CategoryProduct category, int page, int pageSize);
        Task<PagedResult<ProductDto>> GetActiveProductsByUserId(int userId, int page, int pageSize);
        Task<PagedResult<ProductDto>> GetProductsByUserId(int userId, int page, int pageSize);
        Task<PagedResult<ProductDto>> GetActiveProducts(int page, int pageSize);
        Task<PagedResult<ProductDto>> GetProducts(int page, int pageSize);
        Task<PagedResult<ProductDto>> SearchProducts(string query, int page, int pageSize);
    }
}