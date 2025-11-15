using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace EntreLaunch.Identity;

public class GoogleCookieEventsHandler : CookieAuthenticationEvents
{
    public override async Task SigningIn(CookieSigningInContext context)
    {
        var identityService = context.HttpContext.RequestServices.GetService<IIdentityService>()!;
        var signInManager = context.HttpContext.RequestServices.GetService<SignInManager<User>>()!;
        var userManager = context.HttpContext.RequestServices.GetService<UserManager<User>>()!;

        var email = context.Principal?.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value ?? string.Empty;

        if (string.IsNullOrWhiteSpace(email))
        {
            await RedirectToAccessDenied(((PropertiesContext<CookieAuthenticationOptions>)context as RedirectContext<CookieAuthenticationOptions>)!);
            return;
        }

        var user = await identityService.FindOnRegister(email);
        if (user == null)
        {
            await RedirectToAccessDenied(((PropertiesContext<CookieAuthenticationOptions>)context as RedirectContext<CookieAuthenticationOptions>)!);
            return;
        }

        user.Data!.LastTimeLoggedIn = DateTime.UtcNow;
        await userManager.EntreLaunchdateAsync(user.Data);

        var userPrincipal = await signInManager.CreateUserPrincipalAsync(user.Data);

        await signInManager.SignInAsync(user.Data, false, "GoogleCookie");

        context.HttpContext.User = userPrincipal;
    }
}
