namespace EntreLaunch.Web.Controllers.TrainingAPI
{
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.TrainingRoles)]
    public class CourseFieldController(
    ILocalizationManager localization,
    ICourseFieldService courseFieldService,
    CascadeDeleteService deleteService,
    ILogger<CourseFieldController> logger) : AuthenticatedController(localization)
    {
        private readonly ILogger<CourseFieldController> _logger = logger;
        private readonly ICourseFieldService _courseFieldService = courseFieldService;
        private readonly CascadeDeleteService _deleteService = deleteService;

        /// <summary>
        /// Create Course Field.
        /// </summary>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseFieldPermissions.Create)]
        public async Task<IActionResult> Create([FromBody] CourseFieldCreateDto createDto, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                var result = await _courseFieldService.CreateAsync(createDto, cancellationToken);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course field Create method.");
                return this.UnexpectedError("course field Create.");
            }
        }

        /// <summary>
        /// update Course Field.
        /// </summary>
        [HttpPatch("edit/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseFieldPermissions.Edit)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CourseFieldUpdateDto updateDto, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _courseFieldService.UpdateAsync(id, updateDto, cancellationToken);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course field Patch method.");
                return this.UnexpectedError("course field update.");
            }
        }

        /// <summary>
        /// Get All Course Fields.
        /// </summary>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseFieldPermissions.GetAll)]
        public async Task<IActionResult> AllCourseFields([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var result = await _courseFieldService.GetAllAsync(pagination, cancellationToken);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course field GetAll method.");
                return this.UnexpectedError("getting course fields.");
            }
        }

        /// <summary>
        /// Get One Course Field.
        /// </summary>
        [HttpGet("get-one/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseFieldPermissions.GetOne)]
        public async Task<ActionResult<CourseFieldDetailsDto>> GetOne([FromRoute] int id, CancellationToken cancellationToken)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var result = await _courseFieldService.GetOneAsync(id, cancellationToken);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course field GetOne method.");
                return this.UnexpectedError("e getting course field.");
            }
        }

        /// <summary>
        /// Delete Course Field.
        /// </summary>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseFieldPermissions.Delete)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _deleteService.SoftDeleteCascadeAsync<CourseField>(id);
                return result.IsSuccess == true ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while soft deleting course with ID {Id}.", id);
                return this.UnexpectedError("soft deleting the entity.");
            }
        }
    }
}
