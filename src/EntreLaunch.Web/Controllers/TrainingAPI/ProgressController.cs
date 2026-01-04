namespace EntreLaunch.Web.Controllers.TrainingAPI
{
    [ApiController]
    [Authorize(Roles = AppRoles.TrainingRoles)]
    [Route("api/[controller]")]
    public class ProgressController(
        ILocalizationManager localization,
        IStudentProgress progressService,
        ILogger<ProgressController> logger) : AuthenticatedController(localization)
    {
        private readonly IStudentProgress _progressService = progressService;
        private readonly ILogger<ProgressController> _logger = logger;
        private readonly ILocalizationManager _localization = localization;

        /// <summary>
        /// Marks a specified course lesson as completed for the current user based on the provided progress data.
        /// </summary>
        [HttpPost("lesson/mark-complete/")]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ProgressPermissions.MarkLessonCompleted)]
        public async Task<IActionResult> MarkLessonCompleted([FromQuery] int lessonId, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _progressService.MarkLessonCompletedAsync(lessonId, CurrentUserId!, cancellationToken);
                if (result.IsSuccess == false)
                {
                    return result.ErrorType switch
                    {
                        ErrorType.BadRequest => BadRequest(result),
                        ErrorType.NotFound => NotFound(result),
                        ErrorType.InternalServerError => StatusCode(500, result),
                        _ => BadRequest(result)
                    };
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An Unexpected error has occurred while marking a course lesson as completed.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = _localization.GetLocalizedString("UnexpectedError_MarkLesson"), Data = null });
                    }
        }

        /// <summary>
        /// Retrieves the progress of the current user for a specified lesson.
        /// </summary>
        [HttpGet("lesson/get-progress/")]
        [ProducesResponseType(typeof(GeneralResult<LessonProgressDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ProgressPermissions.GetLessonProgress)]
        public async Task<IActionResult> GetLessonProgress([FromQuery] int lessonId, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _progressService.GetLessonProgressAsync(lessonId, CurrentUserId!, cancellationToken);
                if (result.IsSuccess == false)
                {
                    return result.ErrorType switch
                    {
                        ErrorType.BadRequest => BadRequest(result),
                        ErrorType.NotFound => NotFound(result),
                        ErrorType.InternalServerError => StatusCode(500, result),
                        _ => BadRequest(result)
                    };
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An Unexpected error has occurred while getting lesson progress.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = _localization.GetLocalizedString("UnexpectedError_GetLessonProgress"), Data = null });
            }
        }

        /// <summary>
        /// Retrieves a list of lessons completed by the current user in the specified course.
        /// </summary>
        [HttpGet("lesson/get-completed/")]
        [ProducesResponseType(typeof(GeneralResult<LessonProgressDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ProgressPermissions.GetCompletedLessons)]
        public async Task<IActionResult> GetCompletedLessons([FromQuery] int courseId, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _progressService.GetCourseLessonsProgressAsync(courseId, CurrentUserId!, cancellationToken);
                if (result.IsSuccess == false)
                {
                    return result.ErrorType switch
                    {
                        ErrorType.BadRequest => BadRequest(result),
                        ErrorType.NotFound => NotFound(result),
                        ErrorType.InternalServerError => StatusCode(500, result),
                        _ => BadRequest(result)
                    };
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An Unexpected error has occurred while getting completed lessons.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = _localization.GetLocalizedString("UnexpectedError_GetLessonProgressList"), Data = null });
            }
        }

        /// <summary>
        /// Updates the progress of a user in a specific training course.
        /// </summary>
        [HttpPatch("course/update-progress/")]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ProgressPermissions.UpdateCourseProgress)]
        public async Task<IActionResult> UpdateCourseProgress([FromQuery] int courseId, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _progressService.UpdateCourseProgressAsync(courseId, CurrentUserId!, cancellationToken);
                if (result.IsSuccess == false)
                {
                    return result.ErrorType switch
                    {
                        ErrorType.BadRequest => BadRequest(result),
                        ErrorType.NotFound => NotFound(result),
                        ErrorType.InternalServerError => StatusCode(500, result),
                        _ => BadRequest(result)
                    };
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An Unexpected error has occurred while updating course progress.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = _localization.GetLocalizedString("UnexpectedError_UpdateCourse"), Data = null });
            }
        }

        /// <summary>
        /// Retrieves the progress of the current user in the specified course.
        /// </summary>
        [HttpGet("course/get-progress/")]
        [ProducesResponseType(typeof(GeneralResult<CourseProgressDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ProgressPermissions.GetCourseProgress)]
        public async Task<IActionResult> GetCourseProgress([FromQuery] int courseId, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _progressService.GetCourseProgressAsync(courseId, CurrentUserId!, cancellationToken);
                if (result.IsSuccess == false)
                {
                    return result.ErrorType switch
                    {
                        ErrorType.BadRequest => BadRequest(result),
                        ErrorType.NotFound => NotFound(result),
                        ErrorType.InternalServerError => StatusCode(500, result),
                        _ => BadRequest(result)
                    };
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An Unexpected error has occurred while getting course progress.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = _localization.GetLocalizedString("UnexpectedError_GetCourseProgress"), Data = null });
            }
        }

        /// <summary>
        /// Retrieves the progress statuses of the current user's courses.
        /// </summary>
        [HttpGet("course/user-progress")]
        [ProducesResponseType(typeof(GeneralResult<CourseProgressDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ProgressPermissions.GetUserCoursesProgress)]
        public async Task<IActionResult> GetUserCoursesProgress(CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _progressService.GetUserCoursesProgressAsync(CurrentUserId!, cancellationToken);
                if (result.IsSuccess == false)
                {
                    return result.ErrorType switch
                    {
                        ErrorType.BadRequest => BadRequest(result),
                        ErrorType.NotFound => NotFound(result),
                        ErrorType.InternalServerError => StatusCode(500, result),
                        _ => BadRequest(result)
                    };
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An Unexpected error has occurred while getting user course progress.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = _localization.GetLocalizedString("UnexpectedError_GetCourseProgressList"), Data = null });
            }
        }

        /// <summary>
        /// Updates the user's progress in a specific training path.
        /// </summary>
        [HttpPatch("path/update-progress/")]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ProgressPermissions.UpdatePathProgress)]
        public async Task<IActionResult> UpdatePathProgress([FromQuery] int pathId, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _progressService.UpdateTrainingPathProgressAsync(pathId, CurrentUserId!, cancellationToken);
                if (result.IsSuccess == false)
                {
                    return result.ErrorType switch
                    {
                        ErrorType.BadRequest => BadRequest(result),
                        ErrorType.NotFound => NotFound(result),
                        ErrorType.InternalServerError => StatusCode(500, result),
                        _ => BadRequest(result)
                    };
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An Unexpected error has occurred while updating program progress.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = _localization.GetLocalizedString("UnexpectedError_UpdatePath"), Data = null });
            }
        }

        /// <summary>
        /// Retrieves the user's progress in a specific training path.
        /// </summary>
        [HttpGet("path/get-progress/")]
        [ProducesResponseType(typeof(GeneralResult<TrainingPathProgressDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ProgressPermissions.GetPathProgress)]
        public async Task<IActionResult> GetPathProgress([FromQuery] int pathId, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _progressService.GetTrainingPathProgressAsync(pathId, CurrentUserId!, cancellationToken);
                if (result.IsSuccess == false)
                {
                    return result.ErrorType switch
                    {
                        ErrorType.BadRequest => BadRequest(result),
                        ErrorType.NotFound => NotFound(result),
                        ErrorType.InternalServerError => StatusCode(500, result),
                        _ => BadRequest(result)
                    };
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An Unexpected error has occurred while getting program progress.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = _localization.GetLocalizedString("UnexpectedError_GetPathProgress"), Data = null });
            }
        }

        /// <summary>
        /// Retrieves the user's overall progress in all training paths.
        /// </summary>
        [HttpGet("path/user-progress")]
        [ProducesResponseType(typeof(GeneralResult<TrainingPathProgressDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ProgressPermissions.GetUserPathsProgress)]
        public async Task<IActionResult> GetUserPathsProgress(CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _progressService.GetUserTrainingPathsProgressAsync(CurrentUserId!, cancellationToken);
                if (result.IsSuccess == false)
                {
                    return result.ErrorType switch
                    {
                        ErrorType.BadRequest => BadRequest(result),
                        ErrorType.NotFound => NotFound(result),
                        ErrorType.InternalServerError => StatusCode(500, result),
                        _ => BadRequest(result)
                    };
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An Unexpected error has occurred while getting user program progress.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = _localization.GetLocalizedString("UnexpectedError_GetTrainingPathProgressList"), Data = null });
            }
        }

        /// <summary>
        /// Starts a new session for a specific lesson.
        /// </summary>
        [HttpPost("lesson/session/start")]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ProgressPermissions.StartLessonSession)]
        public async Task<IActionResult> StartLessonSession([FromQuery] int lessonId, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _progressService.StartLessonSessionAsync(CurrentUserId!, lessonId, cancellationToken);
                if (result.IsSuccess == false)
                {
                    return result.ErrorType switch
                    {
                        ErrorType.BadRequest => BadRequest(result),
                        ErrorType.NotFound => NotFound(result),
                        ErrorType.InternalServerError => StatusCode(500, result),
                        _ => BadRequest(result)
                    };
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while starting lesson session.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = _localization.GetLocalizedString("UnexpectedErrorStartingSession"), Data = null });
            }
        }

        /// <summary>
        /// Ends an active session for a specific lesson.
        /// </summary>
        [HttpPost("lesson/session/end")]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ProgressPermissions.EndLessonSession)]
        public async Task<IActionResult> EndLessonSession([FromQuery] int lessonId, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _progressService.EndLessonSessionAsync(CurrentUserId!, lessonId, cancellationToken);
                if (result.IsSuccess == false)
                {
                    return result.ErrorType switch
                    {
                        ErrorType.BadRequest => BadRequest(result),
                        ErrorType.NotFound => NotFound(result),
                        ErrorType.InternalServerError => StatusCode(500, result),
                        _ => BadRequest(result)
                    };
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while ending lesson session.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = _localization.GetLocalizedString("UnexpectedErrorEndingSession"), Data = null });
            }
        }

        /// <summary>
        /// Synchronizes all user progress records for a specified training path.
        /// </summary>
        [HttpPost("path/sync-progress")]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(ProgressPermissions.SyncProgramProgress)]
        public async Task<IActionResult> SyncProgramProgress([FromQuery] int pathId, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _progressService.SyncAllUserProgressForPathAsync(pathId, cancellationToken);
                if (result.IsSuccess == false)
                {
                    return result.ErrorType switch
                    {
                        ErrorType.BadRequest => BadRequest(result),
                        ErrorType.NotFound => NotFound(result),
                        ErrorType.InternalServerError => StatusCode(500, result),
                        _ => BadRequest(result)
                    };
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while syncing progress for program.");
                return StatusCode(500,
                    new GeneralResult { IsSuccess = false, Message = _localization.GetLocalizedString("UnexpectedErrorSyncingProgress"), Data = null });
            }
        }
    }
}
