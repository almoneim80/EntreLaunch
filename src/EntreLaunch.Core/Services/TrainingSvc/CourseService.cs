using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Services.TrainingSvc
{
    public class CourseService(
        PgDbContext dbContext,
        ILogger<CourseService> logger,
        IMapper mapper,
        ILocalizationManager localizationManager) : ICourseService
    {
        private readonly PgDbContext _dbContext = dbContext;
        private readonly ILogger<CourseService> _logger = logger;
        private readonly IMapper _mapper = mapper;

        /// <inheritdoc />
        public async Task<GeneralResult> CreateAsync<TCreateDto>(TCreateDto dto)
        {
            try
            {
                var entity = dto switch
                {
                    OnlineCourseCreateDto online => MapToEntity(online),
                    SkillCourseCreateDto skill => MapToEntity(skill),
                    PathCourseCreateDto path => MapToEntity(path),
                    _ => throw new ArgumentException("Unsupported course creation DTO type")
                };

                await _dbContext.Courses.AddAsync(entity);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult(true, localizationManager.GetLocalizedString("CourseCreated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CourseService.CreateAsync");
                return new GeneralResult(false, localizationManager.GetLocalizedString("CourseCreationFailed"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> UpdateAsync<TUpdateDto>(int id, TUpdateDto dto)
        {
            try
            {
                var course = await _dbContext.Courses.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
                if (course == null)
                    return new GeneralResult(false, localizationManager.GetLocalizedString("CourseNotFound"));

                course.UpdatedAt = DateTimeOffset.UtcNow;
                _mapper.Map(dto, course);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult(true, localizationManager.GetLocalizedString("CourseUpdated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CourseService.UpdateAsync");
                return new GeneralResult(false, localizationManager.GetLocalizedString("CourseUpdateFailed"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<object>>> GetAllAsync(CourseType type, PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var courses = await _dbContext.Courses
                    .AsNoTracking()
                    .Where(c => c.Type == type && !c.IsDeleted)
                    .ToListAsync(cancellationToken);

                if (!courses.Any())
                {
                    _logger.LogInformation("No courses found for type {Type}", type);
                    return new GeneralResult<PaginatedResult<object>>(false, localizationManager.GetLocalizedString("NoCoursesFound"), null);
                }

                var mappedCourses = type switch
                {
                    CourseType.OnlineCourse => courses.Select(MapToOnlineDto).ToList<object>(),
                    CourseType.SkillsLibCourse => courses.Select(MapToSkillDto).ToList<object>(),
                    CourseType.PathCourse => courses.Select(MapToPathDto).ToList<object>(),
                    _ => null
                };

                if (mappedCourses == null)
                {
                    _logger.LogWarning("Invalid course type received: {Type}", type);
                    return new GeneralResult<PaginatedResult<object>>(false, localizationManager.GetLocalizedString("InvalidCourseType"), null);
                }

                var pagedItems = mappedCourses
                    .Skip((pagination.Page - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .ToList();

                var paginatedResult = new PaginatedResult<object>
                {
                    Items = pagedItems,
                    TotalCount = mappedCourses.Count,
                    Page = pagination.Page,
                    PageSize = pagination.PageSize
                };

                _logger.LogInformation("Retrieved {Count} courses for type {Type}", pagedItems.Count, type);
                return new GeneralResult<PaginatedResult<object>>(true, localizationManager.GetLocalizedString("CoursesRetrieved"), paginatedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while getting courses of type {Type}", type);
                return new GeneralResult<PaginatedResult<object>>(false, localizationManager.GetLocalizedString("UnexpectedError"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> GetOneAsync(int id, CourseType type)
        {
            var course = await _dbContext.Courses
                //.Include(...) // العلاقات اللازمة
                .FirstOrDefaultAsync(c => c.Id == id && c.Type == type && !c.IsDeleted);

            if (course == null)
                return new GeneralResult(false, localizationManager.GetLocalizedString("CourseNotFound"));

            object? dto = type switch
            {
                CourseType.OnlineCourse => MapToOnlineDto(course),
                CourseType.SkillsLibCourse => MapToSkillDto(course),
                CourseType.PathCourse => MapToPathDto(course),
                _ => null
            };

            if (dto is null)
                return new GeneralResult(false, localizationManager.GetLocalizedString("InvalidCourseType"));

            return new GeneralResult(true, localizationManager.GetLocalizedString("CourseRetrieved"), dto);
        }

        // <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<CoursesRegisterDto>>> GetUsersByCoursePurchaseAsync(PurchaseItemType itemType, int courseId, PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var query = _dbContext.Purchases
                    .AsNoTracking()
                    .Where(p => p.ItemType == itemType &&
                                p.ReferenceId == courseId &&
                                !p.IsDeleted &&
                                !p.IsRefunded);

                var totalCount = await query.CountAsync(cancellationToken);

                if (totalCount == 0)
                {
                    _logger.LogInformation("No enrollments found for course ID {CourseId}.", courseId);
                    return new GeneralResult<PaginatedResult<CoursesRegisterDto>>(false, localizationManager.GetLocalizedString("NoEnrollmentsFound"), null);
                }

                var paged = await query
                    .OrderByDescending(p => p.CreatedAt)
                    .Skip((pagination.Page - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .Select(p => new CoursesRegisterDto
                    {
                        Id = p.Id,
                        FirstName = p.User.FirstName,
                        LastName = p.User.LastName,
                        Email = p.User.Email,
                        EnrolledAt = p.CreatedAt ?? DateTimeOffset.UtcNow
                    })
                    .ToListAsync(cancellationToken);

                var result = new PaginatedResult<CoursesRegisterDto>
                {
                    Items = paged,
                    TotalCount = totalCount,
                    Page = pagination.Page,
                    PageSize = pagination.PageSize
                };

                return new GeneralResult<PaginatedResult<CoursesRegisterDto>>(true, localizationManager.GetLocalizedString("EnrollmentsRetrieved"), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving enrollments for course ID {CourseId}.", courseId);
                return new GeneralResult<PaginatedResult<CoursesRegisterDto>>(false, localizationManager.GetLocalizedString("RetrieveEnrollmentsError"), null);
            }
        }

        #region HELPER METHODS
        private Course MapToEntity(OnlineCourseCreateDto dto)
        {
            return new Course
            {
                Name = dto.Name,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Price = dto.Price,
                Discount = dto.Discount,
                StudyWay = dto.StudyWay,
                Status = dto.Status,
                CertificateExists = dto.CertificateExists,
                IsFree = dto.IsFree,
                Type = CourseType.OnlineCourse,
                CreatedAt = dto.CreatedAt
            };
        }

        private Course MapToEntity(SkillCourseCreateDto dto)
        {
            return new Course
            {
                Name = dto.Name,
                Description = dto.Description,
                FieldId = dto.FieldId,
                Logo = dto.Logo,
                CertificateExists = dto.CertificateExists,
                IsFree = dto.IsFree,
                Price = dto.Price,
                Discount = dto.Discount,
                Type = CourseType.SkillsLibCourse,
                CreatedAt = dto.CreatedAt,
                Lessons = dto.Lessons?.Select(lesson => new Lesson
                {
                    Name = lesson.Name,
                    VideoUrl = lesson.VideoUrl,
                    OrderIndex = lesson.Order,
                    DurationInMinutes = lesson.DurationInMinutes,
                    Description = lesson.Description,
                    LessonAttachments = lesson.Attachments?.Select(att => new LessonAttachment
                    {
                        FileName = att.FileName,
                        FileUrl = att.FileUrl,
                        CreatedAt = DateTimeOffset.UtcNow
                    }).ToList()
                }).ToList()
            };
        }

        private Course MapToEntity(PathCourseCreateDto dto)
        {
            return new Course
            {
                Name = dto.Name,
                Description = dto.Description,
                PathId = dto.PathId,
                Logo = dto.Logo,
                CertificateExists = dto.CertificateExists,
                Audience = dto.Audience,
                Requirements = dto.Requirements,
                Topics = dto.Topics,
                Goals = dto.Goals,
                Outcomes = dto.Outcomes,
                Type = dto.Type,
                CreatedAt = dto.CreatedAt ?? DateTimeOffset.UtcNow,
                Lessons = dto.Lessons?.Select(lesson => new Lesson
                {
                    Name = lesson.Name,
                    VideoUrl = lesson.VideoUrl,
                    OrderIndex = lesson.Order,
                    DurationInMinutes = lesson.DurationInMinutes,
                    Description = lesson.Description,
                    LessonAttachments = lesson.Attachments?.Select(att => new LessonAttachment
                    {
                        FileName = att.FileName,
                        FileUrl = att.FileUrl,
                        CreatedAt = DateTimeOffset.UtcNow
                    }).ToList()
                }).ToList()
            };
        }

        public OnlineCourseDetailsDto MapToOnlineDto(Course course)
        {
            return new OnlineCourseDetailsDto
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
                CertificateExists = course.CertificateExists,
                CertificateValidityInDays = course.CertificateValidityInDays,
                StartDate = course.StartDate ?? DateTimeOffset.UtcNow,
                EndDate = course.EndDate,
                Price = course.Price ?? 0,
                Discount = course.Discount ?? 0,
                StudyWay = course.StudyWay,
                Status = course.Status ?? CourseStatus.Unknown,
                IsFree = course.IsFree,
                Instructors = course.CourseInstructors?
                    .Where(i => !i.IsDeleted && i.User != null)
                    .Select(i => new CourseInstructorData
                    {
                        InstructorId = i.Id,
                        FirstName = i.User.FirstName,
                        LastName = i.User.LastName,
                        Email = i.User.Email,
                        Avatar = i.User.AvatarUrl,
                        Description = i.User.Specialization
                    }).ToList(),
                Enrollments = course.StudentProgresses?
                    .Where(p => !p.IsDeleted && p.User != null)
                    .Select(p => new CourseEnrollmentData
                    {
                        EnrollmentId = p.Id,
                        FirstName = p.User.FirstName,
                        LastName = p.User.LastName,
                        Email = p.User.Email,
                        EnrolledAt = p.CreatedAt ?? DateTimeOffset.UtcNow
                    }).ToList()
            };
        }

        public SkillCourseDetailsDto MapToSkillDto(Course course)
        {
            return new SkillCourseDetailsDto
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
                FieldName = course.CourseField?.Name,
                Logo = course.Logo,
                CertificateExists = course.CertificateExists,
                CertificateValidityInDays = course.CertificateValidityInDays,
                IsFree = course.IsFree,
                Price = course.Price ?? 0,
                Discount = course.Discount ?? 0,
                Lessons = course.Lessons?
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
                            }).ToList()
                    }).ToList(),
                Exams = course.Exams?
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
                                    }).ToList()
                            }).ToList()
                    }).ToList(),
                Tags = course.CourseTags?
                    .Where(ct => !ct.IsDeleted && ct.Tag != null)
                    .Select(ct => new CourseTagData
                    {
                        TagId = ct.Tag?.Id ?? 0,
                        Name = ct.Tag?.Name ?? "Unknown"
                    }).ToList(),
                Ratings = course.CourseRatings?
                    .Where(r => !r.IsDeleted && r.Status == RatingStatus.Approved)
                    .Select(r => new CourseRatingData
                    {
                        RatingId = r.Id,
                        Rating = r.Rating,
                        ReviewerName = $"{r.User?.FirstName} {r.User?.LastName}".Trim(),
                        ReviewComment = r.Review,
                        CreatedAt = r.CreatedAt ?? DateTimeOffset.UtcNow
                    }).ToList(),
                Instructors = course.CourseInstructors?
                    .Where(i => !i.IsDeleted && i.User != null)
                    .Select(i => new CourseInstructorData
                    {
                        InstructorId = i.Id,
                        FirstName = i.User.FirstName,
                        LastName = i.User.LastName,
                        Email = i.User.Email,
                        Avatar = i.User.AvatarUrl,
                        Description = i.User.Specialization
                    }).ToList(),
                Enrollments = course.StudentProgresses?
                    .Where(p => !p.IsDeleted && p.User != null)
                    .Select(p => new CourseEnrollmentData
                    {
                        EnrollmentId = p.Id,
                        FirstName = p.User.FirstName,
                        LastName = p.User.LastName,
                        Email = p.User.Email,
                        EnrolledAt = p.CreatedAt ?? DateTimeOffset.UtcNow
                    }).ToList()
            };
        }

        public PathCourseDetailsDto MapToPathDto(Course course)
        {
            return new PathCourseDetailsDto
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
                PathId = course.PathId ?? 0,
                Logo = course.Logo,
                CertificateExists = course.CertificateExists,
                CertificateValidityInDays = course.CertificateValidityInDays,
                Audience = course.Audience ?? new(),
                Requirements = course.Requirements ?? new(),
                Topics = course.Topics ?? new(),
                Goals = course.Goals ?? new(),
                Outcomes = course.Outcomes ?? new(),
                Lessons = course.Lessons?
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
                            }).ToList()
                    }).ToList(),
                Exams = course.Exams?
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
                                    }).ToList()
                            }).ToList()
                    }).ToList(),
                Tags = course.CourseTags?
                    .Where(ct => !ct.IsDeleted && ct.Tag != null)
                    .Select(ct => new CourseTagData
                    {
                        TagId = ct.Tag?.Id ?? 0,
                        Name = ct.Tag?.Name ?? "Unknown"
                    }).ToList()
            };
        }
        #endregion
    }
}
