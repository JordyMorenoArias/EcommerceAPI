using EcommerceAPI.Data;
using EcommerceAPI.Models.DTOs.Generic;
using EcommerceAPI.Models.DTOs.Tag;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Repositories
{
    /// <summary>
    /// Repository for managing tags in the database.
    /// </summary>
    /// <seealso cref="EcommerceAPI.Repositories.Interfaces.ItagRepository" />
    public class TagRepository : ITagRepository
    {
        private readonly EcommerceContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="TagRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public TagRepository(EcommerceContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets the tag by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A Task that represents the asynchronous operation. The task result contains the TagEntity if found, or null if not found.</returns>
        public async Task<TagEntity?> GetTagById(int id)
        {
            return await _context.Tags.FindAsync(id);
        }

        /// <summary>
        /// Filters the existing tags.
        /// </summary>
        /// <param name="tagIds">The tag ids.</param>
        /// <returns>A filtered enumerable containing only the tag IDs that exist in the system.</returns>
        public async Task<IEnumerable<int>> FilterExistingTags(IEnumerable<int> tagIds)
        {
            var existingTagIds = await _context.Tags
                .Where(tag => tagIds.Contains(tag.Id))
                .Select(tag => tag.Id)
                .ToListAsync();

            return existingTagIds;
        }

        /// <summary>
        /// Gets the tags.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>A Task that represents the asynchronous operation. The task result contains a PagedResult of TagEntity with pagination information.</returns>
        public async Task<PagedResult<TagEntity>> GetTags(GetTagParameters parameters)
        {
            var totalItems = await _context.Tags.CountAsync();

            var tags = await _context.Tags
                .Skip((parameters.Page - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return new PagedResult<TagEntity>
            {
                Items = tags,
                TotalItems = totalItems,
                Page = parameters.Page,
                PageSize = parameters.PageSize
            };
        }

        /// <summary>
        /// Adds the tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>A Task that represents the asynchronous operation. The task result contains the added TagEntity.</returns>
        public async Task<TagEntity> AddTag(TagEntity tag)
        {
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();
            return tag;
        }

        /// <summary>
        /// Updates the tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>A Task that represents the asynchronous operation. The task result contains the updated TagEntity.</returns>
        public async Task<TagEntity> UpdateTag(TagEntity tag)
        {
            _context.Tags.Update(tag);
            await _context.SaveChangesAsync();
            return tag;
        }

        /// <summary>
        /// Deletes the tag.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A Task that represents the asynchronous operation. The task result contains true if the tag was deleted, false if the tag was not found.</returns>
        public async Task<bool> DeleteTag(int id)
        {
            var tag = await _context.Tags.FindAsync(id);

            if (tag == null)
            {
                return false;
            }

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
