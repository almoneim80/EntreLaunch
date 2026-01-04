namespace EntreLaunch.Controllers.LaunchingProjectAPI
{
    [Authorize(Roles = AppRoles.MyTeamRoles)]
    [Route("api/[controller]")]
    [ApiController]
    public class MyTeamController(
        IMyTeamService myTeamService,
        ILocalizationManager localization,
        ISubscriptionService subscriptionService,
        ILogger<MyTeamController> logger) : AuthenticatedController(localization)
    {
        private readonly IMyTeamService _myTeamService = myTeamService;
        private readonly ILogger<MyTeamController> _logger = logger;
        private readonly ISubscriptionService _subscriptionService = subscriptionService;

        /// <summary>
        /// create new Employee request.
        /// </summary>
        [HttpPost("create-employee-request")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyTeamPermissions.Create)]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeCreateDto createDto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                createDto.UserId = CurrentUserId;
                var result = await _myTeamService.CreateEmployeeWithPortfolio(createDto);
                if (result.IsSuccess == false)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in CreateEmployee.");
                return this.UnexpectedError("creating employee");
            }
        }

        /// <summary>
        /// change status of Employee request.
        /// </summary>
        [HttpPatch("process-employee-request")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyTeamPermissions.ChangeStatus)]
        public async Task<IActionResult> ChangeStatus(EmployeeRequestDto employeeRequestDto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myTeamService.ProcessEmployeeRequestStatus(employeeRequestDto);
                if (result.IsSuccess == false) return BadRequest(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in ChangeStatus.");
                return this.UnexpectedError("processing employee request");
            }
        }

        ///// <summary>
        ///// update employee data.
        ///// </summary>
        //[HttpPatch("edit-employee/{employeeId}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[RequiredPermission(MyTeamPermissions.UpdateEmployee)]
        //public async Task<IActionResult> UpdateEmployee(int employeeId, [FromBody] EmployeeUpdateDto updateDto)
        //{
        //    try
        //    {
        //        var userCheck = CheckUserOrUnauthorized();
        //        if (userCheck != null) return userCheck;

        //        var result = await _myTeamService.UpdateEmployee(employeeId, updateDto);
        //        if (result.IsSuccess == false) return BadRequest(result);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred in UpdateEmployee.");
        //        return this.UnexpectedError("updating employee");
        //    }
        //}

        ///// <summary>
        ///// update portfolio data.
        ///// </summary>
        //[HttpPatch("edit-portfolio/{portfolioId}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[RequiredPermission(MyTeamPermissions.UpdatePortfolio)]
        //public async Task<IActionResult> UpdatePortfolio(int portfolioId, [FromBody] EmployeePortfolioUpdateDto updateDto)
        //{
        //    try
        //    {
        //        var userCheck = CheckUserOrUnauthorized();
        //        if (userCheck != null) return userCheck;

        //        var result = await _myTeamService.UpdateEmployeePortfolio(portfolioId, updateDto);
        //        if (result.IsSuccess == false) return BadRequest(result);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred in UpdatePortfolio.");
        //        return this.UnexpectedError("updating portfolio");
        //    }
        //}

        ///// <summary>
        ///// update portfolio attachment data.
        ///// </summary>
        //[HttpPatch("edit-attachments/{attachmentId}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[RequiredPermission(MyTeamPermissions.UpdatePortfolioAttachment)]
        //public async Task<IActionResult> UpdatePortfolioAttachment(int attachmentId, [FromBody] PortfolioAttachmentUpdateDto updateDto)
        //{
        //    try
        //    {
        //        var userCheck = CheckUserOrUnauthorized();
        //        if (userCheck != null) return userCheck;

        //        var result = await _myTeamService.UpdatePortfolioAttachment(attachmentId, updateDto);
        //        if (result.IsSuccess == false) return BadRequest(result);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred in UpdatePortfolioAttachment.");
        //        return this.UnexpectedError("updating portfolio attachment");
        //    }
        //}

        /// <summary>
        /// show all Employee request.
        /// </summary>
        [HttpGet("all-employees")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyTeamPermissions.GetAll)]
        public async Task<IActionResult> GetAllEmployees([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myTeamService.AllEmployeeRequest(pagination, cancellationToken);
                if (result.IsSuccess == false) return NotFound(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetAllEmployees.");
                return this.UnexpectedError("getting all employees");
            }
        }

        /// <summary>
        /// show pending requests.
        /// </summary>
        [HttpGet("pending-employees")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyTeamPermissions.PendingEmployees)]
        public async Task<IActionResult> PendingEmployees([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myTeamService.PendingEmployees(pagination, cancellationToken);
                if (result.IsSuccess == false) return BadRequest(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in PendingEmployees.");
                return this.UnexpectedError("getting pending employees");
            }
        }

        /// <summary>
        /// show accepted requests.
        /// </summary>
        [HttpGet("accepted-employees")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyTeamPermissions.AcceptedEmployees)]
        public async Task<IActionResult> AcceptedEmployees([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myTeamService.AcceptedEmployees(pagination, cancellationToken);
                if (result.IsSuccess == false) return BadRequest(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in AcceptedEmployees.");
                return this.UnexpectedError("getting accepted employees");
            }
        }

        /// <summary>
        /// show rejected requests.
        /// </summary>
        [HttpGet("rejected-employees")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyTeamPermissions.RejectedEmployees)]
        public async Task<IActionResult> RejectedEmployees([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myTeamService.RejectedEmployees(pagination, cancellationToken);
                if (result.IsSuccess == false) return BadRequest(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in RejectedEmployees.");
                return this.UnexpectedError("getting rejected employees");
            }
        }

        /// <summary>
        /// show filtered accepted requests by work field.
        /// </summary>
        [HttpGet("filter-accepted-by-field")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyTeamPermissions.Filtering)]
        public async Task<IActionResult> FilterAcceptedByWorkField([FromQuery] string workField)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myTeamService.FilterAcceptedByWorkField(workField);
                if (result.IsSuccess == false) return BadRequest(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in FilterAcceptedByWorkField.");
                return this.UnexpectedError("filtering accepted employees by work field");
            }
        }

        /// <summary>
        /// show employee by id.
        /// </summary>
        [HttpGet("get-employee/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyTeamPermissions.GetEmployeeById)]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myTeamService.GetEmployeeById(id);
                if (result.IsSuccess == false) return BadRequest(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetEmployeeById.");
                return this.UnexpectedError("getting employee by id");
            }
        }

        /// <summary>
        /// show portfolios by employee id.
        /// </summary>
        [HttpGet("get-portfolios-for-employee/{employeeId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyTeamPermissions.GetPortfoliosForEmployee)]
        public async Task<IActionResult> GetPortfoliosForEmployee(int employeeId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myTeamService.GetPortfoliosByEmployeeId(employeeId);
                if (result.IsSuccess == false) return BadRequest(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetPortfoliosForEmployee.");
                return this.UnexpectedError("getting portfolios for employee");
            }
        }

        /// <summary>
        /// checks if user has access to my team service.
        /// </summary>
        [HttpGet("can-access")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(GeneralResult<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CanAccess()
        {
            var userCheck = CheckUserOrUnauthorized();
            if (userCheck != null) return userCheck;

            var result = await _subscriptionService.HasActiveAccessAsync(CurrentUserId!, SubscriptionType.MyTeam);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
