namespace EntreLaunch.Interfaces.UserIntf;
public interface IUserService
{
    /// <summary>
    /// Creates a new user.
    /// </summary>
    Task<GeneralResult> CreateUserAsync(UserCreateDto value);

    /// <summary>
    /// EntreLaunchdates an existing user.
    /// </summary>
    Task<GeneralResult> CompleteUserAsync(User existingEntity, CompleteUserDetailsDto value);

    /// <summary>
    /// EntreLaunchdates an existing user.
    /// </summary>
    Task<GeneralResult> EntreLaunchdateUserAsync(User existingEntity, UserEntreLaunchdateDto value);

    /// <summary>
    /// Deletes an existing user.
    /// </summary>
    Task<GeneralResult> DeleteUserAsync(User existingEntity);

    /// <summary>
    /// Resets a user's password.
    /// </summary>
    Task<GeneralResult> ResetPasswordAsync(string email, string newPassword, string resetToken);

    /// <summary>
    /// Verifies a user's OTP.
    /// </summary>
    Task<GeneralResult> VerifyOtpAsync(string userId, string inputOtp);

    /// <summary>
    /// Resends an OTP to a user's phone number.
    /// </summary>
    Task ResendOtpAsync(string userId, string phoneNumber);

    /// <summary>
    /// Activates or deactivates a user.
    /// </summary>
    Task<GeneralResult> ToggleUserActiveStatusAsync(string userId, bool isActive, string reason);
}
