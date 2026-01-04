using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EntreLaunch.Interfaces;
using EntreLaunch.PayTabsHosted.API;
using EntreLaunch.PayTabsHosted.API.Services;
using EntreLaunch.PayTabsHosted.API.Types;
using EntreLaunch.PayTabsHosted.API.WebStore;

namespace EntreLaunch.Plugin.EmailSync
{
    public class PayTabsHostedPlugin : IPlugin
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(typeof(PayTabsHostedPlugin));

            // تسجيل `ApiConfiguration` مع قيم الإعدادات المناسبة
            var apiConfig = new ApiConfiguration(
                profileId: configuration.GetValue<int>("PayTabs:ProfileId"),
                serverKey: configuration.GetValue<string>("PayTabs:ServerKey"),
                clientKey: configuration.GetValue<string>("PayTabs:ClientKey"),
                region: configuration.GetValue<Region>("PayTabs:Region"),
                currency: configuration.GetValue<Currency>("PayTabs:Currency"));

            services.AddSingleton(apiConfig);
            services.AddSingleton<WebStoreClient>();
            services.AddScoped<PayTabsService>(); 
        }
    }
}
