namespace EntreLaunch.Web.Extensions
{
    public static class DbContextExtensions
    {
        public static void AddDatabaseContexts(this IServiceCollection services, IConfiguration configuration)
        {
            var postgresConfig = configuration.GetSection("Postgres");
            var connectionString = $"Host={postgresConfig["Server"]};" +
                                   $"Port={postgresConfig["Port"]};" +
                                   $"Database={postgresConfig["Database"]};" +
                                   $"Username={postgresConfig["UserName"]};" +
                                   $"Password={postgresConfig["Password"]}";

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
            dataSourceBuilder.EnableDynamicJson(); // أو UseJsonNet() إذا اخترت Newtonsoft.Json
            var dataSource = dataSourceBuilder.Build();

            services.AddDbContextFactory<PgDbContext>(options =>
            {
                options.UseNpgsql(dataSource).UseLazyLoadingProxies();
            });

            services.AddSingleton<EsDbContext>(); // Elasticsearch context
        }
    }
}
