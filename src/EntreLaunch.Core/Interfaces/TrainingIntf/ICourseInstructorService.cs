using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Interfaces.TrainingIntf
{
    public interface ICourseInstructorService
    {
        /// <summary>
        /// Create a new course instructor.
        /// </summary>
        Task<GeneralResult<CourseInstructorDetailsDto>> CreateAsync(CourseInstructorCreateDto dto);

        /// <summary>
        /// Update an existing course instructor.
        /// </summary>
        Task<GeneralResult<CourseInstructorDetailsDto>> UpdateAsync(int id, CourseInstructorUpdateDto dto);

        /// <summary>
        /// Get a course instructor.
        /// </summary>
        Task<GeneralResult<CourseInstructorDetailsDto>> GetOneAsync(int id);

        /// <summary>
        /// Get all course instructors.
        /// </summary>
        Task<GeneralResult<PaginatedResult<CourseInstructorDetailsDto>>> GetAllAsync(PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Get all course instructors by course id.
        /// </summary>
        Task<GeneralResult<PaginatedResult<CourseInstructorDetailsDto>>> GetInstructorsByCourseIdAsync(int courseId, PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Delete a course instructor.
        /// </summary>
        Task<GeneralResult> DeleteAsync(int id);
    }
}
