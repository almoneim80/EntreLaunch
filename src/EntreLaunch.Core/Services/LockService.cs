namespace EntreLaunch.Services;

/// <summary>
/// Lock service class.
/// </summary>
public class LockService : ILockService
{
    private readonly string connectionSting;
    public LockService(IConfiguration configuration)
    {
        var postgresConfig = configuration.GetSection("Postgres").Get<PostgresConfig>();

        if (postgresConfig == null)
        {
            throw new MissingConfigurationException("Postgres configuration is mandatory.");
        }

        connectionSting = postgresConfig.ConnectionString;
    }

    /// <inheritdoc/>
    public ILockHolder Lock(string key)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public ILockHolder? TryLock(string key)
    {
        var secondaryLock = new PostgresDistributedLock(new PostgresAdvisoryLockKey(key, true), connectionSting);

        var postgresDistributedLock = secondaryLock.TryAcquire();

        if (postgresDistributedLock is null)
        {
            return null;
        }
        else
        {
            return new PostgresLockHolder();
        }
    }
}

/// <summary>
/// PostgresLock holder class.
/// </summary>
public class PostgresLockHolder : ILockHolder
{
    public PostgresLockHolder()
    {
    }
}
