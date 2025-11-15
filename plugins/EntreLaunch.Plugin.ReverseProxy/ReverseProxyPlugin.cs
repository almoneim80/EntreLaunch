using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EntreLaunch.Exceptions;
using EntreLaunch.Interfaces;

namespace EntreLaunch.Plugin.ReverseProxy;

public class ReverseProxyPlugin : IPlugin, IPluginApplication
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var proxyConfig = configuration.GetSection("YARPSettings");
        if (proxyConfig == null)
        {
            throw new MissingConfigurationException("YARP configuration is mandatory.");
        }

        services.AddAuthorization(options =>
        {
            options.AddPolicy("ProxyAuth", policy =>
                policy.RequireAuthenticatedUser().AddAuthenticationSchemes("Identity.Application"));
        });

        services.AddReverseProxy().LoadFromConfig(proxyConfig);
    }

    public void ConfigureApplication(IApplicationBuilder application)
    {
        var app = (WebApplication)application;
        app.MapReverseProxy();
    }
}
