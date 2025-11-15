namespace EntreLaunch.Interfaces.EmailIntf
{
    public interface IEmailFromTemplateService
    {
        /// <summary>
        /// Send an email using a template.
        /// </summary>
        Task SendAsync(string templateName, string language, string[] recipients, Dictionary<string, string>? templateArguments, List<AttachmentDto>? attachments);

        /// <summary>
        /// Send an email to a contact using a template.
        /// </summary>
        Task SendToContactAsync(int contactId, string templateName, Dictionary<string, string>? templateArguments, List<AttachmentDto>? attachments, int scheduleId = 0);
    }
}
