using EntreLaunch.DTOs.ProgressDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Interfaces.TrainingIntf
{
    public interface IStudentProgress
    {
        /// <summary>
        /// Marks a lesson as completed for the given user, updating course and program progress where applicable.
        /// Skips progress tracking for OnlineCourse types.
        /// </summary>
        Task<GeneralResult> MarkLessonCompletedAsync(int lessonId, string userId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the student's progress for a specific lesson.
        /// </summary>
        Task<GeneralResult<LessonProgressDetailsDto>> GetLessonProgressAsync(int lessonId, string userId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the user's progress for all lessons within a specific course.
        /// </summary>
        Task<GeneralResult<List<LessonProgressDetailsDto>>> GetCourseLessonsProgressAsync(int courseId, string userId, CancellationToken cancellationToken);

        /// <summary>
        /// Updates the student's progress for the given course, including completion percentage and total time spent.
        /// </summary>
        Task<GeneralResult> UpdateCourseProgressAsync(int courseId, string userId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the progress details of a user for a specific course.
        /// This includes completion percentage, status, time spent, and last lesson reached.
        /// Only applicable for PathCourse and SkillsLibCourse types.
        /// </summary>
        Task<GeneralResult<CourseProgressDetailsDto>> GetCourseProgressAsync(int courseId, string userId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the progress details of a user for all courses they have completed.
        /// </summary>
        Task<GeneralResult<List<CourseProgressDetailsDto>>> GetUserCoursesProgressAsync(string userId, CancellationToken cancellationToken);

        /// <summary>
        /// Updates the student's progress for a specific training path.
        /// Calculates completion percentage based on completed lessons in all path courses.
        /// </summary>
        Task<GeneralResult> UpdateTrainingPathProgressAsync(int pathId, string userId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the user's progress in a specific training path, including percentage completion and status.
        /// </summary>
        Task<GeneralResult<TrainingPathProgressDetailsDto>> GetTrainingPathProgressAsync(int pathId, string userId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a list of all training paths where the user has recorded progress,
        /// including completion status and performance summary for each path.
        /// </summary>
        Task<GeneralResult<List<TrainingPathProgressDetailsDto>>> GetUserTrainingPathsProgressAsync(string userId, CancellationToken cancellationToken);

        /// <summary>
        /// Starts a lesson session for a specific user.
        /// </summary>
        Task<GeneralResult> StartLessonSessionAsync(string userId, int lessonId, CancellationToken cancellationToken);

        /// <summary>
        /// Ends a lesson session for a specific user.
        /// </summary>
        Task<GeneralResult> EndLessonSessionAsync(string userId, int lessonId, CancellationToken cancellationToken);

        /// <summary>
        /// Recalculates and synchronizes progress for all users enrolled in a training path, 
        /// updating progress at both the course and path levels.
        Task<GeneralResult> SyncAllUserProgressForPathAsync(int pathId, CancellationToken cancellationToken);
    }
}
