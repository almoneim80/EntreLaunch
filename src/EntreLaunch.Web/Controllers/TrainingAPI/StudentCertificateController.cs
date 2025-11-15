namespace EntreLaunch.Web.Controllers.TrainingAPI
{
    [Authorize(Roles = "Admin , Entrepreneur, Trainer")]
    [Route("api/[controller]")]
    public class StudentCertificateController : BaseController<StudentCertificate, StudentCertificateCreateDto, StudentCertificateEntreLaunchdateDto, StudentCertificateDetailsDto, StudentCertificateExportDto>
    {
        private readonly IExtendedBaseService _extendedBaseService;
        private readonly ITrainingSectionService _trainingSection;
        private readonly ILogger<StudentCertificateController> _logger;

        public StudentCertificateController(
            BaseService<StudentCertificate, StudentCertificateCreateDto, StudentCertificateEntreLaunchdateDto, StudentCertificateDetailsDto> service,
            ILocalizationManager? localization,
            ILogger<StudentCertificateController> logger,
            IExtendedBaseService extendedBaseService,
            ITrainingSectionService trainingSectionService,
            IExportService exportService)
            : base(service, localization, logger, exportService)
        {
            _extendedBaseService = extendedBaseService;
            _trainingSection = trainingSectionService;
            _logger = logger;
        }

        /// <summary>
        /// Create certificate by admin.
        /// </summary>
        [HttpPost("issue-by-admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(StudentCertificatePermissions.Create)]
        public async Task<ActionResult<StudentCertificate>> IssueCertificateByAdmin([FromBody] StudentCertificateCreateDto createDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                if (createDto.UserId == null) return BadRequest(new GeneralResult { IsSuccess = false, Message = "User Id is required" });
                if (createDto.ExamId >= 0) return BadRequest(new GeneralResult { IsSuccess = false, Message = "Exam Id is required" });

                var result = await _trainingSection.IssueCertificateAsync(createDto.ExamId, createDto.UserId!);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a Student Certificate.");
                return StatusCode(StatusCodes.Status500InternalServerError, new GeneralResult { IsSuccess = false, Message = "An error occurred while creating a Student Certificate." });
            }
        }

        /// <summary>
        /// Create Certificate By Student.
        /// </summary>
        [HttpPost("issue-by-student")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(StudentCertificatePermissions.Create)]
        public async Task<ActionResult<StudentCertificate>> IssueCertificateByStudent([FromBody] StudentCertificateCreateDto createDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                createDto.UserId = userId;
                if (createDto.UserId == null) return BadRequest(new GeneralResult { IsSuccess = false, Message = "User Id is required" });
                if (createDto.ExamId >= 0) return BadRequest(new GeneralResult { IsSuccess = false, Message = "Exam Id is required" });

                var result = await _trainingSection.IssueCertificateAsync(createDto.ExamId, createDto.UserId!);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a Student Certificate.");
                return StatusCode(StatusCodes.Status500InternalServerError, new GeneralResult { IsSuccess = false, Message = "An error occurred while creating a Student Certificate." });
            }
        }

        /// <summary>
        /// EntreLaunchdate an existing StudentCertificate.
        /// </summary>
        [HttpPatch("edit/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(StudentCertificatePermissions.Edit)]
        public override async Task<ActionResult<StudentCertificateDetailsDto>> Patch(int id, [FromBody] StudentCertificateEntreLaunchdateDto EntreLaunchdateDto)
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
                _logger.LogError(ex, "Error occurred while EntreLaunchdating StudentCertificate.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error occurred while EntreLaunchdating StudentCertificate." });
            }
        }

        /// <summary>
        /// Get all Student Certificate Delivery Method.
        /// </summary>
        [HttpGet("delivery-method")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(StudentCertificatePermissions.GetEnumValues)]
        public ActionResult<IEnumerable<EnumData>> GetStudentCertificateDeliveryMethod()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var enumValues = _extendedBaseService.GetEnumValues<DeliveryMethod>();
                return Ok(new GeneralResult { IsSuccess = true, Message = "Student certificate delivery method retrieved successfully", Data = enumValues });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting Student Certificate Delivery Method.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error occurred while getting Student Certificate Delivery Method." });
            }
        }

        /// <summary>
        /// Get All StudentCertificates.
        /// </summary>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(StudentCertificatePermissions.ShowAll)]
        public override async Task<ActionResult<StudentCertificateDetailsDto[]>> GetAll()
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
                _logger.LogError(ex, "Error occurred while getting all StudentCertificates.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error occurred while getting all StudentCertificates." });
            }
        }

        /// <summary>
        /// Get One StudentCertificate.
        /// </summary>
        [HttpGet("get-one/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(StudentCertificatePermissions.ShowOne)]
        public override async Task<ActionResult<StudentCertificateDetailsDto>> GetOne(int id)
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
                _logger.LogError(ex, "Error occurred while getting one StudentCertificate.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error occurred while getting one StudentCertificate." });
            }
        }

        /// <summary>
        /// Delete an existing StudentCertificate.
        /// </summary>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(StudentCertificatePermissions.Delete)]
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
                _logger.LogError(ex, "Error occurred while deleting StudentCertificate.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error occurred while deleting StudentCertificate." });
            }
        }

        #region cancle
        [NonAction]
        public override async Task<ActionResult<StudentCertificateDetailsDto>> Create([FromBody] StudentCertificateCreateDto createDto)
        {
            return await Task.FromResult<ActionResult>(NotFound());
        }
        [NonAction]
        public override async Task<IActionResult> ExportToCsv()
        {
            return await Task.FromResult<ActionResult>(NotFound());
        }

        [NonAction]
        public override async Task<IActionResult> ExportToExcel()
        {
            return await Task.FromResult<ActionResult>(NotFound());
        }

        [NonAction]
        public override async Task<IActionResult> ExportToJson()
        {
            return await Task.FromResult<ActionResult>(NotFound());
        }
        #endregion
    }
}

