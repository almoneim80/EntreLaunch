namespace EntreLaunch.Interfaces.EmailIntf
{
    public interface IEmailVerificationExtension
    {
        /// <summary>
        /// Send Email.
        /// </summary>
        Task<GeneralResult> SendEmailAsync(string toEmail, string subject, string body);

        /// <summary>
        /// Generate Confirmation Link.
        /// </summary>
        Task<GeneralResult<string>> GenerateConfirmationLink(User user, string token);

        /// <summary>
        /// Generate Fallback Link.
        /// </summary>
        Task<GeneralResult<string>> GenerateFallbackLinkAsync(User user);

        /// <summary>
        /// Get Key for Email.
        /// </summary>
        GeneralResult<string> GetKey(string id);

        /// <summary>
        /// Generate OTP Verification.
        /// </summary>
        GeneralResult<string> Generate(OtpVerificationOptions options, out DateTime expire, out string hash);

        /// <summary>
        /// Scan OTP Verification.
        /// </summary>
        Task<GeneralResult<bool>> Scan(string plain, string hash, OtpVerificationOptions options);

        /// <summary>
        /// Generate Reset Password Link for User.
        /// </summary>
        Task<GeneralResult<string>> GenerateResetPasswordLink(User user, string token);
    }
}
