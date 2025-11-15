namespace EntreLaunch.Services.MyPartnerSvc
{
    public class MyPartnerFilteringService : IMyPartnerFilteringService
    {
        private readonly PgDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<MyPartnerService> _logger;
        public MyPartnerFilteringService(
            PgDbContext dbContext,
            IMapper mapper,
            ILogger<MyPartnerService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Filter Projects.
        /// </summary>
        public async Task<GeneralResult> Filtering(FilterProjectsDto filter)
        {
            var validator = new FilterProjectsValidator();
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

            var projects = _mapper.Map<List<MyPartnerDetailsDto>>(await _dbContext.MyPartners
                .Where(i => i.City!.Equals(filter.City)
                && i.Activity!.Contains(filter.Activity)
                && i.CapitalFrom == filter.CapitalFrom
                && i.CapitalTo == filter.CapitalTo
                && !i.IsDeleted
                && i.Status == MyPartnerStatus.Accepted)
                .Include(p => p.ProjectAttachments)
                .ToListAsync());

            if (!projects.Any())
            {
                _logger.LogError("No projects found for Filtering operation.");
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "No projects found.",
                    Data = null
                };
            }

            return new GeneralResult
            {
                IsSuccess = true,
                Message = "Projects filtered Successfully.",
                Data = projects
            };
        }
    }
}
