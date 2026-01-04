using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Services.TrainingSvc
{
    public class OnlineCourseService(
        PgDbContext dbContext,
        ILogger<OnlineCourseService> logger,
        IMapper mapper,
        ILocalizationManager localizationManager) : IOnlineCourseService
    {
        private readonly PgDbContext _dbContext = dbContext;
        private readonly ILogger<OnlineCourseService> _logger = logger;
        private readonly IMapper _mapper = mapper;

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<OnlineCourseDetailsDto>>> GetByStatusAsync(CourseStatus status, PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                if (!Enum.IsDefined(typeof(CourseStatus), status))
                {
                    _logger.LogInformation("Invalid course status provided: {Status}", status);
                    return new GeneralResult<PaginatedResult<OnlineCourseDetailsDto>>(false, localizationManager.GetLocalizedString("InvalidOnlineCourseStatus"), null);
                }

                var courses = await _dbContext.Courses
                    .AsNoTracking()
                    .Where(c => c.Type == CourseType.OnlineCourse && c.Status == status && !c.IsDeleted)
                    .ToListAsync(cancellationToken);

                if (!courses.Any())
                {
                    _logger.LogInformation("No online courses found for status {Status}", status);
                    return new GeneralResult<PaginatedResult<OnlineCourseDetailsDto>>(false, localizationManager.GetLocalizedString("NoCoursesFound"), null);
                }

                var mapped = MapOnlineCourses(courses);

                var paged = mapped
                    .Skip((pagination.Page - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .ToList();

                var paginatedResult = new PaginatedResult<OnlineCourseDetailsDto>
                {
                    Items = paged,
                    TotalCount = mapped.Count,
                    Page = pagination.Page,
                    PageSize = pagination.PageSize
                };

                _logger.LogInformation("Online courses retrieved for status {Status}.", status);
                return new GeneralResult<PaginatedResult<OnlineCourseDetailsDto>>(true, localizationManager.GetLocalizedString("CoursesRetrieved"), paginatedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving online courses for status {Status}", status);
                return new GeneralResult<PaginatedResult<OnlineCourseDetailsDto>>(false, localizationManager.GetLocalizedString("ErrorRetrievingCourses"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> ChangeCourseStatusAsync(int courseId, CourseStatus newStatus)
        {
            try
            {
                if (!Enum.IsDefined(typeof(CourseStatus), newStatus))
                {
                    _logger.LogInformation($"Status {newStatus} is invalid.");
                    return new GeneralResult(false, localizationManager.GetLocalizedString("InvalidOnlineCourseStatus"), null);
                }

                // Fetching the course from the database
                var onlineCourse = await _dbContext.Courses.FirstOrDefaultAsync(
                    c => c.Id == courseId && !c.IsDeleted && c.Type == CourseType.OnlineCourse);
                if (onlineCourse == null)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = localizationManager.GetLocalizedString("CourseNotFoundOrDeleted"),
                    };
                }

                // course Status Change
                onlineCourse.Status = newStatus;
                onlineCourse.UpdatedAt = DateTimeOffset.UtcNow;
                await _dbContext.SaveChangesAsync();
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = localizationManager.GetLocalizedString("CourseStatusChanged"),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing course status for Course ID {courseId} to {newStatus}", courseId, newStatus);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = localizationManager.GetLocalizedString("ErrorChangingCourseStatus"),
                };
            }
        }

        private List<OnlineCourseDetailsDto> MapOnlineCourses(List<Course> onlineCourses)
        {
            var now = DateTimeOffset.UtcNow;

            return onlineCourses.Select(c => new OnlineCourseDetailsDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                CertificateExists = c.CertificateExists,
                StartDate = c.StartDate ?? now,
                EndDate = c.EndDate,
                Price = c.Price ?? 0,
                Discount = c.Discount ?? 0,
                StudyWay = c.StudyWay,
                Status = c.Status ?? CourseStatus.Unknown,
                IsFree = c.IsFree,

                Instructors = c.CourseInstructors?
                    .Where(ci => ci.User != null && !ci.IsDeleted && !ci.User.IsDeleted)
                    .Select(ci => new CourseInstructorData
                    {
                        InstructorId = ci.Id,
                        FirstName = ci.User.FirstName,
                        LastName = ci.User.LastName,
                        Email = ci.User.Email,
                        Avatar = ci.User.AvatarUrl,
                        Description = ci.User.Specialization
                    }).ToList() ?? new(),

                Enrollments = c.StudentProgresses?
                    .Where(sp => sp.User != null && !sp.IsDeleted && !sp.User.IsDeleted)
                    .Select(sp => new CourseEnrollmentData
                    {
                        EnrollmentId = sp.Id,
                        FirstName = sp.User.FirstName,
                        LastName = sp.User.LastName,
                        Email = sp.User.Email,
                        EnrolledAt = sp.CreatedAt ?? now
                    }).ToList() ?? new()
            }).ToList();
        }
    }
}
