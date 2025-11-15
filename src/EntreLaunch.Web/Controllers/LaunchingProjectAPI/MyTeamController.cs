namespace EntreLaunch.Controllers.LaunchingProjectAPI
{
    [Authorize(Roles = "Admin, Entrepreneur")]
    [Route("api/[controller]")]
    [ApiController]
    public class MyTeamController : AuthenticatedController
    {
        private readonly IMyTeamService _myTeamService;
        private readonly ILogger<MyTeamController> _logger;
        public MyTeamController(IMyTeamService myTeamService, ILogger<MyTeamController> logger)
        {
            _myTeamService = myTeamService;
            _logger = logger;
        }

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

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                var result = await _myTeamService.CreateEmployeeWithPortfolio(createDto);
                if (result.IsSuccess == false)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in CreateEmployee.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while creating employee", Data = null });
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
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while processing employee request", Data = null });
            }
        }

        /// <summary>
        /// EntreLaunchdate employee data.
        /// </summary>
        [HttpPatch("edit-employee/{employeeId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyTeamPermissions.EntreLaunchdateEmployee)]
        public async Task<IActionResult> EntreLaunchdateEmployee(int employeeId, [FromBody] EmployeeEntreLaunchdateDto EntreLaunchdateDto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myTeamService.EntreLaunchdateEmployee(employeeId, EntreLaunchdateDto);
                if (result.IsSuccess == false) return BadRequest(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in EntreLaunchdateEmployee.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while EntreLaunchdating employee", Data = null });
            }
        }

        /// <summary>
        /// EntreLaunchdate portfolio data.
        /// </summary>
        [HttpPatch("edit-portfolio/{portfolioId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyTeamPermissions.EntreLaunchdatePortfolio)]
        public async Task<IActionResult> EntreLaunchdatePortfolio(int portfolioId, [FromBody] EmployeePortfolioEntreLaunchdateDto EntreLaunchdateDto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myTeamService.EntreLaunchdateEmployeePortfolio(portfolioId, EntreLaunchdateDto);
                if (result.IsSuccess == false) return BadRequest(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in EntreLaunchdatePortfolio.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while EntreLaunchdating portfolio", Data = null });
            }
        }

        /// <summary>
        /// EntreLaunchdate portfolio attachment data.
        /// </summary>
        [HttpPatch("edit-attachments/{attachmentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyTeamPermissions.EntreLaunchdatePortfolioAttachment)]
        public async Task<IActionResult> EntreLaunchdatePortfolioAttachment(int attachmentId, [FromBody] PortfolioAttachmentEntreLaunchdateDto EntreLaunchdateDto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myTeamService.EntreLaunchdatePortfolioAttachment(attachmentId, EntreLaunchdateDto);
                if (result.IsSuccess == false) return BadRequest(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in EntreLaunchdatePortfolioAttachment.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while EntreLaunchdating portfolio attachment", Data = null });
            }
        }

        /// <summary>
        /// show all Employee request.
        /// </summary>
        [HttpGet("all-employees")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyTeamPermissions.GetAll)]
        public async Task<IActionResult> GetAllEmployees()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myTeamService.AllEmployeeRequest();
                if (result.IsSuccess == false) return NotFound(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetAllEmployees.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while getting all employees", Data = null });
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
        public async Task<IActionResult> PendingEmployees()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myTeamService.PendingEmployees();
                if (result.IsSuccess == false) return BadRequest(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in PendingEmployees.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while getting pending employees", Data = null });
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
        public async Task<IActionResult> AcceptedEmployees()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myTeamService.AcceptedEmployees();
                if (result.IsSuccess == false) return BadRequest(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in AcceptedEmployees.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while getting accepted employees", Data = null });
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
        public async Task<IActionResult> RejectedEmployees()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myTeamService.RejectedEmployees();
                if (result.IsSuccess == false) return BadRequest(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in RejectedEmployees.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while getting rejected employees", Data = null });
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
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while filtering accepted employees by work field", Data = null });
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
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while getting employee by id", Data = null });
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
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while getting portfolios for employee", Data = null });
            }
        }
    }
}
