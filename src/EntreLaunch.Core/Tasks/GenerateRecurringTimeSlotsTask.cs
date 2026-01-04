using EntreLaunch.Services.TaskSvc;

namespace EntreLaunch.Tasks
{
    public class GenerateRecurringTimeSlotsTask(
        IConfiguration configuration,
        TaskStatusService taskStatusService,
        ICounselorService consultationTimeService) : BaseTask("Tasks:GenerateRecurringTimeSlotsTask", configuration, taskStatusService)
    {
        private readonly ICounselorService _consultationTimeService = consultationTimeService;

        public override async Task<bool> Execute(TaskExecutionLog currentJob)
        {
            try
            {
                await _consultationTimeService.GenerateDailyRecurringTimeSlots();
                Log.Information($"Recurring time slots generated successfully. {currentJob.Id}");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error generating recurring time slots. {currentJob.Id}");
                return false;
            }
        }
    }
}
