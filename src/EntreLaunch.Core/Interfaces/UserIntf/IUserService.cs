using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.TrainingDtos;
using EntreLaunch.DTOs.UserDtos;

namespace EntreLaunch.Interfaces.UserIntf
{
    public interface IUserService
    {
        /// <summary>
        /// Creates a new user.
        /// </summary>
        Task<GeneralResult> CreateUserAsync(UserCreateDto value);

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        Task<GeneralResult> CompleteUserAsync(User existingEntity, CompleteUserDetailsDto value);

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        Task<GeneralResult> UpdateUserAsync(User existingEntity, UserUpdateDto value);

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

        /// <summary>
        /// Retrieves all users.
        /// </summary>
        Task<GeneralResult<PaginatedResult<UserDetailsDto>>> GetAllUsersAsync(PaginationParams pagination, CancellationToken cancellationToken);
    }
}
