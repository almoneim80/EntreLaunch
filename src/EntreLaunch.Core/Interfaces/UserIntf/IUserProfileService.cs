using EntreLaunch.DTOs.TrainingDtos;
using EntreLaunch.DTOs.UserDtos;

namespace EntreLaunch.Interfaces.UserIntf
{
    public interface IUserProfileService
    {
        /// <summary>
        /// Gets a user's full profile.
        /// </summary>
        Task<GeneralResult<UserFullProfileDto>> GetFullProfileAsync(string userId);
    }
}
