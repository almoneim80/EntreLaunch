using EntreLaunch.DTOs.MyPartnerDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Services.MyPartnerSvc
{
    public class MyPartnerFilteringService(
        PgDbContext dbContext,
        IMapper mapper,
        ILogger<MyPartnerService> logger,
        ILocalizationManager localizationManager) : IMyPartnerFilteringService
    {
        private readonly PgDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<MyPartnerService> _logger = logger;
        private readonly ILocalizationManager _localizationManager = localizationManager;

        /// <inheritdoc/>
        public async Task<GeneralResult> Filtering(FilterProjectsDto filter)
        {
            var query = _dbContext.MyPartners
                .Where(i => !i.IsDeleted && i.Status == MyPartnerStatus.Accepted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.City))
                query = query.Where(i => i.City == filter.City);

            if (!string.IsNullOrWhiteSpace(filter.Activity))
                query = query.Where(i => i.Activity != null && i.Activity.Contains(filter.Activity));

            if (filter.CapitalFrom.HasValue)
                query = query.Where(i => i.CapitalFrom >= filter.CapitalFrom.Value);

            if (filter.CapitalTo.HasValue)
                query = query.Where(i => i.CapitalTo <= filter.CapitalTo.Value);

            var projects = _mapper.Map<List<MyPartnerDetailsDto>>(
                await query.Include(p => p.ProjectAttachments).ToListAsync());

            if (!projects.Any())
            {
                _logger.LogWarning("No projects found for Filtering operation.");
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("NoProjectsFound"),
                    Data = null
                };
            }

            return new GeneralResult
            {
                IsSuccess = true,
                Message = _localizationManager.GetLocalizedString("ProjectsFilteredSuccessfully"),
                Data = projects
            };
        }

        /// <inheritdoc/>
        public async Task<GeneralResult<List<string>>> GetAllActivitiesAsync()
        {
            try
            {
                var activities = await _dbContext.MyPartners
                    .Where(i => !i.IsDeleted && !string.IsNullOrWhiteSpace(i.Activity))
                    .Select(i => i.Activity!)
                    .Distinct()
                    .ToListAsync();

                if (!activities.Any())
                {
                    _logger.LogWarning("No activities found.");
                    return new GeneralResult<List<string>>(false, _localizationManager.GetLocalizedString("NoActivitiesFound"), null);
                }

                return new GeneralResult<List<string>>(true, _localizationManager.GetLocalizedString("ActivitiesRetrievedSuccessfully"), activities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving activities.");
                return new GeneralResult<List<string>>(false, _localizationManager.GetLocalizedString("UnexpectedErrorInActivities"), null);
            }
        }
    }
}
