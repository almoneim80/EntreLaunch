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
        Task<GeneralResult> AllRequests();

        /// <summary>
        /// Get all accepted opportunities requests.
        /// </summary>
        Task<GeneralResult> AcceptedRequests();

        /// <summary>
        /// Get all rejected opportunities requests.
        /// </summary>
        Task<GeneralResult> RejectedRequests();

        /// <summary>
        /// Get all pending opportunities requests.
        /// </summary>
        Task<GeneralResult> PendingRequests();

        /// <summary>
        /// Progress requests state (Accepted, Rejected).
        /// </summary>
        Task<GeneralResult> ProgressRequests(ProcessOpportunityRequestDto processOpportunityRequest);
    }
}
