namespace EntreLaunch.Services.EmailSvc
{
    public class EmailVerificationExtensionService : IEmailVerificationExtension
    {
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailVerificationExtensionService> _logger;
        private readonly UserManager<User> _userManager;
        public EmailVerificationExtensionService(
            IEmailService emailService,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            ILogger<EmailVerificationExtensionService> logger,
            UserManager<User> userManager)
        {
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _logger = logger;
            _userManager = userManager;
        }

        // Verification by code

        /// <inheritdoc />
        public GeneralResult<string> Generate(OtpVerificationOptions options, out DateTime expire, out string hash)
        {
            try
            {
                var utcNow = DateTime.UtcNow;
                var plain = GenerateRandomString(options.Size);
                expire = utcNow.AddMinutes(options.Expire);
                hash = ComputeHash(plain, utcNow, options);

                _logger.LogInformation("OTP generated successfully.");
                return new GeneralResult<string>(true, "OTP generated successfully.", plain);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating OTP.");
                expire = DateTime.MinValue; // assign a default value
                hash = string.Empty; // assign a default value
                return new GeneralResult<string>(false, "Error accurred while generating OTP.");
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<bool>> Scan(string plain, string hash, OtpVerificationOptions options)
        {
            try
            {
                var utcNow = DateTime.UtcNow;

                for (int minuteOffset = 0; minuteOffset <= options.Expire; minuteOffset++)
                {
                    var checkTime = utcNow.AddMinutes(-minuteOffset);
                    var hashToCheck = await Task.Run(() => ComputeHash(plain, checkTime, options));

                    if (hash.Equals(hashToCheck, StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogInformation("OTP validated successfully.");
                        return new GeneralResult<bool>(true, "OTP validated successfully.", true);
                    }
                }

                _logger.LogWarning("OTP validation failed.");
                return new GeneralResult<bool>(false, "OTP validation failed.", false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scanning OTP.");
                return new GeneralResult<bool>(false, "Error accurred while scanning OTP.");
            }
        }

        /// <inheritdoc />
        public GeneralResult<string> GetKey(string id)
        {
            try
            {
                var key = $"otp:{id}";
                _logger.LogInformation("Cache key generated: {Key}", key);
                return new GeneralResult<string>(true, "Cache key generated successfully.", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating cache key.");
                return new GeneralResult<string>(false, "Error accurred while generating cache key.");
            }
        }

        // Verification by link

        /// <inheritdoc />
        public async Task<GeneralResult> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var fromEmail = _configuration["EmailSender:FromEmail"];
                var fromName = _configuration["EmailSender:FromName"];

                if (string.IsNullOrEmpty(fromEmail) || string.IsNullOrEmpty(fromName))
                {
                    _logger.LogError("Email sender configuration is missing.");
                    return new GeneralResult(false, "Email sender configuration is missing.");
                }

                await _emailService.SendAsync(subject, fromEmail, fromName, new[] { toEmail }, body, null);
                _logger.LogInformation("Email sent successfully to {ToEmail}.", toEmail);
                return new GeneralResult(true, "Email sent successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {ToEmail}.", toEmail);
                return new GeneralResult(false, "Error accurred while sending email.");
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<string>> GenerateConfirmationLink(User user, string token)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    _logger.LogError("HTTP context is unavailable.");
                    return new GeneralResult<string>(false, "Error accurred while generating confirmation link, HTTP context is unavailable.");
                }

                var request = httpContext.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
                var confirmationLink = $"{baseUrl}/api/Email/ConfirmEmail?userId={Uri.EscapeDataString(user.Id)}&token={Uri.EscapeDataString(token)}";

                _logger.LogInformation("Generated confirmation link for user {UserId}: {ConfirmationLink}", user.Id, confirmationLink);
                return new GeneralResult<string>(true, "Confirmation link generated successfully.", await Task.FromResult(confirmationLink));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating confirmation link for user {UserId}.", user?.Id);
                return new GeneralResult<string>(false, "Error accurred while generating confirmation link.");
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<string>> GenerateResetPasswordLink(User user, string token)
        {
            try
            {
                var baseUrl = _configuration["AppSettings:BaseUrl"];
                if (string.IsNullOrEmpty(baseUrl))
                {
                    _logger.LogError("Base URL is not configured.");
                    return new GeneralResult<string>(false, "Base URL is not configured.");
                }

                var resetPasswordLink = $"{baseUrl}/reset-password?email={Uri.EscapeDataString(user.Email!)}&token={Uri.EscapeDataString(token)}";
                _logger.LogInformation("Generated reset password link for user {UserId}: {ResetPasswordLink}", user.Id, resetPasswordLink);
                return new GeneralResult<string>(true, "Reset password link generated successfully.", await Task.FromResult(resetPasswordLink));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating reset password link for user {UserId}.", user?.Id);
                return new GeneralResult<string>(false, "Error accurred while generating reset password link.");
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<string>> GenerateFallbackLinkAsync(User user)
        {
            try
            {
                if (user == null)
                {
                    _logger.LogError("User is null.");
                    return new GeneralResult<string>(false, "Error accurred while generating fallback link, user is null.");
                }

                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    _logger.LogError("HTTP context is unavailable.");
                    return new GeneralResult<string>(false, "Error accurred while generating fallback link, HTTP context is unavailable.");
                }

                string baseUrl = _configuration["AppSettings:BaseUrl"] ?? $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
                string fallbackToken = await _userManager.GenerateUserTokenAsync(user, "Default", "FallbackConfirmation");
                string fallbackLink = $"{baseUrl}/Account/ConfirmEmail?userId={Uri.EscapeDataString(user.Id)}&token={Uri.EscapeDataString(fallbackToken)}";

                _logger.LogInformation("Generated fallback link for user {UserId}: {FallbackLink}", user.Id, fallbackLink);

                var emailBody = $"""
                We encountered an issue generating the primary confirmation link. 
                Please confirm your account by <a href='{HtmlEncoder.Default.Encode(fallbackLink)}'>clicking here</a>.
                If this issue persists, contact sEntreLaunchport for assistance. Thank you.
                """;
                await SendEmailAsync(
                    user.Email!,
                    "Confirm Your Email - Fallback Link",
                    emailBody);

                return new GeneralResult<string>(true, "Fallback link generated successfully.", fallbackLink);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating fallback link for user {UserId}.", user?.Id);
                return new GeneralResult<string>(false, "Error accurred while generating fallback link.");
            }
        }

        /********************PRIVATE METHODS**************************/

        /// <summary>
        /// Generate random string of digits of given size to be used as OTP string.
        /// </summary>
        private string GenerateRandomString(int size)
        {
            try
            {
                const string digits = "0123456789";
                var otp = new string(Enumerable.Repeat(digits, size)
                    .Select(s => s[RandomNumberGenerator.GetInt32(s.Length)]).ToArray());
                _logger.LogInformation("Random OTP string generated.");
                return otp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating random OTP string.");
                throw;
            }
        }

        /// <summary>
        /// Compute hash of given plain text.
        /// </summary>
        private string ComputeHash(string plain, DateTime time, OtpVerificationOptions options)
        {
            try
            {
                var input = plain + time.ToString("yyyyMMddHHmm");
                var hash = Hash(input, options.Length, options.Iterations);
                _logger.LogInformation("Hash computed successfully.");
                return hash;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error computing hash.");
                throw;
            }
        }

        /// <summary>
        /// Generate hash of given plain text.
        /// </summary>
        private string Hash(string input, int length, int iterations)
        {
            try
            {
                using var sha256 = SHA256.Create();
                byte[] bytes = Encoding.UTF8.GetBytes(input);

                for (int i = 0; i < iterations; i++)
                {
                    bytes = sha256.ComputeHash(bytes);
                }

                var result = BitConverter.ToString(bytes).Replace("-", "").Substring(0, length).ToLowerInvariant();
                _logger.LogInformation("Hash generated successfully.");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating hash.");
                throw;
            }
        }
    }
}
