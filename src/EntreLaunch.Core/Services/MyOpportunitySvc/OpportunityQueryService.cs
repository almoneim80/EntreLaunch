namespace EntreLaunch.Services.MyOpportunitySvc
{
    public class OpportunityQueryService : IOpportunityQueryService
    {
        private readonly PgDbContext _dbContext;
        private readonly ILogger<OpportunityQueryService> _logger;
        private readonly IMapper _mapper;
        public OpportunityQueryService(
            ILogger<OpportunityQueryService> logger,
            IMapper mapper,
            PgDbContext pgDbContext)
        {
            _logger = logger;
            _mapper = mapper;
            _dbContext = pgDbContext;
        }

        /// <inheritdoc />
        public async Task<GeneralResult> AllInvestmentOpportunities()
        {
            try
            {
                var investmentOpportunities = _mapper.Map<List<OpportunityDetailsDto>>(await
                    _dbContext.Opportunities.Where(o => !o.IsDeleted && o.Type == OpportunityType.Investment).ToListAsync());

                if (!investmentOpportunities.Any())
                {
                    _logger.LogInformation("No investment Opportunities found.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "No investment Opportunities found.",
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Investment Opportunities retrieved successfully.",
                    Data = investmentOpportunities
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to get investment opportunities.",
                    Data = null
                };
            }
        }
    }
}
