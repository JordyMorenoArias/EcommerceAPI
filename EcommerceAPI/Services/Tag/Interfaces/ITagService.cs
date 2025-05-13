using EcommerceAPI.Models.DTOs;
using EcommerceAPI.Models.DTOs.Tag;

namespace EcommerceAPI.Services.Tag
{
    /// <summary>
    /// Service for managing tags.
    /// </summary>
    public interface ITagService
    {
        /// <summary>
        /// Adds the tag.
        /// </summary>
        /// <param name="tagAdd">The tag add.</param>
        /// <returns>The newly created tag.</returns>
        Task<TagDto> AddTag(TagAddDto tagAdd);

        /// <summary>
        /// Deletes the tag.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns><c>true</c> if the tag was successfully deleted; otherwise, <c>false</c>.</returns>
        Task<bool> DeleteTag(int id);

        /// <summary>
        /// Filters the existing tags.
        /// </summary>
        /// <param name="tagIds">The tag ids.</param>
        /// <returns>A filtered enumerable containing only the tag IDs that exist in the system.</returns>
        Task<IEnumerable<int>> FilterExistingTags(IEnumerable<int> tagIds);

        /// <summary>
        /// Gets the tag by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The tag with the specified identifier.</returns>
        Task<TagDto> GetTagById(int id);

        /// <summary>
        /// Gets the tags.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A paged result containing all tags.</returns>
        Task<PagedResult<TagDto>> GetTags(GetTagParameters parameters);

        /// <summary>
        /// Searches the tags.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A paged result containing tags that match the search term.</returns>
        Task<PagedResult<TagDto>> SearchTags(SearchTagParameters parameters);

        /// <summary>
        /// Updates the tag.
        /// </summary>
        /// <param name="tagUpdate">The tag update.</param>
        /// <returns>The updated tag.</returns>
        Task<TagDto> UpdateTag(TagUpdateDto tagUpdate);
    }
}