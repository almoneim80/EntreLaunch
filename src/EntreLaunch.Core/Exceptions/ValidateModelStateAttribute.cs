using Microsoft.AspNetCore.Mvc.Filters;

namespace EntreLaunch.Exceptions;

public class ValidateModelStateAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .Select(e => new
                {
                    Name = e.Key,
                    Message = e.Value?.Errors.First().ErrorMessage
                }).ToArray();

            context.Result = new BadRequestObjectResult(new { Errors = errors });
        }
    }
}
