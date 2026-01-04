namespace EntreLaunch.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class GeographyController : ControllerBase
    {
        /// <summary>
        /// Retrieves all continents.
        /// </summary>
        [HttpGet("continents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public ActionResult<Dictionary<string, string>> GetContinents()
        {
            var result = EnumHelper.GetEnumDescriptions<Continent>();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves all countries.
        /// </summary>
        [HttpGet("countries")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public ActionResult<Dictionary<string, string>> GetCountries()
        {
            var result = EnumHelper.GetEnumDescriptions<Country>();
            return Ok(result);
        }
    }
}
