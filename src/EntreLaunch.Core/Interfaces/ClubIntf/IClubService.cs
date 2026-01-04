using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.ClubDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Interfaces.ClubIntf
{
    public interface IClubService
    {
        /// <summary>
        /// registers a user to an event.
        /// </summary>
        Task<GeneralResult> RegisterToEventAsync(ClubEventRegistrationCreateDto dto);

        /// <summary>
        /// unregisters a user from an event.
        /// </summary>
        Task<GeneralResult> CancelEventRegistrationAsync(int registrationId, string userId);

        /// <summary>
        /// Retrieves all registrations for a specific event.
        /// </summary>
        Task<GeneralResult<PaginatedResult<ClubEventRegistrationDetailsDto>>> GetEventRegistrationsAsync(int eventId, PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves all registrations for a specific user.
        /// </summary>
        Task<GeneralResult> GetUserEventRegistrationsAsync(string userId);

        /// <summary>
        /// get all club events.
        /// </summary>
        Task<GeneralResult<PaginatedResult<ClubEventDetails>>> AllEventsAsync(PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// get a specific event by id.
        /// </summary>
        Task<GeneralResult<ClubEventDetails>> OneEventAsync(int eventId, CancellationToken cancellationToken);

        /// <summary>
        /// Updates a specific event by id.
        /// </summary>
        Task<GeneralResult> UpdateClubEventAsync(int eventId, ClubEventUpdateDto dto);

        /// <summary>
        /// Adds a new event.
        /// </summary>
        Task<GeneralResult> AddClubEventAsync(ClubEventCreateDto dto);

        /// <summary>
        /// Checks if a user can register to an event or not.
        /// </summary>
        Task<GeneralResult<bool>> CanRegisterToEventAsync(int eventId, string userId);
    }
}
