using EcommerceAPI.Models.DTOs.Category;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.Category.Interfaces
{
    public interface ICategoryService
    {
        /// <summary>
        /// Adds the category.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="categoryAddDto">The category to add.</param>
        /// <returns>The added category as a DTO.</returns>
        /// <exception cref="System.Exception">Failed to add category.</exception>
        Task<CategoryDto> AddCategory(int userId, CategoryAddDto categoryAddDto);

        /// <summary>
        /// Deletes the category.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="id">The identifier.</param>
        Task DeleteCategory(int userId, int id);

        /// <summary>
        /// Gets the categories.
        /// </summary>
        /// <returns>A collection of category DTOs.</returns>
        Task<IEnumerable<CategoryDto>> GetCategories();

        /// <summary>
        /// Gets the category by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The category data transfer object (DTO) for the specified identifier.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Category with ID {id} not found.</exception>
        Task<CategoryDto?> GetCategoryById(int id);

        /// <summary>
        /// Updates the category.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="categoryUpdateDto">The category to update.</param>
        /// <returns>The updated category as a DTO.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Category with the given ID not found.</exception>
        /// <exception cref="System.Exception">Failed to update category.</exception>
        Task<CategoryDto> UpdateCategory(int userId, CategoryUpdateDto categoryUpdateDto);
    }
}