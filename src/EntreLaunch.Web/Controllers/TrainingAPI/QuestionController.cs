namespace EntreLaunch.Web.Controllers.TrainingAPI
{
    [Authorize(Roles = AppRoles.TrainingRoles)]
    [Route("api/[controller]")]
    public class QuestionController(
        BaseService<Question, QuestionCreateDto, QuestionUpdateDto, QuestionDetailsDto> service,
        ILocalizationManager? localization,
        IExamService examService,
        ILogger<QuestionController> logger,
        IExtendedBaseService extendedBaseService,
        IExportService exportService) : BaseController<Question, QuestionCreateDto, QuestionUpdateDto, QuestionDetailsDto, QuestionExportDto>(service, localization, logger, exportService)
    {
        private readonly IExtendedBaseService _extendedBaseService = extendedBaseService;
        private readonly ILogger<QuestionController> _logger = logger;

        /// <summary>
        /// Creates a new question and associates it with a specific exam.
        /// </summary>
        /// <param name="createDto">The data transfer object containing the question details and associated exam ID.</param>
        /// <returns>Returns the created question details if successful; otherwise, returns appropriate error responses.</returns>
        [HttpPost("create")]
        [SwaggerOperation(Tags = new[] { "ExamsManegment" })]
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
        /// Creates a new question along with its possible answers in a single operation.
        /// </summary>
        /// <param name="createDtoWithChildren">The DTO containing question properties and a list of answer definitions.</param>
        /// <returns>Returns the created question entity including the child answers if successful.</returns>
        [HttpPost("with-answers")]
        [SwaggerOperation(Tags = new[] { "ExamsManegment" })]
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
        /// Updates the details of an existing question.
        /// </summary>
        /// <param name="id">The unique identifier of the question to update.</param>
        /// <param name="updateDto">The data transfer object containing the updated question details.</param>
        /// <returns>Returns the updated question details if the operation is successful.</returns>
        [HttpPatch("edit/{id}")]
        [SwaggerOperation(Tags = new[] { "ExamsManegment" })]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(QuestionPermissions.Edit)]
        public override async Task<ActionResult<QuestionDetailsDto>> Patch([FromRoute] int id, [FromBody] QuestionUpdateDto updateDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var isReferencedValid = await _extendedBaseService.IsEntityExistsAndNotDeletedAsync<Exam>(updateDto.ExamId);
                if (isReferencedValid.IsSuccess == false)
                {
                    return BadRequest(isReferencedValid);
                }

                return await base.Patch(id, updateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating an existing Question.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error occurred while updating an existing Question." });
            }
        }

        /// <summary>
        /// Retrieves all questions available in the system.
        /// </summary>
        /// <returns>Returns a collection of all question details.</returns>
        [HttpGet("all")]
        [SwaggerOperation(Tags = new[] { "ExamsManegment" })]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(QuestionPermissions.GetAll)]
        public override async Task<ActionResult<GeneralResult<PaginatedResult<QuestionDetailsDto>>>> GetAll([FromQuery] PaginationParams pagination)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                return await base.GetAll(pagination);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all Questions.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error occurred while getting all Questions." });
            }
        }

        /// <summary>
        /// Retrieves a single question based on its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the question to retrieve.</param>
        /// <returns>Returns the detailed information of the specified question.</returns>
        [HttpGet("get-one/{id}")]
        [SwaggerOperation(Tags = new[] { "ExamsManegment" })]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(QuestionPermissions.GetOne)]
        public override async Task<ActionResult<QuestionDetailsDto>> GetOne([FromRoute] int id)
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
        /// Retrieves all questions along with their answers associated with a specific exam.
        /// </summary>
        /// <param name="examId">The ID of the exam to fetch questions and answers for.</param>
        /// <returns>Returns a list of questions with their answers linked to the given exam.</returns>
        [HttpGet("questions/{examId}")]
        [SwaggerOperation(Tags = new[] { "ExamsManegment" })]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(Permissions.ExamPermissions.GetAll)]
        public async Task<IActionResult> GetQuestionsWithAnswers([FromRoute] int examId)
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

                var result = await examService.GetQuestionsWithAnswersByExamIdAsync(examId);

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

        #region Export
        [NonAction]
        public override async Task<IActionResult> ExportToCsv()
        {
            return await Task.FromResult((IActionResult)NoContent());
        }

        [NonAction]
        public override async Task<IActionResult> ExportToExcel()
        {
            return await Task.FromResult((IActionResult)NoContent());
        }

        [NonAction]
        public override async Task<IActionResult> ExportToJson()
        {
            return await Task.FromResult((IActionResult)NoContent());
        }
        #endregion

        /// <summary>
        /// Deletes a specific question based on its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the question to delete.</param>
        /// <returns>Returns a success status if the question was deleted successfully.</returns>
        [HttpDelete("delete/{id}")]
        [SwaggerOperation(Tags = new[] { "ExamsManegment" })]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(QuestionPermissions.Delete)]
        public override async Task<ActionResult> Delete([FromRoute] int id)
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
