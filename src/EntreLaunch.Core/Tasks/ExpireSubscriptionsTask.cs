using EntreLaunch.Interfaces.SubscriptionIntf;
using EntreLaunch.Services.TaskSvc;
namespace EntreLaunch.Tasks
{
    public class ExpireSubscriptionsTask : BaseTask
    {
        private readonly ISubscriptionService _subscriptionService;

        public ExpireSubscriptionsTask(
            IConfiguration configuration,
            TaskStatusService taskStatusService,
            ISubscriptionService subscriptionService)
            : base("Tasks:ExpireSubscriptionsTask", configuration, taskStatusService)
        {
            _subscriptionService = subscriptionService;
        }

        public override async Task<bool> Execute(TaskExecutionLog currentJob)
        {
            try
            {
                await _subscriptionService.MarkAsExpiredAsync();
                Log.Information("ExpireSubscriptionsTask executed successfully. TaskId: {TaskId}", currentJob.Id);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while executing ExpireSubscriptionsTask. TaskId: {TaskId}", currentJob.Id);
                return false;
            }
        }
    }
}
