namespace EntreLaunch.Controllers.EmailAPI
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class EmailSendController(IEmailService emailService, ILogger<EmailSendController> logger) : ControllerBase
    {
        private readonly IEmailService _emailService = emailService;
        private readonly ILogger<EmailSendController> _logger = logger;

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
                return this.UnexpectedError("send email");
            }
        }
    }
}
