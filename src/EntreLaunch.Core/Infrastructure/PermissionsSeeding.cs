using static EntreLaunch.Infrastructure.Permissions;
namespace EntreLaunch.Infrastructure
{
    public class PermissionsSeeding
    {
        /// <summary>
        /// Default permissions assigned to roles.
        /// Use "*" to assign all permissions except those marked with [AdminOnly].
        /// </summary>
        private static readonly Dictionary<string, List<string>> DefaultPermissionsByRole = new()
        {
            {
                "Admin", new List<string>
                {
                    "**"
                }
            },
            {
                "SubAdmin", new List<string>
                {
                     "*"
                }
            },
            {
                "Entrepreneur", new List<string>
                {
                    BlogPermissions.Create,
                    BlogPermissions.GetAll,
                    BlogPermissions.Delete,

                    ClubPermissions.EventRegister,
                    ClubPermissions.GetAll,
                    ClubPermissions.GetOne,
                    ClubPermissions.UserEventRegistrations,
                    ClubPermissions.UnregisterFromEvent,

                    ConsultationPermissions.BookingOnlineConsultation,
                    ConsultationPermissions.SendTextConsultation,
                    ConsultationPermissions.SendCounselorRequest,
                    ConsultationPermissions.GetOneConsultation,
                    ConsultationPermissions.GetAllActiveCounselors,
                    ConsultationPermissions.GetCounselorBySpecialization,
                    ConsultationPermissions.GetClientConsultationHistory,
                    ConsultationPermissions.GetCounselorCV,
                    ConsultationPermissions.GetAllCounselorTimes,

                    BlogPermissions.Create,
                    BlogPermissions.MyBlogs,
                    BlogPermissions.Delete,
                    BlogPermissions.GetAll,
                    BlogPermissions.GetOne,

                    TicketPermissions.GetOne,
                    TicketPermissions.GetByConsultation,
                    TicketPermissions.CanAccessToTicket,

                    TicketMessagePermissions.Create,
                    TicketMessagePermissions.GetAll,
                    TicketMessagePermissions.Delete,
                    TicketMessagePermissions.Edit,
                    TicketMessagePermissions.GetByTicke,

                    LocalizationPermissions.FirstTimeSetupOrDefault,

                    LoyaltyPointPermissions.GetOne,
                    LoyaltyPointPermissions.Redeem,

                    MyCommunityPermissions.CreateTextPost,
                    MyCommunityPermissions.CreatePostWithMedia,
                    MyCommunityPermissions.CreateLike,
                    MyCommunityPermissions.CreateReport,
                    MyCommunityPermissions.GetPostById,
                    MyCommunityPermissions.GetPostLikeCount,
                    MyCommunityPermissions.GetAcceptedPosts,
                    MyCommunityPermissions.DeletePost,
                    MyCommunityPermissions.DeleteMedia,
                    MyCommunityPermissions.DeletePostReport,

                    MyOpportunityPermissions.SendOpportunityRequest,
                    MyOpportunityPermissions.Filter,
                    MyOpportunityPermissions.GetOpportunities,
                    MyOpportunityPermissions.GeMyRequests,
                    MyOpportunityPermissions.DeleteOwnRequest,

                    MyPartnerPermissions.Create,
                    MyPartnerPermissions.Filter,
                    MyPartnerPermissions.GetAccepted,
                    MyPartnerPermissions.GetOne,
                    MyPartnerPermissions.GetAttachment,

                    MyTeamPermissions.Create,
                    MyTeamPermissions.AcceptedEmployees,
                    MyTeamPermissions.Filtering,
                    MyTeamPermissions.GetEmployeeById,

                    OnlineCoursePermissions.GetAll,
                    OnlineCoursePermissions.GetOne,
                    OnlineCoursePermissions.GetByStatus,

                    PathCoursePermissions.GetByPath,
                    PathCoursePermissions.GetOne,
                    PathCoursePermissions.GetAll,

                    PaymentPermissions.Create,

                    PurchasePermissions.GetUserPurchases,
                    PurchasePermissions.GetPurchaseById,
                    PurchasePermissions.HasUserPurchased,
                    PurchasePermissions.GetPurchasesType,

                    RefundPermissions.Create,

                    SimulationPermissions.Create,
                    SimulationPermissions.GetAds,
                    SimulationPermissions.LikeCount,

                    SkillCoursePermissions.GetAll,
                    SkillCoursePermissions.GetOne,
                    SkillCoursePermissions.GetByField,

                    SubscriptionPermissions.HasActiveAccess,
                    SubscriptionPermissions.CancelSubscription,
                    SubscriptionPermissions.StartTrial,
                    SubscriptionPermissions.GetUserSubscription,
                    SubscriptionPermissions.GetUserSubscriptions,

                    TrainingPathPermissions.GetAll,
                    TrainingPathPermissions.GetOne,

                    UserPermissions.Complete,
                    UserPermissions.Edit,
                    UserPermissions.GetMe,
                    UserPermissions.MyProfile,
                    UserPermissions.Delete,

                    WheelAwardPermissions.GetAll,

                    WheelPlayerPermissions.Spin,
                    WheelPlayerPermissions.CanPlay,
                    WheelPlayerPermissions.TodaySpin,
                    WheelPlayerPermissions.History,

                    TagPermissions.GetAll,
                    TagPermissions.GetOne,
                }
            },
            {
                "Trainer", new List<string>
                {
                    AnswerPermissions.Create,
                    AnswerPermissions.Import,
                    AnswerPermissions.Edit,
                    AnswerPermissions.GetAll,
                    AnswerPermissions.GetOne,
                    AnswerPermissions.Delete,

                    CourseFieldPermissions.GetAll,

                    CourseRatingPermissions.GetAllByCourse,
                    CourseRatingPermissions.GetByInstructor,

                    CourseTagsPermissions.GetByCourse,
                    CourseTagsPermissions.GetCoursesByTag,
                    CourseTagsPermissions.GetByTag,

                    ExamPermissions.GetOne,

                    LocalizationPermissions.FirstTimeSetupOrDefault,
                }
            },
            {
                "Student", new List<string>
                {
                    AnswerPermissions.GetAll,
                    AnswerPermissions.GetOne,

                    StudentCertificatePermissions.Issue,
                    StudentCertificatePermissions.ShippingCertificate,
                    StudentCertificatePermissions.GetOne,
                    StudentCertificatePermissions.GetMyCertificates,
                    StudentCertificatePermissions.UpdateShippingStatus,

                    CourseFieldPermissions.GetAll,

                    CourseInstructorPermissions.GetInstructorsByCourse,

                    CourseRatingPermissions.Create,
                    CourseRatingPermissions.GetAllByCourse,

                    CourseTagsPermissions.GetByCourse,
                    CourseTagsPermissions.GetCoursesByTag,
                    CourseTagsPermissions.GetByTag,

                    ExamResultPermissions.Submit,
                    ExamResultPermissions.GetByStudent,
                    ExamResultPermissions.GetStudentAttempts,
                    ExamResultPermissions.GetActiveResult,

                    ExamPermissions.Retake,
                    ExamPermissions.CanRetake,
                    ExamPermissions.GetOne,

                    LessonAttachmentPermissions.OpenCounter,
                    LessonAttachmentPermissions.GetAll,
                    LessonAttachmentPermissions.GetOne,

                    LessonPermissions.GetAll,
                    LessonPermissions.GetOne,
                    LessonPermissions.GetByCourse,

                    LocalizationPermissions.FirstTimeSetupOrDefault,

                    ProgressPermissions.MarkLessonCompleted,
                    ProgressPermissions.GetLessonProgress,
                    ProgressPermissions.GetCompletedLessons,
                    ProgressPermissions.UpdateCourseProgress,
                    ProgressPermissions.GetCourseProgress,
                    ProgressPermissions.GetUserCoursesProgress,
                    ProgressPermissions.UpdatePathProgress,
                    ProgressPermissions.GetPathProgress,
                    ProgressPermissions.GetUserPathsProgress,
                    ProgressPermissions.StartLessonSession,
                    ProgressPermissions.EndLessonSession,
                }
            },
            {
                "Counselor", new List<string>
                {
                    ConsultationPermissions.ProcessConsultationStatus,
                    ConsultationPermissions.GetConsultationByCounselor,
                    ConsultationPermissions.GetOneConsultation,
                    ConsultationPermissions.CreateCounselorTime,
                    ConsultationPermissions.ImportCounselorTime,
                    ConsultationPermissions.EditCounselorTime,
                    ConsultationPermissions.GetAllCounselorTimes,
                    ConsultationPermissions.GetCounselorSummaryStats,
                    ConsultationPermissions.GetConsultantConsultationHistory,

                    TicketPermissions.Create,
                    TicketPermissions.Process,
                    TicketPermissions.GetOne,
                    TicketPermissions.GetByConsultation,
                    TicketPermissions.GetByCounselor,
                    TicketPermissions.CanAccessToTicket,

                    TicketMessagePermissions.Create,
                    TicketMessagePermissions.GetAll,
                    TicketMessagePermissions.Delete,
                    TicketMessagePermissions.Edit,
                    TicketMessagePermissions.GetByTicke,

                    LocalizationPermissions.FirstTimeSetupOrDefault,
                }
            },
            {
                "Guest", new List<string>
                {
                    LocalizationPermissions.FirstTimeSetupOrDefault,
                }
            },
            {
                "User", new List<string>
                {
                    LocalizationPermissions.FirstTimeSetupOrDefault,
                }
            }
        };

        /// <summary>
        /// Assigns default permissions to roles.
        /// </summary>
        public static async Task SeedRolePermissionsAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (var (roleName, rawPermissions) in DefaultPermissionsByRole)
            {
                var role = await roleManager.FindByNameAsync(roleName)
                           ?? new IdentityRole(roleName);

                if (role.Id == null)
                {
                    var createResult = await roleManager.CreateAsync(role);
                    if (!createResult.Succeeded)
                        throw new Exception($"Failed to create role '{roleName}': {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                }

                var existingClaims = (await roleManager.GetClaimsAsync(role))
                    .Where(c => c.Type == "Permission")
                    .Select(c => c.Value)
                    .ToHashSet();

                List<string> resolvedPermissions;

                if (rawPermissions.Contains("**"))
                {
                    // All permissions including [AdminOnly]
                    resolvedPermissions = GetAllPermissions(includeAdminOnly: true)
                        .Select(p => p.Value)
                        .ToList();
                }
                else if (rawPermissions.Contains("*"))
                {
                    // All permissions without [AdminOnly]
                    resolvedPermissions = GetAllPermissions(includeAdminOnly: false)
                        .Select(p => p.Value)
                        .ToList();
                }
                else
                {
                    // Explicitly Written permissions
                    resolvedPermissions = rawPermissions;
                }

                foreach (var permission in resolvedPermissions.Distinct())
                {
                    if (string.IsNullOrWhiteSpace(permission) || existingClaims.Contains(permission))
                        continue;

                    var addResult = await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
                    if (!addResult.Succeeded)
                    {
                        throw new Exception($"Failed to add permission '{permission}' to role '{roleName}'.");
                    }
                }
            }
        }

        /// <summary>
        /// get all permissions marked with [AdminOnly].
        /// </summary>
        private static List<string> GetAdminOnlyPermissions()
        {
            return GetAllPermissions(includeAdminOnly: true)
                .Where(p => p.IsAdminOnly)
                .Select(p => p.Value)
                .ToList();
        }

        /// <summary>
        /// get all permissions except admin only.
        /// </summary>
        private static List<string> GetAllNonAdminPermissions()
        {
            return GetAllPermissions(includeAdminOnly: false)
                .Select(p => p.Value)
                .ToList();
        }

        /// <summary>
        /// get all permissions.
        /// </summary>
        public static List<(string Value, bool IsAdminOnly)> GetAllPermissions(bool includeAdminOnly)
        {
            var result = new List<(string, bool)>();
            var nestedTypes = typeof(Permissions).GetNestedTypes(BindingFlags.Public | BindingFlags.Static);

            foreach (var type in nestedTypes)
            {
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                                 .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string));

                foreach (var field in fields)
                {
                    var value = field.GetRawConstantValue() as string;
                    if (string.IsNullOrWhiteSpace(value))
                        continue;

                    var isAdminOnly = field.GetCustomAttribute<AdminOnlyAttribute>() != null;

                    if (!includeAdminOnly && isAdminOnly)
                        continue;

                    result.Add((value, isAdminOnly));
                }
            }

            return result;
        }
    }
}
