using AutoMapper;
using EcommerceAPI.Models.DTOs.Category;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Repositories;
using EcommerceAPI.Services.Category.Interfaces;
using EcommerceAPI.Services.Infrastructure.Interfaces;

namespace EcommerceAPI.Services.Category
{
    /// <summary>
    /// Service for handling category operations.
    /// </summary>
    /// <seealso cref="EcommerceAPI.Services.Category.Interfaces.ICategoryService" />
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper, ICacheService cacheService, ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _cacheService = cacheService;
            _logger = logger;
        }

        /// <summary>
        /// Gets the category by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The category data transfer object (DTO) for the specified identifier.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Category with ID {id} not found.</exception>
        public async Task<CategoryDto?> GetCategoryById(int id)
        {
            var cacheKey = $"Category_{id}";
            var cachedCategory = await _cacheService.Get<CategoryDto>(cacheKey);

            if (cachedCategory != null)
                return cachedCategory;

            var category = await _categoryRepository.GetCategoryById(id);

            if (category == null)
                throw new KeyNotFoundException($"Category with ID {id} not found.");

            var categoryDto = _mapper.Map<CategoryDto>(category);
            await _cacheService.Set(cacheKey, categoryDto, TimeSpan.FromMinutes(5));
            return categoryDto;
        }

        /// <summary>
        /// Gets the categories.
        /// </summary>
        /// <returns>A collection of category DTOs.</returns>
        public async Task<IEnumerable<CategoryDto>> GetCategories()
        {
            var cacheKey = "Categories";
            var cachedCategories = await _cacheService.Get<IEnumerable<CategoryDto>>(cacheKey);

            if (cachedCategories != null)
                return cachedCategories;

            var categories = await _categoryRepository.GetCategories();

            var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            await _cacheService.Set(cacheKey, categoryDtos, TimeSpan.FromMinutes(5));
            return categoryDtos;
        }

        /// <summary>
        /// Adds the category.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="categoryAddDto">The category add dto.</param>
        /// <returns>The added category as a DTO.</returns>
        /// <exception cref="System.Exception">Failed to add category.</exception>
        public async Task<CategoryDto> AddCategory(int userId, CategoryAddDto categoryAddDto)
        {
            var category = _mapper.Map<CategoryEntity>(categoryAddDto);
            var addedCategory = await _categoryRepository.AddCategory(category);

            if (addedCategory == null)
                throw new Exception("Failed to add category.");

            // Log the addition of the category
            _logger.LogInformation($"Category with ID {addedCategory.Id} added by user {userId}.");

            // Invalidate the cache for categories
            await _cacheService.Remove("Categories");

            return _mapper.Map<CategoryDto>(addedCategory);
        }

        /// <summary>
        /// Updates the category.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="categoryUpdateDto">The category to update.</param>
        /// <returns>The updated category as a DTO.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Category with the given ID not found.</exception>
        /// <exception cref="System.Exception">Failed to update category.</exception>
        public async Task<CategoryDto> UpdateCategory(int userId, CategoryUpdateDto categoryUpdateDto)
        {
            var category = await _categoryRepository.GetCategoryById(categoryUpdateDto.Id);

            if (category == null)
                throw new KeyNotFoundException($"Category with ID {categoryUpdateDto.Id} not found.");

            _mapper.Map(categoryUpdateDto, category);
            var updatedCategory = await _categoryRepository.UpdateCategory(category);

            if (updatedCategory == null)
                throw new Exception("Failed to update category.");

            // Log the update of the category
            _logger.LogInformation($"Category with ID {updatedCategory.Id} : {updatedCategory.Name} updated by user {userId}.");

            // Invalidate the cache for categories and the specific category
            await _cacheService.Remove("Categories");
            await _cacheService.Remove($"Category_{categoryUpdateDto.Id}");

            return _mapper.Map<CategoryDto>(updatedCategory);
        }

        /// <summary>
        /// Deletes the category.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="id">The identifier.</param>
        public async Task DeleteCategory(int userId, int id)
        {
            await _categoryRepository.DeleteCategory(id);

            // Log the deletion of the category
            _logger.LogInformation($"Category with ID {id} deleted by user {userId}.");

            // Invalidate the cache for categories and the specific category
            await _cacheService.Remove("Categories");
            await _cacheService.Remove($"Category_{id}");
        }
    }
}