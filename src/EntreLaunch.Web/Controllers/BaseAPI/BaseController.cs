namespace EntreLaunch.Controllers.BaseAPI
{
    [Route("api/[controller]")]
    public class BaseController<T, TC, TU, TD, TE> : ControllerBase
    where T : SharedData, new()
    where TC : class
    where TU : class
    where TD : class
    where TE : class
    {
        protected readonly BaseService<T, TC, TU, TD> _service;
        private readonly ILocalizationManager? _localization;
        private readonly ILogger<BaseController<T, TC, TU, TD, TE>> _logger;
        private readonly IExportService _exportService;

        public BaseController(
            BaseService<T, TC, TU, TD> service,
            ILocalizationManager? localization,
            ILogger<BaseController<T, TC, TU, TD, TE>> logger,
            IExportService exportService)
        {
            _service = service;
            _localization = localization;
            _logger = logger;
            _exportService = exportService;
        }

        /// <summary>
        /// Creates a new entity and saves it to the database.
        /// </summary>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public virtual async Task<ActionResult<TD>> Create([FromBody] TC createDto)
        {
            try
            {
                var result = await _service.CreateAsync(createDto);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }


                var id = ((dynamic)result.Data!).Id;
                _logger.LogInformation("Successfully created record");
                return CreatedAtAction(nameof(GetOne), new { id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in Create.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error adding record", Data = null });
            }
        }

        /// <summary>
        /// EntreLaunchdates an existing entity partially by applying the provided data.
        /// </summary>
        [HttpPatch("edit/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public virtual async Task<ActionResult<TD>> Patch(int id, [FromBody] TU EntreLaunchdateDto)
        {
            try
            {
                var result = await _service.EntreLaunchdateAsync(id, EntreLaunchdateDto);
                if (result.IsSuccess == false)
                {
                    _logger.LogWarning("Record not found for patch operation with ID: {Id}", id);
                    return BadRequest(result);
                }

                _logger.LogInformation("Successfully EntreLaunchdated record with ID: {Id}", id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in Patch for ID: {Id}", id);
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error EntreLaunchdating record", Data = null });
            }
        }

        /// <summary>
        /// Retrieves all entities from the database that are not marked as deleted.
        /// </summary>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public virtual async Task<ActionResult<TD[]>> GetAll()
        {
            try
            {
                var results = await _service.GetAllAsync();
                if (results.IsSuccess == false || results.Data == null)
                {
                    return BadRequest(results);
                }

                Response.Headers.Append("X-Total-Count", results.Data.Count.ToString());
                _service.RemoveSecondLevelObjects(results.Data);
                _logger.LogInformation("Successfully retrieved all records.");
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in Get All.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error retrieving records", Data = null });
            }
        }

        /// <summary>
        /// Retrieves a single entity by its identifier.
        /// </summary>
        [HttpGet("get-one/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public virtual async Task<ActionResult<TD>> GetOne(int id)
        {
            try
            {
                var result = await _service.GetOneAsync(id);
                if (result.IsSuccess == false || result.Data == null)
                {
                    _logger.LogWarning("Record not found for ID: {Id}", id);
                    return BadRequest(result);
                }

                _logger.LogInformation("Successfully retrieved record with ID: {Id}", id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetOne for ID: {Id}", id);
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error retrieving record", Data = null });
            }
        }

        /// <summary>
        /// Exports data to CSV.
        /// </summary>
        [HttpGet("export/csv")]
        [Produces("text/csv")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public virtual async Task<IActionResult> ExportToCsv()
        {
            try
            {
                var csvData = await _exportService.ExportToCsvAsync<T, TE>();
                if (csvData.IsSuccess == false || string.IsNullOrEmpty(csvData.Data))
                {
                    _logger.LogWarning("No data available for CSV export.");
                    return BadRequest(csvData);
                }

                var fileName = $"{typeof(T).Name}_Export_{DateTime.UtcNow:yyyyMMddHHmmss}.csv";
                _logger.LogInformation("CSV export successful.");
                return File(new System.Text.UTF8Encoding().GetBytes(csvData.Data), "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during CSV export.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error exporting to CSV", Data = null });
            }
        }

        /// <summary>
        /// Exports data to Excel.
        /// </summary>
        [HttpGet("export/excel")]
        [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public virtual async Task<IActionResult> ExportToExcel()
        {
            try
            {
                var excelData = await _exportService.ExportToExcelAsync<T, TE>();

                if (excelData.IsSuccess == false || excelData.Data == null)
                {
                    _logger.LogWarning("No data available for Excel export.");
                    return BadRequest(excelData);
                }

                var fileName = $"{typeof(T).Name}_Export_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";
                _logger.LogInformation("Excel export successful.");
                return File(excelData.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during Excel export.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error exporting to Excel", Data = null });
            }
        }

        /// <summary>
        /// Export data to JSON format.
        /// </summary>
        [HttpGet("export/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public virtual async Task<IActionResult> ExportToJson()
        {
            try
            {
                var jsonData = await _exportService.ExportToJsonAsync<T, TE>();

                if (jsonData.IsSuccess == false || string.IsNullOrEmpty(jsonData.Data))
                {
                    _logger.LogWarning("No data available for JSON export.");
                    return BadRequest(jsonData);
                }

                var fileName = $"{typeof(T).Name}_Export_{DateTime.UtcNow:yyyyMMddHHmmss}.json";
                _logger.LogInformation("JSON export successful.");
                return File(new System.Text.UTF8Encoding().GetBytes(jsonData.Data), "application/json", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during JSON export.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error exporting to JSON", Data = null });
            }
        }

        /// <summary>
        /// Soft deletes an existing entity by setting its IsDeleted property to true.
        /// </summary>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public virtual async Task<ActionResult> Delete(int id)
        {
            try
            {
                var result = await _service.DeleteAsync(id);
                if (result.IsSuccess == false)
                {
                    _logger.LogWarning("Record not found for delete operation with ID: {Id}", id);
                    return BadRequest(result);
                }

                _logger.LogInformation("Successfully deleted record with ID: {Id}", id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in Delete for ID: {Id}", id);
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error deleting record", Data = null });
            }
        }
    }
}
