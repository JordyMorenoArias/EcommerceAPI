using EcommerceAPI.Constants;
using EcommerceAPI.Models;

namespace EcommerceAPI.Repositories
{
    public interface IProductRepository
    {
        Task<ProductEntity> AddProduct(ProductEntity product);
        Task<bool> DeleteProduct(int id);
        Task<IEnumerable<ProductEntity>> GetActiveProducts();
        Task<IEnumerable<ProductEntity>> GetActiveProductsByCategory(CategoryProduct category);
        Task<IEnumerable<ProductEntity>> GetActiveProductsByUserId(int userId);
        Task<IEnumerable<ProductEntity>> GetAllProducts();
        Task<ProductEntity?> GetProductById(int id);
        Task<IEnumerable<ProductEntity>> GetProductsByCategory(CategoryProduct category);
        Task<IEnumerable<ProductEntity>> GetProductsByUserId(int userId);
        Task<IEnumerable<ProductEntity>> SearchProducts(string query);
        Task<bool> UpdateProduct(ProductEntity product);
    }
}