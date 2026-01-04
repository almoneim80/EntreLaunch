using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.MyOpportunityDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Interfaces.MyOpportunityIntf
{
    public interface IOpportunityRequestService
    {
        /// <summary>
        /// Send request to opportunity.
        /// </summary>
        Task<GeneralResult> SendRequest(CreateOpportunityRequestDto request);

        /// <summary>
        /// Get all opportunities requests.
        /// </summary>
        Task<GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>> AllRequests(PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Get all accepted opportunities requests.
        /// </summary>
        Task<GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>> AcceptedRequests(PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Get all rejected opportunities requests.
        /// </summary>
        Task<GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>> RejectedRequests(PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Get all pending opportunities requests.
        /// </summary>
        Task<GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>> PendingRequests(PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Progress requests state (Accepted, Rejected).
        /// </summary>
        Task<GeneralResult> ProcessRequest(ProcessOpportunityRequestDto processOpportunityRequest);

        /// <summary>
        /// Delete opportunity request.
        /// </summary>
        Task<GeneralResult> DeleteUserRequestAsync(int requestId, string userId, CancellationToken cancellationToken);

        /// <summary>
        /// Get user requests.
        /// </summary>
        Task<GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>> GetUserRequestsAsync(string userId, PaginationParams pagination, CancellationToken cancellationToken);
    }
}
