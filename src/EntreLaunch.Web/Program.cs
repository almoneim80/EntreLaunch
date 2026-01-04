using EntreLaunch.Web.Filters;

namespace EntreLaunch
{
    public class Program
    {
        private static readonly List<string> AppSettingsFiles = new List<string>();
        private static WebApplication? app;
        public static WebApplication? GetApp() => app;

        public static void AddAppSettingsJsonFile(string path) => AppSettingsFiles.Add(path);

        public static async Task Main(string[] args)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

            AppSettingsFiles.ForEach(path =>
            {
                builder.Configuration.AddJsonFile(path, false, true);
                builder.Configuration.AddJsonFile("pluginsettings.json", optional: false, reloadOnChange: true);
                Log.Information("AppSettingsFile", path + " loaded.");
            });

            ConfigureLogs(builder);
            Serilog.Debugging.SelfLog.Enable(Console.WriteLine);

            PluginManager.Init(builder.Configuration);

            builder.Configuration.AddUserSecrets(typeof(Program).Assembly);
            builder.Configuration.AddEnvironmentVariables();

            builder.Services.AddMemoryCache();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHttpClient<PaytabsPaymentGateway>();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddDataProtection();
            builder.Services.AddLogging();

            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddFluentValidationClientsideAdapters();
            builder.Services.AddValidatorsFromAssembly(typeof(EntreLaunch.Validations.GenericValidator).Assembly);

            // Modular service registrations
            builder.Services.AddProjectServices(builder.Configuration);
            builder.Services.AddApiSettings(builder.Configuration);
            builder.Services.AddDatabaseContexts(builder.Configuration);
            builder.Services.AddQuartzConfiguration(builder.Configuration);
            builder.Services.AddSwaggerConfiguration(builder.Configuration);
            builder.Services.AddCorsConfiguration(builder.Configuration, builder.Environment);

            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<AutoMapperProfiles>();
                cfg.AllowNullCollections = true;
            });

            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.All;
            });

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            builder.Services.AddControllers(options =>
            {
                options.RespectBrowserAcceptHeader = true;
                options.ReturnHttpNotAcceptable = true;
                options.OutputFormatters.RemoveType<StringOutputFormatter>();
                options.InputFormatters.Add(new CsvInputFormatter());
                options.OutputFormatters.Add(new CsvOutputFormatter());
                options.FormatterMappings.SetMediaTypeMappingForFormat("csv", "text/csv");
            }).ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            ConfigureCacheProfiles(builder);
            ConfigureConventions(builder);
            IdentityHelper.ConfigureAuthentication(builder);
            ConfigureControllers(builder);

            app = builder.Build();
            PluginManager.Init(app);

            await app.ApplyMigrationsAndSeedAsync();

            // seed static content
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<PgDbContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                await StaticContentSeeder.SeedAsync(db, logger);
            }

            app.UseAppPipeline();

            app.Run();
        }

        // Keeping logging config here
        private static void ConfigureLogs(WebApplicationBuilder builder)
        {
            var elasticConfig = builder.Configuration.GetSection("Elastic").Get<ElasticConfig>();

            if (elasticConfig == null || string.IsNullOrEmpty(elasticConfig.Server))
            {
                Console.WriteLine("ElasticSearch configuration is missing. Falling back to console logging only.");
                Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .Enrich.WithExceptionDetails()
                    .WriteTo.Console()
                    .CreateLogger();
            }
            else
            {
                Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .Enrich.WithExceptionDetails()
                    .WriteTo.Console()
                    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticConfig.Url))
                    {
                        AutoRegisterTemplate = true,
                        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                        IndexFormat = $"{elasticConfig.IndexPrefix}-logs",
                        ModifyConnectionSettings = x => x
                            .BasicAuthentication(elasticConfig.UserName, elasticConfig.Password)
                            .ServerCertificateValidationCallback((_, _, _, _) => true),
                        FailureCallback = msg => Console.WriteLine($"Failed to send log: {msg.Exception?.Message}"),
                        EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog
                    })
                    .CreateLogger();
            }

            builder.Host.UseSerilog();
        }

        private static void ConfigureCacheProfiles(WebApplicationBuilder builder)
        {
            var cacheProfiles = builder.Configuration.GetSection("CacheProfiles").Get<List<CacheProfileSettings>>();
            if (cacheProfiles == null) throw new MissingConfigurationException("Cache Profiles configuration is mandatory.");

            builder.Services.AddControllers(options =>
            {
                foreach (var profile in cacheProfiles)
                {
                    options.CacheProfiles.Add(profile.Type!, new CacheProfile
                    {
                        Duration = profile.DurationInDays!,
                        VaryByHeader = profile.VaryByHeader!,
                    });
                }
            });
        }

        private static void ConfigureConventions(WebApplicationBuilder builder)
        {
            builder.Services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });

            builder.Services.AddControllers(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(new RouteToKebabCase()));
            });
        }

        private static void ConfigureControllers(WebApplicationBuilder builder)
        {
            var controllersBuilder = builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ValidateModelStateAttribute>();
                options.Filters.Add<ConvertDateTimesToUtcFilter>();
            }).AddJsonOptions(opts => JsonHelper.Configure(opts.JsonSerializerOptions));

            foreach (var plugin in PluginManager.GetPluginList())
            {
                controllersBuilder = controllersBuilder
                    .AddApplicationPart(plugin.GetType().Assembly)
                    .AddControllersAsServices();
                plugin.ConfigureServices(builder.Services, builder.Configuration);
                Log.Information($"************ >> Plugin loaded: {plugin.GetType().Assembly.FullName}");
            }
        }
    }
}
