namespace EntreLaunch.Web.Controllers.LaunchingProjectAPI
{
    [Authorize(Roles = AppRoles.OpportunityRoles)]
    [Route("api/[controller]")]
    public class OpportunityController(
        BaseService<Opportunity, OpportunityCreateDto, OpportunityUpdateDto, OpportunityDetailsDto> service,
        ILocalizationManager? localization,
        ILogger<OpportunityController> logger,
        IImportService<Opportunity, OpportunityImportDto> importService,
        IExportService exportService) : BaseController<Opportunity, OpportunityCreateDto, OpportunityUpdateDto, OpportunityDetailsDto, OpportunityExportDto>(service, localization, logger, exportService)
    {
        private readonly IImportService<Opportunity, OpportunityImportDto> _importService = importService;
        private readonly ILogger<OpportunityController> _logger = logger;

        /// <summary>
        /// Create new Opportunity.
        /// </summary>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(OpportunityPermissions.Create)]
        public override async Task<ActionResult<OpportunityDetailsDto>> Create([FromBody] OpportunityCreateDto createDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                return await base.Create(createDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in CreateOpportunity.");
                return this.UnexpectedError("creating opportunity");
            }
        }

        /// <summary>
        /// Imports data from a list.
        /// (id must be unique.)
        /// </summary>
        [HttpPost("import")]
        [RequestSizeLimit(100 * 1024 * 1024)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(OpportunityPermissions.Import)]
        public async Task<ActionResult<ImportResult>> Import([FromBody] List<OpportunityImportDto> importRecords)
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
                _logger.LogError(ex, "An error occurred in ImportOpportunity.");
                return this.UnexpectedError("importing opportunity");
            }
        }

        /// <summary>
        /// Update one Opportunity.
        /// </summary>
        [HttpPatch("edit/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(OpportunityPermissions.Edit)]
        public override async Task<ActionResult<OpportunityDetailsDto>> Patch([FromRoute] int id, [FromBody] OpportunityUpdateDto updateDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                return await base.Patch(id, updateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in UpdateOpportunity.");
                return this.UnexpectedError("updating opportunity");
            }
        }

        /// <summary>
        /// Get all Opportunities.
        /// </summary>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(OpportunityPermissions.GetAll)]
        public override async Task<ActionResult<GeneralResult<PaginatedResult<OpportunityDetailsDto>>>> GetAll([FromQuery] PaginationParams pagination)
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
                _logger.LogError(ex, "An error occurred in GetAllOpportunities.");
                return this.UnexpectedError("getting opportunities");
            }
        }

        /// <summary>
        /// Get one Opportunity.
        /// </summary>
        [HttpGet("get-one/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(OpportunityPermissions.GetOne)]
        public override async Task<ActionResult<OpportunityDetailsDto>> GetOne(int id)
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
                _logger.LogError(ex, "An error occurred in GetOpportunity.");
                return this.UnexpectedError("getting opportunity");
            }
        }

        /// <summary>
        /// Delete an existing Opportunity.
        /// </summary>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(OpportunityPermissions.Delete)]
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
                _logger.LogError(ex, "An error occurred in DeleteOpportunity.");
                return this.UnexpectedError("deleting opportunity");
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
