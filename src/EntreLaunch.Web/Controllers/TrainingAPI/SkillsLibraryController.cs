namespace EntreLaunch.Web.Controllers.TrainingAPI
{
    [Authorize(Roles = AppRoles.TrainingRoles)]
    [Route("api/courses/skills")]
    public class SkillsLibraryController(
        ILogger<SkillsLibraryController> logger,
        ILocalizationManager localizationManager,
        IPurchaseService purchaseService,
        ICourseService courseService,
        IExtendedBaseService extendedBaseService,
        ISkillCourseService skillCourseService,
        CascadeDeleteService deleteService) : AuthenticatedController(localizationManager)
    {
        private readonly ILogger<SkillsLibraryController> _logger = logger;
        private readonly IPurchaseService _purchaseService = purchaseService;
        private readonly ICourseService _courseService = courseService;
        private readonly ILocalizationManager _localizationManager = localizationManager;
        private readonly ISkillCourseService _skillCourseService = skillCourseService;
        private readonly CascadeDeleteService _deleteService = deleteService;
        private readonly IExtendedBaseService _extendedBaseService = extendedBaseService;

        /// <summary>
        /// Creates a new skill course based on the provided data transfer object (DTO).
        /// Requires the user to have the create permission.
        /// </summary>
        /// <param name="dto">An object containing the necessary data to create a new skill course.</param>
        /// <returns>Returns an IActionResult indicating the outcome of the creation process. Returns 200 OK if successful, or 400 Bad Request if validation fails.</returns>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(SkillCoursePermissions.Create)]
        public async Task<IActionResult> Create([FromBody] SkillCourseCreateDto dto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                var invalidRefCheck = await dto.FieldId.CheckIfEntityExistsAsync<CourseField>(_extendedBaseService, _logger, _localizationManager);
                if (invalidRefCheck != null) return invalidRefCheck;

                var result = await _courseService.CreateAsync<SkillCourseCreateDto>(dto);
                return result.IsSuccess == true ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the skills course Create method.");
                return this.UnexpectedError("skills course Create.");
            }
        }

        /// <summary>
        /// Updates an existing skill course identified by its unique ID using the provided DTO.
        /// Requires the user to have the edit permission.
        /// </summary>
        /// <param name="id">The unique identifier of the skill course to be updated.</param>
        /// <param name="dto">An object containing the updated data for the skill course.</param>
        /// <returns>Returns an IActionResult representing the result of the update operation. Returns 200 OK if successful, or 400 Bad Request if the update fails.</returns>
        [HttpPatch("update/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(SkillCoursePermissions.Edit)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] SkillCourseUpdateDto dto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _courseService.UpdateAsync(id, dto);
                return result.IsSuccess == true ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the skills course update method.");
                return this.UnexpectedError("skills course update.");
            }
        }

        /// <summary>
        /// Retrieves all skill courses classified under the SkillsLibCourse type.
        /// Requires the user to have the permission to view all courses.
        /// </summary>
        /// <returns>Returns an IActionResult containing the list of all skill courses or an appropriate error response if the retrieval fails.</returns>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(SkillCoursePermissions.GetAll)]
        public async Task<IActionResult> GetAll([FromForm] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _courseService.GetAllAsync(CourseType.SkillsLibCourse, pagination, cancellationToken);
                return result.IsSuccess == true ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the skills course GetAll method.");
                return this.UnexpectedError("getting skills courses.");
            }
        }

        /// <summary>
        /// Retrieves a single skill course by its unique ID.
        /// Requires the user to have the permission to view a specific course.
        /// </summary>
        /// <param name="id">The unique identifier of the skill course to retrieve.</param>
        /// <returns>Returns an IActionResult containing the course details if found, or an error response if not.</returns>
        [HttpGet("get-one/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(SkillCoursePermissions.GetOne)]
        public async Task<IActionResult> GetOne([FromRoute] int id)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _courseService.GetOneAsync(id, CourseType.SkillsLibCourse);
                return result.IsSuccess == true ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the skills course GetOne method.");
                return this.UnexpectedError("getting skills course.");
            }
        }

        /// <summary>
        /// Retrieves skill courses associated with a specific field ID.
        /// Requires the user to have the permission to filter courses by field.
        /// </summary>
        /// <param name="fieldId">The unique identifier of the field used to filter skill courses.</param>
        /// <returns>Returns an IActionResult with a list of matching courses or an error message if the operation fails.</returns>
        [HttpGet("by-field/{fieldId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(SkillCoursePermissions.GetByField)]
        public async Task<IActionResult> GetByField([FromRoute] int fieldId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _skillCourseService.GetByFieldAsync(fieldId);
                return result.IsSuccess == true ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the skills course GetOne method.");
                return this.UnexpectedError("getting skills course.");
            }
        }

        /// <summary>
        /// Retrieves a list of users enrolled in a specific skill course.
        /// Requires the user to have the appropriate permission to access enrollment data.
        /// </summary>
        [HttpGet("enrolled/course/")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(SkillCoursePermissions.GetEnrolled)]
        public async Task<IActionResult> GetEnrolledInCourseAsync([FromQuery] int courseId, [FromForm] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _courseService.GetUsersByCoursePurchaseAsync(PurchaseItemType.SkillsLibCourse, courseId, pagination, cancellationToken);
                return result.IsSuccess == true ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "fetching enrolled users in online course.");
                return this.UnexpectedError("fetching enrolled users in online course.");
            }
        }

        /// <summary>
        /// Checks if the current user has access to a specific skill course.
        /// </summary>
        [HttpGet("has-access/")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> HasAccessToCourse([FromQuery] int courseId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _purchaseService.HasUserPurchasedAsync(CurrentUserId!, PurchaseItemType.SkillsLibCourse, courseId);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "fetching enrolled users in skill course.");
                return this.UnexpectedError("fetching enrolled users in skill course.");
            }
        }

        /// <summary>
        /// Performs a soft delete on a skill course identified by its unique ID.
        /// Requires the user to have the delete permission.
        /// </summary>
        /// <param name="id">The unique identifier of the skill course to delete.</param>
        /// <returns>Returns an IActionResult indicating whether the deletion was successful. Returns 200 OK if successful, or 400 Bad Request if the operation fails.</returns>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(SkillCoursePermissions.Delete)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _deleteService.SoftDeleteCascadeAsync<Course>(id);
                return result.IsSuccess == true ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while soft deleting course with ID {Id}.", id);
                return this.UnexpectedError("soft deleting the entity.");
            }
        }
    }
}
