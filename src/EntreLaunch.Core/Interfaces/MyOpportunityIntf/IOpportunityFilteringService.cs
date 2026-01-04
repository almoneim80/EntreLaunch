using EntreLaunch.DTOs.MyOpportunityDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Interfaces.MyOpportunityIntf
{
    public interface IOpportunityFilteringService
    {
        /// <summary>
        /// Filter opportunities.
        /// </summary>
        Task<GeneralResult> Filtering(OpportunityFilterDto filter);

        /// <summary>
        /// Get all sectors.
        /// </summary>
        Task<GeneralResult<List<string>>> GetAllSectorsAsync();

        /// <summary>
        /// Get all costs.
        /// </summary>
        Task<GeneralResult<List<decimal>>> GetAllCostsAsync();
    }
}
