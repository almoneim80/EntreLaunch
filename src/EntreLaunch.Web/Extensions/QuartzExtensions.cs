namespace EntreLaunch.Web.Extensions
{
    public static class QuartzExtensions
    {
        public static void AddQuartzConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var taskRunnerSchedule = configuration.GetValue<string>("TaskRunner:CronSchedule")!;

            services.AddQuartz(q =>
            {
                q.AddJob<TaskRunner>(opts => opts.WithIdentity("TaskRunner"));
                q.AddTrigger(opts =>
                    opts.ForJob("TaskRunner")
                        .WithIdentity("TaskRunnerTrigger")
                        .WithCronSchedule(taskRunnerSchedule));
            });

            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
            services.AddTransient<TaskRunner>();
        }
    }
}
