namespace EntreLaunch.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    public class ActivityLogController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ESOnlyQueryProviderFactory<ActivityLog> queryProviderFactory;

        public ActivityLogController(IMapper mapper, ESOnlyQueryProviderFactory<ActivityLog> queryProviderFactory)
        {
            this.mapper = mapper;
            this.queryProviderFactory = queryProviderFactory;
        }

        /// <summary>
        /// Get All ActivityLog.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public virtual async Task<ActionResult<List<ActivityLogDetailsDto>>> Get([FromQuery] string? query)
        {
            var qp = queryProviderFactory.BuildQueryProvider();

            var result = await qp.GetResult();
            Response.Headers.Append(ResponseHeaderNames.TotalCount, result.TotalCount.ToString());
            Response.Headers.Append(ResponseHeaderNames.AccessControlExposeHeader, ResponseHeaderNames.TotalCount);
            return Ok(new GeneralResult(true, "", mapper.Map<List<ActivityLogDetailsDto>>(result.Records)));
        }
    }
}
