namespace EntreLaunch.Interfaces
{
    public interface IExportService
    {
        /// <summary>
        /// Export to CSV.
        /// </summary>
        Task<GeneralResult<string>> ExportToCsvAsync<T, TDto>() where T : SharedData, new() where TDto : class;

        /// <summary>
        /// Export to Excel file.
        /// </summary>
        Task<GeneralResult<byte[]>> ExportToExcelAsync<T, TDto>() where T : SharedData, new() where TDto : class;

        /// <summary>
        /// Export to Json.
        /// </summary>
        Task<GeneralResult<string>> ExportToJsonAsync<T, TDto>() where T : SharedData, new() where TDto : class;
    }
}
