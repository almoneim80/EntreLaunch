namespace EntreLaunch.Services.TrainingSvc
{
    public class RatingsService : IRatingsService
    {
        private readonly PgDbContext _dbContext;
        private readonly ILogger<RatingsService> _logger;
        private readonly IMapper _mapper;
        public RatingsService(PgDbContext dbContext, ILogger<RatingsService> logger, IMapper mapper)
        {
            _dbContext = dbContext;
            _logger = logger;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<GeneralResult<bool>> ApproveRatingAsync(int ratingId, string adminNote)
        {
            try
            {
                var rating = await _dbContext.CourseRatings.FirstOrDefaultAsync(r => r.Id == ratingId);
                if (rating == null)
                {
                    _logger.LogWarning("Rating with ID {RatingId} not found.", ratingId);
                    return new GeneralResult<bool>(false, "Rating not found.", false );
                }

                // EntreLaunchdate rating status to Approved
                rating.Status = RatingStatus.Approved;
                rating.ReviewNote = adminNote;

                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Rating with ID {RatingId} approved.", ratingId);
                return new GeneralResult<bool>(true, "Rating approved successfully.", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving rating with ID {RatingId}.", ratingId);
                return new GeneralResult<bool>(false, "Error approving rating.", false);
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
                    return new GeneralResult<bool>(false, "Rating not found.", false);
                }

                // EntreLaunchdate rating status to Rejected
                rating.Status = RatingStatus.Rejected;
                rating.ReviewNote = adminNote;

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Rating with ID {RatingId} rejected.", ratingId);
                return new GeneralResult<bool>(true, "Rating rejected successfully.", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting rating with ID {RatingId}.", ratingId);
                return new GeneralResult<bool>(false, "Error rejecting rating.", false);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<CourseRatingDetailsDto>>> GetRatingsByStatusAsync(RatingStatus status)
        {
            try
            {
                var ratings = await _dbContext.CourseRatings.Where(r => r.Status == status).ToListAsync();
                var result = _mapper.Map<List<CourseRatingDetailsDto>>(ratings);
                return new GeneralResult<List<CourseRatingDetailsDto>>(true, "Ratings retrieved successfully.", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ratings with status {Status}.", status);
                return new GeneralResult<List<CourseRatingDetailsDto>>(false, "Error retrieving ratings.", null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<CourseRatingDetailsDto>>> GetApprovedRatingsAsync()
        {
            try
            {
                var approvedRatings = await _dbContext.CourseRatings
                    .Where(r => r.Status == RatingStatus.Approved && !r.IsDeleted).ToListAsync();

                var result = _mapper.Map<List<CourseRatingDetailsDto>>(approvedRatings);
                return new GeneralResult<List<CourseRatingDetailsDto>>(true, "Approved ratings retrieved successfully.", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving approved ratings.");
                return new GeneralResult<List<CourseRatingDetailsDto>>(false, "Error retrieving approved ratings.", null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<bool>> IsRatingAvailableAsync(int ratingId)
        {
            try
            {
                var isAvailable = await _dbContext.CourseRatings
                    .AnyAsync(r => r.Id == ratingId && !r.IsDeleted && r.Status != RatingStatus.Rejected);

                return new GeneralResult<bool>(true, "Availability checked successfully.", isAvailable);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking availability of rating with ID {RatingId}.", ratingId);
                return new GeneralResult<bool>(false, "Error checking availability.", false);
            }
        }
    }
}
