namespace EntreLaunch.Web.Controllers.BlogAPI
{
    [Authorize(Roles = AppRoles.BlogRoles)]
    [Route("api/[controller]")]
    public class BlogController(
        ILogger<BlogController> logger,
        IBlogService blogService,
        FileValidatorHelper fileValidator,
        ILocalizationManager localization) : AuthenticatedController(localization)
    {
        private readonly IBlogService _blogService = blogService;
        //private readonly IFileStorageService _fileStorage = fileStorage;
        private readonly ILogger<BlogController> _logger = logger;
        private readonly ILocalizationManager _localizationManager = localization;
        private readonly FileValidatorHelper _fileValidator = fileValidator;

        /// <summary>
        /// Creates a new blog entry with media support.
        /// </summary>
        /// <param name="title">The title of the blog post.</param>
        /// <param name="details">The body/content of the blog post.</param>
        /// <param name="mediaFile">An optional media file (image/video) to be attached to the blog.</param>
        /// <param name="cancellationToken">Token to cancel the request if needed.</param>
        /// <returns>
        /// - 200 OK: Blog created successfully.
        /// - 400 BadRequest: Validation failed or saving failed.
        /// - 401 Unauthorized: User not authenticated.
        /// - 500 InternalServerError: Unexpected error.
        /// </returns>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(BlogPermissions.Create)]
        public async Task<IActionResult> CreateBlog([FromForm] string title, [FromForm] string details, IFormFile mediaFile, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(details) || mediaFile == null)
                {
                    return this.UnexpectedError(_localizationManager.GetLocalizedString("AllDataIsRequired"));
                }

                var fileResult = mediaFile.PrepareValidatedFile(Enums.MediaType.Image, _fileValidator);
                if (!fileResult.IsValid)
                {
                    return BadRequest(new GeneralResult(false, fileResult.ErrorMessage!));
                }

                //await _fileStorage.UploadFileAsync(uniqueName, "blogs"); // "blogs" مجلد داخل الـ bucket

                var dto = new BlogCreateDto
                {
                    Title = title,
                    Details = details,
                    Media = fileResult.UniqueName,
                    UserId = CurrentUserId!,
                    CreatedAt = DateTimeOffset.UtcNow,
                    Status = BlogStatus.Pending
                };

                var result = await _blogService.CreateBlogAsync(dto, cancellationToken);
                if (result.IsSuccess == false) return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateBlog");
                return this.UnexpectedError("create blog");
            }
        }

        /// <summary>
        /// Updates the processing status of a specific blog entry identified by its ID.
        /// </summary>
        /// <param name="blogId">The unique identifier of the blog to update.</param>
        /// <param name="status">The new status to apply to the blog (e.g., Approved, Rejected, Pending).</param>
        /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> that includes:
        /// - 200 OK if the status update is successful.
        /// - 400 Bad Request if the operation fails (e.g., invalid blog ID or status).
        /// - 401 Unauthorized if the user is not authenticated or lacks necessary permissions.
        /// - 500 Internal Server Error if an unexpected error occurs during the update process.
        /// </returns>
        [HttpPatch("process-status/{blogId}/{status}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(BlogPermissions.ProcessStatus)]
        public async Task<IActionResult> ProcessStatus([FromRoute] int blogId, [FromRoute] BlogStatus status, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _blogService.ProcessStatus(blogId, status, cancellationToken);
                if (result.IsSuccess == false) return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ProcessStatus");
                return this.UnexpectedError("process blog status");
            }
        }

        /// <summary>
        /// Retrieves a list of all blog entries stored in the system.
        /// </summary>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(BlogPermissions.GetAll)]
        public async Task<IActionResult> GetAllBlogs([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _blogService.AllBlogsAsync(pagination, cancellationToken);
                if (!result.IsSuccess) return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllBlogs");
                return this.UnexpectedError("get all blogs");
            }
        }

        /// <summary>
        /// Retrieves a single accepted blog by its ID.
        /// </summary>
        /// <param name="blogId">The unique identifier of the blog.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>
        /// - 200 OK: Blog found and returned.
        /// - 404 Not Found: Blog not found or not accepted.
        /// - 401 Unauthorized: User not authenticated.
        /// - 500 InternalServerError: Unexpected error.
        /// </returns>
        [HttpGet("{blogId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(BlogPermissions.GetOne)]
        public async Task<IActionResult> GetAcceptedBlogById([FromRoute] int blogId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _blogService.GetByIdAsync(blogId, cancellationToken);
                if (!result.IsSuccess || result.Data is null)
                    return NotFound(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAcceptedBlogById");
                return this.UnexpectedError("get blog by id");
            }
        }

        /// <summary>
        /// Retrieves all blog entries authored by a specific user.
        /// </summary>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> that includes:
        /// - 200 OK with the user's blogs if retrieval is successful.
        /// - 404 Not Found if no blogs exist for the specified user or the operation fails.
        /// - 401 Unauthorized if the user is not authenticated or lacks the required permission.
        /// - 500 Internal Server Error if an unexpected error occurs during processing.
        /// </returns>
        [HttpGet("user-blogs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(BlogPermissions.MyBlogs)]
        public async Task<IActionResult> GetUserBlogs(CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _blogService.UserBlogsAsync(CurrentUserId!, cancellationToken);
                if (!result.IsSuccess) return NotFound(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetUserBlogs");
                return this.UnexpectedError("get user blogs");
            }
        }

        /// <summary>
        /// Retrieves all blog entries that match the specified publication status.
        /// </summary>
        [HttpGet("status/{status}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(BlogPermissions.ViewByStatus)]
        public async Task<IActionResult> GetBlogsByStatus([FromRoute] BlogStatus status, [FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _blogService.BlogsByStatusAsync(status, pagination, cancellationToken);
                if (!result.IsSuccess)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetBlogsByStatus.");
                return this.UnexpectedError("get blogs by status");
            }
        }

        /// <summary>
        /// Deletes a blog entry identified by its unique ID.
        /// </summary>
        /// <param name="blogId">The unique identifier of the blog to be deleted.</param>
        /// <param name="cancellationToken">A token to observe for cancellation requests during the deletion process.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> that includes:
        /// - 200 OK if the blog was successfully deleted.
        /// - 400 Bad Request if the deletion operation fails (e.g., blog not found or invalid ID).
        /// - 401 Unauthorized if the user is not authenticated or does not have deletion permissions.
        /// - 500 Internal Server Error if an unexpected error occurs during the deletion process.
        /// </returns>
        [HttpDelete("delete/{blogId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(BlogPermissions.Delete)]
        public async Task<IActionResult> DeleteBlog([FromRoute] int blogId, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _blogService.DeleteBlogAsync(blogId, cancellationToken);
                if (result.IsSuccess == false) return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteBlog");
                return this.UnexpectedError("delete blog");
            }
        }
    }
}
