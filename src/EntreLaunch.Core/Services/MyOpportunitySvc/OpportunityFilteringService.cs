namespace EntreLaunch.Services.MyOpportunitySvc
{
    public class OpportunityFilteringService : IOpportunityFilteringService
    {
        private readonly PgDbContext _dbContext;
        private readonly ILogger<OpportunityFilteringService> _logger;
        private readonly IMapper _mapper;
        public OpportunityFilteringService(PgDbContext dbContext, ILogger<OpportunityFilteringService> logger, IMapper mapper)
        {
            _dbContext = dbContext;
            _logger = logger;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<GeneralResult> Filtering(OpportunityFilterDto filter)
        {
            try
            {
                var validator = new FilterOpportunityValidator();
                var validationResult = validator.Validate(filter);

                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogError("Validation failed: " + string.Join(", ", errors));

                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Validation failed",
                        Data = errors
                    };
                }

                var opportunities = _mapper.Map<List<OpportunityDetailsDto>>(await _dbContext.Opportunities
                    .Where(i => i.Costs.Equals(filter.Costs)
                    && i.Sector!.Contains(filter.Sector!)
                    && i.BrandCountry.Equals(filter.BrandCountry)
                    && !i.IsDeleted && i.Type == OpportunityType.Investment)
                    .ToListAsync());

                if (!opportunities.Any())
                {
                    _logger.LogInformation("No opportunities found for Filtering operation.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "No data found.",
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Filtered Successfully.",
                    Data = opportunities
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while filtering opportunities.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An error occurred while filtering opportunities.",
                    Data = null
                };
            }
        }
    }
}
