namespace EntreLaunch.Web.Controllers.TrainingAPI
{
    [Authorize(Roles = AppRoles.TrainingRoles)]
    [Route("api/courses/path")]
    public class PathCoursesController(
        ILogger<PathCoursesController> logger,
        ILocalizationManager localization,
        ICourseService courseService,
        IPathCourseService pathCourseService,
        CascadeDeleteService deleteService) : AuthenticatedController(localization)
    {
        private readonly ILogger<PathCoursesController> _logger = logger;
        private readonly ICourseService _courseService = courseService;
        private readonly IPathCourseService _pathCourseService = pathCourseService;
        private readonly CascadeDeleteService _deleteService = deleteService;

        /// <summary>
        /// Creates a new path course using the provided data transfer object.
        /// Requires the user to have the PathCourse.Create permission.
        /// </summary>
        /// <param name="dto">An object containing the required information to create the path course.</param>
        /// <returns>
        /// Returns an IActionResult:
        /// - 200 OK if the course was successfully created.
        /// - 400 Bad Request if validation fails or the creation is unsuccessful.
        /// - 401 Unauthorized if the user lacks permissions.
        /// - 404 Not Found or 500 Internal Server Error on failure.
        /// </returns>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(PathCoursePermissions.Create)]
        public async Task<IActionResult> Create([FromBody] PathCourseCreateDto dto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                var result = await _courseService.CreateAsync<PathCourseCreateDto>(dto);
                return result.IsSuccess == true ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the path course Create method.");
                return this.UnexpectedError("path course Create.");
            }
        }

        /// <summary>
        /// Updates an existing path course identified by its unique ID with the new data provided.
        /// Requires the user to have the PathCourse.Edit permission.
        /// </summary>
        /// <param name="id">The unique identifier of the path course to update.</param>
        /// <param name="dto">An object containing the updated information for the path course.</param>
        /// <returns>
        /// Returns an IActionResult:
        /// - 200 OK if the course was successfully updated.
        /// - 400 Bad Request if the update fails or the data is invalid.
        /// - 401 Unauthorized if the user is not permitted.
        /// - 404 Not Found or 500 Internal Server Error on failure.
        /// </returns>
        [HttpPatch("update/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(PathCoursePermissions.Edit)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PathCourseUpdateDto dto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _courseService.UpdateAsync(id, dto);
                return result.IsSuccess == true ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the path course update method.");
                return this.UnexpectedError("path course update.");
            }
        }

        /// <summary>
        /// Retrieves all path courses registered in the system.
        /// Requires the user to have the PathCourse.ShowAll permission.
        /// </summary>
        /// <returns>
        /// Returns an IActionResult containing a list of all path courses:
        /// - 200 OK on success.
        /// - 204 No Content if no courses are available.
        /// - 400 Bad Request, 401 Unauthorized, 404 Not Found, or 500 Internal Server Error on failure.
        /// </returns>
        [HttpGet("all")]
        [ProducesResponseType(typeof(GeneralResult<PaginatedResult<object>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(PathCoursePermissions.GetAll)]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _courseService.GetAllAsync(CourseType.PathCourse, pagination, cancellationToken);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the path course GetAll method.");
                return this.UnexpectedError("getting path courses.");
            }
        }

        /// <summary>
        /// Retrieves a single path course based on the provided unique identifier.
        /// Requires the user to have the PathCourse.ShowOne permission.
        /// </summary>
        /// <param name="id">The unique identifier of the path course to retrieve.</param>
        /// <returns>
        /// Returns an IActionResult with course details if found:
        /// - 200 OK if successful.
        /// - 400 Bad Request, 401 Unauthorized, 404 Not Found, or 500 Internal Server Error on failure.
        /// </returns>
        [HttpGet("get-one/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(PathCoursePermissions.GetOne)]
        public async Task<IActionResult> GetOne([FromRoute] int id)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _courseService.GetOneAsync(id, CourseType.PathCourse);
                return result.IsSuccess == true ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the path course GetOne method.");
                return this.UnexpectedError("getting path course.");
            }
        }

        /// <summary>
        /// Retrieves all path courses associated with a specific learning path ID.
        /// Requires the user to have the PathCourse.ShowOne permission.
        /// </summary>
        /// <param name="pathId">The unique identifier of the learning path to filter courses by.</param>
        /// <returns>
        /// Returns an IActionResult with a list of matching courses:
        /// - 200 OK if successful.
        /// - 204 No Content if no courses match the criteria.
        /// - 400 Bad Request, 401 Unauthorized, 404 Not Found, or 500 Internal Server Error on failure.
        /// </returns>
        [HttpGet("by-path/{pathId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(PathCoursePermissions.GetByPath)]
        public async Task<IActionResult> GetByPath([FromRoute] int pathId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _pathCourseService.GetByPathAsync(pathId);
                return result.IsSuccess == true ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the skills course GetByPath method.");
                return this.UnexpectedError("getting skills course by path.");
            }
        }

        /// <summary>
        /// Performs a soft delete on a path course identified by its unique ID.
        /// This operation flags the course as deleted without permanently removing it.
        /// Requires the user to have the PathCourse.Delete permission.
        /// </summary>
        /// <param name="id">The unique identifier of the path course to delete.</param>
        /// <returns>
        /// Returns an IActionResult:
        /// - 200 OK if the course was successfully soft-deleted.
        /// - 400 Bad Request, 401 Unauthorized, 404 Not Found, or 500 Internal Server Error on failure.
        /// </returns>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(PathCoursePermissions.Delete)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _deleteService.SoftDeleteCascadeAsync<Course>(id);
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
