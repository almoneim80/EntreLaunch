using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Interfaces.TrainingIntf
{
    public interface IRatingsService
    {
        /// <summary>
        /// Approve rating.
        /// </summary>
        Task<GeneralResult<bool>> ApproveRatingAsync(int ratingId, string adminNote);

        /// <summary>
        /// Reject rating.
        /// </summary>
        Task<GeneralResult<bool>> RejectRatingAsync(int ratingId, string adminNote);

        /// <summary>
        /// Get ratings by status.
        /// </summary>
        Task<GeneralResult<List<CourseRatingDetailsDto>>> GetRatingsByStatusAsync(RatingStatus status);

        /// <summary>
        /// Get all approved ratings.
        /// </summary>
        Task<GeneralResult<PaginatedResult<CourseRatingDetailsDto>>> GetApprovedRatingsAsync(PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Checks if a rating is available.
        /// </summary>
        Task<GeneralResult<bool>> IsRatingAvailableAsync(int ratingId);

        /// <summary>
        /// Get course rating statistics.
        /// </summary>
        Task<GeneralResult<(double AverageRating, int RatingCount)>> GetCourseRatingStatisticsAsync(int courseId);

        /// <summary>
        /// Get all ratings for course.
        /// </summary>
        Task<GeneralResult<PaginatedResult<CourseRatingDetailsDto>>> GetAllRatingsForCourseAsync(int courseId, PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Checks if a student has rated a course.
        /// </summary>
        Task<GeneralResult<bool>> CanStudentRateCourseAsync(string studentId, int courseId);

        /// <summary>
        /// Get course rating summary.
        /// </summary>
        Task<GeneralResult<CourseRatingSummaryDto>> GetCourseRatingSummaryAsync(int courseId);

        /// <summary>
        /// Get ratings by instructor.
        /// </summary>
        Task<GeneralResult<List<CourseRatingsDto>>> GetRatingsByInstructorAsync(string instructorId);
    }
}
