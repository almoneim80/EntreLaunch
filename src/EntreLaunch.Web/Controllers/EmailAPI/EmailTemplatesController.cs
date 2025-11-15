namespace EntreLaunch.Controllers.EmailAPI
{
    [Authorize(Roles = "Admin")]
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    public class EmailTemplatesController : BaseController<EmailTemplate, EmailTemplateCreateDto, EmailTemplateEntreLaunchdateDto, EmailTemplateDetailsDto, EmailTemplateExportDto>
    {
        public EmailTemplatesController(
            BaseService<EmailTemplate, EmailTemplateCreateDto, EmailTemplateEntreLaunchdateDto, EmailTemplateDetailsDto> service,
            ILocalizationManager? localization,
            ILogger<EmailTemplatesController> logger,
            IExportService exportService)
            : base(service, localization, logger, exportService)
        {
        }
    }
}
