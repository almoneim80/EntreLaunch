namespace EntreLaunch.Web.Controllers.TrainingAPI
{
    [Authorize(Roles = "Admin , Entrepreneur")]
    [Route("api/[controller]")]
    public class CourseEnrollmentController : BaseController<CourseEnrollment, CourseEnrollmentCreateDto, CourseEnrollmentEntreLaunchdateDto, CourseEnrollmentDetailsDto, CourseEnrollmentExportDto>
    {
        private readonly ITrainingSectionService _trainingSection;
        private readonly ILogger<CourseEnrollmentController> _logger;
        private readonly IExtendedBaseService _extendedBaseService;
        private readonly IRoleService _roleService;

        public CourseEnrollmentController(
            ITrainingSectionService trainingSection,
            ILogger<CourseEnrollmentController> logger,
            BaseService<CourseEnrollment,
            CourseEnrollmentCreateDto,
            CourseEnrollmentEntreLaunchdateDto,
            CourseEnrollmentDetailsDto> service,
            ILocalizationManager? localization,
            IExtendedBaseService extendedBaseService,
            IRoleService roleService,
            IExportService exportService)
        : base(service, localization, logger, exportService)
        {
            _trainingSection = trainingSection;
            _logger = logger;
            _extendedBaseService = extendedBaseService;
            _roleService = roleService;
        }

        /// <summary>
        /// Create new CourseEnrollment.
        /// </summary>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseEnrollmentPermissions.Create)]
        public override async Task<ActionResult<CourseEnrollmentDetailsDto>> Create([FromBody] CourseEnrollmentCreateDto createDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var isReferencedValid = await _extendedBaseService.IsEntityExistsAndNotDeletedAsync<Course>(createDto.CourseId);
                if (isReferencedValid.IsSuccess == false)
                {
                    return BadRequest(new GeneralResult { IsSuccess = false, Message = "The referenced Entity does not exist or has been deleted." });
                }

                var isEligible = await _trainingSection.VerifyCourseEnrollmentEligibilityAsync(createDto.CourseId, createDto.UserId);
                if (isEligible.IsSuccess == false)
                {
                    return BadRequest(isEligible);
                }

                createDto.UserId = userId;
                var addResult = await _trainingSection.EnrollUserToCourseAsync(createDto);
                if (addResult.IsSuccess == false)
                {
                    _logger.LogError($"Failed to create a new Course Enrollment for user with Id {createDto.UserId}");
                    return BadRequest(addResult);
                }

                return Ok(addResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create a new Course Enrollment");
                return StatusCode(StatusCodes.Status500InternalServerError, new GeneralResult { IsSuccess = false, Message = "Failed to create a new Course Enrollment" });
            }
        }

        /// <summary>
        /// EntreLaunchdate existing CourseEnrollment.
        /// </summary>
        [HttpPatch("edit/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseEnrollmentPermissions.Edit)]
        public override async Task<ActionResult<CourseEnrollmentDetailsDto>> Patch(int id, [FromBody] CourseEnrollmentEntreLaunchdateDto EntreLaunchdateDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var isReferencedValid = await _extendedBaseService.IsEntityExistsAndNotDeletedAsync<Course>(EntreLaunchdateDto.CourseId);
                if (isReferencedValid.IsSuccess == false)
                {
                    return BadRequest(isReferencedValid);
                }

                return await base.Patch(id, EntreLaunchdateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to EntreLaunchdate a Course Enrollment");
                return StatusCode(StatusCodes.Status500InternalServerError, new GeneralResult { IsSuccess = false, Message = "Failed to EntreLaunchdate a Course Enrollment" });
            }
        }

        /// <summary>
        /// Get All CourseEnrollments.
        /// </summary>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseEnrollmentPermissions.ShowAll)]
        public override async Task<ActionResult<CourseEnrollmentDetailsDto[]>> GetAll()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                return await base.GetAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all Course Enrollments");
                return StatusCode(StatusCodes.Status500InternalServerError, new GeneralResult { IsSuccess = false, Message = "Failed to get all Course Enrollments" });
            }
        }

        /// <summary>
        /// Get one CourseEnrollment by its id.
        /// </summary>
        [HttpGet("get-one/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseEnrollmentPermissions.ShowOne)]
        public override async Task<ActionResult<CourseEnrollmentDetailsDto>> GetOne(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                return await base.GetOne(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get a Course Enrollment");
                return StatusCode(StatusCodes.Status500InternalServerError, new GeneralResult { IsSuccess = false, Message = "Failed to get a Course Enrollment" });
            }
        }

        /// <summary>
        /// Export all CourseEnrollments to CSV file.
        /// </summary>
        [HttpGet("export/csv")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseEnrollmentPermissions.Export)]
        public override async Task<IActionResult> ExportToCsv()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                return await base.ExportToCsv();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course enrollment ExportToCsv method.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while exporting course enrollments.", Data = null });
            }
        }

        /// <summary>
        /// Export all CourseEnrollments to Excel file.
        /// </summary>
        [HttpGet("export/excel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseEnrollmentPermissions.Export)]
        public override async Task<IActionResult> ExportToExcel()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                return await base.ExportToExcel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course enrollment ExportToExcel method.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while exporting course enrollments.", Data = null });
            }
        }

        /// <summary>
        /// Export all CourseEnrollments to JSON file.
        /// </summary>
        [HttpGet("export/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseEnrollmentPermissions.Export)]
        public override async Task<IActionResult> ExportToJson()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                return await base.ExportToJson();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course enrollment ExportToJson method.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while exporting course enrollments.", Data = null });
            }
        }

        /// <summary>
        /// Get all course enrollments by course id.
        /// </summary>
        [HttpGet("enrollment/{courseId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseEnrollmentPermissions.ShowByCourse)]
        public async Task<ActionResult> GetCourseEnrollments(int courseId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var result = await _trainingSection.GetCourseEnrollmentsWithUsersAsync(courseId);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course enrollment GetCourseEnrollments method.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while getting course enrollments.", Data = null });
            }
        }

        /// <summary>
        /// Get all subscriptions in courses for a particular user.
        /// </summary>
        [HttpGet("subscriptions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseEnrollmentPermissions.GetUserSubscriptions)]
        public async Task<IActionResult> GetUserSubscriptions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
            }

            try
            {
                var subscriptions = await _trainingSection.GetUserSubscriptionsAsync(userId);
                if (subscriptions.IsSuccess == false)
                {
                    return BadRequest(subscriptions);
                }

                return Ok(subscriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course enrollment GetUserSubscriptions method.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while getting user subscriptions.", Data = null });
            }
        }

        /// <summary>
        /// Delete existing CourseEnrollment.
        /// </summary>
        [HttpDelete("delete/enrollment/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseEnrollmentPermissions.Delete)]
        public override async Task<ActionResult> Delete(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                return await base.Delete(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course enrollment Delete method.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while deleting course enrollment.", Data = null });
            }
        }

        /// <summary>
        /// Unsubscribe the user from a particular course.
        /// </summary>
        [HttpDelete("unenroll/{courseId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseEnrollmentPermissions.Unenroll)]
        public async Task<IActionResult> UnenrollUserAsync(int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
            }

            try
            {
                var result = await _trainingSection.UnenrollUserFromCourseAsync(courseId, userId);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course enrollment UnenrollUserAsync method.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while unenrolling user.", Data = null });
            }
        }
    }
}
