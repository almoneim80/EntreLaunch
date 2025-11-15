namespace EntreLaunch.Web.Controllers.EmailAPI
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    public class DomainsController : BaseController<Domain, DomainCreateDto, DomainEntreLaunchdateDto, DomainDetailsDto, DomainExpotDto>
    {
        private readonly IDomainService domainService;
        private readonly ILogger<DomainsController> _logger;

        public DomainsController(
            BaseService<Domain, DomainCreateDto, DomainEntreLaunchdateDto, DomainDetailsDto> service,
            IDomainService domainService,
            ILocalizationManager? localization,
            IExportService exportService,
            ILogger<DomainsController> logger)
            : base(service, localization, logger, exportService)
        {
            this.domainService = domainService;
            _logger = logger;
        }

        public bool IsDeleted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DateTimeOffset? DeletedAt { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// Verify domain name.
        /// </summary>
        [HttpGet("verify/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DomainDetailsDto>> Verify(string name, bool force = false)
        {
            try
            {
                var result = await domainService.VerifyDomainAsync(name, force);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "UnExpected error occured while verifying domain" });
            }
        }

        /// <summary>
        /// SaveRange async.
        /// </summary>
        protected async Task SaveRangeAsync(List<Domain> newRecords)
        {
            await domainService.SaveRangeAsync(newRecords);
        }
    }
}
