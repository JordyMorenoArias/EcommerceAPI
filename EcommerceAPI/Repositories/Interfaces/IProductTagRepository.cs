using EcommerceAPI.Models.DTOs.ProductTag;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Repositories.Interfaces
{
    public interface IProductTagRepository
    {
        /// <summary>
        /// Adds the product tag.
        /// </summary>
        /// <param name="productTagEntity">The product tag entity.</param>
        /// <returns>The added product tag entity.</returns>
        Task<ProductTagEntity> AddProductTag(ProductTagEntity productTagEntity);

        /// <summary>
        /// Adds the range product tag.
        /// </summary>
        /// <param name="entities">The entities.</param>
        /// <returns>The input entities with all database-generated values updated, in the same order as provided.</returns>
        Task<IEnumerable<ProductTagEntity>> AddRangeProductTag(IEnumerable<ProductTagEntity> entities);


        /// <summary>
        /// Retrieves all tag IDs associated with the specified product.
        /// </summary>
        /// <param name="productId">The ID of the product to query for tags.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains an enumerable of tag IDs associated with the product.
        /// </returns>
        Task<IEnumerable<int>> GetTagIdsForProduct(int productId);

        /// <summary>
        /// Products the has tag.
        /// </summary>
        /// <param name="productId">The product identifier.</param>
        /// <param name="tagId">The tag identifier.</param>
        /// <returns>True if the product has the tag, otherwise false.</returns>
        Task<bool> ProductHasTag(int productId, int tagId);

        /// <summary>
        /// Removes the product tag.
        /// </summary>
        /// <param name="productId">The product identifier.</param>
        /// <param name="tagId">The tag identifier.</param>
        /// <returns>True if the tag was removed, false if the tag wasn't found.</returns>
        Task<bool> RemoveProductTag(int productId, int tagId);

        /// <summary>
        /// Removes the tags from product.
        /// </summary>
        /// <param name="productId">The product identifier.</param>
        Task RemoveTagsFromProduct(int productId);
    }
}