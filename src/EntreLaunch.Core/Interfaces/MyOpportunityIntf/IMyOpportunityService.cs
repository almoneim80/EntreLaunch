namespace EntreLaunch.Interfaces.MyOpportunityIntf
{
    public interface IMyOpportunityService
    {
        IOpportunityQueryService Queries { get; }
        IOpportunityRequestService Requests { get; }
        IOpportunityFilteringService Filters { get; }
    }
}
