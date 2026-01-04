namespace EntreLaunch.Exceptions;

[Serializable]
public class TaskNotFoundException : Exception
{
    public TaskNotFoundException(string taskName)
    {
        TaskName = taskName;
    }

    public string TaskName { get; }
}
