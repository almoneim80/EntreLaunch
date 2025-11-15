namespace EntreLaunch.Controllers.FortuneWheelAPI
{
    [Authorize(Roles = "Admin, Entrepreneur")]
    [Route("api/[controller]")]
    public class WheelPlayerController : BaseController<WheelPlayer, WheelPlayerCreateDto, WheelPlayerEntreLaunchdateDto, WheelPlayerDetailsDto, WheelPlayerExportDto>
    {
        private readonly IExtendedBaseService _extendedBaseService;
        private readonly ILogger<WheelPlayerController> _logger;
        public WheelPlayerController(
            BaseService<WheelPlayer, WheelPlayerCreateDto, WheelPlayerEntreLaunchdateDto, WheelPlayerDetailsDto> service,
            ILocalizationManager? localization,
            ILogger<WheelPlayerController> logger,
            IExtendedBaseService extendedBaseService,
            IExportService exportService)
            : base(service, localization, logger, exportService)
        {
            _extendedBaseService = extendedBaseService;
            _logger = logger;
        }

        /// <summary>
        /// Get all WheelPlayers.
        /// </summary>
        [HttpGet("GetAll")]
        [RequiredPermission(Permissions.WheelPlayerPermissions.ShowAll)]
        public override async Task<ActionResult<WheelPlayerDetailsDto[]>> GetAll()
        {
            try
            {
                return await base.GetAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new GeneralResult<WheelPlayerDetailsDto[]>(false, "UnExpected error occurred while retrieving wheel players.", null));
            }
        }

        /// <summary>
        /// Get one WheelPlayer.
        /// </summary>
        [HttpGet("GetOne/{id}")]
        [RequiredPermission(Permissions.WheelPlayerPermissions.ShowOne)]
        public override async Task<ActionResult<WheelPlayerDetailsDto>> GetOne(int id)
        {
            try
            {
                return await base.GetOne(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new GeneralResult<WheelPlayerDetailsDto>(false, "UnExpected error occurred while retrieving wheel player.", null));
            }
        }

        /// <summary>
        /// Create new WheelPlayer.
        /// </summary>
        [HttpPost]
        [RequiredPermission(Permissions.WheelPlayerPermissions.Create)]
        public override async Task<ActionResult<WheelPlayerDetailsDto>> Create([FromBody] WheelPlayerCreateDto createDto)
        {
            try
            {
                var isReferencedValid = await _extendedBaseService.IsEntityExistsAndNotDeletedAsync<WheelAward>(createDto.AwardId);
                if (isReferencedValid.IsSuccess == false)
                {
                    return BadRequest(new { Message = "The referenced Entity does not exist or has been deleted." });
                }

                return await base.Create(createDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new GeneralResult<WheelPlayerDetailsDto>(false, "UnExpected error occurred while creating wheel player.", null));
            }
        }

        /// <summary>
        /// EntreLaunchdate an existing WheelPlayer.
        /// </summary>
        [HttpPatch("{id}")]
        [RequiredPermission(Permissions.WheelPlayerPermissions.Edit)]
        public override async Task<ActionResult<WheelPlayerDetailsDto>> Patch(int id, [FromBody] WheelPlayerEntreLaunchdateDto EntreLaunchdateDto)
        {
            try
            {
                var isReferencedValid = await _extendedBaseService.IsEntityExistsAndNotDeletedAsync<WheelAward>(EntreLaunchdateDto.AwardId ?? 0);
                if (isReferencedValid.IsSuccess == false)
                {
                    return BadRequest(new { Message = "The referenced Entity does not exist or has been deleted." });
                }

                return await base.Patch(id, EntreLaunchdateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new GeneralResult<WheelPlayerDetailsDto>(false, "UnExpected error occurred while EntreLaunchdating wheel player.", null));
            }
        }

        /// <summary>
        /// Export WheelPlayers to CSV file.
        /// </summary>
        [HttpGet("CSVExport")]
        [RequiredPermission(Permissions.WheelPlayerPermissions.Export)]
        public override async Task<IActionResult> ExportToCsv()
        {
            try
            {
                return await base.ExportToCsv();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new GeneralResult<WheelPlayerDetailsDto[]>(false, "UnExpected error occurred while exporting wheel players.", null));
            }
        }

        /// <summary>
        /// Export WheelPlayers to Excel file.
        /// </summary>
        [HttpGet("ExcelExport")]
        [RequiredPermission(Permissions.WheelPlayerPermissions.Export)]
        public override async Task<IActionResult> ExportToExcel()
        {
            try
            {
                return await base.ExportToExcel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new GeneralResult<WheelPlayerDetailsDto[]>(false, "UnExpected error occurred while exporting wheel players.", null));
            }
        }

        /// <summary>
        /// Export WheelPlayers to JSON file.
        /// </summary>
        [HttpGet("JSONExport")]
        [RequiredPermission(Permissions.WheelPlayerPermissions.Export)]
        public override async Task<IActionResult> ExportToJson()
        {
            try
            {
                return await base.ExportToJson();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new GeneralResult<WheelPlayerDetailsDto[]>(false, "UnExpected error occurred while exporting wheel players.", null));
            }
        }

        /// <summary>
        /// Delete WheelPlayer.
        /// </summary>
        [HttpDelete("{id}")]
        [RequiredPermission(Permissions.WheelPlayerPermissions.Delete)]
        public override async Task<ActionResult> Delete(int id)
        {
            try
            {
                return await base.Delete(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new GeneralResult<WheelPlayerDetailsDto>(false, "UnExpected error occurred while deleting wheel player.", null));
            }
        }
    }
}

