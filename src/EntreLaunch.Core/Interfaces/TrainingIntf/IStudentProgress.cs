namespace EntreLaunch.Interfaces.TrainingIntf
{
    public interface IStudentProgress
    {
        /// <summary>
        /// EntreLaunchdates the progress of a student for a specific course.
        /// </summary>
        Task<GeneralResult<bool>> EntreLaunchdateStudentProgressAsync(int courseId, string userId, int lessonId, TimeSpan timeSpent);

        /// <summary>
        /// Retrieves the progress of a student for a specific course.
        /// </summary>
        Task<GeneralResult<StudentProgressDto>> GetStudentProgressAsync(int courseId, string userId);

        /// <summary>
        /// Calculates the completion percentage of a student for a specific course.
        /// </summary>
        Task<GeneralResult<double>> CalculateCompletionPercentageAsync(int courseId, string userId);
    }
}
