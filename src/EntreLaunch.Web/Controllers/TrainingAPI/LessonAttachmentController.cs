namespace EntreLaunch.Web.Controllers.TrainingAPI
{
    [Authorize(Roles = "Admin, Trainer, Student")]
    [Route("api/[controller]")]
    public class LessonAttachmentController : BaseController<LessonAttachment, LessonAttachmentCreateDto, LessonAttachmentEntreLaunchdateDto, LessonAttachmentDetailsDto, LessonAttachmentExportDto>
    {
        private readonly IExtendedBaseService _extendedBaseService;
        private readonly IImportService<LessonAttachment, LessonAttachmentImportDto> _importService;
        private readonly ILogger<LessonAttachmentController> _logger;
        private readonly ITrainingSectionService _trainingSection;
        public LessonAttachmentController(
            BaseService<LessonAttachment, LessonAttachmentCreateDto, LessonAttachmentEntreLaunchdateDto, LessonAttachmentDetailsDto> service,
            ILocalizationManager? localization,
            ILogger<LessonAttachmentController> logger,
            IExtendedBaseService extendedBaseService,
            IImportService<LessonAttachment, LessonAttachmentImportDto> importService,
            ITrainingSectionService trainingSection,
            IExportService exportService)
            : base(service, localization, logger, exportService)
        {
            _extendedBaseService = extendedBaseService;
            _importService = importService;
            _logger = logger;
            _trainingSection = trainingSection;
        }

        /// <summary>
        /// Create a new LessonAttachment.
        /// </summary>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonAttachmentPermissions.Create)]
        public override async Task<ActionResult<LessonAttachmentDetailsDto>> Create([FromBody] LessonAttachmentCreateDto createDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var isReferencedValid = await _extendedBaseService.IsEntityExistsAndNotDeletedAsync<Lesson>(createDto.LessonId);
                if (isReferencedValid.IsSuccess == false)
                {
                    return BadRequest(isReferencedValid);
                }

                return await base.Create(createDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in Create.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in Create." });
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
        [RequiredPermission(LessonAttachmentPermissions.Import)]
        public async Task<ActionResult<ImportResult>> Import([FromBody] List<LessonAttachmentImportDto> importRecords)
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
        /// Increment counter when open or download attachment.
        /// </summary>
        [HttpPost("increment-open/{attachmentId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonAttachmentPermissions.OpenCounter)]
        public async Task<IActionResult> IncrementOpenCount(int attachmentId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var result = await _trainingSection.IncrementAttachmentOpenCountAsync(attachmentId);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(new { result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in IncrementOpenCount.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in IncrementOpenCount." });
            }
        }

        /// <summary>
        /// Validate the file based on extension and size.
        /// </summary>
        [HttpPost("validate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonAttachmentPermissions.ValidateFile)]
        public async Task<IActionResult> ValidateFile([FromBody] string filePath)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                if (string.IsNullOrEmpty(filePath))
                {
                    return BadRequest(new { Message = "File path cannot be null or empty." });
                }

                var result = await _trainingSection.IsValidFile(filePath);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in ValidateFile.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in ValidateFile." });
            }
        }

        /// <summary>
        /// EntreLaunchdate a LessonAttachment.
        /// </summary>
        [HttpPatch("edit/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonAttachmentPermissions.Edit)]
        public override async Task<ActionResult<LessonAttachmentDetailsDto>> Patch(int id, [FromBody] LessonAttachmentEntreLaunchdateDto EntreLaunchdateDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var isReferencedValid = await _extendedBaseService.IsEntityExistsAndNotDeletedAsync<Lesson>(EntreLaunchdateDto.LessonId ?? 0);
                if (isReferencedValid.IsSuccess == false)
                {
                    return BadRequest(isReferencedValid);
                }

                return await base.Patch(id, EntreLaunchdateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to EntreLaunchdate a Lesson Attachment");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to EntreLaunchdate a Lesson Attachment" });
            }
        }

        /// <summary>
        /// Get all LessonAttachments.
        /// </summary>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonAttachmentPermissions.ShowAll)]
        public override async Task<ActionResult<LessonAttachmentDetailsDto[]>> GetAll()
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
                _logger.LogError(ex, "Failed to get all Lesson Attachments");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to get all Lesson Attachments" });
            }
        }

        /// <summary>
        /// Get one LessonAttachment.
        /// </summary>
        [HttpGet("get-one/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonAttachmentPermissions.ShowOne)]
        public override async Task<ActionResult<LessonAttachmentDetailsDto>> GetOne(int id)
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
                _logger.LogError(ex, "Failed to get one Lesson Attachment");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to get one Lesson Attachment" });
            }
        }

        /// <summary>
        /// Export all LessonAttachments.
        /// </summary>
        [HttpGet("export/csv")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonAttachmentPermissions.Export)]
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
                _logger.LogError(ex, "Failed to export all Lesson Attachments");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to export all Lesson Attachments" });
            }
        }

        /// <summary>
        /// Export all LessonAttachments.
        /// </summary>
        [HttpGet("export/excel")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonAttachmentPermissions.Export)]
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
                _logger.LogError(ex, "Failed to export all Lesson Attachments");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to export all Lesson Attachments" });
            }
        }

        /// <summary>
        /// Export all LessonAttachments.
        /// </summary>
        [HttpGet("export/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonAttachmentPermissions.Export)]
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
                _logger.LogError(ex, "Failed to export all Lesson Attachments");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to export all Lesson Attachments" });
            }
        }

        /// <summary>
        /// Get attachment statstics by attachment id.
        /// </summary>
        [HttpGet("status/{attachmentId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonAttachmentPermissions.GetStats)]
        public async Task<IActionResult> GetAttachmentStatus(int attachmentId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var stats = await _trainingSection.GetAttachmentStatsAsync(attachmentId);
                if (stats.IsSuccess == false)
                {
                    return BadRequest(stats);
                }

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get attachment stats");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to get attachment stats" });
            }
        }

        /// <summary>
        /// Delete a LessonAttachment.
        /// </summary>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonAttachmentPermissions.Delete)]
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
                _logger.LogError(ex, "Failed to delete LessonAttachment");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to delete LessonAttachment" });
            }
        }
    }
}
