namespace EntreLaunch.Web.Controllers.TrainingAPI
{
    [Authorize(Roles = AppRoles.TrainingRoles)]
    [Route("api/[controller]")]
    public class ExamsManegmentController(
        ILogger<ExamsManegmentController> logger,
        CascadeDeleteService deleteService,
        ILocalizationManager localization,
        IExtendedBaseService extendedBaseService,
        IExamService examService) : AuthenticatedController(localization)
    {
        private readonly ILogger<ExamsManegmentController> _logger = logger;
        private readonly CascadeDeleteService _deleteService = deleteService;
        private readonly IExtendedBaseService _extendedBaseService = extendedBaseService;

        /// <summary>
        /// Creates a new exam associated with a specific lesson.
        /// Validates the lesson existence before creation.
        /// </summary>
        /// <param name="dto">An object containing detailed data required to create a lesson exam, including the LessonId.</param>
        /// <returns>
        /// Returns an HTTP 200 response with the result if the creation is successful; otherwise, returns HTTP 400 for validation or business logic failures.
        /// </returns>
        [HttpPost("lesson-exam")]
        [RequiredPermission(ExamPermissions.Create)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateLessonExam([FromBody] FullLessonExamDto dto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                if (dto.LessonId != 0)
                {
                    var isLessonValid = await _extendedBaseService.IsEntityExistsAndNotDeletedAsync<Lesson>(dto.LessonId);
                    if (isLessonValid.IsSuccess == false)
                    {
                        return BadRequest(isLessonValid);
                    }
                }

                var result = await examService.CreateLessonExam(dto);
                if (result.IsSuccess == false)
                    return BadRequest(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new lesson exam.");
                return this.UnexpectedError("creating a new lesson exam.");
            }
        }

        /// <summary>
        /// Creates a new exam associated with a specific course.
        /// Validates the course existence before creation.
        /// </summary>
        /// <param name="dto">An object containing detailed data required to create a course exam, including the CourseId.</param>
        /// <returns>
        /// Returns an HTTP 200 response with the result if the creation is successful; otherwise, returns HTTP 400 for validation or business logic failures.
        /// </returns>
        [HttpPost("course-exam")]
        [RequiredPermission(ExamPermissions.Create)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateCourseExam([FromBody] FullCourseExamDto dto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                if (dto.CourseId != 0)
                {
                    var isLessonValid = await _extendedBaseService.IsEntityExistsAndNotDeletedAsync<Course>(dto.CourseId);
                    if (isLessonValid.IsSuccess == false)
                    {
                        return BadRequest(isLessonValid);
                    }
                }

                var result = await examService.CreateCourseExam(dto);
                if (result.IsSuccess == false)
                    return BadRequest(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new course exam.");
                return this.UnexpectedError("creating a new course exam.");
            }
        }

        /// <summary>
        /// Creates a new exam for a learning path.
        /// Assumes path validity is handled internally.
        /// </summary>
        /// <param name="dto">An object containing data required to create a path exam.</param>
        /// <returns>
        /// Returns an HTTP 200 response if the exam is created successfully; otherwise, returns HTTP 400 in case of validation or processing errors.
        /// </returns>
        [HttpPost("path-exam")]
        [RequiredPermission(ExamPermissions.Create)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePathExam([FromBody] FullPathExamDto dto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await examService.CreatePathExam(dto);
                if (result.IsSuccess == false)
                    return BadRequest(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new path exam.");
                return this.UnexpectedError("creating a new path exam.");
            }
        }

        /// <summary>
        /// Retrieves the exam details associated with a specific lesson.
        /// </summary>
        /// <param name="lessonId">The unique identifier of the lesson whose exam should be retrieved.</param>
        /// <returns>
        /// Returns HTTP 200 with exam details if found; otherwise, returns HTTP 404 if no exam is associated with the specified lesson.
        /// </returns>
        [HttpGet("by-lesson/{lessonId:int}")]
        [RequiredPermission(ExamPermissions.GetOne)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetLessonExam([FromRoute] int lessonId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await examService.GetExamByLessonIdAsync(lessonId);
                return result.IsSuccess ? Ok(result) : NotFound(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching lesson exam {lessonId}", lessonId);
                return this.UnexpectedError("fetching lesson exam.");
            }
        }

        /// <summary>
        /// Retrieves the exam details associated with a specific course.
        /// </summary>
        /// <param name="courseId">The unique identifier of the course whose exam should be retrieved.</param>
        /// <returns>
        /// Returns HTTP 200 with exam details if found; otherwise, returns HTTP 404 if no exam is associated with the specified course.
        /// </returns>
        [HttpGet("by-course/{courseId:int}")]
        [RequiredPermission(ExamPermissions.GetOne)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCourseExam([FromRoute] int courseId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await examService.GetExamByCourseIdAsync(courseId);
                return result.IsSuccess ? Ok(result) : NotFound(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching course exam {courseId}", courseId);
                return this.UnexpectedError("fetching course exam.");
            }
        }

        /// <summary>
        /// Retrieves a list of all exams associated with learning paths.
        /// </summary>
        /// <returns>
        /// Returns HTTP 200 with the list of path exams if successful; otherwise, returns HTTP 404 if no exams are found.
        /// </returns>
        [HttpGet("path-exams")]
        [RequiredPermission(ExamPermissions.GetAll)]
        [ProducesResponseType(typeof(PaginatedResult<ExamFullDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPathExams([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await examService.GetPathExamsAsync(pagination, cancellationToken);
                return result.IsSuccess ? Ok(result) : NotFound(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching path exams.");
                return this.UnexpectedError("fetching path exams");
            }
        }

        /// <summary>
        /// Updates an existing lesson exam with new information.
        /// </summary>
        /// <param name="examId">The unique identifier of the lesson exam to update.</param>
        /// <param name="dto">An object containing the updated data for the lesson exam.</param>
        /// <returns>
        /// Returns HTTP 200 if the update is successful; otherwise, returns HTTP 400 if validation or update fails.
        /// </returns>
        [HttpPatch("update-lesson-exam/{examId:int}")]
        [RequiredPermission(ExamPermissions.Edit)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateLessonExam([FromRoute] int examId, [FromBody] UpdateLessonExamDto dto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await examService.UpdateLessonExamAsync(examId, dto);
                return result.IsSuccess == true ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating lesson exam {examId}", examId);
                return this.UnexpectedError("updating lesson exam.");
            }
        }

        /// <summary>
        /// Updates an existing course exam with new information.
        /// </summary>
        /// <param name="examId">The unique identifier of the course exam to update.</param>
        /// <param name="dto">An object containing the updated data for the course exam.</param>
        /// <returns>
        /// Returns HTTP 200 if the update is successful; otherwise, returns HTTP 400 if validation or update fails.
        /// </returns>
        [HttpPatch("update-course-exam/{examId:int}")]
        [RequiredPermission(ExamPermissions.Edit)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCourseExam([FromRoute] int examId, [FromBody] UpdateCourseExamDto dto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await examService.UpdateCourseExamAsync(examId, dto);
                return result.IsSuccess == true ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating course exam {examId}", examId);
                return this.UnexpectedError("updating course exam.");
            }
        }

        /// <summary>
        /// Updates an existing path exam with new information.
        /// </summary>
        /// <param name="examId">The unique identifier of the path exam to update.</param>
        /// <param name="dto">An object containing the updated data for the path exam.</param>
        /// <returns>
        /// Returns HTTP 200 if the update is successful; otherwise, returns HTTP 400 if validation or update fails.
        /// </returns>
        [HttpPatch("update-path-exam/{examId:int}")]
        [RequiredPermission(ExamPermissions.Edit)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePathExam([FromRoute] int examId, [FromBody] UpdatePathExamDto dto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await examService.UpdatePathExamAsync(examId, dto);
                return result.IsSuccess == true ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating path exam {examId}", examId);
                return this.UnexpectedError("updating path exam.");
            }
        }

        /// <summary>
        /// Retrieves all available exam parent entity types.
        /// Requires permission to get enum values.
        /// </summary>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> with a list of <see cref="EnumData"/> representing parent entity types.
        /// </returns>
        [HttpGet("parent-type")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ExamPermissions.GetEnumValues)]
        public ActionResult<IEnumerable<EnumData>> GetExamParentEntityType()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var enumValues = _extendedBaseService.GetEnumValues<ExamParentEntityType>();
                if (enumValues == null)
                {
                    return BadRequest(new GeneralResult { IsSuccess = false, Message = "No Exam Parent Entity Type found. please try again", Data = null });
                }

                return Ok(new GeneralResult { IsSuccess = true, Message = "Success", Data = enumValues });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the Exam GetExamParentEntityType method.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while getting Exam Parent Entity Type.", Data = null });
            }
        }

        /// <summary>
        /// Performs a soft delete of an exam and its related entities.
        /// Requires permission for cascading deletes.
        /// </summary>
        /// <param name="id">The ID of the exam to delete.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the delete operation.
        /// </returns>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ExamPermissions.CascadeDelete)]
        public async Task<IActionResult> DeleteWithCascade([FromRoute] int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
            }

            if (id <= 0)
            {
                return BadRequest(new GeneralResult { IsSuccess = false, Message = "Invalid entity ID." });
            }

            var transactionId = Guid.NewGuid();
            _logger.LogInformation("Transaction {TransactionId}: Starting soft delete for Exam ID {Id}.", transactionId, id);

            try
            {
                var result = await _deleteService.SoftDeleteCascadeAsync<Exam>(id);
                if (result.IsSuccess == false)
                {
                    return NotFound(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the Exam DeleteWithCascade method.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while soft deleting the Exam.", Data = null });
            }
        }
    }
}

