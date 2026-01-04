using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.MyCommunityDtos;
using EntreLaunch.DTOs.TrainingDtos;
using EntreLaunch.Extensions;
namespace EntreLaunch.Services.MyCommunitySvc
{
    public class MyCommunityService(
        PgDbContext dbContext,
        ILogger<MyCommunityService> logger,
        IMapper mapper,
        ILocalizationManager localizationManager,
        IHttpContextHelper httpContextHelper) : IMyCommunityService
    {
        private readonly PgDbContext _dbContext = dbContext;
        private readonly ILogger<MyCommunityService> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly ILocalizationManager _localizationManager = localizationManager;

        #region Create Operations

        /// <inheritdoc />
        public async Task<GeneralResult> CreateTextPostAsync(TextPostCreateDto dto)
        {
            _logger.LogInformation("Start creating a text-only post...");

            if (dto.Text == null)
            {
                _logger.LogError("Post text can not be null.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("InvalidPostData"),
                    Data = null
                };
            }

            try
            {
                var user = await _dbContext.Users.FindAsync(dto.UserId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", dto.UserId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("UserNotFound"),
                        Data = null
                    };
                }

                var post = _mapper.Map<Post>(dto);

                post.ByUserAgent = httpContextHelper.UserAgent;
                post.ByIp = httpContextHelper.IpAddress;
                post.ById = user.Id;
                post.CreatedAt = DateTimeOffset.UtcNow;
                post.IsDeleted = false;

                await _dbContext.Posts.AddAsync(post);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Text-only post created successfully. PostId: {PostId}", post.Id);

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("PostCreatedSuccessfully"),
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating text post.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("ErrorCreatingTextPost"),
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> CreatePostWithMediaAsync(PostWithMediaCreateDto dto)
        {
            _logger.LogInformation("Start creating a post with media...");

            if (dto.Text == null)
            {
                _logger.LogError("Post text can not be null.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("InvalidPostData"),
                    Data = null
                };
            }

            if (dto.Media == null)
            {
                _logger.LogError("Post media can not be null.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("PostMediaIsRequired"),
                    Data = null
                };
            }

            try
            {
                var user = await _dbContext.Users.FindAsync(dto.UserId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", dto.UserId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("UserNotFound"),
                        Data = null
                    };
                }

                var post = _mapper.Map<Post>(dto);
                post.ByUserAgent = httpContextHelper.UserAgent;
                post.ByIp = httpContextHelper.IpAddress;
                post.ById = user.Id;
                post.CreatedAt = DateTimeOffset.UtcNow;
                post.IsDeleted = false;

                // add media
                if (dto.Media != null && dto.Media.Any())
                {
                    post.PostMedias = dto.Media.Select(mediaDto => new PostMedia
                    {
                        MediaType = mediaDto.MediaType,
                        Url = mediaDto.Url,
                        CreatedAt = DateTimeOffset.UtcNow,
                        IsDeleted = false
                    }).ToList();
                }

                await _dbContext.Posts.AddAsync(post);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Post with media created successfully. PostId: {PostId}", post.Id);

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("PostCreatedSuccessfully"),
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating post with media.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("ErrorCreatingMediaPost")
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> CreateLikeAsync(LikeCreateDto dto)
        {
            _logger.LogInformation("Start processing Like for post {PostId} by user {UserId}...", dto?.PostId, dto?.UserId);

            if (dto == null)
            {
                _logger.LogError("LikeCreateDto is null.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("InvalidLikeData"),
                    Data = null
                };
            }

            try
            {
                var user = await _dbContext.Users.AnyAsync(u => u.Id == dto.UserId && !u.IsDeleted);
                if (user == false)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", dto.UserId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("UserNotFound")
                    };
                }

                if (!dto.PostId.HasValue && !dto.CommentId.HasValue)
                {
                    _logger.LogWarning("post or comment required to add like");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("PostOrCommentRequiredForLike"),
                        Data = null
                    };
                }
                else if (dto.PostId.HasValue && dto.CommentId.HasValue)
                {
                    _logger.LogError("Like should be either for post or for comment, not both.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("LikeTargetConflict"),
                        Data = null
                    };
                }
                else if (dto.PostId.HasValue)
                {
                    var post = await _dbContext.Posts.AnyAsync(p => p.Id == dto.PostId && !p.IsDeleted);
                    if (post == false)
                    {
                        _logger.LogWarning("Post with ID {PostId} not found.", dto.PostId);
                        return new GeneralResult
                        {
                            IsSuccess = false,
                            Message = _localizationManager.GetLocalizedString("PostNotFound")
                        };
                    }

                    var existingLike = await _dbContext.PostLikes.FirstOrDefaultAsync(l => l.PostId == dto.PostId && l.UserId == dto.UserId);
                    if (existingLike == null)
                    {
                        var like = _mapper.Map<PostLike>(dto);
                        like.IsActive = true;
                        like.CreatedAt = DateTimeOffset.UtcNow;
                        like.IsDeleted = false;
                        like.ByUserAgent = httpContextHelper.UserAgent;
                        like.ByIp = httpContextHelper.IpAddress;
                        like.ById = dto.UserId;
                        await _dbContext.PostLikes.AddAsync(like);
                    }
                    else
                    {
                        existingLike.IsActive = !(existingLike.IsActive ?? false);
                        existingLike.UpdatedAt = DateTimeOffset.UtcNow;
                        _dbContext.PostLikes.Update(existingLike);
                    }
                }
                else if (dto.CommentId.HasValue)
                {
                    var comment = await _dbContext.PostComments.AnyAsync(p => p.Id == dto.CommentId && !p.IsDeleted);
                    if (comment == false)
                    {
                        _logger.LogWarning("Comment with ID {CommentId} not found.", dto.CommentId);
                        return new GeneralResult
                        {
                            IsSuccess = false,
                            Message = _localizationManager.GetLocalizedString("CommentNotFound"),
                            Data = null
                        };
                    }

                    var existingLike = await _dbContext.PostLikes.FirstOrDefaultAsync(l => l.PostId == dto.PostId && l.UserId == dto.UserId);
                    if (existingLike == null)
                    {
                        var like = _mapper.Map<PostLike>(dto);
                        like.IsActive = true;
                        like.CreatedAt = DateTimeOffset.UtcNow;
                        like.IsDeleted = false;
                        await _dbContext.PostLikes.AddAsync(like);
                    }
                    else
                    {
                        existingLike.IsActive = !(existingLike.IsActive ?? false);
                        existingLike.UpdatedAt = DateTimeOffset.UtcNow;
                        _dbContext.PostLikes.Update(existingLike);
                    }
                }

                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Like processed successfully for post {PostId} by user {UserId}.", dto.PostId, dto.UserId);

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("LikeAddedSuccessfully"),
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error liking post.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("ErrorCreatingLike"),
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> CreateReportAsync(ReportCreateDto dto)
        {
            _logger.LogInformation("Start creating a report for post {PostId} by user {UserId}...", dto?.PostId, dto?.UserId);

            if (dto == null)
            {
                _logger.LogError("ReportCreateDto is null.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("InvalidReportData"),
                    Data = null
                };
            }

            if (dto.Reason == null)
            {
                _logger.LogError("Report reason is null.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("ReportReasonRequired")
                };
            }


            try
            {
                var user = await _dbContext.Users.AnyAsync(u => u.Id == dto.UserId && !u.IsDeleted);
                if (user == false)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", dto.UserId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("UserNotFound"),
                        Data = null
                    };
                }


                if (!dto.PostId.HasValue && !dto.CommentId.HasValue)
                {
                    _logger.LogWarning("post or comment required to add report");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("PostOrCommentRequiredForReport"),
                        Data = null
                    };
                }
                else if (dto.PostId.HasValue && dto.CommentId.HasValue)
                {
                    _logger.LogError("Report should be either for post or for comment, not both.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("ReportTargetConflict"),
                        Data = null
                    };
                }
                else if (dto.PostId.HasValue)
                {
                    var post = await _dbContext.Posts.AnyAsync(p => p.Id == dto.PostId && !p.IsDeleted);
                    if (post == false)
                    {
                        _logger.LogWarning("Post with ID {PostId} not found.", dto.PostId);
                        return new GeneralResult
                        {
                            IsSuccess = false,
                            Message = _localizationManager.GetLocalizedString("PostNotFound")
                        };
                    }

                    var report = _mapper.Map<CommunityReport>(dto);
                    report.Status = RequestStatus.Pending;
                    report.CreatedAt = DateTimeOffset.UtcNow;
                    report.Parent = ReportParent.Post;
                    report.IsDeleted = false;
                    report.ByUserAgent = httpContextHelper.UserAgent;
                    report.ByIp = httpContextHelper.IpAddress;
                    report.ById = dto.UserId;
                    await _dbContext.CommunityReports.AddAsync(report);
                }
                else if (dto.CommentId.HasValue)
                {
                    var comment = await _dbContext.PostComments.AnyAsync(p => p.Id == dto.CommentId && !p.IsDeleted);
                    if (comment == false)
                    {
                        _logger.LogWarning("Comment with ID {CommentId} not found.", dto.CommentId);
                        return new GeneralResult
                        {
                            IsSuccess = false,
                            Message = _localizationManager.GetLocalizedString("CommentNotFound"),
                            Data = null
                        };
                    }

                    var report = _mapper.Map<CommunityReport>(dto);
                    report.Status = RequestStatus.Pending;
                    report.CreatedAt = DateTimeOffset.UtcNow;
                    report.Parent = ReportParent.Comment;
                    report.IsDeleted = false;
                    await _dbContext.CommunityReports.AddAsync(report);
                }


                await _dbContext.SaveChangesAsync();
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("ReportCreatedSuccessfully"),
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating post report.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("ErrorCreatingReport")
                };
            }
        }

        #endregion

        #region Read Operations (Get / Show)

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<PostDetailsDto>>> GetAllPostsAsync(PaginationParams pagination, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start fetching all posts with full data...");

            try
            {
                var query = _dbContext.Posts
                    .AsNoTracking()
                    .Where(p => !p.IsDeleted)
                    .OrderByDescending(p => p.CreatedAt)
                    .Select(p => new PostDetailsDto
                    {
                        Id = p.Id,
                        Text = p.Text,
                        CreatedAt = p.CreatedAt,

                        User = new PostUserData
                        {
                            FirstName = p.User.FirstName,
                            LastName = p.User.LastName,
                            Email = p.User.Email
                        },

                        Media = p.PostMedias!
                            .Where(m => !m.IsDeleted)
                            .Select(m => new PostMediaDetailsDto
                            {
                                Url = m.Url
                            }).ToList(),
                        Likes = p.PostLikes!.Count(l => l.IsActive == true)
                    });

                var paginatedResult = await query.ToPagedResultAsync(pagination, cancellationToken);

                return new GeneralResult<PaginatedResult<PostDetailsDto>>(true,
                    _localizationManager.GetLocalizedString("AllPostsFetchedSuccessfully"),
                    paginatedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all posts.");
                return new GeneralResult<PaginatedResult<PostDetailsDto>>(false,
                    _localizationManager.GetLocalizedString("PostFetchedSuccessfully"),
                    null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> GetPostByIdAsync(int postId)
        {
            _logger.LogInformation("Start fetching post with Id={PostId}", postId);

            try
            {
                var post = await _dbContext.Posts.AsNoTracking()
                    .Where(p => p.Id == postId && !p.IsDeleted && p.Status == RequestStatus.Accepted)
                    .Select(p => new PostDetailsDto
                    {
                        Id = p.Id,
                        Text = p.Text,
                        CreatedAt = p.CreatedAt,

                        User = new PostUserData
                        {
                            FirstName = p.User.FirstName,
                            LastName = p.User.LastName,
                            Email = p.User.Email
                        },

                        Media = p.PostMedias!
                        .Where(m => !m.IsDeleted)
                        .Select(m => new PostMediaDetailsDto
                        {
                            Url = m.Url
                        }).ToList(),
                        Likes = p.PostLikes!.Count(l => l.IsActive == true)
                    })
                    .FirstOrDefaultAsync();

                if (post == null)
                {
                    _logger.LogWarning("Post with ID {PostId} not found.", postId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("PostNotFound"),
                        Data = null
                    };
                }

                var postDto = _mapper.Map<PostDetailsDto>(post);

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("PostFetchedSuccessfully"),
                    Data = postDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching post with Id={PostId}.", postId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("ErrorFetchingPost")
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> GetPostLikeCountAsync(int postId)
        {
            _logger.LogInformation("Start fetching like count for post with Id={PostId}", postId);

            try
            {
                var postExists = await _dbContext.Posts
                    .AnyAsync(p => p.Id == postId && !p.IsDeleted && p.Status == RequestStatus.Accepted);
                if (!postExists)
                {
                    _logger.LogWarning("Post with ID {PostId} not found.", postId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("PostNotFound")
                    };
                }

                var likeCount = await _dbContext.PostLikes
                    .CountAsync(l => l.PostId == postId && l.IsActive == true && !l.IsDeleted);

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("LikeCountFetchedSuccessfully"),
                    Data = likeCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching like count for post with Id={PostId}.", postId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("ErrorFetchingLikeCount")
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> GetPostReportsAsync(int postId)
        {
            _logger.LogInformation("Start fetching reports for post with Id={PostId}", postId);

            try
            {
                var postExists = await _dbContext.Posts.AnyAsync(p => p.Id == postId && !p.IsDeleted && p.Status == RequestStatus.Accepted);
                if (!postExists)
                {
                    _logger.LogWarning("Post with ID {PostId} not found.", postId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("PostNotFound")
                    };
                }

                var reports = await _dbContext.CommunityReports
                    .Where(r => r.PostId == postId && !r.IsDeleted)
                    .Select(r => new ReportDetailsDto
                    {
                        User = new ReportUserData
                        {
                            UserId = r.User.Id,
                            FirstName = r.User.FirstName,
                            LastName = r.User.LastName,
                            Email = r.User.Email
                        },
                        PostId = r.PostId,
                        Status = r.Status,
                        Reason = r.Reason,
                        CreatedAt = r.CreatedAt,
                    })
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();

                if (!reports.Any())
                {
                    _logger.LogWarning("Reports for post with ID {PostId} not found.", postId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("ReportsNotFound"),
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("ReportsFetchedSuccessfully"),
                    Data = reports
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching reports for post with Id={PostId}.", postId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("ErrorFetchingPostReports"),
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> GetCommentReportsAsync(int commentId)
        {
            _logger.LogInformation("Start fetching reports for comment with Id={CommentId}", commentId);

            try
            {
                var commentExists = await _dbContext.PostComments.AnyAsync(c => c.Id == commentId && !c.IsDeleted);
                if (!commentExists)
                {
                    _logger.LogWarning("Comment with ID {CommentId} not found.", commentId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("CommentNotFound"),
                        Data = null
                    };
                }

                var reports = await _dbContext.CommunityReports
                    .Where(r => r.CommentId == commentId && !r.IsDeleted)
                    .Select(r => new ReportDetailsDto
                    {
                        User = new ReportUserData
                        {
                            UserId = r.User.Id,
                            FirstName = r.User.FirstName,
                            LastName = r.User.LastName,
                            Email = r.User.Email
                        },
                        PostId = r.PostId,
                        Status = r.Status,
                        Reason = r.Reason,
                        CreatedAt = r.CreatedAt,
                    })
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();

                if (!reports.Any())
                {
                    _logger.LogWarning("Reports for comment with ID {PostId} not found.", commentId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("ReportsNotFound"),
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("CommentReportsFetchedSuccessfully"),
                    Data = reports
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching reports for comment with Id={CommentId}.", commentId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("ErrorFetchingCommentReports")
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<PostDetailsDto>>> GetPendingPostsAsync(PaginationParams pagination, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start fetching pending posts with pagination...");

            try
            {
                var query = _dbContext.Posts
                    .AsNoTracking()
                    .Where(p => !p.IsDeleted && p.Status == RequestStatus.Pending)
                    .Select(p => new PostDetailsDto
                    {
                        Id = p.Id,
                        Text = p.Text,
                        CreatedAt = p.CreatedAt,
                        User = new PostUserData
                        {
                            UserId = p.User.Id,
                            FirstName = p.User.FirstName,
                            LastName = p.User.LastName,
                            Email = p.User.Email
                        },
                        Media = p.PostMedias!.Where(m => !m.IsDeleted)
                            .Select(m => new PostMediaDetailsDto { Url = m.Url }).ToList(),
                        Likes = p.PostLikes!.Count(l => l.IsActive == true)
                    });

                var paginated = await query.ToPagedResultAsync(pagination);

                return new GeneralResult<PaginatedResult<PostDetailsDto>>(true, _localizationManager.GetLocalizedString("PendingPostsFetchedSuccessfully"), paginated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pending posts with pagination.");
                return new GeneralResult<PaginatedResult<PostDetailsDto>>(false, _localizationManager.GetLocalizedString("ErrorFetchingPendingPosts"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<PostDetailsDto>>> GetAcceptedPostsAsync(PaginationParams pagination, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start fetching accepted posts with pagination...");

            try
            {
                var query = _dbContext.Posts
                    .AsNoTracking()
                    .Where(p => !p.IsDeleted && p.Status == RequestStatus.Accepted)
                    .Select(p => new PostDetailsDto
                    {
                        Id = p.Id,
                        Text = p.Text,
                        CreatedAt = p.CreatedAt,
                        User = new PostUserData
                        {
                            UserId = p.User.Id,
                            FirstName = p.User.FirstName,
                            LastName = p.User.LastName,
                            Email = p.User.Email
                        },
                        Media = p.PostMedias!.Where(m => !m.IsDeleted)
                            .Select(m => new PostMediaDetailsDto { Url = m.Url }).ToList(),
                        Likes = p.PostLikes!.Count(l => l.IsActive == true)
                    });

                var paginated = await query.ToPagedResultAsync(pagination);

                return new GeneralResult<PaginatedResult<PostDetailsDto>>(true, _localizationManager.GetLocalizedString("AcceptedPostsFetchedSuccessfully"), paginated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching accepted posts with pagination.");
                return new GeneralResult<PaginatedResult<PostDetailsDto>>(false, _localizationManager.GetLocalizedString("ErrorFetchingAcceptedPosts"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<PostDetailsDto>>> GetRejectedPostsAsync(PaginationParams pagination, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start fetching rejected posts with pagination...");

            try
            {
                var query = _dbContext.Posts
                    .AsNoTracking()
                    .Where(p => !p.IsDeleted && p.Status == RequestStatus.Rejected)
                    .Select(p => new PostDetailsDto
                    {
                        Id = p.Id,
                        Text = p.Text,
                        CreatedAt = p.CreatedAt,
                        User = new PostUserData
                        {
                            UserId = p.User.Id,
                            FirstName = p.User.FirstName,
                            LastName = p.User.LastName,
                            Email = p.User.Email
                        },
                        Media = p.PostMedias!.Where(m => !m.IsDeleted)
                            .Select(m => new PostMediaDetailsDto { Url = m.Url }).ToList(),
                        Likes = p.PostLikes!.Count(l => l.IsActive == true)
                    });

                var paginated = await query.ToPagedResultAsync(pagination);

                return new GeneralResult<PaginatedResult<PostDetailsDto>>(true, _localizationManager.GetLocalizedString("RejectedPostsFetchedSuccessfully"), paginated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching rejected posts with pagination.");
                return new GeneralResult<PaginatedResult<PostDetailsDto>>(false, _localizationManager.GetLocalizedString("ErrorFetchingRejectedPosts"), null);
            }
        }


        /// <inheritdoc />
        public async Task<GeneralResult> GetAcceptedReportsAsync()
        {
            _logger.LogInformation("Start fetching accepted reports...");

            try
            {
                var reports = await _dbContext.CommunityReports
                    .Where(r => !r.IsDeleted && r.Status == RequestStatus.Accepted)
                    .Select(r => new ReportDetailsDto
                    {
                        User = new ReportUserData
                        {
                            UserId = r.User.Id,
                            FirstName = r.User.FirstName,
                            LastName = r.User.LastName,
                            Email = r.User.Email
                        },
                        PostId = r.PostId,
                        Status = r.Status,
                        Reason = r.Reason,
                        CreatedAt = r.CreatedAt,
                    })
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();

                if (!reports.Any() || reports == null)
                {
                    _logger.LogWarning("Accepted reports not found.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("AcceptedReportsNotFound"),
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("AcceptedReportsFetchedSuccessfully"),
                    Data = reports
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching accepted reports.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("ErrorFetchingAcceptedReports"),
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> GetPendingReportsAsync()
        {
            _logger.LogInformation("Start fetching pending reports...");

            try
            {
                var reports = await _dbContext.CommunityReports
                    .Where(r => !r.IsDeleted && r.Status == RequestStatus.Pending)
                    .Select(r => new ReportDetailsDto
                    {
                        User = new ReportUserData
                        {
                            UserId = r.User.Id,
                            FirstName = r.User.FirstName,
                            LastName = r.User.LastName,
                            Email = r.User.Email
                        },
                        PostId = r.PostId,
                        Status = r.Status,
                        Reason = r.Reason,
                        CreatedAt = r.CreatedAt,
                    })
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();

                if (!reports.Any() || reports == null)
                {
                    _logger.LogInformation("No pending reports found.");
                    return new GeneralResult
                    {
                        IsSuccess = true,
                        Message = _localizationManager.GetLocalizedString("NoPendingReportsFound"),
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("PendingReportsFetchedSuccessfully"),
                    Data = reports
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pending reports.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("ErrorFetchingPendingReports"),
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> GetRejectedReportsAsync()
        {
            _logger.LogInformation("Start fetching rejected reports...");

            try
            {
                var reports = await _dbContext.CommunityReports
                    .Where(r => !r.IsDeleted && r.Status == RequestStatus.Rejected)
                    .Select(r => new ReportDetailsDto
                    {
                        User = new ReportUserData
                        {
                            UserId = r.User.Id,
                            FirstName = r.User.FirstName,
                            LastName = r.User.LastName,
                            Email = r.User.Email
                        },
                        PostId = r.PostId,
                        Status = r.Status,
                        Reason = r.Reason,
                        CreatedAt = r.CreatedAt,
                    })
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();

                if (!reports.Any() || reports == null)
                {
                    _logger.LogInformation("No rejected reports found.");
                    return new GeneralResult
                    {
                        IsSuccess = true,
                        Message = _localizationManager.GetLocalizedString("NoRejectedReportsFound"),
                        Data = null
                    };
                }

                var reportsDto = _mapper.Map<List<ReportDetailsDto>>(reports);

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("RejectedReportsFetchedSuccessfully"),
                    Data = reportsDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching rejected reports.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("ErrorFetchingRejectedReports"),
                    Data = null
                };
            }
        }

        #endregion

        #region Processing Status Operations

        /// <inheritdoc />
        public async Task<GeneralResult> ProcessPostStatusAsync(int postId, RequestStatus status)
        {
            _logger.LogInformation("Start processing post status. PostId={PostId}, NewStatus={Status}", postId, status);

            try
            {
                var post = await _dbContext.Posts
                    .FirstOrDefaultAsync(p => p.Id == postId && !p.IsDeleted);

                if (post == null)
                {
                    _logger.LogWarning("Post with ID {PostId} not found.", postId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("UserNotFound")
                    };
                }

                if (post.Status == status)
                {
                    _logger.LogWarning("Post with ID {PostId} already has the same status.", postId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("PostAlreadyHasSameStatus")
                    };
                }

                post.Status = status;
                post.UpdatedAt = DateTimeOffset.UtcNow;
                _dbContext.Posts.Update(post);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("PostStatusProcessedSuccessfully"),
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing post status. PostId={PostId}", postId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("ErrorProcessingPostStatus"),
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> ProcessReportStatusAsync(int reportId, RequestStatus status)
        {
            _logger.LogInformation("Start processing report status. ReportId={ReportId}, NewStatus={Status}", reportId, status);

            try
            {
                var report = await _dbContext.CommunityReports
                    .FirstOrDefaultAsync(r => r.Id == reportId && !r.IsDeleted);
                if (report == null)
                {
                    _logger.LogWarning("Report with ID {ReportId} not found.", reportId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("ReportNotFound")
                    };
                }

                if (report.Status == status)
                {
                    _logger.LogWarning("Report with ID {ReportId} already has the same status.", reportId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("ReportAlreadyHasSameStatus"),
                        Data = null
                    };
                }

                report.Status = status;
                report.UpdatedAt = DateTimeOffset.UtcNow;
                _dbContext.CommunityReports.Update(report);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("ReportStatusProcessedSuccessfully"),
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing report status. ReportId={ReportId}", reportId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("ErrorProcessingReportStatus")
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> ProcessCommentStatusAsync(int commentId, CommentStatus status)
        {
            _logger.LogInformation("Start processing comment status. CommentId={CommentId}, NewStatus={Status}", commentId, status);

            try
            {
                var comment = await _dbContext.PostComments
                    .FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted);
                if (comment == null)
                {
                    _logger.LogWarning("Comment with ID {CommentId} not found.", commentId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("CommentNotFound")
                    };
                }

                if (comment.Status == status)
                {
                    _logger.LogWarning("Comment with ID {CommentId} already has the same status.", commentId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("CommentAlreadyHasSameStatus"),
                        Data = null
                    };
                }

                comment.Status = status;
                comment.UpdatedAt = DateTimeOffset.UtcNow;

                _dbContext.PostComments.Update(comment);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("CommentStatusProcessedSuccessfully"),
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing comment status. CommentId={CommentId}", commentId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("ErrorProcessingCommentStatus")
                };
            }
        }

        #endregion

        #region Delete Operations

        /// <inheritdoc />
        public async Task<GeneralResult> DeletePostAsync(int postId, string userId)
        {
            _logger.LogInformation("Start deleting post with Id={PostId} by user {UserId}", postId, userId);
            try
            {
                var post = await _dbContext.Posts
                    .FirstOrDefaultAsync(p => p.Id == postId && !p.IsDeleted && p.Status == RequestStatus.Accepted);

                if (post == null)
                {
                    _logger.LogWarning("Post with ID {PostId} not found.", postId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("PostNotFound"),
                        Data = null
                    };
                }

                if (!string.Equals(post.UserId, userId, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("User {UserId} is not the owner of post {PostId}", userId, postId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("UserIsNotOwnerOfPost"),
                        Data = null
                    };
                }

                // remove likes
                if (post.PostLikes != null && post.PostLikes.Any())
                {
                    foreach (var like in post.PostLikes)
                    {
                        like.IsDeleted = true;
                        like.UpdatedAt = DateTimeOffset.UtcNow;
                    }
                }

                // remove comments
                if (post.PostComments != null && post.PostComments.Any())
                {
                    foreach (var comment in post.PostComments)
                    {
                        comment.IsDeleted = true;
                        comment.UpdatedAt = DateTimeOffset.UtcNow;
                    }
                }

                // remove media
                if (post.PostMedias != null && post.PostMedias.Any())
                {
                    foreach (var media in post.PostMedias)
                    {
                        media.IsDeleted = true;
                        media.UpdatedAt = DateTimeOffset.UtcNow;
                    }
                }

                // remove reports
                if (post.CommunityReports != null && post.CommunityReports.Any())
                {
                    foreach (var report in post.CommunityReports)
                    {
                        report.IsDeleted = true;
                        report.UpdatedAt = DateTimeOffset.UtcNow;
                    }
                }

                // remove post
                post.IsDeleted = true;
                post.UpdatedAt = DateTimeOffset.UtcNow;
                _dbContext.Posts.Update(post);
                await _dbContext.SaveChangesAsync();
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("PostDeletedSuccessfully"),
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting post with Id={PostId} by user {UserId}.", postId, userId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("ErrorDeletingPost"),
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> DeleteCommentAsync(int commentId, string userId)
        {
            _logger.LogInformation("Start deleting comment with Id={CommentId} by user {UserId}", commentId, userId);
            try
            {
                var comment = await _dbContext.PostComments
                    .Include(c => c.PostLikes)
                    .FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted);

                if (comment == null)
                {
                    _logger.LogWarning("Comment with ID {CommentId} not found.", commentId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("CommentNotFound"),
                        Data = null
                    };
                }

                if (!string.Equals(comment.UserId, userId, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("User {UserId} is not the owner of comment {CommentId}.", userId, commentId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("UserIsNotOwnerOfComment"),
                        Data = null
                    };
                }

                if (comment.PostLikes != null && comment.PostLikes.Any())
                {
                    foreach (var like in comment.PostLikes)
                    {
                        like.IsDeleted = true;
                        like.UpdatedAt = DateTimeOffset.UtcNow;
                    }
                }

                // remove post
                comment.IsDeleted = true;
                comment.UpdatedAt = DateTimeOffset.UtcNow;
                _dbContext.PostComments.Update(comment);
                await _dbContext.SaveChangesAsync();

                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("CommentDeletedSuccessfully"),
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting comment with Id={CommentId} by user {UserId}.", commentId, userId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("ErrorDeletingComment"),
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> DeleteMediaAsync(int mediaId, string userId)
        {
            _logger.LogInformation("Start deleting media with Id={MediaId} by user {UserId}", mediaId, userId);
            try
            {
                var media = await _dbContext.PostMedias.FirstOrDefaultAsync(m => m.Id == mediaId && !m.IsDeleted);

                if (media == null)
                {
                    _logger.LogWarning("Media with ID {MediaId} not found.", mediaId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("MediaNotFound"),
                        Data = null
                    };
                }

                if (!string.Equals(media.Post.UserId, userId, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("User {UserId} is not the owner of post {PostId} that holds media {MediaId}.", userId, media.PostId, mediaId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("UserIsNotOwnerOfMediaPost"),
                        Data = null
                    };
                }

                media.IsDeleted = true;
                media.UpdatedAt = DateTimeOffset.UtcNow;
                _dbContext.PostMedias.Update(media);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Media with Id={MediaId} deleted successfully by user {UserId}.", mediaId, userId);

                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("MediaDeletedSuccessfully"),
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting media with Id={MediaId} by user {UserId}.", mediaId, userId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("ErrorDeletingMedia"),
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> DeletePostReportAsync(int reportId)
        {
            _logger.LogInformation("Start deleting post report with Id={ReportId}", reportId);
            try
            {
                var report = await _dbContext.CommunityReports
                    .FirstOrDefaultAsync(r => r.Id == reportId && r.PostId != null && !r.IsDeleted);

                if (report == null)
                {
                    _logger.LogWarning("Post report with ID {ReportId} not found.", reportId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("ErrorDeletingPostReport"),
                        Data = null
                    };
                }

                report.IsDeleted = true;
                report.UpdatedAt = DateTimeOffset.UtcNow;
                _dbContext.CommunityReports.Update(report);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Post report with Id={ReportId} deleted successfully.", reportId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("PostReportDeletedSuccessfully"),
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting post report with Id={ReportId}.", reportId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("ErrorDeletingPostReport"),
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> DeleteCommentReportAsync(int reportId)
        {
            _logger.LogInformation("Start deleting comment report with Id={ReportId}", reportId);
            try
            {
                var report = await _dbContext.CommunityReports
                    .FirstOrDefaultAsync(r => r.Id == reportId && r.CommentId != null && !r.IsDeleted);

                if (report == null)
                {
                    _logger.LogWarning("Comment report with ID {ReportId} not found.", reportId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("CommentReportNotFound"),
                        Data = null
                    };
                }

                report.IsDeleted = true;
                report.UpdatedAt = DateTimeOffset.UtcNow;
                _dbContext.CommunityReports.Update(report);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Comment report with Id={ReportId} deleted successfully.", reportId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("CommentReportDeletedSuccessfully"),
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting comment report with Id={ReportId}.", reportId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("ErrorDeletingCommentReport"),
                    Data = null
                };
            }
        }

        #endregion
    }
}

