using Microsoft.Net.Http.Headers;

namespace EntreLaunch.Helpers
{
    public class HttpContextHelper(IHttpContextAccessor httpContextAccessor) : IHttpContextHelper
    {
        private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

        public HttpRequest Request => httpContextAccessor?.HttpContext?.Request!;

        // IPAddress of the client machine.
        public string? IpAddress => httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString();

        // User agent of the client machine.
        public string? UserAgent => httpContextAccessor?.HttpContext?.Request?.Headers[HeaderNames.UserAgent];

        // IPv4 address of the client machine.
        public string? IpAddressV4 => httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.MapToIPv4().ToString();

        // IPv6 address of the client machine.
        public string? IpAddressV6 => httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.MapToIPv6().ToString();

        // Get the current user.
        public async Task<User?> GetCurrentUserAsync()
        {
            var userManager = httpContextAccessor?.HttpContext?.RequestServices.GetService<UserManager<User>>()!;

            return await UserHelper.GetCurrentUserAsync(userManager, httpContextAccessor?.HttpContext?.User);
        }

        // Get the current user id.
        public async Task<string?> GetCurrentUserIdAsync()
        {
            var userManager = httpContextAccessor?.HttpContext?.RequestServices.GetService<UserManager<User>>()!;

            return await UserHelper.GetCurrentUserIdAsync(userManager, httpContextAccessor?.HttpContext?.User);
        }
    }
}
