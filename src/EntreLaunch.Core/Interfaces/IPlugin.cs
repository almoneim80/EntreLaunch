namespace EntreLaunch.Interfaces;

public interface IPlugin
{
    /// <summary>
    /// Configure the application services and middleware used in the application.
    /// </summary>
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration);
}

public interface IPluginApplication
{
    /// <summary>
    /// Configure the application services and middleware used in the application.
    /// </summary>
    public void ConfigureApplication(IApplicationBuilder application);
}
