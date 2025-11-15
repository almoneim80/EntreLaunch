namespace EntreLaunch.Controllers.PaymentAPI
{
    [Authorize(Roles = "Admin , Entrepreneur")]
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]

    public class RefundController : ControllerBase
    {
        private readonly IRefundService _refundService;
        private readonly ILogger<RefundController> _logger;

        public RefundController(IRefundService refundService, ILogger<RefundController> logger)
        {
            _refundService = refundService;
            _logger = logger;
        }

        /// <summary>
        /// Create a new refund request.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(RefundPermissions.Create)]
        public async Task<ActionResult<RefundDetailsDto>> CreateRefund([FromBody] RefundCreateDto refundCreateDto)
        {
            try
            {
                if (refundCreateDto == null)
                {
                    _logger.LogError("RefundCreateDto is null.");
                    return BadRequest("RefundCreateDto is null.");
                }

                var result = await _refundService.CreateRefundAsync(refundCreateDto);
                return CreatedAtAction(nameof(GetRefundById), new { refundId = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating refund request.");
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Approve a refund request.
        /// </summary>
        [HttpPost("{refundId}/approve")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(RefundPermissions.Approve)]
        public async Task<ActionResult<RefundDetailsDto>> ApproveRefund(int refundId)
        {
            try
            {
                var result = await _refundService.ApproveRefundAsync(refundId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while approving refund request.");
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Reject a refund request.
        /// </summary>
        [HttpPost("{refundId}/reject")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(RefundPermissions.Reject)]
        public async Task<ActionResult<RefundDetailsDto>> RejectRefund(int refundId)
        {
            try
            {
                var result = await _refundService.RejectRefundAsync(refundId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while rejecting refund request.");
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get a refund request by ID.
        /// </summary>
        [HttpGet("{refundId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(RefundPermissions.ShowOne)]
        public async Task<ActionResult<RefundDetailsDto>> GetRefundById(int refundId)
        {
            try
            {
                var result = await _refundService.GetRefundByIdAsync(refundId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching refund details.");
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get all refund requests.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(RefundPermissions.ShowAll)]
        public async Task<ActionResult<IEnumerable<RefundDetailsDto>>> GetAllRefunds()
        {
            try
            {
                var result = await _refundService.GetAllRefundsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all refunds.");
                return StatusCode(500, new { message = "An error occurred while fetching refunds." });
            }
        }
    }
}
