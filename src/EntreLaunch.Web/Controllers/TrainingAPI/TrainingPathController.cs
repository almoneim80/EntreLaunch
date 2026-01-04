namespace EntreLaunch.Web.Controllers.TrainingAPI
{
    [Authorize(Roles = AppRoles.TrainingPathRoles)]
    [Route("api/[controller]")]
    public class TrainingPathController(
        BaseService<TrainingPath, TrainingPathCreateDto, TrainingPathUpdateDto, TrainingPathDetailsDto> service,
        ILocalizationManager? localization, ILogger<TrainingPathController> logger,
        ITrainingPathService trainingPathService,
        IExportService exportService) : BaseController<TrainingPath, TrainingPathCreateDto, TrainingPathUpdateDto, TrainingPathDetailsDto, TrainingPathExportDto>(service, localization, logger, exportService)
    {
        private readonly ILogger<TrainingPathController> _logger = logger;

        /// <summary>
        /// Creates a new TrainingPath entity using the provided data.
        /// </summary>
        /// <param name="createDto">The data transfer object containing information needed to create a new training path.</param>
        /// <returns>
        /// Returns the details of the newly created TrainingPath on success; otherwise, returns an error describing the failure.
        /// </returns>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(TrainingPathPermissions.Create)]
        public override async Task<ActionResult<TrainingPathDetailsDto>> Create([FromBody] TrainingPathCreateDto createDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                return await base.Create(createDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UnExpected error occured while create training path in Create method.");
                return StatusCode(StatusCodes.Status500InternalServerError, new GeneralResult { IsSuccess = false, Message = "UnExpected error occured while create training path" });
            }
        }

        /// <summary>
        /// Updates an existing TrainingPath entity.
        /// </summary>
        /// <param name="id">The unique identifier of the TrainingPath to update.</param>
        /// <param name="updateDto">The data transfer object containing updated information for the TrainingPath.</param>
        /// <returns>
        /// Returns the updated TrainingPath details on success; otherwise returns appropriate error information.
        /// </returns>
        [HttpPatch("edit/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(TrainingPathPermissions.Edit)]
        public override async Task<ActionResult<TrainingPathDetailsDto>> Patch([FromRoute] int id, [FromBody] TrainingPathUpdateDto updateDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                return await base.Patch(id, updateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating TrainingPath.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error occurred while updating TrainingPath." });
            }
        }

        /// <summary>
        /// Retrieves all existing TrainingPath records in the system.
        /// </summary>
        /// <returns>
        /// Returns a list of all TrainingPathDetailsDto if retrieval is successful; otherwise returns a suitable error.
        /// </returns>
        [HttpGet("all-with-courses")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(TrainingPathPermissions.GetAll)]
        public async Task<ActionResult> GetAllTrainingPathsWithCourses([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var result = await trainingPathService.GetAllTrainingPathsWithCoursesAsync(pagination, cancellationToken);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all TrainingPaths.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error occurred while getting all TrainingPaths." });
            }
        }

        /// <summary>
        /// Retrieves a single TrainingPath record by its identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the TrainingPath to retrieve.</param>
        /// <returns>
        /// Returns the corresponding TrainingPathDetailsDto if found; otherwise returns 404 or other relevant error.
        /// </returns>
        [HttpGet("get-one/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(TrainingPathPermissions.GetOne)]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var result = await trainingPathService.GetTrainingPathWithCoursesByIdAsync(id);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting one TrainingPath.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error occurred while getting one TrainingPath." });
            }
        }

        /// <summary>
        /// Deletes an existing TrainingPath identified by its ID.
        /// </summary>
        /// <param name="id">The identifier of the TrainingPath to delete.</param>
        /// <returns>
        /// Returns a 204 No Content if deletion is successful, or an error describing the failure condition.
        /// </returns>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(TrainingPathPermissions.Delete)]
        public override async Task<ActionResult> Delete([FromRoute] int id)
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
                _logger.LogError(ex, "Error occurred while deleting TrainingPath.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error occurred while deleting TrainingPath." });
            }
        }

        #region deprecated
        [NonAction]
        public override async Task<ActionResult<TrainingPathDetailsDto>> GetOne(int id)
        {
            return await base.GetOne(id);
        }

        [NonAction]
        public override async Task<ActionResult<GeneralResult<PaginatedResult<TrainingPathDetailsDto>>>> GetAll([FromQuery] PaginationParams pagination)
        {
            return await _service.GetAllAsync(pagination);
        }

         
        [NonAction]
        public override async Task<IActionResult> ExportToCsv()
        {
            return await Task.FromResult((IActionResult)NoContent());
        }

        [NonAction]
        public override async Task<IActionResult> ExportToExcel()
        {
            return await Task.FromResult((IActionResult)NoContent());
        }

        [NonAction]
        public override async Task<IActionResult> ExportToJson()
        {
            return await Task.FromResult((IActionResult)NoContent());
        }
        #endregion
    }
}

