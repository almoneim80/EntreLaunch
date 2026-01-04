namespace EntreLaunch.Web.Controllers.TrainingAPI
{
    [Authorize(Roles = AppRoles.TrainingRoles)]
    [Route("api/[controller]")]
    public class CourseTagsController(ITagService tagService, ILogger<CourseTagsController> logger, ILocalizationManager localization) : AuthenticatedController(localization)
    {
        private readonly ITagService _tagService = tagService;
        private readonly ILogger<CourseTagsController> _logger = logger;

        /// <summary>
        /// Assigns a set of tags to a specific course.
        /// Validates user identity and model state before assignment.
        /// Requires the user to have permission to assign tags to courses.
        /// </summary>
        /// <param name="assginDto">
        /// An <see cref="AssignTagsDto"/> object containing the course ID and a list of tag IDs to assign.
        /// </param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// </returns>
        [HttpPost("to-course")]
        [SwaggerOperation(Tags = new[] { "Tags" })]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseTagsPermissions.AssignToCourse)]
        public async Task<IActionResult> AssignTagsToCourseAsync([FromBody] AssignTagsDto assginDto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                var result = await _tagService.AssignTagsToCourseAsync(assginDto.CourseId, assginDto.TagIds);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
               _logger.LogError(ex, "An error occurred in AssignTagsToCourseAsync.");
               return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in AssignTagsToCourseAsync." });
            }
        }

        /// <summary>
        /// Retrieves all tags associated with a specific course.
        /// Requires the user to have permission to view tags by course.
        /// </summary>
        /// <param name="courseId">
        /// The unique identifier of the course whose tags are to be retrieved.
        /// </param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the list of tags associated with the specified course.
        /// </returns>
        [HttpGet("by-course/{courseId:int}")]
        [SwaggerOperation(Tags = new[] { "Tags" })]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseTagsPermissions.GetByCourse)]
        public async Task<IActionResult> GetTagsForCourseAsync([FromRoute] int courseId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                if (courseId <= 0)
                {
                    return BadRequest(new GeneralResult { IsSuccess = false, Message = "Invalid course ID." });
                }

                var tags = await _tagService.GetTagsForCourseAsync(courseId);
                if (tags.IsSuccess == false)
                {
                    return BadRequest(tags);
                }

                return Ok(tags);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetTagsForCourseAsync.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in GetTagsForCourseAsync." });
            }
        }

        /// <summary>
        /// Retrieves all courses associated with a tag name (case-insensitive).
        /// Requires the user to have permission to view courses by tag.
        /// </summary>
        /// <param name="tagName">The name of the tag to search courses by.</param>
        /// <returns>A list of course names associated with the specified tag.</returns>
        [HttpGet("by-tag-name")]
        [SwaggerOperation(Tags = new[] { "Tags" })]
        [ProducesResponseType(typeof(GeneralResult<List<string>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseTagsPermissions.GetCoursesByTag)]
        public async Task<IActionResult> GetCoursesByTagNameAsync([FromQuery] string tagName)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                if (string.IsNullOrWhiteSpace(tagName))
                {
                    return BadRequest(new GeneralResult { IsSuccess = false, Message = "Tag name must be provided." });
                }

                var result = await _tagService.GetCoursesByTagNameAsync(tagName);
                if (!result.IsSuccess)
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Courses Not Found",
                        Detail = result.Message
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetCoursesByTagNameAsync.");
                return StatusCode(500, new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An error occurred in GetCoursesByTagNameAsync."
                });
            }
        }

        /// <summary>
        /// Retrieves all courses that are associated with a specific tag.
        /// Requires the user to have permission to view courses by tag.
        /// </summary>
        /// <param name="tagId">
        /// The unique identifier of the tag for which related courses are to be retrieved.
        /// </param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the list of courses associated with the specified tag.
        /// </returns>
        [HttpGet("by-tag/{tagId:int}")]
        [SwaggerOperation(Tags = new[] { "Tags" })]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseTagsPermissions.GetByTag)]
        public async Task<IActionResult> GetCoursesByTagAsync([FromRoute] int tagId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                if (tagId <= 0)
                {
                    return BadRequest(new GeneralResult { IsSuccess = false, Message = "Invalid tag ID." });
                }

                var courses = await _tagService.GetCoursesByTagAsync(tagId);
                if (courses.IsSuccess == false)
                {
                    return BadRequest(courses);
                }

                return Ok(courses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetCoursesByTagAsync.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in GetCoursesByTagAsync." });
            }
        }

        /// <summary>
        /// Removes one or more tags from a specific course.
        /// Requires the user to have permission to remove tags from courses.
        /// </summary>
        /// <param name="dto">
        /// An <see cref="AssignTagsDto"/> containing the course ID and a list of tag IDs to remove.
        /// </param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// </returns>
        [HttpDelete("delete")]
        [SwaggerOperation(Tags = new[] { "Tags" })]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseTagsPermissions.RemoveFromCourse)]
        public async Task<IActionResult> RemoveTagsFromCourseAsync([FromBody] AssignTagsDto dto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _tagService.RemoveTagsFromCourseAsync(dto.CourseId, dto.TagIds);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in RemoveTagsFromCourseAsync.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in RemoveTagsFromCourseAsync." });
            }
        }
    }
}
