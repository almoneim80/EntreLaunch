using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.MyOpportunityDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Interfaces.MyOpportunityIntf
{
    public interface IOpportunityQueryService
    {
        /// <summary>
        /// Get all investment opportunities.
        /// </summary>
        Task<GeneralResult<PaginatedResult<OpportunityDetailsDto>>> AllInvestmentOpportunities(PaginationParams pagination, CancellationToken cancellationToken);
    }
}
