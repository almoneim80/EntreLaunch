using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.TrainingDtos;
using EntreLaunch.Extensions;

namespace EntreLaunch.Services.TrainingSvc
{
    public class CourseInstructorService(
        PgDbContext dbContext,
        ILogger<CourseInstructorService> logger,
        IRoleService roleService,
        ILocalizationManager localizationManager) : ICourseInstructorService
    {
        private readonly PgDbContext _dbContext = dbContext;
        private readonly ILogger<CourseInstructorService> _logger = logger;
        private readonly ILocalizationManager _localization = localizationManager;

        /// <inheritdoc />
        public async Task<GeneralResult<CourseInstructorDetailsDto>> CreateAsync(CourseInstructorCreateDto dto)
        {
            try
            {
                // check if course exists and is not deleted.
                var course = await _dbContext.Courses
                    .FirstOrDefaultAsync(c => c.Id == dto.CourseId && !c.IsDeleted);

                if (course == null)
                {
                    _logger.LogWarning("CourseInstructorService - CreateAsync: Course not found. ID = {CourseId}", dto.CourseId);
                    return new GeneralResult<CourseInstructorDetailsDto>(
                        false, _localization.GetLocalizedString("CourseNotFound"));
                }

                // check if the course type is online or skill course
                if (course.Type != CourseType.OnlineCourse && course.Type != CourseType.SkillsLibCourse)
                {
                    return new GeneralResult<CourseInstructorDetailsDto>(false, _localization.GetLocalizedString("CourseTypeNotSupported"));
                }

                // check if user exists
                var user = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Id == dto.UserId && !u.IsDeleted);

                if (user == null)
                {
                    _logger.LogWarning("CourseInstructorService - CreateAsync: User not found. ID = {UserId}", dto.UserId);
                    return new GeneralResult<CourseInstructorDetailsDto>(
                        false, _localization.GetLocalizedString("UserNotFound"));
                }

                var isTrainer = await roleService.IsUserInRoleAsync(user.Id, "Trainer");
                if (isTrainer.IsSuccess == false)
                {
                    _logger.LogWarning("CourseInstructorService - CreateAsync: User is not a trainer. ID = {UserId}", dto.UserId);
                    return new GeneralResult<CourseInstructorDetailsDto>(
                        false, _localization.GetLocalizedString("UserIsNotTrainer"));
                }

                // check if the instructor is already assigned to the course
                var alreadyExists = await _dbContext.CourseInstructors
                    .AnyAsync(ci => ci.CourseId == dto.CourseId && ci.UserId == dto.UserId && !ci.IsDeleted);

                if (alreadyExists)
                {
                    _logger.LogInformation("CourseInstructorService - CreateAsync: Instructor already assigned to course. CourseId = {CourseId}, UserId = {UserId}", dto.CourseId, dto.UserId);
                    return new GeneralResult<CourseInstructorDetailsDto>(
                        false, _localization.GetLocalizedString("InstructorAlreadyAssignedToCourse"));
                }

                // create course instructor
                var entity = new CourseInstructor
                {
                    CourseId = dto.CourseId,
                    UserId = dto.UserId,
                    CreatedAt = DateTimeOffset.UtcNow
                };

                await _dbContext.CourseInstructors.AddAsync(entity);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("CourseInstructorService - CreateAsync: Instructor assigned successfully. ID = {Id}", entity.Id);

                return new GeneralResult<CourseInstructorDetailsDto>(
                    true, _localization.GetLocalizedString("InstructorAssignedSuccessfully"), null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CourseInstructorService - CreateAsync: Unexpected error.");
                return new GeneralResult<CourseInstructorDetailsDto>(
                    false, _localization.GetLocalizedString("UnexpectedCreateError"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<CourseInstructorDetailsDto>> UpdateAsync(int id, CourseInstructorUpdateDto dto)
        {
            try
            {
                // check if course instructor exists and is not deleted
                var existing = await _dbContext.CourseInstructors
                    .FirstOrDefaultAsync(ci => ci.Id == id && !ci.IsDeleted);

                if (existing == null)
                {
                    _logger.LogWarning("CourseInstructorService - UpdateAsync: CourseInstructor not found. ID = {Id}", id);
                    return new GeneralResult<CourseInstructorDetailsDto>(
                        false, _localization.GetLocalizedString("CourseInstructorNotFound"));
                }

                // check if course exists and is not deleted.
                var course = await _dbContext.Courses
                    .FirstOrDefaultAsync(c => c.Id == dto.CourseId && !c.IsDeleted);

                if (course == null)
                {
                    return new GeneralResult<CourseInstructorDetailsDto>(
                        false, _localization.GetLocalizedString("CourseNotFound"));
                }

                if (course.Type != CourseType.OnlineCourse && course.Type != CourseType.SkillsLibCourse)
                {
                    return new GeneralResult<CourseInstructorDetailsDto>(
                        false, _localization.GetLocalizedString("CourseTypeNotSupported"));
                }

                // check if user exists and is not deleted.
                var user = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Id == dto.UserId && !u.IsDeleted);

                if (user == null)
                {
                    return new GeneralResult<CourseInstructorDetailsDto>(
                        false, _localization.GetLocalizedString("UserNotFound"));
                }

                // check if user is a trainer.
                var isTrainer = await roleService.IsUserInRoleAsync(user.Id, "Trainer");
                if (isTrainer.IsSuccess == false)
                {
                    _logger.LogWarning("CourseInstructorService - CreateAsync: User is not a trainer. ID = {UserId}", dto.UserId);
                    return new GeneralResult<CourseInstructorDetailsDto>(
                        false, _localization.GetLocalizedString("UserIsNotTrainer"));
                }

                // check if the instructor is already assigned to the courses
                var duplicateExists = await _dbContext.CourseInstructors
                    .AnyAsync(ci => ci.CourseId == dto.CourseId && ci.UserId == dto.UserId && ci.Id != id && !ci.IsDeleted);

                if (duplicateExists)
                {
                    return new GeneralResult<CourseInstructorDetailsDto>(
                        false, _localization.GetLocalizedString("InstructorAlreadyAssignedToCourse"));
                }

                // update course instructor
                existing.CourseId = dto.CourseId;
                existing.UserId = dto.UserId;
                existing.UpdatedAt = dto.UpdatedAt ?? DateTimeOffset.UtcNow;

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("CourseInstructorService - UpdateAsync: Instructor updated successfully. ID = {Id}", existing.Id);

                return new GeneralResult<CourseInstructorDetailsDto>(
                    true, _localization.GetLocalizedString("InstructorUpdatedSuccessfully"), null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CourseInstructorService - UpdateAsync: Unexpected error.");
                return new GeneralResult<CourseInstructorDetailsDto>(
                    false, _localization.GetLocalizedString("UnexpectedUpdateError"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<CourseInstructorDetailsDto>> GetOneAsync(int id)
        {
            try
            {
                var courseInstructor = await _dbContext.CourseInstructors
                    .Where(ci => ci.Id == id && !ci.IsDeleted)
                    .Include(ci => ci.User)
                    .FirstOrDefaultAsync();

                if (courseInstructor == null)
                {
                    _logger.LogWarning("CourseInstructorService - GetOneAsync: CourseInstructor not found. ID = {Id}", id);
                    return new GeneralResult<CourseInstructorDetailsDto>(
                        false, _localization.GetLocalizedString("CourseInstructorNotFound"));
                }

                if (courseInstructor.User == null)
                {
                    _logger.LogError("CourseInstructorService - GetOneAsync: Linked user is null. ID = {Id}", id);
                    return new GeneralResult<CourseInstructorDetailsDto>(
                        false, _localization.GetLocalizedString("UserNotFound"));
                }

                var dto = new CourseInstructorDetailsDto
                {
                    Id = courseInstructor.Id,
                    FirstName = courseInstructor.User.FirstName,
                    LastName = courseInstructor.User.LastName,
                    Email = courseInstructor.User.Email,
                    CountryCode = courseInstructor.User.CountryCode,
                    Specialization = courseInstructor.User.Specialization
                };

                return new GeneralResult<CourseInstructorDetailsDto>(
                    true, _localization.GetLocalizedString("CourseInstructorRetrieved"), dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CourseInstructorService - GetOneAsync: Unexpected error.");
                return new GeneralResult<CourseInstructorDetailsDto>(
                    false, _localization.GetLocalizedString("UnexpectedGetOneError"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<CourseInstructorDetailsDto>>> GetAllAsync(PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var query = _dbContext.CourseInstructors
                    .AsNoTracking()
                    .Include(ci => ci.User)
                    .Where(ci => !ci.IsDeleted && ci.User != null);

                var paged = await query
                    .Select(ci => new CourseInstructorDetailsDto
                    {
                        Id = ci.Id,
                        FirstName = ci.User.FirstName,
                        LastName = ci.User.LastName,
                        Email = ci.User.Email,
                        CountryCode = ci.User.CountryCode,
                        Specialization = ci.User.Specialization
                    })
                    .ToPagedResultAsync(pagination, cancellationToken);

                if (!paged.Items.Any())
                {
                    _logger.LogInformation("CourseInstructorService - GetAllAsync: No instructors found.");
                    return new GeneralResult<PaginatedResult<CourseInstructorDetailsDto>>(
                        false, _localization.GetLocalizedString("NoCourseInstructorsFound"), null);
                }

                return new GeneralResult<PaginatedResult<CourseInstructorDetailsDto>>(
                    true, _localization.GetLocalizedString("CourseInstructorsRetrieved"), paged);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CourseInstructorService - GetAllAsync: Unexpected error.");
                return new GeneralResult<PaginatedResult<CourseInstructorDetailsDto>>(
                    false, _localization.GetLocalizedString("UnexpectedGetAllError"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<CourseInstructorDetailsDto>>> GetInstructorsByCourseIdAsync(int courseId, PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var courseExists = await _dbContext.Courses
                    .AsNoTracking()
                    .AnyAsync(c => c.Id == courseId && !c.IsDeleted, cancellationToken);

                if (!courseExists)
                {
                    _logger.LogWarning("CourseInstructorService - GetInstructorsByCourseIdAsync: Course not found. ID = {CourseId}", courseId);
                    return new GeneralResult<PaginatedResult<CourseInstructorDetailsDto>>(
                        false, _localization.GetLocalizedString("CourseNotFound"), null);
                }

                var query = _dbContext.CourseInstructors
                    .AsNoTracking()
                    .Include(ci => ci.User)
                    .Where(ci => ci.CourseId == courseId && !ci.IsDeleted && ci.User != null);

                var paged = await query
                    .Select(ci => new CourseInstructorDetailsDto
                    {
                        Id = ci.Id,
                        FirstName = ci.User.FirstName,
                        LastName = ci.User.LastName,
                        Email = ci.User.Email,
                        CountryCode = ci.User.CountryCode,
                        Specialization = ci.User.Specialization
                    })
                    .ToPagedResultAsync(pagination, cancellationToken);

                if (!paged.Items.Any())
                {
                    _logger.LogInformation("CourseInstructorService - GetInstructorsByCourseIdAsync: No instructors found.");
                    return new GeneralResult<PaginatedResult<CourseInstructorDetailsDto>>(
                        false, _localization.GetLocalizedString("NoInstructorsFoundForCourse"), null);
                }

                return new GeneralResult<PaginatedResult<CourseInstructorDetailsDto>>(
                    true, _localization.GetLocalizedString("CourseInstructorsRetrieved"), paged);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CourseInstructorService - GetInstructorsByCourseIdAsync: Unexpected error.");
                return new GeneralResult<PaginatedResult<CourseInstructorDetailsDto>>(
                    false, _localization.GetLocalizedString("UnexpectedGetInstructorsByCourseError"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> DeleteAsync(int id)
        {
            try
            {
                var instructor = await _dbContext.CourseInstructors
                    .FirstOrDefaultAsync(ci => ci.Id == id && !ci.IsDeleted);

                if (instructor == null)
                {
                    _logger.LogWarning("CourseInstructorService - DeleteAsync: CourseInstructor not found. ID = {Id}", id);
                    return new GeneralResult(
                        false, _localization.GetLocalizedString("CourseInstructorNotFound"));
                }

                instructor.IsDeleted = true;
                instructor.UpdatedAt = DateTimeOffset.UtcNow;

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("CourseInstructorService - DeleteAsync: Instructor soft-deleted successfully. ID = {Id}", id);

                return new GeneralResult(
                    true, _localization.GetLocalizedString("InstructorDeletedSuccessfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CourseInstructorService - DeleteAsync: Unexpected error.");
                return new GeneralResult(
                    false, _localization.GetLocalizedString("UnexpectedDeleteError"));
            }
        }
    }
}
