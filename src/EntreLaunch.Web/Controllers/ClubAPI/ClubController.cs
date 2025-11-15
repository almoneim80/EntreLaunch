namespace EntreLaunch.Controllers.ClubAPI
{
    [Authorize(Roles = "Admin , Entrepreneur")]
    [Route("api/[controller]")]
    public class ClubController : BaseController<ClubEvent, ClubEventCreateDto, ClubEventEntreLaunchdateDto, ClubEventDetailsDto, ClubEventExportDto>
    {
        private readonly ILogger<ClubController> _logger;
        private readonly CascadeDeleteService _deleteService;
        private readonly IImportService<ClubEvent, ClubEventImportDto> _importService;
        private readonly IClubService _clubService;
        public ClubController(
            BaseService<ClubEvent, ClubEventCreateDto, ClubEventEntreLaunchdateDto, ClubEventDetailsDto> service,
            ILogger<ClubController> logger,
            CascadeDeleteService deleteService,
            ILocalizationManager? localization,
            IImportService<ClubEvent, ClubEventImportDto> importService,
            IClubService clubService,
            IExportService exportService)
        : base(service, localization, logger, exportService)
        {
            _logger = logger;
            _deleteService = deleteService;
            _importService = importService;
            _clubService = clubService;
        }

        /// <summary>
        /// Creates a new event.
        /// </summary>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(Permissions.ClubPermissions.Create)]
        public override async Task<ActionResult<ClubEventDetailsDto>> Create([FromBody] ClubEventCreateDto createDto)
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
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error accurred while creating event." });
            }
        }

        /// <summary>
        /// Subscribe to club.
        /// </summary>
        [HttpPost("subscribe")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(Permissions.ClubPermissions.SubscribeClub)]
        public async Task<IActionResult> SubscribeClub()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var result = await _clubService.SubscribeToClubAsync(userId);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error accurred while subscribing to club." });
            }
        }

        /// <summary>
        /// Subscribe to event.
        /// </summary>
        [HttpPost("event/subscribe")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(Permissions.ClubPermissions.SubscribeEvent)]
        public async Task<IActionResult> SubscribeEvent([FromBody] ClubSubscribeCreateDto dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                dto.UserId = userId;
                var result = await _clubService.SubscribeToEventAsync(dto);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error accurred while subscribing to event." });
            }
        }

        /// <summary>
        /// Renew club subscription.
        /// </summary>
        [HttpPost("club/renew")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(Permissions.ClubPermissions.RenewClubSubscription)]
        public async Task<IActionResult> RenewClubSubscription()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var result = await _clubService.RenewClubSubscriptionAsync(userId);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error accurred while renewing club subscription." });
            }
        }

        /// <summary>
        /// Imports event data from a list.
        /// (id must be unique.)
        /// </summary>
        [HttpPost("import-list")]
        [RequestSizeLimit(100 * 1024 * 1024)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(Permissions.ClubPermissions.ImportList)]
        public async Task<ActionResult<ImportResult>> Import([FromBody] List<ClubEventImportDto> importRecords)
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
                    return BadRequest(new GeneralResult { IsSuccess = false, Message = "No data provided for import operation." });
                }

                var result = await _importService.ImportFromListAsync(importRecords);
                if(result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error accurred while importing data." });
            }
        }

        /// <summary>
        /// Imports event data from a file (CSV or Excel).
        /// </summary>
        [HttpPost("import-file")]
        [RequestSizeLimit(100 * 1024 * 1024)] // file size = 100 MB
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(Permissions.ClubPermissions.ImportFile)]
        public async Task<ActionResult<ImportResult>> ImportFromFile(IFormFile file)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var result = await _importService.ImportFromFileAsync(file);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error accurred while importing data." });
            }
        }

        /// <summary>
        /// EntreLaunchdates an existing event.
        /// </summary>
        [HttpPatch("edit/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(Permissions.ClubPermissions.Edit)]
        public override async Task<ActionResult<ClubEventDetailsDto>> Patch(int id, [FromBody] ClubEventEntreLaunchdateDto EntreLaunchdateDto)
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
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error accurred while EntreLaunchdating event." });
            }
        }

        /// <summary>
        /// Returns a list of all events.
        /// </summary>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(Permissions.ClubPermissions.ShowAll)]
        public override async Task<ActionResult<ClubEventDetailsDto[]>> GetAll()
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
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error accurred while getting events." });
            }
        }

        /// <summary>
        /// Returns a single event.
        /// </summary>
        [HttpGet("get-one/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(Permissions.ClubPermissions.ShowOne)]
        public override async Task<ActionResult<ClubEventDetailsDto>> GetOne(int id)
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
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error accurred while getting one event." });
            }
        }

        /// <summary>
        /// Generates a CSV template.
        /// </summary>
        [HttpGet("generate-csv")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(Permissions.ClubPermissions.GenerateTemplate)]
        public async Task<IActionResult> GenerateCsvTemplate()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var template = await _importService.GenerateCsvTemplateAsync<ClubEventImportDto>();
                if(template.IsSuccess == false || template.Data == null)
                {
                    return BadRequest(template);
                }

                return File(template.Data, "text/csv", "ClubEvent_template.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error accurred while generating CSV template." });
            }
        }

        /// <summary>
        /// Generates an Excel template.
        /// </summary>
        [HttpGet("generate-excel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(Permissions.ClubPermissions.GenerateTemplate)]
        public async Task<IActionResult> GenerateExcelTemplate()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var template = await _importService.GenerateExcelTemplateAsync<ClubEventImportDto>();
                if(template.IsSuccess == false || template.Data == null)
                {
                    return BadRequest(template);
                }

                return File(template.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ClubEvent_template.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error accurred while generating Excel template." });
            }
        }

        /// <summary>
        /// Get all subscribers of an event.
        /// </summary>
        [HttpGet("subscribers/{eventId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(Permissions.ClubPermissions.GetEventSubscriber)]
        public async Task<IActionResult> GetEventSubscribers([FromRoute] int eventId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var result = await _clubService.GetEventSubscribersAsync(eventId);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error accurred while getting event subscribers." });
            }
        }

        /// <summary>
        /// Get all events that a user has subscribed to.
        /// </summary>
        [HttpGet("user-subscriptions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(Permissions.ClubPermissions.GetUserSubscription)]
        public async Task<IActionResult> GetUserSubscriptions()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var result = await _clubService.GetUserSubscriptionsAsync(userId);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error accurred while getting user subscriptions." });
            }
        }

        /// <summary>
        /// deletes an event.
        /// </summary>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(Permissions.ClubPermissions.SoftDelete)]
        public async Task<IActionResult> SoftDelete(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                if (id <= 0)
                {
                    return BadRequest(new GeneralResult { IsSuccess = false, Message = "Invalid ID provided." });
                }

                var transactionId = Guid.NewGuid();
                _logger.LogInformation("Transaction {TransactionId}: Starting soft delete for entity ID {Id}.", transactionId, id);

                try
                {
                    var result = await _deleteService.SoftDeleteCascadeAsync<ClubEvent>(id);
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
                    _logger.LogError(ex, "Transaction {TransactionId}: Unexpected error occurred while deleting entity ID {Id}.", transactionId, id);
                    return StatusCode(StatusCodes.Status500InternalServerError, new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "An error occurred while deleting the entity. Please try again later.",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while deleting entity ID {Id}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An error occurred while deleting the entity. Please try again later.",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Unsubscribe from event.
        /// </summary>
        [HttpDelete("unsubscribe/{subscriptionId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(Permissions.ClubPermissions.UnsubscribeEvent)]
        public async Task<IActionResult> UnsubscribeFromEvent([FromRoute] int subscriptionId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var result = await _clubService.UnsubscribeFromEventAsync(subscriptionId, userId);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while unsubscribing from event.");
                return StatusCode(StatusCodes.Status500InternalServerError, new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An error occurred while unsubscribing from event. Please try again later.",
                    Data = null
                });
            }
        }

        #region deleted endpoints
        [NonAction]
        public override Task<IActionResult> ExportToCsv()
        {
            return Task.FromResult<IActionResult>(NotFound());
        }

        [NonAction]
        public override Task<IActionResult> ExportToExcel()
        {
            return Task.FromResult<IActionResult>(NotFound());
        }

        [NonAction]
        public override Task<IActionResult> ExportToJson()
        {
            return Task.FromResult<IActionResult>(NotFound());
        }

        [NonAction]
        public override Task<ActionResult> Delete(int id)
        {
            // Return a default value
            return Task.FromResult<ActionResult>(NotFound());
        }
        #endregion
    }
}
