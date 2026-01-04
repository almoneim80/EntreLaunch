namespace EntreLaunch.Controllers.EmailAPI
{
    [Authorize(Roles = AppRoles.AllAdmins)]
    [Route("api/[controller]")]
    public class EmailTemplatesController(
        BaseService<EmailTemplate, EmailTemplateCreateDto, EmailTemplateUpdateDto, EmailTemplateDetailsDto> service,
        ILocalizationManager? localization,
        ILogger<EmailTemplatesController> logger,
        IExportService exportService) : BaseController<EmailTemplate, EmailTemplateCreateDto, EmailTemplateUpdateDto, EmailTemplateDetailsDto, EmailTemplateExportDto>(service, localization, logger, exportService)
    {
    }
}
