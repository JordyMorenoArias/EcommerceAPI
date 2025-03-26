using EcommerceAPI.Constants;
using EcommerceAPI.Models;

namespace EcommerceAPI.Repositories
{
    public interface IProductRepository
    {
        Task<Product> AddProduct(Product product);
        Task<bool> DeleteProduct(int id);
        Task<IEnumerable<Product>> GetActiveProducts();
        Task<IEnumerable<Product>> GetActiveProductsByCategory(CategoryProduct category);
        Task<IEnumerable<Product>> GetActiveProductsByUserId(int userId);
        Task<IEnumerable<Product>> GetAllProducts();
        Task<Product?> GetProductById(int id);
        Task<IEnumerable<Product>> GetProductsByCategory(CategoryProduct category);
        Task<IEnumerable<Product>> GetProductsByUserId(int userId);
        Task<bool> UpdateProduct(Product product);
    }
}