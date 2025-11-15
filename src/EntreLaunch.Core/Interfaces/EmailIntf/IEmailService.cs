namespace EntreLaunch.Interfaces.EmailIntf
{
    public interface IEmailService
    {
        /// <summary>
        /// Sends an email with the specified details.
        /// </summary>
        Task<GeneralResult<string>> SendAsync(
            string subject,
            string fromEmail,
            string fromName,
            string[] recipients,
            string body,
            List<AttachmentDto>? attachments);
    }
}
