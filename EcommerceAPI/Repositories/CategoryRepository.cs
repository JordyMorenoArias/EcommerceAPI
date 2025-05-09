using EcommerceAPI.Data;
using EcommerceAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Repositories
{
    /// <summary>
    /// Repository for handling category data operations.
    /// </summary>
    /// <seealso cref="EcommerceAPI.Repositories.ICategoryRepository" />
    public class CategoryRepository : ICategoryRepository
    {
        private readonly EcommerceContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public CategoryRepository(EcommerceContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets the category by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The category with the specified identifier, or null if not found.</returns>
        public async Task<CategoryEntity?> GetCategoryById(int id)
        {
            return await _context.Categories
                .Include(c => c.Products)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        /// <summary>
        /// Gets the categories.
        /// </summary>
        /// <returns>A collection of all categories with their associated products.</returns>
        public async Task<IEnumerable<CategoryEntity>> GetCategories()
        {
            return await _context.Categories
                .Include(c => c.Products)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Adds the category.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns>The added category with updated information from the database.</returns>
        public async Task<CategoryEntity> AddCategory(CategoryEntity category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        /// <summary>
        /// Updates the category.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns>The updated category.</returns>
        public async Task<CategoryEntity> UpdateCategory(CategoryEntity category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }

        /// <summary>
        /// Deletes the category.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public async Task DeleteCategory(int id)
        {
            var category = await GetCategoryById(id);

            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}
