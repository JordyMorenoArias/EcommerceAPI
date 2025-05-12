using AutoMapper;
using EcommerceAPI.Models.DTOs;
using EcommerceAPI.Models.DTOs.Tag;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Services.ElasticService.Interfaces;
using EcommerceAPI.Services.Infrastructure.Interfaces;

namespace EcommerceAPI.Services.Tag
{
    /// <summary>
    /// Service for managing tags.
    /// </summary>
    /// <seealso cref="EcommerceAPI.Services.Tag.ITagService" />
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;
        private readonly IElasticGenericService<TagDto> _elasticGenericService;
        private readonly IElasticTagService _elasticTagService;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="TagService"/> class.
        /// </summary>
        /// <param name="tagRepository">The tag repository.</param>
        /// <param name="elasticGenericService">The elastic generic service.</param>
        /// <param name="elasticTagService">The elastic tag service.</param>
        /// <param name="cacheService">The cache service.</param>
        /// <param name="mapper">The mapper.</param>
        public TagService(ITagRepository tagRepository, IElasticGenericService<TagDto> elasticGenericService, IElasticTagService elasticTagService, ICacheService cacheService, IMapper mapper)
        {
            _tagRepository = tagRepository;
            _elasticGenericService = elasticGenericService;
            _elasticTagService = elasticTagService;
            _cacheService = cacheService;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets the tag by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The tag with the specified identifier.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Tag with ID {id} not found.</exception>
        public async Task<TagDto> GetTagById(int id)
        {
            var cacheKey = $"tag_{id}";
            var cachedTag = await _cacheService.Get<TagDto>(cacheKey);

            if (cachedTag is not null)
                return cachedTag;

            var tag = await _tagRepository.GetTagById(id);

            if (tag == null)
                throw new KeyNotFoundException($"Tag with ID {id} not found.");

            var tagDto = _mapper.Map<TagDto>(tag);
            await _cacheService.Set(cacheKey, tagDto, TimeSpan.FromMinutes(5));
            return tagDto;
        }

        /// <summary>
        /// Searches the tags.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A paged result containing tags that match the search term.</returns>
        /// <exception cref="System.ArgumentException">Page and PageSize must be greater than 0.</exception>
        public async Task<PagedResult<TagDto>> SearchTags(SearchTagParameters parameters)
        {
            if (string.IsNullOrEmpty(parameters.SearchTerm))
            {
                return new PagedResult<TagDto>
                {
                    Items = new List<TagDto>(),
                    TotalItems = 0,
                    Page = parameters.Page,
                    PageSize = parameters.PageSize
                };
            }

            if (parameters.Page <= 0 || parameters.PageSize <= 0)
                throw new ArgumentException("Page and PageSize must be greater than 0.");

            var tags = await _elasticTagService.SearchTags(parameters);

            return new PagedResult<TagDto>
            {
                Items = tags.Items,
                TotalItems = tags.TotalItems,
                Page = tags.Page,
                PageSize = tags.PageSize
            };
        }

        /// <summary>
        /// Gets the tags.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A paged result containing all tags.</returns>
        /// <exception cref="System.ArgumentException">Page and PageSize must be greater than 0.</exception>
        public async Task<PagedResult<TagDto>> GetTags(GetTagParameters parameters)
        {
            if (parameters.Page <= 0 || parameters.PageSize <= 0)
                throw new ArgumentException("Page and PageSize must be greater than 0.");

            var cacheKey = $"tags_{parameters.Page}_{parameters.PageSize}";

            var cachedTags = await _cacheService.Get<PagedResult<TagDto>>(cacheKey);

            if (cachedTags is not null)
                return cachedTags;

            var tags = await _tagRepository.GetTags(parameters);
            var tagsDtos = _mapper.Map<PagedResult<TagDto>>(tags);

            await _cacheService.Set(cacheKey, tagsDtos, TimeSpan.FromMinutes(5));
            return tagsDtos;
        }

        /// <summary>
        /// Adds the tag.
        /// </summary>
        /// <param name="tagAdd">The tag add.</param>
        /// <returns>The newly created tag.</returns>
        public async Task<TagDto> AddTag(TagAddDto tagAdd)
        {
            var tagEntity = _mapper.Map<TagEntity>(tagAdd);
            var tag = await _tagRepository.AddTag(tagEntity);
            var tagDto = _mapper.Map<TagDto>(tag);

            await _elasticGenericService.Index(tagDto, tagDto.Id.ToString());
            return tagDto;
        }

        /// <summary>
        /// Updates the tag.
        /// </summary>
        /// <param name="tagUpdate">The tag update.</param>
        /// <returns>The updated tag.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Tag with ID {tagUpdate.Id} not found.</exception>
        public async Task<TagDto> UpdateTag(TagUpdateDto tagUpdate)
        {
            var existingTag = await _tagRepository.GetTagById(tagUpdate.Id);

            if (existingTag == null)
                throw new KeyNotFoundException($"Tag with ID {tagUpdate.Id} not found.");

            var tagEntity = _mapper.Map<TagEntity>(tagUpdate);
            var tag = await _tagRepository.UpdateTag(tagEntity);
            var tagDto = _mapper.Map<TagDto>(tag);

            await _cacheService.Remove($"tag_{tagDto.Id}");
            await _elasticGenericService.Index(tagDto, tagDto.Id.ToString());
            return tagDto;
        }

        /// <summary>
        /// Deletes the tag.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns><c>true</c> if the tag was successfully deleted; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Tag with ID {id} not found.</exception>
        public async Task<bool> DeleteTag(int id)
        {
            var tag = await _tagRepository.GetTagById(id);

            if (tag == null)
                throw new KeyNotFoundException($"Tag with ID {id} not found.");

            var isDeleted = await _tagRepository.DeleteTag(tag.Id);

            if (isDeleted)
            {
                await _cacheService.Remove($"tag_{id}");
                await _elasticGenericService.Delete(id.ToString());
            }

            return isDeleted;
        }
    }
}