namespace EntreLaunch.Controllers.EmailAPI
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class ConfirmationController(
        IEmailVerifyService emailVerifyService,
        IMapper mapper,
        IEmailVerificationService emailVerificationService,
        ILogger<ConfirmationController> logger) : ControllerBase
    {
        private readonly IEmailVerifyService _emailVerifyService = emailVerifyService;
        private readonly IMapper _mapper = mapper;
        private readonly IEmailVerificationService _emailVerificationService = emailVerificationService;
        private readonly ILogger<ConfirmationController> _logger = logger;

        /// <summary>
        /// resend verification link.
        /// </summary>
        [HttpPost("resend/verification-link/")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ResendVerificationLink([FromQuery] string email)
        {
            try
            {
                var result = await _emailVerificationService.ResendVerificationLinkAsync(email);
                var succeeded = result.Data;
                if (succeeded.Succeeded)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.UnexpectedError("resend verification link");
            }
        }

        /// <summary>
        /// Verify the user's email using a verification Code.
        /// </summary>
        [HttpPost("verify-otp/")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> VerifyOtp([FromQuery] string id, [FromBody] string code)
        {
            try
            {
                var isValid = await _emailVerificationService.VerifyOtpAsync(id, code);
                if (isValid.IsSuccess)
                {
                    return Ok(isValid);
                }

                return BadRequest(isValid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.UnexpectedError("verify otp");
            }
        }

        /// <summary>
        /// regenerate code for user.
        /// </summary>
        [HttpPost("regenerate-otp/")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RegenerateOtp([FromQuery] string userId)
        {
            try
            {
                var result = await _emailVerificationService.RegenerateOtpAsync(userId);
                var (succeeded, message, expireAt) = result.Data;
                if (!succeeded)
                {
                    return BadRequest(new GeneralResult { IsSuccess = succeeded, Message = message, Data = null });
                }

                if (expireAt == null)
                {
                    return Ok(new GeneralResult { IsSuccess = succeeded, Message = message, Data = null });
                }

                return Ok(new GeneralResult { IsSuccess = succeeded, Message = message, Data = expireAt });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.UnexpectedError("regenerate otp");
            }
        }

        /// <summary>
        /// verify email address Domain.
        /// </summary>
        [HttpGet("verify-domain/")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> VerifyEmailDomain([FromQuery] string email)
        {
            try
            {
                var resultedDomainData = await _emailVerifyService.Verify(email);
                if (resultedDomainData.IsSuccess == false)
                {
                    return BadRequest(resultedDomainData);
                }

                var resultConverted = _mapper.Map<EmailVerifyDetailsDto>(resultedDomainData);
                resultConverted.EmailAddress = email;
                return Ok(resultConverted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.UnexpectedError("verify email domain.");
            }
        }

        /// <summary>
        /// Verify the user's email using a verification link.
        /// </summary>
        [HttpGet("confirm-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            try
            {
                var result = await _emailVerificationService.ConfirmEmailAsync(userId, token);
                var (succeeded, message, errors) = result.Data;
                if (succeeded)
                {
                    return Ok(new GeneralResult { IsSuccess = succeeded, Message = message, Data = null });
                }
                else if (errors != null)
                {
                    return BadRequest(new GeneralResult { IsSuccess = succeeded, Message = message, Data = errors });
                }
                else
                {
                    return BadRequest(new GeneralResult { IsSuccess = succeeded, Message = message, Data = errors });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.UnexpectedError("confirm email");
            }
        }
    }
}
