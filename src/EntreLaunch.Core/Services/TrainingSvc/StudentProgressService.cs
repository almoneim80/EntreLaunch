namespace EntreLaunch.Services.TrainingSvc
{
    public class StudentProgressService : IStudentProgress
    {
        private readonly PgDbContext _dbContext;
        private readonly ILogger<StudentProgressService> _logger;
        public StudentProgressService(PgDbContext dbContext, ILogger<StudentProgressService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<GeneralResult<bool>> EntreLaunchdateStudentProgressAsync(int courseId, string userId, int lessonId, TimeSpan timeSpent)
        {
            try
            {
                // Verify the existence of the course, user, and lesson
                var courseExists = await _dbContext.Courses.AnyAsync(c => c.Id == courseId && !c.IsDeleted);
                if (!courseExists)
                {
                    _logger.LogWarning("Course with ID {CourseId} not found.", courseId);
                    return new GeneralResult<bool>(false, "Course not found.", false );
                }

                var userExists = await _dbContext.Users
                    .AnyAsync(u => u.Id == userId && !u.IsDeleted);
                if (!userExists)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", userId);
                    return new GeneralResult<bool>(false, "User not found.", false);
                }

                var lessonExists = await _dbContext.Lessons.AnyAsync(l => l.Id == lessonId && l.CourseId == courseId && !l.IsDeleted);
                if (!lessonExists)
                {
                    _logger.LogWarning("Lesson with ID {LessonId} not found in Course {CourseId}.", lessonId, courseId);
                    return new GeneralResult<bool>(false, "Lesson not found.", false);
                }

                // EntreLaunchdate Student Progress
                var progress = await _dbContext.StudentProgresses
                    .FirstOrDefaultAsync(p => p.UserId == userId && p.CourseId == courseId);

                if (progress == null)
                {
                    // Create a new record if it doesn't exist
                    progress = new StudentProgress
                    {
                        UserId = userId,
                        CourseId = courseId,
                        LastLessonId = lessonId,
                        TotalTimeSpent = timeSpent,
                        CompletionPercentage = 0
                    };

                    _dbContext.StudentProgresses.Add(progress);
                }
                else
                {
                    // EntreLaunchdate Current Record
                    progress.LastLessonId = lessonId;
                    progress.TotalTimeSpent += timeSpent;

                    // Calculate the ratio based on the number of lessons completed
                    var totalLessons = await _dbContext.Lessons
                        .CountAsync(l => l.CourseId == courseId && !l.IsDeleted);
                    var completedLessons = await _dbContext.Lessons
                        .CountAsync(l => l.Id == lessonId && l.CourseId == courseId);

                    progress.CompletionPercentage = completedLessons / (double)totalLessons * 100;
                }

                await _dbContext.SaveChangesAsync();
                return new GeneralResult<bool>(true, "Progress EntreLaunchdated successfully.", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error EntreLaunchdating progress for User {UserId} in Course {CourseId}.", userId, courseId);
                return new GeneralResult<bool>(false, "Error EntreLaunchdating progress.", false);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<StudentProgressDto>> GetStudentProgressAsync(int courseId, string userId)
        {
            try
            {
                // Verifying the existence of the registry
                var progress = await _dbContext.StudentProgresses
                    .Where(p => p.CourseId == courseId && p.UserId == userId)
                    .Select(p => new StudentProgressDto
                    {
                        UserId = p.UserId,
                        CourseId = p.CourseId,
                        LastLessonId = p.LastLessonId,
                        TotalTimeSpent = p.TotalTimeSpent,
                        CompletionPercentage = p.CompletionPercentage
                    })
                    .FirstOrDefaultAsync();

                if (progress == null)
                {
                    _logger.LogInformation("No progress found for User {UserId} in Course {CourseId}.", userId, courseId);
                    return null!;
                }

                return new GeneralResult<StudentProgressDto>(true, "Progress retrieved successfully.", progress);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving progress for User {UserId} in Course {CourseId}.", userId, courseId);
                return new GeneralResult<StudentProgressDto>(false, "Error retrieving progress.", null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<double>> CalculateCompletionPercentageAsync(int courseId, string userId)
        {
            try
            {
                // Total of lessons
                var totalLessons = await _dbContext.Lessons.CountAsync(l => l.CourseId == courseId && !l.IsDeleted);

                // Number of lessons completed
                var completedLessons = await _dbContext.StudentProgresses
                    .CountAsync(sp => sp.CourseId == courseId && sp.UserId == userId && sp.IsCompleted);

                return new GeneralResult<double>(true, "Completion percentage calculated successfully.", completedLessons / (double)totalLessons * 100);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating completion percentage for User {UserId} in Course {CourseId}.", userId, courseId);
                return new GeneralResult<double>(false, "Error calculating completion percentage.", 0);
            }
        }
    }
}
