namespace EntreLaunch.Web.Controllers.TrainingAPI
{
    [Authorize(Roles = AppRoles.TrainingRoles)]
    [Route("api/[controller]")]
    public class LessonController(
        ILogger<LessonController> logger,
        CascadeDeleteService deleteService,
        ILocalizationManager localization,
        IExtendedBaseService extendedBaseService,
        ILessonService lessonService,
        IMultipleImportService<Lesson, LessonWithRelatedContent> multipleImportService) : AuthenticatedController(localization)
    {
        private readonly ILogger<LessonController> _logger = logger;
        private readonly CascadeDeleteService _deleteService = deleteService;
        private readonly IExtendedBaseService _extendedBaseService = extendedBaseService;
        private readonly ILessonService _lessonService = lessonService;
        private readonly IMultipleImportService<Lesson, LessonWithRelatedContent> _multipleImportService = multipleImportService;

        /// <summary>
        /// Creates a new Lesson entity and persists it to the data store.
        /// </summary>
        /// <param name="createDto">The data required to create the Lesson, including associated CourseId.</param>
        /// <returns>Returns the created Lesson details upon success.</returns>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonPermissions.Create)]
        public async Task<IActionResult> Create([FromBody] LessonCreateDto createDto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                var isReferencedValid = await _extendedBaseService.IsEntityExistsAndNotDeletedAsync<Course>(createDto.CourseId);
                if (isReferencedValid.IsSuccess == false)
                {
                    return BadRequest(isReferencedValid);
                }

                var result = await _lessonService.CreateLessonAsync(createDto);
                return result.IsSuccess == true ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create a new Lesson");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to create a new Lesson" });
            }
        }

        /// <summary>
        /// Imports a list of Lesson records in bulk.
        /// </summary>
        /// <param name="dtos">A list of LessonWithRelatedContent objects to be imported.</param>
        /// <returns>Returns the result of the import operation including success status and errors if any.</returns>
        [HttpPost("import")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonPermissions.Import)]
        public async Task<IActionResult> ImportLessons([FromBody] List<LessonWithRelatedContent> dtos)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                if (dtos is null || dtos.Count == 0)
                    return BadRequest("Request body is empty.");

                var result = await _multipleImportService.ImportAsync(dtos);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to import Lessons");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to import Lessons" });
            }
        }

        /// <summary>
        /// Updates an existing Lesson entity based on provided data.
        /// </summary>
        /// <param name="id">The unique identifier of the Lesson to be updated.</param>
        /// <param name="updateDto">The data object containing updated values for the Lesson.</param>
        /// <returns>Returns the updated Lesson details upon success.</returns>
        [HttpPatch("edit/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonPermissions.Edit)]
        public async Task<IActionResult> EditLesson([FromRoute] int id, [FromBody] LessonUpdateDto updateDto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var isReferencedValid = await _extendedBaseService.IsEntityExistsAndNotDeletedAsync<Course>(updateDto.CourseId);
                if (isReferencedValid.IsSuccess == false)
                {
                    return BadRequest(isReferencedValid);
                }

                var result = await _lessonService.UpdateLessonAsync(id, updateDto);
                return result.IsSuccess == true ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update Lesson");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to update Lesson" });
            }
        }

        /// <summary>
        /// Reorders the list of Lessons in a course based on the provided order.
        /// </summary>
        /// <param name="courseId">The ID of the course whose lessons need to be reordered.</param>
        /// <param name="newOrderList">A list of LessonReorderDto objects representing the new order.</param>
        /// <returns>Returns result indicating success or failure of the operation.</returns>
        [HttpPut("reorder/{courseId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonPermissions.Reorder)]
        public async Task<IActionResult> ReorderLessonsAsync([FromRoute] int courseId, [FromBody] List<LessonReorderDto> newOrderList)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var success = await _lessonService.ReorderLessonsAsync(courseId, newOrderList);
                if (success.IsSuccess == false)
                {
                    return BadRequest(success);
                }

                return Ok(success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reorder Lessons");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to reorder Lessons" });
            }
        }

        /// <summary>
        /// Retrieves all existing Lesson records.
        /// </summary>
        /// <returns>Returns a list of all Lessons in the system.</returns>
        [HttpGet("all")]
        [ProducesResponseType(typeof(PaginatedResult<LessonFullDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonPermissions.GetAll)]
        public async Task<IActionResult> AllLessons([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _lessonService.GetAllLessonsAsync(pagination, cancellationToken);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all Lessons");
                return this.UnexpectedError("get all lessons");
            }
        }

        /// <summary>
        /// Retrieves a single Lesson entity by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the Lesson to retrieve.</param>
        /// <returns>Returns the corresponding Lesson details if found.</returns>
        [HttpGet("get-one/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonPermissions.GetOne)]
        public async Task<IActionResult> GetLesson([FromRoute] int id)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _lessonService.GetLessonByIdAsync(id);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get one Lesson");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to get one Lesson" });
            }
        }

        /// <summary>
        /// Retrieves all lessons associated with a specific course ID.
        /// </summary>
        /// <param name="courseId">The ID of the course to retrieve lessons for.</param>
        /// <returns>Returns all lessons under the specified course.</returns>
        [HttpGet("by-course/{courseId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonPermissions.GetByCourse)]
        public async Task<IActionResult> GetLessonsByCourseIdAsync([FromRoute] int courseId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var lessons = await _lessonService.GetLessonsByCourseIdAsync(courseId);
                if (lessons.IsSuccess == false)
                {
                    return BadRequest(lessons);
                }

                return Ok(lessons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get lessons by course id");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to get lessons by course id" });
            }
        }

        /// <summary>
        /// Performs a soft delete on a Lesson entity, including any related entities.
        /// </summary>
        /// <param name="id">The ID of the Lesson to be soft-deleted.</param>
        /// <returns>Returns result indicating the success or failure of the delete operation.</returns>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LessonPermissions.CascadeDelete)]
        public async Task<IActionResult> DeleteWithCascade([FromRoute] int id)
        {
            var userCheck = CheckUserOrUnauthorized();
            if (userCheck != null) return userCheck;

            if (id <= 0)
            {
                return BadRequest(new GeneralResult { IsSuccess = false, Message = "Invalid entity ID." });
            }

            var transactionId = Guid.NewGuid();
            _logger.LogInformation("Transaction {TransactionId}: Starting soft delete for entity ID {Id}.", transactionId, id);

            try
            {
                var result = await _deleteService.SoftDeleteCascadeAsync<Lesson>(id);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Transaction {TransactionId}: Failed to soft delete entity ID {Id}.", transactionId, id);
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Failed to soft delete entity." });
            }
        }
    }
}
