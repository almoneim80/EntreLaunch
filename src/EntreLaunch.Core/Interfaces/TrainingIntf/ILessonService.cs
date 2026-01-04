using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.LessonDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Interfaces.TrainingIntf
{
    public interface ILessonService
    {
        /// <summary>
        /// Retrieves all lessons.
        /// </summary>
        Task<GeneralResult<PaginatedResult<LessonFullDetailsDto>>> GetAllLessonsAsync(PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a specific lesson.
        /// </summary>
        Task<GeneralResult<LessonFullDetailsDto>> GetLessonByIdAsync(int lessonId);

        /// <summary>
        /// Retrieves all lessons for a specific course.
        /// </summary>
        Task<GeneralResult<List<LessonFullDetailsDto>>> GetLessonsByCourseIdAsync(int courseId);

        /// <summary>
        /// Updates a specific lesson.
        /// </summary>
        Task<GeneralResult> UpdateLessonAsync(int lessonId, LessonUpdateDto dto);

        /// <summary>
        /// Creates a new lesson.
        /// </summary>
        Task<GeneralResult> CreateLessonAsync(LessonCreateDto dto);

        /// <summary>
        /// Reorder lessons.
        /// </summary>
        Task<GeneralResult<bool>> ReorderLessonsAsync(int courseId, List<LessonReorderDto> newOrderList);
    }
}
