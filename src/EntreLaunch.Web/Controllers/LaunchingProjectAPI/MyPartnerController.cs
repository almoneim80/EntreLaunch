namespace EntreLaunch.Controllers.LaunchingProjectAPI
{
    [Authorize(Roles = "Admin, Entrepreneur")]
    [Route("api/[controller]")]
    public class MyPartnerController : AuthenticatedController
    {
        private readonly IMyPartnerService _partnerFacade;
        private readonly ILogger<MyPartnerController> _logger;
        public MyPartnerController(
            ILogger<MyPartnerController> logger,
            IMyPartnerService partnerFacade)
        {
            _logger = logger;
            _partnerFacade = partnerFacade;
        }

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
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while creating project", Data = null });
            }
        }

        /// <summary>
        /// Progress project status (Accepted, Rejected).
        /// </summary>
        [HttpPost("project-progress")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyPartnerPermissions.ProgressProject)]
        public async Task<IActionResult> ProgressProject([FromBody] ProcessProjectsDto processDto)
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
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while progress project", Data = null });
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

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

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
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while filtering projects", Data = null });
            }
        }

        /// <summary>
        /// EntreLaunchdate project.
        /// </summary>
        [HttpPatch("EntreLaunchdate-project/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyPartnerPermissions.Edit)]
        public async Task<IActionResult> EntreLaunchdateProject(int id, [FromBody] MyPartnerEntreLaunchdateDto EntreLaunchdateDto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _partnerFacade.Projects.EntreLaunchdateProject(id, EntreLaunchdateDto);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in EntreLaunchdateProject.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while EntreLaunchdating project", Data = null });
            }
        }

        /// <summary>
        /// EntreLaunchdate project attachments.
        /// </summary>
        [HttpPatch("EntreLaunchdate-attachments/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyPartnerPermissions.Edit)]
        public async Task<IActionResult> EntreLaunchdateAttachments(int id, [FromBody] ProjectAttachmentEntreLaunchdateDto EntreLaunchdateDto)
        {
            var userCheck = CheckUserOrUnauthorized();
            if (userCheck != null) return userCheck;

            var result = await _partnerFacade.Attachments.EntreLaunchdateAttachments(id, EntreLaunchdateDto);
            return result.ToActionResult(_logger, false);
        }

        /// <summary>
        /// Returns all my partner projects.
        /// </summary>
        [HttpGet("all-projects")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(MyPartnerPermissions.ShowAll)]
        public async Task<IActionResult> AllProjects()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _partnerFacade.Projects.AllProjects();
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in AllProjects.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while getting all projects", Data = null });
            }
        }

        /// <summary>
        /// Get all pending projects.
        /// </summary>
        [HttpGet("project-pending")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyPartnerPermissions.ShowPending)]
        public async Task<IActionResult> GetPendingprojects()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _partnerFacade.Projects.PendingProjects();
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetPendingprojects.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while getting pending projects", Data = null });
            }
        }

        /// <summary>
        /// Get all accepted projects.
        /// </summary>
        [HttpGet("project-accepted")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyPartnerPermissions.ShowAccepted)]
        public async Task<IActionResult> GetAcceptedProjects()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _partnerFacade.Projects.AcceptedProjects();
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetAcceptedProjects.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while getting accepted projects", Data = null });
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
        [RequiredPermission(MyPartnerPermissions.ShowRejected)]
        public async Task<IActionResult> GetRejectedProjects()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _partnerFacade.Projects.RejectedProjects();
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetRejectedProjects.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while getting rejected projects", Data = null });
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
        [RequiredPermission(MyPartnerPermissions.ShowOne)]
        public async Task<IActionResult> GetOneProject(int id)
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
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while getting one project", Data = null });
            }
        }

        /// <summary>
        /// Get project attachments.
        /// </summary>
        [HttpGet("project-attachments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(MyPartnerPermissions.ShowAttachment)]
        public async Task<IActionResult> GetProjectAttachments(int id)
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
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while getting project attachments", Data = null });
            }
        }
    }
}

