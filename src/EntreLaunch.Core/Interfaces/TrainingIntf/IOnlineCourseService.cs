using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Interfaces.TrainingIntf
{
    public interface IOnlineCourseService
    {
        /// <summary>
        /// Get course based on status.
        /// </summary>
        Task<GeneralResult<PaginatedResult<OnlineCourseDetailsDto>>> GetByStatusAsync(CourseStatus status, PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Change course status.
        /// </summary>
        Task<GeneralResult> ChangeCourseStatusAsync(int courseId, CourseStatus newStatus);
    }
}
