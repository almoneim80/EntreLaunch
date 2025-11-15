namespace EntreLaunch.Interfaces.MyOpportunityIntf
{
    public interface IOpportunityQueryService
    {
        /// <summary>
        /// Get all investment opportunities.
        /// </summary>
        Task<GeneralResult> AllInvestmentOpportunities();
    }
}
