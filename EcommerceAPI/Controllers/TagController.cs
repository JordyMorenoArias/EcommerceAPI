using EcommerceAPI.Constants;
using EcommerceAPI.Filters;
using EcommerceAPI.Models.DTOs.Tag;
using EcommerceAPI.Services.Tag;
using Microsoft.AspNetCore.Mvc;


namespace EcommerceAPI.Controllers
{
    /// <summary>
    /// Controller responsible for managing tag-related operations.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [ApiController]
    [Route("api/[controller]")]
    public class TagController : Controller
    {
        private readonly ITagService _tagService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TagController"/> class.
        /// </summary>
        /// <param name="tagService">The tag service.</param>
        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }

        /// <summary>
        /// Gets the tag by identifier.
        /// </summary>
        /// <param name="id">The identifier of the tag.</param>
        /// <returns>Returns the tag with the specified identifier.</returns>
        [HttpGet("{id}")]
        [AuthorizeRole(UserRole.Admin, UserRole.Seller)]
        public async Task<IActionResult> GetTagById([FromRoute] int id)
        {
            var tag = await _tagService.GetTagById(id);
            return Ok(tag);
        }

        /// <summary>
        /// Gets the tags.
        /// </summary>
        /// <param name="parameters">The filter and pagination parameters.</param>
        /// <returns>Returns a list of tags based on the given parameters.</returns>
        [HttpGet]
        [AuthorizeRole(UserRole.Admin, UserRole.Seller)]
        public async Task<IActionResult> GetTags([FromQuery] GetTagParameters parameters)
        {
            var tags = await _tagService.GetTags(parameters);
            return Ok(tags);
        }

        /// <summary>
        /// Searches the tags.
        /// </summary>
        /// <param name="parameters">The search parameters including query and filters.</param>
        /// <returns>Returns a filtered list of tags that match the search criteria.</returns>
        [HttpGet("search")]
        [AuthorizeRole(UserRole.Admin, UserRole.Seller)]
        public async Task<IActionResult> SearchTags([FromQuery] SearchTagParameters parameters)
        {
            var tags = await _tagService.SearchTags(parameters);
            return Ok(tags);
        }

        /// <summary>
        /// Adds a new tag.
        /// </summary>
        /// <param name="tagAdd">The tag data to create.</param>
        /// <returns>Returns the created tag with its identifier.</returns>
        [HttpPost]
        [AuthorizeRole(UserRole.Admin)]
        public async Task<IActionResult> AddTag([FromBody] TagAddDto tagAdd)
        {
            var tag = await _tagService.AddTag(tagAdd);
            return CreatedAtAction(nameof(GetTagById), new { id = tag.Id }, tag);
        }

        /// <summary>
        /// Updates an existing tag.
        /// </summary>
        /// <param name="tagUpdate">The tag data to update.</param>
        /// <returns>Returns the updated tag.</returns>
        [HttpPut]
        [AuthorizeRole(UserRole.Admin)]
        public async Task<IActionResult> UpdateTag([FromBody] TagUpdateDto tagUpdate)
        {
            var tag = await _tagService.UpdateTag(tagUpdate);
            return Ok(tag);
        }

        /// <summary>
        /// Deletes the tag by identifier.
        /// </summary>
        /// <param name="id">The identifier of the tag to delete.</param>
        /// <returns>Returns 204 No Content if deleted successfully; otherwise, 404 Not Found.</returns>
        [HttpDelete("{id}")]
        [AuthorizeRole(UserRole.Admin)]
        public async Task<IActionResult> DeleteTag([FromRoute] int id)
        {
            var result = await _tagService.DeleteTag(id);

            if (result)
                return NoContent();

            return NotFound();
        }
    }
}