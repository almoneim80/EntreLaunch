using EntreLaunch.DTOs;

namespace EntreLaunch.Services.TrainingSvc
{
    public class TrainingSectionService : ITrainingSectionService
    {
        private readonly PgDbContext _dbContext;
        private readonly ILogger<TrainingSectionService> _logger;
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;
        private readonly IRefundService _refundService;
        private readonly IConfiguration _configuration;
        private readonly IRoleService _roleService;
        private readonly IEmailVerificationExtension _emailVerificationExtension;
        public TrainingSectionService(
            PgDbContext dbContext,
            ILogger<TrainingSectionService> logger,
            IPaymentService paymentService,
            IMapper mapper,
            IRefundService refundService,
            IConfiguration configuration,
            IRoleService roleService,
            IEmailVerificationExtension emailVerificationExtension)
        {
            _dbContext = dbContext;
            _logger = logger;
            _paymentService = paymentService;
            _mapper = mapper;
            _refundService = refundService;
            _configuration = configuration;
            _roleService = roleService;
            _emailVerificationExtension = emailVerificationExtension;
        }

        /// <inheritdoc />
        public async Task<GeneralResult<(int totalEnrollments, List<EnrollmentWithUserData> enrollments)>> GetCourseEnrollmentsWithUsersAsync(int courseId)
        {
            try
            {
                // 1. Checking the existence of the Course
                bool courseExists = await _dbContext.Courses.AnyAsync(c => c.Id == courseId && !c.IsDeleted);
                if (!courseExists)
                {
                    _logger.LogWarning("Course with ID {CourseId} not found.", courseId);
                    return new GeneralResult<(int totalEnrollments, List<EnrollmentWithUserData> enrollments)>(false, "Course not found.");
                }

                // 2. Fetching enrollment and student data
                var enrollments = await _dbContext.CourseEnrollments.Where(e => e.CourseId == courseId && !e.IsDeleted)
                    .Select(e => new EnrollmentWithUserData
                    {
                        FirstName = e.User.FirstName,
                        LastName = e.User.LastName,
                        NationalId = e.User.NationalId,
                        Email = e.User.Email,
                        EnrolledAt = e.EnrolledAt
                    }).ToListAsync();

                // 3. Return the number and list of enrollments
                return new GeneralResult<(int totalEnrollments, List<EnrollmentWithUserData> enrollments)>(true, "Enrollments retrieved successfully.", (enrollments.Count, enrollments));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving enrollments for course.");
                return new GeneralResult<(int totalEnrollments, List<EnrollmentWithUserData> enrollments)>(false, "An error occurred while retrieving enrollments.");
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> VerifyCourseEnrollmentEligibilityAsync(int courseId, string userId)
        {
            try
            {
                // Checking the existence of the course
                var course = await _dbContext.Courses.FirstOrDefaultAsync(c => c.Id == courseId && !c.IsDeleted);
                if (course == null)
                {
                    _logger.LogWarning("Course with ID {CourseId} not found or is deleted.", courseId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Course not found or is deleted.",
                    };
                }

                // checking the existence of the user
                var userExists = await _dbContext.Users
                    .AnyAsync(u => u.Id == userId && !u.IsDeleted);
                if (!userExists)
                {
                    _logger.LogWarning("User with ID {UserId} not found or is deleted.", userId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "User not found or is deleted.",
                    };
                }

                // Check if the user is already enrolled in the course
                var alreadyEnrolled = await _dbContext.CourseEnrollments
                    .AnyAsync(e => e.CourseId == courseId && e.UserId == userId && !e.IsDeleted);
                if (alreadyEnrolled)
                {
                    _logger.LogError($"User with Id {userId} already enrolled in course with Id {courseId}");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "You are already enrolled in this course",
                    };
                }

                // check the value of Max Enrollment of the course
                if (course.MaxEnrollment.HasValue)
                {
                    int currentEnrollments = await _dbContext.CourseEnrollments
                        .Where(e => e.CourseId == courseId && !e.IsDeleted).CountAsync();

                    if (currentEnrollments >= course.MaxEnrollment.Value)
                    {
                        _logger.LogWarning($"This course has reached its maximum enrollment limit. Course ID: {courseId}");
                        return new GeneralResult
                        {
                            IsSuccess = false,
                            Message = "This course has reached its maximum enrollment limit.",
                        };
                    }
                }

                // 4. Payment Verification
                if (!course.IsFree)
                {
                    var hasPaid = await _paymentService.IsPaid(courseId, userId);
                    if (!hasPaid)
                    {
                        _logger.LogError($"User With Id {userId} dont completed payment for this course {courseId}");
                        return new GeneralResult
                        {
                            IsSuccess = false,
                            Message = "You have not completed payment for this course.",
                        };
                    }
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Everything is fine",
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while enrolling a user to a course.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An error occurred while enrolling a user to a course.",
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<CourseDetailsDto>>> GetCourseBasedOnStatusAsync(int status)
        {
            try
            {
                // Validating status.
                if (!Enum.IsDefined(typeof(CourseStatus), status))
                {
                    _logger.LogWarning("Invalid status value {Status} for CourseStatus enum.", status);
                    return new GeneralResult<List<CourseDetailsDto>>(false, "Invalid status value.");
                }

                // Convert status to CourseStatus
                var parsedStatus = (CourseStatus)status;

                // Fetch courses from the database based on courseId and status (parsedStatus)
                var courses = await _dbContext.Courses.AsNoTracking().Where(c => c.Status == parsedStatus && !c.IsDeleted).ToListAsync();

                // Map the fetched courses to CourseDetailsDto
                var result = _mapper.Map<List<CourseDetailsDto>>(courses);

                if (!result.Any())
                {
                    _logger.LogInformation("No courses found with Status '{Status}'", status);
                    return new GeneralResult<List<CourseDetailsDto>>(false, "No courses found.");
                }

                return new GeneralResult<List<CourseDetailsDto>>(true, "Courses retrieved successfully.", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving courses for Status '{Status}'", status);
                return new GeneralResult<List<CourseDetailsDto>>(false, "Error retrieving courses.");
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> ChangeCourseStatusAsync(int courseId, CourseStatus newStatus)
        {
            try
            {
                // Fetching the course from the database
                var course = await _dbContext.Courses.FirstOrDefaultAsync(c => c.Id == courseId && !c.IsDeleted);
                if (course == null)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Course not found or is deleted.",
                    };
                }

                // course Status Change
                course.Status = newStatus;
                course.EntreLaunchdatedAt = DateTimeOffset.UtcNow;
                await _dbContext.SaveChangesAsync();
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Course status changed successfully.",
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing course status for Course ID {courseId} to {newStatus}", courseId, newStatus);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Error changing course status.",
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<bool>> ReorderLessonsAsync(int courseId, List<LessonReorderDto> newOrderList)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Make sure the list is not empty
                if (newOrderList == null || !newOrderList.Any())
                {
                    _logger.LogWarning("No lesson order data provided for course ID {CourseId}.", courseId);
                    return new GeneralResult<bool>(false, "No lesson order data provided.", false);
                }

                // Checking for negative or zero ordering
                if (newOrderList.Exists(x => x.OrderIndex <= 0))
                {
                    _logger.LogWarning("Detected negative or zero OrderIndex in the reorder list for course ID {CourseId}.", courseId);
                    return new GeneralResult<bool>(false, "Detected negative or zero OrderIndex in the reorder list.", false);
                }

                // OrderIndex dEntreLaunchlicate check (to prevent having two lessons with the same order)
                var orderIndices = newOrderList.Select(x => x.OrderIndex).ToList();
                if (orderIndices.Distinct().Count() != orderIndices.Count)
                {
                    _logger.LogWarning("DEntreLaunchlicate OrderIndex detected in the reorder list for course ID {CourseId}.", courseId);
                    return new GeneralResult<bool>(false, "DEntreLaunchlicate OrderIndex detected in the reorder list.", false);
                }

                // Collect the identifiers of the lessons to be arranged
                var lessonIds = newOrderList.Select(x => x.LessonId).Distinct().ToList();

                // Fetching lessons from the database
                var lessons = await _dbContext.Lessons.Where(l => lessonIds.Contains(l.Id) && l.CourseId == courseId
                                && !l.IsDeleted).ToListAsync();

                // Checking that no lessons are missing in DB compared to the submitted list
                if (lessons.Count != lessonIds.Count)
                {
                    // May mean that some LessonId does not exist or does not belong to the course
                    _logger.LogWarning("Some lessons in the reorder list were not found or do not belong to course {CourseId}.", courseId);
                    return new GeneralResult<bool>(false, "Some lessons in the reorder list were not found or do not belong to course.", false);
                }

                // EntreLaunchdate the OrderIndex field for each lesson according to the incoming values
                foreach (var lesson in lessons)
                {
                    var matchingDto = newOrderList.FirstOrDefault(x => x.LessonId == lesson.Id);
                    if (matchingDto != null)
                    {
                        // You can add a check on the value itself: Is it negative, does it repeat itself with another lesson, etc.
                        lesson.OrderIndex = matchingDto.OrderIndex;
                    }
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return new GeneralResult<bool>(true, "Lessons reordered successfully.", true);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error reordering lessons for course ID {CourseId}.", courseId);
                return new GeneralResult<bool>(false, "Error reordering lessons.", false);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> EnrollUserToCourseAsync(CourseEnrollmentCreateDto createDto)
        {
            // Don't use using here if _dbContext is managed by DI
            var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Verify the existence of the course and recognize a course that is not previously deleted.
                var newStudent = _mapper.Map<CourseEnrollment>(createDto);
                _dbContext.CourseEnrollments.Add(newStudent);
                await _dbContext.SaveChangesAsync();

                // Assign the student role
                var result = await _roleService.IsUserInRoleAsync(createDto.UserId, "Student");
                if (result.Data == false)
                {
                    await _roleService.AssignRoleAsync(createDto.UserId, "Student");
                }

                // EntreLaunchdate the course enrollment count
                await _dbContext.Courses
                    .Where(c => c.Id == createDto.CourseId && !c.IsDeleted)
                    .ExecuteEntreLaunchdateAsync(c => c.SetProperty(course => course.CurrentEnrollmentCount, course => course.CurrentEnrollmentCount + 1)
                    .SetProperty(course => course.EntreLaunchdatedAt, _ => DateTimeOffset.UtcNow));

                await transaction.CommitAsync();

                var userEmail = await _dbContext.Users.Where(u => u.Id == createDto.UserId).Select(u => u.Email).FirstOrDefaultAsync();
                if (userEmail == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found or is deleted.", createDto.UserId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "User not found or is deleted.",
                    };
                }

                await _emailVerificationExtension.SendEmailAsync(userEmail, "Enrollment in a course", "Congratulations! You have successfully enrolled to the course.");
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "User enrolled successfully.",
                };
            }
            catch (DbEntreLaunchdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                return new GeneralResult(false, "Course not found or is deleted.");
            }
            catch
            {
                await transaction.RollbackAsync();
                return new GeneralResult(false, "Error enrolling user.");
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<(double AverageRating, int RatingCount)>> GetCourseRatingStatisticsAsync(int courseId)
        {
            try
            {
                // Fetch all active (not deleted) evaluations for this course
                var ratings = await _dbContext.CourseRatings
                    .Where(r => r.CourseId == courseId && !r.IsDeleted && r.Status == RatingStatus.Approved).ToListAsync();
                if (!ratings.Any())
                {
                    // There is no rating
                    return new GeneralResult<(double AverageRating, int RatingCount)>(false, "There is no rating for this course.", (0, 0));
                }

                double avg = ratings.Average(r => r.Rating);
                int count = ratings.Count;
                return new GeneralResult<(double AverageRating, int RatingCount)>(true, "Rating statistics retrieved successfully.", (avg, count));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting rating statistics for Course {CourseId}.", courseId);
                return new GeneralResult<(double AverageRating, int RatingCount)>(false, "Error getting rating statistics.", (0, 0));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<CourseRatingDetailsDto>>> GetAllRatingsForCourseAsync(int courseId)
        {
            try
            {
                var ratings = await _dbContext.CourseRatings.Where(r => r.CourseId == courseId && !r.IsDeleted && r.Status == RatingStatus.Approved)
                    .OrderByDescending(r => r.CreatedAt).ToListAsync();

                if (!ratings.Any())
                {
                    return new GeneralResult<List<CourseRatingDetailsDto>>(false, "There is no rating for this course.", null);
                }

                var result = _mapper.Map<List<CourseRatingDetailsDto>>(ratings);
                return new GeneralResult<List<CourseRatingDetailsDto>>(true, "Ratings retrieved successfully.", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ratings for Course {CourseId}.", courseId);
                return new GeneralResult<List<CourseRatingDetailsDto>>(false, "Error retrieving ratings.", null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> UnenrollUserFromCourseAsync(int courseId, string userId)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Verify the existence of the subscription and recognize a subscription that is not previously deleted.
                var enrollment = await _dbContext.CourseEnrollments.FirstOrDefaultAsync(e =>
                        e.CourseId == courseId && e.UserId == userId && !e.IsDeleted);

                if (enrollment == null)
                {
                    _logger.LogWarning("Enrollment not found for CourseId {CourseId} and UserId {UserId}.", courseId, userId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Enrollment not found."
                    };
                }

                var course = await _dbContext.Courses.FirstOrDefaultAsync(c => c.Id == courseId && !c.IsDeleted);
                if (course == null)
                {
                    _logger.LogWarning("Course not found for CourseId {CourseId}.", courseId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Course not found."
                    };
                }


                if (course.IsFree)
                {
                    enrollment.IsDeleted = true;
                    enrollment.IsActive = false;
                    enrollment.DeletedAt = DateTimeOffset.UtcNow;
                    _dbContext.CourseEnrollments.EntreLaunchdate(enrollment);
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation("User {UserId} has been successfully unenrolled from course {CourseId}.", userId, courseId);

                    // Check if the user has any active enrollments or remove the "Student" role
                    if (!await HasActiveEnrollmentsAsync(userId))
                    {
                        var result = await _roleService.IsUserInRoleAsync(userId, "Student");
                        if (result.Data == true)
                        {
                            await _roleService.RemoveRoleAsync(userId, "Student");
                        }
                    }

                    var userEmail = await _dbContext.Users.Where(u => u.Id == userId).Select(u => u.Email).FirstOrDefaultAsync();
                    if (userEmail == null)
                    {
                        _logger.LogWarning("User with ID {UserId} not found or is deleted.", userId);
                        return new GeneralResult
                        {
                            IsSuccess = false,
                            Message = "User not found or is deleted.",
                        };
                    }

                    await _emailVerificationExtension.SendEmailAsync(userEmail, "Unenrollment from a course", "We regret that you unsubscribed from this course");

                    return new GeneralResult
                    {
                        IsSuccess = true,
                        Message = "You have been successfully unenrolled from the course."
                    };
                }
                else
                {
                    var isPaid = await _paymentService.IsPaid(courseId, userId);
                    if (!isPaid)
                    {
                        _logger.LogWarning("User {UserId} has not paid for course {CourseId}.", userId, courseId);
                        return new GeneralResult
                        {
                            IsSuccess = false,
                            Message = "You have not paid for this course."
                        };
                    }

                    enrollment.IsDeleted = true;
                    enrollment.IsActive = false;
                    enrollment.DeletedAt = DateTimeOffset.UtcNow;
                    _dbContext.CourseEnrollments.EntreLaunchdate(enrollment);
                    await _dbContext.SaveChangesAsync();

                    var payment = _dbContext.Payments.FirstOrDefault(p => p.TargetId == courseId && p.UserId == userId && !p.IsDeleted);
                    if (payment == null)
                    {
                        _logger.LogError("Payment not found for CourseId {CourseId} and UserId {UserId}.", courseId, userId);
                        return new GeneralResult
                        {
                            IsSuccess = false,
                            Message = "Payment not found."
                        };
                    }

                    var refund = new RefundCreateDto
                    {
                        PaymentId = payment.Id,
                        Reason = $"Unenroll from course that has Id {courseId}.",
                    };

                    var addRefundResult = await _refundService.CreateRefundAsync(refund);
                    if (addRefundResult == null)
                    {
                        _logger.LogWarning("Refund not created for CourseId {CourseId} and UserId {UserId}.", courseId, userId);
                        return new GeneralResult
                        {
                            IsSuccess = false,
                            Message = "Refund not created."
                        };
                    }

                    // Check if the user has any active enrollments or remove the "Student" role
                    if (!await HasActiveEnrollmentsAsync(userId))
                    {
                        var result = await _roleService.IsUserInRoleAsync(userId, "Student");
                        if (result.Data == true)
                        {
                            await _roleService.RemoveRoleAsync(userId, "Student");
                        }
                    }

                    await transaction.CommitAsync();
                    _logger.LogInformation("User {UserId} has been successfully unenrolled from course {CourseId}.", userId, courseId);
                    return new GeneralResult
                    {
                        IsSuccess = true,
                        Message = "You have been successfully unenrolled from the course. Please wait for the refund to be processed."
                    };
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while unenrolling User {UserId} from Course {CourseId}.", userId, courseId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Error occurred while unenrolling from the course."
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<UserSubscriptionDto>>> GetUserSubscriptionsAsync(string userId)
        {
            try
            {
                // check if the user exists
                var userExists = await _dbContext.Users.AnyAsync(u => u.Id == userId && !u.IsDeleted);
                if (!userExists)
                {
                    _logger.LogWarning("User with ID {UserId} not found or is deleted.", userId);
                    return new GeneralResult<List<UserSubscriptionDto>>(false, "User not found or is deleted.");
                }

                // retrieve the user's subscriptions
                var subscriptions = await _dbContext.CourseEnrollments.AsNoTracking().Include(e => e.Course)
                    .Where(e => e.UserId == userId && !e.IsDeleted && e.IsActive)
                    .Select(e => new UserSubscriptionDto
                    {
                        SubscriptionId = e.Id,
                        CourseId = e.CourseId,
                        CourseName = e.Course.Name ?? "Unknown",
                        IsActive = e.IsActive ? "Active" : "Canceled",
                        EnrolledAt = e.EnrolledAt,
                        LastEntreLaunchdatedAt = e.EntreLaunchdatedAt
                    }).OrderByDescending(e => e.EnrolledAt).ToListAsync();

                return new GeneralResult<List<UserSubscriptionDto>>(true, "Subscriptions retrieved successfully.", subscriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscriptions for user ID {UserId}.", userId);
                return new GeneralResult<List<UserSubscriptionDto>>(false, "Error retrieving subscriptions.");
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<CourseDetailsDto>>> GetCoursesByPriceTypeAsync(bool isFree)
        {
            try
            {
                if (isFree)
                {
                    var freeCourses = await _dbContext.Courses.AsNoTracking()
                        .Where(c => !c.IsDeleted && c.Price == 0 && isFree == true).ToListAsync();

                    if (!freeCourses.Any())
                    {
                        _logger.LogInformation("No free courses found.");
                        return new GeneralResult<List<CourseDetailsDto>>(false, "No free courses found.");
                    }

                    var freeCoursesResult = _mapper.Map<List<CourseDetailsDto>>(freeCourses);
                    return new GeneralResult<List<CourseDetailsDto>>(true, "Free courses retrieved successfully.", freeCoursesResult);
                }

                var paidCourses = await _dbContext.Courses.AsNoTracking()
                    .Where(c => !c.IsDeleted && c.Price == 0 && isFree == false).ToListAsync();

                if (!paidCourses.Any())
                {
                    _logger.LogInformation("No paid courses found.");
                    return new GeneralResult<List<CourseDetailsDto>>(false, "No paid courses found.");
                }

                var paidCoursesResult = _mapper.Map<List<CourseDetailsDto>>(paidCourses);
                return new GeneralResult<List<CourseDetailsDto>>(true, "Paid courses retrieved successfully.", paidCoursesResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving courses by price type: {Is free}", isFree);
                return new GeneralResult<List<CourseDetailsDto>>(false, "Error occurred while retrieving courses by price type.");
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<InstructorDetails>>> GetInstructorsByCourseIdAsync(int courseId)
        {
            try
            {
                // Checking the existence of the chorus
                var courseExists = await _dbContext.Courses.AnyAsync(c => c.Id == courseId && !c.IsDeleted);
                if (!courseExists)
                {
                    _logger.LogWarning("Course with ID {CourseId} not found or deleted.", courseId);
                    return new GeneralResult<List<InstructorDetails>>(false, "Course not found or deleted.");
                }

                // Bringing in the instructors associated with the course
                var instructors = await _dbContext.CourseInstructors
                     .AsNoTracking()
                     .Include(ci => ci.User)
                     .Where(ci => ci.CourseId == courseId && !ci.IsDeleted && !ci.User.IsDeleted)
                     .Select(ci => new InstructorDetails
                     {
                         InstructorId = ci.User.Id,
                         FirstName = ci.User.FirstName,
                         LastName = ci.User.LastName,
                         Email = ci.User.Email,
                         Specialization = ci.User.Specialization,
                         ProfilePicture = ci.User.AvatarUrl,
                     }).ToListAsync();

                return new GeneralResult<List<InstructorDetails>>(true, "Instructors retrieved successfully.", instructors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving instructors for course ID {CourseId}.", courseId);
                return new GeneralResult<List<InstructorDetails>>(false, "Error occurred while retrieving instructors.");
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<TrainerPerformanceDto>> GetTrainerPerformanceAsync(string trainerId)
        {
            try
            {
                // Bring the courses that the trainer has taught
                var courses = await _dbContext.CourseInstructors
                    .AsNoTracking()
                    .Where(ci => ci.UserId == trainerId && !ci.IsDeleted)
                    .Select(ci => ci.Course)
                    .Where(c => c != null && !c.IsDeleted)
                    .ToListAsync();

                // If the instructor does not teach any courses
                if (!courses.Any())
                {
                    return new GeneralResult<TrainerPerformanceDto>(false, "Instructor does not teach any courses.", null);
                }

                // Fetch statistics for each course
                var courseStatistics = new List<CourseStatisticsDto>();
                foreach (var course in courses)
                {
                    // count of students participating in the course
                    var studentCount = await _dbContext.CourseEnrollments.AsNoTracking()
                        .CountAsync(ce => ce.CourseId == course.Id && !ce.IsDeleted);

                    // Ratings (average rating and number of ratings)
                    var ratings = await _dbContext.CourseRatings.AsNoTracking()
                        .Where(cr => cr.CourseId == course.Id && !cr.IsDeleted).ToListAsync();

                    var averageRating = ratings.Any() ? ratings.Average(r => r.Rating) : 0;
                    var totalRatings = ratings.Count;

                    // result of each course
                    courseStatistics.Add(new CourseStatisticsDto
                    {
                        CourseId = course.Id,
                        CourseName = course.Name ?? "Unnamed Course",
                        StudentCount = studentCount,
                        AverageRating = averageRating,
                        TotalRatings = totalRatings
                    });
                }

                // build the result of performance
                return new GeneralResult<TrainerPerformanceDto>(true, "Trainer performance retrieved successfully.",
                    new TrainerPerformanceDto
                    {
                        TotalCourses = courses.Count,
                        Courses = courseStatistics
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching performance data for trainer {TrainerId}.", trainerId);
                return new GeneralResult<TrainerPerformanceDto>(false, "An error occurred while fetching performance data.");
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<bool>> CanStudentRateCourseAsync(string studentId, int courseId)
        {
            try
            {
                // checking the existence of the student
                var studentExists = await _dbContext.Users.AnyAsync(u => u.Id == studentId && !u.IsDeleted);
                if (!studentExists)
                {
                    _logger.LogWarning("Student with ID {StudentId} not found or is deleted.", studentId);
                    return new GeneralResult<bool>(false, "Student not found or is deleted.");
                }

                // checking the existence of the course
                var courseExists = await _dbContext.Courses.AnyAsync(c => c.Id == courseId && !c.IsDeleted);
                if (!courseExists)
                {
                    _logger.LogWarning("Course with ID {CourseId} not found or is deleted.", courseId);
                    return new GeneralResult<bool>(false, "Course not found or is deleted.");
                }

                // checking if the student has rated the course
                var hasRated = await _dbContext.CourseRatings
                    .AnyAsync(r => r.UserId == studentId && r.CourseId == courseId && !r.IsDeleted);

                return new GeneralResult<bool>(true, "Check completed successfully.", !hasRated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking if student {StudentId} has rated course {CourseId}.", studentId, courseId);
                return new GeneralResult<bool>(false, "An error occurred while checking if student has rated course.");
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<CourseRatingSummaryDto>> GetCourseRatingSummaryAsync(int courseId)
        {
            try
            {
                // Check for the presence of the course
                var courseExists = await _dbContext.Courses.AnyAsync(c => c.Id == courseId && !c.IsDeleted);
                if (!courseExists)
                {
                    _logger.LogWarning("Course with ID {CourseId} not found or is deleted.", courseId);
                    return new GeneralResult<CourseRatingSummaryDto>(false, "Course not found or is deleted.");
                }

                // Calculate the average and number of ratings
                var ratingSummary = await _dbContext.CourseRatings.Where(r => r.CourseId == courseId && !r.IsDeleted && r.Status == RatingStatus.Approved)
                    .GroEntreLaunchBy(r => r.CourseId).Select(g => new CourseRatingSummaryDto
                    {
                        CourseId = g.Key,
                        AverageRating = g.Average(r => r.Rating),
                        TotalRatings = g.Count()
                    })
                    .FirstOrDefaultAsync();

                // If there are no evaluations, return a default result
                if (ratingSummary == null)
                {
                    ratingSummary = new CourseRatingSummaryDto
                    {
                        CourseId = courseId,
                        AverageRating = 0,
                        TotalRatings = 0
                    };
                }

                return new GeneralResult<CourseRatingSummaryDto>(true, "Rating summary retrieved successfully.", ratingSummary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the rating summary for course ID {CourseId}.", courseId);
                return new GeneralResult<CourseRatingSummaryDto>(false, "An error occurred while fetching the rating summary.");
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<CourseRatingsDto>>> GetRatingsByInstructorAsync(string instructorId)
        {
            try
            {
                // التحقق من وجود المدرب
                var instructorExists = await _dbContext.Users
                    .AnyAsync(u => u.Id == instructorId && !u.IsDeleted);
                if (!instructorExists)
                {
                    return new GeneralResult<List<CourseRatingsDto>>(false, "Instructor not found.");
                }

                // Fetch assessments associated with courses taught by the instructor
                var ratings = await _dbContext.CourseRatings.AsNoTracking()
                    .Where(cr => cr.Course.CourseInstructors!.Any(ci => ci.UserId == instructorId && !ci.IsDeleted)
                    && !cr.IsDeleted && cr.Status == RatingStatus.Approved)
                    .Select(cr => new CourseRatingsDto
                    {
                        Id = cr.Id,
                        CourseId = cr.CourseId,
                        CourseName = cr.Course.Name,
                        ReviewerName = cr.User.FirstName + " " + cr.User.LastName,
                        Rating = cr.Rating,
                        Comment = cr.Review,
                        CreatedAt = cr.CreatedAt ?? DateTimeOffset.UtcNow,
                    })
                    .ToListAsync();

                // If no ratings are found
                if (!ratings.Any())
                {
                    _logger.LogInformation("No ratings found for instructor with ID {InstructorId}.", instructorId);
                    return new GeneralResult<List<CourseRatingsDto>>(true, "No ratings found.");
                }

                return new GeneralResult<List<CourseRatingsDto>>(true, "Ratings retrieved successfully.", ratings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving ratings for instructor with ID {InstructorId}.", instructorId);
                return new GeneralResult<List<CourseRatingsDto>>(false, "An error occurred while retrieving ratings.");
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> IncrementAttachmentOpenCountAsync(int attachmentId)
        {
            try
            {
                // Search for the desired facility
                var attachment = await _dbContext.LessonAttachments
                    .FirstOrDefaultAsync(a => a.Id == attachmentId && !a.IsDeleted);

                if (attachment == null)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Attachment not found or has been deleted."
                    };
                }

                // Increase counter
                attachment.OpenCount++;
                _dbContext.LessonAttachments.EntreLaunchdate(attachment);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Attachment open count incremented successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error incrementing open count for attachment ID {AttachmentId}", attachmentId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An error occurred while incrementing the open count."
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<AttachmentStatsDto?>> GetAttachmentStatsAsync(int attachmentId)
        {
            try
            {
                // Facility Search
                var attachment = await _dbContext.LessonAttachments.AsNoTracking()
                    .FirstOrDefaultAsync(a => a.Id == attachmentId && !a.IsDeleted);

                if (attachment == null)
                {
                    return new GeneralResult<AttachmentStatsDto?>(false, "Attachment not found or has been deleted.", null);
                }

                // Set EntreLaunch statistics as a DTO object
                return new GeneralResult<AttachmentStatsDto?>(true, "Attachment statistics retrieved successfully.",
                    new AttachmentStatsDto
                    {
                        AttachmentId = attachment.Id,
                        FileName = attachment.FileName,
                        OpenCount = attachment.OpenCount,
                        CreatedAt = attachment.CreatedAt ?? DateTimeOffset.UtcNow,
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving statistics for attachment ID {AttachmentId}", attachmentId);
                return new GeneralResult<AttachmentStatsDto?>(false, "An error occurred while retrieving statistics.");
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<bool>> IsValidFile(string filePath)
        {
            try
            {
                // Extracting the extension from the path
                var fileExtension = await Task.Run(() => Path.GetExtension(filePath).ToLowerInvariant());

                // Read the settings for all categories from appsettings.json
                var allCategories = await Task.Run(() => _configuration.GetSection("FileEntreLaunchloadSettings").Get<Dictionary<string, FileCategorySettings>>());
                if (allCategories == null || allCategories.Count == 0)
                {
                    return new GeneralResult<bool>(false, "No file categories configured.", false);
                }

                // Find the category matching the extension
                var matchingCategory = allCategories.FirstOrDefault(c => c.Value.Extensions.Contains(fileExtension));
                if (matchingCategory.Value == null)
                {
                    return new GeneralResult<bool>(false, $"File type '{fileExtension}' is not allowed.", false);
                }

                // Check file size based on category
                var maxSize = matchingCategory.Value.MaxSizePerExtension.ContainsKey(fileExtension)
                    ? matchingCategory.Value.MaxSizePerExtension[fileExtension]
                    : matchingCategory.Value.MaxSizePerExtension["default"];

                var fileInfo = new FileInfo(filePath);
                if (!IsFileSizeValid(fileInfo.Length, maxSize))
                {
                    return new GeneralResult<bool>(false, $"File size exceeds the maximum allowed size of {maxSize} for '{fileExtension}'.", false);
                }

                return new GeneralResult<bool>(true, "File is valid.", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while validating the file.");
                return new GeneralResult<bool>(false, "An error occurred while validating the file.", false);
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
                    return new GeneralResult<ExamResultWithStudentDto?>(false, "Exam not found.", null);
                }

                // جلب نتيجة الطالب المحددة
                var result = await _dbContext.ExamResults.AsNoTracking()
                    .Where(er => er.ExamId == examId && er.UserId == userId)
                    .Select(er => new ExamResultWithStudentDto
                    {
                        ExamId = er.ExamId,
                        StudentId = er.User.Id,
                        FirstName = er.User.FirstName ?? string.Empty,
                        LastName = er.User.LastName ?? string.Empty,
                        Email = er.User.Email ?? string.Empty,
                        Mark = er.Mark,
                        Status = er.Status
                    }).FirstOrDefaultAsync();

                if (result == null)
                {
                    _logger.LogInformation("No result found for Exam ID {ExamId} and User ID {UserId}.", examId, userId);
                    return new GeneralResult<ExamResultWithStudentDto?>(false, "No result found.", null);
                }

                return new GeneralResult<ExamResultWithStudentDto?>(true, "Result retrieved successfully.", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the result for Exam ID {ExamId} and User ID {UserId}.", examId, userId);
                return new GeneralResult<ExamResultWithStudentDto?>(false, "An error occurred while retrieving the result.");
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<ExamResultDto>> CalculateExamResultAsync(int examId, string userId, List<UserAnswerDto> userAnswers, int timeTakenInSeconds)
        {
            try
            {
                if (userAnswers == null)
                {
                    return new GeneralResult<ExamResultDto>(false, "No user answers provided.");
                }

                // Checking the existence of the test
                var exam = await _dbContext.Exams.Include(e => e.Questions!).ThenInclude(q => q.Answers)
                    .FirstOrDefaultAsync(e => e.Id == examId && !e.IsDeleted);

                if (exam == null)
                {
                    _logger.LogWarning("Exam with ID {ExamId} not found or is deleted.", examId);
                    return new GeneralResult<ExamResultDto>(false, "Exam not found or is deleted.");
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
                return new GeneralResult<ExamResultDto>(true, "Result calculated successfully.",
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
                return new GeneralResult<ExamResultDto>(false, "Error calculating exam result.");
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
                    return new GeneralResult<StudentComparisonDto>(false, "Exam not found.");
                }

                // Calculating the average results of the batch
                var batchAverage = await _dbContext.ExamResults.Where(r => r.ExamId == examId).AverageAsync(r => r.Mark ?? 0);

                // Fetch Student Result
                var studentResult = await _dbContext.ExamResults.FirstOrDefaultAsync(r => r.ExamId == examId && r.UserId == userId);

                if (studentResult == null)
                {
                    _logger.LogWarning("No result found for the student.");
                    return new GeneralResult<StudentComparisonDto>(false, "No result found for the student.");
                }

                // Create a result object
                return new GeneralResult<StudentComparisonDto>(true, "Result calculated successfully.",
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
                return new GeneralResult<StudentComparisonDto>(false, "Error comparing student result.");
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
                    return new GeneralResult<ExamStatisticsDto>(false, "Exam not found.");
                }

                // Fetching results
                var results = await _dbContext.ExamResults.Where(r => r.ExamId == examId)
                    .Select(r => r.Mark ?? 0).ToListAsync();

                if (!results.Any())
                {
                    _logger.LogWarning("No results found for Exam {ExamId}.", examId);
                    return new GeneralResult<ExamStatisticsDto>(false, "No results found.");
                }

                // Calculating statistics
                _logger.LogInformation("Calculating statistics for Exam {ExamId}.", examId);
                return new GeneralResult<ExamStatisticsDto>(true, "Statistics calculated successfully.",
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
                return new GeneralResult<ExamStatisticsDto>(false, "Error calculating average result.");
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
                    return new GeneralResult<List<TopStudentDto>>(false, "Exam not found.");
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
                    return new GeneralResult<List<TopStudentDto>>(false, "No results found.");
                }

                _logger.LogInformation("Retrieved top students for Exam {ExamId}.", examId);
                return new GeneralResult<List<TopStudentDto>>(true, "Top students retrieved successfully.", topStudents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving top students for Exam {ExamId}.", examId);
                return new GeneralResult<List<TopStudentDto>>(false, "Error retrieving top students.");
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
                    return new GeneralResult<bool>(false, "Exam not found.");
                }

                // Checking the number of previous attempts
                var attemptCount = await _dbContext.ExamResults.CountAsync(r => r.ExamId == examId && r.UserId == userId);

                // Checking the maximum number of attempts
                var maxAttemptsAllowed = exam.MaxAttempts ?? 1;
                _logger.LogInformation("User {UserId} has attempted {AttemptCount} out of {MaxAttemptsAllowed} for Exam {ExamId}.", userId, attemptCount, maxAttemptsAllowed, examId);
                return new GeneralResult<bool>(true, "Can retake the exam.", attemptCount < maxAttemptsAllowed);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user {UserId} can retake Exam {ExamId}.", userId, examId);
                return new GeneralResult<bool>(false, "Error checking if user can retake the exam.");
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
                    return new GeneralResult<ExamResultDto>(false, "You have reached the maximum number of attempts for this exam.");
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

                return new GeneralResult<ExamResultDto>(true, "Retake Exam Result created successfully.", result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retaking Exam {ExamId} for User {UserId}.", examId, userId);
                return new GeneralResult<ExamResultDto>(false, "Error retaking the exam.");
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
                    return new GeneralResult<StudentAttemptsWithBestDto>(false, "No attempts found.");
                }

                var bestAttempt = attempts.OrderByDescending(a => a.Mark).FirstOrDefault();

                return new GeneralResult<StudentAttemptsWithBestDto>(true, "Attempts retrieved successfully.",
                    new StudentAttemptsWithBestDto
                    {
                        Attempts = attempts,
                        BestAttempt = bestAttempt
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting attempts for User {UserId} and Exam {ExamId}.", userId, examId);
                return new GeneralResult<StudentAttemptsWithBestDto>(false, "Error getting attempts.");
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
                    return new GeneralResult<StudentAttemptDto?>(false, "No active result found.");
                }

                _logger.LogInformation("Active result retrieved for User {UserId} and Exam {ExamId}.", userId, examId);
                return new GeneralResult<StudentAttemptDto?>(true, "Active result retrieved successfully.", activeResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active result for User {UserId} and Exam {ExamId}.", userId, examId);
                return new GeneralResult<StudentAttemptDto?>(false, "Error getting active result.");
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<CertificateResult<StudentCertificateDto>>> CheckIfCertificateIssuedAsync(int enrollmentId, int courseId)
        {
            try
            {
                // Input Validation
                if (enrollmentId <= 0 || courseId <= 0)
                {
                    _logger.LogWarning("Invalid input parameters. EnrollmentId: {EnrollmentId}, CourseId: {CourseId}.", enrollmentId, courseId);
                    return new GeneralResult<CertificateResult<StudentCertificateDto>>(false, "Invalid input parameters.");
                }

                // Search the database
                var certificate = await _dbContext.StudentCertificates.AsNoTracking()
                    .Where(sc => sc.EnrollmentId == enrollmentId && sc.CourseId == courseId)
                    .Select(sc => new StudentCertificateDto
                    {
                        Id = sc.Id,
                        EnrollmentId = sc.EnrollmentId,
                        CourseId = sc.CourseId,
                        Url = sc.Url,
                        IssuedAt = sc.IssuedAt,
                        DeliveryMethod = sc.DeliveryMethod,
                        ShippingStatus = sc.ShippingStatus,
                        ShippingAddress = sc.ShippingAddress
                    }).FirstOrDefaultAsync();

                // Verify the existence of the certificate
                if (certificate == null)
                {
                    return new GeneralResult<CertificateResult<StudentCertificateDto>>(false, "Certificate not found.");
                }

                return new GeneralResult<CertificateResult<StudentCertificateDto>>(true, "Certificate found.",
                    new CertificateResult<StudentCertificateDto>
                    {
                        IsSuccess = true,
                        Message = "Certificate found.",
                        Data = certificate
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking for certificate issuance.");
                return new GeneralResult<CertificateResult<StudentCertificateDto>>(false, "An error occurred while checking for certificate issuance.");
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<StudentCertificate>> IssueCertificateAsync(int examId, string userId)
        {
            try
            {
                // Checking for existing exam
                var exam = await _dbContext.Exams.Include(e => e.Course)
                    .FirstOrDefaultAsync(e => e.Id == examId && !e.IsDeleted);

                if (exam == null)
                {
                    _logger.LogWarning("Exam with ID {ExamId} not found.", examId);
                    return new GeneralResult<StudentCertificate>(false, "Exam not found.");
                }

                // Verify that the student has completed the exam
                var examResult = await _dbContext.ExamResults
                    .FirstOrDefaultAsync(er => er.ExamId == examId
                                            && er.UserId == userId
                                            && er.Status == ExamStatus.Passed.ToString()
                                            && er.IsActive
                                            && !er.IsDeleted);

                if (examResult == null)
                {
                    _logger.LogWarning("No valid exam result found for exam ID {ExamId} and user ID {UserId}.", examId, userId);
                    return new GeneralResult<StudentCertificate>(false, "No valid exam result found.");
                }

                // Fetch the student's subscription to the course
                var enrollment = await _dbContext.CourseEnrollments.FirstOrDefaultAsync(e => e.CourseId == exam.CourseId
                               && e.UserId == userId && !e.IsDeleted);

                if (enrollment == null)
                {
                    _logger.LogWarning("No enrollment found for course ID {CourseId} and user ID {UserId}.", exam.CourseId, userId);
                    return new GeneralResult<StudentCertificate>(false, "No enrollment found.");
                }

                // Checking if the certificate has already been issued
                var isCertificateIssued = CheckIfCertificateIssuedAsync(enrollment.Id, exam.CourseId ?? 0);
                if (isCertificateIssued.Result.IsSuccess)
                {
                    _logger.LogWarning("The certificate with has already been issued.");
                    return new GeneralResult<StudentCertificate>(false, "The certificate has already been issued.");
                }

                // Create a new certificate
                var newCertificate = new StudentCertificate
                {
                    EnrollmentId = enrollment.Id,
                    CourseId = exam.CourseId ?? 0,
                    Url = GenerateCertificateUrl(),
                    IssuedAt = DateTimeOffset.UtcNow,
                    DeliveryMethod = DeliveryMethod.Online,
                    CertificateId = Guid.NewGuid().ToString(),
                };

                // Barcode generation
                newCertificate.BarcodeUrl = GenerateBarcodeUrl(newCertificate.CertificateId);

                // Memorizing the testimony
                _dbContext.StudentCertificates.Add(newCertificate);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult<StudentCertificate>(true, "Certificate issued successfully.", newCertificate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while issuing a certificate for exam with Id {examId}");
                return new GeneralResult<StudentCertificate>(false, "An error occurred while issuing a certificate.");
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<CourseDetailsDto>>> GetCoursesByPaymentTypeAsync(bool isFree)
        {
            try
            {
                var courses = await _dbContext.Courses.Where(c => c.IsFree == isFree && !c.IsDeleted).ToListAsync();
                if (!courses.Any())
                {
                    _logger.LogInformation("No courses found with IsFree = {IsFree}", isFree);
                    return new GeneralResult<List<CourseDetailsDto>>(false, "No courses found.", null);
                }

                var courseDetails = _mapper.Map<List<CourseDetailsDto>>(courses);
                _logger.LogInformation("Retrieved {Count} courses with IsFree = {IsFree}", courseDetails.Count, isFree);
                return new GeneralResult<List<CourseDetailsDto>>(true, "Courses retrieved successfully.", courseDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching courses based on IsFree = {IsFree}", isFree);
                return new GeneralResult<List<CourseDetailsDto>>(false, "Error fetching courses.", null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<LessonDetailsDto>>> GetLessonsByCourseIdAsync(int courseId)
        {
            try
            {
                var lessons = await _dbContext.Lessons
                    .Where(l => l.CourseId == courseId && !l.IsDeleted).ToListAsync();
                if (!lessons.Any())
                {
                    _logger.LogInformation("No lessons found for CourseId {CourseId}.", courseId);
                    return new GeneralResult<List<LessonDetailsDto>>(false, "No lessons found.", null);
                }

                var result = _mapper.Map<List<LessonDetailsDto>>(lessons);
                return new GeneralResult<List<LessonDetailsDto>>(true, "Lessons retrieved successfully.", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching lessons for CourseId {CourseId}.", courseId);
                return new GeneralResult<List<LessonDetailsDto>>(false, "Error occurred while fetching lessons.", null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<CourseFullDetailsDto>> GetCourseFullDetailsAsync(int courseId)
        {
            try
            {
                var course = await _dbContext.Courses
                    .AsNoTracking()
                    .Where(c => c.Id == courseId && !c.IsDeleted)
                    .Select(c => new CourseFullDetailsDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        Price = c.Price,
                        Discount = c.Discount,
                        StudyWay = c.StudyWay,
                        DurationInDays = c.DurationInDays,
                        IsFree = c.IsFree,
                        Status = c.Status,
                        Logo = c.Logo,
                        CertificateExists = c.CertificateExists,
                        CertificateUrl = c.CertificateUrl,
                        StartDate = c.StartDate,
                        EndDate = c.EndDate,
                        MaxEnrollment = c.MaxEnrollment,
                        CurrentEnrollmentCount = c.CurrentEnrollmentCount,
                        Audience = c.Audience,
                        Requirements = c.Requirements,
                        Topics = c.Topics,
                        Goals = c.Goals,
                        Outcomes = c.Outcomes,
                        EntreLaunchdatedAt = c.EntreLaunchdatedAt,

                        Field = c.CourseField == null ? null : new CourseFieldData
                        {
                            Id = c.CourseField.Id,
                            Name = c.CourseField.Name
                        },

                        Path = c.TrainingPath == null ? null : new TrainingPathData
                        {
                            Id = c.TrainingPath.Id,
                            Name = c.TrainingPath.Name
                        },

                        Instructors = (c.CourseInstructors ?? Enumerable.Empty<CourseInstructor>())
                            .Where(ci => !ci.IsDeleted && !ci.User.IsDeleted)
                            .Select(ci => new CourseInstructorData
                            {
                                InstructorId = ci.User.Id,
                                FullName = (ci.User.FirstName ?? "") + " " + (ci.User.LastName ?? ""),
                                Email = ci.User.Email,
                                Specialization = ci.User.Specialization,
                                AvatarUrl = ci.User.AvatarUrl
                            }).ToList(),

                        Enrollments = (c.CourseEnrollments ?? Enumerable.Empty<CourseEnrollment>())
                            .Where(e => !e.IsDeleted && !e.User.IsDeleted)
                            .Select(e => new CourseEnrollmentData
                            {
                                StudentId = e.User.Id,
                                FullName = (e.User.FirstName ?? "") + " " + (e.User.LastName ?? ""),
                                Email = e.User.Email,
                                EnrolledAt = e.EnrolledAt ?? DateTimeOffset.UtcNow
                            }).ToList(),

                        Lessons = (c.Lessons ?? Enumerable.Empty<Lesson>())
                            .Where(l => !l.IsDeleted)
                            .Select(l => new LessonData
                            {
                                LessonId = l.Id,
                                Title = l.Name,
                                OrderIndex = l.Order ?? 0,
                                Attachments = (l.LessonAttachments ?? Enumerable.Empty<LessonAttachment>())
                                    .Where(a => !a.IsDeleted)
                                    .Select(a => new LessonAttachmentData
                                    {
                                        AttachmentId = a.Id,
                                        FileName = a.FileName,
                                        FilePath = a.FileUrl,
                                        OpenCount = a.OpenCount
                                    }).ToList()
                            }).ToList(),

                        Exams = (c.Exams ?? Enumerable.Empty<Exam>())
                            .Where(e => !e.IsDeleted)
                            .Select(e => new ExamData
                            {
                                ExamId = e.Id,
                                ExamName = e.Name,
                                DurationInMinutes = e.DurationInMinutes,
                                Questions = (e.Questions ?? Enumerable.Empty<Question>())
                                    .Where(q => !q.IsDeleted)
                                    .Select(q => new QuestionData
                                    {
                                        QuestionId = q.Id,
                                        Text = q.Text,
                                        Answers = (q.Answers ?? Enumerable.Empty<Answer>())
                                            .Where(a => !a.IsDeleted)
                                            .Select(a => new AnswerData
                                            {
                                                AnswerId = a.Id,
                                                Text = a.Text,
                                                IsCorrect = a.IsCorrect
                                            }).ToList()
                                    }).ToList()
                            }).ToList(),

                        Ratings = (c.CourseRatings ?? Enumerable.Empty<CourseRating>())
                            .Where(r => !r.IsDeleted && r.Status == RatingStatus.Approved)
                            .Select(r => new CourseRatingData
                            {
                                RatingId = r.Id,
                                Rating = r.Rating,
                                ReviewerName = (r.User.FirstName ?? "") + " " + (r.User.LastName ?? "") ?? "Unknown",
                                ReviewComment = r.Review,
                                CreatedAt = r.CreatedAt ?? DateTimeOffset.UtcNow
                            }).ToList(),

                        Tags = (c.CourseTags ?? Enumerable.Empty<CourseTag>())
                            .Where(ct => !ct.IsDeleted && ct.Tag != null)
                            .Select(ct => new CourseTagData
                            {
                                TagId = (ct.Tag ?? new Tag()).Id,
                                Name = (ct.Tag ?? new Tag()).Name
                            }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (course == null)
                {
                    return new GeneralResult<CourseFullDetailsDto>(false, "Course not found.");
                }

                return new GeneralResult<CourseFullDetailsDto>(true, "Course retrieved successfully.", course);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching full course details for CourseId {CourseId}.", courseId);
                return new GeneralResult<CourseFullDetailsDto>(false, "Error occurred while fetching course details.");
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<ExamFullDetailsDto>> GetExamFullDetailsAsync(int examId)
        {
            try
            {
                var exam = await _dbContext.Exams
                    .AsNoTracking()
                    .Include(e => e.Questions!)
                        .ThenInclude(q => q.Answers)
                    .FirstOrDefaultAsync(e => e.Id == examId && !e.IsDeleted);

                if (exam == null)
                {
                    _logger.LogWarning("Exam with ID {ExamId} not found.", examId);
                    return new GeneralResult<ExamFullDetailsDto>(false, "Exam not found.");
                }

                var examDetails = new ExamFullDetailsDto
                {
                    ExamId = exam.Id,
                    Name = exam.Name,
                    Type = exam.Type,
                    Description = exam.Description,
                    MinMark = exam.MinMark,
                    MaxMark = exam.MaxMark,
                    DurationInMinutes = exam.DurationInMinutes,
                    MaxAttempts = exam.MaxAttempts,
                    Status = exam.Status,
                    ParentEntityName = exam.ParentEntityType switch
                    {
                        ExamParentEntityType.Course => exam.Course?.Name ?? "Unknown Course",
                        ExamParentEntityType.Lesson => exam.Lesson?.Name ?? "Unknown Lesson",
                        ExamParentEntityType.TrainingPath => exam.Path?.Name ?? "Unknown Path",
                        _ => "Unknown"
                    },
                    Questions = (exam.Questions ?? Enumerable.Empty<Question>())
                        .Where(q => !q.IsDeleted)
                        .Select(q => new QuestionDetailsData
                        {
                            QuestionId = q.Id,
                            Text = q.Text,
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

                return new GeneralResult<ExamFullDetailsDto>(true, "Exam retrieved successfully.", examDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching exam full details for ExamId {ExamId}.", examId);
                return new GeneralResult<ExamFullDetailsDto>(false, "An error occurred while fetching exam details.");
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
                    return new GeneralResult<List<QuestionWithAnswersFullData>>(false, "Exam not found or deleted.");
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
                    return new GeneralResult<List<QuestionWithAnswersFullData>>(false, "No questions found for this exam.");
                }

                return new GeneralResult<List<QuestionWithAnswersFullData>>(true, "Questions retrieved successfully.", questions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching questions for ExamId {ExamId}.", examId);
                return new GeneralResult<List<QuestionWithAnswersFullData>>(false, "An error occurred while fetching questions.");
            }
        }
        // help methods

        /// <summary>
        /// Checks if the file size is within the allowed range.
        /// </summary>
        private bool IsFileSizeValid(long fileSizeInBytes, string maxSize)
        {
            var sizeUnit = maxSize[^2..].ToUpper(); // Last two characters (KB, MB, GB)
            var sizeValue = double.Parse(maxSize[..^2]); // Digital Value

            var sizeInBytes = sizeUnit switch
            {
                "KB" => sizeValue * 1024,
                "MB" => sizeValue * 1024 * 1024,
                "GB" => sizeValue * 1024 * 1024 * 1024,
                _ => throw new ArgumentException($"Invalid size unit: {sizeUnit}")
            };

            return fileSizeInBytes <= sizeInBytes;
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

            _dbContext.EntreLaunchdateRange(previousAttempts);
        }

        /// <summary>
        /// Generating a certificate URL.
        /// </summary>
        private string GenerateCertificateUrl()
        {
            // مثال: يمكن أن يكون مسارًا في الخادم أو رابط خدمة تخزين
            return $"{_configuration["CertificateBaseUrl"]}/certificates/{Guid.NewGuid()}.pdf";
        }

        /// <summary>
        /// Generating a barcode URL.
        /// </summary>
        private string GenerateBarcodeUrl(string certificateId)
        {
            // يمكن استخدام مكتبة لتوليد باركود كرابط
            return $"{_configuration["VerificationBaseUrl"]}/verify/{certificateId}";
        }

        /// <summary>
        /// Checks if the user has any active enrollments.
        /// </summary>
        private async Task<bool> HasActiveEnrollmentsAsync(string userId)
        {
            return await _dbContext.CourseEnrollments
                .AnyAsync(e => e.UserId == userId && !e.IsDeleted);
        }
    }
}
