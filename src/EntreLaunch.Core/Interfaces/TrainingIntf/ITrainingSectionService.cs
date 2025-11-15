namespace EntreLaunch.Interfaces.TrainingIntf
{
    public interface ITrainingSectionService
    {
        /// <summary>
        /// Get course enrollments with user data.
        /// </summary>
        Task<GeneralResult<(int totalEnrollments, List<EnrollmentWithUserData> enrollments)>> GetCourseEnrollmentsWithUsersAsync(int courseId);

        /// <summary>
        /// Enroll user to course if paid.
        /// </summary>
        Task<GeneralResult> VerifyCourseEnrollmentEligibilityAsync(int courseId, string userId);

        /// <summary>
        /// Get course based on status.
        /// </summary>
        Task<GeneralResult<List<CourseDetailsDto>>> GetCourseBasedOnStatusAsync(int status);

        /// <summary>
        /// Change course status.
        /// </summary>
        Task<GeneralResult> ChangeCourseStatusAsync(int courseId, CourseStatus newStatus);

        /// <summary>
        /// Get courses by payment type free or paid.
        /// </summary>
        Task<GeneralResult<List<CourseDetailsDto>>> GetCoursesByPaymentTypeAsync(bool isFree);

        /// <summary>
        /// Reorder lessons.
        /// </summary>
        Task<GeneralResult<bool>> ReorderLessonsAsync(int courseId, List<LessonReorderDto> newOrderList);

        /// <summary>
        /// Enroll User To Course.
        /// </summary>
        Task<GeneralResult> EnrollUserToCourseAsync(CourseEnrollmentCreateDto createDto);

        /// <summary>
        /// Get course rating statistics.
        /// </summary>
        Task<GeneralResult<(double AverageRating, int RatingCount)>> GetCourseRatingStatisticsAsync(int courseId);

        /// <summary>
        /// Get all ratings for course.
        /// </summary>
        Task<GeneralResult<List<CourseRatingDetailsDto>>> GetAllRatingsForCourseAsync(int courseId);

        /// <summary>
        /// Unenroll user from course.
        /// </summary>
        Task<GeneralResult> UnenrollUserFromCourseAsync(int courseId, string userId);

        /// <summary>
        /// Get user subscriptions in courses.
        /// </summary>
        Task<GeneralResult<List<UserSubscriptionDto>>> GetUserSubscriptionsAsync(string userId);

        /// <summary>
        /// Get courses by price type.
        /// </summary>
        Task<GeneralResult<List<CourseDetailsDto>>> GetCoursesByPriceTypeAsync(bool isFree);

        /// <summary>
        /// Get instructors by course id.
        /// </summary>
        Task<GeneralResult<List<InstructorDetails>>> GetInstructorsByCourseIdAsync(int courseId);

        /// <summary>
        /// Get trainer performance.
        /// </summary>
        Task<GeneralResult<TrainerPerformanceDto>> GetTrainerPerformanceAsync(string trainerId);

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

        /// <summary>
        /// Increment attachment open count.
        /// </summary>
        Task<GeneralResult> IncrementAttachmentOpenCountAsync(int attachmentId);

        /// <summary>
        /// Get attachment stats.
        /// </summary>
        Task<GeneralResult<AttachmentStatsDto?>> GetAttachmentStatsAsync(int attachmentId);

        /// <summary>
        /// Validates the file is valid.
        /// </summary>
        Task<GeneralResult<bool>> IsValidFile(string filePath);

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
        /// Check if certificate is issued.
        /// </summary>
        Task<GeneralResult<CertificateResult<StudentCertificateDto>>> CheckIfCertificateIssuedAsync(int enrollmentId, int courseId);

        /// <summary>
        /// Issue certificate.
        /// </summary>
        Task<GeneralResult<StudentCertificate>> IssueCertificateAsync(int examId, string userId);

        /// <summary>
        /// Retrieves all lessons for a specific course.
        /// </summary>
        Task<GeneralResult<List<LessonDetailsDto>>> GetLessonsByCourseIdAsync(int courseId);

        /// <summary>
        /// Retrieves all related data for a specific course.
        /// </summary>
        Task<GeneralResult<CourseFullDetailsDto>> GetCourseFullDetailsAsync(int courseId);

        /// <summary>
        /// Retrieves an exam with its questions and answers.
        /// </summary>
        Task<GeneralResult<ExamFullDetailsDto>> GetExamFullDetailsAsync(int examId);

        /// <summary>
        /// Retrieves all questions and answers for a specific exam.
        /// </summary>
        Task<GeneralResult<List<QuestionWithAnswersFullData>>> GetQuestionsWithAnswersByExamIdAsync(int examId);
    }
}
