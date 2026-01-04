using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.ConsultationDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Interfaces.ConsultationsIntf
{
    public interface IConsultationBookingService
    {
        /// <summary>
        /// Book a new consultation session (typically online or scheduled).
        /// </summary>
        Task<GeneralResult> BookConsultation(OnlineConsultationCreateDto dto);

        /// <summary>
        /// Submit a text-based consultation request to a counselor.
        /// </summary>
        Task<GeneralResult> SubmitTextConsultation(TextConsultationCreateDto dto);

        /// <summary>
        /// Update the status of an existing consultation (e.g., Scheduled, InProgress, Completed).
        /// </summary>
        Task<GeneralResult> UpdateConsultationStatus(ProcessConsultationStatusDto dto);

        /// <summary>
        /// Retrieve all consultation records in the system.
        /// </summary>
        Task<GeneralResult<PaginatedResult<ConsultationAllData>>> GetAllConsultations(PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieve a specific consultation by its ID.
        /// </summary>
        Task<GeneralResult<ConsultationAllData>> GetConsultationById(int id);

        /// <summary>
        /// Retrieve all consultations filtered by consultation type (e.g., Online, Text).
        /// </summary>
        Task<GeneralResult<PaginatedResult<ConsultationAllData>>> GetConsultationsByType(ConsultationType type, PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieve all consultations assigned to a specific counselor.
        /// </summary>
        Task<GeneralResult<List<ConsultationAllData>>> GetConsultationsByCounselorId(int counselorId);

        /// <summary>
        /// Retrieve all consultations assigned to a specific client.
        /// </summary>
        Task<GeneralResult<List<ConsultationAllData>>> GetClientHistory(string clientId);
    }
}
