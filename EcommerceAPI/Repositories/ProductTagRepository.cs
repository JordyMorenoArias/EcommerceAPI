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
        /// Adds the product tag.
        /// </summary>
        /// <param name="productTagEntity">The product tag entity.</param>
        /// <returns>The added product tag entity.</returns>
        public async Task<ProductTagEntity> AddProductTag(ProductTagEntity productTagEntity)
        {
            await _context.ProductTags.AddAsync(productTagEntity);
            await _context.SaveChangesAsync();
            return productTagEntity;
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
        /// Adds the tags product.
        /// </summary>
        /// <param name="productTagsAddDto">The product tags add dto.</param>
        public async Task AddTagsProduct(ProductTagsAddDto productTagsAddDto)
        {
            var existingTags = await _context.ProductTags
                .Where(pt => pt.ProductId == productTagsAddDto.ProductId)
                .Select(pt => pt.TagId)
                .ToListAsync();

            var newTags = productTagsAddDto.TagIds.Except(existingTags).ToList();

            foreach (var tagId in newTags)
            {
                var productTag = new ProductTagEntity
                {
                    ProductId = productTagsAddDto.ProductId,
                    TagId = tagId
                };

                await _context.ProductTags.AddAsync(productTag);
            }

            await _context.SaveChangesAsync();
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
