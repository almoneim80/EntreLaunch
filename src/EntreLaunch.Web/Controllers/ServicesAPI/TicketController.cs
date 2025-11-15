namespace EntreLaunch.Web.Controllers.ServicesAPI
{
    [Authorize(Roles = "Admin, Entrepreneur, Counselor")]
    [Route("api/[controller]")]
    public class TicketController : AuthenticatedController
    {
        private readonly ILogger<TicketController> _logger;
        private readonly IConsultation _consultationService;
        private readonly IExtendedBaseService _extendedBaseService;
        public TicketController(
            ILogger<TicketController> logger,
            ILocalizationManager? localization,
            IConsultation consultationService,
            IExtendedBaseService extendedBaseService)
        {
            _logger = logger;
            _consultationService = consultationService;
            _extendedBaseService = extendedBaseService;
        }

        /// <summary>
        /// Open a new ticket.
        /// </summary>
        [HttpPost("open")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(TicketPermissions.Create)]
        public async Task<IActionResult> OpenTicket([FromBody] TicketCreateDto createDto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var isValidCounselor = await createDto.CreatorId.CheckIfEntityExistsAsync<Counselor>(_extendedBaseService, _logger);
                if (isValidCounselor != null) return isValidCounselor;

                var isValidConsultation = await createDto.ConsultationId.CheckIfEntityExistsAsync<Consultation>(_extendedBaseService, _logger);
                if (isValidConsultation != null) return isValidConsultation;

                var result = await _consultationService.OpenTicket(createDto);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in OpenTicket.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while open ticket", Data = null });
            }
        }

        /// <summary>
        /// Change the ticket status (Open, Closed).
        /// </summary>
        [HttpPost("process")]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(TicketPermissions.Process)]
        public async Task<IActionResult> ProcessTicket([FromBody] ProcessTicketDto processTicketDto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var isReferencedValid = await processTicketDto.Id.CheckIfEntityExistsAsync<ConsultationTicket>(_extendedBaseService, _logger);
                if (isReferencedValid != null) return isReferencedValid;

                var result = await _consultationService.ProgressTicket(processTicketDto);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in ProcessTicket.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while process ticket", Data = null });
            }
        }

        /// <summary>
        /// Returns all tickets.
        /// </summary>
        [HttpGet("all")]
        [ProducesResponseType(typeof(List<TicketDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(TicketPermissions.ShowAll)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var tickets = await _consultationService.AllConsultationTickets();
                if (tickets.IsSuccess == false)
                {
                    return BadRequest(tickets);
                }

                return Ok(tickets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetAll.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while get all tickets", Data = null });
            }
        }

        /// <summary>
        /// Returns one ticket.
        /// </summary>
        [HttpGet("get-one/{id:int}")]
        [ProducesResponseType(typeof(TicketDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(TicketPermissions.ShowOne)]
        public async Task<IActionResult> GetOne(int id)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var ticket = await _consultationService.GetTicketById(id);
                if (ticket.IsSuccess == false)
                {
                    return BadRequest(ticket);
                }

                return Ok(ticket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving ticket. in GetOne");
                return StatusCode(500, "An Unexpected error while retrieving ticket.");
            }
        }

        /// <summary>
        /// Show ticket by consultation id.
        /// </summary>
        [HttpGet("by-consultation/{id:int}")]
        [ProducesResponseType(typeof(TicketDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(TicketPermissions.ShowByConsultation)]
        public async Task<IActionResult> GetConsultationTicketById(int id)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var isValidCounselor = await id.CheckIfEntityExistsAsync<Consultation>(_extendedBaseService, _logger);
                if (isValidCounselor != null) return isValidCounselor;

                var ticket = await _consultationService.GetConsultationTicketById(id);
                if (ticket.IsSuccess == false)
                {
                    return BadRequest(ticket);
                }

                return Ok(ticket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetConsultationTicketById");
                return StatusCode(500, "An Unexpected error occurred while getting ticket by consultation id.");
            }
        }
    }
}

