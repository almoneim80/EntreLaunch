using EntreLaunch.DTOs.ImportDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Services.ImportSvc
{
    public class MultipleImportService<TRoot, TAggregateDto>(
        IDbContextFactory<PgDbContext> dbContextFactory,
        IMapper mapper,
        ILogger<MultipleImportService<TRoot, TAggregateDto>> logger,
        ILocalizationManager localizationManager) : IMultipleImportService<TRoot, TAggregateDto>
        where TAggregateDto : BaseEntityWithId
        where TRoot : BaseEntityWithId, new()
    {
        private readonly ILogger<MultipleImportService<TRoot, TAggregateDto>> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly IDbContextFactory<PgDbContext> _dbContextFactory = dbContextFactory;
        private readonly ILocalizationManager _localizationManager = localizationManager;

        public async Task<GeneralResult<ImportResult>> ImportAsync(IEnumerable<TAggregateDto> aggregates)
        {
            if (aggregates == null || !aggregates.Any())
            {
                return new(false, _localizationManager.GetLocalizedString("NoDataToImport"), null);
            }

            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            dbContext.IsImportRequest = true;

            try
            {
                // convert to root entity and add to db
                var entities = _mapper.Map<List<TRoot>>(aggregates);

                // save to db in one batch
                await dbContext.Set<TRoot>().AddRangeAsync(entities);

                // save changes to db and get number of changes
                var savedChanges = await dbContext.SaveChangesAsync();
                if(savedChanges <= 0)
                {
                    return new GeneralResult<ImportResult>(false, _localizationManager.GetLocalizedString("NoDataToImport"), null);
                }

                var result = new ImportResult
                {
                    Added = savedChanges,
                    Messages = new List<string> { $"Imported {savedChanges} records." }
                };

                return new GeneralResult<ImportResult>(true, _localizationManager.GetLocalizedString("ImportSuccess"), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An Unexpected error occurred while importing data.");
                return new GeneralResult<ImportResult>(false, _localizationManager.GetLocalizedString("ImportError"), null);
            }
        }
    }
}
