using Microsoft.Extensions.Logging;
using EntreLaunch.DataAnnotations;
using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.SMSDtos;
using EntreLaunch.Infrastructure;
using EntreLaunch.Services.BaseSvc;
namespace EntreLaunch.Plugin.Sms.Controllers
{
    /// <summary>
    /// sms template controller class that extends the base class.
    /// </summary>
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin, SubAdmin, Trainer, Student")]
    public class SmsTemplateController : ControllerBase
    {
        private readonly ILogger<SmsTemplateController> _logger;
        private readonly BaseService<SmsTemplate, SmsTemplateCreateDto, SmsTemplateUpdateDto, SmsTemplateDetailsDto> _baseService;
        public SmsTemplateController(
            BaseService<SmsTemplate, SmsTemplateCreateDto, SmsTemplateUpdateDto, SmsTemplateDetailsDto> service,
            ILogger<SmsTemplateController> logger)
        {
            _logger = logger;
            _baseService = service;
        }

        /// <summary>
        /// Retrieves all sms templates from the database that are not marked as deleted.
        /// </summary>
        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(Permissions.SmsTemplatePermissions.GetAll)]
        public virtual async Task<ActionResult> GetAll([FromQuery] PaginationParams pagination)
        {
            try
            {
                var result = await _baseService.GetAllAsync(pagination);
                if (result.IsSuccess == false || result.Data == null)
                {
                    _logger.LogWarning("No results found in GetAll.");
                    return NotFound("No results found");
                }

                Response.Headers.Append("X-Total-Count", result.Data.TotalCount.ToString());
                _baseService.RemoveSecondLevelObjects(result.Data.Items);

                _logger.LogInformation("Successfully retrieved all records.");
                return Ok(new
                {
                    message = "Get All Success",
                    data = result,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetAll.");
                throw;
            }
        }

        /// <summary>
        /// Retrieves a single sms template by its identifier.
        /// </summary>
        [HttpGet("GetOne/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(Permissions.SmsTemplatePermissions.GetOne)]
        public virtual async Task<ActionResult> GetOne(int id)
        {
            try
            {
                var result = await _baseService.GetOneAsync(id);
                if (result == null)
                {
                    _logger.LogWarning("Record not found for ID: {Id}", id);
                    return NotFound("Record Not Found");
                }

                _logger.LogInformation("Successfully retrieved record with ID: {Id}", id);
                return Ok(new
                {
                    message = "Get One Success",
                    data = result,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetOne for ID: {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Creates a new sms template and saves it to the database.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(Permissions.SmsTemplatePermissions.Create)]
        public virtual async Task<ActionResult> Create([FromBody] SmsTemplateCreateDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _baseService.CreateAsync(createDto);
                if (result == null)
                {
                    _logger.LogWarning("Failed to create record.");
                    return BadRequest();
                }

                var id = ((dynamic)result).Id;
                _logger.LogInformation("Successfully created record");
                return CreatedAtAction(nameof(GetOne), new { id = id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in Create.");
                throw;
            }
        }

        /// <summary>
        /// Updates an existing sms template partially by applying the provided data.
        /// </summary>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(Permissions.SmsTemplatePermissions.Edit)]
        public virtual async Task<ActionResult> Patch(int id, [FromBody] SmsTemplateUpdateDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _baseService.UpdateAsync(id, updateDto);
                if (result == null)
                {
                    _logger.LogWarning("Record not found for patch operation with ID: {Id}", id);
                    return NotFound("Record Not Found");
                }

                _logger.LogInformation("Successfully updated record with ID: {Id}", id);
                return Ok(new { message = "Update Success.", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in Patch for ID: {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Soft deletes an existing sms template by setting its IsDeleted property to true.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(Permissions.SmsTemplatePermissions.Delete)]
        public virtual async Task<ActionResult> Delete(int id)
        {
            try
            {
                var result = await _baseService.DeleteAsync(id);
                if (result.IsSuccess == false)
                {
                    _logger.LogWarning("Record not found for delete operation with ID: {Id}", id);
                    return NotFound("Record Not Found");
                }

                _logger.LogInformation("Successfully deleted record with ID: {Id}", id);
                return Ok(new { Message = "Delete Success" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in Delete for ID: {Id}", id);
                throw;
            }
        }
    }
}
