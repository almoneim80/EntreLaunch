namespace EntreLaunch.Interfaces.EmailIntf
{
    public interface IEmailVerificationService
    {
        // Verification by link

        /// <summary>
        /// Confirms email address.
        /// </summary>
        Task<GeneralResult<(bool Succeeded, string Message, IEnumerable<IdentityError>? Errors)>> ConfirmEmailAsync(string userId, string token);

        /// <summary>
        /// Resends verification link.
        /// </summary>
        Task<GeneralResult<(bool Succeeded, string Message)>> ResendVerificationLinkAsync(string email);

        // Verification by code

        /// <summary>
        /// Generates and returns verification code.
        /// </summary>
        Task<GeneralResult<(OtpVia Otp, DateTime Expire)>> GenerateAsync(string id, OtpVerificationOptions options);

        /// <summary>
        /// Verifies code.
        /// </summary>
        Task<GeneralResult<bool>> VerifyOtpAsync(string id, string code);

        /// <summary>
        /// Regenerates code.
        /// </summary>
        Task<GeneralResult<(bool Succeeded, string? Message, DateTime? ExpireAt)>> RegenerateOtpAsync(string userId);

        /// <summary>
        /// Verifies code.
        /// </summary>
        Task<GeneralResult<bool>> VerifyAsync(string id, string code, OtpVerificationOptions options);
    }
}
