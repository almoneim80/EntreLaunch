namespace EntreLaunch.Web.Controllers.ServicesAPI
{
    [Authorize(Roles = "Admin, Entrepreneur, Counselor")]
    [Route("api/[controller]")]
    public class TicketMessageController : AuthenticatedController
    {
        private readonly IExtendedBaseService _extendedBaseService;
        private readonly ILogger<TicketMessageController> _logger;
        private readonly IConsultation _consultationService;
        public TicketMessageController(
            ILocalizationManager? localization,
            ILogger<TicketMessageController> logger,
            IExtendedBaseService extendedBaseService,
            IConsultation consultationService)
        {
            _extendedBaseService = extendedBaseService;
            _logger = logger;
            _consultationService = consultationService;
        }

        /// <summary>
        /// send ticket message.
        /// </summary>
        [HttpPost("send")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(TicketMessagePermissions.Create)]
        public async Task<IActionResult> SendMessage([FromBody] TicketMessageCreateDto createDto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var isValidTicket = await createDto.TicketId.CheckIfEntityExistsAsync<ConsultationTicket>(_extendedBaseService, _logger);
                if (isValidTicket != null) return isValidTicket;

                createDto.SenderId = CurrentUserId!;
                var result = await _consultationService.SendTicketMessage(createDto);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in SendMessage.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while send message to ticket", Data = null });
            }
        }

        /// <summary>
        /// EntreLaunchdates a ticket message.
        /// </summary>
        [HttpPatch("edit/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(TicketMessagePermissions.Edit)]
        public async Task<IActionResult> EditMessage(int id, [FromBody] TicketMessageEntreLaunchdateDto EntreLaunchdateDto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _consultationService.EditTicketMessage(id, EntreLaunchdateDto);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in EditMessage.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while edit message to ticket", Data = null });
            }
        }

        /// <summary>
        /// Returns all ticket messages.
        /// </summary>
        [HttpGet("show")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(TicketMessagePermissions.ShowByTicke)]
        public async Task<IActionResult> ShowTicketMessages(int ticketId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var isValidTicket = await ticketId.CheckIfEntityExistsAsync<ConsultationTicket>(_extendedBaseService, _logger);
                if (isValidTicket != null) return isValidTicket;

                var messages = await _consultationService.ShowTicketMessages(ticketId);
                if (messages.IsSuccess == false)
                {
                    return BadRequest(messages);
                }

                return Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in ShowTicketMessages.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while show messages to ticket", Data = null });
            }
        }

        /// <summary>
        /// Deletes a ticket message.
        /// </summary>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(TicketMessagePermissions.Delete)]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _consultationService.DeleteTicketMessage(id);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in DeleteMessage.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while delete message to ticket", Data = null });
            }
        }
    }
}

