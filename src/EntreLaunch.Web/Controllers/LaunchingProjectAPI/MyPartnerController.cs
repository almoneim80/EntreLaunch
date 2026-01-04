namespace EntreLaunch.Controllers.LaunchingProjectAPI
{
    [Authorize(Roles = AppRoles.MyPartnerRoles)]
    [Route("api/[controller]")]
    public class MyPartnerController(
        ILogger<MyPartnerController> logger,
        ISubscriptionService subscriptionService,
        ILocalizationManager localization,
        IMyPartnerService partnerFacade) : AuthenticatedController(localization)
    {
        private readonly IMyPartnerService _partnerFacade = partnerFacade;
        private readonly ILogger<MyPartnerController> _logger = logger;
        private readonly ISubscriptionService _subscriptionService = subscriptionService;

        /// <summary>
        /// Creates a new my partner project.
        /// </summary>
        [HttpPost("create-project")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyPartnerPermissions.Create)]
        public async Task<IActionResult> CreateProject([FromBody] MyPartnerCreateDto createDto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                createDto.UserId = CurrentUserId!;
                var result = await _partnerFacade.Projects.CreateProjectWithAttachments(createDto);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in CreateProject.");
                return this.UnexpectedError("creating project");
            }
        }

        /// <summary>
        /// Progress project status (Accepted, Rejected).
        /// </summary>
        [HttpPost("project-process")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyPartnerPermissions.ProgressProject)]
        public async Task<IActionResult> ProcessProject([FromBody] ProcessProjectsDto processDto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                var result = await _partnerFacade.Projects.ProgressProjects(processDto);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in ProgressProject.");
                return this.UnexpectedError("progress project");
            }
        }

        /// <summary>
        /// Filtering projects.
        /// </summary>
        [HttpPost("filtering")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyPartnerPermissions.Filter)]
        public async Task<IActionResult> Filtering([FromBody] FilterProjectsDto filterDto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _partnerFacade.Filtering.Filtering(filterDto);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in Filtering.");
                return this.UnexpectedError("filtering projects");
            }
        }

        ///// <summary>
        ///// Update project.
        ///// </summary>
        //[HttpPatch("update-project/{id}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[RequiredPermission(MyPartnerPermissions.Edit)]
        //public async Task<IActionResult> UpdateProject(int id, [FromBody] MyPartnerUpdateDto updateDto)
        //{
        //    try
        //    {
        //        var userCheck = CheckUserOrUnauthorized();
        //        if (userCheck != null) return userCheck;

        //        var result = await _partnerFacade.Projects.UpdateProject(id, updateDto);
        //        if (result.IsSuccess == false)
        //        {
        //            return BadRequest(result);
        //        }

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred in UpdateProject.");
        //        return this.UnexpectedError("updating project");
        //    }
        //}

        ///// <summary>
        ///// Update project attachments.
        ///// </summary>
        //[HttpPatch("update-attachments/{id}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[RequiredPermission(MyPartnerPermissions.Edit)]
        //public async Task<IActionResult> UpdateAttachments(int id, [FromBody] ProjectAttachmentUpdateDto updateDto)
        //{
        //    var userCheck = CheckUserOrUnauthorized();
        //    if (userCheck != null) return userCheck;

        //    var result = await _partnerFacade.Attachments.UpdateAttachments(id, updateDto);
        //    return result.ToActionResult(_logger, false);
        //}

        /// <summary>
        /// Retrieves a list of all partner projects registered in the system, regardless of status.
        /// Requires the user to have the 'ShowAll' permission.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> that may return:
        /// - 200 OK with a complete list of projects if the retrieval is successful.
        /// - 204 No Content if no projects are found in the system.
        /// - 400 Bad Request if the result is unsuccessful (e.g., service-level failure).
        /// - 401 Unauthorized if the user is not authenticated or lacks required access rights.
        /// - 404 Not Found if the endpoint or related resource is missing.
        /// - 500 Internal Server Error if an unhandled exception occurs during execution.
        /// </returns>
        [HttpGet("all-projects")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(MyPartnerPermissions.GetAll)]
        public async Task<IActionResult> AllProjects([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _partnerFacade.Projects.AllProjects(pagination, cancellationToken);
                if (!result.IsSuccess)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in AllProjects.");
                return this.UnexpectedError("getting all projects");
            }
        }

        /// <summary>
        /// Retrieves a list of partner projects that are currently pending approval or processing.
        /// Requires the user to have the 'ShowPending' permission.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> containing:
        /// - 200 OK with the pending project data if retrieval is successful.
        /// - 204 No Content if there are no pending projects.
        /// - 400 Bad Request if the service call fails or returns an invalid result.
        /// - 401 Unauthorized if the user lacks proper authentication or permissions.
        /// - 404 Not Found if the requested resource is not available.
        /// - 500 Internal Server Error if an unexpected exception occurs during the request.
        /// </returns>
        [HttpGet("project-pending")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyPartnerPermissions.GetPending)]
        public async Task<IActionResult> GetPendingprojects([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _partnerFacade.Projects.PendingProjects(pagination, cancellationToken);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetPendingprojects.");
                return this.UnexpectedError("getting pending projects");
            }
        }

        /// <summary>
        /// Retrieves a list of partner projects that have been marked as rejected.
        /// Requires the user to have the 'ShowRejected' permission.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> that may include:
        /// - 200 OK with the rejected project data if the operation is successful.
        /// - 204 No Content if there are no rejected projects to return.
        /// - 400 Bad Request if the service returns a failure result.
        /// - 401 Unauthorized if the user is not authenticated or lacks the necessary permission.
        /// - 404 Not Found if the requested resource is unavailable.
        /// - 500 Internal Server Error if an unexpected error occurs during execution.
        /// </returns>
        [HttpGet("project-accepted")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyPartnerPermissions.GetAccepted)]
        public async Task<IActionResult> GetAcceptedProjects([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _partnerFacade.Projects.AcceptedProjects(pagination, cancellationToken);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetAcceptedProjects.");
                return this.UnexpectedError("getting accepted projects");
            }
        }

        /// <summary>
        /// Get all rejected projects.
        /// </summary>
        [HttpGet("projects-rejected")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyPartnerPermissions.GetRejected)]
        public async Task<IActionResult> GetRejectedProjects([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _partnerFacade.Projects.RejectedProjects(pagination, cancellationToken);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetRejectedProjects.");
                return this.UnexpectedError("getting rejected projects");
            }
        }

        /// <summary>
        /// Get one project by its id.
        /// </summary>
        [HttpGet("get-one")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(MyPartnerPermissions.GetOne)]
        public async Task<IActionResult> GetOneProject([FromRoute] int id)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _partnerFacade.Projects.GetProjectById(id);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetOneProject.");
                return this.UnexpectedError("getting one project");
            }
        }

        /// <summary>
        /// Retrieves all attachments associated with a specific partner project by its identifier.
        /// Requires the user to have the 'ShowAttachment' permission.
        /// </summary>
        /// <param name="id">The unique identifier of the project whose attachments are to be retrieved.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> that may contain:
        /// - 200 OK with the list of attachments if retrieval is successful.
        /// - 204 No Content if the project exists but has no attachments.
        /// - 400 Bad Request if the retrieval operation fails (e.g., invalid ID or service error).
        /// - 401 Unauthorized if the user is not authenticated or lacks required permissions.
        /// - 404 Not Found if the project or its attachments could not be located.
        /// - 500 Internal Server Error if an unexpected error occurs during processing.
        /// </returns>
        [HttpGet("project-attachments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(MyPartnerPermissions.GetAttachment)]
        public async Task<IActionResult> GetProjectAttachments([FromRoute] int id)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _partnerFacade.Attachments.GetProjectAttachments(id);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetProjectAttachments.");
                return this.UnexpectedError("getting project attachments");
            }
        }

        /// <summary>
        /// Retrieves a complete list of partner activities available in the system.
        /// Requires the user to have the 'ShowAll' permission.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> that may contain:
        /// - 200 OK with the list of activities if the operation is successful.
        /// - 204 No Content if no activities are found.
        /// - 400 Bad Request if the result indicates failure (non-success status).
        /// - 401 Unauthorized if the user is not authenticated or lacks required permissions.
        /// - 404 Not Found if the target resource or endpoint is unavailable.
        /// - 500 Internal Server Error if an unexpected exception is encountered during execution.
        /// </returns>
        [HttpGet("all-activities")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyPartnerPermissions.GetAll)]
        public async Task<IActionResult> GetAllActivities()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _partnerFacade.Filtering.GetAllActivitiesAsync();
                if (!result.IsSuccess)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetAllActivities.");
                return this.UnexpectedError("get all activities");
            }
        }

        /// <summary>
        /// checks if user has access to my partner service.
        /// </summary>
        [HttpGet("can-access")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(GeneralResult<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CanAccess()
        {
            var userCheck = CheckUserOrUnauthorized();
            if (userCheck != null) return userCheck;

            var result = await _subscriptionService.HasActiveAccessAsync(CurrentUserId!, SubscriptionType.MyPartner);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}

