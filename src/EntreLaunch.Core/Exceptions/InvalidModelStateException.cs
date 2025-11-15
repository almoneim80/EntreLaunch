using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EntreLaunch.Exceptions;

public class InvalidModelStateException : Exception
{
    public InvalidModelStateException(ModelStateDictionary modelState)
    {
        ModelState = modelState;
    }

    public ModelStateDictionary? ModelState { get; init; }
}
