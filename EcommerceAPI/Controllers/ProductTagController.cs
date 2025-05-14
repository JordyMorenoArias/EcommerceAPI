using EcommerceAPI.Constants;
using EcommerceAPI.Filters;
using EcommerceAPI.Models.DTOs.ProductTag;
using EcommerceAPI.Services.ProductTag.Interfaces;
using EcommerceAPI.Services.User.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{
    /// <summary>
    /// Controller for managing product tags.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeRole(UserRole.Seller)]
    public class ProductTagController : Controller
    {
        private readonly IProductTagService _productTagService;
        private readonly IUserService _userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductTagController"/> class.
        /// </summary>
        /// <param name="productTagService">The product tag service.</param>
        /// <param name="userService">The user service.</param>
        public ProductTagController(IProductTagService productTagService, IUserService userService)
        {
            _productTagService = productTagService;
            _userService = userService;
        }

        /// <summary>
        /// Assigns the tag to product.
        /// </summary>
        /// <param name="productTagAdd">The product tag add.</param>
        /// <returns>Returns a 201 Created response with the result of the assignment.</returns>
        [HttpPost("tags/assign")]
        public async Task<IActionResult> AssignTagToProduct([FromBody] ProductTagAddDto productTagAdd)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var result = await _productTagService.AssignTagToProduct(userAuthenticated.Id, productTagAdd);
            return CreatedAtAction(nameof(AssignTagToProduct), result);
        }

        /// <summary>
        /// Removes the tag of product.
        /// </summary>
        /// <param name="productId">The product identifier.</param>
        /// <param name="tagId">The tag identifier.</param>
        /// <returns>Returns 204 No Content if successful, or 404 Not Found if the tag was not found.</returns>
        [HttpDelete("products/{productId}/tags/{tagId}")]
        public async Task<IActionResult> RemoveTagOfProduct([FromRoute] int productId, [FromRoute] int tagId)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var result = await _productTagService.RemoveTagOfProduct(userAuthenticated.Id, productId, tagId);
            return result ? NoContent() : NotFound();
        }

        /// <summary>
        /// Tries the assign tags to product.
        /// </summary>
        /// <param name="assignTags">The assign tags.</param>
        /// <returns>Returns 200 OK with a list of successfully assigned tags.</returns>
        [HttpPost("products/try-assign-tags")]
        public async Task<IActionResult> TryAssignTagsToProduct([FromBody] AssignTagsDto assignTags)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var result = await _productTagService.TryAssignTagsToProduct(userAuthenticated.Id, assignTags);
            return Ok(result);
        }

        /// <summary>
        /// Removes the tags from product.
        /// </summary>
        /// <param name="productId">The product identifier.</param>
        /// <returns>Returns 204 No Content when all tags are successfully removed.</returns>
        [HttpDelete("products/{productId}/tags")]
        public async Task<IActionResult> RemoveTagsFromProduct([FromRoute] int productId)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            await _productTagService.RemoveTagsFromProduct(userAuthenticated.Id, productId);
            return NoContent();
        }
    }
}