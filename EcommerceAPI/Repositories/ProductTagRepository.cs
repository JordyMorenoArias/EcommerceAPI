using EcommerceAPI.Data;
using EcommerceAPI.Models.DTOs;
using EcommerceAPI.Models.DTOs.ProductTag;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Repositories
{
    /// <summary>
    /// Repository for managing product tags in the database.
    /// </summary>
    public class ProductTagRepository : IProductTagRepository
    {
        private readonly EcommerceContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductTagRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public ProductTagRepository(EcommerceContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all tag IDs associated with the specified product.
        /// </summary>
        /// <param name="productId">The ID of the product to query for tags.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of tag IDs associated with the product.</returns>
        public async Task<IEnumerable<int>> GetTagIdsForProduct(int productId)
        {
            return await _context.ProductTags
                .Where(pt => pt.ProductId == productId)
                .Select(pt => pt.TagId)
                .ToListAsync();
        }

        /// <summary>
        /// Adds the product tag.
        /// </summary>
        /// <param name="entity">The product tag entity.</param>
        /// <returns>The added product tag entity.</returns>
        public async Task<ProductTagEntity> AddProductTag(ProductTagEntity entity)
        {
            await _context.ProductTags.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Removes the product tag.
        /// </summary>
        /// <param name="productId">The product identifier.</param>
        /// <param name="tagId">The tag identifier.</param>
        /// <returns>True if the tag was removed, false if the tag wasn't found.</returns>
        public async Task<bool> RemoveProductTag(int productId, int tagId)
        {
            var entity = await _context.ProductTags.FirstOrDefaultAsync(pt => pt.ProductId == productId && pt.TagId == tagId);

            if (entity == null)
                return false;

            _context.ProductTags.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Products the has tag.
        /// </summary>
        /// <param name="productId">The product identifier.</param>
        /// <param name="tagId">The tag identifier.</param>
        /// <returns>True if the product has the tag, otherwise false.</returns>
        public async Task<bool> ProductHasTag(int productId, int tagId)
        {
            return await _context.ProductTags.AnyAsync(pt => pt.ProductId == productId && pt.TagId == tagId);
        }

        /// <summary>
        /// Adds the range product tag.
        /// </summary>
        /// <param name="entities">The entities.</param>
        /// <returns>The input entities with all database-generated values updated, in the same order as provided.</returns>
        public async Task<IEnumerable<ProductTagEntity>> AddRangeProductTag(IEnumerable<ProductTagEntity> entities)
        {
            await _context.ProductTags.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
            return entities;
        }

        /// <summary>
        /// Removes the tags from product.
        /// </summary>
        /// <param name="productId">The product identifier.</param>
        public async Task RemoveTagsFromProduct(int productId)
        {
            var productTags = await _context.ProductTags.Where(pt => pt.ProductId == productId).ToListAsync();

            if (productTags.Count > 0)
            {
                _context.ProductTags.RemoveRange(productTags);
                await _context.SaveChangesAsync();
            }
        }
    }
}
