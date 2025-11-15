namespace EntreLaunch.Services.MyPartnerSvc
{
    public class MyPartnerService : IMyPartnerService
    {
        public IMyPartnerProjectService Projects { get; }
        public IMyPartnerFilteringService Filtering { get; }
        public IMyPartnerAttachmentService Attachments { get; }

        public MyPartnerService(
            IMyPartnerProjectService projectService,
            IMyPartnerFilteringService filteringService,
            IMyPartnerAttachmentService attachmentService)
        {
            Projects = projectService;
            Filtering = filteringService;
            Attachments = attachmentService;
        }
    }
}
