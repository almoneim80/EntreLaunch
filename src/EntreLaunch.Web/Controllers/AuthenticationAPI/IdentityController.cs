namespace EntreLaunch.Web.Controllers.AuthenticationAPI
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class IdentityController : AuthenticatedController
    {
        private readonly ILogger<IdentityController> _logger;
        private readonly IExternalAuthService _externalAuthService;
        private readonly ILocalizationManager? _localization;
        private readonly UserManager<User> _userManager;
        private readonly IIdentityService _identityService;
        private readonly IEmailVerificationExtension _emailVerificationExtension;
        private readonly IUserService _userService;
        public IdentityController(
            ILogger<IdentityController> logger,
            IExternalAuthService externalAuthService,
            ILocalizationManager? localization,
            UserManager<User> userManager,
            IIdentityService identityService,
            IEmailVerificationExtension emailVerificationExtension,
            IUserService userService)
        {
            _logger = logger;
            _externalAuthService = externalAuthService;
            _localization = localization;
            _userManager = userManager;
            _identityService = identityService;
            _emailVerificationExtension = emailVerificationExtension;
            _userService = userService;
        }

        /// <summary>
        /// Create a new user in the system.
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public virtual async Task<ActionResult> Post([FromBody] UserCreateDto value)
        {
            try
            {
                if (value.Email == null)
                {
                    return BadRequest(new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localization!.GetLocalizedString("EmailNull"),
                        Data = null
                    });
                }

                var user = await _userManager.FindByEmailAsync(value.Email);
                if (user != null)
                {
                    return BadRequest(new GeneralResult
                    {
                        IsSuccess = false, Message = _localization!.GetLocalizedString("UserAlreadytaken"), Data = null
                    });
                }

                var result = await _userService.CreateUserAsync(value);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (IdentityException identityEx)
            {
                _logger.LogError(identityEx, "User creation failed for user name: {User Name}", value.FirstName);
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = _localization!.GetLocalizedString("UserCreationFailed") });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while creating user.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = _localization!.GetLocalizedString("UserCreationFailed") });
            }
        }

        /// <summary>
        /// Login with email and password.
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                var loginResult = await _identityService.LoginAsync(dto);
                if (loginResult.IsSuccess == false)
                {
                    return BadRequest(loginResult);
                }

                return Ok(loginResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging in user.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = _localization!.GetLocalizedString("LoginError") });
            }
        }

        /// <summary>
        /// Generates a new refresh token value.
        /// </summary>
        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                var refreshTokenResult = await _identityService.RefreshTokenAsync(request.RefreshToken);
                if (refreshTokenResult.IsSuccess == false)
                {
                    return BadRequest(refreshTokenResult);
                }

                return Ok(refreshTokenResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in refreshing token.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = _localization!.GetLocalizedString("RefreshTokenError") });
            }
        }

        /// <summary>
        /// Logouts.
        /// </summary>
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest logoutRequest)
        {
            try
            {
                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var logoutResult = await _identityService.LogoutAsync(CurrentUserId!, logoutRequest.RefreshToken);
                if (logoutResult.IsSuccess == false)
                {
                    return BadRequest(logoutResult);
                }

                return Ok(logoutResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in refreshing token.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = _localization!.GetLocalizedString("LogoutError") });
            }
        }

        /// <summary>
        /// Endpoint to reset a user's password.
        /// </summary>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Token) || string.IsNullOrWhiteSpace(dto.NewPassword) || string.IsNullOrWhiteSpace(dto.Email))
                {
                    return BadRequest(new GeneralResult { IsSuccess = false, Message = "Invalid request data." });
                }

                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null)
                {
                    return BadRequest(new GeneralResult { IsSuccess = false, Message = "User not found." });
                }

                // Reset password using the provided token and new password
                var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
                if (!result.Succeeded)
                {
                    _logger.LogError((Exception?)result.Errors, "Password reset failed for user: " + user.UserName);
                    return BadRequest(result);
                }

                _logger.LogInformation("Password reset successful for user: " + user.UserName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while resetting password.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = _localization!.GetLocalizedString("UnexpectedError") });
            }
        }

        /// <summary>
        /// Endpoint to request a password reset email.
        /// </summary>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return BadRequest(new GeneralResult { IsSuccess = false, Message = "Invalid request data." });
                }

                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return BadRequest(new GeneralResult { IsSuccess = false, Message = "User not found." });
                }

                // Generate the reset token
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                if (string.IsNullOrEmpty(resetToken))
                {
                    return BadRequest(new GeneralResult { IsSuccess = false, Message = "Failed to generate reset token." });
                }

                // Generate the reset password link
                var resetPasswordLink = await _emailVerificationExtension.GenerateResetPasswordLink(user, resetToken);
                if (string.IsNullOrEmpty(resetPasswordLink.Data))
                {
                    return BadRequest(resetPasswordLink);
                }

                // Send the link via email
                await _emailVerificationExtension.SendEmailAsync(user.Email!, "Reset Password",
                    $"Click <a href='{HtmlEncoder.Default.Encode(resetPasswordLink.Data)}'>here</a> to reset your password.");

                _logger.LogInformation("Password reset email sent successfully for user: " + user.UserName);
                return Ok(resetPasswordLink);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while sending password reset email.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = _localization!.GetLocalizedString("UnexpectedError") });
            }
        }

        /// <summary>
        /// Initiates the Google login process by generating a login URL.
        /// </summary>
        [HttpGet("google/login")]
        public IActionResult GoogleLogin()
        {
            try
            {
                string loginUrl = _externalAuthService.GenerateGoogleLoginUrl(new List<string> { "email", "profile" });
                _logger.LogInformation("Google login URL generated successfully.");
                return Redirect(loginUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating Google login.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = _localization!.GetLocalizedString("GoogleLoginUrlError") });
            }
        }

        /// <summary>
        /// Callback endpoint for Google after the user has authenticated.
        /// </summary>
        [HttpGet("callback")]
        public async Task<IActionResult> GoogleCallback([FromQuery] string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                _logger.LogWarning("Google callback called with empty code.");
                return BadRequest(new GeneralResult { IsSuccess = false, Message = "Google callback failed" });
            }

            try
            {
                var authResult = await _externalAuthService.HandleGoogleAuthCallbackAsync(code);
                if (authResult.Success)
                {
                    _logger.LogInformation("Google callback handled successfully for user {Email}.", authResult.Email);
                    return Ok(new
                    {
                        Message = authResult.ErrorMessage,
                        authResult.Token,
                        authResult.Email
                    });
                }
                else
                {
                    _logger.LogWarning("Google callback failed with error: {ErrorMessage}", authResult.ErrorMessage);
                    return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Google callback failed" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling Google callback.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = _localization!.GetLocalizedString("GoogleCallbackError") });
            }
        }
    }
}
