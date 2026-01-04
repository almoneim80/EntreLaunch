using Microsoft.AspNetCore.Mvc.Filters;
namespace EntreLaunch.Web.Filters;

public class ConvertDateTimesToUtcFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument == null) continue;

            var props = argument.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                if (prop.PropertyType == typeof(DateTimeOffset))
                {
                    var value = (DateTimeOffset)prop.GetValue(argument)!;
                    prop.SetValue(argument, value.ToUniversalTime());
                }
                else if (prop.PropertyType == typeof(DateTime))
                {
                    var value = (DateTime)prop.GetValue(argument)!;
                    if (value.Kind != DateTimeKind.Utc)
                    {
                        prop.SetValue(argument, DateTime.SpecifyKind(value, DateTimeKind.Utc));
                    }
                }
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // No action needed after execution
    }
}
