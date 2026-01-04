using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.MyOpportunityDtos;
using EntreLaunch.DTOs.TrainingDtos;
using EntreLaunch.Extensions;
namespace EntreLaunch.Services.MyOpportunitySvc
{
    public class OpportunityQueryService(
        ILogger<OpportunityQueryService> logger,
        IMapper mapper,
        PgDbContext pgDbContext,
        ILocalizationManager localizationManager) : IOpportunityQueryService
    {
        private readonly PgDbContext _dbContext = pgDbContext;
        private readonly ILogger<OpportunityQueryService> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly ILocalizationManager _localizationManager = localizationManager;

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<OpportunityDetailsDto>>> AllInvestmentOpportunities(PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var query = _dbContext.Opportunities
                    .AsNoTracking()
                    .Where(o => !o.IsDeleted && o.Type == OpportunityType.Investment)
                    .OrderByDescending(o => o.CreatedAt)
                    .Select(o => new OpportunityDetailsDto
                    {
                        Id = o.Id,
                        CompanyName = o.CompanyName,
                        Logo = o.Logo,
                        Description = o.Description,
                        Sector = o.Sector,
                        Costs = o.Costs,
                        ContractDurationInDay = o.ContractDurationInDay,
                        AcceptRequirements = o.AcceptRequirements,
                        BrandCountry = o.BrandCountry,
                        Type = o.Type,
                        CreatedAt = o.CreatedAt,
                        UpdatedAt = o.UpdatedAt
                    });

                var paginatedResult = await query.ToPagedResultAsync(pagination, cancellationToken);

                return new GeneralResult<PaginatedResult<OpportunityDetailsDto>>(true,
                    _localizationManager.GetLocalizedString("InvestmentOpportunitiesRetrievedSuccessfully"),
                    paginatedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch investment opportunities");
                return new GeneralResult<PaginatedResult<OpportunityDetailsDto>>(false,
                    _localizationManager.GetLocalizedString("FailedToGetInvestmentOpportunities"),
                    null);
            }
        }
    }
}
