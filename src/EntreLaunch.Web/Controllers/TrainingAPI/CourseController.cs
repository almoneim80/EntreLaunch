namespace EntreLaunch.Web.Controllers.TrainingAPI
{
    [Authorize(Roles = "Admin , Entrepreneur, Trainer")]
    [Route("api/[controller]")]
    public class CourseController : BaseController<Course, CourseCreateDto, CourseEntreLaunchdateDto, CourseDetailsDto, CourseExportDto>
    {
        private readonly ILogger<CourseController> _logger;
        private readonly CascadeDeleteService _deleteService;
        private readonly IExtendedBaseService _extendedBaseService;
        private readonly IImportService<Course, CourseImportDto> _importService;
        private readonly ITrainingSectionService _trainingSection;
        private readonly IExportService _exportService;
        public CourseController(
            BaseService<Course, CourseCreateDto, CourseEntreLaunchdateDto, CourseDetailsDto> service,
            ILogger<CourseController> logger,
            ILocalizationManager? localization,
            CascadeDeleteService deleteService,
            IExtendedBaseService extendedBaseService,
            IImportService<Course, CourseImportDto> importService,
            ITrainingSectionService trainingSection,
            IExportService exportService)
            : base(service, localization, logger, exportService)
        {
            _logger = logger;
            _deleteService = deleteService;
            _extendedBaseService = extendedBaseService;
            _importService = importService;
            _trainingSection = trainingSection;
            _exportService = exportService;
        }

        /// <summary>
        /// Create new Course.
        /// </summary>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(Permissions.CoursePermissions.Create)]
        public override async Task<ActionResult<CourseDetailsDto>> Create([FromBody] CourseCreateDto createDto)
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

                if (createDto.PathId != null)
                {
                    var isPathValid = await _extendedBaseService.IsEntityExistsAndNotDeletedAsync<TrainingPath>(createDto.PathId ?? 0);
                    if (isPathValid.IsSuccess == false)
                    {
                        return BadRequest(new GeneralResult { IsSuccess = false, Message = "The referenced TrainingPath does not exist or has been deleted." });
                    }
                }

                if (createDto.FieldId != null)
                {
                    var isPathValid = await _extendedBaseService.IsEntityExistsAndNotDeletedAsync<CourseField>(createDto.FieldId ?? 0);
                    if (isPathValid.IsSuccess == false)
                    {
                        return BadRequest(new GeneralResult { IsSuccess = false, Message = "The referenced Field does not exist or has been deleted." });
                    }
                }

                if (createDto.IsFree == true && createDto.Price <= 0)
                {
                    return BadRequest(new GeneralResult { IsSuccess = false, Message = "Price must be greater than 0 for free courses." });
                }

                return await base.Create(createDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course Create method.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in the course Create method." });
            }
        }

        /// <summary>
        /// Creates a course with its related lesson.
        /// ignore couresId.
        /// </summary>
        [HttpPost("with-lessons")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(Permissions.CoursePermissions.CreateFull)]
        public async Task<IActionResult> AddWithLessons([FromBody] CourseWithChildrenDto courseWithChildrenDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
            }

            if (courseWithChildrenDto == null)
            {
                return BadRequest(new GeneralResult { IsSuccess = false, Message = "Data Can Not Be Null" });
            }

            if (courseWithChildrenDto.PathId != null)
            {
                var isPathValid = await _extendedBaseService.IsEntityExistsAndNotDeletedAsync<TrainingPath>(courseWithChildrenDto.PathId ?? 0);
                if (isPathValid.IsSuccess == false)
                {
                    return BadRequest(new GeneralResult { IsSuccess = false, Message = "The referenced TrainingPath does not exist or has been deleted." });
                }
            }

            if (courseWithChildrenDto.FieldId != null)
            {
                var isPathValid = await _extendedBaseService.IsEntityExistsAndNotDeletedAsync<CourseField>(courseWithChildrenDto.FieldId ?? 0);
                if (isPathValid.IsSuccess == false)
                {
                    return BadRequest(new GeneralResult { IsSuccess = false, Message = "The referenced Field does not exist or has been deleted." });
                }
            }

            try
            {
                var courseWithLesson = new Course
                {
                    Name = courseWithChildrenDto.Name,
                    Description = courseWithChildrenDto.Description,
                    FieldId = courseWithChildrenDto.FieldId,
                    PathId = courseWithChildrenDto.PathId,
                    Price = courseWithChildrenDto.Price ?? 0,
                    Discount = courseWithChildrenDto.Discount ?? 0,
                    StudyWay = courseWithChildrenDto.StudyWay,
                    DurationInDays = courseWithChildrenDto.DurationInDays,
                    StartDate = courseWithChildrenDto.StartDate,
                    EndDate = courseWithChildrenDto.EndDate,
                    CertificateExists = courseWithChildrenDto.CertificateExists ?? true,
                    IsFree = courseWithChildrenDto.IsFree,
                    CertificateUrl = courseWithChildrenDto.CertificateUrl,
                    Logo = courseWithChildrenDto.Logo,
                    Status = courseWithChildrenDto.Status ?? 0,
                    Audience = courseWithChildrenDto.Audience,
                    Requirements = courseWithChildrenDto.Requirements,
                    Topics = courseWithChildrenDto.Topics,
                    Goals = courseWithChildrenDto.Goals,
                    Outcomes = courseWithChildrenDto.Outcomes,
                    MaxEnrollment = courseWithChildrenDto.MaxEnrollment,

                    CreatedAt = DateTimeOffset.UtcNow,
                    IsDeleted = false,
                    Source = "courseWithChildren",

                    Lessons = courseWithChildrenDto.Lessons?.Select(l => new Lesson
                    {
                        Name = l.Name,
                        VideoUrl = l.VideoUrl,
                        DurationInMinutes = l.DurationInMinutes,
                        Description = l.Description,
                        CourseId = l.CourseId,
                        Order = l.Order,
                        CreatedAt = DateTimeOffset.UtcNow,
                        IsDeleted = false,
                        Source = "courseWithChildren",
                        LessonAttachments = l.LessonAttachmentFullAddDtos?.Select(a => new LessonAttachment
                        {
                            FileName = a.FileName ?? "",
                            FileUrl = a.FileUrl ?? "",
                            CreatedAt = DateTimeOffset.UtcNow,
                            IsDeleted = false,
                            Source = "courseWithChildren"
                        }).ToList()
                    }).ToList()
                };

                await _extendedBaseService.AddEntityAsync(courseWithLesson);

                _logger.LogInformation("Successfully added course with lessons. Event Name: {EventName}, Activity Count: {ActivityCount}", courseWithLesson.Name, courseWithLesson.Lessons?.Count ?? 0);
                return Ok(new GeneralResult { IsSuccess = true, Message = "Successfully added course with lessons." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course AddWithLessons method.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in the course AddWithLessons method." });
            }
        }

        /// <summary>
        /// Imports data from a list.
        /// (id must be unique.)
        /// </summary>
        [HttpPost("import")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(Permissions.CoursePermissions.Import)]
        public async Task<ActionResult<ImportResult>> Import([FromBody] List<CourseImportDto> importRecords)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var result = await _importService.ImportFromListAsync(importRecords);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course import method.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in the course import method." });
            }
        }

        /// <summary>
        /// EntreLaunchdate existing Course.
        /// </summary>
        [HttpPatch("edit/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(Permissions.CoursePermissions.Edit)]
        public override async Task<ActionResult<CourseDetailsDto>> Patch(int id, [FromBody] CourseEntreLaunchdateDto EntreLaunchdateDto)
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
                _logger.LogError(ex, "An error occurred in the course Patch method.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in the course Patch method." });
            }
        }

        /// <summary>
        /// Changing the status of the course.
        /// </summary>
        [HttpPatch("change-status/{courseId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(Permissions.CoursePermissions.ChangeStatus)]
        public async Task<IActionResult> ChangeStatusAsync(int courseId, [FromBody] CourseStatus newStatus)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
            }

            if (!Enum.IsDefined(typeof(CourseStatus), newStatus))
            {
                _logger.LogInformation($"Status {newStatus} is invalid.");
                return BadRequest(new GeneralResult { IsSuccess = false, Message = "Status is invalid." });
            }

            try
            {
                var result = await _trainingSection.ChangeCourseStatusAsync(courseId, newStatus);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course ChangeStatus method.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in the course ChangeStatus method." });
            }
        }

        /// <summary>
        /// Get all CoursePermissions.
        /// </summary>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(Permissions.CoursePermissions.ShowAll)]
        public override async Task<ActionResult<CourseDetailsDto[]>> GetAll()
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
                _logger.LogError(ex, "An error occurred in the course GetAll method.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while getting courses.", Data = null });
            }
        }

        /// <summary>
        /// Get one Course by its id.
        /// </summary>
        [HttpGet("get-one/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(Permissions.CoursePermissions.ShowOne)]
        public override async Task<ActionResult<CourseDetailsDto>> GetOne(int id)
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
                _logger.LogError(ex, "An error occurred in the course GetOne method.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while getting course.", Data = null });
            }
        }

        /// <summary>
        /// Get the full detailed information of a course including all related data.
        /// </summary>
        [HttpGet("full-details/{courseId}")]
        [ProducesResponseType(typeof(GeneralResult<CourseFullDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(Permissions.CoursePermissions.ShowOne)]
        public async Task<IActionResult> GetCourseFullDetailsAsync([FromRoute] int courseId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                if (courseId <= 0)
                {
                    return BadRequest(new GeneralResult { IsSuccess = false, Message = "Invalid Course ID." });
                }

                var result = await _trainingSection.GetCourseFullDetailsAsync(courseId);
                if (!result.IsSuccess)
                {
                    return NotFound(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching full details for CourseId {CourseId}.", courseId);
                return StatusCode(StatusCodes.Status500InternalServerError, new GeneralResult { IsSuccess = false, Message = "An error occurred while fetching course details." });
            }
        }


        /// <summary>
        /// Export all Courses to CSV file.
        /// </summary>
        [HttpGet("export/csv")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(Permissions.CoursePermissions.Export)]
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
                _logger.LogError(ex, "An error occurred in the course ExportToCsv method.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while exporting courses.", Data = null });
            }
        }

        /// <summary>
        /// Export all Courses to Excel file.
        /// </summary>
        [HttpGet("export/excel")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(Permissions.CoursePermissions.Export)]
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
                _logger.LogError(ex, "An error occurred in the course ExportToExcel method.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while exporting courses.", Data = null });
            }
        }

        /// <summary>
        /// Export all Courses to JSON file.
        /// </summary>
        [HttpGet("export/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(Permissions.CoursePermissions.Export)]
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
                _logger.LogError(ex, "An error occurred in the course ExportToJson method.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while exporting courses.", Data = null });
            }
        }

        /// <summary>
        /// Get all course statuses.
        /// </summary>
        [HttpGet("status")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(Permissions.CoursePermissions.GetEnumValues)]
        public ActionResult<IEnumerable<EnumData>> GetCourseStatuses()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var enumValues = _extendedBaseService.GetEnumValues<CourseStatus>();
                return Ok(enumValues);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the course GetCourseStatuses method.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while getting course statuses.", Data = null });
            }
        }

        /// <summary>
        /// Fetch courses based on the course status.
        /// </summary>
        [HttpGet("by-status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(Permissions.CoursePermissions.GetEnumValues)]
        public async Task<IActionResult> GetCourseByStatusAsync(int status)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
            }

            if (status < 0)
            {
                _logger.LogInformation($"Status {status} can not be negative.");
                return BadRequest(new GeneralResult { IsSuccess = false, Message = "Invalid status value, must be greater than or equal to zero." });
            }

            try
            {
                var courses = await _trainingSection.GetCourseBasedOnStatusAsync(status);
                if (courses.IsSuccess == false)
                {
                    _logger.LogInformation("No courses found for Status {Status}.", status);
                    return BadRequest(courses);
                }

                return Ok(courses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching courses by status: Status = {Status}", status);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new GeneralResult { IsSuccess = false, Data = null, Message = "An error occurred while fetching the courses." });
            }
        }

        /// <summary>
        /// Fetching courses by payment type (IsFree or not).
        /// </summary>
        [HttpGet("by-payment")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(Permissions.CoursePermissions.GetByPaymentType)]
        public async Task<IActionResult> GetCoursesByPaymentTypeAsync([FromQuery] bool isFree)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
            }

            try
            {
                var courses = await _trainingSection.GetCoursesByPaymentTypeAsync(isFree);
                if(courses.IsSuccess == false)
                {
                    return BadRequest(courses);
                }

                return Ok(courses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching courses by payment type: IsFree = {IsFree}", isFree);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new GeneralResult { IsSuccess = false, Data = null, Message = "An error occurred while fetching the courses." });
            }
        }

        /// <summary>
        /// Soft deletes an entity and its related entities with cascading soft delete.
        /// </summary>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(Permissions.CoursePermissions.CascadeDelete)]
        public async Task<IActionResult> DeleteWithCascade(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
            }

            if (id <= 0)
            {
                return BadRequest(new GeneralResult { IsSuccess = false, Message = "Invalid entity ID." });
            }

            var transactionId = Guid.NewGuid();
            _logger.LogInformation("Transaction {TransactionId}: Starting soft delete for entity ID {Id}.", transactionId, id);

            try
            {
                var result = await _deleteService.SoftDeleteCascadeAsync<Course>(id);
                if (result.IsSuccess == false)
                {
                    _logger.LogWarning("Transaction {TransactionId}: Entity with ID {Id} not found or already deleted.", transactionId, id);
                    return BadRequest(result);
                }

                _logger.LogInformation("Transaction {TransactionId}: Successfully soft deleted entity ID {Id}.", transactionId, id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Transaction {TransactionId}: An error occurred while soft deleting entity ID {Id}.", transactionId, id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new GeneralResult { IsSuccess = false, Data = null, Message = "An error occurred while soft deleting the entity." });
            }
        }

        [NonAction]
        public override Task<ActionResult> Delete(int id)
        {
            // Return a default value
            return Task.FromResult<ActionResult>(NotFound());
        }
    }
}

