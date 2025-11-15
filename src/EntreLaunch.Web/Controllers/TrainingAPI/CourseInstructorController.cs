namespace EntreLaunch.Web.Controllers.TrainingAPI
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    public class CourseInstructorController : BaseController<CourseInstructor, CourseInstructorCreateDto, CourseInstructorEntreLaunchdateDto, CourseInstructorDetailsDto, CourseInstructorExportDto>
    {
        private readonly IExtendedBaseService _extendedBaseService;
        private readonly ITrainingSectionService _trainingSection;
        private readonly ILogger<CourseInstructorController> _logger;
        protected readonly UserManager<User> _userManager;
        private readonly IRoleService _roleService;
        public CourseInstructorController(
            BaseService<CourseInstructor, CourseInstructorCreateDto, CourseInstructorEntreLaunchdateDto, CourseInstructorDetailsDto> service,
            ILocalizationManager? localization,
            ILogger<CourseInstructorController> logger,
            IExtendedBaseService extendedBaseService,
            ITrainingSectionService trainingSection,
            UserManager<User> userManager,
            IRoleService roleService,
            IExportService exportService)
            : base(service, localization, logger, exportService)
        {
            _extendedBaseService = extendedBaseService;
            _trainingSection = trainingSection;
            _logger = logger;
            _userManager = userManager;
            _roleService = roleService;
        }

        /// <summary>
        ///     Create new CourseInstructor.
        /// </summary>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseInstructorPermissions.Create)]
        public override async Task<ActionResult<CourseInstructorDetailsDto>> Create([FromBody] CourseInstructorCreateDto createDto)
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
                    return BadRequest(isReferencedValid);
                }

                var user = await _userManager.Users.FirstOrDefaultAsync(usr => usr.Id == createDto.UserId && !usr.IsDeleted);
                if (user == null)
                {
                    return BadRequest(new GeneralResult { IsSuccess = false, Message = "User not found." });
                }

                var result = await _roleService.IsUserInRoleAsync(createDto.UserId, "Trainer");
                if (result.Data == false)
                {
                    return BadRequest(result);
                }

                return await base.Create(createDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new GeneralResult { IsSuccess = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// EntreLaunchdate existing CourseInstructor.
        /// </summary>
        [HttpPatch("edit/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseInstructorPermissions.Edit)]
        public override async Task<ActionResult<CourseInstructorDetailsDto>> Patch(int id, [FromBody] CourseInstructorEntreLaunchdateDto EntreLaunchdateDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                return await base.Patch(id, EntreLaunchdateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course instructor Patch method.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in the course instructor Patch method." });
            }
        }

        /// <summary>
        /// Get all CourseInstructors.
        /// </summary>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseInstructorPermissions.ShowAll)]
        public override async Task<ActionResult<CourseInstructorDetailsDto[]>> GetAll()
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
                _logger.LogError(ex, "An error occurred in the course instructor GetAll method.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in the course instructor GetAll method." });
            }
        }

        /// <summary>
        /// Get one CourseInstructor by its id.
        /// </summary>
        [HttpGet("get-one/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseInstructorPermissions.ShowOne)]
        public override async Task<ActionResult<CourseInstructorDetailsDto>> GetOne(int id)
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
                _logger.LogError(ex, "An error occurred in the course instructor GetOne method.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in the course instructor GetOne method." });
            }
        }

        /// <summary>
        /// Export all CourseInstructors to CSV file.
        /// </summary>
        [HttpGet("export/csv")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseInstructorPermissions.Export)]
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
                _logger.LogError(ex, "An error occurred in the course instructor ExportToCsv method.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while exporting course instructors.", Data = null });
            }
        }

        /// <summary>
        /// Export all CourseInstructors to Excel file.
        /// </summary>
        [HttpGet("export/excel")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseInstructorPermissions.Export)]
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
                _logger.LogError(ex, "An error occurred in the course instructor ExportToExcel method.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while exporting course instructors.", Data = null });
            }
        }

        /// <summary>
        /// Export all CourseInstructors to JSON file.
        /// </summary>
        [HttpGet("export/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseInstructorPermissions.Export)]
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
                _logger.LogError(ex, "An error occurred in the course instructor ExportToJson method.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while exporting course instructors.", Data = null });
            }
        }

        /// <summary>
        /// View the list of trainers based on the course number.
        /// </summary>
        [HttpGet("by-coures/{courseId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseInstructorPermissions.GetInstructorsByCourse)]
        public async Task<IActionResult> GetInstructorsByCourseIdAsync(int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
            }

            try
            {
                var instructors = await _trainingSection.GetInstructorsByCourseIdAsync(courseId);
                if (instructors.IsSuccess == false)
                {
                    return BadRequest(instructors);
                }

                return Ok(instructors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course instructor GetInstructorsByCourseIdAsync method.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in the course instructor GetInstructorsByCourseIdAsync method." });
            }
        }

        /// <summary>
        /// Get the performance statistics of a trainer.
        /// </summary>
        [HttpGet("trainer/{trainerId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseInstructorPermissions.GetTrainerPerformance)]
        public async Task<IActionResult> GetTrainerPerformanceAsync(string trainerId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var performance = await _trainingSection.GetTrainerPerformanceAsync(trainerId);
                if (performance.IsSuccess == false)
                {
                    return BadRequest(performance);
                }

                return Ok(performance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course instructor GetTrainerPerformanceAsync method.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in the course instructor GetTrainerPerformanceAsync method." });
            }
        }

        /// <summary>
        /// Delete existing CourseInstructor.
        /// </summary>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CourseInstructorPermissions.Delete)]
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
                _logger.LogError(ex, "An error occurred in the course instructor Delete method.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while deleting course instructor.", Data = null });
            }
        }
    }
}

