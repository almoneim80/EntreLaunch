using PhoneNumbers;
using Serilog;
using EntreLaunch.Services.BaseSvc;
namespace EntreLaunch.Plugin.Sms.Controllers;

[Route("api/messages")]
public class MessagesController : Controller
{
    private readonly ISmsService _smsService;
    private readonly BaseService<SmsTemplate, SmsTemplateCreateDto, SmsTemplateEntreLaunchdateDto, SmsTemplateDetailsDto> _baseService;

    public MessagesController(ISmsService smsService, BaseService<SmsTemplate, SmsTemplateCreateDto, SmsTemplateEntreLaunchdateDto, SmsTemplateDetailsDto> baseService)
    {
        _smsService = smsService;
        _baseService = baseService;
    }

    [HttpPost]
    [Route("sms")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> SendSms([FromBody] SmsDetailsDto smsDetails)
    {
        try
        {
            var recipient = string.Empty;

            try
            {
                recipient = _smsService.ValidateAndFormatPhoneNumber(smsDetails.Recipient);
            }
            catch (NumberParseException npex)
            {
                ModelState.AddModelError(npex.ErrorType.ToString(), npex.Message);
            }


            if (!ModelState.IsValid)
            {
                throw new InvalidModelStateException(ModelState);
            }


            var smsLog = new CreateSmsDto
            {
                Sender = _smsService.GetSender(recipient),
                Recipient = smsDetails.Recipient,
                Message = smsDetails.Message,
                Status = SmsSendStatus.NotSent,
                CreatedAt = DateTimeOffset.UtcNow,
                Source = "EntreLaunch Platform"
            };

            var result = await _smsService.AddSmsToDb(smsLog);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            await _smsService.SendAsync(recipient, smsDetails.Message);
            await _smsService.SuccessSent(result.SmsId);
            return Ok("SMS sent successfully.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to send SMS message to {0}: {1}", smsDetails.Recipient, smsDetails.Message);
            return BadRequest(" Failed to send SMS message.");
        }
    }

    [HttpPost]
    [Route("send-sms-template")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> SendSmsWithTemplate([FromBody] SmsWithTemplateDto smsWithTemplate)
    {
        try
        {
            var recipient = string.Empty;

            try
            {
                recipient = _smsService.ValidateAndFormatPhoneNumber(smsWithTemplate.Recipient);
            }
            catch (NumberParseException npex)
            {
                return BadRequest(new { message = $"Invalid phone number: {npex.Message}" });
            }

            // Get the template from the service
            var template = await _baseService.GetOneAsync(smsWithTemplate.TemplateId);
            if (template.Data == null)
            {
                return BadRequest(new { message = "Invalid template ID. Template not found." });
            }

            // Replace variables in the template if any
            var messageContent = _smsService.ReplacePlaceholders(template.Data.Content, smsWithTemplate.Placeholders);

            var smsLog = new CreateSmsDto
            {
                Sender = _smsService.GetSender(recipient),
                Recipient = smsWithTemplate.Recipient,
                Message = messageContent,
                Status = SmsSendStatus.NotSent,
                CreatedAt = DateTimeOffset.UtcNow,
                Source = "EntreLaunch Platform"
            };

            var result = await _smsService.AddSmsToDb(smsLog);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            await _smsService.SendAsync(recipient, messageContent);
            await _smsService.SuccessSent(result.SmsId);

            return Ok("SMS sent successfully using template.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to send SMS message using template ID {TemplateId} to {Recipient}.", smsWithTemplate.TemplateId, smsWithTemplate.Recipient);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while sending the SMS using the template." });
        }
    }

    [HttpGet]
    [Route("all-sms")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllSms()
    {
        try
        {
            var smsLogs = await _smsService.GetAllSms();
            return Ok(new { message = "SMS logs retrieved successfully.", data = smsLogs });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error retrieving SMS logs.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving SMS logs." });
        }
    }

    [HttpGet]
    [Route("sms/{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSmsById(int id)
    {
        try
        {
            var smsLog = await _smsService.GetSmsById(id);
            return Ok(new { message = "SMS log retrieved successfully.", data = smsLog });
        }
        catch (SmsPluginException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error retrieving SMS log with ID {Id}.", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving the SMS log." });
        }
    }
}
