namespace EntreLaunch.Web.Controllers.TrainingAPI
{
    [Authorize(Roles = AppRoles.TrainingRoles)]
    [Route("api/[controller]")]
    public class CertificateController(
        ILocalizationManager localization,
        ILogger<CertificateController> logger,
        IExtendedBaseService extendedBaseService,
        ICertificateService certificateService) : AuthenticatedController(localization)
    {
        private readonly IExtendedBaseService _extendedBaseService = extendedBaseService;
        private readonly ICertificateService _certificateService = certificateService;
        private readonly ILogger<CertificateController> _logger = logger;

        [HttpPost("admin/path/{pathId:int}/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(StudentCertificatePermissions.Issue)]
        public async Task<IActionResult> IssuePathCertificateByAdmin(int pathId, [FromRoute] string userId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _certificateService.IssuePathCertificateAsync(pathId, userId);
                return (result.IsSuccess == false) ? BadRequest(result) : Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while issuing certificate by admin.");
                return this.UnexpectedError("issuing certificate.");
            }
        }

        [HttpPost("path/{pathId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(StudentCertificatePermissions.Issue)]
        public async Task<IActionResult> IssuePathCertificateByStudent([FromRoute] int pathId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _certificateService.IssuePathCertificateAsync(pathId, CurrentUserId!);
                return (result.IsSuccess == false) ? BadRequest(result) : Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while issuing certificate by student.");
                return this.UnexpectedError("issuing certificate.");
            }
        }

        [HttpPost("admin/course/{courseId:int}/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(StudentCertificatePermissions.Issue)]
        public async Task<IActionResult> IssueCourseCertificateByAdmin([FromRoute] int courseId, [FromRoute] string userId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _certificateService.IssueCourseCertificateAsync(courseId, userId);
                return (result.IsSuccess == false) ? BadRequest(result) : Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while issuing certificate by admin.");
                return this.UnexpectedError("issuing certificate.");
            }
        }

        [HttpPost("course/{courseId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(StudentCertificatePermissions.Issue)]
        public async Task<IActionResult> IssueCourseCertificateByStudent([FromRoute] int courseId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _certificateService.IssueCourseCertificateAsync(courseId, CurrentUserId!);
                return (result.IsSuccess == false) ? BadRequest(result) : Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while issuing certificate by student.");
                return this.UnexpectedError("issuing certificate.");
            }
        }

        [HttpPatch("edit-delivery-method/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(StudentCertificatePermissions.ShippingCertificate)]
        public async Task<IActionResult> ShippingCertificate([FromRoute] int id, [FromQuery] string shippingAddress)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                var result = await _certificateService.ShippingCertificateAsync(id, shippingAddress, CurrentUserId!);
                return (result.IsSuccess == false) ? BadRequest(result) : Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating StudentCertificate.");
                return this.UnexpectedError(" updating StudentCertificate.");
            }
        }

        /// <summary>
        /// Retrieves all available student certificate delivery methods.
        /// Requires the user to have permission to retrieve enumeration values.
        /// </summary>
        /// <returns>
        /// An <see cref="ActionResult"/> containing a list of available delivery methods as <see cref="EnumData"/>.
        /// </returns>
        [HttpGet("delivery-method")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(StudentCertificatePermissions.GetEnumValues)]
        public ActionResult<IEnumerable<EnumData>> GetStudentCertificateDeliveryMethod()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var enumValues = _extendedBaseService.GetEnumValues<DeliveryMethod>();
                return Ok(new GeneralResult { IsSuccess = true, Message = "Student certificate delivery method retrieved successfully", Data = enumValues });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting Student Certificate Delivery Method.");
                return this.UnexpectedError(" getting Student Certificate Delivery Method.");
            }
        }

        /// <summary>
        /// Retrieves all student certificates.
        /// </summary>
        [HttpGet("all")]
        [ProducesResponseType(typeof(PaginatedResult<CertificateDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(StudentCertificatePermissions.GetAll)]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _certificateService.GetAllAsync(pagination, cancellationToken);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all StudentCertificates.");
                return this.UnexpectedError("getting all StudentCertificates.");
            }
        }

        [HttpGet("shipping-requests")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(StudentCertificatePermissions.GetAllShippingRequests)]
        public async Task<IActionResult> GetAllShippingRequests()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _certificateService.GetAllShippingCertificatesAsync();
                return (!result.IsSuccess) ? BadRequest(result) : Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all StudentCertificates.");
                return this.UnexpectedError("getting all StudentCertificates.");
            }
        }

        /// <summary>
        /// Retrieves detailed information about a specific student certificate by its ID.
        /// Requires the user to have permission to view individual certificates.
        /// </summary>
        /// <param name="certificateId">
        /// The unique identifier of the certificate to retrieve details for.
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing a <see cref="GeneralResult{T}"/> with the certificate details if successful.
        /// </returns>
        [HttpGet("get-one/{certificateId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(StudentCertificatePermissions.GetOne)]
        public async Task<IActionResult> GetOne([FromRoute] int certificateId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                if (certificateId <= 0)
                {
                    return BadRequest(new GeneralResult { IsSuccess = false, Message = "Invalid certificate ID." });
                }

                var result = await _certificateService.GetOneAsync(certificateId);
                return (!result.IsSuccess) ? BadRequest(result) : Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving certificate details.");
                return this.UnexpectedError("retrieving certificate details.");
            }
        }

        [HttpPatch("update-shipping-status/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(StudentCertificatePermissions.UpdateShippingStatus)]
        public async Task<IActionResult> UpdateShippingStatus([FromRoute] int id, [FromQuery] int newStatus)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _certificateService.UpdateShippingStatusAsync(id, (ShippingStatus)newStatus);
                return (result.IsSuccess == false) ? BadRequest(result) : Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating shipping status for certificate ID {CertificateId}.", id);
                return this.UnexpectedError("updating shipping status.");
            }
        }

        [HttpGet("my-certificates")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(StudentCertificatePermissions.GetMyCertificates)]
        public async Task<IActionResult> GetMyCertificates()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _certificateService.GetUserCertificatesAsync(CurrentUserId!);
                return (!result.IsSuccess) ? BadRequest(result) : Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user certificates.");
                return this.UnexpectedError("retrieving user certificates.");
            }
        }

        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(StudentCertificatePermissions.Delete)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _certificateService.DeleteAsync(id);
                return (result.IsSuccess == false) ? BadRequest(result) : Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting StudentCertificate.");
                return this.UnexpectedError("deleting StudentCertificate.");
            }
        }
    }
}

