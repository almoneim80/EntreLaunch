using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.BlogDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Interfaces.BolgIntf
{
    public interface IBlogService
    {
        /// <summary>
        /// Create a new blog.
        /// </summary>
        Task<GeneralResult> CreateBlogAsync(BlogCreateDto dto, CancellationToken cancellationToken);

        /// <summary>
        /// Update blog status by id.
        /// </summary>
        Task<GeneralResult> ProcessStatus(int blogId, BlogStatus status, CancellationToken cancellationToken);

        /// <summary>
        /// Delete blog by id.
        /// </summary>
        Task<GeneralResult> DeleteBlogAsync(int blogId, CancellationToken cancellationToken);

        /// <summary>
        /// Get all blog.
        /// </summary>
        Task<GeneralResult<PaginatedResult<BlogDetailsDto>>> AllBlogsAsync(PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Get all blog by user id.
        /// </summary>
        Task<GeneralResult<List<BlogDetailsDto>>> UserBlogsAsync(string userId, CancellationToken cancellationToken);

        /// <summary>
        /// Get blog by id.
        /// </summary>
        Task<GeneralResult<BlogDetailsDto>> GetByIdAsync(int blogId, CancellationToken cancellationToken);

        /// <summary>
        /// Get all blogs by status.
        /// </summary>
        Task<GeneralResult<PaginatedResult<BlogDetailsDto>>> BlogsByStatusAsync(BlogStatus status, PaginationParams pagination, CancellationToken cancellationToken);
    }
}
