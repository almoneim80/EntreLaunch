namespace EntreLaunch.Web.Controllers.TrainingAPI
{
    [Authorize(Roles = AppRoles.TrainingRoles)]
    [Route("api/[controller]")]
    public class ExamResultController(
        ILocalizationManager localization,
        IExamService examService,
        ILogger<ExamResultController> logger) : AuthenticatedController(localization)
    {
        private readonly ILogger<ExamResultController> _logger = logger;

        /// <summary>
        /// Submits an exam result for a specific student.
        /// Performs evaluation and stores the result.
        /// </summary>
        /// <param name="examId">The ID of the exam to submit.</param>
        /// <param name="submission">The student's answers and submission data.</param>
        /// <returns>An <see cref="IActionResult"/> indicating success or failure.</returns>
        [HttpPost("submit/{examId:int}")]
        [SwaggerOperation(Tags = new[] { "ExamsManegment" })]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ExamResultPermissions.Submit)]
        public async Task<IActionResult> SubmitExamAsync([FromRoute] int examId, [FromBody] ExamSubmissionDto submission)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                
                var result = await examService.CalculateExamResultAsync(
                    examId, userId, submission.Answers, submission.TimeTakenInSeconds);

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
        /// Retrieves the exam result for the currently authenticated student.
        /// </summary>
        /// <param name="examId">The exam ID.</param>
        /// <returns>The student's result for the specified exam.</returns>
        [HttpGet("student-result/{examId:int}")]
        [SwaggerOperation(Tags = new[] { "ExamsManegment" })]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ExamResultPermissions.GetByStudent)]
        public async Task<IActionResult> GetExamResultForStudentAsync([FromRoute] int examId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
            }

            try
            {
                var result = await examService.GetExamResultForStudentAsync(examId, userId);
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
        /// Compares a student's result with the batch average.
        /// </summary>
        /// <param name="examId">The exam ID.</param>
        /// <returns>A comparison summary between the student and batch performance.</returns>
        [HttpGet("compare-result/{examId:int}")]
        [SwaggerOperation(Tags = new[] { "ExamsManegment" })]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ExamResultPermissions.CompareStudentResult)]
        public async Task<IActionResult> CompareStudentResultWithBatchAsync([FromRoute] int examId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
            }

            try
            {
                var comparisonResult = await examService.CompareStudentResultWithBatchAsync(examId, userId);
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
        /// Retrieves statistics for a given exam.
        /// Includes averages, distribution, etc.
        /// </summary>
        /// <param name="examId">The ID of the exam.</param>
        /// <returns>Statistics about the exam's results.</returns>
        [HttpGet("statistics/{examId:int}")]
        [SwaggerOperation(Tags = new[] { "ExamsManegment" })]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ExamResultPermissions.GetExamStatistics)]
        public async Task<IActionResult> GetExamStatisticsAsync([FromRoute] int examId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var statistics = await examService.GetExamStatisticsAsync(examId);
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
        /// Retrieves the top 10 students ranked by score in a specific exam.
        /// </summary>
        /// <param name="examId">The ID of the exam.</param>
        /// <returns>A list of top-performing students.</returns>
        [HttpGet("top-students/{examId:int}")]
        [SwaggerOperation(Tags = new[] { "ExamsManegment" })]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ExamResultPermissions.GetTopTenStudents)]
        public async Task<IActionResult> GetTopTenStudentsAsync([FromRoute] int examId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var topStudents = await examService.GetTopTenStudentsAsync(examId);
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
        /// Retrieves all attempts for a student in a given exam.
        /// </summary>
        /// <param name="examId">The ID of the exam.</param>
        /// <returns>A list of student attempts including best score.</returns>
        [HttpGet("attempts/{examId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(Tags = new[] { "ExamsManegment" })]
        [RequiredPermission(ExamResultPermissions.GetStudentAttempts)]
        public async Task<IActionResult> GetStudentAttemptsAsync(int examId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
            }

            try
            {
                var attemptsWithBest = await examService.GetStudentAttemptsAsync(examId, userId);
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
        /// Retrieves the currently active result for a student in a specific exam.
        /// </summary>
        /// <param name="examId">The exam ID.</param>
        /// <returns>The active result for the student if available.</returns>
        [HttpGet("active-result/{examId:int}")]
        [SwaggerOperation(Tags = new[] { "ExamsManegment" })]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ExamResultPermissions.GetActiveResult)]
        public async Task<IActionResult> GetActiveResultAsync([FromRoute] int examId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
            }

            try
            {
                var activeResult = await examService.GetActiveResultAsync(examId, userId);
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

        /// <summary>
        /// Allows a user to retake a specific exam.
        /// Requires the user to have permission to retake exams.
        /// </summary>
        /// <param name="examId">The unique identifier of the exam to be retaken.</param>
        /// <param name="submission">The submission data containing answers and time taken.</param>
        /// <returns>An <see cref="IActionResult"/> containing the retake result or validation errors.</returns>
        [HttpPost("retake/{examId:int}")]
        [SwaggerOperation(Tags = new[] { "ExamsManegment" })]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ExamPermissions.Retake)]
        public async Task<IActionResult> RetakeExamAsync([FromRoute] int examId, [FromBody] ExamSubmissionDto submission)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var result = await examService.RetakeExamAsync(
                    examId, submission.UserId, submission.Answers, submission.TimeTakenInSeconds);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Retake not allowed: {Message}", ex.Message);
                return BadRequest(new { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retaking Exam {ExamId} for User {UserId}.", examId, submission.UserId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { Message = "An error occurred while processing the retake request." });
            }
        }

        /// <summary>
        /// Checks whether the current user is allowed to retake a given exam.
        /// Requires retake-check permission.
        /// </summary>
        /// <param name="examId">The ID of the exam to check.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating whether the user can retake the exam.
        /// </returns>
        [HttpGet("can-retake/{examId:int}")]
        [SwaggerOperation(Tags = new[] { "ExamsManegment" })]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [RequiredPermission(ExamPermissions.CanRetake)]
        public async Task<IActionResult> CanRetakeExamAsync([FromRoute] int examId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
            }

            try
            {
                var canRetake = await examService.CanRetakeExamAsync(examId, userId);
                if (canRetake.IsSuccess == false)
                {
                    return BadRequest(canRetake);
                }

                return Ok(canRetake);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the Exam CanRetakeExamAsync method.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while checking if the user can retake the exam.", Data = null });
            }
        }
    }
}
