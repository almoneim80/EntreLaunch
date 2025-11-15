namespace EntreLaunch.Interfaces.AuthenticationIntf;
public interface IOtpService
{
    /// <summary>
    /// Generate and send otp.
    /// </summary>
    Task<(string OtpCode, DateTime ExpireAt)> GenerateAndSendOtpAsync(string userId, string phoneNumber);

    /// <summary>
    /// Resend otp.
    /// </summary>
    Task<(string OtpCode, DateTime ExpireAt)> ResendOtpAsync(string userId, string phoneNumber);
}
