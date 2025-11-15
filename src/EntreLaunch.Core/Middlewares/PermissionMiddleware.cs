namespace EntreLaunch.Middlewares
{
    public class PermissionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly ILocalizationManager? _localization;

        public PermissionMiddleware(RequestDelegate next, IMemoryCache cache, ILocalizationManager? localization)
        {
            _next = next;
            _cache = cache;
            _localization = localization;
        }

        public async Task Invoke(HttpContext context, IPermissionService permissionService)
        {
            var endpoint = context.GetEndpoint();
            var requiredPermission = endpoint?.Metadata.GetMetadata<RequiredPermissionAttribute>()?.Permission;

            if (string.IsNullOrEmpty(requiredPermission))
            {
                await _next(context);
                return;
            }

            var userManager = context.RequestServices.GetService<UserManager<User>>();
            if (userManager == null)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync(_localization!.GetLocalizedString("UserManagerNotRegistered"));
                return;
            }

            var user = await userManager.GetUserAsync(context.User);
            if (user == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync(_localization!.GetLocalizedString("UserNotAuthenticated"));
                return;
            }

            // Using permission caching
            var cacheKey = $"UserPermissions_{user.Id}";
            if (!_cache.TryGetValue(cacheKey, out HashSet<string>? permissions))
            {
                var result = await permissionService.GetPermissionsForUserAsync(user.Id);
                permissions = result.Data?.ToHashSet();
                _cache.Set(cacheKey, permissions, TimeSpan.FromMinutes(10)); // Temporary permissions for 10 minutes
            }

            if (!permissions!.Contains(requiredPermission))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                var message = _localization!.GetLocalizedString("MissingPermission").Replace("{requiredPermission}", requiredPermission);
                var resultmessage = new
                {
                    Error = message,
                    Permission = requiredPermission
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(resultmessage));
                return;
            }

            await _next(context);
        }
    }
}
