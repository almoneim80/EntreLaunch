using Microsoft.AspNetCore.Http.HttpResults;
using System.Runtime.Serialization;

namespace EntreLaunch.Exceptions;

[Serializable]
public class TooManyRequestsException : Exception
{
    public TooManyRequestsException()
    {
    }

    public TooManyRequestsException(string? message)
        : base(message)
    {
    }

    public TooManyRequestsException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
