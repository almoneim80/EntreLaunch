using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.MyOpportunityDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Interfaces.MyFinancingIntf
{
    public interface IMyFinancingService
    {
        /// <summary>
        /// Get all financing opportunities.
        /// </summary>
        Task<GeneralResult> AllFinancingOpportunities();

        /// <summary>
        /// Filter financing companies.
        /// </summary>
        Task<GeneralResult> Filtering([FromBody] OpportunityFilterDto filter);

        /// <summary>
        /// Send request to company.
        /// </summary>
        Task<GeneralResult> SendRequest([FromBody] CreateOpportunityRequestDto request);

        /// <summary>
        /// Get all financing requests.
        /// </summary>
        Task<GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>> AllRequests(PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Get all accepted Financing requests.
        /// </summary>
        Task<GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>> AcceptedRequests(PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Get all rejected Financing requests.
        /// </summary>
        Task<GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>> RejectedRequests(PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Get all pending Financing requests.
        /// </summary>
        Task<GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>> PendingRequests(PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Progress Financing requests state (Accepted, Rejected).
        /// </summary>
        Task<GeneralResult> ProcessRequests([FromBody] ProcessOpportunityRequestDto processOpportunityRequest);

        /// <summary>
        /// Get all sectors.
        /// </summary>
        Task<GeneralResult<List<string>>> GetAllSectorsAsync();

        /// <summary>
        /// Get all costs.
        /// </summary>
        Task<GeneralResult<List<decimal>>> GetAllCostsAsync();

        /// <summary>
        /// Delete Financing request.
        /// </summary>
        Task<GeneralResult> DeleteUserRequestAsync(int requestId, string userId, CancellationToken cancellationToken);

        /// <summary>
        /// Get all financing requests by user.
        /// </summary>
        Task<GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>> GetUserRequestsAsync(string userId, PaginationParams pagination, CancellationToken cancellationToken);
    }
}
