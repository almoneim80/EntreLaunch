namespace EntreLaunch.Web.Controllers.TrainingAPI
{
    [Authorize(Roles = "Admin , Student, Trainer")]
    [Route("api/[controller]")]
    public class CourseRatingController : BaseController<CourseRating, CourseRatingCreateDto, CourseRatingEntreLaunchdateDto, CourseRatingDetailsDto, CourseRatingExportDto>
    {
        private readonly IExtendedBaseService _extendedBaseService;
        private readonly ITrainingSectionService _trainingSection;
        private readonly IRatingsService _ratingsService;
        private readonly ILogger<CourseRatingController> _logger;
        private readonly IRoleService _roleService;
        public CourseRatingController(
            BaseService<CourseRating, CourseRatingCreateDto, CourseRatingEntreLaunchdateDto, CourseRatingDetailsDto> service,
            ILocalizationManager? localization,
            ILogger<CourseRatingController> logger,
            IExtendedBaseService extendedBaseService,
            ITrainingSectionService trainingSection,
            IRoleService roleService,
            IRatingsService ratingsService,
            IExportService exportService)
        : base(service, localization, logger, exportService)
        {
            _extendedBaseService = extendedBaseService;
            _trainingSection = trainingSection;
            _logger = logger;
            _roleService = roleService;
            _ratingsService = ratingsService;
        }

        /// <summary>
        /// Create new CourseRating.
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

                var canRate = await _trainingSection.CanStudentRateCourseAsync(createDto.UserId, createDto.CourseId);
                if (canRate.IsSuccess == false)
                {
                    return BadRequest(canRate);
                }

                return await base.Create(createDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new CourseRating.");
                return StatusCode(StatusCodes.Status500InternalServerError, new GeneralResult { IsSuccess = false, Message = "An error occurred while creating a new CourseRating." });
            }
        }

        /// <summary>
        /// EntreLaunchdate existing CourseRating.
        /// </summary>
        [HttpPatch("edit/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseRatingPermissions.Edit)]
        public override async Task<ActionResult<CourseRatingDetailsDto>> Patch(int id, [FromBody] CourseRatingEntreLaunchdateDto EntreLaunchdateDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                EntreLaunchdateDto.UserId = userId;
                var result = await _ratingsService.IsRatingAvailableAsync(id);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
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
                _logger.LogError(ex, "An error occurred while EntreLaunchdating a CourseRating.");
                return StatusCode(StatusCodes.Status500InternalServerError, new GeneralResult { IsSuccess = false, Message = "An error occurred while EntreLaunchdating a CourseRating." });
            }
        }

        /// <summary>
        /// Approve a specific rating.
        /// </summary>
        [HttpPut("approve/{ratingId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseRatingPermissions.Approve)]
        public async Task<IActionResult> ApproveRatingAsync(int ratingId, [FromBody] string note)
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
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in the course rating ApproveRatingAsync method." });
            }
        }

        /// <summary>
        /// Reject a specific rating.
        /// </summary>
        [HttpPut("reject/{ratingId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseRatingPermissions.Reject)]
        public async Task<IActionResult> RejectRatingAsync(int ratingId, [FromBody] string note)
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
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in the course rating RejectRatingAsync method." });
            }
        }

        /// <summary>
        /// Get All CourseRatings.
        /// </summary>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseRatingPermissions.ShowAll)]
        public override async Task<ActionResult<CourseRatingDetailsDto[]>> GetAll()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var ratings = await _ratingsService.GetApprovedRatingsAsync();
                if (ratings.IsSuccess == false)
                {
                    return BadRequest(ratings);
                }

                return Ok(ratings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course rating GetAll method.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in the course rating GetAll method." });
            }
        }

        /// <summary>
        /// Get one CourseRating by its id.
        /// </summary>
        [HttpGet("get-one/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseRatingPermissions.ShowOne)]
        public override async Task<ActionResult<CourseRatingDetailsDto>> GetOne(int id)
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
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in the course rating GetOne method." });
            }
        }

        /// <summary>
        /// Fetch course evaluation statistics (average rating and number of evaluators).
        /// </summary>
        [HttpGet("stats/{courseId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseRatingPermissions.GetRatingStats)]
        public async Task<IActionResult> GetCourseRatingStats(int courseId)
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
                var result = await _trainingSection.GetCourseRatingStatisticsAsync(courseId);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course rating GetCourseRatingStats method.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in the course rating GetCourseRatingStats method." });
            }
        }

        /// <summary>
        /// Fetch all evaluations for a specific course.
        /// </summary>
        [HttpGet("all/{courseId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseRatingPermissions.GetAllByCourse)]
        public async Task<ActionResult<CourseRatingDetailsDto[]>> GetAllRatingsForCourse(int courseId)
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
                var ratings = await _trainingSection.GetAllRatingsForCourseAsync(courseId);
                if (ratings.IsSuccess == false)
                {
                    return BadRequest(ratings);
                }

                return Ok(ratings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course rating GetAllRatingsForCourse method.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in the course rating GetAllRatingsForCourse method." });
            }
        }

        /// <summary>
        /// Get the average rating and the total number of ratings for a given course.
        /// </summary>
        [HttpGet("summary/{courseId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseRatingPermissions.GetSummary)]
        public async Task<IActionResult> GetCourseRatingSummaryAsync(int courseId)
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
                var summary = await _trainingSection.GetCourseRatingSummaryAsync(courseId);
                if(summary.IsSuccess == false)
                {
                    return BadRequest(summary);
                }

                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course rating GetCourseRatingSummaryAsync method.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in the course rating GetCourseRatingSummaryAsync method." });
            }
        }

        /// <summary>
        /// Fetch ratings associated with a specific coach.
        /// </summary>
        [HttpGet("instructor/{instructorId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseRatingPermissions.GetByInstructor)]
        public async Task<IActionResult> GetRatingsByInstructorAsync(string instructorId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
            }

            try
            {
                var ratings = await _trainingSection.GetRatingsByInstructorAsync(instructorId);
                if (ratings.IsSuccess == false)
                {
                    return BadRequest(ratings);
                }

                return Ok(ratings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course rating GetRatingsByInstructorAsync method.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in the course rating GetRatingsByInstructorAsync method." });
            }
        }

        /// <summary>
        /// Get all ratings by status.
        /// </summary>
        [HttpGet("all-status/{status}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseRatingPermissions.GetByStatus)]
        public async Task<IActionResult> GetRatingsByStatusAsync(string status)
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
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in the course rating GetRatingsByStatusAsync method." });
            }
        }

        /// <summary>
        /// Delete existing CourseRating.
        /// </summary>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseRatingPermissions.Delete)]
        public override async Task<ActionResult> Delete(int id)
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
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while deleting course rating.", Data = null });
            }
        }

        #region Export
        [NonAction]
        public override async Task<IActionResult> ExportToCsv()
        {
            return await Task.FromResult<ActionResult>(NotFound());
        }

        [NonAction]
        public override async Task<IActionResult> ExportToExcel()
        {
            return await Task.FromResult<ActionResult>(NotFound());
        }

        [NonAction]
        public override async Task<IActionResult> ExportToJson()
        {
            return await Task.FromResult<ActionResult>(NotFound());
        }
        #endregion
    }
}
