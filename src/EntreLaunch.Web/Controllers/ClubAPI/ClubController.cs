namespace EntreLaunch.Controllers.ClubAPI
{
    [Authorize(Roles = AppRoles.ClubRoles)]
    [ApiController]
    [Route("api/[controller]")]
    public class ClubController(
        ILogger<ClubController> logger,
        CascadeDeleteService deleteService,
        ILocalizationManager localization,
        IImportService<ClubEvent, ClubEventImportDto> importService,
        IClubService clubService) : AuthenticatedController(localization)
    {
        private readonly ILogger<ClubController> _logger = logger;
        private readonly CascadeDeleteService _deleteService = deleteService;
        private readonly IImportService<ClubEvent, ClubEventImportDto> _importService = importService;

        /// <summary>
        /// Creates a new club event.
        /// Requires the user to have permission to create club events.
        /// </summary>
        /// <param name="createDto">
        /// An object of type <see cref="ClubEventCreateDto"/> containing the event details.
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing the created event details.
        /// </returns>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(ClubPermissions.Create)]
        public async Task<IActionResult> CreateEvent([FromBody] ClubEventCreateDto createDto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await clubService.AddClubEventAsync(createDto);
                if (result.IsSuccess == false)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.UnexpectedError("creating event");
            }
        }

        /// <summary>
        /// Registers the authenticated user to a specific club event.
        /// Validates the provided registration data.
        /// Requires the user to have permission to register to events.
        /// </summary>
        /// <param name="dto">
        /// An object of type <see cref="ClubEventRegistrationCreateDto"/> containing registration details.
        /// </param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the registration operation.
        /// </returns>
        [HttpPost("event/register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(ClubPermissions.EventRegister)]
        public async Task<IActionResult> SubscribeEvent([FromBody] ClubEventRegistrationCreateDto dto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                dto.UserId = CurrentUserId!;
                var result = await clubService.RegisterToEventAsync(dto);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.UnexpectedError("registering to event.");
            }
        }

        ///// <summary>
        ///// Imports multiple club events from a provided list.
        ///// Requires the user to have permission to import event lists.
        ///// </summary>
        ///// <param name="importRecords">
        ///// A list of <see cref="ClubEventImportDto"/> containing the event data to import.
        ///// </param>
        ///// <returns>
        ///// An <see cref="ActionResult{T}"/> summarizing the import result.
        ///// </returns>
        //[HttpPost("import-list")]
        //[RequestSizeLimit(100 * 1024 * 1024)]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        //[RequiredPermission(ClubPermissions.ImportList)]
        //public async Task<IActionResult> Import([FromBody] List<ClubEventImportDto> importRecords)
        //{
        //    try
        //    {
        //        var userCheck = CheckUserOrUnauthorized();
        //        if (userCheck != null) return userCheck;

        //        if (importRecords == null || importRecords.Count == 0)
        //        {
        //            _logger.LogWarning("No data provided for import operation.");
        //            return BadRequest(new GeneralResult { IsSuccess = false, Message = "No data provided for import operation." });
        //        }

        //        foreach(var item in importRecords)
        //        {
        //            item.StartDate = item.StartDate.ToUniversalTime();
        //            item.EndDate = item.EndDate.ToUniversalTime();
        //        }

        //        var result = await _importService.ImportFromListAsync(importRecords);
        //        if(result.IsSuccess == false)
        //        {
        //            return BadRequest(result);
        //        }

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, ex.Message);
        //        return this.UnexpectedError("importing data.");
        //    }
        //}

        /// <summary>
        /// Updates an existing club event.
        /// Requires the user to have permission to edit club events.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the event to update.
        /// </param>
        /// <param name="updateDto">
        /// An object of type <see cref="ClubEventUpdateDto"/> containing updated event details.
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing the updated event details.
        /// </returns>
        [HttpPatch("edit/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(ClubPermissions.Edit)]
        public async Task<IActionResult> EditEvent([FromRoute] int id, [FromBody] ClubEventUpdateDto updateDto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await clubService.UpdateClubEventAsync(id, updateDto);
                if (result.IsSuccess == false)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.UnexpectedError("updating event.");
            }
        }

        /// <summary>
        /// Retrieves all club events in the system.
        /// Requires the user to have permission to view all club events.
        /// </summary>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing an array of <see cref="ClubEventDetails"/>.
        /// </returns>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(ClubPermissions.GetAll)]
        public async Task<IActionResult> AllEvents([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await clubService.AllEventsAsync(pagination, cancellationToken);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.UnexpectedError("getting events.");
            }
        }

        /// <summary>
        /// Retrieves the details of a specific club event by its ID.
        /// Requires the user to have permission to view a specific event.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the event to retrieve.
        /// </param>
        /// <param name="cancellationToken"> an instance of <see cref="CancellationToken"/>.</param>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing the event details.
        /// </returns>
        [HttpGet("get-one/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(ClubPermissions.GetOne)]
        public async Task<IActionResult> OneEvent([FromRoute] int id, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await clubService.OneEventAsync(id, cancellationToken);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.UnexpectedError("getting one event.");
            }
        }

        /// <summary>
        /// Retrieves the list of registrants for a specific event.
        /// Requires the user to have permission to view event registrations.
        /// </summary>
        [HttpGet("event/registrants/{eventId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(ClubPermissions.GetEventSubscriber)]
        public async Task<IActionResult> GetEventRegistrants([FromRoute] int eventId, [FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await clubService.GetEventRegistrationsAsync(eventId, pagination, cancellationToken);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.UnexpectedError("getting event registrants.");
            }
        }

        /// <summary>
        /// Retrieves all events that the authenticated user has registered to.
        /// Requires the user to have permission to view event registrations.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the user's event registrations.
        /// </returns>
        [HttpGet("user-registrations")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(ClubPermissions.UserEventRegistrations)]
        public async Task<IActionResult> GetUserEventRegistrations()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await clubService.GetUserEventRegistrationsAsync(CurrentUserId!);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.UnexpectedError("getting user event registrations.");
            }
        }

        /// <summary>
        /// Performs a soft delete on a specified club event and its related entities.
        /// Requires the user to have permission for soft deletion.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the event to delete.
        /// </param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the delete operation.
        /// </returns>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(ClubPermissions.SoftDelete)]
        public async Task<IActionResult> SoftDelete([FromRoute] int id)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

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
                return this.UnexpectedError("deleting the entity. Please try again later.");
            }
        }

        /// <summary>
        /// Cancels the authenticated user's registration to a specific club event.
        /// Requires the user to have permission to unregister from events.
        /// </summary>
        /// <param name="registrationId">
        /// The unique identifier of the event registration to cancel.
        /// </param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the cancellation.
        /// </returns>
        [HttpDelete("event/unregister/{registrationId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(ClubPermissions.UnregisterFromEvent)]
        public async Task<IActionResult> UnregisterFromEvent([FromRoute] int registrationId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await clubService.CancelEventRegistrationAsync(registrationId, CurrentUserId!);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while unregistering from event.");
                return this.UnexpectedError("unregistering from event. Please try again later.");
            }
        }

        /// <summary>
        /// Checks if a user can register to an event.
        /// </summary>
        [HttpGet("can-register/")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(GeneralResult<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CanRegister([FromQuery] int eventId)
        {
            var userCheck = CheckUserOrUnauthorized();
            if (userCheck != null) return userCheck;

            var result = await clubService.CanRegisterToEventAsync(eventId, CurrentUserId!);
            return Ok(result);
        }
    }
}
