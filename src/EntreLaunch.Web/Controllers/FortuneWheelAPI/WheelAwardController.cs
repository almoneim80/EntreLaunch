namespace EntreLaunch.Controllers.FortuneWheelAPI
{
    [Authorize(Roles = "Admin, Entrepreneur")]
    [Route("api/[controller]")]
    public class WheelAwardController : BaseController<WheelAward, WheelAwardCreateDto, WheelAwardEntreLaunchdateDto, WheelAwardDetailsDto, WheelAwardExportDto>
    {
        private readonly IImportService<WheelAward, WheelAwardImportDto> _importService;
        private readonly ILogger<WheelAwardController> _logger;
        public WheelAwardController(
            BaseService<WheelAward, WheelAwardCreateDto, WheelAwardEntreLaunchdateDto, WheelAwardDetailsDto> service,
            ILocalizationManager? localization,
            ILogger<WheelAwardController> logger,
            IImportService<WheelAward, WheelAwardImportDto> importService,
            IExportService exportService)
            : base(service, localization, logger, exportService)
        {
            _importService = importService;
            _logger = logger;
        }

        /// <summary>
        /// Get all WheelAwards.
        /// </summary>
        [HttpGet("GetAll")]
        [RequiredPermission(Permissions.WheelAwardPermissions.ShowAll)]
        public override async Task<ActionResult<WheelAwardDetailsDto[]>> GetAll()
        {
            try
            {
                return await base.GetAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new GeneralResult<WheelAwardDetailsDto[]>(false, "UnExpected error occurred while retrieving wheel awards.", null));
            }
        }

        /// <summary>
        /// Get one WheelAward.
        /// </summary>
        [HttpGet("GetOne/{id}")]
        [RequiredPermission(Permissions.WheelAwardPermissions.ShowOne)]
        public override async Task<ActionResult<WheelAwardDetailsDto>> GetOne(int id)
        {
            try
            {
                return await base.GetOne(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new GeneralResult<WheelAwardDetailsDto>(false, "UnExpected error occurred while retrieving wheel award.", null));
            }
        }

        /// <summary>
        /// Create new WheelAward.
        /// </summary>
        [HttpPost]
        [RequiredPermission(Permissions.WheelAwardPermissions.Create)]
        public override async Task<ActionResult<WheelAwardDetailsDto>> Create([FromBody] WheelAwardCreateDto createDto)
        {
            try
            {
                return await base.Create(createDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new GeneralResult<WheelAwardDetailsDto>(false, "UnExpected error occurred while creating wheel award.", null));
            }
        }

        /// <summary>
        /// EntreLaunchdate an existing WheelAward.
        /// </summary>
        [HttpPatch("{id}")]
        [RequiredPermission(Permissions.WheelAwardPermissions.Edit)]
        public override async Task<ActionResult<WheelAwardDetailsDto>> Patch(int id, [FromBody] WheelAwardEntreLaunchdateDto EntreLaunchdateDto)
        {
            try
            {
                return await base.Patch(id, EntreLaunchdateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new GeneralResult<WheelAwardDetailsDto>(false, "UnExpected error occurred while EntreLaunchdating wheel award.", null));
            }
        }

        /// <summary>
        /// Export all WheelAward to CSV file.
        /// </summary>
        [HttpGet("CSVExport")]
        [RequiredPermission(Permissions.WheelAwardPermissions.Export)]
        public override async Task<IActionResult> ExportToCsv()
        {
            try
            {
                return await base.ExportToCsv();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new GeneralResult<WheelAwardDetailsDto[]>(false, "UnExpected error occurred while exporting wheel awards.", null));
            }
        }

        /// <summary>
        /// Export all WheelAward to Excel file.
        /// </summary>
        [HttpGet("ExcelExport")]
        [RequiredPermission(Permissions.WheelAwardPermissions.Export)]
        public override async Task<IActionResult> ExportToExcel()
        {
            try
            {
                return await base.ExportToExcel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new GeneralResult<WheelAwardDetailsDto[]>(false, "UnExpected error occurred while exporting wheel awards.", null));
            }
        }

        /// <summary>
        /// Export all WheelAward to JSON file.
        /// </summary>
        [HttpGet("JSONExport")]
        [RequiredPermission(Permissions.WheelAwardPermissions.Export)]
        public override async Task<IActionResult> ExportToJson()
        {
            try
            {
                return await base.ExportToJson();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new GeneralResult<WheelAwardDetailsDto[]>(false, "UnExpected error occurred while exporting wheel awards.", null));
            }
        }

        /// <summary>
        /// Delete an existing WheelAward.
        /// </summary>
        [HttpDelete("{id}")]
        [RequiredPermission(Permissions.WheelAwardPermissions.Delete)]
        public override async Task<ActionResult> Delete(int id)
        {
            try
            {
                return await base.Delete(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new GeneralResult<WheelAwardDetailsDto>(false, "UnExpected error occurred while deleting wheel award.", null));
            }
        }
    }
}

