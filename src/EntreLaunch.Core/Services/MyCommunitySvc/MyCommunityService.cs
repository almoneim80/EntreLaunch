namespace EntreLaunch.Services.MyCommunitySvc
{
    public class MyCommunityService : IMyCommunityService
    {
        private readonly PgDbContext _dbContext;
        private readonly ILogger<MyCommunityService> _logger;
        private readonly IMapper _mapper;
        public MyCommunityService(
            PgDbContext dbContext,
            ILogger<MyCommunityService> logger,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _logger = logger;
            _mapper = mapper;
        }

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
                    Message = "Invalid post data.",
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
                        Message = $"User not found.",
                        Data = null
                    };
                }

                var post = _mapper.Map<Post>(dto);
                post.CreatedAt = DateTimeOffset.UtcNow;
                post.IsDeleted = false;
                await _dbContext.Posts.AddAsync(post);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Text-only post created successfully. PostId: {PostId}", post.Id);

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Post created successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating text post.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An error occurred while creating the text post.",
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
                    Message = "Invalid post data.",
                    Data = null
                };
            }

            if (dto.Media == null)
            {
                _logger.LogError("Post media can not be null.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Post media can not be null.",
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
                        Message = $"User not found.",
                        Data = null
                    };
                }

                var post = _mapper.Map<Post>(dto);
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
                    Message = "Post with media created successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating post with media.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An error occurred while creating the post with media."
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> CreateMediaToPostAsync(int postId, List<MediaCreateDto> dto, string userId)
        {
            _logger.LogInformation("Start adding media to post with Id={PostId} by user {UserId}", postId, userId);
            try
            {
                var post = await _dbContext.Posts
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == postId && !p.IsDeleted && p.Status == RequestStatus.Accepted);

                if (post == null)
                {
                    _logger.LogWarning("Post with Id {PostId} not found or not available.", postId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Post not found.",
                        Data = null
                    };
                }

                if (!string.Equals(post.UserId, userId, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("User {UserId} is not the owner of post {PostId}", userId, postId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "User is not the owner of the post.",
                        Data = null
                    };
                }

                var newMediaEntities = new List<PostMedia>();
                foreach (var mediaDto in dto)
                {
                    var newMedia = new PostMedia
                    {
                        PostId = post.Id,
                        MediaType = mediaDto.MediaType,
                        Url = mediaDto.Url,
                        CreatedAt = DateTimeOffset.UtcNow
                    };

                    newMediaEntities.Add(newMedia);
                }

                // إضافة الوسائط إلى قاعدة البيانات
                await _dbContext.PostMedias.AddRangeAsync(newMediaEntities);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Media added successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding media to post {PostId} by user {UserId}", postId, userId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An error occurred while adding media.",
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> CreateCommentAsync(CommentCreateDto dto)
        {
            _logger.LogInformation("Start creating a comment for post {PostId}...", dto?.PostId);

            if (dto == null)
            {
                _logger.LogError("CommentCreateDto is null.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Invalid comment data."
                };
            }

            try
            {
                var user = await _dbContext.Users.Where(u => u.Id == dto.UserId && !u.IsDeleted).FirstOrDefaultAsync();
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", dto.UserId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = $"User with ID {dto.UserId} not found."
                    };
                }

                var post = await _dbContext.Posts.AnyAsync(p => p.Id == dto.PostId && !p.IsDeleted);
                if (post == false)
                {
                    _logger.LogWarning("Post with ID {PostId} not found.", dto.PostId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = $"Post with ID {dto.PostId} not found."
                    };
                }

                // is comment for post
                string parentName = "";
                if (dto.ParentCommentId.HasValue)
                {
                    var parentComment = await _dbContext.PostComments.FirstOrDefaultAsync(c => c.Id == dto.ParentCommentId && !c.IsDeleted);
                    if (parentComment == null)
                    {
                        _logger.LogWarning("Parent comment with ID {ParentCommentId} not found.", dto.ParentCommentId);
                        return new GeneralResult
                        {
                            IsSuccess = false,
                            Message = $"Parent comment not found.",
                            Data = null
                        };
                    }

                    if (parentComment.PostId != dto.PostId)
                    {
                        _logger.LogWarning("Parent comment with ID {ParentCommentId} does not belong to post with ID {PostId}.", dto.ParentCommentId, dto.PostId);
                        return new GeneralResult
                        {
                            IsSuccess = false,
                            Message = $"Parent comment does not belong to post.",
                            Data = null
                        };
                    }

                    parentName = "Comment";
                }
                else
                {
                    parentName = "Post";
                }

                var comment = _mapper.Map<PostComment>(dto);
                comment.ParentCommentId = dto.ParentCommentId;
                comment.ParentName = parentName;
                comment.Status = CommentStatus.Good;
                comment.CreatedAt = DateTimeOffset.UtcNow;
                comment.IsDeleted = false;

                await _dbContext.PostComments.AddAsync(comment);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Comment created successfully. CommentId: {CommentId}", comment.Id);

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Comment created successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating comment.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An error occurred while creating the comment."
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
                    Message = "Invalid like data.",
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
                        Message = $"User not found."
                    };
                }

                if (!dto.PostId.HasValue && !dto.CommentId.HasValue)
                {
                    _logger.LogWarning("post or comment required to add like");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = $"post or comment required to add like",
                        Data = null
                    };
                }
                else if (dto.PostId.HasValue && dto.CommentId.HasValue)
                {
                    _logger.LogError("Like should be either for post or for comment, not both.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Like should be either for post or for comment, not both.",
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
                            Message = $"Post not found."
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
                        existingLike.EntreLaunchdatedAt = DateTimeOffset.UtcNow;
                        _dbContext.PostLikes.EntreLaunchdate(existingLike);
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
                            Message = $"Comment not found.",
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
                        existingLike.EntreLaunchdatedAt = DateTimeOffset.UtcNow;
                        _dbContext.PostLikes.EntreLaunchdate(existingLike);
                    }
                }

                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Like processed successfully for post {PostId} by user {UserId}.", dto.PostId, dto.UserId);

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Like added successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error liking post.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An error occurred while liking.",
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
                    Message = "Invalid report data.",
                    Data = null
                };
            }

            if (dto.Reason == null)
            {
                _logger.LogError("Report reason is null.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Report reason is required."
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
                        Message = $"User not found.",
                        Data = null
                    };
                }


                if (!dto.PostId.HasValue && !dto.CommentId.HasValue)
                {
                    _logger.LogWarning("post or comment required to add report");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = $"post or comment required to add report",
                        Data = null
                    };
                }
                else if (dto.PostId.HasValue && dto.CommentId.HasValue)
                {
                    _logger.LogError("Report should be either for post or for comment, not both.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Report should be either for post or for comment, not both.",
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
                            Message = $"Post not found."
                        };
                    }

                    var report = _mapper.Map<CommunityReport>(dto);
                    report.Status = RequestStatus.Pending;
                    report.CreatedAt = DateTimeOffset.UtcNow;
                    report.Parent = ReportParent.Post;
                    report.IsDeleted = false;
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
                            Message = $"Comment not found.",
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
                    Message = $"Report created for report successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating post report.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An error occurred while creating the post report."
                };
            }
        }

        #endregion

        #region EntreLaunchdate Operations

        /// <inheritdoc />
        public async Task<GeneralResult> EntreLaunchdatePostAsync(int postId, PostEntreLaunchdateDto dto)
        {
            _logger.LogInformation("Start EntreLaunchdating post with ID {PostId}...", postId);

            if (dto == null)
            {
                _logger.LogError("PostEntreLaunchdateDto is null.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Invalid post EntreLaunchdate data."
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
                        Message = $"User not found.",
                        Data = null
                    };
                }

                var post = await _dbContext.Posts.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == postId && !p.IsDeleted);
                if (post == null)
                {
                    _logger.LogWarning("Post with ID {PostId} not found.", postId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = $"Post not found.",
                        Data = null
                    };
                }

                if (post.UserId != dto.UserId)
                {
                    _logger.LogWarning("Post with ID {PostId} not found.", postId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Can not EntreLaunchdate other user's post.",
                        Data = null
                    };
                }

                if (dto.Text != null)
                {
                    post.Text = dto.Text;
                }

                post.EntreLaunchdatedAt = DateTimeOffset.UtcNow;
                _dbContext.Posts.EntreLaunchdate(post);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Post with ID {PostId} EntreLaunchdated successfully.", postId);
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Post EntreLaunchdated successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error EntreLaunchdating post with ID {PostId}.", postId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An error occurred while EntreLaunchdating the post."
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> EntreLaunchdateMediaAsync(int mediaId, MediaEntreLaunchdateDto dto)
        {
            _logger.LogInformation("Start EntreLaunchdating media with ID {MediaId}...", mediaId);

            if (dto == null)
            {
                _logger.LogError("MediaEntreLaunchdateDto is null.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Invalid media EntreLaunchdate data.",
                    Data = null
                };
            }

            try
            {
                var media = await _dbContext.PostMedias.Include(m => m.Post).FirstOrDefaultAsync(m => m.Id == mediaId && !m.IsDeleted);
                if (media == null)
                {
                    _logger.LogWarning("Media with ID {MediaId} not found.", mediaId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = $"Media with ID {mediaId} not found.",
                        Data = null
                    };
                }

                if (!string.IsNullOrWhiteSpace(dto.Url))
                {
                    media.Url = dto.Url;
                }

                media.EntreLaunchdatedAt = DateTimeOffset.UtcNow;
                _dbContext.PostMedias.EntreLaunchdate(media);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Media EntreLaunchdated successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error EntreLaunchdating media with ID {MediaId}.", mediaId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An error occurred while EntreLaunchdating the media.",
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> EntreLaunchdateCommentAsync(int commentId, CommentEntreLaunchdateDto dto)
        {
            _logger.LogInformation("Start EntreLaunchdating comment with ID {CommentId}...", commentId);

            if (dto == null)
            {
                _logger.LogError("CommentEntreLaunchdateDto is null.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Invalid comment EntreLaunchdate data."
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
                        Message = $"User not found.",
                        Data = null
                    };
                }

                var comment = await _dbContext.PostComments.FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted);
                if (comment == null)
                {
                    _logger.LogWarning("Comment with ID {CommentId} not found.", commentId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = $"Comment not found."
                    };
                }

                if (comment.UserId != dto.UserId)
                {
                    _logger.LogWarning("You can't EntreLaunchdate a comment that you don't own.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = $"You can't EntreLaunchdate a comment that you don't own.",
                        Data = null
                    };
                }

                if (dto.Content != null)
                {
                    comment.Content = dto.Content;
                }

                comment.EntreLaunchdatedAt = DateTimeOffset.UtcNow;

                _dbContext.PostComments.EntreLaunchdate(comment);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Comment EntreLaunchdated successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error EntreLaunchdating comment with ID {CommentId}.", commentId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An error occurred while EntreLaunchdating the comment."
                };
            }
        }

        #endregion

        #region Read Operations (Get / Show)

        /// <inheritdoc />
        public async Task<GeneralResult> GetAllPostsAsync()
        {
            _logger.LogInformation("Start fetching all posts with full data...");

            try
            {
                var posts = await _dbContext.Posts
                    .AsNoTracking()
                    .Where(p => !p.IsDeleted)
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

                        Comments = p.PostComments!
                        .Where(c => !c.IsDeleted && c.Status == CommentStatus.Good)
                        .Select(c => new CommentDetailsDto
                        {
                            Content = c.Content,
                            Status = c.Status ?? CommentStatus.Good,
                            ParentCommentId = c.ParentCommentId ?? 0,
                            ParentName = c.ParentName,
                            CreatedAt = c.CreatedAt ?? DateTimeOffset.UtcNow,
                            User = new CommentUserData
                            {
                                FirstName = c.User.FirstName,
                                LastName = c.User.LastName
                            },
                        }).ToList(),

                        Likes = p.PostLikes!.Count(l => l.IsActive == true)
                    }).ToListAsync();

                if (!posts.Any())
                {
                    _logger.LogInformation("No posts found in the database.");
                    return new GeneralResult
                    {
                        IsSuccess = true,
                        Message = "No posts found.",
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "All posts fetched successfully.",
                    Data = posts
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all posts.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An error occurred while fetching all posts."
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> GetPostByIdAsync(int postId)
        {
            _logger.LogInformation("Start fetching post with Id={PostId}", postId);

            try
            {
                var post = await _dbContext.Posts
                    .AsNoTracking()
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

                        Comments = p.PostComments!
                        .Where(c => !c.IsDeleted && c.Status == CommentStatus.Good)
                        .Select(c => new CommentDetailsDto
                        {
                            Content = c.Content,
                            Status = c.Status ?? CommentStatus.Good,
                            ParentCommentId = c.ParentCommentId ?? 0,
                            ParentName = c.ParentName,
                            CreatedAt = c.CreatedAt ?? DateTimeOffset.UtcNow,
                            User = new CommentUserData
                            {
                                FirstName = c.User.FirstName,
                                LastName = c.User.LastName
                            },
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
                        Message = $"Post not found.",
                        Data = null
                    };
                }

                var postDto = _mapper.Map<PostDetailsDto>(post);

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Post fetched successfully.",
                    Data = postDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching post with Id={PostId}.", postId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An error occurred while fetching the post."
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
                        Message = $"Post not found."
                    };
                }

                var likeCount = await _dbContext.PostLikes
                    .CountAsync(l => l.PostId == postId && l.IsActive == true && !l.IsDeleted);

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Like count fetched successfully.",
                    Data = likeCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching like count for post with Id={PostId}.", postId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An error occurred while fetching the like count."
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> GetPostCommentsAsync(int postId)
        {
            _logger.LogInformation("Start fetching comments for post with Id={PostId}", postId);

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
                        Message = $"Post not found."
                    };
                }

                var comments = await _dbContext.PostComments
                    .Where(c => c.PostId == postId && !c.IsDeleted && c.Status == CommentStatus.Good)
                    .Select(c => new CommentDetailsDto
                    {
                        Content = c.Content,
                        Status = c.Status ?? CommentStatus.Good,
                        ParentCommentId = c.ParentCommentId ?? 0,
                        ParentName = c.ParentName,
                        CreatedAt = c.CreatedAt ?? DateTimeOffset.UtcNow,
                        User = new CommentUserData
                        {
                            FirstName = c.User.FirstName,
                            LastName = c.User.LastName
                        }
                    })
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                if (!comments.Any())
                {
                    _logger.LogWarning("Comments for post with ID {PostId} not found.", postId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = $"Comments not found.",
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Comments fetched successfully.",
                    Data = comments
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching comments for post with Id={PostId}.", postId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An error occurred while fetching the comments."
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
                        Message = $"Post not found."
                    };
                }

                var reports = await _dbContext.CommunityReports
                    .Where(r => r.PostId == postId && !r.IsDeleted)
                    .Select(r => new ReportDetailsDto
                    {
                        User = new ReportUserData
                        {
                            FirstName = r.User.FirstName,
                            LastName = r.User.LastName,
                            Email = r.User.Email
                        },
                        PostId = r.PostId,
                        CommentId = null,
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
                        Message = $"Reports not found.",
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Reports fetched successfully.",
                    Data = reports
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching reports for post with Id={PostId}.", postId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An error occurred while fetching the post reports.",
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
                        Message = $"Comment not found.",
                        Data = null
                    };
                }

                var reports = await _dbContext.CommunityReports
                    .Where(r => r.CommentId == commentId && !r.IsDeleted)
                    .Select(r => new ReportDetailsDto
                    {
                        User = new ReportUserData
                        {
                            FirstName = r.User.FirstName,
                            LastName = r.User.LastName,
                            Email = r.User.Email
                        },
                        PostId = r.PostId,
                        CommentId = null,
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
                        Message = $"Reports not found.",
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Comment reports fetched successfully.",
                    Data = reports
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching reports for comment with Id={CommentId}.", commentId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An error occurred while fetching the comment reports."
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> GetPendingPostsAsync()
        {
            _logger.LogInformation("Start fetching pending posts...");

            try
            {
                var posts = await _dbContext.Posts
                    .AsNoTracking()
                    .Where(p => !p.IsDeleted && p.Status == RequestStatus.Pending)
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

                        Media = p.PostMedias!.Where(m => !m.IsDeleted).Select(m => new PostMediaDetailsDto
                        {
                            Url = m.Url
                        }).ToList(),

                        Comments = p.PostComments!
                        .Where(c => !c.IsDeleted && c.Status == CommentStatus.Good)
                        .Select(c => new CommentDetailsDto
                        {
                            Content = c.Content,
                            Status = c.Status ?? CommentStatus.Good,
                            ParentCommentId = c.ParentCommentId ?? 0,
                            ParentName = c.ParentName,
                            CreatedAt = c.CreatedAt ?? DateTimeOffset.UtcNow,
                            User = new CommentUserData
                            {
                                FirstName = c.User.FirstName,
                                LastName = c.User.LastName
                            },
                        }).ToList(),

                        Likes = p.PostLikes!.Count(l => l.IsActive == true)
                    }).ToListAsync();

                if (!posts.Any() || posts == null)
                {
                    _logger.LogInformation("No pending posts found.");
                    return new GeneralResult
                    {
                        IsSuccess = true,
                        Message = "No pending posts found.",
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Pending posts fetched successfully.",
                    Data = posts
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pending posts.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An error occurred while fetching pending posts."
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> GetAcceptedPostsAsync()
        {
            _logger.LogInformation("Start fetching accepted posts...");

            try
            {
                var posts = await _dbContext.Posts
                    .AsNoTracking()
                    .Where(p => !p.IsDeleted && p.Status == RequestStatus.Accepted)
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

                        Comments = p.PostComments!
                        .Where(c => !c.IsDeleted && c.Status == CommentStatus.Good)
                        .Select(c => new CommentDetailsDto
                        {
                            Content = c.Content,
                            Status = c.Status ?? CommentStatus.Good,
                            ParentCommentId = c.ParentCommentId ?? 0,
                            ParentName = c.ParentName,
                            CreatedAt = c.CreatedAt ?? DateTimeOffset.UtcNow,
                            User = new CommentUserData
                            {
                                FirstName = c.User.FirstName,
                                LastName = c.User.LastName
                            },
                        }).ToList(),

                        Likes = p.PostLikes!.Count(l => l.IsActive == true)
                    }).ToListAsync();

                if (!posts.Any())
                {
                    _logger.LogInformation("No accepted posts found.");
                    return new GeneralResult
                    {
                        IsSuccess = true,
                        Message = "No accepted posts found.",
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Accepted posts fetched successfully.",
                    Data = posts
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching accepted posts.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An error occurred while fetching accepted posts."
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> GetRejectedPostsAsync()
        {
            _logger.LogInformation("Start fetching rejected posts...");

            try
            {
                var posts = await _dbContext.Posts
                    .AsNoTracking()
                    .Where(p => !p.IsDeleted && p.Status == RequestStatus.Rejected)
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

                        Comments = p.PostComments!
                        .Where(c => !c.IsDeleted && c.Status == CommentStatus.Good)
                        .Select(c => new CommentDetailsDto
                        {
                            Content = c.Content,
                            Status = c.Status ?? CommentStatus.Good,
                            ParentCommentId = c.ParentCommentId ?? 0,
                            ParentName = c.ParentName,
                            CreatedAt = c.CreatedAt ?? DateTimeOffset.UtcNow,
                            User = new CommentUserData
                            {
                                FirstName = c.User.FirstName,
                                LastName = c.User.LastName
                            },
                        }).ToList(),

                        Likes = p.PostLikes!.Count(l => l.IsActive == true)
                    }).ToListAsync();

                if (!posts.Any() || posts == null)
                {
                    _logger.LogInformation("No rejected posts found.");
                    return new GeneralResult
                    {
                        IsSuccess = true,
                        Message = "No rejected posts found.",
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Rejected posts fetched successfully.",
                    Data = posts
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching rejected posts.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An error occurred while fetching rejected posts."
                };
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
                            FirstName = r.User.FirstName,
                            LastName = r.User.LastName,
                            Email = r.User.Email
                        },
                        PostId = r.PostId,
                        CommentId = null,
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
                        Message = $"Accepted reports not found.",
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Accepted reports fetched successfully.",
                    Data = reports
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching accepted reports.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An error occurred while fetching accepted reports.",
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
                            FirstName = r.User.FirstName,
                            LastName = r.User.LastName,
                            Email = r.User.Email
                        },
                        PostId = r.PostId,
                        CommentId = null,
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
                        Message = "No pending reports found.",
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Pending reports fetched successfully.",
                    Data = reports
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pending reports.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An error occurred while fetching pending reports.",
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
                            FirstName = r.User.FirstName,
                            LastName = r.User.LastName,
                            Email = r.User.Email
                        },
                        PostId = r.PostId,
                        CommentId = null,
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
                        Message = "No rejected reports found.",
                        Data = null
                    };
                }

                var reportsDto = _mapper.Map<List<ReportDetailsDto>>(reports);

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Rejected reports fetched successfully.",
                    Data = reportsDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching rejected reports.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An error occurred while fetching rejected reports.",
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
                        Message = $"Post not found."
                    };
                }

                if (post.Status == status)
                {
                    _logger.LogWarning("Post with ID {PostId} already has the same status.", postId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = $"Post already has the same status."
                    };
                }

                post.Status = status;
                post.EntreLaunchdatedAt = DateTimeOffset.UtcNow;
                _dbContext.Posts.EntreLaunchdate(post);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Post status processed successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing post status. PostId={PostId}", postId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An error occurred while processing the post status.",
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
                        Message = $"Report not found."
                    };
                }

                if (report.Status == status)
                {
                    _logger.LogWarning("Report with ID {ReportId} already has the same status.", reportId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = $"Report already has the same status.",
                        Data = null
                    };
                }

                report.Status = status;
                report.EntreLaunchdatedAt = DateTimeOffset.UtcNow;
                _dbContext.CommunityReports.EntreLaunchdate(report);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Report status processed successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing report status. ReportId={ReportId}", reportId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An error occurred while processing the report status."
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
                        Message = $"Comment with ID {commentId} not found."
                    };
                }

                if (comment.Status == status)
                {
                    _logger.LogWarning("Comment with ID {CommentId} already has the same status.", commentId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = $"Comment already has the same status.",
                        Data = null
                    };
                }

                comment.Status = status;
                comment.EntreLaunchdatedAt = DateTimeOffset.UtcNow;

                _dbContext.PostComments.EntreLaunchdate(comment);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Comment status processed successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing comment status. CommentId={CommentId}", commentId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An error occurred while processing the comment status."
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
                        Message = $"Post not found.",
                        Data = null
                    };
                }

                if (!string.Equals(post.UserId, userId, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("User {UserId} is not the owner of post {PostId}", userId, postId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = $"User is not the owner of post.",
                        Data = null
                    };
                }

                // remove likes
                if (post.PostLikes != null && post.PostLikes.Any())
                {
                    foreach (var like in post.PostLikes)
                    {
                        like.IsDeleted = true;
                        like.EntreLaunchdatedAt = DateTimeOffset.UtcNow;
                    }
                }

                // remove comments
                if (post.PostComments != null && post.PostComments.Any())
                {
                    foreach (var comment in post.PostComments)
                    {
                        comment.IsDeleted = true;
                        comment.EntreLaunchdatedAt = DateTimeOffset.UtcNow;
                    }
                }

                // remove media
                if (post.PostMedias != null && post.PostMedias.Any())
                {
                    foreach (var media in post.PostMedias)
                    {
                        media.IsDeleted = true;
                        media.EntreLaunchdatedAt = DateTimeOffset.UtcNow;
                    }
                }

                // remove reports
                if (post.CommunityReports != null && post.CommunityReports.Any())
                {
                    foreach (var report in post.CommunityReports)
                    {
                        report.IsDeleted = true;
                        report.EntreLaunchdatedAt = DateTimeOffset.UtcNow;
                    }
                }

                // remove post
                post.IsDeleted = true;
                post.EntreLaunchdatedAt = DateTimeOffset.UtcNow;
                _dbContext.Posts.EntreLaunchdate(post);
                await _dbContext.SaveChangesAsync();
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = $"Post deleted successfully",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting post with Id={PostId} by user {UserId}.", postId, userId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An error occurred while deleting the post.",
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
                        Message = $"Comment not found.",
                        Data = null
                    };
                }

                if (!string.Equals(comment.UserId, userId, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("User {UserId} is not the owner of comment {CommentId}.", userId, commentId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "User is not the owner of comment.",
                        Data = null
                    };
                }

                if (comment.PostLikes != null && comment.PostLikes.Any())
                {
                    foreach (var like in comment.PostLikes)
                    {
                        like.IsDeleted = true;
                        like.EntreLaunchdatedAt = DateTimeOffset.UtcNow;
                    }
                }

                // remove post
                comment.IsDeleted = true;
                comment.EntreLaunchdatedAt = DateTimeOffset.UtcNow;
                _dbContext.PostComments.EntreLaunchdate(comment);
                await _dbContext.SaveChangesAsync();

                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = $"Comment deleted successfully",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting comment with Id={CommentId} by user {UserId}.", commentId, userId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An error occurred while deleting the comment.",
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
                        Message = $"Media not found.",
                        Data = null
                    };
                }

                if (!string.Equals(media.Post.UserId, userId, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("User {UserId} is not the owner of post {PostId} that holds media {MediaId}.", userId, media.PostId, mediaId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "User is not the owner of post that holds media.",
                        Data = null
                    };
                }

                media.IsDeleted = true;
                media.EntreLaunchdatedAt = DateTimeOffset.UtcNow;
                _dbContext.PostMedias.EntreLaunchdate(media);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Media with Id={MediaId} deleted successfully by user {UserId}.", mediaId, userId);

                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = $"Media deleted successfully",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting media with Id={MediaId} by user {UserId}.", mediaId, userId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An error occurred while deleting the post media.",
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
                        Message = $"Post report not found.",
                        Data = null
                    };
                }

                report.IsDeleted = true;
                report.EntreLaunchdatedAt = DateTimeOffset.UtcNow;
                _dbContext.CommunityReports.EntreLaunchdate(report);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Post report with Id={ReportId} deleted successfully.", reportId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Post report successfully",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting post report with Id={ReportId}.", reportId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An error occurred while deleting the post report.",
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
                        Message = $"Comment report not found.",
                        Data = null
                    };
                }

                report.IsDeleted = true;
                report.EntreLaunchdatedAt = DateTimeOffset.UtcNow;
                _dbContext.CommunityReports.EntreLaunchdate(report);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Comment report with Id={ReportId} deleted successfully.", reportId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Comment report successfully",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting comment report with Id={ReportId}.", reportId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An error occurred while deleting the comment report.",
                    Data = null
                };
            }
        }

        #endregion
    }
}

