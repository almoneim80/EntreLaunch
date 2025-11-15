namespace EntreLaunch.Controllers.EmailAPI
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class EmailSendController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailSendController> _logger;
        public EmailSendController(IEmailService emailService, ILogger<EmailSendController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        /// <summary>
        /// Send an email using the provided request.
        /// </summary>
        [HttpPost("send")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailRequest request)
        {
            try
            {
                var messageId = await _emailService.SendAsync(request.Subject, request.FromEmail, request.FromName,
                    request.Recipients, request.Body, request.Attachments);
                if(string.IsNullOrEmpty(messageId.Data) || messageId.IsSuccess == false)
                {
                    return BadRequest(messageId);
                }

                return Ok(messageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error accurred while sending email" });
            }
        }
    }
}
