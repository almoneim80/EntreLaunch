namespace EntreLaunch.Controllers.EmailAPI
{
    [Authorize(Roles = AppRoles.AllAdmins)]
    [Route("api/[controller]")]
    public class EmailGroupsController(
        BaseService<EmailGroup, EmailGroupCreateDto, EmailGroupUpdateDto, EmailGroupDetailsDto> service,
        ILocalizationManager? localization,
        ILogger<EmailGroupsController> logger,
        IExportService exportService) : BaseController<EmailGroup, EmailGroupCreateDto, EmailGroupUpdateDto, EmailGroupDetailsDto, EmailGroupExportDto>(service, localization, logger, exportService)
    {
    }
}
