namespace EntreLaunch.Web.Controllers.TrainingAPI
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class TagsController : AuthenticatedController
    {
        private readonly ITagService _tagService;
        private readonly ILogger<TagsController> _logger;
        public TagsController(ITagService tagService, ILogger<TagsController> logger)
        {
            _tagService = tagService;
            _logger = logger;
        }

        /// <summary>
        /// Add a new tag.
        /// </summary>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(TagPermissions.Create)]
        public async Task<IActionResult> AddTagAsync([FromBody] string tagName)
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
        /// EntreLaunchdate a tag.
        /// </summary>
        [HttpPatch("edit/{tagId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(TagPermissions.Edit)]
        public async Task<IActionResult> EntreLaunchdateTagAsync(int tagId, [FromBody] string newTagName)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _tagService.EntreLaunchdateTagAsync(tagId, newTagName);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UnExpected error occured while EntreLaunchdate tag in EntreLaunchdateTagAsync method.");
                return StatusCode(StatusCodes.Status500InternalServerError, new GeneralResult { IsSuccess = false, Message = "An error occurred while EntreLaunchdating tag." });
            }
        }

        /// <summary>
        /// View all tags.  
        /// </summary>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(TagPermissions.ShowAll)]
        public async Task<IActionResult> GetAllTagsAsync()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var tags = await _tagService.GetAllTagsAsync();
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
        /// Get a tag by ID.
        /// </summary>
        [HttpGet("get-one/{tagId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(TagPermissions.ShowOne)]
        public async Task<IActionResult> GetTagByIdAsync(int tagId)
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
        /// Soft delete a tag.
        /// </summary>
        [HttpDelete("delete/{tagId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(TagPermissions.Delete)]
        public async Task<IActionResult> DeleteTagAsync(int tagId)
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
