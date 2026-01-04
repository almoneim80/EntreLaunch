namespace EntreLaunch.Web.Controllers.ServicesAPI
{
    [Authorize(Roles = AppRoles.ConsultationRoles)]
    [Route("api/[controller]")]
    public class ConsultationBookingController(
        ILocalizationManager localization,
        ILogger<ConsultationBookingController> logger,
        IExtendedBaseService extendedBaseService,
        IConsultationBookingService consultationService,
        IImportService<ConsultationTime, ConsultationTimeImportDto> importService,
        CascadeDeleteService deleteService) : AuthenticatedController(localization)
    {
        private readonly IExtendedBaseService _extendedBaseService = extendedBaseService;
        private readonly ILogger<ConsultationBookingController> _logger = logger;
        private readonly IConsultationBookingService _consultationService = consultationService;
        private readonly IImportService<ConsultationTime, ConsultationTimeImportDto> _importService = importService;
        private readonly ILocalizationManager _localizationManager = localization;

        /// <summary>
        /// Creates a new online consultation request after validating the user, consultation time, and counselor.
        /// </summary>
        /// <param name="dto">
        /// DTO containing consultation details, including counselor ID and consultation time ID.
        /// </param>
        /// <returns>
        /// Returns 200 OK with consultation result on success;
        /// 400 Bad Request for validation or business logic failures;
        /// 409 Conflict if the consultation conflicts with existing data;
        /// 422 Unprocessable Entity for semantic errors;
        /// 500 Internal Server Error for unexpected failures;
        /// 401 Unauthorized if the user is not authenticated.
        /// </returns>
        [HttpPost("create/online")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(ConsultationPermissions.BookingOnlineConsultation)]
        public async Task<IActionResult> OnlineConsultation([FromBody] OnlineConsultationCreateDto dto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var consultationTimeId = dto.ConsultationTimeId;
                var isTimeValid = await consultationTimeId.CheckIfEntityExistsAsync<ConsultationTime>(_extendedBaseService, _logger, _localizationManager);
                if (isTimeValid != null) return isTimeValid;

                var isCounselorValid = await dto.CounselorId.CheckIfEntityExistsAsync<Counselor>(_extendedBaseService, _logger, _localizationManager);
                if (isCounselorValid != null) return isCounselorValid;

                dto.ClientId = CurrentUserId!;
                dto.Type = ConsultationType.Online;
                var consultationRequest = await _consultationService.BookConsultation(dto);
                if (consultationRequest.IsSuccess == false)
                {
                    return BadRequest(consultationRequest);
                }

                return Ok(consultationRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in OnlineConsultation.");
                return this.UnexpectedError("add online consultation");
            }
        }

        /// <summary>
        /// Submits a new text-based consultation request after validating user and counselor.
        /// </summary>
        /// DTO containing required information for creating the text consultation, such as counselor ID.
        /// <returns>
        /// Returns 200 OK with a success message on success;
        /// 400 Bad Request with details if submission fails due to business logic or validation issues;
        /// 409 Conflict or 422 Unprocessable Entity for domain-specific failures;
        /// 500 Internal Server Error if an unhandled exception occurs;
        /// 401 Unauthorized if the user is not authenticated.
        /// </returns>
        [HttpPost("create/text")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(ConsultationPermissions.SendTextConsultation)]
        public async Task<IActionResult> TextConsultation([FromBody] TextConsultationCreateDto dto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var isCounselorValid = await dto.CounselorId.CheckIfEntityExistsAsync<Counselor>(_extendedBaseService, _logger, _localizationManager);
                if (isCounselorValid != null) return isCounselorValid;

                dto.ClientId = CurrentUserId!;
                dto.Type = ConsultationType.text;
                var result = await _consultationService.SubmitTextConsultation(dto);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in TextConsultation.");
                return this.UnexpectedError("add text consultation");
            }
        }

        /// <summary>
        /// Updates the status of an existing consultation, typically transitioning it from Scheduled to another state.
        /// </summary>
        /// <param name="dto">
        /// DTO containing consultation ID and the new status to apply.
        /// </param>
        /// <returns>
        /// Returns 200 OK if the status update is successful;
        /// 400 Bad Request for invalid input or business rule violations;
        /// 404 Not Found if the consultation does not exist;
        /// 500 Internal Server Error if an unexpected error occurs;
        /// 401 Unauthorized if the user is not authenticated.
        /// </returns>
        [HttpPost("process-status")]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ConsultationPermissions.ProcessConsultationStatus)]
        public async Task<IActionResult> ProcessStatus([FromBody] ProcessConsultationStatusDto dto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _consultationService.UpdateConsultationStatus(dto);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in ProcessStatus.");
                return this.UnexpectedError("process status");
            }
        }

        /// <summary>
        /// Retrieves all consultations filtered by a specified consultation type.
        /// </summary>
        [HttpGet("by-type")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(ConsultationPermissions.GetConsultationByType)]
        public async Task<IActionResult> GetByType([FromQuery] ConsultationType type, [FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _consultationService.GetConsultationsByType(type, pagination, cancellationToken);
                if (!result.IsSuccess)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetByType.");
                return this.UnexpectedError("get consultations by type");
            }
        }

        /// <summary>
        /// Retrieves details of a specific consultation by its ID.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the consultation to fetch.
        /// </param>
        /// <returns>
        /// Returns 200 OK with consultation details;
        /// 400 or 404 if the consultation does not exist;
        /// 500 Internal Server Error on failure;
        /// 401 Unauthorized if user is not logged in.
        /// </returns>
        [HttpGet("get-one/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(ConsultationPermissions.GetOneConsultation)]
        public async Task<IActionResult> GetOne([FromRoute] int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not logged in.");
                }

                var consultationRequest = await _consultationService.GetConsultationById(id);
                if (consultationRequest == null)
                {
                    _logger.LogWarning("No consultation request found.");
                    return BadRequest("No consultation request found.");
                }

                return Ok(consultationRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetOne.");
                return this.UnexpectedError("get one consultation");
            }
        }

        /// <summary>
        /// Retrieves all consultation requests available in the system.
        /// </summary>
        /// <returns>
        /// Returns 200 OK with the list of consultations;
        /// 400 or 404 if data is missing or invalid;
        /// 500 Internal Server Error for any failure;
        /// 401 Unauthorized if the user is not authenticated.
        /// </returns>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(ConsultationPermissions.GetAllConsultation)]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var consultationRequest = await _consultationService.GetAllConsultations(pagination, cancellationToken);
                if (!consultationRequest.IsSuccess)
                {
                    return BadRequest(consultationRequest);
                }

                return Ok(consultationRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetAll.");
                return this.UnexpectedError("get all consultations");
            }
        }

        /// <summary>
        /// Retrieves all possible values of the consultation status enumeration.
        /// </summary>
        /// <returns>
        /// Returns 200 OK with a list of consultation status values;
        /// 401 Unauthorized if the user is not authenticated.
        /// </returns>
        [HttpGet("all-status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(ConsultationPermissions.GetEnumValues)]
        public IActionResult GetAllConsultationStatus()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var enumValues = _extendedBaseService.GetEnumValues<ConsultationStatus>();
                return Ok(new GeneralResult { IsSuccess = true, Message = "Success get all consultation status", Data = enumValues });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetAllStatus.");
                return this.UnexpectedError("get all consultation status");
            }
        }

        /// <summary>
        /// Retrieves the authenticated client's consultation history, including all related consultation data.
        /// </summary>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing:
        /// <list type="bullet">
        /// <item><description><c>400 Bad Request</c> if the consultation retrieval fails due to business logic.</description></item>
        /// <item><description><c>401 Unauthorized</c> if the user is not authenticated.</description></item>
        /// <item><description><c>500 Internal Server Error</c> if an unexpected error occurs during processing.</description></item>
        /// </list>
        /// </returns>
        [HttpGet("client/my-history")]
        [ProducesResponseType(typeof(GeneralResult<List<ConsultationAllData>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ConsultationPermissions.GetClientConsultationHistory)]
        public async Task<IActionResult> GetClientHistory()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _consultationService.GetClientHistory(CurrentUserId!);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetClientHistory.");
                return this.UnexpectedError("get consultation history for client");
            }
        }

        /// <summary>
        /// Retrieves all consultations assigned to a specific counselor.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the counselor.
        /// </param>
        /// <returns>
        /// Returns 200 OK with consultations data;
        /// 400 or 404 if the counselor or consultations are invalid or not found;
        /// 500 Internal Server Error on failure;
        /// 401 Unauthorized if the user is not authenticated.
        /// </returns>
        [HttpGet("get-by-counselor/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(ConsultationPermissions.GetConsultationByCounselor)]
        public async Task<IActionResult> GetConsultationsByCounselorId([FromRoute] int id)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var consultationRequest = await _consultationService.GetConsultationsByCounselorId(id);
                if (consultationRequest.IsSuccess == false)
                {
                    return BadRequest(consultationRequest);
                }

                return Ok(consultationRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetByCounselor.");
                return this.UnexpectedError("get consultations by counselor");
            }
        }

        /// <summary>
        /// Soft deletes a consultation time slot and all its related entities using a cascading delete operation.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the consultation time to delete.
        /// </param>
        /// <returns>
        /// Returns 204 No Content if deletion is successful;
        /// 400 Bad Request for invalid ID;
        /// 404 Not Found if the entity is not found;
        /// 500 Internal Server Error on failure;
        /// 401 Unauthorized if the user is not authenticated.
        /// </returns>
        [HttpDelete("delete-time/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(ConsultationPermissions.Delete)]
        public async Task<IActionResult> DeleteWithCascade([FromRoute] int id)
        {
            var userCheck = CheckUserOrUnauthorized();
            if (userCheck != null) return userCheck;

            if (id <= 0)
            {
                return BadRequest(new GeneralResult { IsSuccess = false, Message = "Invalid ID supplied." });
            }

            var transactionId = Guid.NewGuid();
            _logger.LogInformation("Transaction {TransactionId}: Starting soft delete for entity ID {Id}.", transactionId, id);

            try
            {
                var result = await deleteService.SoftDeleteCascadeAsync<ConsultationTime>(id);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in DeleteWithCascade.");
                return this.UnexpectedError("delete with cascade");
            }
        }
    }
}
