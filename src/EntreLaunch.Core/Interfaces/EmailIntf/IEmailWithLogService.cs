namespace EntreLaunch.Interfaces.EmailIntf
{
    public interface IEmailWithLogService
    {
        /// <summary>
        /// Sends an email with the specified details.
        /// </summary>
        Task<GeneralResult> SendAsync(string subject, string fromEmail, string fromName, string[] recipients, string body, List<AttachmentDto>? attachments, int templateId = 0);

        /// <summary>
        /// Sends an email with the specified details to a specific contact.
        /// </summary>
        Task<GeneralResult> SendToContactAsync(int contactId, string subject, string fromEmail, string fromName, string body, List<AttachmentDto>? attachments, int scheduleId = 0, int templateId = 0);
    }
}
