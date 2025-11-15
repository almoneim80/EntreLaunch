namespace EntreLaunch.Services.EmailSvc
{
    public class EmailWithLogService : IEmailWithLogService
    {
        private readonly IEmailService emailService;
        private readonly PgDbContext pgDbContext;
        public EmailWithLogService(IEmailService emailService, PgDbContext pgDbContext)
        {
            this.emailService = emailService;
            this.pgDbContext = pgDbContext;
        }

        /// <inheritdoc />
        public async Task<GeneralResult> SendAsync(string subject, string fromEmail, string fromName, string[] recipients, string body, List<AttachmentDto>? attachments, int templateId = 0)
        {
            var emailStatus = false;
            var emails = string.Join(";", recipients);
            string messageId = string.Empty;

            try
            {
                var result = await emailService.SendAsync(subject, fromEmail, fromName, recipients, body, attachments);
                if(string.IsNullOrEmpty(result.Data))
                {
                    emailStatus = false;
                    messageId = string.Empty;
                    return new GeneralResult(false, "Error accurred while sending email.");
                }

                messageId = result.Data!;
                emailStatus = true;
                Log.Information($"Email with subject {subject} sent to {recipients} from {fromEmail}");
                return new GeneralResult(true, "Email sent successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error occurred when sending email with subject {subject} to {emails} from {fromEmail}");
                return new GeneralResult(false, "Error accurred while sending email.");
            }
            finally
            {
                await AddEmailLogEntry(subject, fromEmail, body, emails, emailStatus, messageId, templateId: templateId);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> SendToContactAsync(int contactId, string subject, string fromEmail, string fromName, string body, List<AttachmentDto>? attachments, int scheduleId = 0, int templateId = 0)
        {
            var emailStatus = false;
            var recipient = string.Empty;
            string messageId = string.Empty;

            try
            {
                recipient = await GetContactEmailById(contactId);
                var recipientCollection = new[] { recipient };
                var result = await emailService.SendAsync(subject, fromEmail, fromName, recipientCollection, body, attachments);
                if(string.IsNullOrEmpty(result.Data))
                {
                    emailStatus = false;
                    messageId = string.Empty;
                    return new GeneralResult(false, result.Message!);
                }
                messageId = result.Data!;
                emailStatus = true;
                Log.Information($"Email with subject {subject} sent to {contactId} from {fromEmail}");
                return new GeneralResult(true, "Email sent successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error occurred when sending email with subject {subject} to {recipient} from {fromEmail}");
                return new GeneralResult(false, "Error accurred while sending email.");
            }
            finally
            {
                await AddEmailLogEntry(subject, fromEmail, body, recipient, emailStatus, messageId, contactId, scheduleId, templateId);
            }
        }


        /**************************PRIVATE METHODS*************************/

        /// <summary>
        /// Add a email log entry to the database table "EmailLogs".
        /// </summary>
        private async Task AddEmailLogEntry(string subject, string fromEmail, string body, string recipient, bool status, string messageId, int contactId = 0, int scheduleId = 0, int templateId = 0)
        {
            try
            {
                var log = new EmailLog();

                if (contactId > 0)
                {
                    log.ContactId = contactId;
                }

                if (scheduleId > 0)
                {
                    log.ScheduleId = scheduleId;
                }

                if (templateId > 0)
                {
                    log.TemplateId = templateId;
                }

                log.Subject = subject;
                log.FromEmail = fromEmail;
                log.HtmlBody = body;
                log.Recipients = recipient;
                log.Status = status ? EmailStatus.Sent : EmailStatus.NotSent;
                log.CreatedAt = DateTime.UtcNow;
                log.MessageId = messageId;

                await pgDbContext.EmailLogs!.AddAsync(log);
                await pgDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred when adding a email log entry.");
            }
        }

        /// <summary>
        /// Get contact email by id from the database table "Contacts".
        /// </summary>
        private async Task<string> GetContactEmailById(int contactId)
        {
            var contact = await pgDbContext.Contacts!.FirstOrDefaultAsync(x => x.Id == contactId);

            return contact!.Email;
        }
    }
}
