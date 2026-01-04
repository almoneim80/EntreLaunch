using System.Data;
using EntreLaunch.DTOs.ProgressDtos;
using EntreLaunch.DTOs.TrainingDtos;
namespace EntreLaunch.Services.TrainingSvc
{
    public class StudentProgressService(PgDbContext dbContext, ILogger<StudentProgressService> logger, ILocalizationManager localization) : IStudentProgress
    {
        private readonly PgDbContext _dbContext = dbContext;
        private readonly ILogger<StudentProgressService> _logger = logger;
        private readonly ILocalizationManager _localization = localization;

        /// <inheritdoc/>
        public async Task<GeneralResult> MarkLessonCompletedAsync(int lessonId, string userId, CancellationToken cancellationToken)
        {
            const string method = nameof(MarkLessonCompletedAsync);
            try
            {
                if (lessonId <= 0)
                {
                    _logger.LogInformation("{Method} - Invalid lesson id {LessonId}.", method, lessonId);
                    return new GeneralResult(false, _localization.GetLocalizedString("IdInvalid"), null, ErrorType.BadRequest);
                }

                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogInformation("{Method} - Invalid user id.", method);
                    return new GeneralResult(false, _localization.GetLocalizedString("IdInvalid"), null, ErrorType.BadRequest);
                }

                var lesson = await _dbContext.Lessons
                    .Include(l => l.Course)
                    .FirstOrDefaultAsync(l => l.Id == lessonId && !l.IsDeleted, cancellationToken);

                if (lesson == null)
                {
                    _logger.LogInformation("{Method} - Lesson {LessonId} not found.", method, lessonId);
                    return new GeneralResult(false, _localization.GetLocalizedString("LessonNotFound"), null, ErrorType.NotFound);
                }

                if (lesson.Course == null || lesson.Course.Type == CourseType.OnlineCourse)
                {
                    _logger.LogInformation("{Method} - Lesson {LessonId} belongs to an OnlineCourse. Skipping progress tracking.", method, lessonId);
                    return new GeneralResult(false, _localization.GetLocalizedString("OnlineCourseNoProgress"), null, ErrorType.BadRequest);
                }

                var user = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken);

                if (user == null)
                {
                    _logger.LogInformation("{Method} - User {UserId} not found.", method, userId);
                    return new GeneralResult(false, _localization.GetLocalizedString("UserNotFound"), null, ErrorType.NotFound);
                }

                var existing = await _dbContext.LessonProgresses
                    .FirstOrDefaultAsync(lp => lp.UserId == userId && lp.LessonId == lessonId && !lp.IsDeleted, cancellationToken);

                if (existing != null && existing.IsCompleted)
                {
                    _logger.LogInformation("{Method} - Lesson {LessonId} already marked as completed for user {UserId}.", method, lessonId, userId);
                    return new GeneralResult(true, _localization.GetLocalizedString("LessonAlreadyCompleted"), null, ErrorType.Success);
                }

                if (existing != null)
                {
                    existing.IsCompleted = true;
                    existing.CompletedAt = DateHelper.UtcNow;
                    existing.UpdatedAt = DateHelper.UtcNow;

                    _logger.LogInformation("{Method} - Lesson {LessonId} updated to completed for user {UserId}.", method, lessonId, userId);
                }
                else
                {
                    var progress = new LessonProgress
                    {
                        UserId = userId,
                        LessonId = lessonId,
                        IsCompleted = true,
                        CompletedAt = DateHelper.UtcNow,
                        CreatedAt = DateHelper.UtcNow
                    };

                    await _dbContext.LessonProgresses.AddAsync(progress, cancellationToken);

                    _logger.LogInformation("{Method} - Lesson {LessonId} marked as completed for user {UserId}.", method, lessonId, userId);
                }

                await _dbContext.SaveChangesAsync(cancellationToken);

                await UpdateCourseProgressAsync(lesson.CourseId, userId, cancellationToken);

                if (lesson.Course.Type == CourseType.PathCourse && lesson.Course.PathId.HasValue)
                {
                    await UpdateTrainingPathProgressAsync(lesson.Course.PathId.Value, userId, cancellationToken);
                }

                return new GeneralResult(true, _localization.GetLocalizedString("LessonCompleted"), null, ErrorType.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Method} - Error marking lesson as completed for user {UserId}.", method, userId);
                return new GeneralResult(false, _localization.GetLocalizedString("UnexpectedError_MarkLesson"), null, ErrorType.InternalServerError);
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult<LessonProgressDetailsDto>> GetLessonProgressAsync(int lessonId, string userId, CancellationToken cancellationToken)
        {
            const string method = nameof(GetLessonProgressAsync);
            try
            {
                if (lessonId <= 0)
                {
                    _logger.LogWarning("{Method}: Invalid lessonId {LessonId}.", method, lessonId);
                    return new GeneralResult<LessonProgressDetailsDto>(false, _localization.GetLocalizedString("IdInvalid"), null, ErrorType.BadRequest);
                }

                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogWarning("{Method}: Invalid UserId.", method);
                    return new GeneralResult<LessonProgressDetailsDto>(false, _localization.GetLocalizedString("IdInvalid"), null, ErrorType.BadRequest);
                }

                var lesson = await _dbContext.Lessons
                    .Include(l => l.Course!)
                        .ThenInclude(c => c.TrainingPath)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(l => l.Id == lessonId && !l.IsDeleted, cancellationToken);

                if (lesson == null)
                {
                    _logger.LogInformation("{Method}: Lesson {LessonId} not found.", method, lessonId);
                    return new GeneralResult<LessonProgressDetailsDto>(false, _localization.GetLocalizedString("LessonNotFound"), null, ErrorType.NotFound);
                }

                if (lesson.Course.Type == CourseType.OnlineCourse)
                {
                    _logger.LogInformation("{Method}: Lesson {LessonId} belongs to an OnlineCourse and not trackable.", method, lessonId);
                    return new GeneralResult<LessonProgressDetailsDto>(false, _localization.GetLocalizedString("CourseTypeNotSupported"), null, ErrorType.BadRequest);
                }

                var progress = await _dbContext.LessonProgresses
                    .AsNoTracking()
                    .FirstOrDefaultAsync(lp => lp.LessonId == lessonId && lp.UserId == userId && !lp.IsDeleted, cancellationToken);

                var dto = new LessonProgressDetailsDto
                {
                    LessonId = lesson.Id,
                    LessonName = lesson.Name ?? string.Empty,
                    CourseId = lesson.Course.Id,
                    CourseName = lesson.Course.Name ?? string.Empty,
                    PathId = lesson.Course.PathId,
                    RelatedPathName = lesson.Course.TrainingPath?.Name,
                    IsCompleted = progress?.IsCompleted ?? false,
                    CompletedAt = progress?.CompletedAt,
                    TimeSpent = progress?.TimeSpent ?? TimeSpan.Zero
                };

                return new GeneralResult<LessonProgressDetailsDto>(true, _localization.GetLocalizedString("LessonProgressRetrieved"), dto, ErrorType.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Method}: Error retrieving lesson progress for user {UserId}.", method, userId);
                return new GeneralResult<LessonProgressDetailsDto>(false, _localization.GetLocalizedString("UnexpectedError_GetLessonProgress"), null, ErrorType.InternalServerError);
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult<List<LessonProgressDetailsDto>>> GetCourseLessonsProgressAsync(int courseId, string userId, CancellationToken cancellationToken)
        {
            const string method = nameof(GetCourseLessonsProgressAsync);
            try
            {
                if (courseId <= 0 || string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogWarning("{Method}: Invalid CourseId or UserId. courseId={CourseId}, userId={UserId}",
                        method, courseId, userId);
                    return new GeneralResult<List<LessonProgressDetailsDto>>(false,
                        _localization.GetLocalizedString("IdInvalid"),
                        null, ErrorType.BadRequest);
                }

                var userExists = await _dbContext.Users.AsNoTracking()
                    .AnyAsync(u => u.Id == userId && !u.IsDeleted && u.IsActive, cancellationToken);
                if (!userExists)
                {
                    _logger.LogInformation("{Method}: User {UserId} not found.", method, userId);
                    return new GeneralResult<List<LessonProgressDetailsDto>>(false,
                        _localization.GetLocalizedString("UserNotFound"),
                        null, ErrorType.NotFound);
                }

                var course = await _dbContext.Courses.AsNoTracking()
                    .Include(c => c.Lessons)
                    .Include(c => c.TrainingPath)
                    .FirstOrDefaultAsync(c => c.Id == courseId && !c.IsDeleted &&
                                              (c.Type == CourseType.PathCourse || c.Type == CourseType.SkillsLibCourse),
                        cancellationToken);
                if (course == null)
                {
                    _logger.LogInformation("{Method}: Course {CourseId} not found or not trackable.", method, courseId);
                    return new GeneralResult<List<LessonProgressDetailsDto>>(false,
                        _localization.GetLocalizedString("CourseNotFoundOrNotTrackable"),
                        null, ErrorType.NotFound);
                }

                var lessonIds = course.Lessons!
                    .Where(l => !l.IsDeleted)
                    .Select(l => l.Id)
                    .ToList();
                if (!lessonIds.Any())
                {
                    _logger.LogInformation("{Method}: Course {CourseId} has no lessons.", method, courseId);
                    return new GeneralResult<List<LessonProgressDetailsDto>>(false,
                        _localization.GetLocalizedString("NoLessonsInCourse"),
                        null, ErrorType.NotFound);
                }

                var progresses = await _dbContext.LessonProgresses.AsNoTracking()
                    .Where(lp => lp.UserId == userId &&
                                 lessonIds.Contains(lp.LessonId) &&
                                 !lp.IsDeleted)
                    .Include(lp => lp.Lesson)
                        .ThenInclude(l => l.Course)
                    .ToListAsync(cancellationToken);

                var dtoList = lessonIds
                    .Select(id =>
                    {
                        var lp = progresses.FirstOrDefault(p => p.LessonId == id);
                        var lesson = course.Lessons!.First(l => l.Id == id);
                        return new LessonProgressDetailsDto
                        {
                            LessonId = lesson.Id,
                            LessonName = lesson.Name ?? string.Empty,
                            CourseId = course.Id,
                            CourseName = course.Name ?? string.Empty,
                            PathId = course.PathId,
                            RelatedPathName = course.TrainingPath?.Name ?? string.Empty,
                            IsCompleted = lp?.IsCompleted ?? false,
                            CompletedAt = lp?.CompletedAt,
                            TimeSpent = lp?.TimeSpent ?? TimeSpan.Zero
                        };
                    })
                    .ToList();

                _logger.LogInformation("{Method}: Retrieved {Count} lessons progress for user {UserId} in course {CourseId}.",
                    method, dtoList.Count, userId, courseId);

                return new GeneralResult<List<LessonProgressDetailsDto>>(true,
                    _localization.GetLocalizedString("LessonProgressRetrieved"),
                    dtoList, ErrorType.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Method}: Error retrieving course lessons progress for user {UserId}.", method, userId);
                return new GeneralResult<List<LessonProgressDetailsDto>>(false,
                    _localization.GetLocalizedString("UnexpectedError_GetLessonProgressList"),
                    null, ErrorType.InternalServerError);
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult> UpdateCourseProgressAsync(int courseId, string userId, CancellationToken cancellationToken)
        {
            const string method = nameof(UpdateCourseProgressAsync);
            try
            {
                if (courseId <= 0)
                {
                    _logger.LogWarning("{Method}: Invalid CourseId {CourseId}.", method, courseId);
                    return new GeneralResult(false, _localization.GetLocalizedString("IdInvalid"), null, ErrorType.BadRequest);
                }

                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogWarning("{Method}: Invalid UserId.", method);
                    return new GeneralResult(false, _localization.GetLocalizedString("IdInvalid"), null, ErrorType.BadRequest);
                }

                var course = await _dbContext.Courses
                    .Include(c => c.Lessons)
                    .FirstOrDefaultAsync(c => c.Id == courseId && !c.IsDeleted, cancellationToken);

                if (course == null || course.Lessons == null || !course.Lessons.Any())
                {
                    _logger.LogInformation("{Method}: Course {CourseId} not found or has no lessons.", method, courseId);
                    return new GeneralResult(false, _localization.GetLocalizedString("CourseNotFoundOrEmpty"), null, ErrorType.NotFound);
                }

                if (course.Type == CourseType.OnlineCourse)
                {
                    _logger.LogInformation("StudentProgressService - UpdateCourseProgressAsync : Course {CourseId} is OnlineCourse type and not supported for progress tracking.", courseId);
                    return new GeneralResult(false, _localization.GetLocalizedString("CourseTypeNotSupported"), null, ErrorType.BadRequest);
                }

                var lessonIds = course.Lessons.Select(l => l.Id).ToList();
                var completedIds = await _dbContext.LessonProgresses.AsNoTracking()
                    .Where(lp => lp.UserId == userId && !lp.IsDeleted && lp.IsCompleted && lessonIds.Contains(lp.LessonId))
                    .Select(lp => lp.LessonId)
                    .ToListAsync(cancellationToken);

                if (!completedIds.Any())
                {
                    _logger.LogInformation("{Method}: No completed lessons for user {UserId} in course {CourseId}.", method, userId, courseId);
                    return new GeneralResult(false, _localization.GetLocalizedString("NoCompletedLessons"), null, ErrorType.NotFound);
                }

                var percentage = (double)completedIds.Count / course.Lessons.Count * 100;
                var isCompleted = completedIds.Count == course.Lessons.Count;

                var existing = await _dbContext.StudentProgresses
                    .FirstOrDefaultAsync(sp => sp.UserId == userId && sp.CourseId == courseId && !sp.IsDeleted, cancellationToken);

                if (existing == null)
                {
                    var prog = new StudentProgress
                    {
                        UserId = userId,
                        CourseId = courseId,
                        LastLessonId = completedIds.Max(),
                        CompletionPercentage = percentage,
                        IsCompleted = isCompleted,
                        TotalTimeSpent = TimeSpan.FromTicks(
                            _dbContext.LessonProgresses
                                .Where(lp => lp.UserId == userId && lessonIds.Contains(lp.LessonId) && !lp.IsDeleted)
                                .Sum(lp => lp.TimeSpent.Ticks)
                        ),
                        CreatedAt = DateHelper.UtcNow
                    };
                    await _dbContext.StudentProgresses.AddAsync(prog, cancellationToken);
                }
                else
                {
                    existing.CompletionPercentage = percentage;
                    existing.IsCompleted = isCompleted;
                    existing.LastLessonId = completedIds.Max();
                    existing.TotalTimeSpent = TimeSpan.FromTicks(
                        _dbContext.LessonProgresses
                            .Where(lp => lp.UserId == userId && lessonIds.Contains(lp.LessonId) && !lp.IsDeleted)
                            .Sum(lp => lp.TimeSpent.Ticks)
                    );
                    existing.UpdatedAt = DateHelper.UtcNow;
                }

                await _dbContext.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("{Method}: Course progress updated for user {UserId} in course {CourseId}.", method, userId, courseId);

                if (course.Type == CourseType.PathCourse && course.PathId.HasValue)
                {
                    await UpdateTrainingPathProgressAsync(course.PathId.Value, userId, cancellationToken);
                }

                return new GeneralResult(true, _localization.GetLocalizedString("CourseProgressUpdated"), null, ErrorType.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Method}: Error updating course progress for user {UserId} in course {CourseId}.", method, userId, courseId);
                return new GeneralResult(false, _localization.GetLocalizedString("UnexpectedError_UpdateCourse"), null, ErrorType.InternalServerError);
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult<CourseProgressDetailsDto>> GetCourseProgressAsync(int courseId, string userId, CancellationToken cancellationToken)
        {
            const string method = nameof(GetCourseProgressAsync);
            try
            {
                if (courseId <= 0)
                {
                    _logger.LogWarning("{Method}: Invalid CourseId {CourseId}.", method, courseId);
                    return new GeneralResult<CourseProgressDetailsDto>(false, _localization.GetLocalizedString("IdInvalid"), null, ErrorType.BadRequest);
                }

                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogWarning("{Method}: Invalid UserId.", method);
                    return new GeneralResult<CourseProgressDetailsDto>(false, _localization.GetLocalizedString("IdInvalid"), null, ErrorType.BadRequest);
                }

                var userExists = await _dbContext.Users
                    .AsNoTracking()
                    .AnyAsync(u => u.Id == userId && !u.IsDeleted && u.IsActive, cancellationToken);

                if (!userExists)
                {
                    _logger.LogInformation("{Method}: User {UserId} not found.", method, userId);
                    return new GeneralResult<CourseProgressDetailsDto>(false, _localization.GetLocalizedString("UserNotFound"), null, ErrorType.NotFound);
                }

                var course = await _dbContext.Courses
                    .AsNoTracking()
                    .Include(c => c.Lessons)
                    .Include(c => c.TrainingPath)
                    .FirstOrDefaultAsync(c => c.Id == courseId && !c.IsDeleted
                     && (c.Type == CourseType.PathCourse || c.Type == CourseType.SkillsLibCourse), cancellationToken);

                if (course == null || course.Type == CourseType.OnlineCourse)
                {
                    _logger.LogInformation("{Method}: Course {CourseId} not found or not trackable.", method, courseId);
                    return new GeneralResult<CourseProgressDetailsDto>(false, _localization.GetLocalizedString("CourseNotFoundOrNotTrackable"), null, ErrorType.BadRequest);
                }

                var lessons = course.Lessons?.Where(l => !l.IsDeleted).ToList() ?? new();
                if (!lessons.Any())
                {
                    _logger.LogInformation("{Method}: Course {CourseId} has no lessons.", method, courseId);
                    return new GeneralResult<CourseProgressDetailsDto>(false, _localization.GetLocalizedString("NoLessonsInCourse"), null, ErrorType.NotFound);
                }

                var lessonIds = lessons.Select(l => l.Id).ToList();

                var lessonProgresses = await _dbContext.LessonProgresses
                    .AsNoTracking()
                    .Where(lp => lp.UserId == userId && lessonIds.Contains(lp.LessonId) && !lp.IsDeleted)
                    .ToListAsync(cancellationToken);

                var progress = await _dbContext.StudentProgresses
                    .AsNoTracking()
                    .FirstOrDefaultAsync(sp => sp.UserId == userId && sp.CourseId == courseId && !sp.IsDeleted, cancellationToken);

                var completedLessons = lessonProgresses.Count(lp => lp.IsCompleted);
                var completionPercentage = (double)completedLessons / lessons.Count * 100;
                var totalTime = lessonProgresses.Sum(lp => lp.TimeSpent.Ticks);
                //var lastCompletedAt = lessonProgresses.Where(lp => lp.IsCompleted).Max(lp => lp.CompletedAt);

                var dto = new CourseProgressDetailsDto
                {
                    CourseId = course.Id,
                    CourseName = course.Name ?? string.Empty,
                    PathId = course.PathId,
                    RelatedPathName = course.TrainingPath?.Name,
                    CertificateExists = course.CertificateExists,
                    CertificateValidityInDays = course.CertificateValidityInDays,
                    IsCompleted = progress?.IsCompleted ?? false,
                    CompletionPercentage = progress?.CompletionPercentage ?? completionPercentage,
                    CompletedAt = progress?.IsCompleted == true ? progress.UpdatedAt ?? progress.CreatedAt : null,
                    TotalTimeSpent = TimeSpan.FromTicks(totalTime),
                    Lessons = lessons.Select(lesson =>
                    {
                        var lessonProgress = lessonProgresses.FirstOrDefault(lp => lp.LessonId == lesson.Id);
                        return new LessonProgressSummaryDto
                        {
                            LessonId = lesson.Id,
                            LessonName = lesson.Name ?? string.Empty,
                            IsCompleted = lessonProgress?.IsCompleted ?? false,
                            CompletedAt = lessonProgress?.CompletedAt,
                            TimeSpent = lessonProgress?.TimeSpent ?? TimeSpan.Zero
                        };
                    }).ToList()
                };

                _logger.LogInformation("{Method}: Course progress retrieved for user {UserId} in course {CourseId}.", method, userId, courseId);
                return new GeneralResult<CourseProgressDetailsDto>(true, _localization.GetLocalizedString("CourseProgressRetrieved"), dto, ErrorType.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Method}: Error retrieving course progress for user {UserId} in course {CourseId}.", method, userId, courseId);
                return new GeneralResult<CourseProgressDetailsDto>(false, _localization.GetLocalizedString("UnexpectedError_GetCourseProgress"), null, ErrorType.InternalServerError);
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult<List<CourseProgressDetailsDto>>> GetUserCoursesProgressAsync(string userId, CancellationToken cancellationToken)
        {
            const string method = nameof(GetUserCoursesProgressAsync);
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogWarning("{Method}: Invalid UserId.", method);
                    return new GeneralResult<List<CourseProgressDetailsDto>>(false, _localization.GetLocalizedString("IdInvalid"), null, ErrorType.BadRequest);
                }

                var userExists = await _dbContext.Users.AsNoTracking()
                    .AnyAsync(u => u.Id == userId && !u.IsDeleted && u.IsActive, cancellationToken);
                if (!userExists)
                {
                    _logger.LogInformation("{Method}: User {UserId} not found.", method, userId);
                    return new GeneralResult<List<CourseProgressDetailsDto>>(false, _localization.GetLocalizedString("UserNotFound"), null, ErrorType.NotFound);
                }

                var progresses = await _dbContext.StudentProgresses
                    .AsNoTracking()
                    .Include(sp => sp.Course!)
                        .ThenInclude(c => c.TrainingPath)
                    .Where(sp =>
                        sp.UserId == userId &&
                        !sp.IsDeleted &&
                        sp.Course != null &&
                        (sp.Course.Type == CourseType.PathCourse || sp.Course.Type == CourseType.SkillsLibCourse))
                    .ToListAsync(cancellationToken);

                if (!progresses.Any())
                {
                    _logger.LogInformation("{Method}: No course progress found for user {UserId}.", method, userId);
                    return new GeneralResult<List<CourseProgressDetailsDto>>(true, _localization.GetLocalizedString("NoCourseProgressFound"), new List<CourseProgressDetailsDto>(), ErrorType.Success);
                }

                var result = new List<CourseProgressDetailsDto>();

                foreach (var progress in progresses)
                {
                    var course = progress.Course!;
                    var lessons = course.Lessons?.Where(l => !l.IsDeleted).ToList() ?? new();
                    var lessonIds = lessons.Select(l => l.Id).ToList();

                    var lessonProgresses = await _dbContext.LessonProgresses.AsNoTracking()
                        .Where(lp => lp.UserId == userId && lessonIds.Contains(lp.LessonId) && !lp.IsDeleted)
                        .ToListAsync(cancellationToken);

                    var dto = new CourseProgressDetailsDto
                    {
                        CourseId = course.Id,
                        CourseName = course.Name ?? string.Empty,
                        PathId = course.PathId,
                        RelatedPathName = course.TrainingPath?.Name,
                        CertificateExists = course.CertificateExists,
                        CertificateValidityInDays = course.CertificateValidityInDays,
                        IsCompleted = progress.IsCompleted,
                        CompletionPercentage = progress.CompletionPercentage,
                        CompletedAt = progress.IsCompleted ? progress.UpdatedAt ?? progress.CreatedAt : null,
                        TotalTimeSpent = progress.TotalTimeSpent,
                        Lessons = lessons.Select(lesson =>
                        {
                            var lp = lessonProgresses.FirstOrDefault(x => x.LessonId == lesson.Id);
                            return new LessonProgressSummaryDto
                            {
                                LessonId = lesson.Id,
                                LessonName = lesson.Name ?? string.Empty,
                                IsCompleted = lp?.IsCompleted ?? false,
                                CompletedAt = lp?.CompletedAt,
                                TimeSpent = lp?.TimeSpent ?? TimeSpan.Zero
                            };
                        }).ToList()
                    };

                    result.Add(dto);
                }

                _logger.LogInformation("{Method}: Retrieved {Count} course progress records for user {UserId}.", method, result.Count, userId);
                return new GeneralResult<List<CourseProgressDetailsDto>>(true, _localization.GetLocalizedString("CourseProgressRetrieved"), result, ErrorType.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Method}: Error retrieving course progress for user {UserId}.", method, userId);
                return new GeneralResult<List<CourseProgressDetailsDto>>(false, _localization.GetLocalizedString("UnexpectedError_GetCourseProgressList"), null, ErrorType.InternalServerError);
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult> UpdateTrainingPathProgressAsync(int pathId, string userId, CancellationToken cancellationToken)
        {
            const string method = nameof(UpdateTrainingPathProgressAsync);
            try
            {
                if (pathId <= 0)
                {
                    _logger.LogWarning("{Method}: Invalid PathId {PathId}.", method, pathId);
                    return new GeneralResult(false, _localization.GetLocalizedString("IdInvalid"), null, ErrorType.BadRequest);
                }

                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogWarning("{Method}: Invalid UserId.", method);
                    return new GeneralResult(false, _localization.GetLocalizedString("IdInvalid"), null, ErrorType.BadRequest);
                }

                var path = await _dbContext.TrainingPaths
                    .Include(p => p.Courses!)
                        .ThenInclude(c => c.Lessons)
                    .FirstOrDefaultAsync(p => p.Id == pathId && !p.IsDeleted, cancellationToken);

                if (path == null || path.Courses == null || !path.Courses.Any())
                {
                    _logger.LogInformation("{Method}: Training path {PathId} not found or has no courses.", method, pathId);
                    return new GeneralResult(false, _localization.GetLocalizedString("PathNotFoundOrEmpty"), null, ErrorType.NotFound);
                }

                var pathCourses = path.Courses
                    .Where(c => c.Type == CourseType.PathCourse && !c.IsDeleted && c.Lessons != null && c.Lessons.Any())
                    .ToList();

                if (!pathCourses.Any())
                {
                    _logger.LogInformation("{Method}: No valid PathCourses with lessons found for path {PathId}.", method, pathId);
                    return new GeneralResult(false, _localization.GetLocalizedString("PathNoValidCourses"), null, ErrorType.NotFound);
                }

                var allLessonIds = pathCourses.SelectMany(c => c.Lessons!).Select(l => l.Id).ToList();
                var completedLessonIds = await _dbContext.LessonProgresses.AsNoTracking()
                    .Where(lp => lp.UserId == userId && !lp.IsDeleted && lp.IsCompleted && allLessonIds.Contains(lp.LessonId))
                    .Select(lp => lp.LessonId)
                    .ToListAsync(cancellationToken);

                if (!completedLessonIds.Any())
                {
                    _logger.LogInformation("{Method}: No completed lessons found for user {UserId} in path {PathId}.", method, userId, pathId);
                    return new GeneralResult(false, _localization.GetLocalizedString("NoCompletedLessons"), null, ErrorType.NotFound);
                }

                var totalLessons = allLessonIds.Count;
                var completedCount = completedLessonIds.Count;
                var percentage = (double)completedCount / totalLessons * 100;
                var isCompleted = completedCount == totalLessons;

                var totalTicks = _dbContext.LessonProgresses
                    .Where(lp => lp.UserId == userId && allLessonIds.Contains(lp.LessonId) && !lp.IsDeleted)
                    .Sum(lp => lp.TimeSpent.Ticks);

                var existing = await _dbContext.StudentProgresses
                    .FirstOrDefaultAsync(sp => sp.UserId == userId && sp.PathId == pathId && !sp.IsDeleted, cancellationToken);

                if (existing == null)
                {
                    var newProgress = new StudentProgress
                    {
                        UserId = userId,
                        PathId = pathId,
                        CompletionPercentage = percentage,
                        IsCompleted = isCompleted,
                        TotalTimeSpent = TimeSpan.FromTicks(totalTicks),
                        LastLessonId = completedLessonIds.Max(),
                        CreatedAt = DateHelper.UtcNow
                    };
                    await _dbContext.StudentProgresses.AddAsync(newProgress, cancellationToken);
                }
                else
                {
                    existing.CompletionPercentage = percentage;
                    existing.IsCompleted = isCompleted;
                    existing.TotalTimeSpent = TimeSpan.FromTicks(totalTicks);
                    existing.LastLessonId = completedLessonIds.Max();
                    existing.UpdatedAt = DateHelper.UtcNow;
                }

                await _dbContext.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("{Method}: Training path progress updated for user {UserId} in path {PathId}.", method, userId, pathId);

                return new GeneralResult(true, _localization.GetLocalizedString("PathProgressUpdated"), null, ErrorType.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Method}: Error updating training path progress for user {UserId} in path {PathId}.", method, userId, pathId);
                return new GeneralResult(false, _localization.GetLocalizedString("UnexpectedError_UpdatePath"), null, ErrorType.InternalServerError);
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult<TrainingPathProgressDetailsDto>> GetTrainingPathProgressAsync(int pathId, string userId, CancellationToken cancellationToken)
        {
            const string method = nameof(GetTrainingPathProgressAsync);
            try
            {
                if (pathId <= 0)
                {
                    _logger.LogWarning("{Method}: Invalid PathId {PathId}.", method, pathId);
                    return new GeneralResult<TrainingPathProgressDetailsDto>(false, _localization.GetLocalizedString("IdInvalid"), null, ErrorType.BadRequest);
                }

                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogWarning("{Method}: Invalid UserId.", method);
                    return new GeneralResult<TrainingPathProgressDetailsDto>(false, _localization.GetLocalizedString("IdInvalid"), null, ErrorType.BadRequest);
                }

                var path = await _dbContext.TrainingPaths
                    .AsNoTracking()
                    .Include(p => p.Courses)
                    .FirstOrDefaultAsync(p => p.Id == pathId && !p.IsDeleted, cancellationToken);

                if (path == null)
                {
                    _logger.LogInformation("{Method}: Training path {PathId} not found.", method, pathId);
                    return new GeneralResult<TrainingPathProgressDetailsDto>(false, _localization.GetLocalizedString("TrainingPathNotFound"), null, ErrorType.NotFound);
                }

                var pathCourses = path.Courses?
                    .Where(c => !c.IsDeleted && c.Type == CourseType.PathCourse)
                    .ToList();

                if (pathCourses == null || !pathCourses.Any())
                {
                    _logger.LogInformation("{Method}: No valid courses found for path {PathId}.", method, pathId);
                    return new GeneralResult<TrainingPathProgressDetailsDto>(false, _localization.GetLocalizedString("NoValidCoursesInPath"), null, ErrorType.NotFound);
                }

                var courseIds = pathCourses.Select(c => c.Id).ToList();
                var progresses = await _dbContext.StudentProgresses.AsNoTracking()
                    .Where(sp => sp.UserId == userId && sp.CourseId.HasValue && courseIds.Contains(sp.CourseId.Value) && !sp.IsDeleted)
                    .ToListAsync(cancellationToken);

                var completedCount = progresses.Count(p => p.IsCompleted);
                var totalCourses = courseIds.Count;
                var completionPercentage = totalCourses > 0 ? (double)completedCount / totalCourses * 100 : 0;
                var totalTime = progresses.Sum(p => p.TotalTimeSpent.Ticks);

                var dto = new TrainingPathProgressDetailsDto
                {
                    PathId = path.Id,
                    PathName = path.Name ?? string.Empty,
                    CertificateExists = path.CertificateExists,
                    CertificateValidityInDays = path.CertificateValidityInDays,
                    CompletionPercentage = completionPercentage,
                    IsCompleted = completedCount == totalCourses,
                    CompletedCoursesCount = completedCount,
                    TotalCoursesCount = totalCourses,
                    TotalTimeSpent = TimeSpan.FromTicks(totalTime),
                    CompletedAt = progresses
                        .Where(p => p.IsCompleted)
                        .Max(p => p.UpdatedAt ?? p.CreatedAt) ?? default
                };

                _logger.LogInformation("{Method}: Training path progress retrieved for user {UserId} in path {PathId}.", method, userId, pathId);
                return new GeneralResult<TrainingPathProgressDetailsDto>(true, _localization.GetLocalizedString("TrainingPathProgressRetrieved"), dto, ErrorType.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Method}: Error retrieving training path progress for user {UserId} in path {PathId}.", method, userId, pathId);
                return new GeneralResult<TrainingPathProgressDetailsDto>(false, _localization.GetLocalizedString("UnexpectedError_GetPathProgress"), null, ErrorType.InternalServerError);
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult<List<TrainingPathProgressDetailsDto>>> GetUserTrainingPathsProgressAsync(string userId, CancellationToken cancellationToken)
        {
            const string method = nameof(GetUserTrainingPathsProgressAsync);
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogWarning("{Method}: Invalid UserId.", method);
                    return new GeneralResult<List<TrainingPathProgressDetailsDto>>(false, _localization.GetLocalizedString("IdInvalid"), null, ErrorType.BadRequest);
                }

                var userExists = await _dbContext.Users
                    .AsNoTracking()
                    .AnyAsync(u => u.Id == userId && !u.IsDeleted && u.IsActive, cancellationToken);

                if (!userExists)
                {
                    _logger.LogInformation("{Method}: User {UserId} not found.", method, userId);
                    return new GeneralResult<List<TrainingPathProgressDetailsDto>>(false, _localization.GetLocalizedString("UserNotFound"), null, ErrorType.NotFound);
                }

                var userProgresses = await _dbContext.StudentProgresses
                    .AsNoTracking()
                    .Include(sp => sp.Course!).ThenInclude(c => c.TrainingPath)
                    .Where(sp => sp.UserId == userId && !sp.IsDeleted
                        && sp.Course != null
                        && sp.Course.Type == CourseType.PathCourse
                        && sp.Course.PathId.HasValue)
                    .ToListAsync(cancellationToken);

                if (!userProgresses.Any())
                {
                    _logger.LogInformation("{Method}: No path course progresses found for user {UserId}.", method, userId);
                    return new GeneralResult<List<TrainingPathProgressDetailsDto>>(
                        false,
                        _localization.GetLocalizedString("NoPathProgressFound"),
                        new List<TrainingPathProgressDetailsDto>(),
                        ErrorType.NotFound);
                }

                var groupedByPath = userProgresses
                    .GroupBy(sp => sp.Course!.PathId!.Value)
                    .ToList();

                var result = new List<TrainingPathProgressDetailsDto>();

                foreach (var group in groupedByPath)
                {
                    var firstProgress = group.FirstOrDefault();
                    if (firstProgress == null)
                        continue;
                    var course = firstProgress.Course;
                    var path = course?.TrainingPath;

                    var totalCourses = group.Count();
                    var completedCourses = group.Count(p => p.IsCompleted);
                    var percentage = totalCourses > 0 ? (double)completedCourses / totalCourses * 100 : 0;
                    var totalTime = group.Sum(p => p.TotalTimeSpent.Ticks);
                    var completedGroup = group.Where(p => p.IsCompleted).ToList();
                    var lastCompletedAt = completedGroup.Any()
                        ? completedGroup.Max(p => p.UpdatedAt ?? p.CreatedAt)
                        : null;

                    if (path == null)
                    {
                        _logger.LogWarning("{Method}: Course {CourseId} has null TrainingPath.", method, course?.Id);
                        continue;
                    }

                    result.Add(new TrainingPathProgressDetailsDto
                    {
                        PathId = path.Id,
                        PathName = path.Name ?? string.Empty,
                        CertificateExists = path.CertificateExists,
                        CertificateValidityInDays = path.CertificateValidityInDays,
                        CompletionPercentage = percentage,
                        IsCompleted = completedCourses == totalCourses,
                        CompletedCoursesCount = completedCourses,
                        TotalCoursesCount = totalCourses,
                        TotalTimeSpent = TimeSpan.FromTicks(totalTime),
                        CompletedAt = completedCourses == totalCourses ? lastCompletedAt : null
                    });
                }

                _logger.LogInformation("{Method}: {Count} training paths progress found for user {UserId}.", method, result.Count, userId);
                return new GeneralResult<List<TrainingPathProgressDetailsDto>>(true, _localization.GetLocalizedString("UserTrainingPathsProgressRetrieved"), result, ErrorType.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Method}: Error retrieving training path progress for user {UserId}.", method, userId);
                return new GeneralResult<List<TrainingPathProgressDetailsDto>>(false, _localization.GetLocalizedString("UnexpectedError_GetTrainingPathProgressList"), null, ErrorType.InternalServerError);
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult> StartLessonSessionAsync(string userId, int lessonId, CancellationToken cancellationToken)
        {
            const string method = nameof(StartLessonSessionAsync);
            var now = DateHelper.UtcNow;

            if (string.IsNullOrWhiteSpace(userId) || lessonId <= 0)
            {
                _logger.LogWarning("StudentProgressService - {Method}: Invalid input. userId: {UserId}, lessonId: {LessonId}", method, userId, lessonId);
                return new GeneralResult(false, _localization.GetLocalizedString("IdInvalid"), null, ErrorType.BadRequest);
            }

            try
            {
                using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

                var userExists = await _dbContext.Users
                    .AsNoTracking()
                    .AnyAsync(u => u.Id == userId && !u.IsDeleted && u.IsActive, cancellationToken);

                if (!userExists)
                {
                    _logger.LogInformation("StudentProgressService - {Method}: User {UserId} not found.", method, userId);
                    return new GeneralResult(false, _localization.GetLocalizedString("UserNotFound"), null, ErrorType.NotFound);
                }

                var lesson = await _dbContext.Lessons
                    .Include(l => l.Course)
                    .FirstOrDefaultAsync(l => l.Id == lessonId && !l.IsDeleted, cancellationToken);

                if (lesson == null)
                {
                    _logger.LogInformation("StudentProgressService - {Method}: Lesson {LessonId} not found.", method, lessonId);
                    return new GeneralResult(false, _localization.GetLocalizedString("LessonNotFound"), null, ErrorType.NotFound);
                }

                if (lesson.Course == null ||
                    (lesson.Course.Type != CourseType.PathCourse && lesson.Course.Type != CourseType.SkillsLibCourse))
                {
                    _logger.LogInformation("StudentProgressService - {Method}: Lesson {LessonId} not eligible for progress tracking (CourseType: {CourseType}).",
                        method, lessonId, lesson.Course?.Type);
                    return new GeneralResult(false, _localization.GetLocalizedString("ProgressNotAllowed"), null, ErrorType.BadRequest);
                }

                var activeSessionExists = await _dbContext.LessonSessions
                    .AnyAsync(s => s.UserId == userId && s.LessonId == lessonId && s.EndedAt == null && !s.IsDeleted, cancellationToken);

                if (activeSessionExists)
                {
                    _logger.LogInformation("StudentProgressService - {Method}: Active session already exists for user {UserId} and lesson {LessonId}.",
                        method, userId, lessonId);
                    return new GeneralResult(false, _localization.GetLocalizedString("SessionAlreadyActive"), null, ErrorType.BadRequest);
                }

                var session = new LessonSession
                {
                    UserId = userId,
                    LessonId = lessonId,
                    StartedAt = now,
                };

                await _dbContext.LessonSessions.AddAsync(session, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation("StudentProgressService - {Method}: Lesson session started for user {UserId} and lesson {LessonId}.",
                    method, userId, lessonId);

                return new GeneralResult(true, _localization.GetLocalizedString("SessionStarted"), null, ErrorType.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "StudentProgressService - {Method}: Exception while starting session for user {UserId} and lesson {LessonId}.",
                    method, userId, lessonId);
                return new GeneralResult(false, _localization.GetLocalizedString("UnexpectedErrorStartingSession"), null, ErrorType.InternalServerError);
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult> EndLessonSessionAsync(string userId, int lessonId, CancellationToken cancellationToken)
        {
            const string method = nameof(EndLessonSessionAsync);
            var now = DateHelper.UtcNow;

            if (string.IsNullOrWhiteSpace(userId) || lessonId <= 0)
            {
                _logger.LogWarning("StudentProgressService - {Method}: Invalid input. userId: {UserId}, lessonId: {LessonId}", method, userId, lessonId);
                return new GeneralResult(false, _localization.GetLocalizedString("IdInvalid"), null, ErrorType.BadRequest);
            }

            try
            {
                var userExists = await _dbContext.Users
                    .AsNoTracking()
                    .AnyAsync(u => u.Id == userId && !u.IsDeleted && u.IsActive, cancellationToken);

                if (!userExists)
                {
                    _logger.LogInformation("StudentProgressService - {Method}: User {UserId} not found.", method, userId);
                    return new GeneralResult(false, _localization.GetLocalizedString("UserNotFound"), null, ErrorType.NotFound);
                }

                var lesson = await _dbContext.Lessons
                    .Include(l => l.Course)
                    .FirstOrDefaultAsync(l => l.Id == lessonId && !l.IsDeleted, cancellationToken);

                if (lesson == null)
                {
                    _logger.LogInformation("StudentProgressService - {Method}: Lesson {LessonId} not found.", method, lessonId);
                    return new GeneralResult(false, _localization.GetLocalizedString("LessonNotFound"), null, ErrorType.NotFound);
                }

                if (lesson.Course == null ||
                    (lesson.Course.Type != CourseType.PathCourse && lesson.Course.Type != CourseType.SkillsLibCourse))
                {
                    _logger.LogInformation("StudentProgressService - {Method}: Lesson {LessonId} not eligible for progress tracking (CourseType: {CourseType}).",
                        method, lessonId, lesson.Course?.Type);
                    return new GeneralResult(false, _localization.GetLocalizedString("ProgressNotAllowed"), null, ErrorType.BadRequest);
                }

                var session = await _dbContext.LessonSessions
                    .Where(s => s.UserId == userId && s.LessonId == lessonId && s.EndedAt == null && !s.IsDeleted)
                    .OrderByDescending(s => s.StartedAt)
                    .FirstOrDefaultAsync(cancellationToken);

                if (session == null)
                {
                    _logger.LogInformation("StudentProgressService - {Method}: No active session found for user {UserId} and lesson {LessonId}.",
                        method, userId, lessonId);
                    return new GeneralResult(false, _localization.GetLocalizedString("NoActiveSession"), null, ErrorType.BadRequest);
                }

                session.EndedAt = now;
                var duration = session.EndedAt - session.StartedAt ?? TimeSpan.Zero;

                var lessonProgress = await _dbContext.LessonProgresses
                    .FirstOrDefaultAsync(lp => lp.UserId == userId && lp.LessonId == lessonId && !lp.IsDeleted, cancellationToken);

                var totalDurationMinutes = (lessonProgress?.TimeSpent.TotalMinutes ?? 0) + duration.TotalMinutes;
                var lessonDuration = lesson.DurationInMinutes;
                var shouldMarkCompleted = lessonDuration > 0 && (totalDurationMinutes / lessonDuration) >= 0.9;

                if (lessonProgress == null)
                {
                    lessonProgress = new LessonProgress
                    {
                        UserId = userId,
                        LessonId = lessonId,
                        TimeSpent = duration,
                        IsCompleted = shouldMarkCompleted,
                        CompletedAt = shouldMarkCompleted ? now : null,
                        CreatedAt = now
                    };
                    await _dbContext.LessonProgresses.AddAsync(lessonProgress, cancellationToken);
                }
                else
                {
                    lessonProgress.TimeSpent += duration;
                    lessonProgress.UpdatedAt = now;

                    if (shouldMarkCompleted && !lessonProgress.IsCompleted)
                    {
                        lessonProgress.IsCompleted = true;
                        lessonProgress.CompletedAt = now;
                    }
                }

                // Update course progress
                var courseId = lesson.CourseId;

                if (courseId != 0)
                {
                    var totalTimeTicks = _dbContext.LessonProgresses
                        .Where(lp => lp.UserId == userId && !lp.IsDeleted)
                        .Include(lp => lp.Lesson)
                        .AsEnumerable()
                        .Where(lp => lp.Lesson.CourseId == courseId)
                        .Sum(lp => lp.TimeSpent.Ticks);

                    var courseProgress = await _dbContext.StudentProgresses
                        .FirstOrDefaultAsync(tp => tp.UserId == userId && tp.CourseId == courseId, cancellationToken);

                    if (courseProgress != null)
                    {
                        courseProgress.TotalTimeSpent = TimeSpan.FromTicks(totalTimeTicks);
                        courseProgress.UpdatedAt = now;
                    }
                }

                // Update path progress if course is linked to a TrainingPath
                var pathId = lesson.Course?.PathId;

                if (pathId.HasValue && pathId != 0)
                {
                    var courseIds = await _dbContext.Courses
                        .Where(c => c.PathId == pathId && !c.IsDeleted)
                        .Select(c => c.Id)
                        .ToListAsync(cancellationToken);

                    var pathTotalTicks = _dbContext.StudentProgresses
                        .Where(sp => sp.UserId == userId && sp.CourseId.HasValue && courseIds.Contains(sp.CourseId.Value))
                        .AsEnumerable()
                        .Sum(sp => sp.TotalTimeSpent.Ticks);

                    var pathProgress = await _dbContext.StudentProgresses
                        .FirstOrDefaultAsync(sp => sp.UserId == userId && sp.PathId == pathId, cancellationToken);

                    if (pathProgress != null)
                    {
                        pathProgress.TotalTimeSpent = TimeSpan.FromTicks(pathTotalTicks);
                        pathProgress.UpdatedAt = now;
                    }
                }

                await _dbContext.SaveChangesAsync(cancellationToken);

                await UpdateCourseProgressAsync(courseId, userId, cancellationToken);
                if (pathId.HasValue)
                    await UpdateTrainingPathProgressAsync(pathId.Value, userId, cancellationToken);

                _logger.LogInformation("StudentProgressService - {Method}: Ended lesson session for user {UserId} and lesson {LessonId}.",
                    method, userId, lessonId);

                return new GeneralResult(true, _localization.GetLocalizedString("SessionEnded"), null, ErrorType.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "StudentProgressService - {Method}: Error ending lesson session for user {UserId} and lesson {LessonId}.",
                    method, userId, lessonId);
                return new GeneralResult(false, _localization.GetLocalizedString("UnexpectedErrorEndingSession"), null, ErrorType.InternalServerError);
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult> SyncAllUserProgressForPathAsync(int pathId, CancellationToken cancellationToken)
        {
            const string method = nameof(SyncAllUserProgressForPathAsync);

            try
            {
                if (pathId <= 0)
                {
                    _logger.LogWarning("StudentProgressService - {Method}: Invalid pathId {PathId}.", method, pathId);
                    return new GeneralResult(false, _localization.GetLocalizedString("IdInvalid"), null, ErrorType.BadRequest);
                }

                var path = await _dbContext.TrainingPaths
                    .Include(p => p.Courses)
                    .FirstOrDefaultAsync(p => p.Id == pathId && !p.IsDeleted, cancellationToken);

                if (path == null || path.Courses == null || !path.Courses.Any())
                {
                    _logger.LogInformation("StudentProgressService - {Method}: Path {PathId} not found or has no courses.", method, pathId);
                    return new GeneralResult(false, _localization.GetLocalizedString("PathNotFoundOrEmpty"), null, ErrorType.NotFound);
                }

                var courseIds = path.Courses.Select(c => c.Id).ToList();

                var users = await _dbContext.Subscriptions
                    .Where(p => p.Type == SubscriptionType.TrainingPath &&
                                p.ReferenceId == pathId)
                    .Select(p => p.UserId)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                if (!users.Any())
                {
                    _logger.LogInformation("StudentProgressService - {Method}: No users enrolled in path {PathId}.", method, pathId);
                    return new GeneralResult(false, _localization.GetLocalizedString("NoEnrolledUsers"), null, ErrorType.NotFound);
                }

                foreach (var userId in users)
                {
                    foreach (var courseId in courseIds)
                    {
                        await UpdateCourseProgressAsync(courseId, userId, cancellationToken);
                    }

                    await UpdateTrainingPathProgressAsync(pathId, userId, cancellationToken);
                }

                _logger.LogInformation("StudentProgressService - {Method}: Synced progress for all users in path {PathId}.", method, pathId);
                return new GeneralResult(true, _localization.GetLocalizedString("ProgressSynced"), null, ErrorType.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "StudentProgressService - {Method}: Error syncing progress for path {PathId}.", method, pathId);
                return new GeneralResult(false, _localization.GetLocalizedString("UnexpectedErrorSyncingProgress"), null, ErrorType.InternalServerError);
            }
        }
    }
}
