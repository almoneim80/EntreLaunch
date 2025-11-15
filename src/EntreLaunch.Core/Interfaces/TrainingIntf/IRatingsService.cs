namespace EntreLaunch.Interfaces.TrainingIntf
{
    public interface IRatingsService
    {
        /// <summary>
        /// Approve rating.
        /// </summary>
        Task<GeneralResult<bool>> ApproveRatingAsync(int ratingId, string adminNote);

        /// <summary>
        /// Reject rating.
        /// </summary>
        Task<GeneralResult<bool>> RejectRatingAsync(int ratingId, string adminNote);

        /// <summary>
        /// Get ratings by status.
        /// </summary>
        Task<GeneralResult<List<CourseRatingDetailsDto>>> GetRatingsByStatusAsync(RatingStatus status);

        /// <summary>
        /// Get all approved ratings.
        /// </summary>
        Task<GeneralResult<List<CourseRatingDetailsDto>>> GetApprovedRatingsAsync();

        /// <summary>
        /// Checks if a rating is available.
        /// </summary>
        Task<GeneralResult<bool>> IsRatingAvailableAsync(int ratingId);
    }
}
