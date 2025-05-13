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
        /// Adds the tags product.
        /// </summary>
        /// <param name="productTagsAddDto">The product tags add dto.</param>
        Task AddTagsProduct(ProductTagsAddDto productTagsAddDto);

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