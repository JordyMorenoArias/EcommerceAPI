using EcommerceAPI.Models.DTOs;
using EcommerceAPI.Models.DTOs.Tag;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Repositories.Interfaces
{
    /// <summary>
    /// Repository for managing tags in the database.
    /// </summary>
    public interface ITagRepository
    {
        /// <summary>
        /// Adds the tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>A Task that represents the asynchronous operation. The task result contains the added TagEntity.</returns>
        Task<TagEntity> AddTag(TagEntity tag);

        /// <summary>
        /// Deletes the tag.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A Task that represents the asynchronous operation. The task result contains true if the tag was deleted, false if the tag was not found.</returns>
        Task<bool> DeleteTag(int id);

        /// <summary>
        /// Gets the tag by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A Task that represents the asynchronous operation. The task result contains the TagEntity if found, or null if not found.</returns>
        Task<TagEntity?> GetTagById(int id);

        /// <summary>
        /// Gets the tags.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A Task that represents the asynchronous operation. The task result contains a PagedResult of TagEntity with pagination information.</returns>
        Task<PagedResult<TagEntity>> GetTags(GetTagParameters parameters);

        /// <summary>
        /// Updates the tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>A Task that represents the asynchronous operation. The task result contains the updated TagEntity.</returns>
        Task<TagEntity> UpdateTag(TagEntity tag);
    }
}