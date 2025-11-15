namespace EntreLaunch.Services.TaskSvc
{
    public class TaskStatusService
    {
        private readonly Dictionary<string, bool> taskStatusByName = new Dictionary<string, bool>();
        public void SetInitialState(string name, bool running)
        {
            if (!taskStatusByName.ContainsKey(name))
            {
                taskStatusByName[name] = running;
            }
        }

        /// <summary>
        /// Returns true if the task is running false otherwise.
        /// </summary>
        public bool IsRunning(string name)
        {
            if (taskStatusByName.ContainsKey(name))
            {
                return taskStatusByName[name];
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Sets the running status of the task with the given name to the given value.
        /// </summary>
        public void SetRunning(string name, bool running)
        {
            taskStatusByName[name] = running;
        }
    }
}
