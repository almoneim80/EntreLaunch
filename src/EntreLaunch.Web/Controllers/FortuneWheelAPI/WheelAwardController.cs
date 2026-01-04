namespace EntreLaunch.Controllers.FortuneWheelAPI
{
    [Authorize(Roles = AppRoles.WheelRoles)]
    [Route("api/[controller]")]
    public class WheelAwardController(
        BaseService<WheelAward, WheelAwardCreateDto, WheelAwardUpdateDto, WheelAwardDetailsDto> service,
        ILocalizationManager? localization,
        IExtendedBaseService extendedBaseService,
        ILogger<WheelAwardController> logger,
        IExportService exportService) : BaseController<WheelAward, WheelAwardCreateDto, WheelAwardUpdateDto, WheelAwardDetailsDto, WheelAwardExportDto>(service, localization, logger, exportService)
    {
        private readonly ILogger<WheelAwardController> _logger = logger;

        /// <summary>
        /// Creates a new WheelAward entity based on the provided data.
        /// </summary>
        /// <param name="createDto">An object containing the required information to create a WheelAward.</param>
        /// <returns>
        /// Returns the created WheelAward details if successful; otherwise, returns an appropriate error response.
        /// </returns>
        [HttpPost("create")]
        [RequiredPermission(WheelAwardPermissions.Create)]  
        public override async Task<ActionResult<WheelAwardDetailsDto>> Create([FromBody] WheelAwardCreateDto createDto)
        {
            try
            {
                return await base.Create(createDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.UnexpectedError("creating wheel award");
            }
        }

        /// <summary>
        /// Updates an existing WheelAward with the specified ID.
        /// </summary>
        /// <param name="id">The unique identifier of the WheelAward to update.</param>
        /// <param name="updateDto">An object containing the updated WheelAward data.</param>
        /// <returns>
        /// Returns the updated WheelAward details if the operation succeeds; otherwise, returns an error response.
        /// </returns>
        [HttpPatch("edit/{id}")]
        [RequiredPermission(WheelAwardPermissions.Edit)]
        public override async Task<ActionResult<WheelAwardDetailsDto>> Patch([FromRoute] int id, [FromBody] WheelAwardUpdateDto updateDto)
        {
            try
            {
                return await base.Patch(id, updateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.UnexpectedError("updating wheel award");
            }
        }

        /// <summary>
        /// Retrieves a list of all WheelAwards available in the system.
        /// </summary>
        /// <returns>
        /// Returns an array of WheelAward details; returns an error response if the operation fails.
        /// </returns>
        [HttpGet("all")]
        [RequiredPermission(WheelAwardPermissions.GetAll)]
        public override async Task<ActionResult<GeneralResult<PaginatedResult<WheelAwardDetailsDto>>>> GetAll([FromQuery] PaginationParams pagination)
        {
            try
            {
                return await base.GetAll(pagination);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.UnexpectedError("getting wheel awards");
            }
        }

        /// <summary>
        /// Retrieves the details of a specific WheelAward by its unique ID.
        /// </summary>
        /// <param name="id">The unique identifier of the WheelAward to retrieve.</param>
        /// <returns>
        /// Returns the WheelAward details if found; otherwise, returns an appropriate error response.
        /// </returns>
        [HttpGet("get-one/{id}")]
        [RequiredPermission(WheelAwardPermissions.GetOne)]
        public override async Task<ActionResult<WheelAwardDetailsDto>> GetOne([FromRoute] int id)
        {
            try
            {
                return await base.GetOne(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.UnexpectedError("getting wheel award");
            }
        }

        /// <summary>
        /// Retrieves all available award types defined in the AwardType enumeration.
        /// </summary>
        /// <returns>
        /// Returns a collection of award type metadata; returns Unauthorized if the user is not authenticated.
        /// </returns>
        [HttpGet("types")]
        public ActionResult<IEnumerable<EnumData>> GetAwardTypes()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var enumValues = extendedBaseService.GetEnumValues<AwardType>();
                return Ok(enumValues);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting award types.");
                return this.UnexpectedError("getting award types.");
            }
        }

        /// <summary>
        /// Deletes an existing WheelAward specified by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the WheelAward to delete.</param>
        /// <returns>
        /// Returns HTTP 200 if the deletion succeeds; otherwise, returns an error response.
        /// </returns>
        [HttpDelete("delete/{id}")]
        [RequiredPermission(WheelAwardPermissions.Delete)]
        public override async Task<ActionResult> Delete([FromRoute] int id)
        {
            try
            {
                return await base.Delete(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.UnexpectedError("delete wheel award");
            }
        }
    }
}

