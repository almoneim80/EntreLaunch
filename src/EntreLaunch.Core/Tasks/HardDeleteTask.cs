using EntreLaunch.Services.TaskSvc;

namespace EntreLaunch.Tasks;
public class HardDeleteTask : BaseTask
{
    private readonly CascadeDeleteService _deleteService;
    public HardDeleteTask(
        IConfiguration configuration,
        TaskStatusService taskStatusService,
        CascadeDeleteService deleteService)
        : base("Tasks:HardDeleteTask", configuration, taskStatusService)
    {
        _deleteService = deleteService;
    }

    public override async Task<bool> Execute(TaskExecutionLog currentJob)
    {
        try
        {
            await Task.Run(() => _deleteService.HardDeleteExpiredEntitiesAsync<User>());
            Log.Information($"Hard deleted expired entities. {currentJob.Id}");
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"No expired entities found for hard delete. {currentJob.Id}");
            return false;
        }
    }
}
