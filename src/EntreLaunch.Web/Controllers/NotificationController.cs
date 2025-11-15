namespace EntreLaunch.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ValidateModelState]
    public class NotificationController : BaseController<Notification, NotificationCreateDto, NotificationEntreLaunchdateDto, NotificationDetailsDto, NotificationExportDto>
    {
        private readonly IExtendedBaseService _extendedBaseService;
        public NotificationController(
            BaseService<Notification, NotificationCreateDto, NotificationEntreLaunchdateDto, NotificationDetailsDto> service,
            ILocalizationManager? localization,
            ILogger<NotificationController> logger,
            IExtendedBaseService extendedBaseService,
            IExportService exportService)
            : base(service, localization, logger, exportService)
        {
            _extendedBaseService = extendedBaseService;
        }

        /// <summary>
        /// Create new Notification.
        /// </summary>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(Permissions.NotificationPermissions.Create)]
        public override async Task<ActionResult<NotificationDetailsDto>> Create([FromBody] NotificationCreateDto createDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not logged in.");
            }

            return await base.Create(createDto);
        }

        /// <summary>
        /// EntreLaunchdate Notification.
        /// </summary>
        [HttpPatch("edit/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(Permissions.NotificationPermissions.Edit)]
        public override async Task<ActionResult<NotificationDetailsDto>> Patch(int id, [FromBody] NotificationEntreLaunchdateDto EntreLaunchdateDto)
        {
            return await base.Patch(id, EntreLaunchdateDto);
        }


        /// <summary>
        /// Get all Notifications.
        /// </summary>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(Permissions.NotificationPermissions.ShowAll)]
        public override async Task<ActionResult<NotificationDetailsDto[]>> GetAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not logged in.");
            }

            return await base.GetAll();
        }

        /// <summary>
        /// Get one Notification.
        /// </summary>
        [HttpGet("get-one/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(Permissions.NotificationPermissions.ShowOne)]
        public override async Task<ActionResult<NotificationDetailsDto>> GetOne(int id)
        {
            return await base.GetOne(id);
        }

        /// <summary>
        /// Export Notifications to CSV file.
        /// </summary>
        [HttpGet("export/csv")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(Permissions.NotificationPermissions.Export)]
        public override async Task<IActionResult> ExportToCsv()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not logged in.");
            }

            return await base.ExportToCsv();
        }

        /// <summary>
        /// Export Notifications to Excel file.
        /// </summary>
        [HttpGet("export/excel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(Permissions.NotificationPermissions.Export)]
        public override async Task<IActionResult> ExportToExcel()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not logged in.");
            }

            return await base.ExportToExcel();
        }

        /// <summary>
        /// Export Notifications to JSON file.
        /// </summary>
        [HttpGet("export/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(NotificationPermissions.Export)]
        public override async Task<IActionResult> ExportToJson()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not logged in.");
            }

            return await base.ExportToJson();
        }

        /// <summary>
        /// Get all Notification Type.
        /// </summary>
        [HttpGet("types")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(NotificationPermissions.GetEnumValues)]
        public ActionResult<IEnumerable<EnumData>> GetNotificationType()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not logged in.");
            }

            var enumValues = _extendedBaseService.GetEnumValues<NotificationType>();
            return Ok(enumValues);
        }

        /// <summary>
        /// Delete Notification.
        /// </summary>
        [HttpDelete("dlete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(Permissions.NotificationPermissions.Delete)]
        public override async Task<ActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not logged in.");
            }

            return await base.Delete(id);
        }
    }
}

