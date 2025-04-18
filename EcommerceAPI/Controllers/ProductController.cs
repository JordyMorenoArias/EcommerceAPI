using EcommerceAPI.Constants;
using EcommerceAPI.Filters;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Services.Product.Interfaces;
using EcommerceAPI.Services.User.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EcommerceAPI.Controllers
{
    /// <summary>
    /// Controller responsible for managing product-related operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeRole(UserRole.Admin, UserRole.Seller, UserRole.Customer)]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IUserService _userService;

        public ProductController(IProductService productService, IUserService userService)
        {
            _productService = productService;
            _userService = userService;
        }

        /// <summary>
        /// Retrieves a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product.</param>
        /// <returns>The product details.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductById(id);
            return Ok(product);
        }

        /// <summary>
        /// Retrieves all products (Admin only).
        /// </summary>
        /// <returns>List of all products.</returns>
        [AuthorizeRole(UserRole.Admin)]
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var products = await _productService.GetProducts(page, pageSize);
            return Ok(products);
        }

        /// <summary>
        /// Retrieves all active (available) products.
        /// </summary>
        /// <returns>List of active products.</returns>
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var products = await _productService.GetActiveProducts(page, pageSize);
            return Ok(products);
        }

        /// <summary>
        /// Searches products by a query string.
        /// </summary>
        /// <param name="query">The search query.</param>
        /// <returns>List of matching products.</returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var products = await _productService.SearchProducts(query, page, pageSize);
            return Ok(products);
        }

        /// <summary>
        /// Retrieves all products in a specific category (Admin only).
        /// </summary>
        /// <param name="category">The category name.</param>
        /// <returns>List of products in the category.</returns>
        [AuthorizeRole(UserRole.Admin)]
        [HttpGet("{category}")]
        public async Task<IActionResult> GetProductsByCategory(string category, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            if (!Enum.TryParse(typeof(CategoryProduct), category, true, out var categoryEnum))
            {
                return BadRequest("Invalid category.");
            }

            var products = await _productService.GetProductsByCategory((CategoryProduct)categoryEnum, page, pageSize);
            return Ok(products);
        }

        /// <summary>
        /// Retrieves all active products in a specific category.
        /// </summary>
        /// <param name="category">The category name.</param>
        /// <returns>List of active products in the category.</returns>
        [HttpGet("{category}/active")]
        public async Task<IActionResult> GetActiveProductsByCategory(string category, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            if (!Enum.TryParse(typeof(CategoryProduct), category, true, out var categoryEnum))
            {
                return BadRequest("Invalid category.");
            }

            var products = await _productService.GetActiveProductsByCategory((CategoryProduct)categoryEnum, page, pageSize);
            return Ok(products);
        }

        /// <summary>
        /// Retrieves all products created by a specific user (Admin and Seller only).
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>List of the user's products.</returns>
        [AuthorizeRole(UserRole.Admin, UserRole.Seller)]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetProductsByUserId([FromRoute] int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);

            if (userAuthenticated.Role != UserRole.Admin && userAuthenticated.Id != userId)
                return Unauthorized("You are not authorized to access this resource.");

            var products = await _productService.GetProductsByUserId(userId, page, pageSize);
            return Ok(products);
        }

        /// <summary>
        /// Retrieves all active products created by a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>List of the user's active products.</returns>
        [HttpGet("user/{userId}/active")]
        public async Task<IActionResult> GetActiveProductsByUserId([FromRoute] int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);

            if (userAuthenticated.Id != userId)
                return Unauthorized("You are not authorized to access this resource.");

            var products = await _productService.GetActiveProductsByUserId(userId, page, pageSize);
            return Ok(products);
        }

        /// <summary>
        /// Adds a new product (Admin and Seller only).
        /// </summary>
        /// <param name="productAdd">The product to add.</param>
        /// <returns>The created product.</returns>
        [AuthorizeRole(UserRole.Admin, UserRole.Seller)]
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] ProductAddDto productAdd)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);

            var product = await _productService.AddProduct(userAuthenticated.Id, productAdd);
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        /// <summary>
        /// Updates an existing product (Admin and Seller only).
        /// </summary>
        /// <param name="productId">The ID of the product to update.</param>
        /// <param name="productUpdate">The updated product information.</param>
        /// <returns>The updated product.</returns>
        [AuthorizeRole(UserRole.Admin, UserRole.Seller)]
        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateProduct(int productId, [FromBody] ProductUpdateDto productUpdate)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var product = await _productService.UpdateProduct(userAuthenticated.Id, productId, productUpdate);

            return Ok(product);
        }

        /// <summary>
        /// Deletes a product by its ID (Admin and Seller only).
        /// </summary>
        /// <param name="productId">The ID of the product to delete.</param>
        /// <returns>No content if deleted, or not found/error message.</returns>
        [AuthorizeRole(UserRole.Admin, UserRole.Seller)]
        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var result = await _productService.DeleteProduct(userAuthenticated.Id, productId);

            if (result)
                return NoContent();

            return NotFound("Product not found or you do not have permission to delete it.");
        }
    }
}