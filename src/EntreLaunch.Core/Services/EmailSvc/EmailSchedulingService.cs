using EntreLaunch.DTOs.TrainingDtos;
using EntreLaunch.Interfaces;

namespace EntreLaunch.Services.EmailSvc
{
    public class EmailSchedulingService(PgDbContext dbContext, IOptions<ApiSettingsConfig> apiSettingsConfig, ILogger<EmailSchedulingService> logger, ILocalizationManager localizationManager) : IEmailSchedulingService
    {
        private readonly IOptions<ApiSettingsConfig> _apiSettingsConfig = apiSettingsConfig;
        private PgDbContext _dbContext = dbContext;
        private readonly ILogger<EmailSchedulingService> _logger = logger;
        private readonly ILocalizationManager _localizationManager = localizationManager;

        /// <inheritdoc />
        public async Task<GeneralResult<EmailSchedule>> FindByGroupAndLanguage(string groupName, string languageCode)
        {
            try
            {
                EmailSchedule? result;

                // Check if Contact.Language is in two-letter format and adjust the query accordingly
                // Basic query to search for email tables
                var emailSchedulesQuery = _dbContext.EmailSchedules!
                    .Include(c => c.Group)
                    .Where(e => e.Group!.Name == groupName);

                // Dealing with two-character language codes
                if (languageCode.Length == 2)
                {
                    result = await emailSchedulesQuery.FirstOrDefaultAsync(e => e.Group!.Language.StartsWith(languageCode));
                }
                else
                {
                    // Find an exact match first
                    result = await emailSchedulesQuery.FirstOrDefaultAsync(e => e.Group!.Language == languageCode);

                    // If no exact match is found, try searching using the first part of the language code
                    if (result == null)
                    {
                        var lang = languageCode.Split('-')[0];
                        result = await emailSchedulesQuery.FirstOrDefaultAsync(e => e.Group!.Language.StartsWith(lang));
                    }
                }

                // If no result is found, use the default language
                if (result == null)
                {
                    result = await emailSchedulesQuery.FirstOrDefaultAsync(e => e.Group!.Language == _apiSettingsConfig.Value.DefaultLanguage);
                }

                return new GeneralResult<EmailSchedule>(true, _localizationManager.GetLocalizedString("EmailScheduleFound"), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to find email schedule");
                return new GeneralResult<EmailSchedule>(false, _localizationManager.GetLocalizedString("FailedToFindEmailSchedule"), null);
            }
        }

        /// <inheritdoc />
        public void SetDBContext(PgDbContext pgDbContext)
        {
            _dbContext = pgDbContext;
        }
    }
}
