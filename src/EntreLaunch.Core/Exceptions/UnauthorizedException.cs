using System.Runtime.Serialization;

namespace EntreLaunch.Exceptions;

[Serializable]
public class UnauthorizedException : Exception
{
    public UnauthorizedException()
        : base("Failed to login")
    {
    }
}
