namespace EntreLaunch.Web.Extensions
{
    public static class MigrationExtensions
    {
        public static async Task ApplyMigrationsAndSeedAsync(this WebApplication app)
        {
            var config = app.Configuration;
            var shouldMigrate = config.GetValue<bool>("MigrateOnStart");

            if (!shouldMigrate) return;

            using var scope = app.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<PgDbContext>();

            using (LockManager.GetWaitLock("MigrationWaitLock", context.Database.GetConnectionString()!))
            {
                // Run EF Core migration
                await context.Database.MigrateAsync();

                // Migrate plugins
                var pluginContexts = scope.ServiceProvider.GetServices<PluginDbContextBase>();
                foreach (var pluginContext in pluginContexts)
                {
                    pluginContext.Database.Migrate();
                }

                // Seed identity and roles
                await CreateDefaultIdentityAsync(scope);

                // Seed permissions
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                await PermissionsSeeding.SeedRolePermissionsAsync(roleManager);
            }
        }

        private static async Task CreateDefaultIdentityAsync(IServiceScope scope)
        {
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in AppRoles.All)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var defaultUsers = config.GetSection("DefaultUsers").Get<DefaultUsersConfig>()!;

            foreach (var userDef in defaultUsers)
            {
                var user = new User
                {
                    CreatedAt = DateTime.UtcNow,
                    FirstName = userDef.UserName,
                    Email = userDef.Email,
                    EmailConfirmed = true,
                };

                if (await userManager.FindByEmailAsync(user.Email) == null)
                {
                    var result = await userManager.CreateAsync(user, userDef.Password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRolesAsync(user, userDef.Roles);
                    }
                }
            }
        }
    }
}
