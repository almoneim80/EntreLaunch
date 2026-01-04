using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.ConsultationDtos;

namespace EntreLaunch.Web.Controllers.ServicesAPI
{
    [Authorize(Roles = AppRoles.ConsultationRoles)]
    [Route("api/[controller]")]
    public class ConsultationTicketController(
        ILocalizationManager localization,
        ILogger<ConsultationTicketController> logger,
        IExtendedBaseService extendedBaseService,
        ITicketService ticketService) : AuthenticatedController(localization)
    {
        private readonly IExtendedBaseService _extendedBaseService = extendedBaseService;
        private readonly ILogger<ConsultationTicketController> _logger = logger;
        private readonly ITicketService _ticketService = ticketService;
        private readonly ILocalizationManager _localizationManager = localization;

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
        public async Task<IActionResult> CreateTicket([FromBody] TicketCreateDto dto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var isValidCounselor = await dto.CreatorId.CheckIfEntityExistsAsync<Counselor>(_extendedBaseService, _logger, _localizationManager);
                if (isValidCounselor != null) return isValidCounselor;

                var isValidConsultation = await dto.ConsultationId.CheckIfEntityExistsAsync<Consultation>(_extendedBaseService, _logger, _localizationManager);
                if (isValidConsultation != null) return isValidConsultation;

                var result = await _ticketService.CreateTicket(dto);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in OpenTicket.");
                return this.UnexpectedError("open ticket");
            }
        }

        /// <summary>
        /// Change the ticket status (Open, Closed).
        /// </summary>
        [HttpPost("update-status")]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(TicketPermissions.Process)]
        public async Task<IActionResult> UpdateTicketStatus([FromBody] ProcessTicketDto dto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var isReferencedValid = await dto.Id.CheckIfEntityExistsAsync<ConsultationTicket>(_extendedBaseService, _logger, _localizationManager);
                if (isReferencedValid != null) return isReferencedValid;

                var result = await _ticketService.UpdateTicketStatus(dto);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in ProcessTicket.");
                return this.UnexpectedError("process ticket");
            }
        }

        /// <summary>
        /// Returns all tickets.
        /// </summary>
        [HttpGet("get-all")]
        [ProducesResponseType(typeof(PaginatedResult<TicketFullDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(TicketPermissions.GetAll)]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var tickets = await _ticketService.GetAllTickets(pagination, cancellationToken);
                if (!tickets.IsSuccess)
                {
                    return BadRequest(tickets);
                }

                return Ok(tickets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetAll.");
                return this.UnexpectedError("get all tickets");
            }
        }

        /// <summary>
        /// Returns one ticket.
        /// </summary>
        [HttpGet("get-one/{id:int}")]
        [ProducesResponseType(typeof(TicketFullDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(TicketPermissions.GetOne)]
        public async Task<IActionResult> GetOne([FromRoute] int id)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var ticket = await _ticketService.GetTicketById(id);
                if (ticket.IsSuccess == false)
                {
                    return BadRequest(ticket);
                }

                return Ok(ticket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving ticket. in GetOne");
                return this.UnexpectedError("retrieving ticket.");
            }
        }

        /// <summary>
        /// Show ticket by consultation id.
        /// </summary>
        [HttpGet("get-by-consultation/{id:int}")]
        [ProducesResponseType(typeof(TicketFullDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(TicketPermissions.GetByConsultation)]
        public async Task<IActionResult> GetTicketByConsultationId([FromRoute] int id)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var isValidCounselor = await id.CheckIfEntityExistsAsync<Consultation>(_extendedBaseService, _logger, _localizationManager);
                if (isValidCounselor != null) return isValidCounselor;

                var ticket = await _ticketService.GetTicketByConsultationId(id);
                if (ticket.IsSuccess == false)
                {
                    return BadRequest(ticket);
                }

                return Ok(ticket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetConsultationTicketById");
                return this.UnexpectedError("getting ticket by consultation id.");
            }
        }

        /// <summary>
        /// send ticket attachment.
        /// </summary>
        [HttpPost("send")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(TicketMessagePermissions.Create)]
        public async Task<IActionResult> CreateTicketAttachment([FromBody] TicketAttachmentCreateDto dto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var isValidTicket = await dto.TicketId.CheckIfEntityExistsAsync<ConsultationTicket>(_extendedBaseService, _logger, _localizationManager);
                if (isValidTicket != null) return isValidTicket;

                dto.SenderId = CurrentUserId;
                var result = await _ticketService.CreateTicketAttachment(dto);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in SendAttachment.");
                return this.UnexpectedError("send attachment");
            }
        }

        /// <summary>
        /// Returns all ticket attachments.
        /// </summary>
        [HttpGet("attachments/{ticketId:int}")]
        [ProducesResponseType(typeof(PaginatedResult<TicketAttachmentDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(TicketMessagePermissions.GetAll)]
        public async Task<IActionResult> GetTicketAttachments([FromRoute] int ticketId, [FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var isValidTicket = await ticketId.CheckIfEntityExistsAsync<ConsultationTicket>(_extendedBaseService, _logger, _localizationManager);
                if (isValidTicket != null) return isValidTicket;

                var result = await _ticketService.GetTicketAttachments(ticketId, pagination, cancellationToken);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in ShowTicketAttachment.");
                return this.UnexpectedError("show attachments");
            }
        }

        /// <summary>
        /// Deletes a ticket attachment.
        /// </summary>
        [HttpDelete("delete-attachment/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(TicketMessagePermissions.Delete)]
        public async Task<IActionResult> DeleteTicketAttachment([FromRoute] int id)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _ticketService.DeleteTicketAttachment(id, CurrentUserId!);
                if (result.IsSuccess == false)
                {
                    return NotFound(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in DeleteAttachment.");
                return this.UnexpectedError("delete attachment");
            }
        }

        /// <summary>
        /// send ticket message.
        /// </summary>
        [HttpPost("message")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(TicketMessagePermissions.Create)]
        public async Task<IActionResult> CreateTicketMessage([FromBody] TicketMessageCreateDto createDto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var isValidTicket = await createDto.TicketId.CheckIfEntityExistsAsync<ConsultationTicket>(_extendedBaseService, _logger, _localizationManager);
                if (isValidTicket != null) return isValidTicket;

                createDto.SenderId = CurrentUserId!;
                var result = await _ticketService.CreateTicketMessage(createDto);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in SendMessage.");
                return this.UnexpectedError(" send message to ticket");
            }
        }

        /// <summary>
        /// Updates a ticket message.
        /// </summary>
        [HttpPatch("update-message/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(TicketMessagePermissions.Edit)]
        public async Task<IActionResult> UpdateTicketMessage([FromRoute] int id, [FromBody] TicketMessageUpdateDto updateDto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _ticketService.UpdateTicketMessage(id, updateDto);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in EditMessage.");
                return this.UnexpectedError("edit message to ticket");
            }
        }

        /// <summary>
        /// Returns all ticket messages.
        /// </summary>
        [HttpGet("message/{ticketId}")]
        [ProducesResponseType(typeof(PaginatedResult<TicketMessageDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(TicketMessagePermissions.GetByTicke)]
        public async Task<IActionResult> GetTicketMessages([FromRoute] int ticketId, [FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var isValidTicket = await ticketId.CheckIfEntityExistsAsync<ConsultationTicket>(_extendedBaseService, _logger, _localizationManager);
                if (isValidTicket != null) return isValidTicket;

                var result = await _ticketService.GetTicketMessages(ticketId, pagination, cancellationToken);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in ShowTicketMessages.");
                return this.UnexpectedError("show messages to ticket");
            }
        }

        /// <summary>
        /// Retrieves all tickets created by a specific counselor.
        /// </summary>
        [HttpGet("get-by-counselor/{counselorId}")]
        [ProducesResponseType(typeof(PaginatedResult<TicketFullDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(TicketPermissions.GetByCounselor)]
        public async Task<IActionResult> GetTicketsByCounselor([FromRoute] int counselorId, [FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _ticketService.GetTicketsByCounselor(counselorId, pagination, cancellationToken);
                return result.IsSuccess ? Ok(result) : NotFound(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetTicketsByCounselor.");
                return this.UnexpectedError("get tickets by counselor");
            }
        }

        /// <summary>
        /// Retrieves open tickets related to a specific consultation.
        /// </summary>
        /// <param name="consultationId">The ID of the consultation.</param>
        /// <returns>
        /// 200 OK with the list of open tickets;
        /// 400 Bad Request for invalid input;
        /// 404 Not Found if no matching tickets exist;
        /// 401 Unauthorized if the user is not authenticated;
        /// 500 Internal Server Error on failure.
        /// </returns>
        [HttpGet("get-open-by-consultation/{consultationId}")]
        [ProducesResponseType(typeof(List<TicketFullDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(TicketPermissions.GetOpenTickets)]
        public async Task<IActionResult> GetOpenTicketsByConsultation([FromRoute] int consultationId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _ticketService.GetOpenTicketsForConsultation(consultationId);
                if (!result.IsSuccess)
                {
                    return NotFound(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetOpenTicketsByConsultation.");
                return this.UnexpectedError("get open tickets by consultation");
            }
        }

        /// <summary>
        /// Checks if the current user has access to a specific ticket (either as client or counselor).
        /// </summary>
        [HttpGet("can-access/{ticketId}")]
        [ProducesResponseType(typeof(GeneralResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(TicketPermissions.CanAccessToTicket)]
        public async Task<IActionResult> CanUserAccessTicket([FromRoute] int ticketId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _ticketService.CanUserAccessTicket(ticketId, CurrentUserId!);
                if (!result.IsSuccess)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in CanUserAccessTicket.");
                return this.UnexpectedError("check user access to ticket");
            }
        }

        /// <summary>
        /// Deletes a ticket message.
        /// </summary>
        [HttpDelete("delete-message/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(TicketMessagePermissions.Delete)]
        public async Task<IActionResult> DeleteMessage([FromRoute] int id)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _ticketService.DeleteTicketMessage(id);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in DeleteMessage.");
                return this.UnexpectedError("delete message to ticket");
            }
        }
    }
}
