namespace EntreLaunch.Interfaces.ImportIntf
{
    public interface IImportService<T, TI>
    where TI : BaseEntityWithId
    where T : BaseEntityWithId, new()
    {
        /// <summary>
        /// import from list.
        /// </summary>
        Task<GeneralResult<ImportResult>> ImportFromListAsync(List<TI> importRecords);

        /// <summary>
        /// import from file.
        /// </summary>
        Task<GeneralResult<ImportResult>> ImportFromFileAsync(IFormFile file);

        /// <summary>
        /// generate template as bytes array for csv.
        /// </summary>
        Task<GeneralResult<byte[]>> GenerateCsvTemplateAsync<TDto>() where TDto : class;

        /// <summary>
        /// generate template as bytes array for excel.
        /// </summary>
        Task<GeneralResult<byte[]>> GenerateExcelTemplateAsync<TDto>() where TDto : class;
    }
}
