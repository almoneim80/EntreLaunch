namespace EntreLaunch.Web.Controllers.TrainingAPI
{
    [Authorize(Roles = AppRoles.TrainingRoles)]
    [Route("api/courses/online")]
    public class OnlineCoursesController(
        ILogger<OnlineCoursesController> logger,
        ILocalizationManager localization,
        ICourseService courseService,
        IPurchaseService purchaseService,
        IOnlineCourseService onlineCourseService,
        CascadeDeleteService deleteService,
        IExtendedBaseService extendedBaseService) : AuthenticatedController(localization)
    {
        private readonly ILogger<OnlineCoursesController> _logger = logger;
        private readonly IPurchaseService _purchaseService = purchaseService;
        private readonly ILocalizationManager _localization = localization;
        private readonly ICourseService _courseService = courseService;
        private readonly IOnlineCourseService _onlineCourseService = onlineCourseService;
        private readonly IExtendedBaseService _extendedBaseService = extendedBaseService;
        private readonly CascadeDeleteService _deleteService = deleteService;

        /// <summary>
        /// Creates a new online course using the provided data transfer object.
        /// Requires the OnlineCourse.Create permission.
        /// </summary>
        /// <param name="dto">The data transfer object containing course details to be created.</param>
        /// <returns>
        /// IActionResult indicating the result:
        /// - 200 OK if creation is successful.
        /// - 400 Bad Request if validation fails or input is invalid.
        /// - 401 Unauthorized if the user lacks permission.
        /// - 404 Not Found or 500 Internal Server Error in case of failure.
        /// </returns>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(OnlineCoursePermissions.Create)]
        public async Task<IActionResult> Create([FromBody] OnlineCourseCreateDto dto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                var result = await _courseService.CreateAsync<OnlineCourseCreateDto>(dto);
                return result.IsSuccess == true ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the online course Create method.");
                return this.UnexpectedError("online course Create.");
            }
        }

        /// <summary>
        /// Updates an existing online course identified by its ID with the provided data.
        /// Requires the OnlineCourse.Edit permission.
        /// </summary>
        /// <param name="id">The unique identifier of the online course to update.</param>
        /// <param name="dto">The updated course data.</param>
        /// <returns>
        /// IActionResult indicating the result:
        /// - 200 OK if the update is successful.
        /// - 400 Bad Request if the update fails or the input is invalid.
        /// - 401 Unauthorized, 404 Not Found, or 500 Internal Server Error on failure.
        /// </returns>
        [HttpPatch("update/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(OnlineCoursePermissions.Edit)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] OnlineCourseUpdateDto dto)
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
                _logger.LogError(ex, "An error occurred in the online course update method.");
                return this.UnexpectedError("online course update.");
            }
        }

        /// <summary>
        /// Retrieves all online courses available in the system.
        /// Requires the OnlineCourse.ShowAll permission.
        /// </summary>
        /// <returns>
        /// IActionResult with the list of online courses or an appropriate error response:
        /// - 200 OK if successful.
        /// - 204 No Content if no data is found.
        /// - 400 Bad Request, 401 Unauthorized, 404 Not Found, or 500 Internal Server Error on failure.
        /// </returns>
        [HttpGet("all")]
        [ProducesResponseType(typeof(PaginatedResult<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(OnlineCoursePermissions.GetAll)]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _courseService.GetAllAsync(CourseType.OnlineCourse, pagination, cancellationToken);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the onlinecourse GetAll method.");
                return this.UnexpectedError("getting online courses.");
            }
        }

        /// <summary>
        /// Retrieves a specific online course by its ID.
        /// Requires the OnlineCourse.ShowOne permission.
        /// </summary>
        /// <param name="id">The unique identifier of the course.</param>
        /// <returns>
        /// IActionResult with course details:
        /// - 200 OK if found.
        /// - 400 Bad Request, 401 Unauthorized, 404 Not Found, or 500 Internal Server Error otherwise.
        /// </returns>
        [HttpGet("get-one/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(OnlineCoursePermissions.GetOne)]
        public async Task<IActionResult> GetOne([FromRoute] int id)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _courseService.GetOneAsync(id, CourseType.OnlineCourse);
                return result.IsSuccess == true ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the online course GetOne method.");
                return this.UnexpectedError("getting online course.");
            }
        }

        /// <summary>
        /// Changes the status of a specific online course.
        /// Requires the OnlineCourse.ChangeStatus permission.
        /// </summary>
        /// <param name="courseId">The unique identifier of the course whose status will be updated.</param>
        /// <param name="newStatus">The new status to assign to the course.</param>
        /// <returns>
        /// IActionResult indicating success or failure:
        /// - 200 OK if the status update succeeds.
        /// - 404 Not Found or 500 Internal Server Error otherwise.
        /// </returns>
        [HttpPatch("change-status/{courseId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(OnlineCoursePermissions.ChangeStatus)]
        public async Task<IActionResult> ChangeStatusAsync([FromRoute] int courseId, [FromForm] CourseStatus newStatus)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _onlineCourseService.ChangeCourseStatusAsync(courseId, newStatus);
                return result.IsSuccess == true ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the online course ChangeStatus method.");
                return this.UnexpectedError("online course ChangeStatus.");
            }
        }

        /// <summary>
        /// Retrieves a list of all possible course statuses as enum values.
        /// Requires the OnlineCourse.GetStatuses permission.
        /// </summary>
        /// <returns>
        /// A list of EnumData representing available course statuses:
        /// - 201 Created if successful.
        /// - 400 Bad Request, 401 Unauthorized, or 500 Internal Server Error on failure.
        /// </returns>
        [HttpGet("course-status")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(OnlineCoursePermissions.GetStatuses)]
        public ActionResult<IEnumerable<EnumData>> GetCourseStatuses()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = _localization.GetLocalizedString("UserNotLoggedIn") });
                }

                var enumValues = _extendedBaseService.GetEnumValues<CourseStatus>();
                return Ok(enumValues);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course GetCourseStatuses method.");
                return this.UnexpectedError("getting course statuses.");
            }
        }

        /// <summary>
        /// Retrieves online courses filtered by a specific status value.
        /// Requires the OnlineCourse.GetByStatus permission.
        /// </summary>
        [HttpGet("by-status")]
        [ProducesResponseType(typeof(PaginatedResult<OnlineCourseDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(OnlineCoursePermissions.GetByStatus)]
        public async Task<IActionResult> GetCourseByStatusAsync([FromQuery] CourseStatus status, [FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _onlineCourseService.GetByStatusAsync(status, pagination, cancellationToken);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching online courses by status: Status = {Status}", status);
                return this.UnexpectedError("fetching online courses by status.");
            }
        }

        /// <summary>
        /// Retrieves a list of users enrolled in a specific online course.
        /// Requires the OnlineCourse.GetEnrolled permission.
        /// </summary>
        [HttpGet("enrolled/course/{courseId:int}")]
        [ProducesResponseType(typeof(PaginatedResult<CoursesRegisterDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(OnlineCoursePermissions.GetEnrolled)]
        public async Task<IActionResult> GetEnrolledInCourseAsync([FromRoute] int courseId, [FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _courseService.GetUsersByCoursePurchaseAsync(PurchaseItemType.OnlineCourse, courseId, pagination, cancellationToken);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "fetching enrolled users in online course.");
                return this.UnexpectedError("fetching enrolled users in online course.");
            }
        }

        /// <summary>
        /// Checks if the current user has access to a specific online course.
        /// </summary>
        [HttpGet("has-access/{courseId}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> HasAccessToCourse([FromRoute] int courseId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _purchaseService.HasUserPurchasedAsync(CurrentUserId!, PurchaseItemType.OnlineCourse, courseId);
                return result.IsSuccess == true ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "fetching enrolled users in online course.");
                return this.UnexpectedError("fetching enrolled users in online course.");
            }
        }

        /// <summary>
        /// Soft deletes an online course by marking it as deleted without removing it permanently.
        /// Requires the OnlineCourse.Delete permission.
        /// </summary>
        /// <param name="id">The unique identifier of the course to delete.</param>
        /// <returns>
        /// IActionResult indicating the result:
        /// - 200 OK if the course was successfully soft deleted.
        /// - 400 Bad Request, 401 Unauthorized, 404 Not Found, or 500 Internal Server Error otherwise.
        /// </returns>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(OnlineCoursePermissions.Delete)]
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
