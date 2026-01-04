namespace EntreLaunch.Web.Controllers.TrainingAPI
{
    [Authorize(Roles = AppRoles.TrainingRoles)]
    [Route("api/[controller]")]
    public class CourseRatingController(
        BaseService<CourseRating, CourseRatingCreateDto, CourseRatingUpdateDto, CourseRatingDetailsDto> service,
        ILocalizationManager? localization,
        ILogger<CourseRatingController> logger,
        IExtendedBaseService extendedBaseService,
        ICourseService courseService,
        IRoleService roleService,
        IRatingsService ratingsService,
        IExportService exportService) : BaseController<CourseRating, CourseRatingCreateDto, CourseRatingUpdateDto, CourseRatingDetailsDto, CourseRatingExportDto>(service, localization, logger, exportService)
    {
        private readonly IExtendedBaseService _extendedBaseService = extendedBaseService;
        private readonly ICourseService _courseService = courseService;
        private readonly IRatingsService _ratingsService = ratingsService;
        private readonly ILogger<CourseRatingController> _logger = logger;
        private readonly IRoleService _roleService = roleService;

        /// <summary>
        /// Creates a new course rating based on the provided data and associates it with the currently authenticated user.
        /// Performs validations including course existence, user role, and eligibility to rate the course.
        /// Requires the user to have creation permissions.
        /// </summary>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseRatingPermissions.Create)]
        public override async Task<ActionResult<CourseRatingDetailsDto>> Create([FromBody] CourseRatingCreateDto createDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                if (createDto == null)
                {
                    return BadRequest(new GeneralResult { IsSuccess = false, Message = "Data Can Not Be Null" });
                }

                createDto.UserId = userId;
                var isReferencedValid = await _extendedBaseService.IsEntityExistsAndNotDeletedAsync<Course>(createDto.CourseId);
                if (isReferencedValid.IsSuccess == false)
                {
                    return BadRequest(isReferencedValid);
                }

                var result = await _roleService.IsUserInRoleAsync(createDto.UserId, "Student");
                if (result.Data == false)
                {
                    return BadRequest(result);
                }

                var canRate = await _ratingsService.CanStudentRateCourseAsync(createDto.UserId, createDto.CourseId);
                if (canRate.IsSuccess == false)
                {
                    return BadRequest(canRate);
                }

                return await base.Create(createDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new CourseRating.");
                return this.UnexpectedError("creating a new course rating.");
            }
        }

        [NonAction]
        public override async Task<ActionResult<CourseRatingDetailsDto>> Patch([FromRoute] int id, [FromBody] CourseRatingUpdateDto updateDto)
        {
            return await base.Patch(id, updateDto);
        }

        /// <summary>
        /// Approves a specific course rating identified by its ID and logs an approval note for auditing purposes.
        /// Requires the user to have the appropriate approval permission.
        /// </summary>
        [HttpPut("approve/{ratingId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseRatingPermissions.Approve)]
        public async Task<IActionResult> ApproveRatingAsync([FromRoute] int ratingId, [FromBody] string note)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                if (note == null)
                {
                    return BadRequest(new GeneralResult { IsSuccess = false, Message = "Note can not be null." });
                }

                var result = await _ratingsService.ApproveRatingAsync(ratingId, note);
                if (result.IsSuccess == false)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course rating ApproveRatingAsync method.");
                return this.UnexpectedError("course rating approve rating.");
            }
        }

        /// <summary>
        /// Rejects a specific course rating identified by its ID, and logs a rejection note for audit purposes.
        /// Requires the user to have the appropriate rejection permission.
        /// </summary>
        [HttpPut("reject/{ratingId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseRatingPermissions.Reject)]
        public async Task<IActionResult> RejectRatingAsync([FromRoute] int ratingId, [FromBody] string note)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var result = await _ratingsService.RejectRatingAsync(ratingId, note);
                if (result.IsSuccess == false)
                    return NotFound(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course rating RejectRatingAsync method.");
                return this.UnexpectedError("ourse rating reject rating.");
            }
        }

        /// <summary>
        /// Retrieves all approved course ratings available in the system.
        /// Requires the user to have the appropriate permission to view all ratings.
        /// </summary>
        [HttpGet("approved")]
        [ProducesResponseType(typeof(PaginatedResult<CourseRatingDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseRatingPermissions.GetAll)]
        public async Task<IActionResult> GetApprovedRatings([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var result = await _ratingsService.GetApprovedRatingsAsync(pagination, cancellationToken);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course rating GetAll method.");
                return this.UnexpectedError("course rating get all.");
            }
        }

        /// <summary>
        /// Retrieves the details of a specific course rating by its unique identifier.
        /// Validates the rating's availability before fetching the full details.
        /// Requires the user to have permission to view individual ratings.
        /// </summary>
        [HttpGet("get-one/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseRatingPermissions.GetOne)]
        public override async Task<ActionResult<CourseRatingDetailsDto>> GetOne([FromRoute] int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var result = await _ratingsService.IsRatingAvailableAsync(id);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return await base.GetOne(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course rating GetOne method.");
                return this.UnexpectedError("course rating get one.");
            }
        }

        /// <summary>
        /// Retrieves aggregated rating statistics for a specific course identified by its ID.
        /// Validates that the course exists and is not deleted before computing statistics.
        /// Requires the user to have permission to access course rating statistics.
        /// </summary>
        [HttpGet("stats/{courseId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseRatingPermissions.GetRatingStats)]
        public async Task<IActionResult> GetCourseRatingStats([FromRoute] int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
            }

            var isReferencedValid = await _extendedBaseService.IsEntityExistsAndNotDeletedAsync<Course>(courseId);
            if (isReferencedValid.IsSuccess == false)
            {
                return BadRequest(isReferencedValid);
            }

            try
            {
                var result = await _ratingsService.GetCourseRatingStatisticsAsync(courseId);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course rating GetCourseRatingStats method.");
                return this.UnexpectedError("course rating get course rating stats.");
            }
        }

        /// <summary>
        /// Retrieves all course ratings associated with a specific course ID.
        /// Ensures that the referenced course exists and is not marked as deleted before retrieving data.
        /// Requires the user to have permission to view all ratings for a course.
        /// </summary>
        [HttpGet("all/{courseId:int}")]
        [ProducesResponseType(typeof(PaginatedResult<CourseRatingDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseRatingPermissions.GetAllByCourse)]
        public async Task<IActionResult> GetAllRatingsForCourse([FromRoute] int courseId, [FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
            }

            var isReferencedValid = await _extendedBaseService.IsEntityExistsAndNotDeletedAsync<Course>(courseId);
            if (!isReferencedValid.IsSuccess)
            {
                return BadRequest(isReferencedValid);
            }

            try
            {
                var result = await _ratingsService.GetAllRatingsForCourseAsync(courseId, pagination, cancellationToken);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetAllRatingsForCourse.");
                return this.UnexpectedError("get all ratings for course");
            }
        }

        /// <summary>
        /// Retrieves a summarized view of the course rating statistics for a specific course.
        /// Validates that the course exists and is not deleted before fetching the summary.
        /// Requires the user to have permission to access course rating summaries.
        /// </summary>
        [HttpGet("summary/{courseId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseRatingPermissions.GetSummary)]
        public async Task<IActionResult> GetCourseRatingSummaryAsync([FromRoute] int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
            }

            var isReferencedValid = await _extendedBaseService.IsEntityExistsAndNotDeletedAsync<Course>(courseId);
            if (isReferencedValid.IsSuccess == false)
            {
                return BadRequest(isReferencedValid);
            }

            try
            {
                var summary = await _ratingsService.GetCourseRatingSummaryAsync(courseId);
                if (summary.IsSuccess == false)
                {
                    return BadRequest(summary);
                }

                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course rating GetCourseRatingSummaryAsync method.");
                return this.UnexpectedError("course rating get course rating summary.");
            }
        }

        /// <summary>
        /// Retrieves all course ratings associated with a specific instructor, identified by their unique ID.
        /// Requires the user to have permission to view ratings by instructor.
        /// </summary>
        [HttpGet("instructor/{instructorId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseRatingPermissions.GetByInstructor)]
        public async Task<IActionResult> GetRatingsByInstructorAsync([FromRoute] string instructorId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
            }

            try
            {
                var ratings = await _ratingsService.GetRatingsByInstructorAsync(instructorId);
                if (ratings.IsSuccess == false)
                {
                    return BadRequest(ratings);
                }

                return Ok(ratings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course rating GetRatingsByInstructorAsync method.");
                return this.UnexpectedError("course rating get ratings by instructor.");
            }
        }

        /// <summary>
        /// Retrieves all course ratings filtered by a specific status.
        /// Validates the status value before querying the data source.
        /// Requires the user to have permission to view ratings by status.
        /// </summary>
        [HttpGet("all-status/{status}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseRatingPermissions.GetByStatus)]
        public async Task<IActionResult> GetRatingsByStatusAsync([FromRoute] string status)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                if (!Enum.TryParse<RatingStatus>(status, true, out var parsedStatus))
                {
                    return BadRequest(new GeneralResult { IsSuccess = false, Message = "Invalid status value." });
                }

                var ratings = await _ratingsService.GetRatingsByStatusAsync(parsedStatus);
                if (ratings.IsSuccess == false)
                {
                    return BadRequest(ratings);
                }

                return Ok(ratings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course rating GetRatingsByStatusAsync method.");
                return this.UnexpectedError("course rating get ratings by status.");
            }
        }

        /// <summary>
        /// Retrieves all available rating statuses defined in the system.
        /// </summary>
        [HttpGet("rating-status")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseRatingPermissions.GetRatingStatuses)]
        public ActionResult<IEnumerable<EnumData>> GetRatingStatuses()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var enumValues = _extendedBaseService.GetEnumValues<RatingStatus>();
                return Ok(enumValues);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course GetRatingStatuses method.");
                return this.UnexpectedError("getting rating statuses.");
            }
        }

        /// <summary>
        /// Deletes a specific course rating identified by its ID.
        /// Validates the rating's availability before performing the deletion.
        /// Requires the user to have delete permissions.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the course rating to be deleted. Must correspond to an existing and available rating.
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult"/> indicating the result of the delete operation:
        /// <list type="bullet">
        /// <item><description><c>204 No Content</c>: The course rating was successfully deleted.</description></item>
        /// <item><description><c>400 Bad Request</c>: The rating is not valid for deletion or failed availability checks.</description></item>
        /// <item><description><c>401 Unauthorized</c>: The user is not authenticated.</description></item>
        /// <item><description><c>404 Not Found</c>: The specified rating was not found.</description></item>
        /// <item><description><c>500 Internal Server Error</c>: An unexpected error occurred during the deletion process.</description></item>
        /// </list>
        /// </returns>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseRatingPermissions.Delete)]
        public override async Task<ActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var result = await _ratingsService.IsRatingAvailableAsync(id);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return await base.Delete(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course rating Delete method.");
                return this.UnexpectedError("deleting course rating.");
            }
        }
    }
}
