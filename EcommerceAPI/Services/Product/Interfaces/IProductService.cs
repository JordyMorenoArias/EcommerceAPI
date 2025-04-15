
using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs.Product;

namespace EcommerceAPI.Services.Product.Interfaces
{
    public interface IProductService
    {
        Task<ProductDto> AddProduct(int userId, ProductAddDto productAdd);
        Task<bool> DeleteProduct(int userId, int productId);
        Task<IEnumerable<ProductDto>> GetActiveProducts();
        Task<IEnumerable<ProductDto>> GetActiveProductsByCategory(CategoryProduct category);
        Task<IEnumerable<ProductDto>> GetActiveProductsByUserId(int userId);
        Task<IEnumerable<ProductDto>> GetAllProducts();
        Task<ProductDto?> GetProductById(int productId);
        Task<IEnumerable<ProductDto>> GetProductsByCategory(CategoryProduct category);
        Task<IEnumerable<ProductDto>> SearchProducts(string query);
        Task<ProductDto?> UpdateProduct(int userId, int productId, ProductUpdateDto productUpdate);
    }
}