using EntreLaunch.DTOs.TrainingDtos;
using EntreLaunch.Interfaces;

namespace EntreLaunch.Services.EmailSvc
{
    public class EmailVerificationService(
        UserManager<User> userManager,
        ILogger<EmailVerificationService> logger,
        IMemoryCache memoryCache,
        IOptions<OtpVerificationOptions> options,
        IDataProtectionProvider dataProtectionProvider,
        IEmailVerificationExtension emailVerificationExtension,
        ILocalizationManager localizationManager) : IEmailVerificationService
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly ILogger<EmailVerificationService> _logger = logger;
        private readonly IMemoryCache _memoryCache = memoryCache;
        private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("EmailVerificationService");
        private readonly IOptions<OtpVerificationOptions> _options = options;
        private readonly IEmailVerificationExtension _emailVerificationExtension = emailVerificationExtension;
        private readonly ILocalizationManager _localizationManager = localizationManager;

        // Verification by link

        /// <inheritdoc />
        public async Task<GeneralResult<(bool Succeeded, string Message, IEnumerable<IdentityError>? Errors)>> ConfirmEmailAsync(string userId, string token)
        {
            try
            {
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Invalid confirmation request. Missing userId or token.");
                    return new GeneralResult<(bool Succeeded, string Message, IEnumerable<IdentityError>? Errors)>(
                        false, "", (false, _localizationManager.GetLocalizedString("InvalidConfirmationRequest"), null));
                }

                _logger.LogInformation("Confirming email for UserId: {UserId}", userId);

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User not found with Id: {UserId}", userId);
                    return new GeneralResult<(bool Succeeded, string Message, IEnumerable<IdentityError>? Errors)>(
                        false, "", (false, _localizationManager.GetLocalizedString("UserNotFound"), null));
                }

                var storedToken = await _userManager.GetAuthenticationTokenAsync(user, "Default", "EmailVerificationToken");
                if (storedToken == null)
                {
                    _logger.LogWarning("No stored token found for UserId: {UserId}", userId);
                    return new GeneralResult<(bool Succeeded, string Message, IEnumerable<IdentityError>? Errors)>(
                        false, "", (false, _localizationManager.GetLocalizedString("InvalidToken"), null));
                }

                if (!string.Equals(storedToken, token, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("Invalid token for UserId: {UserId}", userId);
                    return new GeneralResult<(bool Succeeded, string Message, IEnumerable<IdentityError>? Errors)>(
                        false, "", (false, _localizationManager.GetLocalizedString("InvalidStoredToken"), null));
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
                    return new GeneralResult<(bool Succeeded, string Message, IEnumerable<IdentityError>? Errors)>(
                        true, "", (true, _localizationManager.GetLocalizedString("EmailConfirmedSuccessfully"), null));
                }
                else
                {
                    var errorDetails = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogError("Email confirmation failed for {Email}. Errors: {Errors}", user.Email, errorDetails);
                    return new GeneralResult<(bool Succeeded, string Message, IEnumerable<IdentityError>? Errors)>(
                        false, "", (false, _localizationManager.GetLocalizedString("EmailConfirmationFailed"), null));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming email for UserId: {UserId}", userId);
                return new GeneralResult<(bool Succeeded, string Message, IEnumerable<IdentityError>? Errors)>(
                    false, "", (false, _localizationManager.GetLocalizedString("ConfirmEmailUnexpectedError"), null));
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
                    return new GeneralResult<(bool Succeeded, string Message)>(false, "", (false, _localizationManager.GetLocalizedString("EmailRequired")));
                }

                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogWarning("User not found for email: {Email}", email);
                    return new GeneralResult<(bool Succeeded, string Message)>(
                        false, "", (false, _localizationManager.GetLocalizedString("UserNotFound")));
                }

                if (await _userManager.IsEmailConfirmedAsync(user))
                {
                    _logger.LogInformation("Email already confirmed for user: {Email}", email);
                    return new GeneralResult<(bool Succeeded, string Message)>(true, "", (false, _localizationManager.GetLocalizedString("EmailAlreadyConfirmed")));
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
                    return new GeneralResult<(bool Succeeded, string Message)>(false, "", (false, _localizationManager.GetLocalizedString("ConfirmationLinkFailed")));
                }

                _logger.LogInformation("Confirmation email resent for user {UserId} to {Email}", user.Id, user.Email);
                return new GeneralResult<(bool Succeeded, string Message)>(true, "", (true, _localizationManager.GetLocalizedString("ConfirmationResent")));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending verification link for email: {Email}", email);
                return new GeneralResult<(bool Succeeded, string Message)>(false, "", (false, _localizationManager.GetLocalizedString("ResendVerificationError")));
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
                    return new GeneralResult<(OtpVia Otp, DateTime Expire)>(false, _localizationManager.GetLocalizedString("OtpNullOrEmpty"),
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

                return await Task.FromResult(new GeneralResult<(OtpVia, DateTime)>(true,
                    _localizationManager.GetLocalizedString("OtpGeneratedSuccessfully"), (new OtpVia(plain.Data, url), expire)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating OTP for user ID: {Id}", id);
                return new GeneralResult<(OtpVia, DateTime)>(
                    false, _localizationManager.GetLocalizedString("OtpGenerationFailed"), (new OtpVia(string.Empty, string.Empty), DateTime.MinValue));
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
                        return new GeneralResult<bool>(false, _localizationManager.GetLocalizedString("UserNotFound"), false);
                    }

                    if (!user.EmailConfirmed)
                    {
                        user.EmailConfirmed = true;
                        var updateResult = await _userManager.UpdateAsync(user);

                        if (!updateResult.Succeeded)
                        {
                            var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                            _logger.LogError("Failed to update EmailConfirmed for user {Id}. Errors: {Errors}", id, errors);
                            return new GeneralResult<bool>(false, _localizationManager.GetLocalizedString("OtpUpdateFailed"), false);
                        }

                        _logger.LogInformation("EmailConfirmed updated successfully for user {Id}.", id);
                    }

                    return new GeneralResult<bool>(true, _localizationManager.GetLocalizedString("OtpVerifiedSuccessfully"), true);
                }

                return new GeneralResult<bool>(false, isValid.Message, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying OTP for user ID: {Id}", id);
                return new GeneralResult<bool>(false, _localizationManager.GetLocalizedString("OtpVerificationFailed"), false);
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
                    return new GeneralResult<(bool, string?, DateTime?)>(false, "User not found.", (false, _localizationManager.GetLocalizedString("UserNotFound"), null));
                }

                if (user.EmailConfirmed)
                {
                    _logger.LogInformation("User {UserId} attempted to regenerate code, but email is already confirmed.", userId);
                    return new GeneralResult<(bool, string?, DateTime?)>(false, "Email is already confirmed.", (
                        false, _localizationManager.GetLocalizedString("EmailAlreadyConfirmedShort"), null));
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
                    _localizationManager.GetLocalizedString("OtpSentSuccessfully"),
                    (true, "OTP generated and sent", expire));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while regenerating OTP for user ID: {UserId}", userId);
                return new GeneralResult<(bool, string?, DateTime?)>(
                    false,
                    _localizationManager.GetLocalizedString("OtpResendUnexpectedError"),
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
                    return new GeneralResult<bool>(false, _localizationManager.GetLocalizedString("OtpExpiredOrMissing"), false);
                }

                var isValid = await _emailVerificationExtension.Scan(code, hash, _options.Value);
                if (isValid.IsSuccess)
                {
                    _memoryCache.Remove(cacheKey);
                    _logger.LogInformation("OTP verified successfully for user ID: {Id}", id);
                    return new GeneralResult<bool>(true, _localizationManager.GetLocalizedString("OtpVerificationSuccess"), true);
                }

                _logger.LogWarning("OTP verification failed for user ID: {Id}", id);
                return new GeneralResult<bool>(false, _localizationManager.GetLocalizedString("OtpInvalidCode"), false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying OTP for user ID: {Id}", id);
                return new GeneralResult<bool>(false, _localizationManager.GetLocalizedString("OtpVerifyUnexpectedError"), false);
            }
        }
    }
}
