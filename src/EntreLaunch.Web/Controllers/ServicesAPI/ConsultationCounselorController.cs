using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.ConsultationDtos;

namespace EntreLaunch.Web.Controllers.ServicesAPI
{
    [Authorize(Roles = AppRoles.ConsultationRoles)]
    [Route("api/[controller]")]
    public class ConsultationCounselorController(ILocalizationManager localization,
        ILogger<ConsultationCounselorController> logger,
        ICounselorService counselorService,
        IImportService<ConsultationTime, ConsultationTimeImportDto> importService) : AuthenticatedController(localization)
    {
        private readonly ILogger<ConsultationCounselorController> _logger = logger;
        private readonly ICounselorService _counselorService = counselorService;
        private readonly IImportService<ConsultationTime, ConsultationTimeImportDto> _importService = importService;

        /// <summary>
        /// Submits a request to register a new counselor into the system.
        /// </summary>
        /// <param name="dto">
        /// DTO containing the information needed for the counselor registration request.
        /// </param>
        /// <returns>
        /// Returns 201 Created if the request is submitted successfully;
        /// 400 Bad Request for missing or invalid data;
        /// 404 Not Found if a required entity is missing;
        /// 500 Internal Server Error for unexpected failures;
        /// 401 Unauthorized if the user is not authenticated.
        /// </returns>
        [HttpPost("submit-application")]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ConsultationPermissions.SendCounselorRequest)]
        public async Task<IActionResult> SubmitCounselorApplication([FromBody] CreateCounselorRequestDto dto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                dto.UserId = CurrentUserId!;
                var result = await _counselorService.SubmitCounselorApplication(dto);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in SendCounselorRequest.");
                return this.UnexpectedError("send counselor request");
            }
        }

        /// <summary>
        /// Processes an existing counselor request by either accepting or rejecting it.
        /// </summary>
        /// <param name="dto">
        /// DTO containing the request ID and the action to apply (accept/reject).
        /// </param>
        /// <returns>
        /// Returns 200 OK if the action is completed successfully;
        /// 400 Bad Request for invalid inputs;
        /// 404 Not Found if the request does not exist;
        /// 500 Internal Server Error if an unexpected error occurs;
        /// 401 Unauthorized if the user is not authenticated.
        /// </returns>
        [HttpPost("update-application-status")]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ConsultationPermissions.ProcessCounselorRequest)]
        public async Task<IActionResult> UpdateCounselorApplicationStatus([FromBody] ProcessCounselorRequestDto dto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _counselorService.UpdateCounselorApplicationStatus(dto);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in ProcessCounselorRequest.");
                return this.UnexpectedError("process counselor request");
            }
        }

        /// <summary>
        /// Retrieves all counselor registration requests submitted to the system.
        /// </summary>
        /// <returns>
        /// Returns 200 OK with a list of counselor requests;
        /// 401 Unauthorized if the user is not authenticated;
        /// 500 Internal Server Error for unexpected issues.
        /// </returns>
        [HttpGet("all-applications")]
        [ProducesResponseType(typeof(PaginatedResult<CounselorRequestDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ConsultationPermissions.GetAllCounselorRequests)]
        public async Task<IActionResult> GetAllCounselorApplications([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var counselors = await _counselorService.GetAllCounselorApplications(pagination, cancellationToken);
                if (!counselors.IsSuccess)
                {
                    return BadRequest(counselors);
                }

                return Ok(counselors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetAllCounselorRequests.");
                return this.UnexpectedError("get all counselor requests");
            }
        }

        /// <summary>
        /// Retrieves all pending counselor registration requests awaiting approval.
        /// </summary>
        /// <returns>
        /// Returns 200 OK with a list of pending requests;
        /// 401 Unauthorized if the user is not authenticated;
        /// 500 Internal Server Error on failure.
        /// </returns>
        [HttpGet("get-applications-by-status")]
        [ProducesResponseType(typeof(PaginatedResult<CounselorRequestDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ConsultationPermissions.GetPendingCounselorRequests)]
        public async Task<IActionResult> GetCounselorRequestsBasedOnStatus([FromQuery] CounselorRequesttStatus status, [FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var counselors = await _counselorService.GetCounselorRequestsBasedOnStatus(status, pagination, cancellationToken);
                if (!counselors.IsSuccess)
                {
                    return BadRequest(counselors);
                }

                return Ok(counselors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetPendingCounselorRequests.");
                return this.UnexpectedError("get counselor requests by status");
            }
        }

        /// <summary>
        /// Retrieves a list of all counselors currently active in the system.
        /// Requires the user to have the 'ShowAll' permission.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> containing:
        /// - 200 OK with a <see cref="List{CounselorRequestDetailsDto}"/> if the retrieval is successful.
        /// - 400 Bad Request if the result is not successful due to service-level failure.
        /// - 401 Unauthorized if the user is not authenticated or lacks the necessary permissions.
        /// - 500 Internal Server Error if an unexpected error occurs during processing.
        /// </returns>
        [HttpGet("active-counselors")]
        [ProducesResponseType(typeof(PaginatedResult<CounselorRequestDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ConsultationPermissions.GetAllActiveCounselors)]
        public async Task<IActionResult> GetAllActiveCounselors([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _counselorService.GetAllActiveCounselors(pagination, cancellationToken);
                if (!result.IsSuccess)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetAllActiveCounselors.");
                return this.UnexpectedError("get all active counselors");
            }
        }

        /// <summary>
        /// Retrieves all counselor registration requests filtered by specialization.
        /// </summary>
        [HttpGet("by-specialization")]
        [ProducesResponseType(typeof(PaginatedResult<CounselorRequestDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ConsultationPermissions.GetCounselorBySpecialization)]
        public async Task<IActionResult> GetCounselorsBySpecialization([FromQuery] string specialization, [FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _counselorService.GetCounselorsBySpecialization(specialization, pagination, cancellationToken);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in CounselorBySpecialization.");
                return this.UnexpectedError("counselor by specialization");
            }
        }

        /// <summary>
        /// Retrieves the CV and detailed profile of a specific counselor.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the counselor.
        /// </param>
        /// <returns>
        /// Returns 200 OK with counselor CV details;
        /// 401 Unauthorized if the user is not authenticated;
        /// 500 Internal Server Error on failure.
        /// </returns>
        [HttpGet("cv/{id}")]
        [ProducesResponseType(typeof(List<CounselorRequestDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ConsultationPermissions.GetCounselorCV)]
        public async Task<IActionResult> GetCounselorProfileById([FromRoute] int id)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var counselors = await _counselorService.GetCounselorProfileById(id);
                if (counselors.IsSuccess == false)
                {
                    return BadRequest(counselors);
                }

                return Ok(counselors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in CounselorCV.");
                return this.UnexpectedError("counselor cv");
            }
        }

        /// <summary>
        /// Imports multiple counselor consultation times from a provided list.
        /// </summary>
        /// <param name="importRecords">
        /// List of consultation time DTOs to be imported. Each must have a unique ID.
        /// </param>
        /// <returns>
        /// Returns 200 OK if all records are successfully imported;
        /// 422 Unprocessable Entity if any records fail validation;
        /// 500 Internal Server Error if an error occurs;
        /// 401 Unauthorized if the user is not authenticated.
        /// </returns>
        [HttpPost("counselor/import-time")]
        [RequestSizeLimit(100 * 1024 * 1024)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ConsultationPermissions.ImportCounselorTime)]
        public async Task<IActionResult> Import([FromBody] List<ConsultationTimeImportDto> importRecords)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

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
                return this.UnexpectedError("import counselor time");
            }
        }

        /// <summary>
        /// Retrieves a list of all available counselor specializations in the system.
        /// Requires the user to have the 'ShowAll' permission.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> that contains:
        /// - 200 OK with a <see cref="GeneralResult{T}"/> of type <see cref="List{String}"/> if the operation is successful.
        /// - 400 Bad Request if the result indicates failure.
        /// - 401 Unauthorized if the user is not authenticated or lacks permissions.
        /// - 500 Internal Server Error if an unexpected error occurs during processing.
        /// </returns>
        [HttpGet("specializations")]
        [ProducesResponseType(typeof(GeneralResult<List<string>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCounselorSpecializations()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _counselorService.GetAllCounselorSpecializations();
                if (!result.IsSuccess)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetAllCounselorSpecializations.");
                return this.UnexpectedError("get all counselor specializations");
            }
        }

        /// <summary>
        /// Creates a new available consultation time slot for a specified counselor.
        /// </summary>
        /// <param name="createDto">
        /// DTO containing time slot details including counselor ID and time range.
        /// </param>
        /// <returns>
        /// Returns 200 OK if creation succeeds;
        /// 400 Bad Request for validation errors;
        /// 409 Conflict if the time slot overlaps or already exists;
        /// 422 Unprocessable Entity for logical issues;
        /// 500 Internal Server Error if an error occurs during processing;
        /// 401 Unauthorized if the user is not authenticated.
        /// </returns>
        [HttpPost("time-slot")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(ConsultationPermissions.CreateCounselorTime)]
        public async Task<IActionResult> CreateAvailableTimeSlot([FromBody] ConsultationTimeCreateDto createDto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var isValidCounselor = await _counselorService.IsCounselor(createDto.CounselorId);
                if (isValidCounselor.IsSuccess == false)
                {
                    return BadRequest(isValidCounselor);
                }

                var consultationTimeRequest = await _counselorService.CreateAvailableTimeSlot(createDto);
                if (consultationTimeRequest.IsSuccess == false)
                {
                    return BadRequest(consultationTimeRequest);
                }

                return Ok(consultationTimeRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in CreateCounselorTime.");
                return this.UnexpectedError("create counselor time");
            }
        }

        /// <summary>
        /// Updates an existing counselor's consultation time slot.
        /// </summary>
        /// <param name="id">
        /// The identifier of the consultation time to be updated.
        /// </param>
        /// <param name="updateDto">
        /// DTO containing updated consultation time details including counselor ID and timing.
        /// </param>
        /// <returns>
        /// Returns 200 OK if the update succeeds;
        /// 400 Bad Request for validation or logic errors;
        /// 404 Not Found if the consultation time does not exist;
        /// 422 Unprocessable Entity if update conditions fail;
        /// 500 Internal Server Error for unexpected errors;
        /// 401 Unauthorized if the user is not authenticated.
        /// </returns>
        [HttpPatch("update-time/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(ConsultationPermissions.EditCounselorTime)]
        public async Task<IActionResult> UpdateAvailableTimeSlot([FromRoute] int id, [FromBody] ConsultationTimeUpdateDto updateDto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var isValidCounselor = await _counselorService.IsCounselor(updateDto.CounselorId);
                if (isValidCounselor.IsSuccess == false)
                {
                    return BadRequest(isValidCounselor);
                }

                var consultationTimeRequest = await _counselorService.UpdateAvailableTimeSlot(id, updateDto);
                if (consultationTimeRequest.IsSuccess == false)
                {
                    return BadRequest(consultationTimeRequest);
                }

                return Ok(consultationTimeRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in EditCounselorTimes.");
                return this.UnexpectedError("edit counselor times");
            }
        }

        /// <summary>
        /// Retrieves all available consultation time slots for a specific counselor.
        /// </summary>
        [HttpGet("all-time/{counselorId}")]
        [ProducesResponseType(typeof(PaginatedResult<ConsultationTimeDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(ConsultationPermissions.GetAllCounselorTimes)]
        public async Task<IActionResult> GetAvailableTimeSlotsByCounselor([FromRoute] int counselorId, [FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var isValidCounselor = await _counselorService.IsCounselor(counselorId);
                if (!isValidCounselor.IsSuccess)
                    return BadRequest(isValidCounselor);

                var result = await _counselorService.GetAvailableTimeSlotsByCounselor(counselorId, pagination, cancellationToken);
                if (!result.IsSuccess)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetAllCounselorTimes.");
                return this.UnexpectedError("get all counselor times");
            }
        }

        /// <summary>
        /// Checks if a user has a pending counselor application.
        /// </summary>
        /// <param name="userId">The user ID to check for pending application.</param>
        /// <returns>
        /// Returns true if a pending application exists, otherwise false;
        /// 400 for invalid input; 500 for unexpected errors; 401 if unauthorized.
        /// </returns>
        [HttpGet("has-pending/{userId}")]
        [ProducesResponseType(typeof(GeneralResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ConsultationPermissions.CheckPendingCounselorRequest)]
        public async Task<IActionResult> HasPendingApplication([FromRoute] string userId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _counselorService.HasPendingApplication(userId);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in HasPendingApplication.");
                return this.UnexpectedError("check pending counselor application");
            }
        }

        /// <summary>
        /// Retrieves a counselor profile by the associated user ID.
        /// </summary>
        /// <param name="userId">The user ID to look up the counselor by.</param>
        /// <returns>
        /// Returns the counselor profile if found;
        /// 400 for invalid user ID; 404 if not found; 500 for internal error.
        /// </returns>
        [HttpGet("by-user/{userId}")]
        [ProducesResponseType(typeof(GeneralResult<CounselorRequestDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(ConsultationPermissions.GetCounselorByUserId)]
        public async Task<IActionResult> GetCounselorByUserId([FromRoute] string userId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _counselorService.GetCounselorByUserId(userId);
                if (!result.IsSuccess)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetCounselorByUserId.");
                return this.UnexpectedError("get counselor by user ID");
            }
        }

        /// <summary>
        /// Retrieves statistical summary for counselors.
        /// </summary>
        /// <returns>
        /// Returns 200 OK with summary data;
        /// 500 Internal Server Error on failure;
        /// 401 Unauthorized if the user is not authenticated.
        /// </returns>
        [HttpGet("summary-stats")]
        [ProducesResponseType(typeof(CounselorSummaryStatsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ConsultationPermissions.GetCounselorSummaryStats)]
        public async Task<IActionResult> GetCounselorSummaryStats()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _counselorService.GetCounselorSummaryStats();
                if (!result.IsSuccess)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetCounselorSummaryStats.");
                return this.UnexpectedError("get counselor summary stats");
            }
        }

        /// <summary>
        /// Retrieves the authenticated consultant's full consultation history, including all relevant consultation data.
        /// This endpoint is intended for use by consultants to view their own consultation records.
        /// </summary>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing:
        /// <list type="bullet">
        /// <item><description><c>400 Bad Request</c> if the operation fails due to business logic errors.</description></item>
        /// <item><description><c>401 Unauthorized</c> if the user is not authenticated or lacks the required permission.</description></item>
        /// <item><description><c>500 Internal Server Error</c> if an unexpected error occurs during processing.</description></item>
        /// </list>
        /// </returns>
        [HttpGet("consultant/my-history")]
        [ProducesResponseType(typeof(PaginatedResult<ConsultationAllData>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ConsultationPermissions.GetConsultantConsultationHistory)]
        public async Task<IActionResult> GetConsultantHistory([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _counselorService.GetConsultationsByCounselorId(CurrentUserId!, pagination, cancellationToken);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetConsultantHistory.");
                return this.UnexpectedError("get consultation history for consultant");
            }
        }
    }
}
