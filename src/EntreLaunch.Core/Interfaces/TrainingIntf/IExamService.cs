using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.ExamDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Interfaces.TrainingIntf
{
    public interface IExamService
    {
        /// <summary>
        /// create lesson full exam.
        /// </summary>
        Task<GeneralResult> CreateLessonExam(FullLessonExamDto dto);

        /// <summary>
        /// create course full exam.
        /// </summary>
        Task<GeneralResult> CreateCourseExam(FullCourseExamDto dto);

        /// <summary>
        /// create path full exam.
        /// </summary>
        Task<GeneralResult> CreatePathExam(FullPathExamDto dto);

        /// <summary>
        /// Get exam by lesson id.
        /// </summary>
        Task<GeneralResult<ExamFullDetailsDto>> GetExamByLessonIdAsync(int lessonId);

        /// <summary>
        /// Get exam by course id.
        /// </summary>
        Task<GeneralResult<ExamFullDetailsDto>> GetExamByCourseIdAsync(int courseId);

        /// <summary>
        /// Get path exams.
        /// </summary>
        Task<GeneralResult<PaginatedResult<ExamFullDetailsDto>>> GetPathExamsAsync(PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Update lesson exam details.
        /// </summary>
        Task<GeneralResult> UpdateLessonExamAsync(int examId, UpdateLessonExamDto dto);

        /// <summary>
        /// Update course exam details.
        /// </summary>
        Task<GeneralResult> UpdateCourseExamAsync(int examId, UpdateCourseExamDto dto);

        /// <summary>
        /// Update path exam details.
        /// </summary>
        Task<GeneralResult> UpdatePathExamAsync(int examId, UpdatePathExamDto dto);

        /// <summary>
        /// Soft delete Exam.
        /// </summary>
        Task<GeneralResult> SoftDeleteExamAsync(int examId);

        /// <summary>
        /// Get student result.
        /// </summary>
        Task<GeneralResult<ExamResultWithStudentDto?>> GetExamResultForStudentAsync(int examId, string userId);

        /// <summary>
        /// Calculates the exam result.
        /// </summary>
        Task<GeneralResult<ExamResultDto>> CalculateExamResultAsync(int examId, string userId, List<UserAnswerDto> userAnswers, int timeTakenInSeconds);

        /// <summary>
        /// Compare student result with batch.
        /// </summary>
        Task<GeneralResult<StudentComparisonDto>> CompareStudentResultWithBatchAsync(int examId, string userId);

        /// <summary>
        /// Get exam statistics.
        /// </summary>
        Task<GeneralResult<ExamStatisticsDto>> GetExamStatisticsAsync(int examId);

        /// <summary>
        /// Get top 10 students.
        /// </summary>
        Task<GeneralResult<List<TopStudentDto>>> GetTopTenStudentsAsync(int examId);

        /// <summary>
        /// Checks if a student can retake an exam.
        /// </summary>
        Task<GeneralResult<bool>> CanRetakeExamAsync(int examId, string userId);

        /// <summary>
        /// Retake exam.
        /// </summary>
        Task<GeneralResult<ExamResultDto>> RetakeExamAsync(int examId, string userId, List<UserAnswerDto> userAnswers, int timeTakenInSeconds);

        /// <summary>
        /// Get student attempts.
        /// </summary>
        Task<GeneralResult<StudentAttemptsWithBestDto>> GetStudentAttemptsAsync(int examId, string userId);

        /// <summary>
        /// Get active result.
        /// </summary>
        Task<GeneralResult<StudentAttemptDto?>> GetActiveResultAsync(int examId, string userId);

        /// <summary>
        /// Retrieves all questions and answers for a specific exam.
        /// </summary>
        Task<GeneralResult<List<QuestionWithAnswersFullData>>> GetQuestionsWithAnswersByExamIdAsync(int examId);
    }
}
