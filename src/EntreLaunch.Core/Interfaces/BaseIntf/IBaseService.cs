namespace EntreLaunch.Interfaces.BaseIntf
{
    public interface IBaseService<T>
    where T : class
    {
        /// <summary>
        /// Get all records.
        /// </summary>
        Task<List<T>> GetAllAsync();

        /// <summary>
        /// Get one record.
        /// </summary>
        Task<T?> GetOneAsync(int id);

        /// <summary>
        /// Get paged records.
        /// </summary>
        Task<(List<T> Items, int TotalCount)> GetPagedAsync(int? page = null, int? pageSize = null);

        /// <summary>
        /// Create a new record.
        /// </summary>
        Task<T> CreateAsync(T entity);

        /// <summary>
        /// Update a record.
        /// </summary>
        Task<T?> UpdateAsync(int id, T entity);

        /// <summary>
        /// Delete a record.
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Check if elastic search is supported.
        /// </summary>
        /// <returns></returns>
        bool SupportsElastic();
    }
}
