using EcommerceAPI.Models.DTOs.ProductTag;
using EcommerceAPI.Models.DTOs.ProductTags;

namespace EcommerceAPI.Services.ProductTag.Interfaces
{
    public interface IProductTagService
    {
        /// <summary>
        /// Assigns the tag to product.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="productTagAdd">The product tag add.</param>
        /// <returns>The assigned <see cref="ProductTagDto"/>.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">
        /// Product not found
        /// or
        /// Tag not found
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">You are not the owner of this product</exception>
        /// <exception cref="System.InvalidOperationException">
        /// Product already has this tag
        /// or
        /// Product already has 10 tags
        /// </exception>
        Task<ProductTagDto> AssignTagToProduct(int userId, ProductTagAddDto productTagAdd);

        /// <summary>
        /// Removes the tag of product.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="productId">The product identifier.</param>
        /// <param name="tagId">The tag identifier.</param>
        /// <returns><c>true</c> if the tag was successfully removed; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">
        /// Product not found
        /// or
        /// Tag not found
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">You are not the owner of this product</exception>
        /// <exception cref="System.InvalidOperationException">Product does not have this tag</exception>
        Task<bool> RemoveTagOfProduct(int userId, int productId, int tagId);

        /// <summary>
        /// Removes all tags from the specified product.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="productId">The product identifier.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Product not found</exception>
        /// <exception cref="System.UnauthorizedAccessException">You are not the owner of this product</exception>
        Task RemoveTagsFromProduct(int userId, int productId);

        /// <summary>
        /// Tries to assign multiple tags to the product.
        /// Only valid and non-duplicate tags are added, up to a maximum of 10 tags per product.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="assignTags">The assign tags DTO containing the product ID and tag IDs.</param>
        /// <returns>A collection of <see cref="ProductTagDto"/> representing the tags that were successfully assigned.</returns>
        Task<IEnumerable<ProductTagDto>> TryAssignTagsToProduct(int userId, AssignTagsDto assignTags);
    }
}