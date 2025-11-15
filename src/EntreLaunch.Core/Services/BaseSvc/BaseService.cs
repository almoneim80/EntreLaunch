namespace EntreLaunch.Services.BaseSvc
{
    public class BaseService<T, TC, TU, TD>
    where T : SharedData, new()
    where TC : class
    where TU : class
    where TD : class
    {
        private readonly IDbContextFactory<PgDbContext> _dbContextFactory;
        private readonly IMapper _mapper;
        private readonly CacheSettings _cacheSettings;
        private readonly ILogger<BaseService<T, TC, TU, TD>> _logger;
        private readonly ICacheService _cacheService;
        private readonly ElasticClient? _elasticClient;
        private readonly string _cacheKeyPrefix;
        public BaseService(
            IDbContextFactory<PgDbContext> dbContextFactory,
            IMapper mapper,
            ICacheService cacheService,
            IOptions<CacheSettings> cacheSettings,
            ILogger<BaseService<T, TC, TU, TD>> logger,
            EsDbContext esDbContext)
        {
            _dbContextFactory = dbContextFactory;
            _mapper = mapper;
            _cacheSettings = cacheSettings.Value;
            _logger = logger;
            _cacheService = cacheService;
            _cacheKeyPrefix = typeof(T).Name.ToLower();
            _elasticClient = esDbContext.ElasticClient;
        }

        public BaseService(
            IDbContextFactory<PgDbContext> dbContextFactory,
            IMapper mapper,
            ICacheService cacheService,
            IOptions<CacheSettings> cacheSettings,
            ILogger<BaseService<T, TC, TU, TD>> logger)
        {
            _dbContextFactory = dbContextFactory;
            _mapper = mapper;
            _cacheSettings = cacheSettings.Value;
            _logger = logger;
            _cacheService = cacheService;
            _cacheKeyPrefix = typeof(T).Name.ToLower();
        }

        /// <summary>
        /// Retrieves all entities from the database that are not marked as deleted.
        /// </summary>
        public async Task<GeneralResult<List<TD>>> GetAllAsync()
        {
            string cacheKey = $"{_cacheKeyPrefix}_all";
            var cachedData = await _cacheService.GetAsync<List<T>>(cacheKey);
            if (cachedData != null)
            {
                _logger.LogInformation("BaseService - GetAllAsync : Retrieved data from cache for key: {CacheKey}", cacheKey);
                var mappedData = _mapper.Map<List<TD>>(cachedData);
                return new GeneralResult<List<TD>>(true, "Retrieved data from cache.", mappedData);
            }
            else
            {
                try
                {
                    using var dbContext = _dbContextFactory.CreateDbContext();
                    var entities = await dbContext.Set<T>().AsNoTracking().Where(e => !e.IsDeleted).ToListAsync();
                    if (entities == null)
                    {
                        _logger.LogWarning("BaseService - GetAllAsync : No data found in database.");
                        return new GeneralResult<List<TD>>(false, "No data found in database.", null);
                    }

                    var result = _mapper.Map<List<TD>>(entities);
                    await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(_cacheSettings.CacheExpirationMinutes));
                    _logger.LogInformation("BaseService - GetAllAsync : Retrieved data from database.");
                    return new GeneralResult<List<TD>>(true, "Retrieved data from database.", result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "BaseService - GetAllAsync : Error occurred while retrieving all records.");
                    return new GeneralResult<List<TD>>(false, "Error occurred while retrieving all records.", null);
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
                return new GeneralResult<TD?>(true, "Retrieved data from cache.", cachedData);
            }

            try
            {
                using var dbContext = _dbContextFactory.CreateDbContext();
                var entity = await dbContext.Set<T>().AsNoTracking().FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
                if (entity == null)
                {
                    _logger.LogWarning("Entity not found.");
                    return new GeneralResult<TD?>(false, "Entity not found.", null);
                }

                var result = _mapper.Map<TD>(entity);
                await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(_cacheSettings.CacheExpirationMinutes));
                return new GeneralResult<TD?>(true, "Retrieved data from database.", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving entity with ID {Id}.", id);
                return new GeneralResult<TD?>(false, "Error occurred while retrieving entity.", null);
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
                return new GeneralResult<TD>(true, "Entity created successfully.", _mapper.Map<TD>(entity));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "BaseService - CreateAsync : Error occurred while creating a new entity.");
                return new GeneralResult<TD>(false, "Error occurred while creating a new entity.", null);
            }
        }

        /// <summary>
        /// EntreLaunchdates an existing entity by its ID using the provided data.
        /// </summary>
        public async Task<GeneralResult<TD?>> EntreLaunchdateAsync(int id, TU EntreLaunchdateDto)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var entityToEntreLaunchdate = await dbContext.Set<T>().FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
                if (entityToEntreLaunchdate == null)
                {
                    _logger.LogWarning("BaseService - EntreLaunchdateAsync : Entity with ID {Id} not found for EntreLaunchdate.", id);
                    return new GeneralResult<TD?>(false, "Entity not found for EntreLaunchdate.", null);
                }

                _mapper.Map(EntreLaunchdateDto, entityToEntreLaunchdate);
                await dbContext.SaveChangesAsync();

                // EntreLaunchdate cache
                string cacheKeyForOne = $"{_cacheKeyPrefix}_one_{id}";
                string cacheKeyForAll = $"{_cacheKeyPrefix}_all";

                // remove old data from cache
                _cacheService.Remove(cacheKeyForOne);
                _cacheService.Remove(cacheKeyForAll);

                // add new data to cache
                var EntreLaunchdatedDto = _mapper.Map<TD>(entityToEntreLaunchdate);
                await _cacheService.SetAsync(cacheKeyForOne, EntreLaunchdatedDto, TimeSpan.FromMinutes(_cacheSettings.CacheExpirationMinutes));

                // EntreLaunchdate elastic
                //if (SupportsElastic())
                //{
                //    await SyncWithElastic(entityToEntreLaunchdate);
                //}

                await transaction.CommitAsync();
                _logger.LogInformation("BaseService - EntreLaunchdateAsync : Entity with ID {Id} EntreLaunchdated successfully.", id);
                return new GeneralResult<TD?>(true, "Entity EntreLaunchdated successfully.", _mapper.Map<TD>(entityToEntreLaunchdate));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "BaseService - EntreLaunchdateAsync : Error occurred while EntreLaunchdating entity with ID {Id}.", id);
                return new GeneralResult<TD?>(false, "Error occurred while EntreLaunchdating entity.", null);
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
                    return new GeneralResult<bool>(false, "Entity not found for deletion.", false);
                }

                entity.IsDeleted = true;
                entity.DeletedAt = DateTimeOffset.UtcNow;
                await dbContext.SaveChangesAsync();

                // remove caches
                string cacheKey = $"{_cacheKeyPrefix}_one_{id}";
                _cacheService.Remove(cacheKey);
                _logger.LogInformation("BaseService - DeleteAsync : Removed data from cache for key: {CacheKey}", cacheKey);

                // EntreLaunchdate cache list
                string allCacheKey = $"{_cacheKeyPrefix}_all";
                var cachedList = await _cacheService.GetAsync<List<T>>(allCacheKey);
                if (cachedList != null)
                {
                    var EntreLaunchdatedList = cachedList.Where(item => item.Id != id).ToList();
                    await _cacheService.SetAsync(allCacheKey, EntreLaunchdatedList, TimeSpan.FromMinutes(_cacheSettings.CacheExpirationMinutes));
                    _logger.LogInformation("BaseService - DeleteAsync : EntreLaunchdated cached list after deleting entity with ID {Id}.", id);
                }

                // remove from elastic
                //if (SupportsElastic())
                //{
                //    await RemoveFromElastic(entity.Id);
                //}

                await transaction.CommitAsync();
                _logger.LogInformation("BaseService - DeleteAsync : Entity with ID {Id} deleted successfully.", id);
                return new GeneralResult<bool>(true, "Entity deleted successfully.", true);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "BaseService - DeleteAsync : Error occurred while deleting entity with ID {Id}.", id);
                return new GeneralResult<bool>(false, "Error occurred while deleting entity.", false);
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
        //        ? ElasticHelper.GetIndexName("EntreLaunch", typeof(T))
        //        : throw new InvalidOperationException($"Index name for {typeof(T).Name} is not set.");
        //}
    }
}
