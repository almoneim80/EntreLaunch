namespace EntreLaunch.Web.Controllers.ServicesAPI
{
    [Authorize(Roles = AppRoles.MyCommunityRoles)]
    [Route("api/[controller]")]
    public class MyCommunityController(
        ILogger<MyCommunityController> logger,
        IExtendedBaseService extendedBaseService,
        ILocalizationManager localization,
        IMyCommunityService myCommunityService) : AuthenticatedController(localization)
    {
        private readonly IExtendedBaseService _extendedBaseService = extendedBaseService;
        private readonly IMyCommunityService _myCommunityService = myCommunityService;
        private readonly ILogger<MyCommunityController> _logger = logger;
        private readonly ILocalizationManager _localizationManager = localization;

        /// <summary>
        /// create post without media.
        /// </summary>
        [HttpPost("posts/text-only")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyCommunityPermissions.CreateTextPost)]
        public async Task<IActionResult> CreateTextPost([FromBody] TextPostCreateDto dto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                dto.UserId = CurrentUserId!;
                var result = await _myCommunityService.CreateTextPostAsync(dto);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in CreateTextPost.");
                return this.UnexpectedError("create text post");
            }
        }

        /// <summary>
        /// create post with media.
        /// </summary>
        [HttpPost("posts/with-media")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyCommunityPermissions.CreatePostWithMedia)]
        public async Task<IActionResult> CreatePostWithMedia([FromBody] PostWithMediaCreateDto dto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                dto.UserId = CurrentUserId!;
                var result = await _myCommunityService.CreatePostWithMediaAsync(dto);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result.Message);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in CreatePostWithMedia.");
                return this.UnexpectedError("create post with media");
            }
        }

        ///// <summary>
        ///// Add media to an existing post.
        ///// </summary>
        //[HttpPost("create-media/{postId}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[RequiredPermission(MyCommunityPermissions.CreateMediaToPost)]
        //public async Task<IActionResult> CreateMediaToPost([FromRoute] int postId, [FromBody] List<MediaCreateDto> mediaDtos)
        //{
        //    try
        //    {
        //        var userCheck = CheckUserOrUnauthorized();
        //        if (userCheck != null) return userCheck;

        //        var modelCheck = this.ValidateModelState(_logger);
        //        if (modelCheck != null) return modelCheck;

        //        if (mediaDtos.Count == 0)
        //        {
        //            return BadRequest(new { Message = "Media list is empty." });
        //        }

        //        var result = await _myCommunityService.CreateMediaToPostAsync(postId, mediaDtos, CurrentUserId!);
        //        if (result.IsSuccess != true)
        //        {
        //            return BadRequest(result);
        //        }

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred in CreateMediaToPost.");
        //        return this.UnexpectedError("create media to post");
        //    }
        //}

        ///// <summary>
        ///// create comment.
        ///// </summary>
        //[HttpPost("create-comment")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[RequiredPermission(MyCommunityPermissions.CreateComment)]
        //public async Task<IActionResult> CreateComment([FromBody] CommentCreateDto dto)
        //{
        //    try
        //    {
        //        var userCheck = CheckUserOrUnauthorized();
        //        if (userCheck != null) return userCheck;

        //        var modelCheck = this.ValidateModelState(_logger);
        //        if (modelCheck != null) return modelCheck;

        //        dto.UserId = CurrentUserId;
        //        var isValidPost = await dto.PostId.CheckIfEntityExistsAsync<Post>(_extendedBaseService, _logger, _localizationManager);
        //        if (isValidPost != null) return isValidPost;

        //        var result = await _myCommunityService.CreateCommentAsync(dto);
        //        if (result.IsSuccess == false)
        //        {
        //            return BadRequest(result);
        //        }

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred in CreateComment.");
        //        return this.UnexpectedError("create comment");
        //    }
        //}

        /// <summary>
        /// create post like.
        /// </summary>
        [HttpPost("posts/like")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyCommunityPermissions.CreateLike)]
        public async Task<IActionResult> CreateLike([FromBody] LikeCreateDto dto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                if (dto.PostId.HasValue)
                {
                    var postData = dto.PostId ?? default;
                    var isValidPost = await postData.CheckIfEntityExistsAsync<Post>(_extendedBaseService, _logger, _localizationManager);
                    if (isValidPost != null) return isValidPost;
                }

                if (dto.CommentId.HasValue)
                {
                    var commentData = dto.CommentId ?? default;
                    var isValidComment = await commentData.CheckIfEntityExistsAsync<PostComment>(_extendedBaseService, _logger, _localizationManager);
                    if (isValidComment != null) return isValidComment;
                }

                dto.UserId = CurrentUserId;
                var result = await _myCommunityService.CreateLikeAsync(dto);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in CreateLike.");
                return this.UnexpectedError("create like");
            }
        }

        /// <summary>
        /// create report.
        /// </summary>
        [HttpPost("posts/report")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyCommunityPermissions.CreateReport)]
        public async Task<IActionResult> CreateReport([FromBody] ReportCreateDto dto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                if (dto.PostId.HasValue)
                {
                    var postData = dto.PostId ?? default;
                    var isValidPost = await postData.CheckIfEntityExistsAsync<Post>(_extendedBaseService, _logger, _localizationManager);
                    if (isValidPost != null) return isValidPost;
                }

                if (dto.CommentId.HasValue)
                {
                    var commentData = dto.CommentId ?? default;
                    var isValidComment = await commentData.CheckIfEntityExistsAsync<PostComment>(_extendedBaseService, _logger, _localizationManager);
                    if (isValidComment != null) return isValidComment;
                }

                dto.UserId = CurrentUserId;
                var result = await _myCommunityService.CreateReportAsync(dto);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in CreateReport.");
                return this.UnexpectedError("create report");
            }
        }

        /***************Edit*****************/
        /***************Edit*****************/

        ///// <summary>
        /////  Update the text of a specific post (no media changes here).
        ///// </summary>
        //[HttpPatch("edit-post/{id}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[RequiredPermission(MyCommunityPermissions.UpdatePost)]
        //public async Task<IActionResult> UpdatePost(int id, [FromBody] PostUpdateDto dto)
        //{
        //    try
        //    {
        //        var userCheck = CheckUserOrUnauthorized();
        //        if (userCheck != null) return userCheck;

        //        var modelCheck = this.ValidateModelState(_logger);
        //        if (modelCheck != null) return modelCheck;

        //        var isPostValid = await id.CheckIfEntityExistsAsync<Post>(_extendedBaseService, _logger, _localizationManager);
        //        if (isPostValid != null) return isPostValid;

        //        dto.UserId = CurrentUserId;
        //        var result = await _myCommunityService.UpdatePostAsync(id, dto);
        //        if (result.IsSuccess == false)
        //        {
        //            return BadRequest(result);
        //        }

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred in UpdatePost.");
        //        return this.UnexpectedError("update post");
        //    }
        //}

        ///// <summary>
        ///// Update the media of a particular post.
        ///// </summary>
        //[HttpPatch("edit-media/{id}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[RequiredPermission(MyCommunityPermissions.UpdateMedia)]
        //public async Task<IActionResult> UpdateMedia(int id, [FromBody] MediaUpdateDto dto)
        //{
        //    try
        //    {
        //        var userCheck = CheckUserOrUnauthorized();
        //        if (userCheck != null) return userCheck;

        //        var modelCheck = this.ValidateModelState(_logger);
        //        if (modelCheck != null) return modelCheck;

        //        var isMediaValid = await id.CheckIfEntityExistsAsync<PostMedia>(_extendedBaseService, _logger, _localizationManager);
        //        if (isMediaValid != null) return isMediaValid;

        //        var result = await _myCommunityService.UpdateMediaAsync(id, dto);
        //        if (result.IsSuccess == false)
        //        {
        //            return BadRequest(result);
        //        }

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred in UpdateMedia.");
        //        return this.UnexpectedError("update media");
        //    }
        //}


        ///// <summary>
        ///// Update a specific comment.
        ///// </summary>
        //[HttpPatch("edit-comment/{id}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[RequiredPermission(MyCommunityPermissions.UpdateComment)]
        //public async Task<IActionResult> UpdateComment(int id, [FromBody] CommentUpdateDto dto)
        //{
        //    try
        //    {
        //        var userCheck = CheckUserOrUnauthorized();
        //        if (userCheck != null) return userCheck;

        //        var modelCheck = this.ValidateModelState(_logger);
        //        if (modelCheck != null) return modelCheck;
                
        //        var isCommentValid = await id.CheckIfEntityExistsAsync<PostComment>(_extendedBaseService, _logger, _localizationManager);
        //        if (isCommentValid != null) return isCommentValid;

        //        dto.UserId = CurrentUserId;
        //        var result = await _myCommunityService.UpdateCommentAsync(id, dto);
        //        if (result.IsSuccess == false)
        //        {
        //            return BadRequest(result);
        //        }

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred in UpdateComment.");
        //        return this.UnexpectedError("update comment");
        //    }
        //}

        /***************Show*****************/
        /***************Show*****************/

        /// <summary>
        /// Fetch all posts (with media, comments and likes).
        /// </summary>
        [HttpGet("get-all-posts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyCommunityPermissions.GetAllPosts)]
        public async Task<IActionResult> GetAllPosts([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myCommunityService.GetAllPostsAsync(pagination, cancellationToken);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetAllPosts.");
                return this.UnexpectedError("get all posts");
            }
        }

        /// <summary>
        /// View specific post information: (post + media + comments + likes) where post status is accapted.
        /// </summary>
        [HttpGet("get-post/{postId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyCommunityPermissions.GetPostById)]
        public async Task<IActionResult> GetPostById([FromRoute] int postId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myCommunityService.GetPostByIdAsync(postId);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetPostById.");
                return this.UnexpectedError("get post by id");
            }
        }

        /// <summary>
        /// show post likes count.
        /// </summary>
        [HttpGet("get-post-like-count/{postId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyCommunityPermissions.GetPostLikeCount)]
        public async Task<IActionResult> GetPostLikeCount([FromRoute] int postId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myCommunityService.GetPostLikeCountAsync(postId);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetPostLikeCount.");
                return this.UnexpectedError("get post like count");
            }
        }

        ///// <summary>
        ///// show post comments.
        ///// </summary>
        //[HttpGet("get-post-comments/{postId}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[RequiredPermission(MyCommunityPermissions.GetPostComments)]
        //public async Task<IActionResult> GetPostComments([FromRoute] int postId)
        //{
        //    try
        //    {
        //        var userCheck = CheckUserOrUnauthorized();
        //        if (userCheck != null) return userCheck;

        //        var result = await _myCommunityService.GetPostCommentsAsync(postId);
        //        if (result.IsSuccess == false)
        //        {
        //            return BadRequest(result);
        //        }

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred in GetPostComments.");
        //        return this.UnexpectedError("get post comments");
        //    }
        //}

        /// <summary>
        /// show post reports.
        /// </summary>
        [HttpGet("get-post-reports/{postId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyCommunityPermissions.GetPostReports)]
        public async Task<IActionResult> GetPostReports([FromRoute] int postId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myCommunityService.GetPostReportsAsync(postId);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetPostReports.");
                return this.UnexpectedError("get post reports");
            }
        }

        ///// <summary>
        ///// show comment reports.
        ///// </summary>
        //[HttpGet("get-comment-reports/{commentId}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[RequiredPermission(MyCommunityPermissions.GetCommentReports)]
        //public async Task<IActionResult> GetCommentReports([FromRoute] int commentId)
        //{
        //    try
        //    {
        //        var userCheck = CheckUserOrUnauthorized();
        //        if (userCheck != null) return userCheck;

        //        var result = await _myCommunityService.GetCommentReportsAsync(commentId);
        //        if (result.IsSuccess == false)
        //        {
        //            return BadRequest(result);
        //        }

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred in GetCommentReports.");
        //        return this.UnexpectedError("get comment reports");
        //    }
        //}

        /// <summary>
        /// show pending post.
        /// </summary>
        [HttpGet("get-pending-posts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyCommunityPermissions.GetPendingPosts)]
        public async Task<IActionResult> GetPendingPosts([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myCommunityService.GetPendingPostsAsync(pagination, cancellationToken);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetPendingPosts.");
                return this.UnexpectedError("get pending posts");
            }
        }

        /// <summary>
        /// show accepted post.
        /// </summary>
        [HttpGet("get-accepted-posts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyCommunityPermissions.GetAcceptedPosts)]
        public async Task<IActionResult> GetAcceptedPosts([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myCommunityService.GetAcceptedPostsAsync(pagination, cancellationToken);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetAcceptedPosts.");
                return this.UnexpectedError("get accepted posts");
            }
        }

        /// <summary>
        /// show rejected post.
        /// </summary>
        [HttpGet("get-rejected-posts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyCommunityPermissions.GetRejectedPosts)]
        public async Task<IActionResult> GetRejectedPosts([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myCommunityService.GetRejectedPostsAsync(pagination, cancellationToken);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetRejectedPosts.");
                return this.UnexpectedError("get rejected posts");
            }
        }

        /// <summary>
        /// show pending report.
        /// </summary>
        [HttpGet("get-pending-reports")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyCommunityPermissions.GetPendingReports)]
        public async Task<IActionResult> GetPendingReports()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _myCommunityService.GetPendingReportsAsync();
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetPendingReports.");
                return this.UnexpectedError("get pending reports");
            }
        }

        /// <summary>
        /// show accepted report.
        /// </summary>
        [HttpGet("get-accepted-reports")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyCommunityPermissions.GetAcceptedReports)]
        public async Task<IActionResult> GetAcceptedReports()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _myCommunityService.GetAcceptedReportsAsync();
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetAcceptedReports.");
                return this.UnexpectedError("get accepted reports");
            }
        }

        /// <summary>
        /// show rejected report.
        /// </summary>
        [HttpGet("get-rejected-reports")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyCommunityPermissions.GetRejectedReports)]
        public async Task<IActionResult> GetRejectedReports()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _myCommunityService.GetRejectedReportsAsync();
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetRejectedReports.");
                return this.UnexpectedError("get rejected reports");
            }
        }

        /***************process*****************/
        /***************process*****************/

        /// <summary>
        /// change status of post request.
        /// </summary>
        [HttpPatch("process-post-status/{postId}/{status}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyCommunityPermissions.ProcessPostStatus)]
        public async Task<IActionResult> ProcessPostStatus([FromRoute] int postId, [FromRoute] RequestStatus status)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myCommunityService.ProcessPostStatusAsync(postId, status);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in ProcessPostStatus.");
                return this.UnexpectedError("process post status");
            }
        }

        /// <summary>
        /// change status of report request.
        /// </summary>
        [HttpPatch("process-report-status/{reportId}/{status}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyCommunityPermissions.ProcessReportStatus)]
        public async Task<IActionResult> ProcessReportStatus([FromRoute] int reportId, [FromRoute] RequestStatus status)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myCommunityService.ProcessReportStatusAsync(reportId, status);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in ProcessReportStatus.");
                return this.UnexpectedError(" process report status");
            }
        }

        ///// <summary>
        ///// change status of comment (Good, Spam, Dangerous).
        ///// </summary>
        //[HttpPatch("process-comment-status/{commentId}/{status}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[RequiredPermission(MyCommunityPermissions.ProcessCommentStatus)]
        //public async Task<IActionResult> ProcessCommentStatus([FromRoute] int commentId, [FromRoute] CommentStatus status)
        //{
        //    try
        //    {
        //        var userCheck = CheckUserOrUnauthorized();
        //        if (userCheck != null) return userCheck;

        //        var result = await _myCommunityService.ProcessCommentStatusAsync(commentId, status);
        //        if (result.IsSuccess == false)
        //        {
        //            return BadRequest(result);
        //        }

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred in ProcessCommentStatus.");
        //        return this.UnexpectedError("process comment status");
        //    }
        //}

        /***************delete*****************/
        /***************delete*****************/

        /// <summary>
        /// Deleting a post (post + media + comments + likes) by the owner (User).
        /// </summary>
        [HttpDelete("delete-post/{postId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyCommunityPermissions.DeletePost)]
        public async Task<IActionResult> DeletePost([FromRoute] int postId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myCommunityService.DeletePostAsync(postId, CurrentUserId!);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in DeletePost.");
                return this.UnexpectedError("delete post");
            }
        }

        ///// <summary>
        ///// Delete a specific comment by the owner (User).
        ///// </summary>
        //[HttpDelete("delete-comment/{commentId}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[RequiredPermission(MyCommunityPermissions.DeleteComment)]
        //public async Task<IActionResult> DeleteComment([FromRoute] int commentId)
        //{
        //    try
        //    {
        //        var userCheck = CheckUserOrUnauthorized();
        //        if (userCheck != null) return userCheck;

        //        var result = await _myCommunityService.DeleteCommentAsync(commentId, CurrentUserId!);
        //        if (result.IsSuccess == false)
        //        {
        //            return BadRequest(result);
        //        }

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred in DeleteComment.");
        //        return this.UnexpectedError("delete comment");
        //    }
        //}

        /// <summary>
        /// Delete a media assigned to a post by the owner (User).
        /// </summary>
        [HttpDelete("delete-media/{mediaId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyCommunityPermissions.DeleteMedia)]
        public async Task<IActionResult> DeleteMedia([FromRoute] int mediaId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myCommunityService.DeleteMediaAsync(mediaId, CurrentUserId!);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in DeleteMedia.");
                return this.UnexpectedError("delete media");
            }
        }

        /// <summary>
        /// Delete a post's Report.
        /// </summary>
        [HttpDelete("delete-post-report/{reportId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyCommunityPermissions.DeletePostReport)]
        public async Task<IActionResult> DeletePostReport([FromRoute] int reportId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myCommunityService.DeletePostReportAsync(reportId);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in DeletePostReport.");
                return this.UnexpectedError("delete post report");
            }
        }

        ///// <summary>
        ///// delete comment's report.
        ///// </summary>
        //[HttpDelete("delete-comment-report/{reportId}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[RequiredPermission(MyCommunityPermissions.DeleteCommentReport)]
        //public async Task<IActionResult> DeleteCommentReport([FromRoute] int reportId)
        //{
        //    try
        //    {
        //        var userCheck = CheckUserOrUnauthorized();
        //        if (userCheck != null) return userCheck;

        //        var result = await _myCommunityService.DeleteCommentReportAsync(reportId);
        //        if (result.IsSuccess == false)
        //        {
        //            return BadRequest(result);
        //        }

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred in DeleteCommentReport.");
        //        return this.UnexpectedError("delete comment report");
        //    }
        //}
    }
}
