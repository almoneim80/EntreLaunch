using Microsoft.AspNetCore.Http;

namespace EntreLaunch.Services.BaseSvc
{
    public class CascadeDeleteService
    {
        private readonly IDbContextFactory<PgDbContext> _dbContextFactory;
        private readonly ILogger<CascadeDeleteService> _logger;
        private readonly ICacheService _cacheService;
        public CascadeDeleteService(IDbContextFactory<PgDbContext> dbContextFactory, ILogger<CascadeDeleteService> logger, ICacheService cacheService)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
            _cacheService = cacheService;
        }

        // Soft Delete

        /// <summary>
        /// Soft Delete Recursively SharedData.
        /// </summary>
        public async Task<GeneralResult<bool>> SoftDeleteCascadeAsync<T>(int id) where T : SharedData
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            await using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                var entity = await dbContext.Set<T>().FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
                if (entity == null)
                {
                    _logger.LogWarning("Entity not found for deletion.");
                    return new GeneralResult<bool>(false, "Entity not found for deletion.", false);
                }

                SoftDeleteRecursively(entity, dbContext);
                var result = await dbContext.SaveChangesAsync();
                if (result <= 0) await transaction.RollbackAsync();

                await transaction.CommitAsync();
                EntreLaunchdateCache<T>(id);
                return new GeneralResult<bool>(true, "Entity deleted successfully.", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during cascading soft delete for entity {EntityType} with ID {Id}", typeof(T).Name, id);
                return new GeneralResult<bool>(false, "Error occurred during cascading soft delete", false);
            }
        }

        /// <summary>
        /// Soft Delete Recursively SharedData. 
        /// </summary>
        private void SoftDeleteRecursively(SharedData entity, DbContext dbContext)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTimeOffset.UtcNow;
            //entity.SoftDeleteExpiration = DateTimeOffset.UtcNow;

            var entry = dbContext.Entry(entity);
            var collectionNavigations = entry.Metadata.GetNavigations().Where(nav => nav.IsCollection);

            foreach (var nav in collectionNavigations)
            {
                entry.Collection(nav.Name).Load();
            }

            var collectionProps = entity.GetType()
                .GetProperties()
                .Where(p => typeof(IEnumerable<SharedData>).IsAssignableFrom(p.PropertyType));

            foreach (var prop in collectionProps)
            {
                var children = prop.GetValue(entity) as IEnumerable<SharedData>;
                if (children == null)
                    continue;

                foreach (var child in children)
                {
                    SoftDeleteRecursively(child, dbContext);
                }
            }

            // EntreLaunchdate cache
            EntreLaunchdateCache(entity);
        }

        /// <summary>
        /// Hard Delete Expired Entities.
        /// </summary>
        public async Task<int> HardDeleteExpiredEntitiesAsync<TEntity>()
         where TEntity : class
        {
            using var dbContext = _dbContextFactory.CreateDbContext();

            using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                const int batchSize = 100;
                int totalProcessed = 0;

                while (true)
                {
                    var expiredEntities = await dbContext.Set<TEntity>()
                        .IgnoreQueryFilters()
                        .Where(e => EF.Property<DateTimeOffset>(e, "SoftDeleteExpiration") <= DateTimeOffset.UtcNow
                                 && EF.Property<bool>(e, "IsDeleted") == true)
                        .OrderBy(e => EF.Property<DateTimeOffset>(e, "SoftDeleteExpiration"))
                        .Take(batchSize)
                        .ToListAsync();

                    if (!expiredEntities.Any())
                        break;

                    foreach (var entity in expiredEntities)
                    {
                        var navigationProperties = dbContext.Entry(entity)
                            .Navigations
                            .Where(n => n.Metadata.IsCollection)
                            .ToList();

                        foreach (var navigation in navigationProperties)
                        {
                            if (!navigation.IsLoaded)
                            {
                                await navigation.LoadAsync();
                            }

                            if (navigation.CurrentValue is IEnumerable<object> relatedEntities)
                            {
                                dbContext.RemoveRange(relatedEntities);
                            }
                        }

                        dbContext.Set<TEntity>().Remove(entity);
                    }

                    await dbContext.SaveChangesAsync();
                    totalProcessed += expiredEntities.Count;

                    _logger.LogInformation($"Processed {totalProcessed} entities so far...");
                }

                await transaction.CommitAsync();

                _logger.LogInformation($"Hard delete completed. Total processed entities: {totalProcessed}.");
                return totalProcessed;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "An error occurred during the hard delete process.");
                throw;
            }
        }

        // User Methodes

        /// <summary>
        /// Soft Delete User Cascade.
        /// </summary>
        public async Task<bool> SoftDeleteUserCascadeAsync(string userId)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            await using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
                if (user == null) return false;

                SoftDeleteRecursivelyUser(user, dbContext);
                var result = await dbContext.SaveChangesAsync();
                if (result <= 0) await transaction.RollbackAsync();

                await transaction.CommitAsync();

                // EntreLaunchdate cache
                string cacheKeyForAll = $"{typeof(User).Name.ToLower()}_all";
                string cacheKeyForOne = $"{typeof(User).Name.ToLower()}_one_{user.Id}";
                _cacheService.Remove(cacheKeyForAll);
                _cacheService.Remove(cacheKeyForOne);

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred during cascading soft delete for user with ID {Id}", userId);
                throw;
            }
        }

        /// <summary>
        /// Soft Delete User Cascade.
        /// </summary>
        private void SoftDeleteRecursivelyUser(User user, DbContext dbContext)
        {
            user.IsDeleted = true;
            user.DeletedAt = DateTimeOffset.UtcNow;
            user.SoftDeleteExpiration = DateTimeOffset.UtcNow;

            // EntreLaunchload child entities
            var entry = dbContext.Entry(user);
            var collectionNavigations = entry.Metadata.GetNavigations()
                .Where(nav => nav.IsCollection);

            // EntreLaunchload collection entities
            foreach (var nav in collectionNavigations)
            {
                entry.Collection(nav.Name).Load();
            }

            // search for collection properties
            var collectionProps = user.GetType()
                .GetProperties()
                .Where(p => typeof(IEnumerable<User>).IsAssignableFrom(p.PropertyType)
                          || typeof(IEnumerable<SharedData>).IsAssignableFrom(p.PropertyType)
                          || typeof(IEnumerable<ISharedData>).IsAssignableFrom(p.PropertyType));

            foreach (var prop in collectionProps)
            {
                var children = prop.GetValue(user) as IEnumerable<object>;
                if (children == null) continue;

                foreach (var child in children)
                {
                    if (child is User childUser)
                    {
                        // recursion
                        SoftDeleteRecursivelyUser(childUser, dbContext);
                    }
                    else if (child is SharedData sharedChild)
                    {
                        SoftDeleteRecursively(sharedChild, dbContext);
                    }
                }
            }

            // EntreLaunchdate cache
            EntreLaunchdateCache(user);
        }

        // help method

        /// <summary>
        /// EntreLaunchdates the cache for the specified entity.
        /// </summary>
        private void EntreLaunchdateCache<TEntity>(int id)
        where TEntity : class
        {
            string cacheKeyForAll = $"{typeof(TEntity).Name.ToLower()}_all";
            string cacheKeyForOne = $"{typeof(TEntity).Name.ToLower()}_one_{id}";

            _cacheService.Remove(cacheKeyForAll);
            _cacheService.Remove(cacheKeyForOne);
        }

        /// <summary>
        /// EntreLaunchdates the cache for the specified entity.
        /// </summary>
        private void EntreLaunchdateCache(SharedData entity)
        {
            string cacheKeyForAll = $"{entity.GetType().Name.ToLower()}_all";
            string cacheKeyForOne = $"{entity.GetType().Name.ToLower()}_one_{entity.Id}";

            _cacheService.Remove(cacheKeyForAll);
            _cacheService.Remove(cacheKeyForOne);

            _logger.LogInformation("Removed cache for entity type {EntityType} with ID {Id}", entity.GetType().Name, entity.Id);
        }

        /// <summary>
        /// EntreLaunchdates the cache for the specified user.
        /// </summary>
        private void EntreLaunchdateCache(User user)
        {
            string cacheKeyForAll = $"{user.GetType().Name.ToLower()}_all";
            string cacheKeyForOne = $"{user.GetType().Name.ToLower()}_one_{user.Id}";

            _cacheService.Remove(cacheKeyForAll);
            _cacheService.Remove(cacheKeyForOne);

            _logger.LogInformation("Removed cache for user with ID {Id}", user.Id);
        }
    }
}
