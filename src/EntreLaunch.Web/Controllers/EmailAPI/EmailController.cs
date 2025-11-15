namespace EntreLaunch.Controllers.EmailAPI
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailVerifyService _emailVerifyService;
        private readonly IMapper _mapper;
        private readonly IEmailVerificationService _emailVerificationService;
        private readonly ILogger<EmailController> _logger;
        public EmailController(
            IEmailVerifyService emailVerifyService,
            IMapper mapper,
            IEmailVerificationService emailVerificationService,
            ILogger<EmailController> logger)
        {
            _emailVerifyService = emailVerifyService;
            _mapper = mapper;
            _emailVerificationService = emailVerificationService;
            _logger = logger;
        }

        /// <summary>
        /// resend verification link.
        /// </summary>
        [HttpPost("resend/verification-link/{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ResendVerificationLink(string email)
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
                return StatusCode(500, new GeneralResult
                {
                    IsSuccess = false, Message = "Unexpected error accurred while resending verification link"
                });
            }
        }

        /// <summary>
        /// Verify the user's email using a verification Code.
        /// </summary>
        [HttpPost("verify-otp/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> VerifyOtp(string id, [FromBody] string code)
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
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error accurred while verifying otp" });
            }
        }

        /// <summary>
        /// regenerate code for user.
        /// </summary>
        [HttpPost("regenerate-otp/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RegenerateOtp(string userId)
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
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error accurred while regenerating otp" });
            }
        }

        /// <summary>
        /// verify email address Domain.
        /// </summary>
        [HttpGet("verify-domain/{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> VerifyEmailDomain([EmailAddress] string email)
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
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "UnExpected error occured while verifying domain" });
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
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
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
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error accurred while confirming email" });
            }
        }
    }
}
