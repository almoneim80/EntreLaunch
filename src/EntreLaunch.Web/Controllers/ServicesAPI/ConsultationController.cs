namespace EntreLaunch.Controllers.ServicesAPI
{
    [Authorize(Roles = "Admin, Counselor, Entrepreneur")]
    [Route("api/[controller]")]
    public class ConsultationController : AuthenticatedController
    {
        private readonly IExtendedBaseService _extendedBaseService;
        private readonly ILogger<ConsultationController> _logger;
        private readonly IConsultation _consultationService;
        private readonly IImportService<ConsultationTime, ConsultationTimeImportDto> _importService;
        private readonly CascadeDeleteService _deleteService;

        public ConsultationController(
            BaseService<Counselor, ConsultationCreateDto, ConsultationEntreLaunchdateDto, ConsultationDetailsDto> service,
            ILocalizationManager? localization,
            ILogger<ConsultationController> logger,
            IExtendedBaseService extendedBaseService,
            IConsultation consultationService,
            IImportService<ConsultationTime, ConsultationTimeImportDto> importService,
            CascadeDeleteService deleteService)
        {
            _extendedBaseService = extendedBaseService;
            _logger = logger;
            _consultationService = consultationService;
            _importService = importService;
            _deleteService = deleteService;
        }

        /// <summary>
        /// Create new consultation.
        /// </summary>
        [HttpPost("online")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(Permissions.ConsultationPermissions.BookingConsultation)]
        public async Task<IActionResult> OnlineConsultation([FromBody] ConsultationCreateDto createDto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var consultationTimeId = createDto.ConsultationTimeId ?? 0;
                var isTimeValid = await consultationTimeId.CheckIfEntityExistsAsync<ConsultationTime>(_extendedBaseService, _logger);
                if (isTimeValid != null) return isTimeValid;

                var isCounselorValid = await createDto.CounselorId.CheckIfEntityExistsAsync<Counselor>(_extendedBaseService, _logger);
                if (isCounselorValid != null) return isCounselorValid;

                var consultationRequest = await _consultationService.BookingConsultation(createDto);
                if (consultationRequest.IsSuccess == false)
                {
                    return BadRequest(consultationRequest);
                }

                return Ok(consultationRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in OnlineConsultation.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while add online consultation", Data = null });
            }
        }

        /// <summary>
        /// send text consultation.
        /// </summary>
        [HttpPost("text")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(Permissions.ConsultationPermissions.TextConsultation)]
        public async Task<IActionResult> TextConsultation([FromBody] ConsultationCreateDto createDto)
        {
            var userCheck = CheckUserOrUnauthorized();
            if (userCheck != null) return userCheck;

            var isCounselorValid = await createDto.CounselorId.CheckIfEntityExistsAsync<Counselor>(_extendedBaseService, _logger);
            if (isCounselorValid != null) return isCounselorValid;

            var consultationRequest = await _consultationService.SendTextConsultation(createDto);
            if (consultationRequest.IsSuccess == false)
            {
                _logger.LogError(consultationRequest.Message);
                return BadRequest(new ProblemDetails
                {
                    Title = "Error in consultation requst.",
                    Detail = consultationRequest.Message
                });
            }

            return Ok(consultationRequest.Message);
        }

        /// <summary>
        /// Modify the consultation status (from Scheduled to Other).
        /// </summary>
        [HttpPost("process-status")]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(Permissions.ConsultationPermissions.ProgressStatus)]
        public async Task<IActionResult> ProcessStatus([FromBody] ProcessConsultationStatusDto processConsultationStatus)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _consultationService.ProgressConsultationStatus(processConsultationStatus);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in ProcessStatus.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while process status", Data = null });
            }
        }

        /// <summary>
        /// Creates a new consultation time.
        /// </summary>
        [HttpPost("counselor/new-time")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(Permissions.ConsultationTimePermissions.Create)]
        public async Task<IActionResult> CreateCounselorTime([FromBody] ConsultationTimeCreateDto createDto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var isValidCounselor = await _consultationService.IsCounselor(createDto.CounselorId);
                if (isValidCounselor.IsSuccess == false)
                {
                    return BadRequest(isValidCounselor);
                }

                var consultationTimeRequest = await _consultationService.CreateCounselorTime(createDto);
                if (consultationTimeRequest.IsSuccess == false)
                {
                    return BadRequest(consultationTimeRequest);
                }

                return Ok(consultationTimeRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in CreateCounselorTime.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while create counselor time", Data = null });
            }
        }

        /// <summary>
        /// Imports data from a list.
        /// (id must be unique.)
        /// </summary>
        [HttpPost("counselor/import-time")]
        [RequestSizeLimit(100 * 1024 * 1024)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ConsultationTimePermissions.Import)]
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
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while import", Data = null });
            }
        }

        /// <summary>
        /// A new Counselor request is sent by a user.
        /// </summary>
        [HttpPost("counselor/request")]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CounselorPermissions.Create)]
        public async Task<IActionResult> SendCounselorRequest([FromBody] CreateCounselorRequestDto counselorRequestDto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _consultationService.SendCounselorRequest(counselorRequestDto);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in SendCounselorRequest.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while send counselor request", Data = null });
            }
        }

        /// <summary>
        /// Processing the status of the counselor's request (acceptance or rejection).
        /// </summary>
        [HttpPost("counselor/requests-process")]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CounselorPermissions.ProcessCounselorRequest)]
        public async Task<IActionResult> ProcessCounselorRequest([FromBody] ProcessCounselorRequestDto processCounselorRequest)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _consultationService.ProgressCounselorRequest(processCounselorRequest);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in ProcessCounselorRequest.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while process counselor request", Data = null });
            }
        }

        /// <summary>
        /// EntreLaunchdates an existing consultation time.
        /// </summary>
        [HttpPatch("counselor/edit-time/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(Permissions.ConsultationTimePermissions.Edit)]
        public async Task<IActionResult> EditCounselorTimes(int id, [FromBody] ConsultationTimeEntreLaunchdateDto EntreLaunchdateDto)
        {
            var userCheck = CheckUserOrUnauthorized();
            if (userCheck != null) return userCheck;

            var isValidCounselor = await _consultationService.IsCounselor(EntreLaunchdateDto.CounselorId);
            if (isValidCounselor.IsSuccess == false)
            {
                return BadRequest(isValidCounselor);
            }

            var consultationTimeRequest = await _consultationService.EditCounselorTimes(id, EntreLaunchdateDto);
            if (consultationTimeRequest.IsSuccess == false)
            {
                return BadRequest(consultationTimeRequest);
            }

            return Ok(consultationTimeRequest);
        }

        /// <summary>
        /// Get all consultations.
        /// </summary>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(ConsultationPermissions.ShowAll)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var consultationRequest = await _consultationService.AllConsultationRequest();
                if (consultationRequest.IsSuccess == false)
                {
                    return BadRequest(consultationRequest);
                }

                return Ok(consultationRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetAll.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while get all consultations", Data = null });
            }
        }

        /// <summary>
        /// Get all consultations by type.
        /// </summary>
        [HttpGet("by-type")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(ConsultationPermissions.GetByType)]
        public async Task<IActionResult> GetByType(ConsultationType type)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var consultations = await _consultationService.GetConsultationByType(type);
                if (consultations.IsSuccess == false)
                {
                    return BadRequest(consultations);
                }

                return Ok(consultations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetByType.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while get consultations by type", Data = null });
            }
        }

        /// <summary>
        /// Get one consultation.
        /// </summary>
        [HttpGet("get-one/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(ConsultationPermissions.ShowOne)]
        public async Task<IActionResult> GetOne(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not logged in.");
            }

            var consultationRequest = await _consultationService.GetConsultationRequestById(id);
            if (consultationRequest == null)
            {
                _logger.LogWarning("No consultation request found.");
                return BadRequest("No consultation request found.");
            }

            return Ok(consultationRequest);
        }

        /// <summary>
        /// Get Counselor consultations.
        /// </summary>
        [HttpGet("counselor/get-one/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(ConsultationPermissions.GetCounselorConsultations)]
        public async Task<IActionResult> GetByCounselor(int id)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var consultationRequest = await _consultationService.GetConsultationForCounselor(id);
                if (consultationRequest.IsSuccess == false)
                {
                    return BadRequest(consultationRequest);
                }

                return Ok(consultationRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetByCounselor.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while get consultations by counselor", Data = null });
            }
        }

        /// <summary>
        /// Get all Consultation Status.
        /// </summary>
        [HttpGet("all-status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(ConsultationPermissions.GetEnumValues)]
        public IActionResult GetAllStatus()
        {
            var userCheck = CheckUserOrUnauthorized();
            if (userCheck != null) return userCheck;

            var enumValues = _extendedBaseService.GetEnumValues<ConsultationStatus>();
            return Ok(new GeneralResult { IsSuccess = true, Message = "Success get all consultation status", Data = enumValues });
        }

        /// <summary>
        /// Retrieves a list of all counselor consultation times.
        /// </summary>
        [HttpGet("counselor/get-times")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(Permissions.ConsultationTimePermissions.ShowAll)]
        public async Task<IActionResult> GetAllCounselorTimes(int counselorId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var isValidCounselor = await _consultationService.IsCounselor(counselorId);
                if (isValidCounselor.IsSuccess == false)
                {
                    return BadRequest(isValidCounselor);
                }

                var counselorTimes = await _consultationService.GetAllCounselorTimes(counselorId);
                if (counselorTimes.IsSuccess == false)
                {
                    return BadRequest(counselorTimes);
                }

                return Ok(counselorTimes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetAllCounselorTimes.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while get all counselor times", Data = null });
            }
        }

        /// <summary>
        /// Fetch all Counselor Requests.
        /// </summary>
        [HttpGet("counselor/all-requests")]
        [ProducesResponseType(typeof(List<CounselorRequestDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CounselorPermissions.ShowAll)]
        public async Task<IActionResult> GetAllCounselorRequests()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var counselors = await _consultationService.AllCounselorRequest();
                if (counselors.IsSuccess == false)
                {
                    return BadRequest(counselors);
                }

                return Ok(counselors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetAllCounselorRequests.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while get all counselor requests", Data = null });
            }
        }

        /// <summary>
        /// Show of all pending counselor requests.
        /// </summary>
        [HttpGet("counselor/pending")]
        [ProducesResponseType(typeof(List<CounselorRequestDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CounselorPermissions.GetPending)]
        public async Task<IActionResult> GetPendingCounselorRequests()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var counselors = await _consultationService.PendingCounselorRequest();
                if (counselors.IsSuccess == false)
                {
                    return BadRequest(counselors);
                }

                return Ok(counselors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetPendingCounselorRequests.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while get pending counselor requests", Data = null });
            }
        }

        /// <summary>
        /// Show all accepted counselor requests.
        /// </summary>
        [HttpGet("counselor/accepted")]
        [ProducesResponseType(typeof(List<CounselorRequestDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CounselorPermissions.GetAccepted)]
        public async Task<IActionResult> GetAcceptedCounselorRequests()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var counselors = await _consultationService.AcceptedCounselorRequest();
                if (counselors.IsSuccess == false)
                {
                    return BadRequest(counselors);
                }

                return Ok(counselors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetAcceptedCounselorRequests.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while get accepted counselor requests", Data = null });
            }
        }

        /// <summary>
        /// show of all rejected counselor requests.
        /// </summary>
        [HttpGet("counselor/rejected")]
        [ProducesResponseType(typeof(List<CounselorRequestDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CounselorPermissions.GetRejected)]
        public async Task<IActionResult> GetRejectedCounselorRequests()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var counselors = await _consultationService.RejectedCounselorRequest();
                if (counselors.IsSuccess == false)
                {
                    return BadRequest(counselors);
                }

                return Ok(counselors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetRejectedCounselorRequests.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while get rejected counselor requests", Data = null });
            }
        }

        /// <summary>
        /// show all counselor requests by specialization.
        /// </summary>
        [HttpGet("counselor/by-specialization")]
        [ProducesResponseType(typeof(List<CounselorRequestDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CounselorPermissions.CounselorBySpecialization)]
        public async Task<IActionResult> CounselorBySpecialization(string specialization)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var counselors = await _consultationService.CounselorBySpecialization(specialization);
                if (counselors.IsSuccess == false)
                {
                    return BadRequest(counselors);
                }

                return Ok(counselors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in CounselorBySpecialization.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while counselor by specialization", Data = null });
            }
        }

        /// <summary>
        /// show counselor cv.
        /// </summary>
        [HttpGet("counselor/cv")]
        [ProducesResponseType(typeof(List<CounselorRequestDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(CounselorPermissions.ShowCounselorCV)]
        public async Task<IActionResult> CounselorCV(int id)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var counselors = await _consultationService.CounselorCV(id);
                if (counselors.IsSuccess == false)
                {
                    return BadRequest(counselors);
                }

                return Ok(counselors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in CounselorCV.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while counselor cv", Data = null });
            }
        }

        /// <summary>
        /// Soft deletes an entity and its related entities with cascading soft delete.
        /// </summary>
        [HttpDelete("delete-time/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(ConsultationTimePermissions.CascadeDelete)]
        public async Task<IActionResult> DeleteWithCascade(int id)
        {
            var userCheck = CheckUserOrUnauthorized();
            if (userCheck != null) return userCheck;

            if (id <= 0)
            {
                return BadRequest(new GeneralResult { IsSuccess = false, Message = "Invalid ID sEntreLaunchplied." });
            }

            var transactionId = Guid.NewGuid();
            _logger.LogInformation("Transaction {TransactionId}: Starting soft delete for entity ID {Id}.", transactionId, id);

            try
            {
                var result = await _deleteService.SoftDeleteCascadeAsync<ConsultationTime>(id);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in DeleteWithCascade.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while delete with cascade", Data = null });
            }
        }
    }
}
