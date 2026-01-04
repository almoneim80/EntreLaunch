using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Services.TrainingSvc
{
    public class PathCourseService(
        PgDbContext dbContext,
        ILogger<PathCourseService> logger,
        ICourseService courseService,
        ILocalizationManager localizationManager) : IPathCourseService
    {
        private readonly PgDbContext _dbContext = dbContext;
        private readonly ILogger<PathCourseService> _logger = logger;

        /// <inheritdoc />
        public async Task<GeneralResult<List<PathCourseDetailsDto>>> GetByPathAsync(int pathId)
        {
            try
            {
                var fieldExists = await _dbContext.TrainingPaths
                    .AsNoTracking()
                    .AnyAsync(p => p.Id == pathId && !p.IsDeleted);

                if (!fieldExists)
                {
                    _logger.LogInformation("No field found with id {fieldId}", pathId);
                    return new GeneralResult<List<PathCourseDetailsDto>>(
                        false, localizationManager.GetLocalizedString("NoPathsFound"), null);
                }

                var pathCourses = await _dbContext.Courses
                    .Where(c => c.Type == CourseType.PathCourse && c.PathId == pathId && !c.IsDeleted)
                    .Include(c => c.Lessons!)
                        .ThenInclude(l => l.LessonAttachments!)
                    .Include(c => c.Exams!)
                        .ThenInclude(e => e.Questions!)
                            .ThenInclude(q => q.Answers!)
                    .Include(c => c.CourseTags!)
                        .ThenInclude(ct => ct.Tag!)
                    .Include(c => c.StudentProgresses!)
                        .ThenInclude(sp => sp.User!)
                    .ToListAsync();

                if (!pathCourses.Any())
                {
                    _logger.LogInformation("No courses found with path id {pathId}", pathId);
                    return new GeneralResult<List<PathCourseDetailsDto>>(false, localizationManager.GetLocalizedString("NoCoursesFound"));
                }

                var result = pathCourses.Select(courseService.MapToPathDto).ToList();

                return new GeneralResult<List<PathCourseDetailsDto>>(
                    true, localizationManager.GetLocalizedString("CoursesRetrieved"), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving skill courses with fieldId = {fieldId}", pathId);
                return new GeneralResult<List<PathCourseDetailsDto>>(false, localizationManager.GetLocalizedString("ErrorRetrievingCourses"));
            }
        }
    }
}
