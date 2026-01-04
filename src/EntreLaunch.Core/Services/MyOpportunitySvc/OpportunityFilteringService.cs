using EntreLaunch.DTOs.MyOpportunityDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Services.MyOpportunitySvc
{
    public class OpportunityFilteringService(
        PgDbContext dbContext,
        ILogger<OpportunityFilteringService> logger,
        IMapper mapper,
        ILocalizationManager localizationManager) : IOpportunityFilteringService
    {
        private readonly PgDbContext _dbContext = dbContext;
        private readonly ILogger<OpportunityFilteringService> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly ILocalizationManager _localizationManager = localizationManager;

        /// <inheritdoc />
        public async Task<GeneralResult> Filtering(OpportunityFilterDto filter)
        {
            try
            {
                var query = _dbContext.Opportunities
                    .Where(i => !i.IsDeleted && i.Type == OpportunityType.Investment)
                    .AsQueryable();

                if (filter.Costs.HasValue)
                    query = query.Where(i => i.Costs == filter.Costs.Value);

                if (!string.IsNullOrWhiteSpace(filter.Sector))
                    query = query.Where(i => i.Sector != null && i.Sector.Contains(filter.Sector));

                if (filter.BrandCountry.HasValue)
                    query = query.Where(i => i.BrandCountry == filter.BrandCountry);

                var opportunities = _mapper.Map<List<OpportunityDetailsDto>>(await query.ToListAsync());

                if (!opportunities.Any())
                {
                    _logger.LogInformation("No opportunities found for Filtering operation.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("NoDataFound"),
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("FilteredSuccessfully"),
                    Data = opportunities
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while filtering opportunities.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("ErrorWhileFilteringOpportunities"),
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<decimal>>> GetAllCostsAsync()
        {
            try
            {
                var costs = await _dbContext.Opportunities
                    .Where(o => !o.IsDeleted && o.Type == OpportunityType.Investment && o.Costs.HasValue)
                    .Select(o => o.Costs!.Value)
                    .Distinct()
                    .ToListAsync();

                if (!costs.Any())
                {
                    return new GeneralResult<List<decimal>>(false, _localizationManager.GetLocalizedString("NoCostsFound"), null);
                }

                return new GeneralResult<List<decimal>>(true, _localizationManager.GetLocalizedString("CostsRetrieved"), costs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Costs list.");
                return new GeneralResult<List<decimal>>(false, _localizationManager.GetLocalizedString("UnexpectedError"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<string>>> GetAllSectorsAsync()
        {
            try
            {
                var sectors = await _dbContext.Opportunities
                    .Where(o => !o.IsDeleted && o.Type == OpportunityType.Investment && o.Sector != null)
                    .Select(o => o.Sector!)
                    .Distinct()
                    .ToListAsync();

                if (!sectors.Any())
                {
                    return new GeneralResult<List<string>>(false, _localizationManager.GetLocalizedString("NoSectorsFound"), null);
                }

                return new GeneralResult<List<string>>(true, _localizationManager.GetLocalizedString("SectorsRetrieved"), sectors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Sectors list.");
                return new GeneralResult<List<string>>(false, _localizationManager.GetLocalizedString("UnexpectedError"), null);
            }
        }
    }
}
