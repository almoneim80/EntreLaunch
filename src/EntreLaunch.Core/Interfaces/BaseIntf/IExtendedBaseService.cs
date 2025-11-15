namespace EntreLaunch.Interfaces
{
    public interface IExtendedBaseService
    {
        /// <summary>
        /// Checks if the entity exists and is not deleted.
        /// </summary>
        Task<GeneralResult<bool>> IsEntityExistsAndNotDeletedAsync<TEntity>(int entityId) where TEntity : SharedData;

        /// <summary>
        /// Checks if the entity exists and is not deleted.
        /// </summary>
        Task AddEntityAsync<T>(T entity) where T : class;

        /// <summary>
        /// Checks if the entity exists and is not deleted.
        /// </summary>
        IEnumerable<EnumData> GetEnumValues<T>() where T : Enum;
    }
}
