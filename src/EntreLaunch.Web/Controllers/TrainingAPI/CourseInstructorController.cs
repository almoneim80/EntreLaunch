namespace EntreLaunch.Web.Controllers.TrainingAPI
{
    [Authorize(Roles = AppRoles.TrainingRoles)]
    [Route("api/courses/instructors")]
    public class CourseInstructorController(
        ILocalizationManager localization,
        ILogger<CourseInstructorController> logger,
        ICourseInstructorService courseInstructorService,
        UserManager<User> userManager) : AuthenticatedController(localization)
    {
        private readonly ICourseInstructorService _courseInstructorService = courseInstructorService;
        private readonly ILogger<CourseInstructorController> _logger = logger;
        protected readonly UserManager<User> _userManager = userManager;

        /// <summary>
        /// Creates a new course instructor entry linking a trainer to a course.
        /// Validates that the referenced course and trainer exist and are valid.
        /// Requires the user to have permission to create course instructors.
        /// </summary>
        /// <param name="createDto">
        /// A <see cref="CourseInstructorCreateDto"/> containing the details of the course and instructor association.
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing the created course instructor details.
        /// </returns>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseInstructorPermissions.Create)]
        public async Task<IActionResult> Create([FromBody] CourseInstructorCreateDto createDto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                var result = await _courseInstructorService.CreateAsync(createDto);
                return result.IsSuccess == true ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in create course instructor");
                return this.UnexpectedError("create course instructor.");
            }
        }

        /// <summary>
        /// Updates an existing course instructor entry identified by its ID.
        /// Requires the user to have permission to edit course instructors.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the course instructor to update.
        /// </param>
        /// <param name="updateDto">
        /// A <see cref="CourseInstructorUpdateDto"/> object containing the updated data.
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing the updated course instructor details.
        /// </returns>
        [HttpPatch("update/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseInstructorPermissions.Edit)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CourseInstructorUpdateDto updateDto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _courseInstructorService.UpdateAsync(id, updateDto);
                return result.IsSuccess == true ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course instructor Patch method.");
                return this.UnexpectedError("course instructor update ");
            }
        }

        /// <summary>
        /// Retrieves all course instructors available in the system.
        /// Requires the user to have permission to view all course instructors.
        /// </summary>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing an array of <see cref="CourseInstructorDetailsDto"/>.
        /// </returns>
        [HttpGet("all")]
        [ProducesResponseType(typeof(PaginatedResult<CourseInstructorDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseInstructorPermissions.GetAll)]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _courseInstructorService.GetAllAsync(pagination, cancellationToken);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course instructor GetAll method.");
                return this.UnexpectedError("course instructor get all.");
            }
        }

        /// <summary>
        /// Retrieves the details of a specific course instructor by its ID.
        /// Requires the user to have permission to view a specific course instructor.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the course instructor to retrieve.
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing the course instructor details.
        /// </returns>
        [HttpGet("get-one/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseInstructorPermissions.GetOne)]
        public async Task<IActionResult> GetOne([FromRoute] int id)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _courseInstructorService.GetOneAsync(id);
                return result.IsSuccess == true ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course instructor GetOne method.");
                return this.UnexpectedError("course instructor get one.");
            }
        }

        /// <summary>
        /// Retrieves the list of instructors associated with a specific course by its ID.
        /// Requires the user to have permission to view instructors by course.
        /// </summary>
        [HttpGet("by-course/{courseId:int}")]
        [ProducesResponseType(typeof(PaginatedResult<CourseInstructorDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseInstructorPermissions.GetInstructorsByCourse)]
        public async Task<IActionResult> GetInstructorsByCourseIdAsync([FromRoute] int courseId, [FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _courseInstructorService.GetInstructorsByCourseIdAsync(courseId, pagination, cancellationToken);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course instructor GetInstructorsByCourseIdAsync method.");
                return this.UnexpectedError("course instructor get instructors by course");
            }
        }

        /// <summary>
        /// Deletes an existing course instructor entry identified by its ID.
        /// Requires the user to have permission to delete course instructors.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the course instructor to delete.
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult"/> indicating the result of the delete operation.
        /// </returns>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseInstructorPermissions.Delete)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _courseInstructorService.DeleteAsync(id);
                return result.IsSuccess == true ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course instructor Delete method.");
                return this.UnexpectedError("deleting course instructor.");
            }
        }
    }
}

