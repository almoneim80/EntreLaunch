using Microsoft.Extensions.Hosting;

namespace EntreLaunch.Web.Extensions
{
    public static class CorsExtensions
    {
        public static void AddCorsConfiguration(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            var corsSettings = configuration.GetSection("Cors").Get<CorsConfig>();

            if (corsSettings == null)
                throw new MissingConfigurationException("CORS configuration is mandatory.");

            if (!corsSettings.AllowedOrigins.Any())
                throw new MissingConfigurationException("Specify CORS allowed domains (Use '*' only in development).");

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyMethod().AllowAnyHeader();

                    if (environment.IsDevelopment())
                    {
                        // Allow all in development
                        policy.SetIsOriginAllowed(_ => true);
                    }
                    else
                    {
                        if (corsSettings.AllowedOrigins.Contains("*"))
                            throw new InvalidOperationException("Using '*' is not allowed in production with AllowCredentials.");

                        policy.WithOrigins(corsSettings.AllowedOrigins.ToArray())
                              .AllowCredentials();
                    }
                });
            });
        }
    }
}
