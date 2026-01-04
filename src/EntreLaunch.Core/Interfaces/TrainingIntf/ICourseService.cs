using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Interfaces.TrainingIntf
{
    public interface ICourseService
    {
        /// <summary>
        /// Create a new course and return the created course details.
        /// </summary>
        Task<GeneralResult> CreateAsync<TCreateDto>(TCreateDto dto);

        /// <summary>
        /// Update a course and return the updated course details.
        /// </summary>
        Task<GeneralResult> UpdateAsync<TUpdateDto>(int id, TUpdateDto dto);

        /// <summary>
        /// Get all courses.
        /// </summary>
        Task<GeneralResult<PaginatedResult<object>>> GetAllAsync(CourseType type, PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Get one course.
        /// </summary>
        Task<GeneralResult> GetOneAsync(int id, CourseType type);

        /// <summary>
        /// Get enrolled users by course id and purchase type.
        /// </summary>
        Task<GeneralResult<PaginatedResult<CoursesRegisterDto>>> GetUsersByCoursePurchaseAsync(PurchaseItemType itemType, int courseId, PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Map course to skill course details.
        /// </summary>
        SkillCourseDetailsDto MapToSkillDto(Course course);

        /// <summary>
        /// Map course to skill course details.
        /// </summary>
        PathCourseDetailsDto MapToPathDto(Course course);
    }
}
