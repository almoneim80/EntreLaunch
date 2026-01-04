namespace EntreLaunch.Services.MyOpportunitySvc
{
    public class MyOpportunityService : IMyOpportunityService
    {
        public IOpportunityQueryService Queries { get; }
        public IOpportunityRequestService Requests { get; }
        public IOpportunityFilteringService Filters { get; }
        public MyOpportunityService(
            IOpportunityQueryService queryService,
            IOpportunityRequestService requestService,
            IOpportunityFilteringService filterService)
        {
            Queries = queryService;
            Requests = requestService;
            Filters = filterService;
        }
    }
}
