using System.Runtime.Serialization;

namespace EntreLaunch.Exceptions;

[Serializable]
public class EmailException : Exception
{
    public EmailException()
    {
    }

    public EmailException(string? message)
        : base(message)
    {
    }

    public EmailException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
