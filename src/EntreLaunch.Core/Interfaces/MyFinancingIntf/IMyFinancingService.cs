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
        Task<GeneralResult> AllRequests();

        /// <summary>
        /// Get all accepted Financing requests.
        /// </summary>
        Task<GeneralResult> AcceptedRequests();

        /// <summary>
        /// Get all rejected Financing requests.
        /// </summary>
        Task<GeneralResult> RejectedRequests();

        /// <summary>
        /// Get all pending Financing requests.
        /// </summary>
        Task<GeneralResult> PendingRequests();

        /// <summary>
        /// Progress Financing requests state (Accepted, Rejected).
        /// </summary>
        Task<GeneralResult> ProgressRequests([FromBody] ProcessOpportunityRequestDto processOpportunityRequest);
    }
}
