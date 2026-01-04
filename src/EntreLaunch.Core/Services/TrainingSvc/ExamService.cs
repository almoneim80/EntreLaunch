using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.ExamDtos;
using EntreLaunch.DTOs.TrainingDtos;
using EntreLaunch.Extensions;

namespace EntreLaunch.Services.TrainingSvc
{
    public class ExamService(
        PgDbContext dbContext,
        ILogger<ExamService> logger,
        ILocalizationManager localizationManager) : IExamService
    {
        private readonly PgDbContext _dbContext = dbContext;
        private readonly ILogger<ExamService> _logger = logger;

        /// <inheritdoc/>
        public async Task<GeneralResult> CreateLessonExam(FullLessonExamDto dto)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var lessonExam = await _dbContext.Exams
                    .FirstOrDefaultAsync(e => e.LessonId == dto.LessonId && !e.IsDeleted && e.ParentEntityType == ExamParentEntityType.Lesson);
                if(lessonExam != null)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = localizationManager.GetLocalizedString("LessonAlreadyHasAnExam"),
                        Data = null
                    };
                }

                var exam = new Exam
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    MinMark = dto.MinMark,
                    MaxMark = dto.MaxMark,
                    DurationInMinutes = dto.DurationInMinutes,
                    LessonId = dto.LessonId,
                    ParentEntityType = dto.ParentEntityType,
                    CreatedAt = dto.CreatedAt ?? DateTimeOffset.UtcNow
                };

                _dbContext.Exams.Add(exam);
                await _dbContext.SaveChangesAsync();

                foreach (var q in dto.Questions)
                {
                    var question = new Question
                    {
                        Text = q.Text,
                        Mark = q.Mark,
                        ExamId = exam.Id
                    };

                    _dbContext.Questions.Add(question);
                    await _dbContext.SaveChangesAsync();

                    foreach (var a in q.Answers ?? new())
                    {
                        var answer = new Answer
                        {
                            Text = a.Text!,
                            IsCorrect = a.IsCorrect ?? false,
                            QuestionId = question.Id
                        };

                        _dbContext.Answers.Add(answer);
                    }
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = localizationManager.GetLocalizedString("ExamCreatedSuccessfully"),
                    Data = null
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating exam");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = localizationManager.GetLocalizedString("ErrorCreatingExam"),
                    Data = null
                };
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult> CreateCourseExam(FullCourseExamDto dto)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var lessonExam = await _dbContext.Exams
                    .FirstOrDefaultAsync(e => e.CourseId == dto.CourseId && !e.IsDeleted && e.ParentEntityType == ExamParentEntityType.Course);
                if (lessonExam != null)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = localizationManager.GetLocalizedString("CourseAlreadyHasAnExam"),
                        Data = null
                    };
                }

                var exam = new Exam
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    MinMark = dto.MinMark,
                    MaxMark = dto.MaxMark,
                    DurationInMinutes = dto.DurationInMinutes,
                    CourseId = dto.CourseId,
                    ParentEntityType = dto.ParentEntityType,
                    CreatedAt = dto.CreatedAt ?? DateTimeOffset.UtcNow
                };

                _dbContext.Exams.Add(exam);
                await _dbContext.SaveChangesAsync();

                foreach (var q in dto.Questions)
                {
                    var question = new Question
                    {
                        Text = q.Text!,
                        Mark = q.Mark ?? 0,
                        ExamId = exam.Id
                    };

                    _dbContext.Questions.Add(question);
                    await _dbContext.SaveChangesAsync();

                    foreach (var a in q.Answers ?? new())
                    {
                        var answer = new Answer
                        {
                            Text = a.Text!,
                            IsCorrect = a.IsCorrect ?? false,
                            QuestionId = question.Id
                        };

                        _dbContext.Answers.Add(answer);
                    }
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = localizationManager.GetLocalizedString("ExamCreatedSuccessfully"),
                    Data = null
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating course exam");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = localizationManager.GetLocalizedString("ErrorCreatingExam"),
                    Data = null
                };
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult> CreatePathExam(FullPathExamDto dto)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var exam = new Exam
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    MinMark = dto.MinMark,
                    MaxMark = dto.MaxMark,
                    ParentEntityType = dto.ParentEntityType,
                    CreatedAt = dto.CreatedAt ?? DateTimeOffset.UtcNow
                };

                _dbContext.Exams.Add(exam);
                await _dbContext.SaveChangesAsync();

                foreach (var q in dto.Questions)
                {
                    var question = new Question
                    {
                        Text = q.Text!,
                        Mark = q.Mark ?? 0,
                        ExamId = exam.Id
                    };

                    _dbContext.Questions.Add(question);
                    await _dbContext.SaveChangesAsync();

                    foreach (var a in q.Answers ?? new())
                    {
                        var answer = new Answer
                        {
                            Text = a.Text!,
                            IsCorrect = a.IsCorrect ?? false,
                            QuestionId = question.Id
                        };

                        _dbContext.Answers.Add(answer);
                    }
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = localizationManager.GetLocalizedString("PathfindingTestCreated"),
                    Data = null
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating path exam");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = localizationManager.GetLocalizedString("ErrorCreatingExam"),
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<ExamFullDetailsDto>> GetExamByLessonIdAsync(int lessonId)
        {
            try
            {
                var exam = await _dbContext.Exams
                    .AsNoTracking()
                    .Include(e => e.Lesson)
                    .Include(e => e.Questions!)
                        .ThenInclude(q => q.Answers)
                    .FirstOrDefaultAsync(e => e.LessonId == lessonId && !e.IsDeleted && e.ParentEntityType == ExamParentEntityType.Lesson);

                if (exam == null)
                {
                    _logger.LogWarning("Exam for lesson ID {lessonId} not found.", lessonId);
                    return new GeneralResult<ExamFullDetailsDto>(false, localizationManager.GetLocalizedString("ExamNotFound"));
                }

                var examDetails = new ExamFullDetailsDto
                {
                    ExamId = exam.Id,
                    Name = exam.Name,
                    Description = exam.Description,
                    MinMark = exam.MinMark ?? 0,
                    MaxMark = exam.MaxMark ?? 0,
                    DurationInMinutes = exam.DurationInMinutes ?? 0,
                    MaxAttempts = exam.MaxAttempts ?? 0,
                    Status = exam.Status,
                    ParentEntityName = exam.Lesson?.Name ?? "Lesson",
                    Questions = (exam.Questions ?? Enumerable.Empty<Question>())
                        .Where(q => !q.IsDeleted)
                        .Select(q => new QuestionDetailsData
                        {
                            QuestionId = q.Id,
                            Text = q.Text,
                            Mark = q.Mark ?? 0,
                            Answers = (q.Answers ?? Enumerable.Empty<Answer>())
                                .Where(a => !a.IsDeleted)
                                .Select(a => new AnswerDetailsData
                                {
                                    AnswerId = a.Id,
                                    Text = a.Text,
                                    IsCorrect = a.IsCorrect
                                }).ToList()
                        }).ToList()
                };

                return new GeneralResult<ExamFullDetailsDto>(true, localizationManager.GetLocalizedString("ExamDetailsRetrieved"), examDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching exam full details for lesson with Id {lessonId}.", lessonId);
                return new GeneralResult<ExamFullDetailsDto>(false, localizationManager.GetLocalizedString("ErrorFetchingExamDetails"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<ExamFullDetailsDto>> GetExamByCourseIdAsync(int courseId)
        {
            try
            {
                var exam = await _dbContext.Exams
                    .AsNoTracking()
                    .Include(e => e.Course)
                    .Include(e => e.Questions!)
                        .ThenInclude(q => q.Answers)
                    .FirstOrDefaultAsync(e => e.CourseId == courseId && !e.IsDeleted && e.ParentEntityType == ExamParentEntityType.Course);

                if (exam == null)
                {
                    _logger.LogWarning("Exam for course ID {courseId} not found.", courseId);
                    return new GeneralResult<ExamFullDetailsDto>(false, localizationManager.GetLocalizedString("ExamNotFound"));
                }

                var examDetails = new ExamFullDetailsDto
                {
                    ExamId = exam.Id,
                    Name = exam.Name,
                    Description = exam.Description,
                    MinMark = exam.MinMark ?? 0,
                    MaxMark = exam.MaxMark ?? 0,
                    DurationInMinutes = exam.DurationInMinutes ?? 0,
                    MaxAttempts = exam.MaxAttempts ?? 0,
                    Status = exam.Status,
                    ParentEntityName = exam.Course?.Name ?? "Course",
                    Questions = (exam.Questions ?? Enumerable.Empty<Question>())
                        .Where(q => !q.IsDeleted)
                        .Select(q => new QuestionDetailsData
                        {
                            QuestionId = q.Id,
                            Text = q.Text,
                            Mark = q.Mark ?? 0,
                            Answers = (q.Answers ?? Enumerable.Empty<Answer>())
                                .Where(a => !a.IsDeleted)
                                .Select(a => new AnswerDetailsData
                                {
                                    AnswerId = a.Id,
                                    Text = a.Text,
                                    IsCorrect = a.IsCorrect
                                }).ToList()
                        }).ToList()
                };

                return new GeneralResult<ExamFullDetailsDto>(true, localizationManager.GetLocalizedString("ExamDetailsRetrieved"), examDetails);
            }
            catch (Exception ex)
            { 
                _logger.LogError(ex, "An error occurred while fetching exam full details for course with Id {courseId}.", courseId);
                return new GeneralResult<ExamFullDetailsDto>(false, localizationManager.GetLocalizedString("ErrorFetchingExamDetails"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<ExamFullDetailsDto>>> GetPathExamsAsync(PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var query = _dbContext.Exams
                    .AsNoTracking()
                    .Include(e => e.Questions!)
                        .ThenInclude(q => q.Answers)
                    .Where(e => e.ParentEntityType == ExamParentEntityType.Path && !e.IsDeleted);

                var paged = await query
                    .Select(exam => new ExamFullDetailsDto
                    {
                        ExamId = exam.Id,
                        Name = exam.Name,
                        Description = exam.Description,
                        MinMark = exam.MinMark ?? 0,
                        MaxMark = exam.MaxMark ?? 0,
                        DurationInMinutes = exam.DurationInMinutes ?? 0,
                        MaxAttempts = exam.MaxAttempts ?? 0,
                        Status = exam.Status,
                        ParentEntityName = "Path Exam",
                        Questions = (exam.Questions ?? Enumerable.Empty<Question>())
                            .Where(q => !q.IsDeleted)
                            .Select(q => new QuestionDetailsData
                            {
                                QuestionId = q.Id,
                                Text = q.Text,
                                Mark = q.Mark ?? 0,
                                Answers = (q.Answers ?? Enumerable.Empty<Answer>())
                                    .Where(a => !a.IsDeleted)
                                    .Select(a => new AnswerDetailsData
                                    {
                                        AnswerId = a.Id,
                                        Text = a.Text,
                                        IsCorrect = a.IsCorrect
                                    }).ToList()
                            }).ToList()
                    })
                    .ToPagedResultAsync(pagination, cancellationToken);

                if (!paged.Items.Any())
                {
                    _logger.LogWarning("No path exams found.");
                    return new GeneralResult<PaginatedResult<ExamFullDetailsDto>>(false, localizationManager.GetLocalizedString("ExamNotFound"), null);
                }

                return new GeneralResult<PaginatedResult<ExamFullDetailsDto>>(true, localizationManager.GetLocalizedString("ExamDetailsRetrieved"), paged);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching path exams.");
                return new GeneralResult<PaginatedResult<ExamFullDetailsDto>>(false, localizationManager.GetLocalizedString("ErrorFetchingExamDetails"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> UpdateLessonExamAsync(int examId, UpdateLessonExamDto dto)
        {
            try
            {
                var exam = await _dbContext.Exams
                    .FirstOrDefaultAsync(e => e.Id == examId && !e.IsDeleted && e.ParentEntityType == ExamParentEntityType.Lesson);

                if (exam == null)
                    return new GeneralResult(false, localizationManager.GetLocalizedString("ExamNotFound"));

                exam.Name = dto.Name ?? exam.Name;
                exam.Description = dto.Description ?? exam.Description;
                exam.MinMark = dto.MinMark ?? exam.MinMark;
                exam.MaxMark = dto.MaxMark ?? exam.MaxMark;
                exam.DurationInMinutes = dto.DurationInMinutes ?? exam.DurationInMinutes;
                exam.LessonId = dto.LessonId ?? exam.LessonId;
                exam.UpdatedAt = dto.UpdatedAt ?? DateTimeOffset.UtcNow;

                await _dbContext.SaveChangesAsync();

                return new GeneralResult(true, localizationManager.GetLocalizedString("ExamUpdated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating lesson exam");
                return new GeneralResult(false, localizationManager.GetLocalizedString("ExamUpdateFailed"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> UpdateCourseExamAsync(int examId, UpdateCourseExamDto dto)
        {
            try
            {
                var exam = await _dbContext.Exams
                    .FirstOrDefaultAsync(e => e.Id == examId && !e.IsDeleted && e.ParentEntityType == ExamParentEntityType.Course);

                if (exam == null)
                    return new GeneralResult(false, localizationManager.GetLocalizedString("ExamNotFound"));

                exam.Name = dto.Name ?? exam.Name;
                exam.Description = dto.Description ?? exam.Description;
                exam.MinMark = dto.MinMark ?? exam.MinMark;
                exam.MaxMark = dto.MaxMark ?? exam.MaxMark;
                exam.DurationInMinutes = dto.DurationInMinutes ?? exam.DurationInMinutes;
                exam.CourseId = dto.CourseId ?? exam.CourseId;
                exam.UpdatedAt = dto.UpdatedAt ?? DateTimeOffset.UtcNow;

                await _dbContext.SaveChangesAsync();

                return new GeneralResult(true, localizationManager.GetLocalizedString("ExamUpdated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating course exam");
                return new GeneralResult(false, localizationManager.GetLocalizedString("ExamUpdateFailed"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> UpdatePathExamAsync(int examId, UpdatePathExamDto dto)
        {
            try
            {
                var exam = await _dbContext.Exams
                    .FirstOrDefaultAsync(e => e.Id == examId && !e.IsDeleted && e.ParentEntityType == ExamParentEntityType.Path);

                if (exam == null)
                    return new GeneralResult(false, localizationManager.GetLocalizedString("ExamNotFound"));

                exam.Name = dto.Name ?? exam.Name;
                exam.Description = dto.Description ?? exam.Description;
                exam.MinMark = dto.MinMark ?? exam.MinMark;
                exam.MaxMark = dto.MaxMark ?? exam.MaxMark;
                exam.DurationInMinutes = dto.DurationInMinutes ?? exam.DurationInMinutes;
                exam.UpdatedAt = dto.UpdatedAt ?? DateTimeOffset.UtcNow;

                await _dbContext.SaveChangesAsync();

                return new GeneralResult(true, localizationManager.GetLocalizedString("ExamUpdated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating path exam");
                return new GeneralResult(false, localizationManager.GetLocalizedString("ExamUpdateFailed"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> SoftDeleteExamAsync(int examId)
        {
            try
            {
                var exam = await _dbContext.Exams
                    .FirstOrDefaultAsync(e => e.Id == examId && !e.IsDeleted);

                if (exam == null)
                    return new GeneralResult(false, localizationManager.GetLocalizedString("ExamNotFound"));

                exam.IsDeleted = true;
                exam.UpdatedAt = DateTimeOffset.UtcNow;

                await _dbContext.SaveChangesAsync();

                return new GeneralResult(true, localizationManager.GetLocalizedString("ExamDeleted"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error soft-deleting exam {examId}", examId);
                return new GeneralResult(false, localizationManager.GetLocalizedString("ExamDeleteFailed"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<QuestionWithAnswersFullData>>> GetQuestionsWithAnswersByExamIdAsync(int examId)
        {
            try
            {
                var examExists = await _dbContext.Exams.AnyAsync(e => e.Id == examId && !e.IsDeleted);
                if (!examExists)
                {
                    _logger.LogWarning("Exam with ID {ExamId} not found or deleted.", examId);
                    return new GeneralResult<List<QuestionWithAnswersFullData>>(false, localizationManager.GetLocalizedString("ExamNotFound"));
                }

                var questions = await _dbContext.Questions
                    .AsNoTracking()
                    .Where(q => q.ExamId == examId && !q.IsDeleted)
                    .Select(q => new QuestionWithAnswersFullData
                    {
                        QuestionId = q.Id,
                        Text = q.Text,
                        Mark = q.Mark,
                        Answers = (q.Answers ?? Enumerable.Empty<Answer>())
                            .Where(a => !a.IsDeleted)
                            .Select(a => new AnswerFullData
                            {
                                AnswerId = a.Id,
                                Text = a.Text,
                                IsCorrect = a.IsCorrect
                            }).ToList()
                    })
                    .ToListAsync();

                if (!questions.Any())
                {
                    _logger.LogInformation("No questions found for ExamId {ExamId}.", examId);
                    return new GeneralResult<List<QuestionWithAnswersFullData>>(false, localizationManager.GetLocalizedString("NoExamQuestionsFound"));
                }

                return new GeneralResult<List<QuestionWithAnswersFullData>>(true, localizationManager.GetLocalizedString("ExamQuestionsRetrieved"), questions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching questions for ExamId {ExamId}.", examId);
                return new GeneralResult<List<QuestionWithAnswersFullData>>(false, localizationManager.GetLocalizedString("ErrorFetchingExamQuestions"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<ExamResultWithStudentDto?>> GetExamResultForStudentAsync(int examId, string userId)
        {
            try
            {
                var examExists = await _dbContext.Exams.AnyAsync(e => e.Id == examId && !e.IsDeleted);
                if (!examExists)
                {
                    _logger.LogWarning("Exam with ID {ExamId} not found or is deleted.", examId);
                    return new GeneralResult<ExamResultWithStudentDto?>(false, localizationManager.GetLocalizedString("ExamNotFound"), null);
                }

                // fetch student result
                var result = await _dbContext.ExamResults
                    .AsNoTracking()
                    .Where(er => er.ExamId == examId && er.UserId == userId)
                    .Select(er => new ExamResultWithStudentDto
                    {
                        ExamId = er.ExamId,
                        ExamName = er.Exam.Name,
                        ExamParent = er.Exam.ParentEntityType.ToString(),
                        StudentId = er.User.Id,
                        FirstName = er.User.FirstName ?? string.Empty,
                        LastName = er.User.LastName ?? string.Empty,
                        Email = er.User.Email ?? string.Empty,
                        Mark = er.Mark,
                        Status = er.Status
                    })
                    .FirstOrDefaultAsync();

                if (result == null)
                {
                    _logger.LogInformation("No result found for Exam ID {ExamId} and User ID {UserId}.", examId, userId);
                    return new GeneralResult<ExamResultWithStudentDto?>(false, localizationManager.GetLocalizedString("NoExamResultFound"), null);
                }

                return new GeneralResult<ExamResultWithStudentDto?>(true, localizationManager.GetLocalizedString("ResultRetrieved"), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the result for Exam ID {ExamId} and User ID {UserId}.", examId, userId);
                return new GeneralResult<ExamResultWithStudentDto?>(false, localizationManager.GetLocalizedString("ErrorRetrievingExamResult"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<ExamResultDto>> CalculateExamResultAsync(int examId, string userId, List<UserAnswerDto> userAnswers, int timeTakenInSeconds)
        {
            try
            {
                if (userAnswers == null)
                {
                    return new GeneralResult<ExamResultDto>(false, localizationManager.GetLocalizedString("NoUserAnswers"));
                }

                // Checking the existence of the test
                var exam = await _dbContext.Exams.Include(e => e.Questions!).ThenInclude(q => q.Answers)
                    .FirstOrDefaultAsync(e => e.Id == examId && !e.IsDeleted);

                if (exam == null)
                {
                    _logger.LogWarning("Exam with ID {ExamId} not found or is deleted.", examId);
                    return new GeneralResult<ExamResultDto>(false, localizationManager.GetLocalizedString("ExamDeleted"));
                }

                // Extracting the correct answers
                var correctAnswers = exam.Questions!.SelectMany(q => q.Answers!).Where(a => a.IsCorrect).ToList();

                // Calculating grades
                decimal totalMark = exam.Questions!.Sum(q => q.Mark ?? 0);
                decimal obtainedMark = 0;

                foreach (var userAnswer in userAnswers)
                {
                    var correctAnswer = correctAnswers.FirstOrDefault(a => a.QuestionId == userAnswer.QuestionId);
                    if (correctAnswer != null && correctAnswer.Id == userAnswer.AnswerId)
                    {
                        var question = exam.Questions!.FirstOrDefault(q => q.Id == userAnswer.QuestionId);
                        if (question != null)
                        {
                            obtainedMark += question.Mark ?? 0;
                        }
                    }
                }

                // Completion Rate Calculation
                var completionPercentage = (decimal)userAnswers.Count / exam.Questions!.Count * 100;

                // Return Result
                return new GeneralResult<ExamResultDto>(true, localizationManager.GetLocalizedString("ResultCalculated"),
                    new ExamResultDto
                    {
                        ExamName = exam.Name ?? string.Empty,
                        OriginalDuration = exam.DurationInMinutes * 60 ?? 0,
                        TimeTakenInSeconds = timeTakenInSeconds,
                        CompletionPercentage = Math.Round(completionPercentage, 2),
                        MaxMark = totalMark,
                        ObtainedMark = obtainedMark
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating exam result for user {UserId} in exam {ExamId}.", userId, examId);
                return new GeneralResult<ExamResultDto>(false, localizationManager.GetLocalizedString("ErrorCalculatingExamResult"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<StudentComparisonDto>> CompareStudentResultWithBatchAsync(int examId, string userId)
        {
            try
            {
                // Checking the existence of the test
                var examExists = await _dbContext.Exams.AnyAsync(e => e.Id == examId);
                if (!examExists)
                {
                    _logger.LogWarning("Exam with ID {ExamId} not found.", examId);
                    return new GeneralResult<StudentComparisonDto>(false, localizationManager.GetLocalizedString("ExamNotFound"));
                }

                // Calculating the average results of the batch
                var batchAverage = await _dbContext.ExamResults.Where(r => r.ExamId == examId).AverageAsync(r => r.Mark ?? 0);

                // Fetch Student Result
                var studentResult = await _dbContext.ExamResults.FirstOrDefaultAsync(r => r.ExamId == examId && r.UserId == userId);

                if (studentResult == null)
                {
                    _logger.LogWarning("No result found for the student.");
                    return new GeneralResult<StudentComparisonDto>(false, localizationManager.GetLocalizedString("NoResultForStudent"));
                }

                // Create a result object
                return new GeneralResult<StudentComparisonDto>(true, localizationManager.GetLocalizedString("StudentComparisonCalculated"),
                    new StudentComparisonDto
                    {
                        ExamId = examId,
                        UserId = userId,
                        StudentMark = studentResult.Mark ?? 0,
                        BatchAverageMark = batchAverage,
                        ComparisonStatus = studentResult.Mark >= batchAverage ? "Above Average" : "Below Average"
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error comparing student result for User {UserId} in Exam {ExamId}.", userId, examId);
                return new GeneralResult<StudentComparisonDto>(false, localizationManager.GetLocalizedString("ErrorComparingStudentResult"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<ExamStatisticsDto>> GetExamStatisticsAsync(int examId)
        {
            try
            {
                // Checking the existence of the test
                var examExists = await _dbContext.Exams.AnyAsync(e => e.Id == examId);
                if (!examExists)
                {
                    _logger.LogWarning("Exam with ID {ExamId} not found.", examId);
                    return new GeneralResult<ExamStatisticsDto>(false, localizationManager.GetLocalizedString("ExamNotFound"));
                }

                // Fetching results
                var results = await _dbContext.ExamResults.Where(r => r.ExamId == examId)
                    .Select(r => r.Mark ?? 0).ToListAsync();

                if (!results.Any())
                {
                    _logger.LogWarning("No results found for Exam {ExamId}.", examId);
                    return new GeneralResult<ExamStatisticsDto>(false, localizationManager.GetLocalizedString("NoResultsFound"));
                }

                // Calculating statistics
                _logger.LogInformation("Calculating statistics for Exam {ExamId}.", examId);
                return new GeneralResult<ExamStatisticsDto>(true, localizationManager.GetLocalizedString("StatisticsCalculated"),
                    new ExamStatisticsDto
                    {
                        ExamId = examId,
                        AverageMark = Math.Round(results.Average(), 2),
                        MinimumMark = results.Min(),
                        MaximumMark = results.Max(),
                        TotalParticipants = results.Count
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating average result for Exam {ExamId}.", examId);
                return new GeneralResult<ExamStatisticsDto>(false, localizationManager.GetLocalizedString("ErrorCalculatingAverageResult"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<TopStudentDto>>> GetTopTenStudentsAsync(int examId)
        {
            try
            {
                // Checking the existence of the test
                var examExists = await _dbContext.Exams.AnyAsync(e => e.Id == examId);
                if (!examExists)
                {
                    _logger.LogWarning("Exam with ID {ExamId} not found.", examId);
                    return new GeneralResult<List<TopStudentDto>>(false, localizationManager.GetLocalizedString("ExamNotFound"));
                }

                // Fetching and organizing student results
                var topStudents = await _dbContext.ExamResults.Where(r => r.ExamId == examId)
                    .OrderByDescending(r => r.Mark).Take(10).Select((r, index) =>
                    new TopStudentDto
                    {
                        UserId = r.UserId,
                        StudentName = $"{r.User.FirstName} {r.User.LastName}",
                        Mark = r.Mark ?? 0,
                        Rank = index + 1
                    }).ToListAsync();

                if (!topStudents.Any())
                {
                    _logger.LogWarning("No results found for Exam {ExamId}.", examId);
                    return new GeneralResult<List<TopStudentDto>>(false, localizationManager.GetLocalizedString("NoResultsFound"));
                }

                _logger.LogInformation("Retrieved top students for Exam {ExamId}.", examId);
                return new GeneralResult<List<TopStudentDto>>(true, localizationManager.GetLocalizedString("TopStudentsRetrieved"), topStudents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving top students for Exam {ExamId}.", examId);
                return new GeneralResult<List<TopStudentDto>>(false, localizationManager.GetLocalizedString("ErrorRetrievingTopStudents"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<bool>> CanRetakeExamAsync(int examId, string userId)
        {
            try
            {
                // Fetch Exam
                var exam = await _dbContext.Exams.FirstOrDefaultAsync(e => e.Id == examId);
                if (exam == null)
                {
                    _logger.LogWarning("Exam with ID {ExamId} not found.", examId);
                    return new GeneralResult<bool>(false, localizationManager.GetLocalizedString("ExamNotFound"));
                }

                // Checking the number of previous attempts
                var attemptCount = await _dbContext.ExamResults.CountAsync(r => r.ExamId == examId && r.UserId == userId);

                // Checking the maximum number of attempts
                var maxAttemptsAllowed = exam.MaxAttempts ?? 1;
                _logger.LogInformation("User {UserId} has attempted {AttemptCount} out of {MaxAttemptsAllowed} for Exam {ExamId}.", userId, attemptCount, maxAttemptsAllowed, examId);
                return new GeneralResult<bool>(true, localizationManager.GetLocalizedString("CanRetakeExam"), attemptCount < maxAttemptsAllowed);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user {UserId} can retake Exam {ExamId}.", userId, examId);
                return new GeneralResult<bool>(false, localizationManager.GetLocalizedString("ErrorCheckingRetakePermission"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<ExamResultDto>> RetakeExamAsync(int examId, string userId, List<UserAnswerDto> userAnswers, int timeTakenInSeconds)
        {
            try
            {
                // Checking the possibility of retaking the exam
                var canRetake = await CanRetakeExamAsync(examId, userId);
                if (!canRetake.IsSuccess)
                {
                    _logger.LogWarning("User {UserId} has reached the max number of attempts for Exam {ExamId}.", userId, examId);
                    return new GeneralResult<ExamResultDto>(false, localizationManager.GetLocalizedString("MaxAttemptsReached"));
                }

                // Disabling previous attempts
                await DeactivatePreviousAttemptsAsync(examId, userId);

                // Calculating the new score
                var result = await CalculateExamResultAsync(examId, userId, userAnswers, timeTakenInSeconds);

                var previousAttempts = await _dbContext.ExamResults
                    .Where(r => r.ExamId == examId && r.UserId == userId)
                    .ToListAsync();

                // Recording the new score
                var newExamResult = new ExamResult
                {
                    ExamId = examId,
                    UserId = userId,
                    Mark = result.Data!.ObtainedMark,
                    Status = result.Data!.ObtainedMark >= result.Data.MaxMark / 2 ? "Passed" : "Failed",
                    AttemptNumber = previousAttempts.Count + 1,
                    IsActive = true,
                    CreatedAt = DateTimeOffset.UtcNow
                };

                _dbContext.ExamResults.Add(newExamResult);
                await _dbContext.SaveChangesAsync();

                // Logs
                _logger.LogInformation("New attempt recorded for Exam {ExamId} by User {UserId}: AttemptNumber = {AttemptNumber}, Mark = {Mark}, Status = {Status}.",
                    examId, userId, newExamResult.AttemptNumber, newExamResult.Mark, newExamResult.Status);

                return new GeneralResult<ExamResultDto>(true, localizationManager.GetLocalizedString("RetakeExamCreated"), result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retaking Exam {ExamId} for User {UserId}.", examId, userId);
                return new GeneralResult<ExamResultDto>(false, localizationManager.GetLocalizedString("ErrorRetakingExam"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<StudentAttemptsWithBestDto>> GetStudentAttemptsAsync(int examId, string userId)
        {
            try
            {
                var attempts = await _dbContext.ExamResults.Where(r => r.ExamId == examId && r.UserId == userId)
        .OrderBy(r => r.AttemptNumber).Select(r => new StudentAttemptDto
        {
            AttemptNumber = r.AttemptNumber,
            Mark = r.Mark ?? 0,
            Status = r.Status,
            IsActive = r.IsActive,
            AttemptDate = r.CreatedAt ?? DateTimeOffset.UtcNow,
        }).ToListAsync();
                if (!attempts.Any())
                {
                    _logger.LogWarning("No attempts found for User {UserId} and Exam {ExamId}.", userId, examId);
                    return new GeneralResult<StudentAttemptsWithBestDto>(false, localizationManager.GetLocalizedString("NoAttemptsFound"));
                }

                var bestAttempt = attempts.OrderByDescending(a => a.Mark).FirstOrDefault();

                return new GeneralResult<StudentAttemptsWithBestDto>(true, localizationManager.GetLocalizedString("AttemptsRetrieved"),
                    new StudentAttemptsWithBestDto
                    {
                        Attempts = attempts,
                        BestAttempt = bestAttempt
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting attempts for User {UserId} and Exam {ExamId}.", userId, examId);
                return new GeneralResult<StudentAttemptsWithBestDto>(false, localizationManager.GetLocalizedString("ErrorGettingAttempts"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<StudentAttemptDto?>> GetActiveResultAsync(int examId, string userId)
        {
            try
            {
                var activeResult = await _dbContext.ExamResults
                        .Where(r => r.ExamId == examId && r.UserId == userId && r.IsActive)
                        .Select(r => new StudentAttemptDto
                        {
                            AttemptNumber = r.AttemptNumber,
                            Mark = r.Mark ?? 0,
                            Status = r.Status,
                            IsActive = r.IsActive,
                            AttemptDate = r.CreatedAt ?? DateTimeOffset.UtcNow
                        }).FirstOrDefaultAsync();
                if (activeResult == null)
                {
                    _logger.LogWarning("No active result found for User {UserId} and Exam {ExamId}.", userId, examId);
                    return new GeneralResult<StudentAttemptDto?>(false, localizationManager.GetLocalizedString("NoActiveResult"));
                }

                _logger.LogInformation("Active result retrieved for User {UserId} and Exam {ExamId}.", userId, examId);
                return new GeneralResult<StudentAttemptDto?>(true, localizationManager.GetLocalizedString("ActiveResultRetrieved"), activeResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active result for User {UserId} and Exam {ExamId}.", userId, examId);
                return new GeneralResult<StudentAttemptDto?>(false, localizationManager.GetLocalizedString("ErrorGettingActiveResult"));
            }
        }

        /// <summary>
        /// Disabling previous attempts.
        /// </summary>
        private async Task DeactivatePreviousAttemptsAsync(int examId, string userId)
        {
            var previousAttempts = await _dbContext.ExamResults.Where(r => r.ExamId == examId && r.UserId == userId)
                .ToListAsync();

            foreach (var attempt in previousAttempts)
            {
                attempt.IsActive = false;
            }

            _dbContext.UpdateRange(previousAttempts);
        }
    }
}
