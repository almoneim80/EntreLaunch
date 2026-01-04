using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Services.TrainingSvc
{
    public class TrainingPathService(PgDbContext dbContext,
        ILogger<TrainingPathService> logger,
        ILocalizationManager localizationManager) : ITrainingPathService
    {
        private readonly PgDbContext _dbContext = dbContext;
        private readonly ILogger<TrainingPathService> _logger = logger;

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<TrainingPathFullDetailsDto>>> GetAllTrainingPathsWithCoursesAsync(PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var query = _dbContext.TrainingPaths
                    .AsNoTracking()
                    .Where(p => !p.IsDeleted)
                    .Include(p => p.Courses!)
                        .ThenInclude(c => c.Lessons!)
                    .Include(p => p.Courses!)
                        .ThenInclude(c => c.Exams!)
                    .Include(p => p.Courses!)
                        .ThenInclude(c => c.CourseTags!)
                            .ThenInclude(ct => ct.Tag)
                    .Include(p => p.Courses!)
                        .ThenInclude(c => c.Lessons!)
                            .ThenInclude(l => l.LessonAttachments);

                var totalCount = await query.CountAsync(cancellationToken);

                if (totalCount == 0)
                {
                    _logger.LogInformation("No training paths found.");
                    return new GeneralResult<PaginatedResult<TrainingPathFullDetailsDto>>(false, localizationManager.GetLocalizedString("NoTrainingPathsFound"), null);
                }

                var pagedData = await query
                    .OrderBy(p => p.Id)
                    .Skip((pagination.Page - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .ToListAsync(cancellationToken);

                var dtoList = pagedData.Select(p => new TrainingPathFullDetailsDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description,
                    CertificateExists = p.CertificateExists,
                    CertificateValidityInDays = p.CertificateValidityInDays,
                    MaxEnrollment = p.MaxEnrollment ?? 0,
                    IsFree = p.IsFree,
                    PathCourses = (p.Courses ?? new List<Course>())
                                .Where(c => !c.IsDeleted)
                                .Select(c => new PathCourseDetailsDto
                                {
                                    Id = c.Id,
                                    Name = c.Name,
                                    Description = c.Description,
                                    Logo = c.Logo,
                                    CertificateExists = c.CertificateExists,
                                    Audience = c.Audience,
                                    Requirements = c.Requirements,
                                    Topics = c.Topics,
                                    Goals = c.Goals,
                                    Outcomes = c.Outcomes,
                                    Lessons = c.Lessons?
                                        .Where(l => !l.IsDeleted)
                                        .Select(l => new LessonData
                                        {
                                            LessonId = l.Id,
                                            Title = l.Name,
                                            OrderIndex = l.Order ?? 0,
                                            Attachments = l.LessonAttachments?
                                                .Where(a => !a.IsDeleted)
                                                .Select(a => new LessonAttachmentData
                                                {
                                                    AttachmentId = a.Id,
                                                    FileName = a.FileName,
                                                    FilePath = a.FileUrl,
                                                    OpenCount = a.OpenCount
                                                }).ToList() ?? new()
                                        }).ToList() ?? new(),
                                    Exams = c.Exams?
                                        .Where(e => !e.IsDeleted)
                                        .Select(e => new ExamData
                                        {
                                            ExamId = e.Id,
                                            ExamName = e.Name,
                                            DurationInMinutes = e.DurationInMinutes ?? 0,
                                            Questions = e.Questions?
                                                .Where(q => !q.IsDeleted)
                                                .Select(q => new QuestionData
                                                {
                                                    QuestionId = q.Id,
                                                    Text = q.Text,
                                                    Answers = q.Answers?
                                                        .Where(a => !a.IsDeleted)
                                                        .Select(a => new AnswerData
                                                        {
                                                            AnswerId = a.Id,
                                                            Text = a.Text,
                                                            IsCorrect = a.IsCorrect
                                                        }).ToList() ?? new()
                                                }).ToList() ?? new()
                                        }).ToList() ?? new(),
                                    Tags = (c.CourseTags ?? new List<CourseTag>())
                                            .Where(ct => !ct.IsDeleted && ct.Tag != null)
                                            .Select(ct => new CourseTagData
                                            {
                                                TagId = ct.Tag!.Id,
                                                Name = ct.Tag!.Name
                                            }).ToList()
                                }).ToList()
                }).ToList();

                var result = new PaginatedResult<TrainingPathFullDetailsDto>
                {
                    Items = dtoList,
                    Page = pagination.Page,
                    PageSize = pagination.PageSize,
                    TotalCount = totalCount
                };

                return new GeneralResult<PaginatedResult<TrainingPathFullDetailsDto>>(true, localizationManager.GetLocalizedString("TrainingPathsRetrieved"), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching training paths with full course details.");
                return new GeneralResult<PaginatedResult<TrainingPathFullDetailsDto>>(false, localizationManager.GetLocalizedString("ErrorFetchingTrainingPaths"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<TrainingPathFullDetailsDto>> GetTrainingPathWithCoursesByIdAsync(int pathId)
        {
            try
            {
                var path = await _dbContext.TrainingPaths
                            .AsNoTracking()
                            .Include(p => p.Courses!)
                                .ThenInclude(c => c.Lessons!)
                            .Include(p => p.Courses!)
                                .ThenInclude(c => c.Exams!)
                            .Include(p => p.Courses!)
                                .ThenInclude(c => c.CourseTags!)
                            .Where(p => p.Id == pathId && !p.IsDeleted)
                            .FirstOrDefaultAsync();

                if (path == null)
                {
                    return new GeneralResult<TrainingPathFullDetailsDto>(false, localizationManager.GetLocalizedString("TrainingPathNotFound"));
                }

                var dto = new TrainingPathFullDetailsDto
                {
                    Id = path.Id,
                    Name = path.Name,
                    Price = path.Price,
                    Description = path.Description,
                    CertificateExists = path.CertificateExists,
                    CertificateValidityInDays = path.CertificateValidityInDays,
                    MaxEnrollment = path.MaxEnrollment ?? 0,
                    IsFree = path.IsFree,

                    PathCourses = (path.Courses ?? Enumerable.Empty<Course>())
                            .Where(c => !c.IsDeleted)
                            .Select(c => new PathCourseDetailsDto
                            {
                                Id = c.Id,
                                Name = c.Name,
                                PathId = path.Id,
                                Description = c.Description,
                                Logo = c.Logo,
                                CertificateExists = c.CertificateExists,
                                Audience = c.Audience,
                                Requirements = c.Requirements,
                                Topics = c.Topics,
                                Goals = c.Goals,
                                Outcomes = c.Outcomes,

                                Lessons = (c.Lessons ?? Enumerable.Empty<Lesson>())
                                    .Where(l => !l.IsDeleted)
                                    .Select(l => new LessonData
                                    {
                                        LessonId = l.Id,
                                        Title = l.Name,
                                        OrderIndex = l.Order ?? 0,
                                        Attachments = (l.LessonAttachments ?? Enumerable.Empty<LessonAttachment>())
                                            .Where(a => !a.IsDeleted)
                                            .Select(a => new LessonAttachmentData
                                            {
                                                AttachmentId = a.Id,
                                                FileName = a.FileName,
                                                FilePath = a.FileUrl,
                                                OpenCount = a.OpenCount
                                            }).ToList()
                                    }).ToList(),

                                Exams = (c.Exams ?? Enumerable.Empty<Exam>())
                                    .Where(e => !e.IsDeleted)
                                    .Select(e => new ExamData
                                    {
                                        ExamId = e.Id,
                                        ExamName = e.Name,
                                        DurationInMinutes = e.DurationInMinutes ?? 0,
                                        Questions = (e.Questions ?? Enumerable.Empty<Question>())
                                            .Where(q => !q.IsDeleted)
                                            .Select(q => new QuestionData
                                            {
                                                QuestionId = q.Id,
                                                Text = q.Text,
                                                Answers = (q.Answers ?? Enumerable.Empty<Answer>())
                                                    .Where(a => !a.IsDeleted)
                                                    .Select(a => new AnswerData
                                                    {
                                                        AnswerId = a.Id,
                                                        Text = a.Text,
                                                        IsCorrect = a.IsCorrect
                                                    }).ToList()
                                            }).ToList()
                                    }).ToList(),

                                Tags = (c.CourseTags ?? Enumerable.Empty<CourseTag>())
                                    .Where(ct => !ct.IsDeleted && ct.Tag != null)
                                    .Select(ct => new CourseTagData
                                    {
                                        TagId = (ct.Tag ?? new Tag()).Id,
                                        Name = (ct.Tag ?? new Tag()).Name
                                    }).ToList()
                            }).ToList()
                };

                return new GeneralResult<TrainingPathFullDetailsDto>(true, localizationManager.GetLocalizedString("TrainingPathRetrieved"), dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching training path with ID {PathId}.", pathId);
                return new GeneralResult<TrainingPathFullDetailsDto>(false, localizationManager.GetLocalizedString("ErrorFetchingTrainingPath"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<(int totalEnrollments, List<TrainingPathSubscripersDto> enrollments)>> GetPathEnrollmentsAsync(int pathId)
        {
            try
            {
                // Checking the existence of the path
                bool courseExists = await _dbContext.TrainingPaths.AnyAsync(c => c.Id == pathId && !c.IsDeleted);
                if (!courseExists)
                {
                    _logger.LogWarning("Training path with ID {pathId} not found.", pathId);
                    return new GeneralResult<(int totalEnrollments, List<TrainingPathSubscripersDto> enrollments)>(false, localizationManager.GetLocalizedString("TrainingPathNotFound"));
                }

                // Fetching enrollment and student data
                var enrollments = await _dbContext.Subscriptions
                    .AsNoTracking()
                    .Where(s =>
                    s.Type == SubscriptionType.TrainingPath &&
                    s.ReferenceId == pathId &&
                    s.Status == SubscriptionStatus.Active &&
                    !s.IsDeleted)
                    .Select(s => new TrainingPathSubscripersDto
                    {
                        Id = s.Id,
                        FirstName = s.User.FirstName,
                        LastName = s.User.LastName,
                        Email = s.User.Email,
                        SubscribedAt = s.CreatedAt ?? DateTimeOffset.UtcNow,
                    })
                    .Distinct().ToListAsync();

                if (!enrollments.Any())
                {
                    _logger.LogWarning("No enrollments found for Training path with ID {pathId}.", pathId);
                    return new GeneralResult<(int totalEnrollments, List<TrainingPathSubscripersDto> enrollments)>(false, localizationManager.GetLocalizedString("NoEnrollmentsFound"));
                }

                // Return the number and list of enrollments
                return new GeneralResult<(int totalEnrollments, List<TrainingPathSubscripersDto> enrollments)>(true, localizationManager.GetLocalizedString("EnrollmentsRetrieved"), (enrollments.Count, enrollments));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving enrollments for path.");
                return new GeneralResult<(int totalEnrollments, List<TrainingPathSubscripersDto> enrollments)>(false, localizationManager.GetLocalizedString("RetrieveEnrollmentsError"));
            }
        }
    }
}
