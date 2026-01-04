namespace EntreLaunch.Interfaces;
public interface ILockService
{
    /// <summary>
    /// Lock a key in the database and return a lock holder that can be used to release the lock when the lock holder is disposed.
    /// </summary>
    ILockHolder Lock(string key);

    /// <summary>
    /// Try to lock a key in the database and return a lock holder that can be used to release the lock when the lock holder is disposed.
    /// </summary>
    ILockHolder? TryLock(string key);
}

public interface ILockHolder
{
}
