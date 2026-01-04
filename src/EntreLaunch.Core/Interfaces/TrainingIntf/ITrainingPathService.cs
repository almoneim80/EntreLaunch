using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Interfaces.TrainingIntf
{
    public interface ITrainingPathService
    {
        /// <summary>
        /// Retrieves all training paths.
        /// </summary>
        Task<GeneralResult<PaginatedResult<TrainingPathFullDetailsDto>>> GetAllTrainingPathsWithCoursesAsync(PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a specific training path.
        /// </summary>
        Task<GeneralResult<TrainingPathFullDetailsDto>> GetTrainingPathWithCoursesByIdAsync(int pathId);

        /// <summary>
        /// Get path enrollments with user data.
        /// </summary>
        Task<GeneralResult<(int totalEnrollments, List<TrainingPathSubscripersDto> enrollments)>> GetPathEnrollmentsAsync(int pathId);
    }
}
