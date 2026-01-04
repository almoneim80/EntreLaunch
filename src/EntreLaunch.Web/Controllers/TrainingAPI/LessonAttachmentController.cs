namespace EntreLaunch.Web.Controllers.TrainingAPI
{
    [Authorize(Roles = AppRoles.TrainingRoles)]
    [Route("api/[controller]")]
    public class LessonAttachmentController(
        BaseService<LessonAttachment, LessonAttachmentCreateDto, LessonAttachmentUpdateDto, LessonAttachmentDetailsDto> service,
        ILocalizationManager? localization,
        ILogger<LessonAttachmentController> logger,
        IExtendedBaseService extendedBaseService,
        IAttachmentService attachmentService,
        IExportService exportService) : BaseController<LessonAttachment, LessonAttachmentCreateDto, LessonAttachmentUpdateDto, LessonAttachmentDetailsDto, LessonAttachmentExportDto>(service, localization, logger, exportService)
    {
        private readonly IExtendedBaseService _extendedBaseService = extendedBaseService;
        private readonly ILogger<LessonAttachmentController> _logger = logger;
        private readonly IAttachmentService _attachmentService = attachmentService;

        /// <summary>
        /// Creates a new LessonAttachment associated with a specific lesson.
        /// </summary>
        /// <param name="createDto">The data transfer object containing LessonAttachment creation details.</param>
        /// <returns>The created <see cref="LessonAttachmentDetailsDto"/> entity.</returns>
        [HttpPost("create")]
        [SwaggerOperation(Tags = new[] { "Lesson" })]
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
        /// Increments the open/download counter for a specific attachment.
        /// </summary>
        /// <param name="attachmentId">The ID of the attachment to increment counter for.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the operation result.</returns>
        [HttpPost("increment-open/{attachmentId:int}")]
        [SwaggerOperation(Tags = new[] { "Lesson" })]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonAttachmentPermissions.OpenCounter)]
        public async Task<IActionResult> IncrementOpenCount([FromRoute] int attachmentId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var result = await _attachmentService.IncrementAttachmentOpenCountAsync(attachmentId);
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
        /// Updates an existing LessonAttachment.
        /// </summary>
        /// <param name="id">The ID of the attachment to update.</param>
        /// <param name="updateDto">The updated LessonAttachment data.</param>
        /// <returns>The updated <see cref="LessonAttachmentDetailsDto"/> entity.</returns>
        [HttpPatch("edit/{id}")]
        [SwaggerOperation(Tags = new[] { "Lesson" })]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonAttachmentPermissions.Edit)]
        public override async Task<ActionResult<LessonAttachmentDetailsDto>> Patch([FromRoute] int id, [FromBody] LessonAttachmentUpdateDto updateDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var isReferencedValid = await _extendedBaseService.IsEntityExistsAndNotDeletedAsync<Lesson>(updateDto.LessonId ?? 0);
                if (isReferencedValid.IsSuccess == false)
                {
                    return BadRequest(isReferencedValid);
                }

                return await base.Patch(id, updateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update a Lesson Attachment");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to update a Lesson Attachment" });
            }
        }

        /// <summary>
        /// Retrieves all LessonAttachments.
        /// </summary>
        /// <returns>A list of all <see cref="LessonAttachmentDetailsDto"/> entities.</returns>
        [HttpGet("all")]
        [SwaggerOperation(Tags = new[] { "Lesson" })]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonAttachmentPermissions.GetAll)]
        public override async Task<ActionResult<GeneralResult<PaginatedResult<LessonAttachmentDetailsDto>>>> GetAll([FromQuery] PaginationParams pagination)
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
                _logger.LogError(ex, "Failed to get all Lesson Attachments");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to get all Lesson Attachments" });
            }
        }

        /// <summary>
        /// Retrieves a specific LessonAttachment by its ID.
        /// </summary>
        /// <param name="id">The ID of the attachment to retrieve.</param>
        /// <returns>The requested <see cref="LessonAttachmentDetailsDto"/> entity.</returns>
        [HttpGet("get-one/{id}")]
        [SwaggerOperation(Tags = new[] { "Lesson" })]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonAttachmentPermissions.GetOne)]
        public override async Task<ActionResult<LessonAttachmentDetailsDto>> GetOne([FromRoute] int id)
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

        ///// <summary>
        ///// Exports all LessonAttachments to a CSV file.
        ///// </summary>
        ///// <returns>A CSV formatted file containing LessonAttachments data.</returns>
        //[HttpGet("export/csv")]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[RequiredPermission(LessonAttachmentPermissions.Export)]
        //public override async Task<IActionResult> ExportToCsv()
        //{
        //    try
        //    {
        //        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //        if (string.IsNullOrEmpty(userId))
        //        {
        //            return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
        //        }

        //        return await base.ExportToCsv();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Failed to export all Lesson Attachments");
        //        return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to export all Lesson Attachments" });
        //    }
        //}

        ///// <summary>
        ///// Exports all LessonAttachments to an Excel file.
        ///// </summary>
        ///// <returns>An Excel formatted file containing LessonAttachments data.</returns>
        //[HttpGet("export/excel")]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[RequiredPermission(LessonAttachmentPermissions.Export)]
        //public override async Task<IActionResult> ExportToExcel()
        //{
        //    try
        //    {
        //        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //        if (string.IsNullOrEmpty(userId))
        //        {
        //            return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
        //        }

        //        return await base.ExportToExcel();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Failed to export all Lesson Attachments");
        //        return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to export all Lesson Attachments" });
        //    }
        //}

        ///// <summary>
        ///// Exports all LessonAttachments to a JSON file.
        ///// </summary>
        ///// <returns>A JSON formatted file containing LessonAttachments data.</returns>
        //[HttpGet("export/json")]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[RequiredPermission(LessonAttachmentPermissions.Export)]
        //public override async Task<IActionResult> ExportToJson()
        //{
        //    try
        //    {
        //        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //        if (string.IsNullOrEmpty(userId))
        //        {
        //            return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
        //        }

        //        return await base.ExportToJson();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Failed to export all Lesson Attachments");
        //        return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to export all Lesson Attachments" });
        //    }
        //}

        /// <summary>
        /// Retrieves statistics for a specific LessonAttachment, such as view/download counts.
        /// </summary>
        /// <param name="attachmentId">The ID of the attachment to get statistics for.</param>
        /// <returns>Attachment statistics including counts and related data.</returns>
        [HttpGet("status/{attachmentId:int}")]
        [SwaggerOperation(Tags = new[] { "Lesson" })]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonAttachmentPermissions.GetStats)]
        public async Task<IActionResult> GetAttachmentStatus([FromRoute] int attachmentId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var stats = await _attachmentService.GetAttachmentStatsAsync(attachmentId);
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
        /// Deletes a LessonAttachment by its ID.
        /// </summary>
        /// <param name="id">The ID of the attachment to delete.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the outcome of the delete operation.</returns>
        [HttpDelete("delete/{id}")]
        [SwaggerOperation(Tags = new[] { "Lesson" })]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonAttachmentPermissions.Delete)]
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
                _logger.LogError(ex, "Failed to delete LessonAttachment");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to delete LessonAttachment" });
            }
        }
    }
}
