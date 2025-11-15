namespace EntreLaunch.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    public class VersionController : ControllerBase
    {
        private readonly IHttpContextHelper? httpContextHelper;
        public VersionController(IHttpContextHelper? httpContextHelper)
        {
            this.httpContextHelper = httpContextHelper;
        }

        /// <summary>
        /// Returns version.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public ActionResult<VersionDto> Get()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            return Ok(
                new VersionDto
                {
                    Version = fileVersionInfo.ProductVersion!,
                    IP = httpContextHelper!.IpAddress,
                    IPv4 = httpContextHelper!.IpAddressV4,
                    IPv6 = httpContextHelper!.IpAddressV6,
                    Headers = HttpContext.Request.Headers.ToList(),
                });
        }
    }
}
