namespace EntreLaunch.Web.Controllers.ServicesAPI
{
    [Authorize(Roles = "Admin, Entrepreneur, Counselor")]
    [Route("api/[controller]")]
    public class TicketAttachmentController : AuthenticatedController
    {
        private readonly IExtendedBaseService _extendedBaseService;
        private readonly ILogger<TicketAttachmentController> _logger;
        private readonly IConsultation _consultationService;
        public TicketAttachmentController(
            ILocalizationManager? localization,
            ILogger<TicketAttachmentController> logger,
            IExtendedBaseService extendedBaseService,
            IConsultation consultationService)
        {
            _extendedBaseService = extendedBaseService;
            _logger = logger;
            _consultationService = consultationService;
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
        public async Task<IActionResult> SendAttachment([FromBody] TicketAttachmentCreateDto createDto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var isValidTicket = await createDto.TicketId.CheckIfEntityExistsAsync<ConsultationTicket>(_extendedBaseService, _logger);
                if (isValidTicket != null) return isValidTicket;

                createDto.SenderId = CurrentUserId;
                var result = await _consultationService.SendTicketAttachment(createDto);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in SendAttachment.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while send attachment", Data = null });
            }
        }

        /// <summary>
        /// Returns all ticket attachments.
        /// </summary>
        [HttpGet("show")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(TicketMessagePermissions.ShowAll)]
        public async Task<IActionResult> ShowTicketAttachment(int ticketId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var isValidTicket = await ticketId.CheckIfEntityExistsAsync<ConsultationTicket>(_extendedBaseService, _logger);
                if (isValidTicket != null) return isValidTicket;

                var messages = await _consultationService.ShowTicketAttachment(ticketId);
                if (messages.IsSuccess == false)
                {
                    return BadRequest(messages);
                }

                return Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in ShowTicketAttachment.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while show attachments", Data = null });
            }
        }

        /// <summary>
        /// Deletes a ticket attachment.
        /// </summary>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(TicketMessagePermissions.Delete)]
        public async Task<IActionResult> DeleteAttachment(int id)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _consultationService.DeleteTicketAttachment(id, CurrentUserId!);
                if (result.IsSuccess == false)
                {
                    return NotFound(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in DeleteAttachment.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while delete attachment", Data = null });
            }
        }
    }
}
