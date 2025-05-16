using EcommerceAPI.Constants;
using EcommerceAPI.Filters;
using EcommerceAPI.Models.DTOs.Category;
using EcommerceAPI.Services.Category.Interfaces;
using EcommerceAPI.Services.User.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{
    /// <summary>
    /// Controller responsible for managing category-related operations.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeRole(UserRole.Admin, UserRole.Seller, UserRole.Customer)]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IUserService _userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryController"/> class.
        /// </summary>
        /// <param name="categoryService">The category service.</param>
        /// <param name="userService">The user service.</param>
        public CategoryController(ICategoryService categoryService, IUserService userService)
        {
            _categoryService = categoryService;
            _userService = userService;
        }

        /// <summary>
        /// Gets the category by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A 200 OK response with the category data if found; otherwise, a 404 Not Found response.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById([FromRoute] int id)
        {
            var category = await _categoryService.GetCategoryById(id);
            return Ok(category);
        }

        /// <summary>
        /// Gets the categories.
        /// </summary>
        /// <returns>A 200 OK response containing a list of all categories.</returns>
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.GetCategories();
            return Ok(categories);
        }

        /// <summary>
        /// Adds the category.
        /// </summary>
        /// <param name="categoryAddDto">The category add dto.</param>
        /// <returns>A 201 Created response with the newly created category data.</returns>
        [HttpPost]
        [AuthorizeRole(UserRole.Admin)]
        public async Task<IActionResult> AddCategory([FromBody] CategoryAddDto categoryAddDto)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var category = await _categoryService.AddCategory(userAuthenticated.Id, categoryAddDto);
            return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
        }

        /// <summary>
        /// Updates the category.
        /// </summary>
        /// <param name="categoryUpdateDto">The category update dto.</param>
        /// <returns>A 200 OK response with the updated category data.</returns>
        [HttpPut]
        [AuthorizeRole(UserRole.Admin)]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryUpdateDto categoryUpdateDto)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var category = await _categoryService.UpdateCategory(userAuthenticated.Id, categoryUpdateDto);
            return Ok(category);
        }

        /// <summary>
        /// Deletes the category.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A 204 No Content response if the deletion was successful.</returns>
        [HttpDelete("{id}")]
        [AuthorizeRole(UserRole.Admin)]
        public async Task<IActionResult> DeleteCategory([FromRoute] int id)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            await _categoryService.DeleteCategory(userAuthenticated.Id, id);
            return NoContent();
        }
    }
}
