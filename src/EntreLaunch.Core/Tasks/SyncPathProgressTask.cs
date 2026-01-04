using EntreLaunch.Services.TaskSvc;

namespace EntreLaunch.Tasks
{
    public class SyncPathProgressTask(
        IConfiguration configuration,
        TaskStatusService taskStatusService,
        IStudentProgress progressService) : BaseTask("Tasks:SyncPathProgressTask", configuration, taskStatusService)
    {
        private readonly IStudentProgress _progressService = progressService;
        private readonly IConfiguration _configuration = configuration;

        public override async Task<bool> Execute(TaskExecutionLog currentJob)
        {
            try
            {
                var pathId = _configuration.GetValue<int>("Tasks:SyncPathProgressTask:PathId");

                var result = await _progressService.SyncAllUserProgressForPathAsync(pathId, CancellationToken.None);

                Log.Information($"Path progress sync completed for pathId={pathId}. TaskLogId={currentJob.Id}. Success={result.IsSuccess}");
                return result.IsSuccess ?? false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error syncing path progress. TaskLogId={currentJob.Id}");
                return false;
            }
        }
    }   
}
