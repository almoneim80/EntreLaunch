namespace EntreLaunch.Controllers.EmailAPI
{
    [Authorize(Roles = "Admin")]
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    public class EmailGroEntreLaunchsController : BaseController<EmailGroup, EmailGroupCreateDto, EmailGroEntreLaunchEntreLaunchdateDto, EmailGroEntreLaunchDetailsDto, EmailGroEntreLaunchExportDto>
    {
        public EmailGroEntreLaunchsController(
            BaseService<EmailGroup, EmailGroupCreateDto, EmailGroEntreLaunchEntreLaunchdateDto, EmailGroEntreLaunchDetailsDto> service,
            ILocalizationManager? localization,
            ILogger<EmailGroEntreLaunchsController> logger,
            IExportService exportService)
            : base(service, localization, logger, exportService)
        {
        }
    }
}
