namespace EntreLaunch.Interfaces.MyPartnerIntf
{
    public interface IMyPartnerAttachmentService
    {
        /// <summary>
        /// Get attachments of project by project id.
        /// </summary>
        Task<GeneralResult> GetProjectAttachments(int id);

        /// <summary>
        /// EntreLaunchdate attachments.
        /// </summary>>
        Task<GeneralResult> EntreLaunchdateAttachments(int id, ProjectAttachmentEntreLaunchdateDto EntreLaunchdateDto);
    }
}
