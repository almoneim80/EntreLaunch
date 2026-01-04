namespace EntreLaunch.Web.Controllers.AuthenticationAPI
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class IdentityController(
        ILogger<IdentityController> logger,
        IExternalAuthService externalAuthService,
        ILocalizationManager localization,
        UserManager<User> userManager,
        IIdentityService identityService,
        IEmailVerificationExtension emailVerificationExtension,
        IUserService userService) : AuthenticatedController(localization)
    {
        private readonly ILogger<IdentityController> _logger = logger;
        private readonly IExternalAuthService _externalAuthService = externalAuthService;
        private readonly ILocalizationManager _localization = localization;
        private readonly UserManager<User> _userManager = userManager;
        private readonly IIdentityService _identityService = identityService;
        private readonly IEmailVerificationExtension _emailVerificationExtension = emailVerificationExtension;
        private readonly IUserService _userService = userService;

        /// <summary>
        /// Registers a new user in the system after validating their uniqueness by email.
        /// </summary>
        /// <param name="value">Object containing the new user registration details including email and password.</param>
        /// <returns>Result indicating success or failure of the user registration process.</returns>
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

                if(value.Password != value.ConfirmPassword)
                {
                    return BadRequest(new GeneralResult
                    {
                        IsSuccess = false, Message = _localization!.GetLocalizedString("PasswordsDoNotMatch"), Data = null, ErrorType = ErrorType.InvalidData
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
                return this.UnexpectedError("register");
            }
        }

        /// <summary>
        /// Authenticates a user using provided credentials and returns a JWT token upon success.
        /// </summary>
        /// <param name="dto">Login credentials including email and password.</param>
        /// <returns>Result containing the JWT token and user information if authentication is successful.</returns>
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
                return this.UnexpectedError("Login");
            }
        }

        /// <summary>
        /// Issues a new access token using a valid refresh token.
        /// </summary>
        /// <param name="request">Request containing the existing refresh token.</param>
        /// <returns>Newly issued access token or an error if the refresh token is invalid.</returns>
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
                return this.UnexpectedError("Refresh Token");
            }
        }

        /// <summary>
        /// Logs out a user by invalidating their refresh token.
        /// </summary>
        /// <param name="logoutRequest">Contains the refresh token to be invalidated.</param>
        /// <returns>Operation result indicating logout status.</returns>
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
                return this.UnexpectedError("Logout");
            }
        }

        /// <summary>
        /// Resets the password for a user using a valid reset token.
        /// </summary>
        /// <param name="dto">Reset password details including email, new password, and token.</param>
        /// <returns>Success message if password is reset successfully; otherwise an error response.</returns>
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
                return this.UnexpectedError("resetting password.");
            }
        }

        /// <summary>
        /// Sends a password reset email to the specified user.
        /// </summary>
        /// <param name="email">Email address of the user requesting password reset.</param>
        /// <returns>Status of the email sending operation.</returns>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
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
                if (string.IsNullOrEmpty(resetPasswordLink.Data) && resetPasswordLink.IsSuccess == false)
                {
                    return BadRequest(resetPasswordLink);
                }

                // Send the link via email
                await _emailVerificationExtension.SendEmailAsync(user.Email!, "Reset Password",
                    $"Click <a href='{HtmlEncoder.Default.Encode(resetPasswordLink.Data!)}'>here</a> to reset your password.");

                _logger.LogInformation("Password reset email sent successfully for user: " + user.UserName);
                return Ok(new GeneralResult { IsSuccess = true, Message = resetPasswordLink.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while sending password reset email.");
                return this.UnexpectedError("sending password reset email.");
            }
        }

        /// <summary>
        /// Generates the Google login URL and redirects the user to the Google OAuth page.
        /// </summary>
        /// <returns>Redirect response to Google's login page.</returns>
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
                return this.UnexpectedError("Google login");
            }
        }

        /// <summary>
        /// Handles the callback from Google authentication and processes the login.
        /// </summary>
        /// <param name="code">Authorization code received from Google.</param>
        /// <returns>Login result including user details and token upon success, or error if the process fails.</returns>
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
                return this.UnexpectedError("Google callback");
            }
        }
    }
}
