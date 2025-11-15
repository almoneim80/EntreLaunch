namespace EntreLaunch.Web.Controllers.TrainingAPI
{
    [Authorize(Roles = "Admin, Trainer, Student")]
    [Route("api/[controller]")]
    public class ExamResultController : BaseController<ExamResult, ExamResultCreateDto, ExamResultEntreLaunchdateDto, ExamResultDetailsDto, ExamResultExportDto>
    {
        private readonly IExtendedBaseService _extendedBaseService;
        private readonly ITrainingSectionService _trainingSection;
        private readonly ILogger<ExamResultController> _logger;
        public ExamResultController(
            BaseService<ExamResult, ExamResultCreateDto, ExamResultEntreLaunchdateDto, ExamResultDetailsDto> service,
            ILocalizationManager? localization,
            ILogger<ExamResultController> logger,
            IExtendedBaseService extendedBaseService,
            ITrainingSectionService trainingSection,
            IExportService exportService)
            : base(service, localization, logger, exportService)
        {
            _extendedBaseService = extendedBaseService;
            _trainingSection = trainingSection;
            _logger = logger;
        }

        /// <summary>
        /// Submit the result of a specific student in a specific exam.
        /// </summary>
        [HttpPost("submit/{examId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SubmitExamAsync(int examId, [FromBody] ExamSubmissionDto submission)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var result = await _trainingSection.CalculateExamResultAsync(
                    examId, submission.UserId, submission.Answers, submission.TimeTakenInSeconds);

                if(result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in SubmitExamAsync.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in SubmitExamAsync." });
            }
        }

        /// <summary>
        /// EntreLaunchdates an existing entity.
        /// </summary>
        [HttpPatch("edit/{id}")]
        [RequiredPermission(ExamResultPermissions.Edit)]
        public override async Task<ActionResult<ExamResultDetailsDto>> Patch(int id, [FromBody] ExamResultEntreLaunchdateDto EntreLaunchdateDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var isReferencedValid = await _extendedBaseService.IsEntityExistsAndNotDeletedAsync<Exam>(EntreLaunchdateDto.ExamId);
                if (isReferencedValid.IsSuccess == false)
                {
                    return BadRequest(isReferencedValid);
                }

                return await base.Patch(id, EntreLaunchdateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to EntreLaunchdate a Exam Result");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to EntreLaunchdate a Exam Result" });
            }
        }

        /// <summary>
        /// Get all ExamResults.
        /// </summary>
        [HttpGet("all")]
        [RequiredPermission(ExamResultPermissions.ShowAll)]
        public override async Task<ActionResult<ExamResultDetailsDto[]>> GetAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
            }

            return await base.GetAll();
        }

        /// <summary>
        /// Get one ExamResult.
        /// </summary>
        [HttpGet("get-one/{id}")]
        [RequiredPermission(ExamResultPermissions.ShowOne)]
        public override async Task<ActionResult<ExamResultDetailsDto>> GetOne(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
            }

            return await base.GetOne(id);
        }

        /// <summary>
        /// Exports ExamResults to CSV.
        /// </summary>
        [HttpGet("export/csv")]
        [RequiredPermission(ExamResultPermissions.Export)]
        public override async Task<IActionResult> ExportToCsv()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
            }

            return await base.ExportToCsv();
        }

        /// <summary>
        /// Exports ExamResults to Excel file.
        /// </summary>
        [HttpGet("export/excel")]
        [RequiredPermission(ExamResultPermissions.Export)]
        public override async Task<IActionResult> ExportToExcel()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
            }

            return await base.ExportToExcel();
        }

        /// <summary>
        /// Exports ExamResults to JSON file.
        /// </summary>
        [HttpGet("export/json")]
        [RequiredPermission(ExamResultPermissions.Export)]
        public override async Task<IActionResult> ExportToJson()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
            }

            return await base.ExportToJson();
        }

        /// <summary>
        /// Fetch the result of a specific student in a specific exam.
        /// </summary>
        [HttpGet("student-result/{examId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetExamResultForStudentAsync(int examId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
            }

            try
            {
                var result = await _trainingSection.GetExamResultForStudentAsync(examId, userId);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetExamResultForStudentAsync.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in GetExamResultForStudentAsync." });
            }
        }

        /// <summary>
        /// Compare the result of a specific student in a specific exam with the batch result.
        /// </summary>
        [HttpGet("compare-result/{examId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CompareStudentResultWithBatchAsync(int examId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
            }

            try
            {
                var comparisonResult = await _trainingSection.CompareStudentResultWithBatchAsync(examId, userId);
                if (comparisonResult.IsSuccess == false)
                {
                    return BadRequest(comparisonResult);
                }

                return Ok(comparisonResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in CompareStudentResultWithBatchAsync.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in CompareStudentResultWithBatchAsync." });
            }
        }

        /// <summary>
        /// Fetch the statistics for a specific exam.
        /// </summary>
        [HttpGet("statistics/{examId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetExamStatisticsAsync(int examId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var statistics = await _trainingSection.GetExamStatisticsAsync(examId);
                if (statistics.IsSuccess == false)
                {
                    return BadRequest(statistics);
                }

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetExamStatisticsAsync.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in GetExamStatisticsAsync." });
            }
        }

        /// <summary>
        /// get top 10 student in exam.
        /// </summary>
        [HttpGet("top-students/{examId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTopTenStudentsAsync(int examId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var topStudents = await _trainingSection.GetTopTenStudentsAsync(examId);
                if (topStudents.IsSuccess == false)
                {
                    return BadRequest(topStudents);
                }

                return Ok(topStudents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetTopTenStudentsAsync.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in GetTopTenStudentsAsync." });
            }
        }

        /// <summary>
        /// Get all attempts for a student.
        /// </summary>
        [HttpGet("attempts/{examId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStudentAttemptsAsync(int examId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
            }

            try
            {
                var attemptsWithBest = await _trainingSection.GetStudentAttemptsAsync(examId, userId);
                if (attemptsWithBest.IsSuccess == false)
                {
                    return BadRequest(attemptsWithBest);
                }

                return Ok(attemptsWithBest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the Exam GetStudentAttemptsAsync method.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while getting student attempts.", Data = null });
            }
        }

        /// <summary>
        /// Get the active result for a specific student in a specific exam.
        /// </summary>
        [HttpGet("active-result/{examId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetActiveResultAsync(int examId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
            }

            try
            {
                var activeResult = await _trainingSection.GetActiveResultAsync(examId, userId);
                if (activeResult.IsSuccess == false)
                {
                    return NotFound(activeResult);
                }

                return Ok(activeResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetActiveResultAsync.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "An error occurred in GetActiveResultAsync." });
            }
        }

        [NonAction]
        public override Task<ActionResult<ExamResultDetailsDto>> Create([FromBody] ExamResultCreateDto value)
        {
            throw new NotImplementedException("Create is not sEntreLaunchported in this controller.");
        }
    }
}
