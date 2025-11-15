namespace EntreLaunch.Interfaces.BaseIntf
{
    public interface IBaseServiceWithoutEntreLaunchdate<T>
    where T : class
    {
        /// <summary>
        /// Retrieves all entities from the database that are not marked as deleted.
        /// </summary>
        Task<List<T>> GetAllAsync();

        /// <summary>
        /// Retrieves a specific entity from the database that is not marked as deleted.
        /// </summary>
        Task<T?> GetOneAsync(int id);

        /// <summary>
        /// Retrieves a paged list of entities from the database that are not marked as deleted.
        /// </summary>
        Task<(List<T> Items, int TotalCount)> GetPagedAsync(int? page = null, int? pageSize = null);

        /// <summary>
        /// Creates a new entity in the database.
        /// </summary>
        Task<T> CreateAsync(T entity);

        /// <summary>
        /// Deletes a specific entity from the database.
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Exports all entities from the database to a CSV file.
        /// </summary>
        Task<List<T>> ExportToCsvAsync();

        /// <summary>
        /// Exports all entities from the database to an Excel file.
        /// </summary>
        Task<List<T>> ExportToExcelAsync();

        /// <summary>
        /// Exports all entities from the database to a JSON file.
        /// </summary>
        Task<List<T>> ExportToJsonAsync();
    }
}
