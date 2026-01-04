

namespace EntreLaunch.Web.Controllers.AuthenticationAPI
{
    [ApiController]
    public abstract class AuthenticatedController(ILocalizationManager localizationManager) : ControllerBase
    {
        protected string? CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
        protected bool IsUserLoggedIn => !string.IsNullOrEmpty(CurrentUserId);

        protected IActionResult? CheckUserOrUnauthorized()
        {
            return IsUserLoggedIn ? null : Unauthorized(new GeneralResult { IsSuccess = false, Message = localizationManager.GetLocalizedString("UserNotLoggedIn") });
        }
    }
}
