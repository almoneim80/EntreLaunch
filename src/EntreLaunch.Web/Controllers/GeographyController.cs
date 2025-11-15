namespace EntreLaunch.Controllers;

[Route("api/[controller]")]
public class ContinentsController : ControllerBase
{
    /// <summary>
    /// Retrieves all continents.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public virtual ActionResult<Dictionary<string, string>> GetAll()
    {
        var result = EnumHelper.GetEnumDescriptions<Continent>();

        return Ok(result);
    }
}

[Route("api/[controller]")]
public class CountriesController : ControllerBase
{
    /// <summary>
    /// Retrieves all countries.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public virtual ActionResult<Dictionary<string, string>> GetAll()
    {
        var result = EnumHelper.GetEnumDescriptions<Country>();

        return Ok(result);
    }
}
