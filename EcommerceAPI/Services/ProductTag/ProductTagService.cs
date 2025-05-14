using AutoMapper;
using EcommerceAPI.Models.DTOs.ProductTag;
using EcommerceAPI.Models.DTOs.ProductTags;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Services.Product.Interfaces;
using EcommerceAPI.Services.ProductTag.Interfaces;
using EcommerceAPI.Services.Tag;
using Elastic.Clients.Elasticsearch.Security;

namespace EcommerceAPI.Services.ProductTag
{
    /// <summary>
    /// Service for managing product tags.
    /// </summary>
    /// <seealso cref="EcommerceAPI.Services.ProductTag.Interfaces.IProductTagService" />
    public class ProductTagService : IProductTagService
    {
        private readonly IProductTagRepository _productTagRepository;
        private readonly IProductService _productService;
        private readonly ITagService _tagService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductTagService"/> class.
        /// </summary>
        /// <param name="productTagRepository">The product tag repository.</param>
        /// <param name="productService">The product service.</param>
        /// <param name="tagService">The tag service.</param>
        /// <param name="mapper">The mapper.</param>
        public ProductTagService(IProductTagRepository productTagRepository, IProductService productService, ITagService tagService,IMapper mapper)
        {
            _productTagRepository = productTagRepository;
            _productService = productService;
            _tagService = tagService;
            _mapper = mapper;
        }

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
        public async Task<ProductTagDto> AssignTagToProduct(int userId, ProductTagAddDto productTagAdd)
        {
            var product = await _productService.GetProductById(productTagAdd.ProductId);

            if (product == null)
                throw new KeyNotFoundException("Product not found");

            if (product.UserId != userId)
                throw new UnauthorizedAccessException("You are not the owner of this product");

            var tag = await _tagService.GetTagById(productTagAdd.TagId);

            if (tag == null)
                throw new KeyNotFoundException("Tag not found");

            var existingProductTag = await _productTagRepository.ProductHasTag(productTagAdd.ProductId, productTagAdd.TagId);

            if (existingProductTag)
                throw new InvalidOperationException("Product already has this tag");

            var tagIds = await _productTagRepository.GetTagIdsForProduct(productTagAdd.ProductId);

            if (tagIds.Count() >= 10)
                throw new InvalidOperationException("Product already has 10 tags");

            var productTagEntity = _mapper.Map<ProductTagEntity>(productTagAdd);
            var productTag = await _productTagRepository.AddProductTag(productTagEntity);

            return _mapper.Map<ProductTagDto>(productTag);
        }

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
        public async Task<bool> RemoveTagOfProduct(int userId, int productId, int tagId)
        {
            var product = await _productService.GetProductById(productId);

            if (product == null)
                throw new KeyNotFoundException("Product not found");

            if (product.UserId != userId)
                throw new UnauthorizedAccessException("You are not the owner of this product");

            var tag = await _tagService.GetTagById(tagId);

            if (tag == null)
                throw new KeyNotFoundException("Tag not found");

            var existingProductTag = await _productTagRepository.ProductHasTag(productId, tagId);

            if (!existingProductTag)
                throw new InvalidOperationException("Product does not have this tag");

            return await _productTagRepository.RemoveProductTag(productId, tagId);
        }

        /// <summary>
        /// Tries to assign multiple tags to the product.
        /// Only valid and non-duplicate tags are added, up to a maximum of 10 tags per product.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="assignTags">The assign tags DTO containing the product ID and tag IDs.</param>
        /// <returns>A collection of <see cref="ProductTagDto"/> representing the tags that were successfully assigned.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Product not found</exception>
        /// <exception cref="System.UnauthorizedAccessException">You are not the owner of this product</exception>
        public async Task<IEnumerable<ProductTagDto>> TryAssignTagsToProduct(int userId, AssignTagsDto assignTags)
        {
            var product = await _productService.GetProductById(assignTags.ProductId);

            if (product == null)
                throw new KeyNotFoundException("Product not found");

            if (product.UserId != userId)
                throw new UnauthorizedAccessException("You are not the owner of this product");

            var validTags = await _tagService.FilterExistingTags(assignTags.TagIds);
            var existingTags = await _productTagRepository.GetTagIdsForProduct(assignTags.ProductId);
            var newTags = validTags.Except(existingTags).ToList();

            if (!newTags.Any())
                return Enumerable.Empty<ProductTagDto>();

            if (existingTags.Count() + newTags.Count() > 10)
                newTags = newTags.Take(10 - existingTags.Count()).ToList();

            var productTags = await _productTagRepository.AddRangeProductTag(newTags.Select(tagId => new ProductTagEntity
            {
                ProductId = assignTags.ProductId,
                TagId = tagId
            }));

            return _mapper.Map<IEnumerable<ProductTagDto>>(productTags);
        }

        /// <summary>
        /// Removes all tags from the specified product.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="productId">The product identifier.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Product not found</exception>
        /// <exception cref="System.UnauthorizedAccessException">You are not the owner of this product</exception>
        public async Task RemoveTagsFromProduct(int userId, int productId)
        {
            var product = await _productService.GetProductById(productId);

            if (product == null)
                throw new KeyNotFoundException("Product not found");

            if (product.UserId != userId)
                throw new UnauthorizedAccessException("You are not the owner of this product");

            await _productTagRepository.RemoveTagsFromProduct(productId);
        }
    }
}