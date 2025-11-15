using PhoneNumbers;
namespace EntreLaunch.Plugin.Sms.Services
{
    public class SmsService : ISmsService
    {
        private readonly PluginConfig _pluginSettings = new PluginConfig();
        private readonly Dictionary<string, ISmsService> _countrySmsServices = new Dictionary<string, ISmsService>();
        private readonly PgDbContext _dbContext;
        private readonly PhoneNumberUtil _phoneNumberUtil = PhoneNumberUtil.GetInstance();

        public SmsService(IConfiguration configuration, PgDbContext dbContext)
        {
            var settings = configuration.Get<PluginConfig>();

            if (settings != null)
            {
                _pluginSettings = settings;
                InitGateways();
            }
            _dbContext = dbContext;
        }

        /// <summary>
        /// Send an SMS message to the selected recipient.
        /// </summary>
        public Task SendAsync(string recipient, string message)
        {
            var smsService = GetSmsService(recipient);

            if (smsService == null)
            {
                throw new UnknownCountryCodeException();
            }

            return smsService.SendAsync(recipient, message);
        }

        /// <summary>
        /// Gets the sender ID used for the specified recipient.
        /// </summary>
        public string GetSender(string recipient)
        {
            var smsService = GetSmsService(recipient);

            return smsService != null ? smsService.GetSender(recipient) : string.Empty;
        }

        /// <summary>
        /// Add SMS to the database.
        /// </summary>
        public async Task<AddSmsResult> AddSmsToDb(CreateSmsDto create)
        {
            if (create == null)
            {
                return new AddSmsResult
                {
                    IsSuccess = false,
                    Message = "CreateSmsDto is null.",
                };
            }

            var smsLog = new SmsLog
            {
                Sender = create.Sender ?? string.Empty,
                Recipient = create.Recipient ?? string.Empty,
                Message = create.Message ?? string.Empty,
                Status = SmsSendStatus.NotSent,
                CreatedAt = DateTime.UtcNow,
                Source = "EntreLaunch",
                IsDeleted = false,
            };

            _dbContext.SmsLogs.Add(smsLog);
            var result = await _dbContext.SaveChangesAsync();

            if (result == 0)
            {
                return new AddSmsResult
                {
                    IsSuccess = false,
                    Message = "Failed to add SMS to database.",
                };
            }
            else
            {
                return new AddSmsResult
                {
                    IsSuccess = true,
                    Message = "SMS with recipient " + create.Recipient + " added successfully.",
                    SmsId = smsLog.Id
                };
            }
        }

        /// <summary>
        /// Get all SMS.
        /// </summary>
        public async Task<List<SmsLog>> GetAllSms()
        {
            var smsLogs = await _dbContext.SmsLogs.Where(s => s.IsDeleted == false).ToListAsync();

            if (smsLogs.Count == 0)
            {
                throw new SmsPluginException("SMS data not found.");
            }
            else
            {
                return smsLogs;
            }
        }

        /// <summary>
        /// Get one SMS by ID.
        /// </summary>
        public async Task<SmsLog> GetSmsById(int id)
        {
            var smsLog = await _dbContext.SmsLogs.FindAsync(id);

            if (smsLog == null)
            {
                throw new SmsPluginException("SMS not found.");
            }

            return smsLog;
        }

        /// <summary>
        /// Get SMS service based on recipient's country code.
        /// </summary>
        private ISmsService? GetSmsService(string recipient)
        {
            var key = _countrySmsServices.Keys.FirstOrDefault(key => recipient.StartsWith(key));

            if (key != null)
            {
                return _countrySmsServices[key];
            }

            if (_countrySmsServices.TryGetValue("default", out var smsService))
            {
                return smsService;
            }

            return null;
        }

        /// <summary>
        /// Initialize SMS gateways.
        /// </summary>
        private void InitGateways()
        {
            foreach (var countryGateway in _pluginSettings.SmsCountryGateways)
            {
                ISmsService? gatewayService = null;

                var gatewayName = countryGateway.Gateway;

                switch (gatewayName)
                {
                    case "Msegat":
                        gatewayService = new MsegatSmsService(_pluginSettings.SmsGateways.Msegat);
                        break;
                    case "Twilio":
                        gatewayService = new TwilioSmsService(_pluginSettings.SmsGateways.Twilio);
                        break;
                }

                if (gatewayService != null)
                {
                    _countrySmsServices[countryGateway.Code] = gatewayService;
                }
            }
        }

        /// <summary>
        /// EntreLaunchdate SMS status to "Sent".
        /// </summary>
        public async Task SuccessSent(int? smsLogId)
        {
            if (smsLogId == null)
            {
                throw new ArgumentNullException(nameof(smsLogId));
            }

            var smsLog = await _dbContext.SmsLogs.FindAsync(smsLogId);

            if (smsLog == null)
            {
                throw new SmsPluginException($"SMS log with ID {smsLogId} not found.");
            }

            smsLog.Status = SmsSendStatus.Sent;

            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Validates and formats a phone number using PhoneNumbers library.
        /// </summary>
        public string ValidateAndFormatPhoneNumber(string phoneNumber)
        {
            var parsedNumber = _phoneNumberUtil.Parse(phoneNumber, string.Empty);
            return _phoneNumberUtil.Format(parsedNumber, PhoneNumberFormat.E164);
        }

        /// <summary>
        /// Replaces placeholders in SMS template.
        /// </summary>
        public string ReplacePlaceholders(string templateContent, Dictionary<string, string> placeholders)
        {
            foreach (var placeholder in placeholders)
            {
                templateContent = templateContent.Replace($"{{{placeholder.Key}}}", placeholder.Value);
            }
            return templateContent;
        }
    }
}
