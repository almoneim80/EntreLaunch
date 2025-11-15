namespace EntreLaunch.Controllers;

[RequiredPermission(Permissions.LocalizationPermissions.FirstTimeSetEntreLaunchOrDefault)]
public class LocalizationController : ControllerBase
{
    private readonly ILocalizationManager _localization;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizationController"/> class.
    /// </summary>
    /// <param name="localization">Instance of the <see cref="ILocalizationManager"/> interface.</param>
    public LocalizationController(ILocalizationManager localization)
    {
        _localization = localization;
    }

    /// <summary>
    /// Gets known cultures.
    /// </summary>
    /// <response code="200">Known cultures returned.</response>
    /// <returns>An <see cref="OkResult"/> containing the list of cultures.</returns>
    [HttpGet("Cultures")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<CultureDto>> GetCultures()
    {
        return Ok(_localization.GetCultures());
    }

    /// <summary>
    /// Gets known countries.
    /// </summary>
    /// <response code="200">Known countries returned.</response>
    /// <returns>An <see cref="OkResult"/> containing the list of countries.</returns>
    [HttpGet("Countries")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<CountryInfo>> GetCountries()
    {
        return Ok(_localization.GetCountries());
    }

    /// <summary>
    /// Gets known parental ratings.
    /// </summary>
    /// <response code="200">Known parental ratings returned.</response>
    /// <returns>An <see cref="OkResult"/> containing the list of parental ratings.</returns>
    [HttpGet("ParentalRatings")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<ParentalRating>> GetParentalRatings()
    {
        return Ok(_localization.GetParentalRatings());
    }

    /// <summary>
    /// Gets localization options.
    /// </summary>
    /// <response code="200">Localization options returned.</response>
    /// <returns>An <see cref="OkResult"/> containing the list of localization options.</returns>
    [HttpGet("Options")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<LocalizationOption>> GetLocalizationOptions()
    {
        return Ok(_localization.GetLocalizationOptions());
    }

    /// <summary>
    /// change language and culture.
    /// </summary>
    [HttpPost("setculture")]
    public IActionResult SetCulture([FromBody] string culture)
    {
        try
        {
            _localization.SetCulture(culture);

            // Store the preferred culture in a cookie
            Response.Cookies.Append("PreferredCulture", culture, new CookieOptions
            {
                Expires = DateTime.UtcNow.AddYears(1),
                HttpOnly = true,
                Secure = true, // Use this if your site uses HTTPS
            });

            return Ok($"Culture set to {culture}");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Gets success message.   
    /// </summary>
    [HttpGet("success")]
    public IActionResult GetSuccessMessage()
    {
        var message = _localization.GetLocalizedString("CameraImageEntreLaunchloadedFrom");
        return Ok(new { Message = message });
    }
}
