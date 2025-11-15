using Microsoft.Extensions.Logging;
using EntreLaunch.Interfaces.AuthenticationIntf;
using EntreLaunch.Interfaces.CacheIntf;

namespace EntreLaunch.Plugin.Sms.Services;
public class SmsOtpService : IOtpService
{
    private readonly ISmsService _smsService;
    private readonly IOptions<OtpOptions> _options;
    private readonly ILogger<SmsOtpService> _logger;
    private readonly ICacheService _cacheService;

    public SmsOtpService(
        ISmsService smsService,
        IOptions<OtpOptions> options,
        ILogger<SmsOtpService> logger,
        ICacheService cacheService)
    {
        _smsService = smsService;
        _options = options;
        _logger = logger;
        _cacheService = cacheService;
    }

    /// <summary>
    /// Generates and sends an OTP to the specified phone number.
    /// </summary>
    public async Task<(string OtpCode, DateTime ExpireAt)> GenerateAndSendOtpAsync(string userId, string phoneNumber)
    {
        try
        {
            // Generate OTP
            var otpCode = new Random().Next(100000, 999999).ToString(); // 6-digit OTP
            var expireAt = DateTime.UtcNow.AddMinutes(_options.Value.ExpireInMinutes);

            // Save to cache
            var timeToLive = expireAt - DateTime.UtcNow;
            await _cacheService.SetAsync($"otp_{userId}", otpCode, timeToLive);

            // Prepare SMS message
            var template = "Your EntreLaunch-tc verification code is {otp}. It will expire in {minutes} minutes.";
            var placeholders = new Dictionary<string, string>
            {
                { "otp", otpCode },
                { "minutes", _options.Value.ExpireInMinutes.ToString() }
            };

            var message = _smsService.ReplacePlaceholders(template, placeholders);

            // Send SMS
            await _smsService.SendAsync(phoneNumber, message);

            _logger.LogInformation("OTP sent successfully to {PhoneNumber}, expires at {ExpireAt}", phoneNumber, expireAt);
            return (otpCode, expireAt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate or send OTP for {PhoneNumber}.", phoneNumber);
            throw new InvalidOperationException("Failed to generate or send OTP. Please try again.");
        }
    }

    /// <summary>
    /// Resends an OTP to the specified phone number.
    /// </summary>
    public async Task<(string OtpCode, DateTime ExpireAt)> ResendOtpAsync(string userId, string phoneNumber)
    {
        try
        {
            // Generate new OTP
            var otpCode = new Random().Next(100000, 999999).ToString(); // 6-digit OTP
            var expireAt = DateTime.UtcNow.AddMinutes(_options.Value.ExpireInMinutes);

            // Save to cache
            var timeToLive = expireAt - DateTime.UtcNow;
            await _cacheService.SetAsync($"otp_{userId}", otpCode, timeToLive);

            // Prepare SMS message
            var template = "Your new EntreLaunch-tc verification code is {otp}. It will expire in {minutes} minutes.";
            var placeholders = new Dictionary<string, string>
        {
            { "otp", otpCode },
            { "minutes", _options.Value.ExpireInMinutes.ToString() }
        };

            var message = _smsService.ReplacePlaceholders(template, placeholders);

            // Send SMS
            await _smsService.SendAsync(phoneNumber, message);

            _logger.LogInformation("Resent OTP {Otp} to {PhoneNumber}, expires at {ExpireAt}", otpCode, phoneNumber, expireAt);
            return (otpCode, expireAt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to resend OTP for {PhoneNumber}.", phoneNumber);
            throw new InvalidOperationException("Failed to resend OTP. Please try again.");
        }
    }
}
