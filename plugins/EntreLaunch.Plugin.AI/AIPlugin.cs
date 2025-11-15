using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EntreLaunch.Interfaces;
using EntreLaunch.Plugin.AI.Service;

namespace EntreLaunch.Plugin.EmailSync
{
    public class AIPlugin : IPlugin
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(typeof(AIPlugin));
            services.AddSingleton<DifyApiClient>(provider => new DifyApiClient("app-oSv7mGba5IfBF1VlfxYsSMrH"));
        }
    }
}
