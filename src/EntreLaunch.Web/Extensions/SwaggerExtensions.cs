using Microsoft.OpenApi.Models;

namespace EntreLaunch.Web.Extensions
{
    public static class SwaggerExtensions
    {
        public static void AddSwaggerConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var openApiInfo = new OpenApiInfo
            {
                Version = typeof(Program).Assembly.GetName().Version?.ToString() ?? "1.0.0",
                Title = "EntreLaunch API",
                Description = "EntreLaunch Backend API",
            };

            var swaggerConfigurators = PluginManager.GetPluginList()
                .OfType<ISwaggerConfigurator>();

            services.AddSwaggerGen(config =>
            {
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                config.IncludeXmlComments(xmlPath);

                foreach (var plugin in swaggerConfigurators)
                {
                    plugin.ConfigureSwagger(config, openApiInfo);
                }

                config.SwaggerDoc("v1", openApiInfo);

                config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Copy 'Bearer ' + valid JWT token into field",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                config.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });

                config.EnableAnnotations();
                config.SupportNonNullableReferenceTypes();
                config.SchemaFilter<CustomSwaggerScheme>();
                config.UseInlineDefinitionsForEnums();

                var entitiesConfig = configuration.GetSection("Entities").Get<EntitiesConfig>();
                config.DocumentFilter<SwaggerEntitiesFilter>(entitiesConfig);
            });
        }
    }
}
