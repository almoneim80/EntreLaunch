using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.ConsultationDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Interfaces.ConsultationsIntf
{
    public interface ICounselorService
    {
        /// <summary>
        /// Check if the given user ID belongs to an active counselor.
        /// </summary>
        Task<GeneralResult<bool>> IsCounselor(int id);

        /// <summary>
        /// Submit a request to become a counselor.
        /// </summary>
        Task<GeneralResult> SubmitCounselorApplication(CreateCounselorRequestDto dto);

        /// <summary>
        /// Accept or reject a counselor application by updating its status.
        /// </summary>
        Task<GeneralResult> UpdateCounselorApplicationStatus(ProcessCounselorRequestDto dto);

        /// <summary>
        /// Retrieve all counselor application requests.
        /// </summary>
        Task<GeneralResult<PaginatedResult<CounselorRequestDetailsDto>>> GetAllCounselorApplications(PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieve all pending counselor application requests.
        /// </summary>
        Task<GeneralResult<PaginatedResult<CounselorRequestDetailsDto>>> GetCounselorRequestsBasedOnStatus(CounselorRequesttStatus status, PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieve all active counselors currently in the system.
        /// </summary>
        Task<GeneralResult<PaginatedResult<CounselorRequestDetailsDto>>> GetAllActiveCounselors(PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieve counselors by their specialization (e.g., psychology, finance).
        /// </summary>
        Task<GeneralResult<PaginatedResult<CounselorRequestDetailsDto>>> GetCounselorsBySpecialization(string specialization, PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieve the full profile (CV) of a specific counselor by ID.
        /// </summary>
        Task<GeneralResult<List<CounselorRequestDetailsDto>>> GetCounselorProfileById(int id);

        /// <summary>
        /// Retrieve all distinct counselor specializations available in the system.
        /// </summary>
        Task<GeneralResult<List<string>>> GetAllCounselorSpecializations();

        /// <summary>
        /// Create a new available consultation time slot for a counselor.
        /// </summary>
        Task<GeneralResult> CreateAvailableTimeSlot(ConsultationTimeCreateDto dto);

        /// <summary>
        /// Update an existing consultation time slot.
        /// </summary>
        Task<GeneralResult> UpdateAvailableTimeSlot(int id, ConsultationTimeUpdateDto dto);

        /// <summary>
        /// Retrieve all available consultation time slots for a specific counselor.
        /// </summary>
        Task<GeneralResult<PaginatedResult<ConsultationTimeDetailsDto>>> GetAvailableTimeSlotsByCounselor(int counselorId, PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieve a counselor by their user ID.
        /// </summary>
        Task<GeneralResult<CounselorRequestDetailsDto>> GetCounselorByUserId(string userId);

        /// <summary>
        /// Check if a user has a pending counselor application.
        /// </summary>
        Task<GeneralResult<bool>> HasPendingApplication(string userId);

        /// <summary>
        /// Retrieve counselor summary statistics.
        /// </summary>
        Task<GeneralResult<CounselorSummaryStatsDto>> GetCounselorSummaryStats();

        /// <summary>
        /// Retrieve all consultations for a specific counselor.
        /// </summary>
        Task<GeneralResult<PaginatedResult<ConsultationAllData>>> GetConsultationsByCounselorId(string counselorUserId, PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Generate daily recurring time slots for counselors.
        /// </summary>
        Task GenerateDailyRecurringTimeSlots();
    }
}
