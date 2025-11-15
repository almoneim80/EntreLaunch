namespace EntreLaunch.Interfaces.MyOpportunityIntf
{
    public interface IOpportunityFilteringService
    {
        /// <summary>
        /// Filter opportunities.
        /// </summary>
        Task<GeneralResult> Filtering(OpportunityFilterDto filter);
    }
}
