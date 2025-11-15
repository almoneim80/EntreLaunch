namespace EntreLaunch.Web.Controllers.ServicesAPI
{
    [Authorize(Roles = "Admin, SubAdmin, Entrepreneur")]
    [Route("api/[controller]")]
    public class MyCommunityController : AuthenticatedController
    {
        private readonly IExtendedBaseService _extendedBaseService;
        private readonly IMyCommunityService _myCommunityService;
        private readonly ILogger<MyCommunityController> _logger;
        public MyCommunityController(
            ILogger<MyCommunityController> logger,
            IExtendedBaseService extendedBaseService,
            IMyCommunityService myCommunityService)
        {
            _extendedBaseService = extendedBaseService;
            _myCommunityService = myCommunityService;
            _logger = logger;
        }

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
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while create text post", Data = null });
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
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while create post with media", Data = null });
            }
        }

        /// <summary>
        /// Add media to an existing post.
        /// </summary>
        [HttpPost("create-media/{postId}/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyCommunityPermissions.CreateMediaToPost)]
        public async Task<IActionResult> CreateMediaToPost([FromRoute] int postId, [FromBody] List<MediaCreateDto> mediaDtos)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                if (mediaDtos.Count == 0)
                {
                    return BadRequest(new { Message = "Media list is empty." });
                }

                var result = await _myCommunityService.CreateMediaToPostAsync(postId, mediaDtos, CurrentUserId!);
                if (result.IsSuccess != true)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in CreateMediaToPost.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while create media to post", Data = null });
            }
        }

        /// <summary>
        /// create comment.
        /// </summary>
        [HttpPost("create-comment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyCommunityPermissions.CreateComment)]
        public async Task<IActionResult> CreateComment([FromBody] CommentCreateDto dto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                dto.UserId = CurrentUserId;
                var isValidPost = await dto.PostId.CheckIfEntityExistsAsync<Post>(_extendedBaseService, _logger);
                if (isValidPost != null) return isValidPost;

                var result = await _myCommunityService.CreateCommentAsync(dto);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in CreateComment.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while create comment", Data = null });
            }
        }

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
                    var isValidPost = await postData.CheckIfEntityExistsAsync<Post>(_extendedBaseService, _logger);
                    if (isValidPost != null) return isValidPost;
                }

                if (dto.CommentId.HasValue)
                {
                    var commentData = dto.CommentId ?? default;
                    var isValidComment = await commentData.CheckIfEntityExistsAsync<PostComment>(_extendedBaseService, _logger);
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
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while create like", Data = null });
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
                    var isValidPost = await postData.CheckIfEntityExistsAsync<Post>(_extendedBaseService, _logger);
                    if (isValidPost != null) return isValidPost;
                }

                if (dto.CommentId.HasValue)
                {
                    var commentData = dto.CommentId ?? default;
                    var isValidComment = await commentData.CheckIfEntityExistsAsync<PostComment>(_extendedBaseService, _logger);
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
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while create report", Data = null });
            }
        }


        /***************Edit*****************/
        /***************Edit*****************/

        /// <summary>
        ///  EntreLaunchdate the text of a specific post (no media changes here).
        /// </summary>
        [HttpPatch("edit-post/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyCommunityPermissions.EntreLaunchdatePost)]
        public async Task<IActionResult> EntreLaunchdatePost(int id, [FromBody] PostEntreLaunchdateDto dto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                var isPostValid = await id.CheckIfEntityExistsAsync<Post>(_extendedBaseService, _logger);
                if (isPostValid != null) return isPostValid;

                dto.UserId = CurrentUserId;
                var result = await _myCommunityService.EntreLaunchdatePostAsync(id, dto);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in EntreLaunchdatePost.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while EntreLaunchdate post", Data = null });
            }
        }


        /// <summary>
        /// EntreLaunchdate the media of a particular post.
        /// </summary>
        [HttpPatch("edit-media/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyCommunityPermissions.EntreLaunchdateMedia)]
        public async Task<IActionResult> EntreLaunchdateMedia(int id, [FromBody] MediaEntreLaunchdateDto dto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                var isMediaValid = await id.CheckIfEntityExistsAsync<PostMedia>(_extendedBaseService, _logger);
                if (isMediaValid != null) return isMediaValid;

                var result = await _myCommunityService.EntreLaunchdateMediaAsync(id, dto);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in EntreLaunchdateMedia.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while EntreLaunchdate media", Data = null });
            }
        }


        /// <summary>
        /// EntreLaunchdate a specific comment.
        /// </summary>
        [HttpPatch("edit-comment/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyCommunityPermissions.EntreLaunchdateComment)]
        public async Task<IActionResult> EntreLaunchdateComment(int id, [FromBody] CommentEntreLaunchdateDto dto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;
                
                var isCommentValid = await id.CheckIfEntityExistsAsync<PostComment>(_extendedBaseService, _logger);
                if (isCommentValid != null) return isCommentValid;

                dto.UserId = CurrentUserId;
                var result = await _myCommunityService.EntreLaunchdateCommentAsync(id, dto);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in EntreLaunchdateComment.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while EntreLaunchdate comment", Data = null });
            }
        }


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
        public async Task<IActionResult> GetAllPosts()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myCommunityService.GetAllPostsAsync();
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetAllPosts.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while get all posts", Data = null });
            }
        }


        /// <summary>
        /// View specific post information: (post + media + comments + likes).
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
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while get post by id", Data = null });
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
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while get post like count", Data = null });
            }
        }

        /// <summary>
        /// show post comments.
        /// </summary>
        [HttpGet("get-post-comments/{postId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyCommunityPermissions.GetPostComments)]
        public async Task<IActionResult> GetPostComments([FromRoute] int postId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myCommunityService.GetPostCommentsAsync(postId);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetPostComments.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while get post comments", Data = null });
            }
        }


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
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while get post reports", Data = null });
            }
        }

        /// <summary>
        /// show comment reports.
        /// </summary>
        [HttpGet("get-comment-reports/{commentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyCommunityPermissions.GetCommentReports)]
        public async Task<IActionResult> GetCommentReports([FromRoute] int commentId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myCommunityService.GetCommentReportsAsync(commentId);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetCommentReports.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while get comment reports", Data = null });
            }
        }


        /// <summary>
        /// show pending post.
        /// </summary>
        [HttpGet("get-pending-posts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyCommunityPermissions.GetPendingPosts)]
        public async Task<IActionResult> GetPendingPosts()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myCommunityService.GetPendingPostsAsync();
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetPendingPosts.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while get pending posts", Data = null });
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
        public async Task<IActionResult> GetAcceptedPosts()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myCommunityService.GetAcceptedPostsAsync();
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetAcceptedPosts.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while get accepted posts", Data = null });
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
        public async Task<IActionResult> GetRejectedPosts()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myCommunityService.GetRejectedPostsAsync();
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetRejectedPosts.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while get rejected posts", Data = null });
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
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while get pending reports", Data = null });
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
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while get accepted reports", Data = null });
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
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while get rejected reports", Data = null });
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
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while process post status", Data = null });
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
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while process report status", Data = null });
            }
        }

        /// <summary>
        /// change status of comment (Good, Spam, Dangerous).
        /// </summary>
        [HttpPatch("process-comment-status/{commentId}/{status}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyCommunityPermissions.ProcessCommentStatus)]
        public async Task<IActionResult> ProcessCommentStatus([FromRoute] int commentId, [FromRoute] CommentStatus status)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myCommunityService.ProcessCommentStatusAsync(commentId, status);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in ProcessCommentStatus.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while process comment status", Data = null });
            }
        }

        /***************delete*****************/
        /***************delete*****************/

        /// <summary>
        /// Deleting a post (post + media + comments + likes) by the owner (User).
        /// </summary>
        [HttpDelete("delete-post/{postId}/{userId}")]
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
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while delete post", Data = null });
            }
        }

        /// <summary>
        /// Delete a specific comment by the owner (User).
        /// </summary>
        [HttpDelete("delete-comment/{commentId}/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyCommunityPermissions.DeleteComment)]
        public async Task<IActionResult> DeleteComment([FromRoute] int commentId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myCommunityService.DeleteCommentAsync(commentId, CurrentUserId!);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in DeleteComment.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while delete comment", Data = null });
            }
        }

        /// <summary>
        /// Delete a media assigned to a post by the owner (User).
        /// </summary>
        [HttpDelete("delete-media/{mediaId}/{userId}")]
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
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while delete media", Data = null });
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
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while delete post report", Data = null });
            }
        }

        /// <summary>
        /// delete comment's report.
        /// </summary>
        [HttpDelete("delete-comment-report/{reportId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyCommunityPermissions.DeleteCommentReport)]
        public async Task<IActionResult> DeleteCommentReport([FromRoute] int reportId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myCommunityService.DeleteCommentReportAsync(reportId);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in DeleteCommentReport.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while delete comment report", Data = null });
            }
        }
    }
}
