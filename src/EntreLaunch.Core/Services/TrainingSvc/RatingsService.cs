using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.TrainingDtos;
using EntreLaunch.Extensions;

namespace EntreLaunch.Services.TrainingSvc
{
    public class RatingsService(
        PgDbContext dbContext,
        ILogger<RatingsService> logger,
        IMapper mapper,
        ILocalizationManager localizationManager) : IRatingsService
    {
        private readonly PgDbContext _dbContext = dbContext;
        private readonly ILogger<RatingsService> _logger = logger;
        private readonly IMapper _mapper = mapper;

        /// <inheritdoc />
        public async Task<GeneralResult<bool>> ApproveRatingAsync(int ratingId, string adminNote)
        {
            try
            {
                var rating = await _dbContext.CourseRatings.FirstOrDefaultAsync(r => r.Id == ratingId);
                if (rating == null)
                {
                    _logger.LogWarning("Rating with ID {RatingId} not found.", ratingId);
                    return new GeneralResult<bool>(false, localizationManager.GetLocalizedString("RatingNotFound"), false );
                }

                // update rating status to Approved
                rating.Status = RatingStatus.Approved;
                rating.ReviewNote = adminNote;

                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Rating with ID {RatingId} approved.", ratingId);
                return new GeneralResult<bool>(true, localizationManager.GetLocalizedString("RatingApproved"), true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving rating with ID {RatingId}.", ratingId);
                return new GeneralResult<bool>(false, localizationManager.GetLocalizedString("RatingApprovalError"), false);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<bool>> RejectRatingAsync(int ratingId, string adminNote)
        {
            try
            {
                var rating = await _dbContext.CourseRatings.FirstOrDefaultAsync(r => r.Id == ratingId);
                if (rating == null)
                {
                    _logger.LogWarning("Rating with ID {RatingId} not found.", ratingId);
                    return new GeneralResult<bool>(false, localizationManager.GetLocalizedString("RatingNotFound"), false);
                }

                // update rating status to Rejected
                rating.Status = RatingStatus.Rejected;
                rating.ReviewNote = adminNote;

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Rating with ID {RatingId} rejected.", ratingId);
                return new GeneralResult<bool>(true, localizationManager.GetLocalizedString("RatingRejected"), true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting rating with ID {RatingId}.", ratingId);
                return new GeneralResult<bool>(false, localizationManager.GetLocalizedString("RatingRejectionError"), false);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<CourseRatingDetailsDto>>> GetRatingsByStatusAsync(RatingStatus status)
        {
            try
            {
                var ratings = await _dbContext.CourseRatings.Where(r => r.Status == status).ToListAsync();
                var result = _mapper.Map<List<CourseRatingDetailsDto>>(ratings);
                return new GeneralResult<List<CourseRatingDetailsDto>>(true, localizationManager.GetLocalizedString("RatingsRetrieved"), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ratings with status {Status}.", status);
                return new GeneralResult<List<CourseRatingDetailsDto>>(false, localizationManager.GetLocalizedString("RatingsRetrievalError"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<CourseRatingDetailsDto>>> GetApprovedRatingsAsync(PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var query = _dbContext.CourseRatings
                    .AsNoTracking()
                    .Include(r => r.Course)
                    .Include(r => r.User)
                    .Where(r => r.Status == RatingStatus.Approved && !r.IsDeleted);

                var paged = await query
                    .Select(r => new CourseRatingDetailsDto
                    {
                        Id = r.Id,
                        CourseName = r.Course.Name,
                        RatingValue = r.Rating,
                        Review = r.Review,
                        ReviewerName = r.User.FirstName + " " + r.User.LastName,
                        CreatedAt = r.CreatedAt
                    }).ToPagedResultAsync(pagination, cancellationToken);

                if (!paged.Items.Any())
                {
                    _logger.LogInformation("No approved ratings found.");
                    return new GeneralResult<PaginatedResult<CourseRatingDetailsDto>>(false, localizationManager.GetLocalizedString("NoApprovedRatingsFound"), null);
                }

                return new GeneralResult<PaginatedResult<CourseRatingDetailsDto>>(true, localizationManager.GetLocalizedString("ApprovedRatingsRetrieved"), paged);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving approved ratings.");
                return new GeneralResult<PaginatedResult<CourseRatingDetailsDto>>(false, localizationManager.GetLocalizedString("ApprovedRatingsRetrievalError"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<bool>> IsRatingAvailableAsync(int ratingId)
        {
            try
            {
                var isAvailable = await _dbContext.CourseRatings
                    .AnyAsync(r => r.Id == ratingId && !r.IsDeleted && r.Status != RatingStatus.Rejected);

                return new GeneralResult<bool>(true, localizationManager.GetLocalizedString("AvailabilityChecked"), isAvailable);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking availability of rating with ID {RatingId}.", ratingId);
                return new GeneralResult<bool>(false, localizationManager.GetLocalizedString("AvailabilityCheckError"), false);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<(double AverageRating, int RatingCount)>> GetCourseRatingStatisticsAsync(int courseId)
        {
            try
            {
                // Fetch all active (not deleted) evaluations for this course
                var ratings = await _dbContext.CourseRatings
                    .Where(r => r.CourseId == courseId && !r.IsDeleted && r.Status == RatingStatus.Approved).ToListAsync();
                if (!ratings.Any())
                {
                    // There is no rating
                    return new GeneralResult<(double AverageRating, int RatingCount)>(false, localizationManager.GetLocalizedString("NoCourseRatings"), (0, 0));
                }

                double avg = ratings.Average(r => r.Rating);
                int count = ratings.Count;
                return new GeneralResult<(double AverageRating, int RatingCount)>(true, localizationManager.GetLocalizedString("RatingStatisticsRetrieved"), (avg, count));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting rating statistics for Course {CourseId}.", courseId);
                return new GeneralResult<(double AverageRating, int RatingCount)>(false, localizationManager.GetLocalizedString("ErrorGettingRatingStatistics"), (0, 0));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<CourseRatingDetailsDto>>> GetAllRatingsForCourseAsync(int courseId, PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var query = _dbContext.CourseRatings
                    .AsNoTracking()
                    .Where(r => r.CourseId == courseId && !r.IsDeleted && r.Status == RatingStatus.Approved)
                    .OrderByDescending(r => r.CreatedAt);

                var paged = await query
                    .Select(r => new CourseRatingDetailsDto
                    {
                        Id = r.Id,
                        CourseName = r.Course.Name,
                        RatingValue = r.Rating,
                        Review = r.Review,
                        ReviewerName = r.User.FirstName + " " + r.User.LastName,
                        CreatedAt = r.CreatedAt
                    }).ToPagedResultAsync(pagination, cancellationToken);

                if (!paged.Items.Any())
                {
                    return new GeneralResult<PaginatedResult<CourseRatingDetailsDto>>(
                        false, localizationManager.GetLocalizedString("NoCourseRatings"), null);
                }

                return new GeneralResult<PaginatedResult<CourseRatingDetailsDto>>(
                    true, localizationManager.GetLocalizedString("RatingStatisticsRetrieved"), paged);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ratings for Course {CourseId}.", courseId);
                return new GeneralResult<PaginatedResult<CourseRatingDetailsDto>>(
                    false, localizationManager.GetLocalizedString("ErrorRetrievingRatings"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<bool>> CanStudentRateCourseAsync(string studentId, int courseId)
        {
            try
            {
                // checking the existence of the student
                var studentExists = await _dbContext.Users.AnyAsync(u => u.Id == studentId && !u.IsDeleted);
                if (!studentExists)
                {
                    _logger.LogWarning("Student with ID {StudentId} not found or is deleted.", studentId);
                    return new GeneralResult<bool>(false, localizationManager.GetLocalizedString("StudentNotFound"));
                }

                // checking the existence of the course
                var courseExists = await _dbContext.Courses.AnyAsync(c => c.Id == courseId && !c.IsDeleted);
                if (!courseExists)
                {
                    _logger.LogWarning("Course with ID {CourseId} not found or is deleted.", courseId);
                    return new GeneralResult<bool>(false, localizationManager.GetLocalizedString("NoCoursesFound"));
                }

                // checking if the student has rated the course
                var hasRated = await _dbContext.CourseRatings
                    .AnyAsync(r => r.UserId == studentId && r.CourseId == courseId && !r.IsDeleted);

                return new GeneralResult<bool>(true, localizationManager.GetLocalizedString("CheckCompleted"), !hasRated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking if student {StudentId} has rated course {CourseId}.", studentId, courseId);
                return new GeneralResult<bool>(false, localizationManager.GetLocalizedString("ErrorCheckingStudentRating"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<CourseRatingSummaryDto>> GetCourseRatingSummaryAsync(int courseId)
        {
            try
            {
                // Check for the presence of the course
                var courseExists = await _dbContext.Courses.AnyAsync(c => c.Id == courseId && !c.IsDeleted);
                if (!courseExists)
                {
                    _logger.LogWarning("Course with ID {CourseId} not found or is deleted.", courseId);
                    return new GeneralResult<CourseRatingSummaryDto>(false, localizationManager.GetLocalizedString("NoCoursesFound"));
                }

                // Calculate the average and number of ratings
                var ratingSummary = await _dbContext.CourseRatings.Where(r => r.CourseId == courseId && !r.IsDeleted && r.Status == RatingStatus.Approved)
                    .GroupBy(r => r.CourseId).Select(g => new CourseRatingSummaryDto
                    {
                        CourseId = g.Key,
                        AverageRating = g.Average(r => r.Rating),
                        TotalRatings = g.Count()
                    })
                    .FirstOrDefaultAsync();

                // If there are no evaluations, return a default result
                if (ratingSummary == null)
                {
                    ratingSummary = new CourseRatingSummaryDto
                    {
                        CourseId = courseId,
                        AverageRating = 0,
                        TotalRatings = 0
                    };
                }

                return new GeneralResult<CourseRatingSummaryDto>(true, localizationManager.GetLocalizedString("RatingSummaryRetrieved"), ratingSummary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the rating summary for course ID {CourseId}.", courseId);
                return new GeneralResult<CourseRatingSummaryDto>(false, localizationManager.GetLocalizedString("ErrorFetchingRatingSummary"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<CourseRatingsDto>>> GetRatingsByInstructorAsync(string instructorId)
        {
            try
            {
                // التحقق من وجود المدرب
                var instructorExists = await _dbContext.Users
                    .AnyAsync(u => u.Id == instructorId && !u.IsDeleted);
                if (!instructorExists)
                {
                    return new GeneralResult<List<CourseRatingsDto>>(false, localizationManager.GetLocalizedString("InstructorNotFound"));
                }

                // Fetch assessments associated with courses taught by the instructor
                var ratings = await _dbContext.CourseRatings.AsNoTracking()
                    .Where(cr => cr.Course.CourseInstructors!.Any(ci => ci.UserId == instructorId && !ci.IsDeleted)
                    && !cr.IsDeleted && cr.Status == RatingStatus.Approved)
                    .Select(cr => new CourseRatingsDto
                    {
                        Id = cr.Id,
                        CourseId = cr.CourseId,
                        CourseName = cr.Course.Name,
                        ReviewerName = cr.User.FirstName + " " + cr.User.LastName,
                        Rating = cr.Rating,
                        Comment = cr.Review,
                        CreatedAt = cr.CreatedAt ?? DateTimeOffset.UtcNow,
                    })
                    .ToListAsync();

                // If no ratings are found
                if (!ratings.Any())
                {
                    _logger.LogInformation("No ratings found for instructor with ID {InstructorId}.", instructorId);
                    return new GeneralResult<List<CourseRatingsDto>>(true, localizationManager.GetLocalizedString("NoRatingsFound"));
                }

                return new GeneralResult<List<CourseRatingsDto>>(true, localizationManager.GetLocalizedString("RatingsRetrieved"), ratings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving ratings for instructor with ID {InstructorId}.", instructorId);
                return new GeneralResult<List<CourseRatingsDto>>(false, localizationManager.GetLocalizedString("ErrorFetchingInstructorRatings"));
            }
        }
    }
}
