namespace EntreLaunch.Web.Controllers.TrainingAPI
{
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.TrainingRoles)]
    public class TagsController(ITagService tagService, ILogger<TagsController> logger, ILocalizationManager localization) : AuthenticatedController(localization)
    {
        private readonly ITagService _tagService = tagService;
        private readonly ILogger<TagsController> _logger = logger;

        /// <summary>
        /// Creates a new tag with the specified name.
        /// </summary>
        /// <param name="tagName">The name of the tag to be added. Cannot be null or whitespace.</param>
        /// <returns>
        /// Returns 200 OK with the operation result if the tag is added successfully;
        /// otherwise returns an appropriate error response with failure details.
        /// </returns>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(TagPermissions.Create)]
        public async Task<IActionResult> AddTagAsync([FromQuery] string tagName)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                if (string.IsNullOrWhiteSpace(tagName))
                {
                    return BadRequest(new GeneralResult(false, "Tag Name is required", null));
                }

                var result = await _tagService.AddTagAsync(tagName);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UnExpected error occured while add tag in AddTagAsync method.");
                return StatusCode(StatusCodes.Status500InternalServerError, new GeneralResult { IsSuccess = false, Message = "An error occurred while adding tag." });
            }
        }

        /// <summary>
        /// Updates the name of an existing tag identified by its ID.
        /// </summary>
        /// <param name="tagId">The unique identifier of the tag to update.</param>
        /// <param name="newTagName">The new name to assign to the tag.</param>
        /// <returns>
        /// Returns 200 OK with the update result if successful;
        /// otherwise returns an appropriate error response.
        /// </returns>
        [HttpPatch("edit/{tagId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(TagPermissions.Edit)]
        public async Task<IActionResult> UpdateTagAsync([FromRoute] int tagId, [FromBody] string newTagName)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _tagService.UpdateTagAsync(tagId, newTagName);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UnExpected error occured while update tag in UpdateTagAsync method.");
                return StatusCode(StatusCodes.Status500InternalServerError, new GeneralResult { IsSuccess = false, Message = "An error occurred while updating tag." });
            }
        }

        /// <summary>
        /// Retrieves all existing tags in the system.
        /// </summary>
        /// <returns>
        /// Returns 200 OK with a list of tags if retrieval is successful;
        /// otherwise returns a relevant error response.
        /// </returns>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(TagPermissions.GetAll)]
        public async Task<IActionResult> GetAllTagsAsync([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var tags = await _tagService.GetAllTagsAsync(pagination, cancellationToken);
                if (tags.IsSuccess == false)
                {
                    return BadRequest(tags);
                }

                return Ok(tags);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UnExpected error occured while get all tags in GetAllTagsAsync method.");
                return StatusCode(StatusCodes.Status500InternalServerError, new GeneralResult { IsSuccess = false, Message = "An error occurred while getting all tags." });
            }
        }

        /// <summary>
        /// Retrieves the details of a tag by its unique identifier.
        /// </summary>
        /// <param name="tagId">The identifier of the tag to retrieve.</param>
        /// <returns>
        /// Returns 200 OK with tag details if found;
        /// returns 404 if the tag does not exist, or another error response if retrieval fails.
        /// </returns>
        [HttpGet("get-one/{tagId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(TagPermissions.GetOne)]
        public async Task<IActionResult> GetTagByIdAsync([FromRoute] int tagId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var tag = await _tagService.GetTagByIdAsync(tagId);
                if (tag.IsSuccess == false)
                {
                    return BadRequest(tag);
                }

                return Ok(tag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UnExpected error occured while get tag by id in GetTagByIdAsync method.");
                return StatusCode(StatusCodes.Status500InternalServerError, new GeneralResult { IsSuccess = false, Message = "An error occurred while getting tag by id." });
            }
        }

        /// <summary>
        /// Deletes a tag using soft-delete logic based on the provided tag ID.
        /// </summary>
        /// <param name="tagId">The ID of the tag to be deleted.</param>
        /// <returns>
        /// Returns 200 OK if the deletion is successful;
        /// otherwise returns an appropriate error message.
        /// </returns>
        [HttpDelete("delete/{tagId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(TagPermissions.Delete)]
        public async Task<IActionResult> DeleteTagAsync([FromRoute] int tagId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _tagService.DeleteTagAsync(tagId);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UnExpected error occured while delete tag in DeleteTagAsync method.");
                return StatusCode(StatusCodes.Status500InternalServerError, new GeneralResult { IsSuccess = false, Message = "An error occurred while deleting tag." });
            }
        }
    }
}
