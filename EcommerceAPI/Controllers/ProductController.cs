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
        /// Retrieves a paginated list of products based on the provided query parameters.
        /// Filters results depending on the authenticated user's role and permissions.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Returns an HTTP 200 OK response containing a paged result of <see cref="ProductDto"/> items.</returns>
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] ProductQueryParameters parameters)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var products = await _productService.GetProducts(userAuthenticated.Id, userAuthenticated.Role, parameters);
            return Ok(products);
        }

        /// <summary>
        /// Adds a new product (Admin and Seller only).
        /// </summary>
        /// <param name="productAdd">The product to add.</param>
        /// <returns>The created product.</returns>
        [HttpPost]
        [AuthorizeRole(UserRole.Seller)]
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
        [HttpPut]
        [AuthorizeRole(UserRole.Seller)]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductUpdateDto productUpdate)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var product = await _productService.UpdateProduct(userAuthenticated.Id, productUpdate);
            return Ok(product);
        }

        /// <summary>
        /// Deletes a product by its ID (Admin and Seller only).
        /// </summary>
        /// <param name="productId">The ID of the product to delete.</param>
        /// <returns>No content if deleted, or not found/error message.</returns>
        [HttpDelete]
        [AuthorizeRole(UserRole.Admin, UserRole.Seller)]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var result = await _productService.DeleteProduct(userAuthenticated.Id, userAuthenticated.Role, productId);

            if (result)
                return NoContent();

            return NotFound("Product not found or you do not have permission to delete it.");
        }
    }
}