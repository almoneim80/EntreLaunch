using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Services.TrainingSvc
{
    public class SkillCourseService(
        PgDbContext dbContext,
        ILogger<SkillCourseService> logger,
        ICourseService courseService,
        ILocalizationManager localizationManager) : ISkillCourseService
    {
        private readonly PgDbContext _dbContext = dbContext;
        private readonly ILogger<SkillCourseService> _logger = logger;

        /// <inheritdoc />
        public async Task<GeneralResult<List<SkillCourseDetailsDto>>> GetByFieldAsync(int fieldId)
        {
            try
            {
                var fieldExists = await _dbContext.CourseFields
                    .AsNoTracking()
                    .AnyAsync(f => f.Id == fieldId && !f.IsDeleted);

                if (!fieldExists)
                {
                    _logger.LogInformation("No field found with id {fieldId}", fieldId);
                    return new GeneralResult<List<SkillCourseDetailsDto>>(
                        false, localizationManager.GetLocalizedString("NoFieldsFound"), null);
                }

                var skillCourses = await _dbContext.Courses
                    .Where(c => c.Type == CourseType.SkillsLibCourse && c.FieldId == fieldId && !c.IsDeleted)
                    .Include(c => c.CourseField)
                    .Include(c => c.Lessons!) 
                        .ThenInclude(l => l.LessonAttachments!)
                    .Include(c => c.Exams!)
                        .ThenInclude(e => e.Questions!)
                            .ThenInclude(q => q.Answers!)
                    .Include(c => c.CourseTags!)
                        .ThenInclude(ct => ct.Tag!)
                    .Include(c => c.CourseRatings!)
                        .ThenInclude(r => r.User!)
                    .Include(c => c.CourseInstructors!)
                        .ThenInclude(ci => ci.User!)
                    .Include(c => c.StudentProgresses!)
                        .ThenInclude(sp => sp.User!)
                    .ToListAsync();

                if (!skillCourses.Any())
                {
                    _logger.LogInformation("No skill courses found with field id {fieldId}", fieldId);
                    return new GeneralResult<List<SkillCourseDetailsDto>>(false, localizationManager.GetLocalizedString("NoCoursesFound"));
                }

                var result = skillCourses.Select(courseService.MapToSkillDto).ToList();

                return new GeneralResult<List<SkillCourseDetailsDto>>(
                    true, localizationManager.GetLocalizedString("CoursesRetrieved"), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving skill courses with fieldId = {fieldId}", fieldId);
                return new GeneralResult<List<SkillCourseDetailsDto>>(false, localizationManager.GetLocalizedString("ErrorRetrievingCourses"));
            }
        }
    }
}
