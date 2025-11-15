namespace EntreLaunch.Services.EmailSvc
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfig config = new EmailConfig();
        private readonly ILogger<EmailService> _logger;
        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            var settings = configuration.GetSection("Email").Get<EmailConfig>();

            if (settings != null)
            {
                config = settings;
            }
            else
            {
                // throw an exception if the email settings are not found
                throw new MissingConfigurationException($"The specified configuration section for the type {typeof(EmailConfig).FullName} could not be found in the settings file.");
            }
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<GeneralResult<string>> SendAsync(string subject, string fromEmail, string fromName, string[] recipients, string body, List<AttachmentDto>? attachments)
        {
            var client = new MailKit.Net.Smtp.SmtpClient();

            try
            {
                // Connect to the SMTP server
                await client.ConnectAsync(config.Server, config.Port, config.UseSsl);

                // Authentication with the server
                await client.AuthenticateAsync(new NetworkCredential(config.UserName, config.Password));

                // Creating an email message
                var message = await GenerateEmailBody(subject, fromEmail, fromName, recipients, body, attachments);

                // Send Message
                await client.SendAsync(message);

                _logger.LogInformation("Email sent successfully.");
                return new GeneralResult<string>(true, "Email sent successfully", message.MessageId);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error sending email.");

                switch (exception)
                {
                    case AuthenticationException:
                    case ServiceNotAuthenticatedException:
                        _logger.LogError(exception, "Error authenticating with smtp host");
                        return new GeneralResult<string>(false, "Error authenticating with smtp host", null);
                    case ServiceNotConnectedException:
                        _logger.LogError(exception, "Error connecting to smtp host");
                        return new GeneralResult<string>(false, "Error connecting to smtp host", null);
                    default:
                        _logger.LogError(exception, "Error sending email");
                        return new GeneralResult<string>(false, "Error sending email", null);
                }
            }
            finally
            {
                // Disconnect and release resources
                client.Disconnect(true);
                client.Dispose();
            }
        }

        /// <summary>
        /// Generates the email body. 
        /// </summary>
        private static async Task<MimeMessage> GenerateEmailBody(string subject, string fromEmail, string fromName, string[] recipients, string body, List<AttachmentDto>? attachments)
        {
            var message = new MimeMessage();
            message.Subject = subject;
            message.From.Add(new MailboxAddress(fromName, fromEmail));
            foreach (var receipent in recipients)
            {
                message.To.Add(MailboxAddress.Parse(receipent));
            }

            var emailBody = new BodyBuilder()
            {
                HtmlBody = body,
            };

            if (attachments is not null)
            {
                foreach (var attachment in attachments)
                {
                    using (var stream = new MemoryStream(attachment.File))
                    {
                        await emailBody.Attachments.AddAsync(attachment.FileName, stream);
                    }
                }
            }

            message.Body = emailBody.ToMessageBody();

            return message;
        }
    }
}
