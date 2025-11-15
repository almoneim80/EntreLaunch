using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using EntreLaunch.Interfaces.AuthenticationIntf;

namespace EntreLaunch
{
    public class Program
    {
        private static readonly List<string> AppSettingsFiles = new List<string>();
        private static WebApplication? app;
        public static WebApplication? GetApp()
        {
            return app;
        }

        public static void AddAppSettingsJsonFile(string path)
        {
            AppSettingsFiles.Add(path);
        }

        public static async Task Main(string[] args)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

            AppSettingsFiles.ForEach(path =>
            {
                builder.Configuration.AddJsonFile(path, false, true);
                builder.Configuration.AddJsonFile("pluginsettings.json", optional: false, reloadOnChange: true);
                Log.Information("AppSettingsFile", path + "loaded.");
            });

            ConfigureLogs(builder);
            Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine(msg));

            // Initialize PluginManager with configuration
            PluginManager.Init(builder.Configuration);

            // memory cache
            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton<ICacheService, MemoryCacheService>();

            builder.Configuration.AddUserSecrets(typeof(Program).Assembly);
            builder.Configuration.AddEnvironmentVariables();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSingleton<IHttpContextHelper, HttpContextHelper>();
            builder.Services.AddTransient<IMxVerifyService, MxVerifyService>();
            builder.Services.AddTransient<IIdentityService, IdentityService>();
            builder.Services.AddTransient<IDomainService, DomainService>();
            builder.Services.AddTransient<IContactService, ContactService>();
            builder.Services.AddScoped<IVariablesService, VariablesService>();
            builder.Services.AddSingleton<IpDetailsService, IpDetailsService>();
            builder.Services.AddSingleton<ILockService, LockService>();
            builder.Services.AddScoped<IEmailValidationExternalService, EmailValidationExternalService>();
            builder.Services.AddScoped<IAccountExternalService, AccountExternalService>();
            builder.Services.AddSingleton<TaskStatusService, TaskStatusService>();
            builder.Services.AddSingleton<ActivityLogService, ActivityLogService>();
            builder.Services.AddTransient(typeof(QueryProviderFactory<>), typeof(QueryProviderFactory<>));
            builder.Services.AddTransient(typeof(ESOnlyQueryProviderFactory<>), typeof(ESOnlyQueryProviderFactory<>));
            // Email
            builder.Services.AddSingleton<IEmailService, EmailService>();
            builder.Services.AddTransient<IEmailSchedulingService, EmailSchedulingService>();
            builder.Services.AddScoped<IUserService, UserService>();

            // Export
            builder.Services.AddScoped<IExportService, ExportService>();

            // Role
            builder.Services.AddScoped<IRoleService, RoleService>();

            // Permission
            builder.Services.AddScoped<IPermissionService, PermissionService>();

            // flunt validation
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddFluentValidationClientsideAdapters();
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();

            builder.Services.AddScoped(typeof(BaseService<,,,>), typeof(BaseService<,,,>));
            builder.Services.AddScoped(typeof(IImportService<,>), typeof(ImportService<,>));
            builder.Services.AddScoped(typeof(BaseServiceWithoutEntreLaunchdate<,,>), typeof(BaseServiceWithoutEntreLaunchdate<,,>));

            builder.Services.AddScoped<ITrainingSectionService, TrainingSectionService>();
            builder.Services.AddScoped<IExtendedBaseService, ExtendedBaseService>();
            builder.Services.AddScoped<CascadeDeleteService>();

            // Localization config 01
            builder.Services.AddSingleton<ILocalizationManager, LocalizationManager>();
            builder.Services.AddSingleton<IServerConfigurationManager, ServerConfigurationManager>();
            builder.Services.Configure<LocalizationSettings>(builder.Configuration.GetSection("LocalizationSettings"));

            builder.Services.Configure<DefaultRolesConfig>(builder.Configuration.GetSection("DefaultRoles"));
            // memory cache
            builder.Services.Configure<CacheSettings>(builder.Configuration.GetSection("CacheSettings"));
            builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<DefaultRolesConfig>>().Value);

            builder.Services.Configure<EmailSenderOptions>(builder.Configuration.GetSection("EmailSender"));

            // Adding OtpVerification settings to the DI container
            builder.Services.Configure<OtpVerificationOptions>(builder.Configuration.GetSection("OtpVerification"));
            builder.Services.AddScoped<IEmailVerificationService, EmailVerificationService>();
            builder.Services.AddScoped<IEmailVerifyService, EmailVerifyService>();
            builder.Services.AddScoped<IEmailVerificationExtension, EmailVerificationExtensionService>();

            // PayTabs settings
            builder.Services.Configure<PayTabsOptions>(builder.Configuration.GetSection("PayTabsSettings"));
            builder.Services.AddHttpClient<PaytabsPaymentGateway>();
            builder.Services.AddScoped<IPaymentGateway, PaytabsPaymentGateway>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IRefundService, RefundService>();
            builder.Services.AddScoped<ILoyaltyPointsService, LoyaltyPointsService>();

            builder.Services.AddScoped<ITagService, TagService>();
            builder.Services.AddScoped<IRatingsService, RatingsService>();
            builder.Services.AddScoped<IStudentProgress, StudentProgressService>();

            // MyOpportunity
            builder.Services.AddScoped<IMyOpportunityService, MyOpportunityService>();
            builder.Services.AddScoped<IOpportunityFilteringService, OpportunityFilteringService>();
            builder.Services.AddScoped<IOpportunityQueryService, OpportunityQueryService>();
            builder.Services.AddScoped<IOpportunityRequestService, OpportunityRequestService>();


            // MyFinancingService
            builder.Services.AddScoped<IMyFinancingService, MyFinancingService>();

            // MyPartnerService
            builder.Services.AddScoped<IMyPartnerService, MyPartnerService>();
            builder.Services.AddScoped<IMyPartnerFilteringService, MyPartnerFilteringService>();
            builder.Services.AddScoped<IMyPartnerProjectService, MyPartnerProjectService>();
            builder.Services.AddScoped<IMyPartnerAttachmentService, MyPartnerAttachmentService>();

            // MyPartnerService
            builder.Services.AddScoped<IMyTeamService, MyTeamService>();

            // Consultation
            builder.Services.AddScoped<IConsultation, ConsultationService>();

            // MyCommunity
            builder.Services.AddScoped<IMyCommunityService, MyCommunityService>();

            // Simulation Service
            builder.Services.AddScoped<ISimulationService, SimulationService>();

            // Club Service
            builder.Services.AddScoped<IClubService, ClubService>();

            ConfigureCacheProfiles(builder);

            ConfigureConventions(builder);
            IdentityHelper.ConfigureAuthentication(builder);
            ConfigureControllers(builder);

            // builder.Services.AddDbContext<PgDbContext>();
            var postgresConfig = builder.Configuration.GetSection("Postgres");
            var connectionString = $"Host={postgresConfig["Server"]};Port={postgresConfig["Port"]};Database={postgresConfig["Database"]};Username={postgresConfig["UserName"]};Password={postgresConfig["Password"]}";
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
            dataSourceBuilder.EnableDynamicJson(); // أو UseJsonNet() إذا اخترت Newtonsoft.Json
            var dataSource = dataSourceBuilder.Build();

            builder.Services.AddDbContextFactory<PgDbContext>(options =>
            {
                options.UseNpgsql(dataSource).UseLazyLoadingProxies();
            });
            builder.Services.AddSingleton<EsDbContext>();

            ConfigureQuartz(builder);
            ConfigureImageEntreLaunchload(builder);
            ConfigureFileEntreLaunchload(builder);
            ConfigureIpDetailsResolver(builder);
            ConfigureEmailServices(builder);
            ConfigureTasks(builder);
            ConfigureApiSettings(builder);
            ConfigureImportSizeLimit(builder);
            ConfigureEmailVerification(builder);
            ConfigureAccountDetails(builder);
            ConfigureIdentity(builder);

            builder.Services.AddAutoMapper(x =>
            {
                x.AddProfile(new AutoMapperProfiles());
                x.AllowNullCollections = true;
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddDataProtection();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddLogging();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IUrlHelper>(factory =>
            {
                var actionContext = factory.GetRequiredService<IActionContextAccessor>().ActionContext!;
                return new UrlHelper(actionContext);
            });

            builder.Services.AddControllers(options =>
            {
                options.RespectBrowserAcceptHeader = true;
                options.ReturnHttpNotAcceptable = true;
                options.OutputFormatters.RemoveType<StringOutputFormatter>();
                options.InputFormatters.Add(new CsvInputFormatter());
                options.OutputFormatters.Add(new CsvOutputFormatter());
                options.FormatterMappings.SetMediaTypeMappingForFormat("csv", "text/csv");
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SEntreLaunchpressModelStateInvalidFilter = true;
            });

            ConfigureSwagger(builder);

            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.All;
            });

            ConfigureCORS(builder);

            builder.Services.AddHttpClient();
            builder.Services.AddScoped<IExternalAuthService, GoogleAuthService>();
            builder.Services.AddScoped<ITokenService, TokenService>();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            app = builder.Build();
            PluginManager.Init(app);

            // intialize GenericValidator
            //var localizationManager = app.Services.GetRequiredService<ILocalizationManager>();
            //GenericValidator.Initialize(localizationManager);

            // Localization config 02
            app.Services.GetRequiredService<ILocalizationManager>();

            app.UseHttpsRedirection();
            app.UseExceptionHandler("/error");
            app.UseForwardedHeaders();

            await MigrateOnStartIfRequired(app, builder);

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseCors();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            // Middlewares
            app.UseMiddleware<CultureMiddleware>();
            app.UseMiddleware<PermissionMiddleware>();

            app.Use(async (context, next) =>
            {
                Console.WriteLine($"Request Origin: {context.Request.Headers["Origin"]}");
                await next.Invoke();
            });

            app.UseDeveloperExceptionPage();

            app.UseCookiePolicy();
            app.MapControllers();

            app.Run();

            using (var scope = app.Services.CreateScope())
            {
                var esDbContext = scope.ServiceProvider.GetRequiredService<EsDbContext>();
                esDbContext.Migrate();
            }
        }

        // Create Default Identity
        public static async Task CreateDefaultIdentity(IServiceScope scope)
        {
            var defaultRoles = app!.Configuration.GetSection("DefaultRoles").Get<DefaultRolesConfig>()!;

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var defaultRole in defaultRoles)
            {
                if (!await roleManager.RoleExistsAsync(defaultRole))
                {
                    await roleManager.CreateAsync(new IdentityRole(defaultRole));
                }
            }

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var defaultUsers = app!.Configuration.GetSection("DefaultUsers").Get<DefaultUsersConfig>()!;

            foreach (var defaultUser in defaultUsers)
            {
                var user = new User
                {
                    CreatedAt = DateTime.UtcNow,
                    FirstName = defaultUser.UserName,
                    Email = defaultUser.Email,
                    EmailConfirmed = true,
                };

                if (await userManager.FindByEmailAsync(user.Email) == null)
                {
                    var result = await userManager.CreateAsync(user, defaultUser.Password);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRolesAsync(user, defaultUser.Roles);
                    }
                }
            }
        }

        // Configure import file size
        private static void ConfigureImportSizeLimit(WebApplicationBuilder builder)
        {
            var maxImportSizeConfig = builder.Configuration.GetValue<string>("ApiSettings:MaxImportSize");

            if (string.IsNullOrEmpty(maxImportSizeConfig))
            {
                throw new MissingConfigurationException("Import file size is mandatory.");
            }

            var maxImportSize = StringHelper.GetSizeInBytesFromString(maxImportSizeConfig);

            if (maxImportSize is null)
            {
                throw new MissingConfigurationException("Max import file size is invalid.");
            }

            builder.WebHost.UseKestrel(options =>
            {
                options.Limits.MaxRequestBodySize = maxImportSize;
            });
        }

        // Configure logging
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
                    .WriteTo.Elasticsearch(ConfigureELK(elasticConfig))
                    .CreateLogger();
            }

            builder.Host.UseSerilog();
        }

        // Configure ELK
        private static ElasticsearchSinkOptions ConfigureELK(ElasticConfig elasticConfig)
        {
            var uri = new Uri(elasticConfig.Url);

            return new ElasticsearchSinkOptions(uri)
            {
                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                IndexFormat = $"{elasticConfig.IndexPrefix}-logs",
                ModifyConnectionSettings = x => x.BasicAuthentication(elasticConfig.UserName, elasticConfig.Password).ServerCertificateValidationCallback((sender, certificate, chain, sslPolicyErrors) => true),
                FailureCallback = (message) => Console.WriteLine($"Failed to send log to Elasticsearch: {message.Exception!.Message}"),
                EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog
            };
        }

        // Migrate on start
        private static async Task MigrateOnStartIfRequired(WebApplication app, WebApplicationBuilder builder)
        {
            var migrateOnStart = builder.Configuration.GetValue<bool>("MigrateOnStart");

            if (migrateOnStart)
            {
                using (var scope = app.Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<PgDbContext>();

                    using (LockManager.GetWaitLock("MigrationWaitLock", context.Database.GetConnectionString()!))
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<PgDbContext>();
                        dbContext.Database.Migrate();

                        //var esDbContext = scope.ServiceProvider.GetRequiredService<EsDbContext>();
                        //esDbContext.Migrate();

                        var pluginContexts = scope.ServiceProvider.GetServices<PluginDbContextBase>();

                        foreach (var pluginContext in pluginContexts)
                        {
                            pluginContext.Database.Migrate();
                        }

                        // var elasticClient = scope.ServiceProvider.GetRequiredService<ElasticClient>();

                        await CreateDefaultIdentity(scope);

                        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                        await PermissionsSeeding.SeedAdminPermissionsAsync(roleManager);
                    }
                }
            }
        }

        // Configure conventions
        private static void ConfigureConventions(WebApplicationBuilder builder)
        {
            builder.Services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });

            builder.Services.AddControllers(options => options.Conventions.Add(new RouteTokenTransformerConvention(new RouteToKebabCase())));
        }

        // Configure controllers
        private static void ConfigureControllers(WebApplicationBuilder builder)
        {
            var controllersBuilder = builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ValidateModelStateAttribute>();
            })
            .AddJsonOptions(opts =>
            {
                JsonHelper.Configure(opts.JsonSerializerOptions);
            });

            foreach (var plugin in PluginManager.GetPluginList())
            {
                controllersBuilder = controllersBuilder.AddApplicationPart(plugin.GetType().Assembly).AddControllersAsServices();
                plugin.ConfigureServices(builder.Services, builder.Configuration);
                Log.Information($"************ >> Plugin loaded: {plugin.GetType().Assembly.FullName}");
            }
        }

        // Configure Ip Details Resolver
        private static void ConfigureIpDetailsResolver(WebApplicationBuilder builder)
        {
            var geolocationApiConfig = builder.Configuration.GetSection("GeolocationApi");

            if (geolocationApiConfig == null)
            {
                throw new MissingConfigurationException("Geo Location Api configuration is mandatory.");
            }

            builder.Services.Configure<GeolocationApiConfig>(geolocationApiConfig);
        }

        // Configure image EntreLaunchload
        private static void ConfigureImageEntreLaunchload(WebApplicationBuilder builder)
        {
            var imageEntreLaunchloadConfig = builder.Configuration.GetSection("Media");

            if (imageEntreLaunchloadConfig == null)
            {
                throw new MissingConfigurationException("Image EntreLaunchload configuration is mandatory.");
            }

            builder.Services.Configure<MediaConfig>(imageEntreLaunchloadConfig);
        }

        // Configure file EntreLaunchload
        private static void ConfigureFileEntreLaunchload(WebApplicationBuilder builder)
        {
            var fileEntreLaunchloadConfig = builder.Configuration.GetSection("File");

            if (fileEntreLaunchloadConfig == null)
            {
                throw new MissingConfigurationException("File EntreLaunchload configuration is mandatory.");
            }

            builder.Services.Configure<FileConfig>(fileEntreLaunchloadConfig);
        }

        // Configure email verification
        private static void ConfigureEmailVerification(WebApplicationBuilder builder)
        {
            var emailVerificationConfig = builder.Configuration.GetSection("EmailVerificationApi");

            if (emailVerificationConfig == null)
            {
                throw new MissingConfigurationException("Email Verification Api configuration is mandatory.");
            }

            builder.Services.Configure<EmailVerificationApiConfig>(emailVerificationConfig);
        }

        // Configure identity
        private static void ConfigureIdentity(WebApplicationBuilder builder)
        {
            var jwtConfig = builder.Configuration.GetSection("Jwt");

            if (jwtConfig == null)
            {
                throw new MissingConfigurationException("Jwt configuration is mandatory.");
            }

            builder.Services.Configure<JwtConfig>(jwtConfig);
        }

        // Configure account details
        private static void ConfigureAccountDetails(WebApplicationBuilder builder)
        {
            var accountDetailsApiConfig = builder.Configuration.GetSection("AccountDetailsApi");

            if (accountDetailsApiConfig == null)
            {
                throw new MissingConfigurationException("Account Details Api configuration is mandatory.");
            }

            builder.Services.Configure<AccountDetailsApiConfig>(accountDetailsApiConfig);
        }

        // Configure api settings
        private static void ConfigureApiSettings(WebApplicationBuilder builder)
        {
            var apiSettingsConfig = builder.Configuration.GetSection("ApiSettings");

            if (apiSettingsConfig == null)
            {
                throw new MissingConfigurationException("Api settings configuration is mandatory.");
            }

            builder.Services.Configure<ApiSettingsConfig>(apiSettingsConfig);
        }

        // Configure swagger
        private static void ConfigureSwagger(WebApplicationBuilder builder)
        {
            var openApiInfo = new OpenApiInfo()
            {
                Version = typeof(Program).Assembly.GetName().Version!.ToString() ?? "1.2.0",
                Title = "EntreLaunch API",
                Description = "EntreLaunch Backend API",
            };
            var swaggerConfigurators = from p in PluginManager.GetPluginList()
                                       where p is ISwaggerConfigurator
                                       select p as ISwaggerConfigurator;

            builder.Services.AddSwaggerGen(config =>
            {
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                config.IncludeXmlComments(xmlPath);

                foreach (var swaggerConfigurator in swaggerConfigurators)
                {
                    swaggerConfigurator.ConfigureSwagger(config, openApiInfo);
                }

                config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "Copy 'Bearer ' + valid JWT token into field",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                });

                config.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference()
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                },
            });

                config.EnableAnnotations();

                config.SEntreLaunchportNonNullableReferenceTypes();

                config.SchemaFilter<CustomSwaggerScheme>();

                config.UseInlineDefinitionsForEnums();

                config.SwaggerDoc("v1", openApiInfo);

                var conf = builder.Configuration.GetSection("Entities").Get<EntitiesConfig>();
                config.DocumentFilter<SwaggerEntitiesFilter>(conf);
            });
        }

        // Configure Quartz
        private static void ConfigureQuartz(WebApplicationBuilder builder)
        {
            var taskRunnerSchedule = builder.Configuration.GetValue<string>("TaskRunner:CronSchedule")!;

            builder.Services.AddQuartz(q =>
            {
                q.AddJob<TaskRunner>(opts => opts.WithIdentity("TaskRunner"));
                q.AddTrigger(opts =>
                opts.ForJob("TaskRunner").WithIdentity("TaskRunnerTrigger").WithCronSchedule(taskRunnerSchedule));
            });

            builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

            builder.Services.AddTransient<TaskRunner>();
        }

        // Configure cache
        private static void ConfigureCacheProfiles(WebApplicationBuilder builder)
        {
            var cacheProfiles = builder.Configuration.GetSection("CacheProfiles").Get<List<CacheProfileSettings>>();

            if (cacheProfiles == null)
            {
                throw new MissingConfigurationException("Cache Profiles configuration is mandatory.");
            }

            builder.Services.AddControllers(options =>
            {
                foreach (var item in cacheProfiles)
                {
                    options.CacheProfiles.Add(
                        item!.Type!,
                        new CacheProfile()
                        {
                            Duration = item!.DurationInDays!,
                            VaryByHeader = item!.VaryByHeader!,
                        });
                }
            });
        }

        // Configure email
        private static void ConfigureEmailServices(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IEmailWithLogService, EmailWithLogService>();
            builder.Services.AddScoped<IEmailFromTemplateService, EmailFromTemplateService>();
        }

        // configure tasks
        private static void ConfigureTasks(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ITask, SyncEsTask>();
            builder.Services.AddScoped<ITask, SyncIpDetailsTask>();
            builder.Services.AddScoped<ITask, DomainVerificationTask>();
            builder.Services.AddScoped<ITask, ContactScheduledEmailTask>();
            builder.Services.AddScoped<ITask, SyncEmailLogTask>();
            builder.Services.AddScoped<ITask, HardDeleteTask>();
        }

        // Configure CORS
        private static void ConfigureCORS(WebApplicationBuilder builder)
        {
            var corsSettings = builder.Configuration.GetSection("Cors").Get<CorsConfig>();

            if (corsSettings == null)
            {
                throw new MissingConfigurationException("CORS configuration is mandatory.");
            }

            if (!corsSettings.AllowedOrigins.Any())
            {
                throw new MissingConfigurationException("Specify CORS allowed domains (Use '*' only in development).");
            }

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyMethod().AllowAnyHeader();

                    if (builder.Environment.IsDevelopment())
                    {
                        // Allow all domains in the development environment
                        policy.SetIsOriginAllowed(origin => true);
                    }
                    else
                    {
                        // Allow only authorized domains in the production environment
                        if (corsSettings.AllowedOrigins.Contains("*"))
                        {
                            throw new InvalidOperationException("Using '*' is not allowed in production with AllowCredentials.");
                        }
                        else
                        {
                            policy.WithOrigins(corsSettings.AllowedOrigins.ToArray()).AllowCredentials();
                        }
                    }
                });
            });
        }
    }
}
