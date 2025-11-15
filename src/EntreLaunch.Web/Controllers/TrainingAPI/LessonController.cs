using Microsoft.AspNetCore.Http.HttpResults;

namespace EntreLaunch.Web.Controllers.TrainingAPI
{
    [Authorize(Roles = "Admin, Trainer, Student")]
    [Route("api/[controller]")]
    public class LessonController : BaseController<Lesson, LessonCreateDto, LessonEntreLaunchdateDto, LessonDetailsDto, LessonExportDto>
    {
        private readonly ILogger<LessonController> _logger;
        private readonly CascadeDeleteService _deleteService;
        private readonly IExtendedBaseService _extendedBaseService;
        private readonly IImportService<Lesson, LessonImportDto> _importService;
        private readonly ITrainingSectionService _trainingSection;
        private readonly IStudentProgress _studentProgress;
        public LessonController(
            BaseService<Lesson, LessonCreateDto, LessonEntreLaunchdateDto, LessonDetailsDto> service,
            ILogger<LessonController> logger,
            CascadeDeleteService deleteService,
            ILocalizationManager? localization,
            IExtendedBaseService extendedBaseService,
            IImportService<Lesson, LessonImportDto> importService,
            ITrainingSectionService trainingSection,
            IStudentProgress studentProgress,
            IExportService exportService)
            : base(service, localization, logger, exportService)
        {
            _logger = logger;
            _deleteService = deleteService;
            _extendedBaseService = extendedBaseService;
            _importService = importService;
            _trainingSection = trainingSection;
            _studentProgress = studentProgress;
        }

        /// <summary>
        /// Creates a new Lesson.
        /// </summary>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonPermissions.Create)]
        public override async Task<ActionResult<LessonDetailsDto>> Create([FromBody] LessonCreateDto createDto)
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
                return await base.Create(createDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create a new Lesson");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to create a new Lesson" });
            }
        }

        /// <summary>
        /// Recalculate student progress.
        /// </summary>
        [HttpPost("recalculate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonPermissions.CalculateProgress)]
        public async Task<IActionResult> RecalculateProgress([FromBody] RecalculateProgressRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                if (!ModelState.IsValid)
                    return BadRequest(new GeneralResult { IsSuccess = false, Message = "Invalid data.", Data = ModelState });

                var percentage = await _studentProgress.CalculateCompletionPercentageAsync(request.CourseId, request.UserId!);

                return Ok(new { CompletionPercentage = percentage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to recalculate student progress");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to recalculate student progress" });
            }
        }

        /// <summary>
        /// Creates a new Lesson with attachments.
        /// </summary>
        [HttpPost("with-attachments")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonPermissions.CreateFull)]
        public async Task<IActionResult> AddWithAttachments([FromBody] LessonWithChildrenDto lessonWithChildrenDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
            }

            if (lessonWithChildrenDto == null)
            {
                return BadRequest(new GeneralResult { IsSuccess = false, Message = "Invalid data." });
            }

            var isCourseValid = await _extendedBaseService.IsEntityExistsAndNotDeletedAsync<Course>(lessonWithChildrenDto.CourseId);
            if (isCourseValid.IsSuccess == false)
            {
                return BadRequest(new GeneralResult { IsSuccess = false, Message = "The referenced course does not exist or has been deleted." });
            }

            try
            {
                var lessonWithAttachments = new Lesson
                {
                    Name = lessonWithChildrenDto.Name,
                    VideoUrl = lessonWithChildrenDto.VideoUrl,
                    DurationInMinutes = lessonWithChildrenDto.DurationInMinutes,
                    Description = lessonWithChildrenDto.Description,
                    CourseId = lessonWithChildrenDto.CourseId,
                    CreatedAt = DateTimeOffset.UtcNow,
                    IsDeleted = false,
                    Source = "lessonWithChildren",
                    LessonAttachments = lessonWithChildrenDto.LessonAttachmentFullAddDtos?.Select(a => new LessonAttachment
                    {
                        FileName = a.FileName ?? "",
                        FileUrl = a.FileUrl ?? "",
                        CreatedAt = DateTimeOffset.UtcNow,
                        IsDeleted = false,
                        Source = "lessonWithChildren"
                    }).ToList()
                };

                await _extendedBaseService.AddEntityAsync(lessonWithAttachments);
                return Ok(lessonWithAttachments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create a new Lesson");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to create a new Lesson" });
            }
        }

        /// <summary>
        /// Imports data from a list.
        /// (id must be unique.)
        /// </summary>
        [HttpPost("import")]
        [RequestSizeLimit(100 * 1024 * 1024)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonPermissions.Import)]
        public async Task<ActionResult<ImportResult>> Import([FromBody] List<LessonImportDto> importRecords)
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
                _logger.LogError(ex, "Failed to import data");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to import data" });
            }
        }

        /// <summary>
        /// EntreLaunchdate student progress.
        /// </summary>
        [HttpPost("progress/EntreLaunchdate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonPermissions.EntreLaunchdateProgress)]
        public async Task<IActionResult> EntreLaunchdateProgress([FromBody] EntreLaunchdateProgressRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                if (!ModelState.IsValid)
                    return BadRequest(new GeneralResult { IsSuccess = false, Message = "Invalid data.", Data = ModelState });

                var result = await _studentProgress.EntreLaunchdateStudentProgressAsync(
                    request.CourseId, request.UserId!, request.LessonId, request.TimeSpent);

                if (result.IsSuccess == false)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to EntreLaunchdate student progress");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to EntreLaunchdate student progress" });
            }
        }

        /// <summary>
        /// EntreLaunchdates an existing Lesson.
        /// </summary>
        [HttpPatch("edit/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonPermissions.Edit)]
        public override async Task<ActionResult<LessonDetailsDto>> Patch(int id, [FromBody] LessonEntreLaunchdateDto EntreLaunchdateDto)
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
                _logger.LogError(ex, "Failed to EntreLaunchdate Lesson");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to EntreLaunchdate Lesson" });
            }
        }

        /// <summary>
        /// Rearrange the order of lessons within a course.
        /// </summary>
        [HttpPut("reorder/{courseId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonPermissions.Reorder)]
        public async Task<IActionResult> ReorderLessonsAsync(int courseId, [FromBody] List<LessonReorderDto> newOrderList)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var success = await _trainingSection.ReorderLessonsAsync(courseId, newOrderList);
                if (success.IsSuccess == false)
                {
                    return BadRequest(success);
                }

                return Ok(success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reorder Lessons");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to reorder Lessons" });
            }
        }

        /// <summary>
        /// Gets all Lessons.
        /// </summary>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonPermissions.ShowAll)]
        public override async Task<ActionResult<LessonDetailsDto[]>> GetAll()
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
                _logger.LogError(ex, "Failed to get all Lessons");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to get all Lessons" });
            }
        }

        /// <summary>
        /// Gets one Lesson.
        /// </summary>
        [HttpGet("get-one/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonPermissions.ShowOne)]
        public override async Task<ActionResult<LessonDetailsDto>> GetOne(int id)
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
                _logger.LogError(ex, "Failed to get one Lesson");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to get one Lesson" });
            }
        }

        /// <summary>
        /// Exports all Lessons to CSV file.
        /// </summary>
        [HttpGet("export/csv")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonPermissions.Export)]
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
                _logger.LogError(ex, "Failed to export Lessons to CSV");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to export Lessons to CSV" });
            }
        }

        /// <summary>
        /// Exports all Lessons to Excel file.
        /// </summary>
        [HttpGet("export/excel")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonPermissions.Export)]
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
                _logger.LogError(ex, "Failed to export Lessons to Excel");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to export Lessons to Excel" });
            }
        }

        /// <summary>
        /// Exports all Lessons to JSON file.
        /// </summary>
        [HttpGet("export/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonPermissions.Export)]
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
                _logger.LogError(ex, "Failed to export Lessons to JSON");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to export Lessons to JSON" });
            }
        }

        /// <summary>
        /// Get student progress.
        /// </summary>
        [HttpGet("course-lessons-progress/{courseId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonPermissions.GetProgress)]
        public async Task<IActionResult> GetProgress(int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
            }

            var isCourseValid = await _extendedBaseService.IsEntityExistsAndNotDeletedAsync<Course>(courseId);
            if (isCourseValid.IsSuccess == false)
            {
                return BadRequest(new GeneralResult { IsSuccess = false, Message = "The referenced course does not exist or has been deleted." });
            }

            try
            {
                var progress = await _studentProgress.GetStudentProgressAsync(courseId, userId);
                if (progress.IsSuccess == false)
                {
                    return BadRequest(progress);
                }

                return Ok(progress);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get student progress");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to get student progress" });
            }
        }

        /// <summary>
        /// Get all lessons for a specific course.
        /// </summary>
        [HttpGet("by-course/{courseId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonPermissions.GetByCourse)]
        public async Task<ActionResult<LessonDetailsDto[]>> GetLessonsByCourseIdAsync(int courseId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var lessons = await _trainingSection.GetLessonsByCourseIdAsync(courseId);
                if (lessons.IsSuccess == false)
                {
                    return BadRequest(lessons);
                }

                return Ok(lessons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get lessons by course id");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to get lessons by course id" });
            }
        }

        /// <summary>
        /// Soft deletes an entity and its related entities with cascading soft delete.
        /// </summary>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonPermissions.CascadeDelete)]
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
                var result = await _deleteService.SoftDeleteCascadeAsync<Lesson>(id);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Transaction {TransactionId}: Failed to soft delete entity ID {Id}.", transactionId, id);
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to soft delete entity." });
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
