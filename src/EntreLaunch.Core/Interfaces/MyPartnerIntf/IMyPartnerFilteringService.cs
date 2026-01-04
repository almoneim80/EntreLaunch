using EntreLaunch.DTOs.MyPartnerDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Interfaces.MyPartnerIntf
{
    public interface IMyPartnerFilteringService
    {
        /// <summary>
        /// Filter Projects.
        /// </summary>
        Task<GeneralResult> Filtering([FromBody] FilterProjectsDto filter);

        /// <summary>
        /// Get all activities.
        /// </summary>
        Task<GeneralResult<List<string>>> GetAllActivitiesAsync();
    }
}
