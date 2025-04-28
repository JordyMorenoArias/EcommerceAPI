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
        Task<PagedResult<ProductEntity>> GetActiveProductsByCategory(CategoryProduct category, int page, int pageSize);
        Task<PagedResult<ProductEntity>> GetProductsByCategory(CategoryProduct category, int page, int pageSize);
        Task<PagedResult<ProductEntity>> GetActiveProductsByUserId(int userId, int page, int pageSize);
        Task<PagedResult<ProductEntity>> GetProductsByUserId(int userId, int page, int pageSize);
        Task<PagedResult<ProductEntity>> GetActiveProducts(int page, int pageSize);
        Task<PagedResult<ProductEntity>> GetProducts(int page, int pageSize);
        Task<PagedResult<ProductEntity>> SearchProducts(string query, int page, int pageSize);
    }
}