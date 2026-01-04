using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.ExamDtos;
using EntreLaunch.DTOs.LessonDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Services.TrainingSvc
{
    public class LessonService(PgDbContext dbContext,
        ILogger<LessonService> logger,
        IMapper mapper,
        ILocalizationManager localization) : ILessonService
    {
        private readonly PgDbContext _dbContext = dbContext;
        private readonly ILogger<LessonService> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly ILocalizationManager _localization = localization;

        /// <inheritdoc />
        public async Task<GeneralResult<bool>> ReorderLessonsAsync(int courseId, List<LessonReorderDto> newOrderList)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Make sure the list is not empty
                if (newOrderList == null || !newOrderList.Any())
                {
                    _logger.LogWarning("No lesson order data provided for course ID {CourseId}.", courseId);
                    return new GeneralResult<bool>(false, _localization.GetLocalizedString("NoLessonOrderData"), false);
                }

                // Checking for negative or zero ordering
                if (newOrderList.Exists(x => x.OrderIndex <= 0))
                {
                    _logger.LogWarning("Detected negative or zero OrderIndex in the reorder list for course ID {CourseId}.", courseId);
                    return new GeneralResult<bool>(false, _localization.GetLocalizedString("InvalidOrderIndex"), false);
                }

                // OrderIndex duplicate check (to prevent having two lessons with the same order)
                var orderIndices = newOrderList.Select(x => x.OrderIndex).ToList();
                if (orderIndices.Distinct().Count() != orderIndices.Count)
                {
                    _logger.LogWarning("Duplicate OrderIndex detected in the reorder list for course ID {CourseId}.", courseId);
                    return new GeneralResult<bool>(false, _localization.GetLocalizedString("DuplicateOrderIndex"), false);
                }

                // Collect the identifiers of the lessons to be arranged
                var lessonIds = newOrderList.Select(x => x.LessonId).Distinct().ToList();

                // Fetching lessons from the database
                var lessons = await _dbContext.Lessons.Where(l => lessonIds.Contains(l.Id) && l.CourseId == courseId
                                && !l.IsDeleted).ToListAsync();

                // Checking that no lessons are missing in DB compared to the submitted list
                if (lessons.Count != lessonIds.Count)
                {
                    // May mean that some LessonId does not exist or does not belong to the course
                    _logger.LogWarning("Some lessons in the reorder list were not found or do not belong to course {CourseId}.", courseId);
                    return new GeneralResult<bool>(false, _localization.GetLocalizedString("LessonMismatch"), false);
                }

                // Update the OrderIndex field for each lesson according to the incoming values
                foreach (var lesson in lessons)
                {
                    var matchingDto = newOrderList.FirstOrDefault(x => x.LessonId == lesson.Id);
                    if (matchingDto != null)
                    {
                        // You can add a check on the value itself: Is it negative, does it repeat itself with another lesson, etc.
                        lesson.OrderIndex = matchingDto.OrderIndex;
                    }
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return new GeneralResult<bool>(true, _localization.GetLocalizedString("LessonsReordered"), true);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error reordering lessons for course ID {CourseId}.", courseId);
                return new GeneralResult<bool>(false, _localization.GetLocalizedString("ErrorReorderingLessons"), false);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<LessonFullDetailsDto>>> GetLessonsByCourseIdAsync(int courseId)
        {
            try
            {
                var allLessons = await GetRawLessonsAsync();
                var filteredLessons = allLessons.Where(l => l.lessonCourse.Id == courseId).ToList();

                if (!filteredLessons.Any())
                {
                    _logger.LogInformation("No lessons found for CourseId {CourseId}.", courseId);
                    return new GeneralResult<List<LessonFullDetailsDto>>(false, _localization.GetLocalizedString("NoLessonsFound"), null);
                }

                var result = _mapper.Map<List<LessonFullDetailsDto>>(filteredLessons);
                return new GeneralResult<List<LessonFullDetailsDto>>(true, _localization.GetLocalizedString("LessonsRetrieved"), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching lessons for CourseId {CourseId}.", courseId);
                return new GeneralResult<List<LessonFullDetailsDto>>(false, _localization.GetLocalizedString("ErrorFetchingLessons"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<LessonFullDetailsDto>>> GetAllLessonsAsync(PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var allLessons = await GetRawLessonsAsync();

                if (!allLessons.Any())
                {
                    _logger.LogInformation("No lessons found.");
                    return new GeneralResult<PaginatedResult<LessonFullDetailsDto>>(
                        false, _localization.GetLocalizedString("NoLessonsFound"), null);
                }

                var pagedItems = allLessons
                    .Skip(pagination.PageSize * (pagination.Page - 1))
                    .Take(pagination.PageSize)
                    .ToList();

                var paginatedResult = new PaginatedResult<LessonFullDetailsDto>
                {
                    Items = pagedItems,
                    TotalCount = allLessons.Count,
                    PageSize = pagination.PageSize,
                    Page = pagination.Page
                };

                return new GeneralResult<PaginatedResult<LessonFullDetailsDto>>(
                    true,
                    _localization.GetLocalizedString("LessonsRetrieved"),
                    paginatedResult
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading all lessons.");
                return new GeneralResult<PaginatedResult<LessonFullDetailsDto>>(
                    false, _localization.GetLocalizedString("ErrorFetchingLessons"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<LessonFullDetailsDto>> GetLessonByIdAsync(int lessonId)
        {
            try
            {
                var allLessons = await GetRawLessonsAsync();
                var lesson = allLessons.FirstOrDefault(l => l.Id == lessonId);

                if (lesson == null)
                {
                    _logger.LogInformation("Lesson not found with Id {LessonId}", lessonId);
                    return new GeneralResult<LessonFullDetailsDto>(false, _localization.GetLocalizedString("LessonNotFound"));
                }

                return new GeneralResult<LessonFullDetailsDto>(true, _localization.GetLocalizedString("LessonRetrieved"), lesson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching lesson with Id {LessonId}.", lessonId);
                return new GeneralResult<LessonFullDetailsDto>(false, _localization.GetLocalizedString("ErrorFetchingLesson"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> UpdateLessonAsync(int lessonId, LessonUpdateDto dto)
        {
            try
            {
                var lesson = await _dbContext.Lessons.FirstOrDefaultAsync(l => l.Id == lessonId && !l.IsDeleted);
                if (lesson == null)
                {
                    return new GeneralResult(false, _localization.GetLocalizedString("LessonNotFound"));
                }

                lesson.Name = dto.Name ?? lesson.Name;
                lesson.VideoUrl = dto.VideoUrl ?? lesson.VideoUrl;
                lesson.DurationInMinutes = dto.DurationInMinutes ?? lesson.DurationInMinutes;
                lesson.Description = dto.Description ?? lesson.Description;
                lesson.OrderIndex = dto.Order ?? lesson.OrderIndex;
                lesson.CourseId = dto.CourseId;
                lesson.UpdatedAt = dto.UpdatedAt ?? DateTimeOffset.UtcNow;

                await _dbContext.SaveChangesAsync();

                return new GeneralResult(true, _localization.GetLocalizedString("LessonUpdatedSuccessfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating lesson with Id {LessonId}", lessonId);
                return new GeneralResult(false, _localization.GetLocalizedString("ErrorUpdatingLesson"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> CreateLessonAsync(LessonCreateDto dto)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var lesson = new Lesson
                {
                    Name = dto.Name,
                    VideoUrl = dto.VideoUrl,
                    OrderIndex = dto.Order,
                    DurationInMinutes = dto.DurationInMinutes,
                    Description = dto.Description,
                    CourseId = dto.CourseId,
                    CreatedAt = dto.CreatedAt ?? DateTimeOffset.UtcNow
                };

                _dbContext.Lessons.Add(lesson);
                await _dbContext.SaveChangesAsync();

                foreach (var attachment in dto.Attachments ?? new())
                {
                    var newAttachment = new LessonAttachment
                    {
                        LessonId = lesson.Id,
                        FileName = attachment.FileName,
                        FileUrl = attachment.FileUrl,
                        CreatedAt = attachment.CreatedAt ?? DateTimeOffset.UtcNow
                    };

                    _dbContext.LessonAttachments.Add(newAttachment);
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return new GeneralResult(true, _localization.GetLocalizedString("LessonCreatedSuccessfully"));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating new lesson.");
                return new GeneralResult(false, _localization.GetLocalizedString("ErrorCreatingLesson"));
            }
        }

        // help methods

        /// <summary>
        /// Retrieves all lessons from the database.
        /// </summary>
        private async Task<List<LessonFullDetailsDto>> GetRawLessonsAsync()
        {
            var lessons = await _dbContext.Lessons
                .AsNoTracking()
                .Include(l => l.LessonAttachments)
                .Include(l => l.Course)
                    .ThenInclude(c => c.CourseField)
                .Include(l => l.Course)
                    .ThenInclude(c => c.TrainingPath)
                .Include(l => l.Exams!
                    .Where(e => !e.IsDeleted && e.ParentEntityType == ExamParentEntityType.Lesson))
                    .ThenInclude(e => e.Questions!)
                        .ThenInclude(q => q.Answers!)
                .Where(l => !l.IsDeleted)
                .ToListAsync();

            var dtoList = lessons.Select(l => new LessonFullDetailsDto
            {
                Id = l.Id,
                Name = l.Name ?? "",
                VideoUrl = l.VideoUrl ?? "",
                DurationInMinutes = l.DurationInMinutes ?? 0,
                Description = l.Description ?? "",
                OldOrder = l.OrderIndex ?? 0,
                NewOrder = l.OrderIndex ?? 0,

                Attachments = l.LessonAttachments?
                    .Where(a => !a.IsDeleted)
                    .Select(a => new LessonAttachmentDto
                    {
                        Id = a.Id,
                        FileUrl = a.FileUrl ?? ""
                    }).ToList() ?? new(),

                lessonCourse = l.Course != null ? new LessonCourseDto
                {
                    Id = l.Course.Id,
                    Name = l.Course.Name,
                    Description = l.Course.Description,

                    Field = l.Course.CourseField != null ? new CourseFieldDto
                    {
                        Name = l.Course.CourseField.Name,
                        Description = l.Course.CourseField.Description
                    }
                    : null,

                    Path = l.Course.TrainingPath != null ? new CoursePathDto
                    {
                        Id = l.Course.TrainingPath.Id,
                        Name = l.Course.TrainingPath.Name,
                        Description = l.Course.TrainingPath.Description,
                        Price = l.Course.TrainingPath.Price,
                        CertificateExists = l.Course.TrainingPath.CertificateExists,
                        MaxEnrollment = l.Course.TrainingPath.MaxEnrollment ?? 0,
                        IsFree = l.Course.TrainingPath.IsFree
                    }
                    : null,

                    Price = l.Course.Price ?? 0,
                    Discount = l.Course.Discount ?? 0,
                    StudyWay = l.Course.StudyWay,
                    DurationInDays = l.Course.DurationInDays ?? 0,
                    StartDate = l.Course.StartDate,
                    EndDate = l.Course.EndDate,
                    CertificateExists = l.Course.CertificateExists,
                    IsFree = l.Course.IsFree,
                    Logo = l.Course.Logo,
                    Status = l.Course.Status,
                    Type = l.Course.Type,
                    MaxEnrollment = l.Course.InstructorCount,
                    Audience = l.Course.Audience,
                    Requirements = l.Course.Requirements,
                    Topics = l.Course.Topics,
                    Goals = l.Course.Goals,
                    Outcomes = l.Course.Outcomes
                }
                : null,

                LessonExam = l.Exams?
                    .Where(e => !e.IsDeleted)
                    .Select(e => new ExamFullDetailsDto
                    {
                        ExamId = e.Id,
                        Name = e.Name ?? "",
                        Type = e.ParentEntityType.ToString(),
                        Description = e.Description ?? "",
                        MinMark = e.MinMark ?? 0,
                        MaxMark = e.MaxMark ?? 0,
                        DurationInMinutes = e.DurationInMinutes ?? 0,
                        MaxAttempts = e.MaxAttempts ?? 0,
                        Status = e.Status,
                        ParentEntityName = l.Name ?? "",
                        Questions = (e.Questions ?? new List<Question>())
                            .Where(q => !q.IsDeleted)
                            .Select(q => new QuestionDetailsData
                            {
                                QuestionId = q.Id,
                                Text = q.Text ?? "",
                                Mark = q.Mark ?? 0,
                                Answers = (q.Answers ?? new List<Answer>())
                                    .Where(a => !a.IsDeleted)
                                    .Select(a => new AnswerDetailsData
                                    {
                                        AnswerId = a.Id,
                                        Text = a.Text ?? "",
                                        IsCorrect = a.IsCorrect
                                    }).ToList()
                            }).ToList()
                    }).FirstOrDefault()
            }).ToList();

            return dtoList;
        }
    }
}
