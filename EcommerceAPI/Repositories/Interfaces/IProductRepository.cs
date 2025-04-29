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
        Task<PagedResult<ProductEntity>> GetProducts(ProductQueryParameters parameters);
        Task<ProductEntity> UpdateProduct(ProductEntity product);
    }
}