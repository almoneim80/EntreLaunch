namespace EntreLaunch.Web.Controllers.TrainingAPI
{
    [Authorize(Roles = "Admin, Trainer, Student")]
    [Route("api/[controller]")]
    public class QuestionController : BaseController<Question, QuestionCreateDto, QuestionEntreLaunchdateDto, QuestionDetailsDto, QuestionExportDto>
    {
        private readonly IExtendedBaseService _extendedBaseService;
        private readonly ILogger<QuestionController> _logger;
        private readonly ITrainingSectionService _trainingSection;
        public QuestionController(
            BaseService<Question, QuestionCreateDto, QuestionEntreLaunchdateDto, QuestionDetailsDto> service,
            ILocalizationManager? localization,
            ILogger<QuestionController> logger,
            IExtendedBaseService extendedBaseService,
            IExportService exportService,
            ITrainingSectionService trainingSection)
            : base(service, localization, logger, exportService)
        {
            _extendedBaseService = extendedBaseService;
            _logger = logger;
            _trainingSection = trainingSection;
        }

        /// <summary>
        /// Create new Question.
        /// </summary>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(QuestionPermissions.Create)]
        public override async Task<ActionResult<QuestionDetailsDto>> Create([FromBody] QuestionCreateDto createDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var isReferencedValid = await _extendedBaseService.IsEntityExistsAndNotDeletedAsync<Exam>(createDto.ExamId);
                if (isReferencedValid.IsSuccess == false)
                {
                    return BadRequest(isReferencedValid);
                }

                return await base.Create(createDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new Question.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error occurred while creating a new Question." });
            }
        }

        /// <summary>
        /// create new Question with answers.
        /// </summary>
        [HttpPost("with-answers")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(QuestionPermissions.CreateFull)]
        public async Task<IActionResult> AddWithAnswers([FromBody] QuestionWithAnswers createDtoWithChildren)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
            }

            if (createDtoWithChildren == null)
            {
                return BadRequest(new GeneralResult { IsSuccess = false, Message = "Invalid data." });
            }

            try
            {
                var question = new Question
                {
                    ExamId = createDtoWithChildren.ExamId,
                    Text = createDtoWithChildren.Text,
                    Mark = createDtoWithChildren.Mark,
                    CreatedAt = DateTimeOffset.UtcNow,
                    IsDeleted = false,
                    Source = "QuestionWithChildren",
                    Answers = createDtoWithChildren.Answers?.Select(a => new Answer
                    {
                        Text = a.Text!,
                        IsCorrect = a.IsCorrect ?? false,
                        CreatedAt = DateTimeOffset.UtcNow,
                        IsDeleted = false,
                        Source = "QuestionWithChildren"
                    }).ToList()
                };

                await _extendedBaseService.AddEntityAsync(question);
                return Ok(question);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new Question.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error occurred while creating a new Question." });
            }
        }

        /// <summary>
        /// EntreLaunchdate an existing Question.
        /// </summary>
        [HttpPatch("edit/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(QuestionPermissions.Edit)]
        public override async Task<ActionResult<QuestionDetailsDto>> Patch(int id, [FromBody] QuestionEntreLaunchdateDto EntreLaunchdateDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var isReferencedValid = await _extendedBaseService.IsEntityExistsAndNotDeletedAsync<Exam>(EntreLaunchdateDto.ExamId);
                if (isReferencedValid.IsSuccess == false)
                {
                    return BadRequest(isReferencedValid);
                }

                return await base.Patch(id, EntreLaunchdateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while EntreLaunchdating an existing Question.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error occurred while EntreLaunchdating an existing Question." });
            }
        }

        /// <summary>
        /// Get All Questions.
        /// </summary>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(QuestionPermissions.ShowAll)]
        public override async Task<ActionResult<QuestionDetailsDto[]>> GetAll()
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
                _logger.LogError(ex, "Error occurred while getting all Questions.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error occurred while getting all Questions." });
            }
        }

        /// <summary>
        /// Get One Question.
        /// </summary>
        [HttpGet("get-one/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(QuestionPermissions.ShowOne)]
        public override async Task<ActionResult<QuestionDetailsDto>> GetOne(int id)
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
                _logger.LogError(ex, "Error occurred while getting one Question.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error occurred while getting one Question." });
            }
        }

        /// <summary>
        /// Get all questions and their answers for a specific exam.
        /// </summary>
        [HttpGet("questions/{examId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(Permissions.ExamPermissions.ShowAll)]
        public async Task<IActionResult> GetQuestionsWithAnswers(int examId)
        {
            try
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

                var result = await _trainingSection.GetQuestionsWithAnswersByExamIdAsync(examId);

                if (!result.IsSuccess)
                {
                    return NotFound(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching questions and answers for ExamId {ExamId}.", examId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new GeneralResult { IsSuccess = false, Message = "An unexpected error occurred while fetching questions and answers." });
            }
        }


        /// <summary>
        /// Export all Questions to CSV file.
        /// </summary>
        [HttpGet("export/csv")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(QuestionPermissions.Export)]
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
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to export Questions to CSV");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to export Questions to CSV" });
            }
        }

        /// <summary>
        /// Export all Questions to Excel file.
        /// </summary>
        [HttpGet("export/excel")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(QuestionPermissions.Export)]
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
                _logger.LogError(ex, "Failed to export Questions to Excel");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to export Questions to Excel" });
            }
        }

        /// <summary>
        /// Export all Questions to JSON file.
        /// </summary>
        [HttpGet("export/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(QuestionPermissions.Export)]
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
                _logger.LogError(ex, "Failed to export Questions to JSON");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to export Questions to JSON" });
            }
        }

        /// <summary>
        /// Delete an existing Question.
        /// </summary>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(QuestionPermissions.Delete)]
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
                _logger.LogError(ex, "Failed to delete Question");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to delete Question" });
            }
        }
    }
}
