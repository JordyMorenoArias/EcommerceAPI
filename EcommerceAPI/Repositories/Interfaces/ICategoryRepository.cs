using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Repositories
{
    /// <summary>
    /// Repository interface for handling category data operations.
    /// </summary>
    public interface ICategoryRepository
    {
        /// <summary>
        /// Adds the category.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns>The added category with updated information from the database.</returns>
        Task<CategoryEntity> AddCategory(CategoryEntity category);

        Task DeleteCategory(int id);

        /// <summary>
        /// Gets the categories.
        /// </summary>
        /// <returns>A collection of all categories with their associated products.</returns>
        Task<IEnumerable<CategoryEntity>> GetCategories();

        /// <summary>
        /// Gets the category by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The category with the specified identifier, or null if not found.</returns>
        Task<CategoryEntity?> GetCategoryById(int id);

        /// <summary>
        /// Updates the category.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns>The updated category.</returns>
        Task<CategoryEntity> UpdateCategory(CategoryEntity category);
    }
}