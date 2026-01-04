namespace EntreLaunch.Web.Controllers.TrainingAPI
{
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.TrainingRoles)]
    public class AnswerController : BaseController<Answer, AnswerCreateDto, AnswerUpdateDto, AnswerDetailsDto, AnswerExportDto>
    {
        private readonly IExtendedBaseService _extendedBaseService;
        private readonly IImportService<Answer, AnswerImportDto> _importService;
        private readonly ILogger<AnswerController> _logger;
        public AnswerController(
            BaseService<Answer, AnswerCreateDto, AnswerUpdateDto, AnswerDetailsDto> service,
            ILocalizationManager? localization,
            ILogger<AnswerController> logger,
            IExtendedBaseService extendedBaseService,
            IImportService<Answer, AnswerImportDto> importService,
            IExportService exportService)
        : base(service, localization, logger, exportService)
        {
            _extendedBaseService = extendedBaseService;
            _importService = importService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new answer for a specified question.
        /// Validates that the referenced question exists and is not deleted before proceeding.
        /// Requires the user to have permission to create answers.
        /// </summary>
        /// <param name="createDto">
        /// An <see cref="AnswerCreateDto"/> object containing the details of the answer to be created,
        /// including the associated question ID and the answer content.
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing an <see cref="AnswerDetailsDto"/> representing the created answer:
        /// <list type="bullet">
        /// <item><description><c>200 OK</c>: Successfully created the new answer (inherited behavior).</description></item>
        /// <item><description><c>204 No Content</c>: Operation succeeded but no content is returned.</description></item>
        /// <item><description><c>400 Bad Request</c>: Invalid input data or the referenced question does not exist or has been deleted.</description></item>
        /// <item><description><c>401 Unauthorized</c>: The user is not authenticated.</description></item>
        /// <item><description><c>404 Not Found</c>: The referenced question could not be found (depending on validation logic).</description></item>
        /// <item><description><c>500 Internal Server Error</c>: An unexpected error occurred while creating the answer.</description></item>
        /// </list>
        /// </returns>
        [HttpPost("create")]
        [SwaggerOperation(Tags = new[] { "ExamsManegment" })]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(AnswerPermissions.Create)]
        public override async Task<ActionResult<AnswerDetailsDto>> Create([FromBody] AnswerCreateDto createDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var isReferencedValid = await _extendedBaseService.IsEntityExistsAndNotDeletedAsync<Question>(createDto.QuestionId);
                if (isReferencedValid.IsSuccess == false)
                {
                    return BadRequest(new GeneralResult { IsSuccess = false, Message = "The referenced referenced Entity does not exist or has been deleted." });
                }
                return await base.Create(createDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the answer Create method.");
                return this.UnexpectedError("creating answer.");
            }
        }

        /// <summary>
        /// Imports a list of answers in bulk into the system.
        /// Validates that data is provided before processing the import operation.
        /// Requires the user to have permission to perform import operations.
        /// </summary>
        /// <param name="importRecords">
        /// A list of <see cref="AnswerImportDto"/> objects representing the answers to be imported.
        /// Each record should contain the necessary fields for a valid answer entry.
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing an <see cref="ImportResult"/> that summarizes the outcome of the import operation:
        /// <list type="bullet">
        /// <item><description><c>200 OK</c>: Successfully imported the answers.</description></item>
        /// <item><description><c>400 Bad Request</c>: No data was provided for import or validation failed.</description></item>
        /// <item><description><c>401 Unauthorized</c>: The user is not authenticated.</description></item>
        /// <item><description><c>422 Unprocessable Entity</c>: The server understands the request but the input was semantically incorrect.</description></item>
        /// <item><description><c>500 Internal Server Error</c>: An unexpected error occurred during the import process.</description></item>
        /// </list>
        /// </returns>
        [HttpPost("import")]
        [SwaggerOperation(Tags = new[] { "ExamsManegment" })]
        [RequestSizeLimit(100 * 1024 * 1024)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(AnswerPermissions.Import)]
        public async Task<ActionResult<ImportResult>> Import([FromBody] List<AnswerImportDto> importRecords)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                if (importRecords == null || importRecords.Count == 0)
                {
                    _logger.LogWarning("No data provided for import operation.");
                    return BadRequest(new GeneralResult { IsSuccess = false, Message = "No data provided for import." });
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
                _logger.LogError(ex, "An error occurred in the answer Import method.");
                return this.UnexpectedError("importing answers.");
            }
        }

        /// <summary>
        /// Updates an existing answer identified by its ID using the provided update data.
        /// Validates that the associated question exists and is not deleted before applying updates.
        /// Requires the user to have permission to edit answers.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the answer to be updated. Must refer to an existing answer record.
        /// </param>
        /// <param name="updateDto">
        /// An <see cref="AnswerUpdateDto"/> object containing the updated data for the answer,
        /// including the reference to a valid question ID and the updated answer content.
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing an <see cref="AnswerDetailsDto"/> with the updated answer details if successful:
        /// <list type="bullet">
        /// <item><description><c>200 OK</c>: The answer was successfully updated (behavior inherited from the base class).</description></item>
        /// <item><description><c>204 No Content</c>: The update operation completed successfully with no additional content.</description></item>
        /// <item><description><c>400 Bad Request</c>: Input validation failed or the referenced question does not exist or has been deleted.</description></item>
        /// <item><description><c>401 Unauthorized</c>: The user is not authenticated.</description></item>
        /// <item><description><c>404 Not Found</c>: The specified answer was not found.</description></item>
        /// <item><description><c>500 Internal Server Error</c>: An unexpected error occurred during the update process.</description></item>
        /// </list>
        /// </returns>
        [HttpPatch("Edit/{id}")]
        [SwaggerOperation(Tags = new[] { "ExamsManegment" })]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(AnswerPermissions.Edit)]
        public override async Task<ActionResult<AnswerDetailsDto>> Patch([FromRoute] int id, [FromBody] AnswerUpdateDto updateDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var isReferencedValid = await _extendedBaseService.IsEntityExistsAndNotDeletedAsync<Question>(updateDto.QuestionId);
                if (isReferencedValid.IsSuccess == false)
                {
                    return BadRequest(new GeneralResult { IsSuccess = false, Message = "The referenced referenced Entity does not exist or has been deleted." });
                }

                return await base.Patch(id, updateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the answer Patch method.");
                return this.UnexpectedError("updating answer.");
            }
        }

        /// <summary>
        /// Retrieves all available answers from the system.
        /// Requires the user to have permission to view all answers.
        /// </summary>
        [HttpGet("all")]
        [SwaggerOperation(Tags = new[] { "ExamsManegment" })]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(AnswerPermissions.GetAll)]
        public override async Task<ActionResult<GeneralResult<PaginatedResult<AnswerDetailsDto>>>> GetAll([FromQuery] PaginationParams pagination)
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
                _logger.LogError(ex, "An error occurred in the answer GetAll method.");
                return this.UnexpectedError("getting answers.");
            }
        }

        /// <summary>
        /// Retrieves the details of a specific answer identified by its unique ID.
        /// Requires the user to have permission to view individual answers.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the answer to retrieve. Must refer to an existing answer entry.
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing an <see cref="AnswerDetailsDto"/> with the answer details if successful:
        /// <list type="bullet">
        /// <item><description><c>200 OK</c>: Successfully retrieved the answer details.</description></item>
        /// <item><description><c>204 No Content</c>: The answer exists but there is no content to return (unlikely with the current implementation).</description></item>
        /// <item><description><c>400 Bad Request</c>: A bad request occurred (reserved for business validation if implemented).</description></item>
        /// <item><description><c>401 Unauthorized</c>: The user is not authenticated.</description></item>
        /// <item><description><c>404 Not Found</c>: The specified answer was not found.</description></item>
        /// <item><description><c>500 Internal Server Error</c>: An unexpected error occurred while retrieving the answer.</description></item>
        /// </list>
        /// </returns>
        [HttpGet("get-one/{id}")]
        [SwaggerOperation(Tags = new[] { "ExamsManegment" })]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(AnswerPermissions.GetOne)]
        public override async Task<ActionResult<AnswerDetailsDto>> GetOne([FromRoute] int id)
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
                _logger.LogError(ex, "An error occurred in the answer GetOne method.");
                return this.UnexpectedError("getting answer.");
            }
        }

        /// <summary>
        /// Deletes a specific answer identified by its unique ID.
        /// Requires the user to have permission to delete answers.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the answer to be deleted. Must refer to an existing answer entry.
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult"/> indicating the result of the delete operation:
        /// <list type="bullet">
        /// <item><description><c>204 No Content</c>: The answer was successfully deleted.</description></item>
        /// <item><description><c>400 Bad Request</c>: A bad request occurred (reserved for validation failures if implemented).</description></item>
        /// <item><description><c>401 Unauthorized</c>: The user is not authenticated.</description></item>
        /// <item><description><c>404 Not Found</c>: The specified answer was not found.</description></item>
        /// <item><description><c>500 Internal Server Error</c>: An unexpected error occurred during the deletion process.</description></item>
        /// </list>
        /// </returns>
        [HttpDelete("delete/{id}")]
        [SwaggerOperation(Tags = new[] { "ExamsManegment" })]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(AnswerPermissions.Delete)]
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
                _logger.LogError(ex, "An error occurred in the answer Delete method.");
                return this.UnexpectedError("deleting answer.");
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
    }
}
