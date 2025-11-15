namespace EntreLaunch.Services.EmailSvc
{
    public class EmailSchedulingService : IEmailSchedulingService
    {
        private readonly IOptions<ApiSettingsConfig> _apiSettingsConfig;
        private PgDbContext _dbContext;
        private readonly ILogger<EmailSchedulingService> _logger;
        public EmailSchedulingService(PgDbContext dbContext, IOptions<ApiSettingsConfig> apiSettingsConfig, ILogger<EmailSchedulingService> logger)
        {
            _dbContext = dbContext;
            _apiSettingsConfig = apiSettingsConfig;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<GeneralResult<EmailSchedule>> FindByGroEntreLaunchAndLanguage(string groEntreLaunchName, string languageCode)
        {
            try
            {
                EmailSchedule? result;

                // Check if Contact.Language is in two-letter format and adjust the query accordingly
                // Basic query to search for email tables
                var emailSchedulesQuery = _dbContext.EmailSchedules!
                    .Include(c => c.GroEntreLaunch)
                    .Where(e => e.GroEntreLaunch!.Name == groEntreLaunchName);

                // Dealing with two-character language codes
                if (languageCode.Length == 2)
                {
                    result = await emailSchedulesQuery.FirstOrDefaultAsync(e => e.GroEntreLaunch!.Language.StartsWith(languageCode));
                }
                else
                {
                    // Find an exact match first
                    result = await emailSchedulesQuery.FirstOrDefaultAsync(e => e.GroEntreLaunch!.Language == languageCode);

                    // If no exact match is found, try searching using the first part of the language code
                    if (result == null)
                    {
                        var lang = languageCode.Split('-')[0];
                        result = await emailSchedulesQuery.FirstOrDefaultAsync(e => e.GroEntreLaunch!.Language.StartsWith(lang));
                    }
                }

                // If no result is found, use the default language
                if (result == null)
                {
                    result = await emailSchedulesQuery.FirstOrDefaultAsync(e => e.GroEntreLaunch!.Language == _apiSettingsConfig.Value.DefaultLanguage);
                }

                return new GeneralResult<EmailSchedule>(true, "Email schedule found", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to find email schedule");
                return new GeneralResult<EmailSchedule>(false, "Failed to find email schedule", null);
            }
        }

        /// <inheritdoc />
        public void SetDBContext(PgDbContext pgDbContext)
        {
            _dbContext = pgDbContext;
        }
    }
}
