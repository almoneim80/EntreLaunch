namespace EntreLaunch.Interfaces.CacheIntf
{
    public interface ICacheService
    {
        /// <summary>
        /// Get a value from the cache by key.
        /// </summary>
        T? Get<T>(string key);

        /// <summary>
        /// Set a value in the cache by key.
        /// </summary>
        void Set<T>(string key, T value, TimeSpan expirationTime, bool useSlidingExpiration = false, CacheItemPriority priority = CacheItemPriority.Normal, long? size = null);

        /// <summary>
        /// Remove a value from the cache by key.
        /// </summary>
        void Remove(string key);

        /// <summary>
        /// Clear all values from the cache.
        /// </summary>
        void Clear();

        // async

        /// <summary>
        /// Get a value from the cache by key.
        /// </summary>
        Task<T?> GetAsync<T>(string key);

        /// <summary>
        /// Set a value in the cache by key.
        /// </summary>
        Task SetAsync<T>(string key, T value, TimeSpan expirationTime, bool useSlidingExpiration = false, CacheItemPriority priority = CacheItemPriority.Normal, long? size = null);

        // Lazy Loading
        // public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan expirationTime, CacheItemPriority priority = CacheItemPriority.Normal, long? size = null, bool useSlidingExpiration = false);
    }
}
