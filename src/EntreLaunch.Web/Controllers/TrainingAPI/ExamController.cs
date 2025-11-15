namespace EntreLaunch.Web.Controllers.TrainingAPI
{
    [Authorize(Roles = "Admin, Trainer, Student")]
    [Route("api/[controller]")]
    public class ExamController : BaseController<Exam, ExamCreateDto, ExamEntreLaunchdateDto, ExamDetailsDto, ExamExportDto>
    {
        private readonly ILogger<ExamController> _logger;
        private readonly CascadeDeleteService _deleteService;
        private readonly IExtendedBaseService _extendedBaseService;
        private readonly IImportService<Exam, ExamImportDto> _importService;
        private readonly ITrainingSectionService _trainingSection;
        public ExamController(
            BaseService<Exam, ExamCreateDto, ExamEntreLaunchdateDto, ExamDetailsDto> service,
            ILogger<ExamController> logger,
            CascadeDeleteService deleteService,
            ILocalizationManager? localization,
            IExtendedBaseService extendedBaseService,
            IImportService<Exam, ExamImportDto> importService,
            ITrainingSectionService trainingSection,
            IExportService exportService)
            : base(service, localization, logger, exportService)
        {
            _logger = logger;
            _deleteService = deleteService;
            _extendedBaseService = extendedBaseService;
            _importService = importService;
            _trainingSection = trainingSection;
        }

        /// <summary>
        /// Creates a new entity.
        /// </summary>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ExamPermissions.Create)]
        public override async Task<ActionResult<ExamDetailsDto>> Create([FromBody] ExamCreateDto createDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var hasCourseId = createDto.CourseId.HasValue;
                var hasLessonId = createDto.LessonId.HasValue;
                var hasPathId = createDto.PathId.HasValue;

                var selectedCount = (hasCourseId ? 1 : 0) + (hasLessonId ? 1 : 0) + (hasPathId ? 1 : 0);
                if (selectedCount != 1)
                {
                    return BadRequest(new GeneralResult { IsSuccess = false, Message = "You must select one of the following: Course, Lesson or Path." });
                }

                if (hasCourseId)
                {
                    var isCourseValid = await _extendedBaseService.IsEntityExistsAndNotDeletedAsync<Course>(createDto.CourseId ?? 0);
                    if (isCourseValid.IsSuccess == false)
                    {
                        return BadRequest(isCourseValid);
                    }
                }

                if (hasLessonId)
                {
                    var isLessonValid = await _extendedBaseService.IsEntityExistsAndNotDeletedAsync<Lesson>(createDto.LessonId ?? 0);
                    if (isLessonValid.IsSuccess == false)
                    {
                        return BadRequest(isLessonValid);
                    }
                }

                if (hasPathId)
                {
                    var isPathValid = await _extendedBaseService.IsEntityExistsAndNotDeletedAsync<TrainingPath>(createDto.PathId ?? 0);
                    if (isPathValid.IsSuccess == false)
                    {
                        return BadRequest(isPathValid);
                    }
                }

                return await base.Create(createDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new entity.");
                return StatusCode(StatusCodes.Status500InternalServerError, new GeneralResult { IsSuccess = false, Message = "An error occurred while creating a new entity." });
            }
        }

        /// <summary>
        /// add full exam.
        /// </summary>
        [HttpPost("add-all")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ExamPermissions.CreateFull)]
        public async Task<IActionResult> AddWithChildren([FromBody] ExamWithChildrenDto examWithChildren)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
            }

            if (examWithChildren == null)
            {
                return BadRequest(new GeneralResult { IsSuccess = false, Message = "Data Can Not Be Null" });
            }

            try
            {
                var exam = new Exam
                {
                    Name = examWithChildren.Name,
                    Type = examWithChildren.Type,
                    Description = examWithChildren.Description,
                    MinMark = examWithChildren.MinMark,
                    MaxMark = examWithChildren.MaxMark,
                    DurationInMinutes = examWithChildren.DurationInMinutes,
                    ParentEntityType = examWithChildren.ParentEntityType ?? ExamParentEntityType.Course,
                    CourseId = examWithChildren.CourseId,
                    LessonId = examWithChildren.LessonId,
                    PathId = examWithChildren.PathId,

                    CreatedAt = DateTimeOffset.UtcNow,
                    IsDeleted = false,
                    Source = "ExamWithChildrenDto",

                    Questions = examWithChildren.Questions?.Select(q => new Question
                    {
                        Text = q.Text,
                        Mark = q.Mark,

                        CreatedAt = DateTimeOffset.UtcNow,
                        IsDeleted = false,
                        Source = "ExamWithChildrenDto",

                        Answers = q.Answers?.Select(ans => new Answer
                        {
                            Text = ans.Text ?? "",
                            IsCorrect = ans.IsCorrect ?? false,

                            CreatedAt = DateTimeOffset.UtcNow,
                            IsDeleted = false,
                            Source = "ExamWithChildrenDto",
                        }).ToList()
                    }).ToList()
                };

                await _extendedBaseService.AddEntityAsync(exam);
                return Ok(new GeneralResult { IsSuccess = true, Message = "Exam added successfully.", Data = exam });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new entity.");
                return StatusCode(StatusCodes.Status500InternalServerError, new GeneralResult { IsSuccess = false, Message = "An error occurred while adding a new entity." });
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
        [RequiredPermission(ExamPermissions.Import)]
        public async Task<ActionResult<ImportResult>> Import([FromBody] List<ExamImportDto> importRecords)
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
                _logger.LogError(ex, "An error occurred in Import.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in Import." });
            }
        }

        /// <summary>
        /// Retake an exam.
        /// </summary>
        [HttpPost("retake/{examId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ExamPermissions.Retake)]
        public async Task<IActionResult> RetakeExamAsync(int examId, [FromBody] ExamSubmissionDto submission)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var result = await _trainingSection.RetakeExamAsync(
                    examId, submission.UserId, submission.Answers, submission.TimeTakenInSeconds);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Retake not allowed: {Message}", ex.Message);
                return BadRequest(new { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retaking Exam {ExamId} for User {UserId}.", examId, submission.UserId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { Message = "An error occurred while processing the retake request." });
            }
        }

        /// <summary>
        /// EntreLaunchdates an existing entity.
        /// </summary>
        [HttpPatch("edit/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ExamPermissions.Edit)]
        public override async Task<ActionResult<ExamDetailsDto>> Patch(int id, [FromBody] ExamEntreLaunchdateDto EntreLaunchdateDto)
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
                _logger.LogError(ex, "An error occurred in the Exam Patch method.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in the Exam Patch method." });
            }
        }

        /// <summary>
        /// Gets all entities.
        /// </summary>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ExamPermissions.ShowAll)]
        public override async Task<ActionResult<ExamDetailsDto[]>> GetAll()
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
                _logger.LogError(ex, "An error occurred in the Exam GetAll method.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in the Exam GetAll method." });
            }
        }

        /// <summary>
        /// Gets one entity by its id.
        /// </summary>
        [HttpGet("get-one/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ExamPermissions.ShowOne)]
        public override async Task<ActionResult<ExamDetailsDto>> GetOne(int id)
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
                _logger.LogError(ex, "An error occurred in the Exam GetOne method.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in the Exam GetOne method." });
            }
        }

        /// <summary>
        /// Get full details of an exam, including questions and answers.
        /// </summary>
        [HttpGet("exam-full-details/{examId:int}")]
        [ProducesResponseType(typeof(GeneralResult<ExamFullDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(Permissions.ExamPermissions.ShowOne)]
        public async Task<IActionResult> GetExamFullDetailsAsync(int examId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
            }

            if (examId <= 0)
            {
                return BadRequest(new GeneralResult { IsSuccess = false, Message = "Invalid exam ID." });
            }

            try
            {
                var examDetails = await _trainingSection.GetExamFullDetailsAsync(examId);
                if (!examDetails.IsSuccess)
                {
                    return NotFound(examDetails);
                }

                return Ok(examDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the GetExamFullDetailsAsync method for ExamId {ExamId}.", examId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new GeneralResult { IsSuccess = false, Message = "An unexpected error occurred while fetching exam details." });
            }
        }


        /// <summary>
        /// Exports all entities to Excel file.
        /// </summary>
        [HttpGet("export/excel")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ExamPermissions.Export)]
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
                _logger.LogError(ex, "An error occurred in the Exam ExportToExcel method.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while exporting exams.", Data = null });
            }
        }

        /// <summary>
        /// Exports all entities to CSV file.
        /// </summary>
        [HttpGet("export/csv")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ExamPermissions.Export)]
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
                _logger.LogError(ex, "An error occurred in the Exam ExportToCsv method.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while exporting exams.", Data = null });
            }
        }

        /// <summary>
        /// Exports all entities to JSON file.
        /// </summary>
        [HttpGet("export/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ExamPermissions.Export)]
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
                _logger.LogError(ex, "An error occurred in the Exam ExportToJson method.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while exporting exams.", Data = null });
            }
        }

        /// <summary>
        /// Get all Exam Parent Entity Type.
        /// </summary>
        [HttpGet("parent-type")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ExamPermissions.GetEnumValues)]
        public ActionResult<IEnumerable<EnumData>> GetExamParentEntityType()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var enumValues = _extendedBaseService.GetEnumValues<ExamParentEntityType>();
                if(enumValues == null)
                {
                    return BadRequest(new GeneralResult { IsSuccess = false, Message = "No Exam Parent Entity Type found. please try again", Data = null });
                }

                return Ok(new GeneralResult { IsSuccess = true, Message = "Success", Data = enumValues });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the Exam GetExamParentEntityType method.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while getting Exam Parent Entity Type.", Data = null });
            }
        }

        /// <summary>
        /// Check if a user can retake an exam.
        /// </summary>
        [HttpGet("can-retake/{examId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [RequiredPermission(ExamPermissions.CanRetake)]
        public async Task<IActionResult> CanRetakeExamAsync(int examId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
            }

            try
            {
                var canRetake = await _trainingSection.CanRetakeExamAsync(examId, userId);
                if (canRetake.IsSuccess == false)
                {
                    return BadRequest(canRetake);
                }

                return Ok(canRetake);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the Exam CanRetakeExamAsync method.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while checking if the user can retake the exam.", Data = null });
            }
        }

        /// <summary>
        /// Get all attempts for a student.
        /// </summary>
        [HttpGet("attempt/{examId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [RequiredPermission(ExamPermissions.GetAttempt)]
        public async Task<IActionResult> GetStudentAttemptsAsync(int examId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
            }

            try
            {
                var attemptsWithBest = await _trainingSection.GetStudentAttemptsAsync(examId, userId);
                if (attemptsWithBest.IsSuccess == false)
                {
                    return BadRequest(attemptsWithBest);
                }

                return Ok(attemptsWithBest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the Exam GetStudentAttemptsAsync method.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while getting student attempts.", Data = null });
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
        [RequiredPermission(ExamPermissions.CascadeDelete)]
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
            _logger.LogInformation("Transaction {TransactionId}: Starting soft delete for Exam ID {Id}.", transactionId, id);

            try
            {
                var result = await _deleteService.SoftDeleteCascadeAsync<Exam>(id);
                if (result.IsSuccess == false)
                {
                    return NotFound(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the Exam DeleteWithCascade method.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while soft deleting the Exam.", Data = null });
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

