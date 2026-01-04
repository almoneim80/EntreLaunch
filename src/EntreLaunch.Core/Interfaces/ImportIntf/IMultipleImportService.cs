using EntreLaunch.DTOs.ImportDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Interfaces.ImportIntf
{
    public interface IMultipleImportService<TRoot, TAggregateDto>
    where TAggregateDto : BaseEntityWithId
    where TRoot : BaseEntityWithId, new()
    {
        /// <summary>
        /// import from list.
        /// </summary>
        Task<GeneralResult<ImportResult>> ImportAsync(IEnumerable<TAggregateDto> aggregates);
    }
}
