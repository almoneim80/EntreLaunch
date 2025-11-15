using EntreLaunch.Interfaces.ClubIntf;
using EntreLaunch.Services.TaskSvc;

namespace EntreLaunch.Tasks
{
    public class ClubUnSubscriptionTask : BaseTask
    {
        private readonly IClubService _clubService;
        public ClubUnSubscriptionTask(
            IConfiguration configuration,
            TaskStatusService taskStatusService,
            IClubService clubService)
            : base("Tasks:ClubUnSubscriptionTask", configuration, taskStatusService)
        {
            _clubService = clubService;
        }
        public override async Task<bool> Execute(TaskExecutionLog currentJob)
        {
            try
            {
                await Task.Run(() => _clubService.ExpireClubSubscriptionsAsync());
                Log.Information($"Unsubscribed expired clubs. {currentJob.Id}");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred when unsubscribing expired clubs.");
                return false;
            }
        }
    }
}
