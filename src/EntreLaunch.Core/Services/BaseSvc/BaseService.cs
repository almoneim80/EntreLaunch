using AutoMapper.QueryableExtensions;
using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.TrainingDtos;
using EntreLaunch.Extensions;

namespace EntreLaunch.Services.BaseSvc
{
    public class BaseService<T, TC, TU, TD>(
        IDbContextFactory<PgDbContext> dbContextFactory,
        IMapper mapper,
        ICacheService cacheService,
        IOptions<CacheSettings> cacheSettings,
        ILocalizationManager localizationManager,
        ILogger<BaseService<T, TC, TU, TD>> logger,
        EsDbContext esDbContext)
    where T : SharedData, new()
    where TC : class
    where TU : class
    where TD : class
    {
        private readonly IDbContextFactory<PgDbContext> _dbContextFactory = dbContextFactory;
        private readonly IMapper _mapper = mapper;
        private readonly CacheSettings _cacheSettings = cacheSettings.Value;
        private readonly ILogger<BaseService<T, TC, TU, TD>> _logger = logger;
        private readonly ICacheService _cacheService = cacheService;
        private readonly ElasticClient? _elasticClient = esDbContext.ElasticClient;
        private readonly string _cacheKeyPrefix = typeof(T).Name.ToLower();
        private readonly ILocalizationManager _localizationManager = localizationManager;

        /// <summary>
        /// Retrieves all entities from the database that are not marked as deleted.
        /// </summary>
        public async Task<GeneralResult<PaginatedResult<TD>>> GetAllAsync(PaginationParams pagination)
        {
            if (pagination == null)
            {
                _logger.LogWarning("BaseService - GetAllAsync : Pagination parameters are null.");
                return new GeneralResult<PaginatedResult<TD>>(false, _localizationManager.GetLocalizedString("PaginationParamsNull"), null);
            }

            var cacheKey = $"{_cacheKeyPrefix}_all_p{pagination.Page}_s{pagination.PageSize}";
            var cachedData = await _cacheService.GetAsync<PaginatedResult<TD>>(cacheKey);
            if (cachedData != null)
            {
                _logger.LogInformation("BaseService - GetAllAsync : Retrieved paginated data from cache for key: {CacheKey}", cacheKey);
                return new GeneralResult<PaginatedResult<TD>>(true, _localizationManager.GetLocalizedString("DataRetrievedFromCache"), cachedData);
            }
            else
            {
                try
                {
                    using var dbContext = _dbContextFactory.CreateDbContext();
                    var query = dbContext.Set<T>().AsNoTracking().Where(e => !e.IsDeleted);
                    var paginatedResult = await query.ProjectTo<TD>(_mapper.ConfigurationProvider).ToPagedResultAsync(pagination);
                    await _cacheService.SetAsync(cacheKey, paginatedResult, TimeSpan.FromMinutes(_cacheSettings.CacheExpirationMinutes));

                    _logger.LogInformation("BaseService - GetAllAsync : Retrieved paginated data from database.");
                    return new GeneralResult<PaginatedResult<TD>>(true, _localizationManager.GetLocalizedString("DataRetrievedFromDatabase"), paginatedResult);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "BaseService - GetAllAsync : Error occurred while retrieving paginated records.");
                    return new GeneralResult<PaginatedResult<TD>>(false, _localizationManager.GetLocalizedString("ErrorRetrievingAllRecords"), null);
                }
            }
        }

        /// <summary>
        /// Retrieves a single entity by its ID.
        /// </summary>
        public async Task<GeneralResult<TD?>> GetOneAsync(int id)
        {
            string cacheKey = $"{_cacheKeyPrefix}_one_{id}";
            var cachedData = await _cacheService.GetAsync<TD>(cacheKey);
            if (cachedData != null)
            {
                _logger.LogInformation("Retrieved data from cache for key: {CacheKey}", cacheKey);
                return new GeneralResult<TD?>(true, _localizationManager.GetLocalizedString("DataRetrievedFromCache"), cachedData);
            }

            try
            {
                using var dbContext = _dbContextFactory.CreateDbContext();
                var entity = await dbContext.Set<T>().AsNoTracking().FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
                if (entity == null)
                {
                    _logger.LogWarning("Entity not found.");
                    return new GeneralResult<TD?>(false, _localizationManager.GetLocalizedString("EntityNotFound"), null);
                }

                var result = _mapper.Map<TD>(entity);
                await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(_cacheSettings.CacheExpirationMinutes));
                return new GeneralResult<TD?>(true, _localizationManager.GetLocalizedString("DataRetrievedFromDatabase"), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving entity with ID {Id}.", id);
                return new GeneralResult<TD?>(false, _localizationManager.GetLocalizedString("ErrorRetrievingEntity"), null);
            }
        }

        /// <summary>
        /// Creates a new entity and saves it to the database.
        /// </summary>
        public async Task<GeneralResult<TD>> CreateAsync(TC createDto)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var entity = _mapper.Map<T>(createDto);
                dbContext.Set<T>().Add(entity);
                await dbContext.SaveChangesAsync();

                // add to elastic
                //if (SupportsElastic())
                //{
                //    await SyncWithElastic(entity);
                //}

                await transaction.CommitAsync();
                _logger.LogInformation("BaseService - CreateAsync : Entity created successfully with ID {Id}.", entity.Id);
                return new GeneralResult<TD>(true, _localizationManager.GetLocalizedString("EntityCreatedSuccessfully"), _mapper.Map<TD>(entity));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "BaseService - CreateAsync : Error occurred while creating a new entity.");
                return new GeneralResult<TD>(false, _localizationManager.GetLocalizedString("ErrorCreatingEntity"), null);
            }
        }

        /// <summary>
        /// Updates an existing entity by its ID using the provided data.
        /// </summary>
        public async Task<GeneralResult<TD?>> UpdateAsync(int id, TU updateDto)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var entityToUpdate = await dbContext.Set<T>().FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
                if (entityToUpdate == null)
                {
                    _logger.LogWarning("BaseService - UpdateAsync : Entity with ID {Id} not found for update.", id);
                    return new GeneralResult<TD?>(false, _localizationManager.GetLocalizedString("EntityNotFoundForUpdate"), null);
                }

                _mapper.Map(updateDto, entityToUpdate);
                await dbContext.SaveChangesAsync();

                // update cache
                string cacheKeyForOne = $"{_cacheKeyPrefix}_one_{id}";

                // remove old data from cache
                _cacheService.Remove(cacheKeyForOne);
                await _cacheService.RemoveByPrefixAsync($"{_cacheKeyPrefix}_all_p");

                // add new data to cache
                var updatedDto = _mapper.Map<TD>(entityToUpdate);
                await _cacheService.SetAsync(cacheKeyForOne, updatedDto, TimeSpan.FromMinutes(_cacheSettings.CacheExpirationMinutes));

                // update elastic
                //if (SupportsElastic())
                //{
                //    await SyncWithElastic(entityToUpdate);
                //}

                await transaction.CommitAsync();
                _logger.LogInformation("BaseService - UpdateAsync : Entity with ID {Id} updated successfully.", id);
                return new GeneralResult<TD?>(true, _localizationManager.GetLocalizedString("EntityUpdatedSuccessfully"), null);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "BaseService - UpdateAsync : Error occurred while updating entity with ID {Id}.", id);
                return new GeneralResult<TD?>(false, _localizationManager.GetLocalizedString("ErrorUpdatingEntity"), null);
            }
        }

        /// <summary>
        /// Soft deletes an entity by setting its IsDeleted property to true.
        /// </summary>
        public async Task<GeneralResult<bool>> DeleteAsync(int id)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var entity = await dbContext.Set<T>().FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
                if (entity == null)
                {
                    _logger.LogWarning("BaseService - DeleteAsync : Entity with ID {Id} not found for deletion.", id);
                    return new GeneralResult<bool>(false, _localizationManager.GetLocalizedString("EntityNotFoundForDeletion"), false);
                }

                entity.IsDeleted = true;
                entity.DeletedAt = DateTimeOffset.UtcNow;
                await dbContext.SaveChangesAsync();

                // remove caches
                string cacheKeyForOne = $"{_cacheKeyPrefix}_one_{id}";
                _cacheService.Remove(cacheKeyForOne);
                await _cacheService.RemoveByPrefixAsync($"{_cacheKeyPrefix}_all_p");

                _logger.LogInformation($"BaseService - DeleteAsync : Removed data from cache for ID {id} and prefix {_cacheKeyPrefix}_all_p");

                // remove from elastic
                //if (SupportsElastic())
                //{
                //    await RemoveFromElastic(entity.Id);
                //}

                await transaction.CommitAsync();
                _logger.LogInformation("BaseService - DeleteAsync : Entity with ID {Id} deleted successfully.", id);
                return new GeneralResult<bool>(true, _localizationManager.GetLocalizedString("EntityDeletedSuccessfully"), true);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "BaseService - DeleteAsync : Error occurred while deleting entity with ID {Id}.", id);
                return new GeneralResult<bool>(false, _localizationManager.GetLocalizedString("ErrorDeletingEntity"), false);
            }
        }

        /// <summary>
        /// Removes second-level objects from a list of objects.
        /// </summary>
        public List<TD> RemoveSecondLevelObjects(IList<TD> data)
        {
            var refs = new Dictionary<PropertyInfo, List<PropertyInfo>>();

            foreach (var property in typeof(TD).GetProperties())
            {
                if (property.PropertyType.GetInterface("IEnumerable") != null && property.PropertyType.IsGenericType)
                {
                    var innerType = property.PropertyType.GetGenericArguments()[0];
                    var nestedProps = innerType.GetProperties()
                        .Where(p => !p.PropertyType.IsPrimitive && p.PropertyType != typeof(string))
                        .ToList();

                    if (nestedProps.Any())
                    {
                        refs[property] = nestedProps;
                    }
                }
            }

            foreach (var item in data)
            {
                foreach (var r in refs)
                {
                    var propertyObject = r.Key.GetValue(item);
                    if (propertyObject != null)
                    {
                        if (r.Key.PropertyType.GetInterface("IEnumerable") != null && r.Key.PropertyType.IsGenericType)
                        {
                            var e = propertyObject as System.Collections.IEnumerable;
                            foreach (var obj in e!)
                            {
                                foreach (var p in r.Value)
                                {
                                    p.SetValue(obj, null);
                                }
                            }
                        }
                        else
                        {
                            foreach (var p in r.Value)
                            {
                                p.SetValue(propertyObject, null);
                            }
                        }
                    }
                }
            }

            return data.ToList();
        }

        // helper methods
        //private bool SupportsElastic()
        //{
        //    return typeof(T).GetCustomAttributes(typeof(SupportsElasticAttribute), true).Any();
        //}

        //private async Task SyncWithElastic(T entity)
        //{
        //    await _elasticClient!.IndexAsync(entity, i => i.Index(GetElasticIndexName()));
        //}

        //private async Task RemoveFromElastic(int id)
        //{
        //    await _elasticClient!.DeleteAsync<T>(id, d => d.Index(GetElasticIndexName()));
        //}

        //private string GetElasticIndexName()
        //{
        //    return typeof(T).GetCustomAttributes(typeof(SupportsElasticAttribute), true).Any()
        //        ? ElasticHelper.GetIndexName("up", typeof(T))
        //        : throw new InvalidOperationException($"Index name for {typeof(T).Name} is not set.");
        //}
    }
}
