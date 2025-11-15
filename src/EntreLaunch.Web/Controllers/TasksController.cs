namespace EntreLaunch.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    public class TasksController : AuthenticatedController
    {
        private readonly IEnumerable<ITask> tasks;
        private readonly TaskRunner taskRunner;
        private readonly ILogger<TasksController> _logger;
        public TasksController(IEnumerable<ITask> tasks, TaskRunner taskRunner, IConfiguration configuration, ILogger<TasksController> logger)
        {
            this.taskRunner = taskRunner;
            this.tasks = tasks;
            _logger = logger;
        }

        /// <summary>
        /// Get all tasks.
        /// </summary>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await Task.Run(() => tasks.Select(t => CreateTaskDetailsDto(t)));
                if (!result.Any())
                {
                    return BadRequest(new GeneralResult(false, "no tasks found", null));
                }

                return Ok(new GeneralResult(true, "all tasks retrieved successfully", result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetTasks.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while getting tasks", Data = null });
            }
        }

        /// <summary>
        /// Get task details.
        /// </summary>
        [HttpGet("get-one/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(string name)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await Task.Run(() => tasks.Where(t => t.Name == name));
                if (!result.Any())
                {
                    return BadRequest(new GeneralResult { IsSuccess = false, Message = "Task not found", Data = null });
                }
                else
                {
                    return Ok(new GeneralResult { IsSuccess = true, Message = "Task found", Data = CreateTaskDetailsDto(result.First()) });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetTask.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while getting task", Data = null });
            }
        }

        /// <summary>
        /// Start task.
        /// </summary>
        [HttpGet("start/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public TaskDetailsDto Start(string name)
        {
            return StartOrStop(name, true);
        }

        /// <summary>
        /// Stop task.
        /// </summary>
        [HttpGet("stop/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public TaskDetailsDto Stop(string name)
        {
            return StartOrStop(name, false);
        }

        /// <summary>
        /// Execute task.
        /// </summary>
        [HttpGet("execute/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Execute(string name)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await Task.Run(() => tasks.Where(t => t.Name == name));
                if (!result.Any())
                {
                    return BadRequest(new GeneralResult { IsSuccess = false, Message = "Task not found", Data = name });
                }
                else
                {
                    var completed = await taskRunner.ExecuteTask(result.First());
                    return Ok(new GeneralResult
                    {
                        IsSuccess = true,
                        Message = "Task executed",
                        Data = new TaskExecutionDto
                        {
                            Name = name,
                            Completed = completed,
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in ExecuteTask.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while executing task", Data = null });
            }
        }

        /// <summary>
        /// Start or stop task.
        /// </summary>
        private TaskDetailsDto StartOrStop(string name, bool start)
        {
            var result = tasks.Where(t => t.Name == name);

            if (!result.Any())
            {
                throw new TaskNotFoundException(name);
            }
            else
            {
                var task = result.First();
                taskRunner.StartOrStopTask(task, start);
                return CreateTaskDetailsDto(task);
            }
        }

        /// <summary>
        /// Create task details dto.
        /// </summary>
        private TaskDetailsDto CreateTaskDetailsDto(ITask task)
        {
            return new TaskDetailsDto
            {
                Name = task.Name,
                CronSchedule = task.CronSchedule,
                RetryCount = task.RetryCount,
                RetryInterval = task.RetryInterval,
                IsRunning = task.IsRunning,
            };
        }
    }
}
