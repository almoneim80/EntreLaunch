namespace EntreLaunch.Web.Controllers.TrainingAPI
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    public class CourseTagsController : AuthenticatedController
    {
        private readonly ITagService _tagService;
        private readonly ILogger<CourseTagsController> _logger;
        public CourseTagsController(ITagService tagService, ILogger<CourseTagsController> logger)
        {
            _tagService = tagService;
            _logger = logger;
        }

        /// <summary>
        /// Linking tags to a specific course.
        /// </summary>
        [HttpPost("to-course")]
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
        /// Display tags associated with a particular course.
        /// </summary>
        [HttpGet("by-course/{courseId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseTagsPermissions.GetByCourse)]
        public async Task<IActionResult> GetTagsForCourseAsync(int courseId)
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
        /// Display courses associated with a specific tag.
        /// </summary>
        [HttpGet("by-tag/{tagId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseTagsPermissions.GetByTag)]
        public async Task<IActionResult> GetCoursesByTagAsync(int tagId)
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
        /// Delete tags from a particular course.
        /// </summary>
        [HttpDelete("delete")]
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
