namespace EntreLaunch.Plugin.SmartExam;
public class SmartExamPlugin : IPlugin
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(typeof(SmartExamPlugin));
        services.AddScoped<IExamService, ExamService>();
    }
}
