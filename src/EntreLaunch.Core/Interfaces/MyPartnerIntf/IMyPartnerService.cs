namespace EntreLaunch.Interfaces.MyPartnerIntf
{
    public interface IMyPartnerService
    {
        IMyPartnerProjectService Projects { get; }
        IMyPartnerAttachmentService Attachments { get; }
        IMyPartnerFilteringService Filtering { get; }
    }
}
