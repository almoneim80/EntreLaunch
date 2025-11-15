using EntreLaunch.Services.TaskSvc;

namespace EntreLaunch.Tasks
{
    public class CleanEntreLaunchRefreshTokensTask : BaseTask
    {
        private readonly PgDbContext _dbContext;
        private readonly ILogger<CleanEntreLaunchRefreshTokensTask> _logger;

        public CleanEntreLaunchRefreshTokensTask(
            IConfiguration configuration,
            TaskStatusService taskStatusService,
            CascadeDeleteService deleteService,
            PgDbContext dbContext,
            ILogger<CleanEntreLaunchRefreshTokensTask> logger)
            : base("Tasks:CleanEntreLaunchRefreshTokensTask", configuration, taskStatusService)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public override async Task<bool> Execute(TaskExecutionLog currentJob)
        {
            try
            {
                var cutoffDate = DateTimeOffset.UtcNow.AddMonths(-3);

                // choose refresh tokens that:
                // 1) Expiration < Now
                // 2) or CreatedAt < Now - 3 months
                var tokensToRemove = _dbContext.RefreshTokens
                    .Where(rt => (rt.Expiration < DateTimeOffset.UtcNow) || (rt.CreatedAt < cutoffDate))
                    .ToList();

                if (tokensToRemove.Any())
                {
                    _dbContext.RefreshTokens.RemoveRange(tokensToRemove);
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation("Clean EntreLaunch Refresh tokens Task: Removed {Count} old/expired refresh tokens.", tokensToRemove.Count);
                    return true;
                }
                else
                {
                    _logger.LogInformation("CleanEntreLaunchRefreshTokensTask: No old refresh tokens to remove.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Clean EntreLaunch Refresh tokens Task");
                return false;
            }
        }
    }
}
