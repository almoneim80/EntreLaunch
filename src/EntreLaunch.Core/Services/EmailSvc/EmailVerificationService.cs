namespace EntreLaunch.Services.EmailSvc
{
    public class EmailVerificationService : IEmailVerificationService
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<EmailVerificationService> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IDataProtector _dataProtector;
        private readonly IOptions<OtpVerificationOptions> _options;
        private readonly IEmailVerificationExtension _emailVerificationExtension;
        public EmailVerificationService(
            UserManager<User> userManager,
            ILogger<EmailVerificationService> logger,
            IMemoryCache memoryCache,
            IOptions<OtpVerificationOptions> options,
            IDataProtectionProvider dataProtectionProvider,
            IEmailVerificationExtension emailVerificationExtension)
        {
            _userManager = userManager;
            _logger = logger;
            _memoryCache = memoryCache;
            _options = options;
            _dataProtector = dataProtectionProvider.CreateProtector("EmailVerificationService");
            _emailVerificationExtension = emailVerificationExtension;
        }

        // Verification by link

        /// <inheritdoc />
        public async Task<GeneralResult<(bool Succeeded, string Message, IEnumerable<IdentityError>? Errors)>> ConfirmEmailAsync(string userId, string token)
        {
            try
            {
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Invalid confirmation request. Missing userId or token.");
                    return new GeneralResult<(bool Succeeded, string Message, IEnumerable<IdentityError>? Errors)>(false, "", (false, "Invalid confirmation request.", null));
                }

                _logger.LogInformation("Confirming email for UserId: {UserId}", userId);

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User not found with Id: {UserId}", userId);
                    return new GeneralResult<(bool Succeeded, string Message, IEnumerable<IdentityError>? Errors)>(false, "", (false, "User not found.", null));
                }

                var storedToken = await _userManager.GetAuthenticationTokenAsync(user, "Default", "EmailVerificationToken");
                if (storedToken == null)
                {
                    _logger.LogWarning("No stored token found for UserId: {UserId}", userId);
                    return new GeneralResult<(bool Succeeded, string Message, IEnumerable<IdentityError>? Errors)>(false, "", (false, "Invalid token, please request a new one.", null));
                }

                if (!string.Equals(storedToken, token, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("Invalid token for UserId: {UserId}", userId);
                    return new GeneralResult<(bool Succeeded, string Message, IEnumerable<IdentityError>? Errors)>(false, "", (false, "Invalid token.", null));
                }

                var result = await _userManager.ConfirmEmailAsync(user, storedToken);
                if (result.Succeeded)
                {
                    var removeTokenResult = await _userManager.RemoveAuthenticationTokenAsync(user, "Default", "EmailVerificationToken");
                    if (!removeTokenResult.Succeeded)
                    {
                        _logger.LogWarning("Failed to remove confirmation token for UserId: {UserId}", userId);
                    }

                    _logger.LogInformation("Email confirmed successfully for UserId: {UserId}", userId);
                    return new GeneralResult<(bool Succeeded, string Message, IEnumerable<IdentityError>? Errors)>(true, "", (true, "Email confirmed successfully!", null));
                }
                else
                {
                    var errorDetails = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogError("Email confirmation failed for {Email}. Errors: {Errors}", user.Email, errorDetails);
                    return new GeneralResult<(bool Succeeded, string Message, IEnumerable<IdentityError>? Errors)>(false, "", (false, "Email confirmation failed.", null));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming email for UserId: {UserId}", userId);
                return new GeneralResult<(bool Succeeded, string Message, IEnumerable<IdentityError>? Errors)>(false, "", (false, "An error occurred while confirming email.", null));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<(bool Succeeded, string Message)>> ResendVerificationLinkAsync(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogWarning("Email address is required for resending verification link.");
                    return new GeneralResult<(bool Succeeded, string Message)>(false, "", (false, "Email address is required."));
                }

                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogWarning("User not found for email: {Email}", email);
                    return new GeneralResult<(bool Succeeded, string Message)>(false, "", (false, "User not found."));
                }

                if (await _userManager.IsEmailConfirmedAsync(user))
                {
                    _logger.LogInformation("Email already confirmed for user: {Email}", email);
                    return new GeneralResult<(bool Succeeded, string Message)>(true, "", (false, "Email is already confirmed."));
                }

                var storedToken = await _userManager.GetAuthenticationTokenAsync(user, "Default", "EmailVerificationToken");
                var token = storedToken ?? await _userManager.GenerateEmailConfirmationTokenAsync(user);
                if (storedToken == null)
                {
                    await _userManager.SetAuthenticationTokenAsync(user, "Default", "EmailVerificationToken", token);
                    _logger.LogInformation("New confirmation token generated and stored for user: {Email}", email);
                }
                else
                {
                    _logger.LogInformation("Using existing confirmation token for user: {Email}", email);
                }

                var confirmationLink = await _emailVerificationExtension.GenerateConfirmationLink(user, token);

                if (!string.IsNullOrEmpty(confirmationLink.Data))
                {
                    await _emailVerificationExtension.SendEmailAsync(
                        user.Email!,
                        "Confirmation Email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(confirmationLink.Data)}'>clicking here</a>.");
                }
                else
                {
                    _logger.LogError("Failed to generate confirmation link for user: {UserId}", user.Id);
                    await _emailVerificationExtension.GenerateFallbackLinkAsync(user);
                    return new GeneralResult<(bool Succeeded, string Message)>(false, "", (false, "Failed to send confirmation email. Please try again later or contact sEntreLaunchport."));
                }

                _logger.LogInformation("Confirmation email resent for user {UserId} to {Email}", user.Id, user.Email);
                return new GeneralResult<(bool Succeeded, string Message)>(true, "", (true, "Confirmation email has been resent."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending verification link for email: {Email}", email);
                return new GeneralResult<(bool Succeeded, string Message)>(false, "", (false, "An error occurred while resending the verification email."));
            }
        }

        // Verification by code

        /// <inheritdoc />
        public async Task<GeneralResult<(OtpVia Otp, DateTime Expire)>> GenerateAsync(string id, OtpVerificationOptions options)
        {
            try
            {
                var plain = _emailVerificationExtension.Generate(_options.Value, out DateTime expire, out string hash);
                if (string.IsNullOrEmpty(plain.Data))
                {
                    return new GeneralResult<(OtpVia Otp, DateTime Expire)>(false, "Failed to generate OTP: plain data is null or empty.",
                        (new OtpVia(string.Empty, string.Empty), DateTime.MinValue));
                }

                _memoryCache.Set(_emailVerificationExtension.GetKey(id), hash, new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = expire,
                    Priority = CacheItemPriority.High,
                });

                string url = _options.Value.EnableUrl && !string.IsNullOrWhiteSpace(_options.Value.BaseOtpUrl)
                    ? _options.Value.BaseOtpUrl + _dataProtector.Protect(JsonSerializer.Serialize(new IdPlain(id, plain.Data)))
                    : string.Empty;

                return await Task.FromResult(new GeneralResult<(OtpVia, DateTime)>(true, "OTP generated successfully", (new OtpVia(plain.Data, url), expire)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating OTP for user ID: {Id}", id);
                return new GeneralResult<(OtpVia, DateTime)>(false, "Failed to generate OTP", (new OtpVia(string.Empty, string.Empty), DateTime.MinValue));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<bool>> VerifyOtpAsync(string id, string code)
        {
            try
            {
                var isValid = await VerifyAsync(id, code, _options.Value);
                if (isValid.IsSuccess)
                {
                    var user = await _userManager.FindByIdAsync(id);
                    if (user == null)
                    {
                        _logger.LogError("User not found with ID: {Id}", id);
                        return new GeneralResult<bool>(false, "User not found.", false);
                    }

                    if (!user.EmailConfirmed)
                    {
                        user.EmailConfirmed = true;
                        var EntreLaunchdateResult = await _userManager.EntreLaunchdateAsync(user);

                        if (!EntreLaunchdateResult.Succeeded)
                        {
                            var errors = string.Join(", ", EntreLaunchdateResult.Errors.Select(e => e.Description));
                            _logger.LogError("Failed to EntreLaunchdate EmailConfirmed for user {Id}. Errors: {Errors}", id, errors);
                            return new GeneralResult<bool>(false, errors, false);
                        }

                        _logger.LogInformation("EmailConfirmed EntreLaunchdated successfully for user {Id}.", id);
                    }

                    return new GeneralResult<bool>(true, "Email confirmed successfully.", true);
                }

                return new GeneralResult<bool>(false, isValid.Message, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying OTP for user ID: {Id}", id);
                return new GeneralResult<bool>(false, "Failed to verify OTP", false);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<(bool Succeeded, string? Message, DateTime? ExpireAt)>> RegenerateOtpAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User not found with ID: {UserId}", userId);
                    return new GeneralResult<(bool, string?, DateTime?)>(false, "User not found.", (false, "User not found.", null));
                }

                if (user.EmailConfirmed)
                {
                    _logger.LogInformation("User {UserId} attempted to regenerate code, but email is already confirmed.", userId);
                    return new GeneralResult<(bool, string?, DateTime?)>(false, "Email is already confirmed.", (false, "Email is already confirmed.", null));
                }

                _memoryCache.Remove(_emailVerificationExtension.GetKey(userId));

                // Generate a new OTP
                var generateResult = await GenerateAsync(userId, _options.Value);
                if (!generateResult.IsSuccess)
                {
                    _logger.LogError("Failed to generate OTP for user {UserId}. Reason: {Reason}", userId, generateResult.Message);
                    return new GeneralResult<(bool, string?, DateTime?)>(false, generateResult.Message, (false, generateResult.Message, null));
                }

                var (otp, expire) = generateResult.Data;

                await _emailVerificationExtension.SendEmailAsync(
                    user.Email!,
                    "Verification Code",
                    $"Your verification code is: <b>{otp.Plain}</b>. It will expire after {_options.Value.Expire} minutes.");

                _logger.LogInformation("New verification code sent to user {UserId}. Expires in {Expire} minutes.", userId, _options.Value.Expire);

                return new GeneralResult<(bool, string?, DateTime?)>(
                    true,
                    "A new verification code was generated and sent successfully.",
                    (true, "OTP generated and sent", expire));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while regenerating OTP for user ID: {UserId}", userId);
                return new GeneralResult<(bool, string?, DateTime?)>(
                    false,
                    "An error occurred while regenerating the verification code.",
                    (false, "Unexpected error", null));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<bool>> VerifyAsync(string id, string code, OtpVerificationOptions options)
        {
            try
            {
                var cacheKey = _emailVerificationExtension.GetKey(id);
                var hash = _memoryCache.Get<string>(cacheKey);

                if (string.IsNullOrEmpty(hash))
                {
                    _logger.LogWarning("No OTP hash found for user ID: {Id}", id);
                    return new GeneralResult<bool>(false, "Verification code has expired or does not exist.", false);
                }

                var isValid = await _emailVerificationExtension.Scan(code, hash, _options.Value);
                if (isValid.IsSuccess)
                {
                    _memoryCache.Remove(cacheKey);
                    _logger.LogInformation("OTP verified successfully for user ID: {Id}", id);
                    return new GeneralResult<bool>(true, "Verification successful.", true);
                }

                _logger.LogWarning("OTP verification failed for user ID: {Id}", id);
                return new GeneralResult<bool>(false, "Invalid verification code.", false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying OTP for user ID: {Id}", id);
                return new GeneralResult<bool>(false, "An error occurred during OTP verification.", false);
            }
        }
    }
}
