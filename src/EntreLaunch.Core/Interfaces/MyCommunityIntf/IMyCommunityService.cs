using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.MyCommunityDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Interfaces.MyCommunityIntf
{
    public interface IMyCommunityService
    {
        #region Create Operations

        /// <summary>
        /// Create a text-only post with no media.
        /// </summary>
        Task<GeneralResult> CreateTextPostAsync(TextPostCreateDto dto);

        /// <summary>
        /// Create a post with media (photos/videos).
        /// </summary>
        Task<GeneralResult> CreatePostWithMediaAsync(PostWithMediaCreateDto dto);

        /// <summary>
        /// Like a specific post.
        /// </summary>
        Task<GeneralResult> CreateLikeAsync(LikeCreateDto dto);

        /// <summary>
        /// Create a Report on a post.
        /// </summary>
        Task<GeneralResult> CreateReportAsync(ReportCreateDto dto);

        #endregion

        #region Read Operations (Get / Show)

        /// <summary>
        /// Fetch all posts (with media, comments and likes).
        /// </summary>
        Task<GeneralResult<PaginatedResult<PostDetailsDto>>> GetAllPostsAsync(PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// View specific post information: (post + media + comments + likes).
        /// </summary>
        Task<GeneralResult> GetPostByIdAsync(int postId);

        /// <summary>
        /// Returns the number of likes on a given post.
        /// </summary>
        Task<GeneralResult> GetPostLikeCountAsync(int postId);

        /// <summary>
        /// Returns all communications on a specific post.
        /// </summary>
        Task<GeneralResult> GetPostReportsAsync(int postId);

        /// <summary>
        /// Showing all posts with the status Pending.
        /// </summary>
        Task<GeneralResult<PaginatedResult<PostDetailsDto>>> GetPendingPostsAsync(PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Showing all posts with the status Accepted.
        /// </summary>
        Task<GeneralResult<PaginatedResult<PostDetailsDto>>> GetAcceptedPostsAsync(PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Showing all posts with the status Rejected.
        /// </summary>
        Task<GeneralResult<PaginatedResult<PostDetailsDto>>> GetRejectedPostsAsync(PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Displays all communications with the status Pending.
        /// </summary>
        Task<GeneralResult> GetPendingReportsAsync();

        /// <summary>
        /// Displays all communications with the status Accepted.
        /// </summary>
        Task<GeneralResult> GetAcceptedReportsAsync();

        /// <summary>
        /// Displays all communications with the status Rejected.
        /// </summary>
        Task<GeneralResult> GetRejectedReportsAsync();

        #endregion

        #region Processing Status Operations

        /// <summary>
        /// Processing the status of a particular post (accept or reject).
        /// </summary>
        Task<GeneralResult> ProcessPostStatusAsync(int postId, RequestStatus status);

        /// <summary>
        /// Processing the status of a particular Report (Accept or Reject).
        /// </summary>
        Task<GeneralResult> ProcessReportStatusAsync(int reportId, RequestStatus status);

        #endregion

        #region Delete Operations

        /// <summary>
        /// Deleting a post (post + media + comments + likes) by the owner (User).
        /// </summary>
        Task<GeneralResult> DeletePostAsync(int postId, string userId);

        /// <summary>
        /// Deleting a specific comment by the owner (User).
        /// </summary>
        Task<GeneralResult> DeleteCommentAsync(int commentId, string userId);

        /// <summary>
        /// Delete an argument assigned to a post by the owner (User).
        /// </summary>
        Task<GeneralResult> DeleteMediaAsync(int mediaId, string userId);

        /// <summary>
        /// Deleting a post's Report.
        /// </summary>
        Task<GeneralResult> DeletePostReportAsync(int reportId);

        /// <summary>
        /// Deleting a report for a comment.
        /// </summary>
        Task<GeneralResult> DeleteCommentReportAsync(int reportId);

        #endregion
    }
}
