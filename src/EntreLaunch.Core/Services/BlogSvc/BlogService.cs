using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.BlogDtos;
using EntreLaunch.DTOs.TrainingDtos;
using EntreLaunch.Extensions;
using EntreLaunch.Interfaces.BolgIntf;
namespace EntreLaunch.Services.BlogSvc
{
    public class BlogService(
        PgDbContext dbContext,
        ILogger<BlogService> logger,
        IMapper mapper,
        ILocalizationManager localizationManager) : IBlogService
    {
        private readonly PgDbContext _dbContext = dbContext;
        private readonly ILogger<BlogService> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly ILocalizationManager _localizationManager = localizationManager;

        /// <inheritdoc />
        public async Task<GeneralResult> CreateBlogAsync(BlogCreateDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var blog = _mapper.Map<Blog>(dto);
                await _dbContext.Blogs.AddAsync(blog);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return new GeneralResult(true, _localizationManager.GetLocalizedString("BlogCreatedSuccessfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating blog");
                return new GeneralResult(false, _localizationManager.GetLocalizedString("ErrorCreatingBlog"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> ProcessStatus(int blogId, BlogStatus status, CancellationToken cancellationToken)
        {
            try
            {
                var blog = await _dbContext.Blogs.FirstOrDefaultAsync(b => b.Id == blogId && !b.IsDeleted);
                if (blog == null)
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("BlogNotFound"));

                blog.Status = status;
                blog.UpdatedAt = DateTimeOffset.UtcNow;
                _dbContext.Blogs.Update(blog);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return new GeneralResult(true, _localizationManager.GetLocalizedString("BlogStatusUpdatedSuccessfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating blog status");
                return new GeneralResult(false, _localizationManager.GetLocalizedString("ErrorUpdatingBlogStatus"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> DeleteBlogAsync(int blogId, CancellationToken cancellationToken)
        {
            try
            {
                var blog = await _dbContext.Blogs.FirstOrDefaultAsync(b => b.Id == blogId && !b.IsDeleted && b.Status == BlogStatus.Accepted);
                if (blog == null)
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("BlogNotFound"));

                blog.IsDeleted = true;
                blog.UpdatedAt = DateTimeOffset.UtcNow;
                _dbContext.Blogs.Update(blog);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return new GeneralResult(true, _localizationManager.GetLocalizedString("BlogDeletedSuccessfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting blog");
                return new GeneralResult(false, _localizationManager.GetLocalizedString("ErrorDeletingBlog"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<BlogDetailsDto>>> AllBlogsAsync(PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var query = _dbContext.Blogs
                    .AsNoTracking()
                    .Include(b => b.User)
                    .Where(b => !b.IsDeleted && b.Status == BlogStatus.Accepted)
                    .OrderByDescending(b => b.CreatedAt)
                    .Select(b => new BlogDetailsDto
                    {
                        Id = b.Id,
                        Title = b.Title,
                        Details = b.Details,
                        Media = b.Media,
                        Status = b.Status,
                        CreatedAt = b.CreatedAt ?? DateTimeOffset.UtcNow,
                        Writer = new BlogWriterBto
                        {
                            Name = b.User.FirstName + " " + b.User.LastName,
                            Avatar = b.User.AvatarUrl ?? string.Empty,
                            Email = b.User.Email
                        }
                    });

                var paginatedResult = await query.ToPagedResultAsync(pagination, cancellationToken);

                return new GeneralResult<PaginatedResult<BlogDetailsDto>>(true, _localizationManager.GetLocalizedString("UserBlogsFetchedSuccessfully"), paginatedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching blogs");
                return new GeneralResult<PaginatedResult<BlogDetailsDto>>(false, _localizationManager.GetLocalizedString("ErrorFetchingBlogs"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<BlogDetailsDto>>> UserBlogsAsync(string userId, CancellationToken cancellationToken)
        {
            try
            {
                var blog = await _dbContext.Blogs
                    .Include(b => b.User)
                    .Where(b => !b.IsDeleted && b.UserId == userId && b.Status == BlogStatus.Accepted)
                    .OrderByDescending(b => b.CreatedAt)
                    .Select(b => new BlogDetailsDto
                    {
                        Id = b.Id,
                        Title = b.Title,
                        Details = b.Details,
                        Media = b.Media,
                        Status = b.Status,
                        CreatedAt = b.CreatedAt ?? DateTimeOffset.UtcNow,
                        Writer = new BlogWriterBto
                        {
                            Name = b.User.FirstName + " " + b.User.LastName,
                            Avatar = b.User.AvatarUrl ?? string.Empty,
                            Email = b.User.Email
                        }
                    }).ToListAsync(cancellationToken);

                if (blog == null)
                    return new GeneralResult<List<BlogDetailsDto>>(false, _localizationManager.GetLocalizedString("BlogNotFound"), null);

                return new GeneralResult<List<BlogDetailsDto>>(true, _localizationManager.GetLocalizedString("BlogFetchedSuccessfully"), blog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user blogs");
                return new GeneralResult<List<BlogDetailsDto>>(false, _localizationManager.GetLocalizedString("ErrorFetchingUserBlogs"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<BlogDetailsDto>> GetByIdAsync(int blogId, CancellationToken cancellationToken)
        {
            const string method = nameof(GetByIdAsync);
            try
            {
                var blog = await _dbContext.Blogs
                    .AsNoTracking()
                    .Include(b => b.User)
                    .Where(b => !b.IsDeleted && b.Status == BlogStatus.Accepted && b.Id == blogId)
                    .Select(b => new BlogDetailsDto
                    {
                        Id = b.Id,
                        Title = b.Title,
                        Details = b.Details,
                        Media = b.Media,
                        Status = b.Status,
                        CreatedAt = b.CreatedAt ?? DateTimeOffset.UtcNow,
                        Writer = new BlogWriterBto
                        {
                            Name = b.User.FirstName + " " + b.User.LastName,
                            Avatar = b.User.AvatarUrl ?? string.Empty,
                            Email = b.User.Email
                        }
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                if (blog == null)
                {
                    _logger.LogWarning("{Method} - Blog not found with id {Id}", method, blogId);
                    return new GeneralResult<BlogDetailsDto>(
                        false,
                        _localizationManager.GetLocalizedString("BlogNotFound"),
                        null);
                }

                return new GeneralResult<BlogDetailsDto>(
                    true,
                    _localizationManager.GetLocalizedString("BlogFetchedSuccessfully"),
                    blog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Method} - Error fetching blog with id {Id}", method, blogId);
                return new GeneralResult<BlogDetailsDto>(
                    false,
                    _localizationManager.GetLocalizedString("ErrorFetchingBlog"),
                    null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<BlogDetailsDto>>> BlogsByStatusAsync(BlogStatus status, PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var query = _dbContext.Blogs
                    .Include(b => b.User)
                    .Where(b => !b.IsDeleted && b.Status == status)
                    .OrderByDescending(b => b.CreatedAt)
                    .Select(b => new BlogDetailsDto
                    {
                        Id = b.Id,
                        Title = b.Title,
                        Details = b.Details,
                        Media = b.Media,
                        Status = b.Status,
                        CreatedAt = b.CreatedAt ?? DateTimeOffset.UtcNow,
                        Writer = new BlogWriterBto
                        {
                            Name = b.User.FirstName + " " + b.User.LastName,
                            Avatar = b.User.AvatarUrl ?? string.Empty,
                            Email = b.User.Email
                        }
                    });

                // Apply pagination to the query
                var paginatedResult = await query.ToPagedResultAsync(pagination, cancellationToken);

                return new GeneralResult<PaginatedResult<BlogDetailsDto>>(
                    true,
                    _localizationManager.GetLocalizedString("UserBlogsFetchedSuccessfully"),
                    paginatedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching blogs by status");
                return new GeneralResult<PaginatedResult<BlogDetailsDto>>(
                    false,
                    _localizationManager.GetLocalizedString("ErrorFetchingBlogsByStatus"),
                    null);
            }
        }
    }
}
