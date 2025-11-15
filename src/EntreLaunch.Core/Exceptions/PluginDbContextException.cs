using System.Runtime.Serialization;

namespace EntreLaunch.Exceptions;

[Serializable]
public class PluginDbContextException : Exception
{
    public PluginDbContextException()
        : base()
    {
    }

    public PluginDbContextException(string? message, Type? unregisteredDbContext)
        : base(message)
    {
        UnregisteredDbContext = unregisteredDbContext;
    }

    public PluginDbContextException(string? message, Type? unregisteredDbContext, Exception? innerException)
        : base(message, innerException)
    {
        UnregisteredDbContext = unregisteredDbContext;
    }

    public Type? UnregisteredDbContext { get; private set; }
}
