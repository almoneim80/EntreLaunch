using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EntreLaunch.EmailSync.Tasks;
using EntreLaunch.Interfaces;
using EntreLaunch.Interfaces.TaskIntf;

namespace EntreLaunch.Plugin.EmailSync
{
    public class EmailSyncPlugin : IPlugin
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(typeof(EmailSyncPlugin));
            services.AddScoped<ITask, EmailSyncTask>();
        }
    }
}
