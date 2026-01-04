namespace EntreLaunch.Infrastructure
{
    public static class Permissions
    {
        public static class ActivityLogPermissions
        {
            [AdminOnly]
            public const string GetAll = "Up.ActivityLog.GetAll";
        }

        public static class AnswerPermissions
        {
            public const string Create = "Up.Answer.Create";
            public const string Edit = "Up.Answer.Edit";
            public const string GetAll = "Up.Answer.GetAll";
            public const string GetOne = "Up.Answer.GetOne";
            public const string Delete = "Up.Answer.Delete";
            public const string Import = "Up.Answer.Import";
        }

        public static class ExamPermissions
        {
            public const string Create = "Up.Exam.Create";
            public const string Edit = "Up.Exam.Edit";
            public const string GetAll = "Up.Exam.GetAll";
            public const string GetOne = "Up.Exam.GetOne";
            public const string GetAttempt = "Up.Exam.GetAttempt";
            public const string CascadeDelete = "Up.Exam.CascadeDelete";
            public const string Export = "Up.Exam.Export";
            public const string CreateFull = "Up.Exam.CreateFull";
            public const string Import = "Up.Exam.Import";
            public const string GetEnumValues = "Up.Exam.GetEnumValues";
            public const string CanRetake = "Up.Exam.CanRetake";
            public const string Retake = "Up.Exam.Retake";
        }

        public static class QuestionPermissions
        {
            public const string Create = "Up.Question.Create";
            public const string Edit = "Up.Question.Edit";
            public const string GetAll = "Up.Question.GetAll";
            public const string GetOne = "Up.Question.GetOne";
            public const string Delete = "Up.Question.Delete";
            public const string Export = "Up.Question.Export";
            public const string CreateFull = "Up.Question.CreateFull";
        }

        public static class ExamResultPermissions
        {
            public const string Submit = "Up.ExamResult.Submit";
            public const string GetByStudent = "Up.ExamResult.GetByStudent";
            public const string CompareStudentResult = "Up.ExamResult.CompareStudentResult";
            public const string GetExamStatistics = "Up.ExamResult.GetExamStatistics";
            public const string GetTopTenStudents = "Up.ExamResult.GetTopTenStudents";
            public const string GetStudentAttempts = "Up.ExamResult.GetStudentAttempts";
            public const string GetActiveResult = "Up.ExamResult.GetActiveResult";
        }

        public static class ClubPermissions
        {
            public const string Create = "Up.CLub.Create";
            public const string RenewClubSubscription = "Up.CLub.RenewClubSubscription";
            public const string Edit = "Up.CLub.Edit";
            public const string GetAll = "Up.CLub.GetAll";
            public const string GetOne = "Up.CLub.GetOne";
            public const string GetEventSubscriber = "Up.CLub.GetEventSubscriber";
            public const string SoftDelete = "Up.CLub.SoftDelete";
            public const string Export = "Up.CLub.Export";
            public const string CreateFull = "Up.CLub.CreateFull";
            public const string ImportList = "Up.CLub.ImportList";
            public const string ImportFile = "Up.CLub.ImportFile";
            public const string GenerateTemplate = "Up.CLub.GenerateTemplate";
            public const string GetUserSubscription = "Up.CLub.GetUserSubscription";
            public const string UnregisterFromEvent = "Up.CLub.UnregisterFromEvent";
            public const string EventRegister = "Up.CLub.EventRegister";
            public const string UserEventRegistrations = "Up.CLub.UserEventRegistrations";
        }

        public static class MyCommunityPermissions
        {
            public const string CreateTextPost = "Up.MyCommunity.CreateTextPost";
            public const string CreatePostWithMedia = "Up.MyCommunity.CreatePostWithMedia";
            public const string CreateMediaToPost = "Up.MyCommunity.CreateMediaToPost";
            public const string CreateComment = "Up.MyCommunity.CreateComment";
            public const string CreateLike = "Up.MyCommunity.CreateLike";
            public const string CreateReport = "Up.MyCommunity.CreateReport";
            public const string UpdatePost = "Up.MyCommunity.UpdatePost";
            public const string UpdateMedia = "Up.MyCommunity.UpdateMedia";
            public const string UpdateComment = "Up.MyCommunity.UpdateComment";
            public const string GetAllPosts = "Up.MyCommunity.GetAllPosts";
            public const string GetPostById = "Up.MyCommunity.GetPostById";
            public const string GetPostLikeCount = "Up.MyCommunity.GetPostLikeCount";
            public const string GetPostComments = "Up.MyCommunity.GetPostComments";
            public const string GetPostReports = "Up.MyCommunity.GetPostReports";
            public const string GetCommentReports = "Up.MyCommunity.GetCommentReports";
            public const string GetPendingPosts = "Up.MyCommunity.GetPendingPosts";
            public const string GetAcceptedPosts = "Up.MyCommunity.GetAcceptedPosts";
            public const string GetRejectedPosts = "Up.MyCommunity.GetRejectedPosts";
            public const string GetPendingReports = "Up.MyCommunity.GetPendingReports";
            public const string GetAcceptedReports = "Up.MyCommunity.GetAcceptedReports";
            public const string GetRejectedReports = "Up.MyCommunity.GetRejectedReports";
            public const string ProcessPostStatus = "Up.MyCommunity.ProcessPostStatus";
            public const string ProcessReportStatus = "Up.MyCommunity.ProcessReportStatus";
            public const string ProcessCommentStatus = "Up.MyCommunity.ProcessCommentStatus";
            public const string DeletePost = "Up.MyCommunity.DeletePost";
            public const string DeleteComment = "Up.MyCommunity.DeleteComment";
            public const string DeleteMedia = "Up.MyCommunity.DeleteMedia";
            public const string DeletePostReport = "Up.MyCommunity.DeletePostReport";
            public const string DeleteCommentReport = "Up.MyCommunity.DeleteCommentReport";
        }

        public static class ConsultationPermissions
        {
            public const string BookingOnlineConsultation = "Up.Consultation.BookingOnlineConsultation";
            public const string SendTextConsultation = "Up.Consultation.SendTextConsultation";
            public const string ProcessConsultationStatus = "Up.Consultation.ProcessConsultationStatus";
            public const string CreateCounselorTime = "Up.Consultation.CreateCounselorTime";
            public const string ImportCounselorTime = "Up.Consultation.ImportCounselorTime";
            public const string SendCounselorRequest = "Up.Consultation.SendCounselorRequest";
            public const string ProcessCounselorRequest = "Up.Counselor.ProcessCounselorRequest";
            public const string EditCounselorTime = "Up.Counselor.EditCounselorTime";
            public const string GetAllConsultation = "Up.Counselor.GetAllConsultation";
            public const string GetConsultationByType = "Up.Counselor.GetConsultationByType";
            public const string GetOneConsultation = "Up.Counselor.GetOneConsultation";
            public const string GetConsultationByCounselor = "Up.Counselor.GetConsultationByCounselor";
            public const string GetEnumValues = "Up.Consultation.GetEnumValues";
            public const string GetAllCounselorTimes = "Up.Consultation.GetAllCounselorTimes";
            public const string GetAllCounselorRequests = "Up.Consultation.GetAllCounselorRequests";
            public const string GetPendingCounselorRequests = "Up.Consultation.GetPendingCounselorRequests";
            public const string GetAcceptedCounselorRequests = "Up.Consultation.GetAcceptedCounselorRequests";
            public const string GetRejectedCounselorRequests = "Up.Consultation.GetRejectedCounselorRequests";
            public const string GetCounselorBySpecialization = "Up.Consultation.GetCounselorBySpecialization";
            public const string GetAllCounselorSpecializations = "Up.Consultation.GetAllCounselorSpecializations";
            public const string GetCounselorCV = "Up.Counselor.GetCounselorCV";
            public const string GetAllActiveCounselors = "Up.Counselor.GetAllActiveCounselors";
            public const string Delete = "Up.Counselor.Delete";
            public const string GetMyConsultationTimes = "Up.Counselor.GetMyConsultationTimes";
            public const string GetClientConsultationHistory = "Up.Counselor.GetClientConsultationHistory";
            public const string GetCounselorSummaryStats = "Up.Counselor.GetCounselorSummaryStats";
            public const string CheckPendingCounselorRequest = "Up.Counselor.CheckPendingCounselorRequest";
            public const string GetCounselorByUserId = "Up.Counselor.GetCounselorByUserId";
            public const string GetConsultantConsultationHistory = "Up.Counselor.GetConsultantConsultationHistory";
        }

        public static class BlogPermissions
        {
            public const string Create = "Up.Blog.Create";
            public const string ProcessStatus = "Up.Blog.ProcessStatus";
            public const string GetAll = "Up.Blog.ViewAll";
            public const string MyBlogs = "Up.Blog.MyBlogs";
            public const string Delete = "Up.Blog.Delete";
            public const string ViewByStatus = "Up.Blog.ViewByStatus";
            public const string GetOne = "Up.Blog.GetOne";
        }

        public static class PathCoursePermissions
        {
            public const string Create = "Up.PathCourse.Create";
            public const string Edit = "Up.PathCourse.Edit";
            public const string GetAll = "Up.PathCourse.GetAll";
            public const string GetOne = "Up.PathCourse.GetOne";
            public const string Delete = "Up.PathCourse.Delete";
            public const string GetByPath = "Up.PathCourse.GetByPath";
        }

        public static class OnlineCoursePermissions
        {
            public const string Create = "Up.OnlineCourse.Create";
            public const string Edit = "Up.OnlineCourse.Edit";
            public const string GetAll = "Up.OnlineCourse.GetAll";
            public const string GetOne = "Up.OnlineCourse.GetOne";
            public const string Delete = "Up.OnlineCourse.Delete";
            public const string ChangeStatus = "Up.OnlineCourse.ChangeStatus";
            public const string GetStatuses = "Up.OnlineCourse.GetStatuses";
            public const string GetByStatus = "Up.OnlineCourse.GetByStatus";
            public const string GetEnrolled = "Up.OnlineCourse.GetEnrolled";
        }

        public static class SkillCoursePermissions
        {
            public const string Create = "Up.SkillCourse.Create";
            public const string Edit = "Up.SkillCourse.Edit";
            public const string GetAll = "Up.SkillCourse.GetAll";
            public const string GetOne = "Up.SkillCourse.GetOne";
            public const string Delete = "Up.SkillCourse.Delete";
            public const string GetByField = "Up.SkillCourse.GetByField";
            public const string GetEnrolled = "Up.SkillCourse.GetEnrolled";
        }

        public static class StaticContentPermissions
        {
            public const string Create = "Up.StaticContent.Create";
            public const string Edit = "Up.StaticContent.Edit";
            public const string Delete = "Up.StaticContent.Delete";
            public const string GetStaticContentMediaType = "Up.StaticContent.GetStaticContentMediaType";
            public const string GetStaticContentType = "Up.StaticContent.GetStaticContentType";
        }

        public static class CourseRatingPermissions
        {
            public const string Create = "Up.CourseRating.Create";
            public const string Edit = "Up.CourseRating.Edit";
            public const string GetAll = "Up.CourseRating.GetAll";
            public const string GetOne = "Up.CourseRating.GetOne";
            public const string Delete = "Up.CourseRating.Delete";
            public const string Export = "Up.CourseRating.Export";
            public const string GetRatingStats = "Up.CourseRating.GetRatingStats";
            public const string GetAllByCourse = "Up.CourseRating.GetAllByCourse";
            public const string GetSummary = "Up.CourseRating.GetSummary";
            public const string GetByInstructor = "Up.CourseRating.GetByInstructor";
            public const string Approve = "Up.CourseRating.Approve";
            public const string Reject = "Up.CourseRating.Reject";
            public const string GetByStatus = "Up.CourseRating.GetByStatus";
            public const string GetRatingStatuses = "Up.CourseRating.GetRatingStatuses";
        }

        public static class SubscriptionPermissions
        {
            public const string HasActiveAccess = "Up.Subscription.HasActiveAccess";
            public const string ExtendSubscription = "Up.Subscription.ExtendSubscription";
            public const string CancelSubscription = "Up.Subscription.CancelSubscription";
            public const string StartTrial = "Up.Subscription.StartTrial";
            public const string UpgradeSubscription = "Up.Subscription.UpgradeSubscription";
            public const string CreateChildSubscription = "Up.Subscription.CreateChildSubscription";
            public const string GetUserSubscriptions = "Up.Subscription.GetUserSubscriptions";
            public const string GetUserSubscription = "Up.Subscription.GetUserSubscription";
            public const string GetSubscriptionsByStatus = "Up.Subscription.GetSubscriptionsByStatus";
            public const string GetExpiringSoon = "Up.Subscription.GetExpiringSoon";
            public const string GetSubscriptionStatistics = "Up.Subscription.GetSubscriptionStatistics";
            public const string GetSubscriptionType = "Up.Subscription.GetSubscriptionType";
            public const string GetSubscriptionStatus = "Up.Subscription.GetSubscriptionStatus";
        }

        public static class PurchasePermissions
        {
            public const string GetUserPurchases = "Up.Purchase.GetUserPurchases";
            public const string RefundPurchase = "Up.Purchase.RefundPurchase";
            public const string GetPurchaseById = "Up.Purchase.GetPurchaseById";
            public const string GetPurchaseStats = "Up.Purchase.GetPurchaseStats";
            public const string HasUserPurchased = "Up.Purchase.HasUserPurchased";
            public const string GetPurchasesType = "Up.Purchase.GetPurchasesType";
        }

        public static class TrainingPathPermissions
        {
            public const string Create = "Up.TrainingPath.Create";
            public const string Edit = "Up.TrainingPath.Edit";
            public const string GetAll = "Up.TrainingPath.GetAll";
            public const string GetOne = "Up.TrainingPath.GetOne";
            public const string Delete = "Up.TrainingPath.Delete";
            public const string Export = "Up.TrainingPath.Export";
            public const string Import = "Up.TrainingPath.Import";
        }

        public static class CourseInstructorPermissions
        {
            public const string Create = "Up.CourseInstructor.Create";
            public const string Edit = "Up.CourseInstructor.Edit";
            public const string GetAll = "Up.CourseInstructor.GetAll";
            public const string GetOne = "Up.CourseInstructor.GetOne";
            //public const string GetByCourse = "Up.CourseInstructor.GetByCourse";
            public const string Delete = "Up.CourseInstructor.Delete";
            public const string Export = "Up.CourseInstructor.Export";
            public const string GetInstructorsByCourse = "Up.CourseInstructor.GetInstructorsByCourse";
            public const string GetTrainerPerformance = "Up.CourseInstructor.GetTrainerPerformance";
        }

        public static class CourseEnrollmentPermissions
        {
            public const string Create = "Up.CourseEnrollment.Create";
            public const string Edit = "Up.CourseEnrollment.Edit";
            public const string GetAll = "Up.CourseEnrollment.GetAll";
            public const string GetOne = "Up.CourseEnrollment.GetOne";
            public const string GetByCourse = "Up.CourseEnrollment.GetByCourse";
            public const string Delete = "Up.CourseEnrollment.Delete";
            public const string Export = "Up.CourseEnrollment.Export";
            public const string Unenroll = "Up.CourseEnrollment.Unenroll";
            public const string GetUserSubscriptions = "Up.CourseEnrollment.GetUserSubscriptions";
        }

        public static class LessonPermissions
        {
            public const string Create = "Up.Lesson.Create";
            public const string Edit = "Up.Lesson.Edit";
            public const string GetAll = "Up.Lesson.GetAll";
            public const string GetOne = "Up.Lesson.GetOne";
            public const string CascadeDelete = "Up.Lesson.CascadeDelete";
            public const string Export = "Up.Lesson.Export";
            public const string CreateFull = "Up.Lesson.CreateFull";
            public const string Import = "Up.Lesson.Import";
            public const string Reorder = "Up.Lesson.Reorder";
            public const string UpdateProgress = "Up.Lesson.UpdateProgress";
            public const string GetProgress = "Up.Lesson.GetProgress";
            public const string CalculateProgress = "Up.Lesson.CalculateProgress";
            public const string GetByCourse = "Up.Lesson.GetByCourse";
        }

        public static class LessonAttachmentPermissions
        {
            public const string Create = "Up.LessonAttachment.Create";
            public const string Edit = "Up.LessonAttachment.Edit";
            public const string GetAll = "Up.LessonAttachment.GetAll";
            public const string GetOne = "Up.LessonAttachment.GetOne";
            public const string Delete = "Up.LessonAttachment.Delete";
            public const string Export = "Up.LessonAttachment.Export";
            public const string Import = "Up.LessonAttachment.Import";
            public const string OpenCounter = "Up.LessonAttachment.OpenCounter";
            public const string GetStats = "Up.LessonAttachment.GetStats";
            public const string ValidateFile = "Up.LessonAttachment.ValidateFile";
        }

        public class ProgressPermissions
        {
            public const string MarkLessonCompleted = "Up.Progress.MarkLessonCompleted";
            public const string GetLessonProgress = "Up.Progress.GetLessonProgress";
            public const string GetCompletedLessons = "Up.Progress.GetCompletedLessons";
            public const string UpdateCourseProgress = "Up.Progress.UpdateCourseProgress";
            public const string GetCourseProgress = "Up.Progress.GetCourseProgress";
            public const string GetUserCoursesProgress = "Up.Progress.GetUserCoursesProgress";
            public const string UpdatePathProgress = "Up.Progress.UpdatePathProgress";
            public const string GetPathProgress = "Up.Progress.GetPathProgress";
            public const string GetUserPathsProgress = "Up.Progress.GetUserPathsProgress";
            public const string StartLessonSession = "Up.Progress.StartLessonSession";
            public const string EndLessonSession = "Up.Progress.EndLessonSession";
            public const string SyncProgramProgress = "Up.Progress.SyncProgramProgress";
        }

        public static class OpportunityPermissions
        {
            public const string Create = "Up.InvestmentOpportunity.Create";
            public const string Edit = "Up.InvestmentOpportunity.Edit";
            public const string GetAll = "Up.InvestmentOpportunity.GetAll";
            public const string GetOne = "Up.InvestmentOpportunity.GetOne";
            public const string Delete = "Up.InvestmentOpportunity.Delete";
            public const string Export = "Up.InvestmentOpportunity.Export";
            public const string Import = "Up.InvestmentOpportunity.Import";
        }

        public static class MyOpportunityPermissions
        {
            public const string SendOpportunityRequest = "Up.OpportunityRequest.SenDOBportunityRequest";
            public const string GetAll = "Up.OpportunityRequest.GetAll";
            public const string GetPending = "Up.OpportunityRequest.GetPending";
            public const string GetAccepted = "Up.OpportunityRequest.GetAccepted";
            public const string GetRejected = "Up.OpportunityRequest.GetRejected";
            public const string ProgressRequest = "Up.OpportunityRequest.ProgressRequest";
            public const string GetOpportunities = "Up.OpportunityRequest.GetOpportunities";
            public const string Filter = "Up.InvestmentOpportunity.Filter";
            public const string GeMyRequests = "Up.InvestmentOpportunity.GeMyRequests";
            public const string DeleteOwnRequest = "Up.InvestmentOpportunity.DeleteOwnRequest";
        }

        public static class LocalizationPermissions
        {
            public const string FirstTimeSetupOrDefault = "Up.Localization.FirstTimeSetupOrDefault";
        }

        public static class NotificationPermissions
        {
            public const string Create = "Up.Notification.Create";
            public const string Edit = "Up.Notification.Edit";
            public const string GetAll = "Up.Notification.GetAll";
            public const string GetOne = "Up.Notification.GetOne";
            public const string Delete = "Up.Notification.Delete";
            public const string Export = "Up.Notification.Export";
            public const string GetEnumValues = "Up.Notification.GetEnumValues";
        }

        public static class PaymentPermissions
        {
            public const string Create = "Up.Payment.Create";
            public const string GetAll = "Up.Payment.GetAll";
        }

        public static class PortfolioPermissions
        {
            public const string Create = "Up.Portfolio.Create";
            public const string Edit = "Up.Portfolio.Edit";
            public const string GetAll = "Up.Portfolio.GetAll";
            public const string GetOne = "Up.Portfolio.GetOne";
            public const string CascadeDelete = "Up.Portfolio.CascadeDelete";
            public const string Export = "Up.Portfolio.Export";
            public const string CreateFull = "Up.Portfolio.CreateFull";
        }

        public static class PortfolioAttachmentPermissions
        {
            public const string Create = "Up.PortfolioAttachment.Create";
            public const string Edit = "Up.PortfolioAttachment.Edit";
            public const string GetAll = "Up.PortfolioAttachment.GetAll";
            public const string GetOne = "Up.PortfolioAttachment.GetOne";
            public const string Delete = "Up.PortfolioAttachment.Delete";
            public const string Export = "Up.PortfolioAttachment.Export";
            public const string Import = "Up.PortfolioAttachment.Import";
        }

        public static class SimulationPermissions
        {
            public const string Create = "Up.Simulation.Create";
            public const string Update = "Up.Simulation.Update";
            public const string GetAll = "Up.Simulation.GetAll";
            public const string GetOne = "Up.Simulation.GetOne";
            public const string GetAds = "Up.Simulation.GetAds";
            public const string GetByStatus = "Up.Simulation.GetByStatus";
            public const string LikeCount = "Up.Simulation.LikeCount";
            public const string Delete = "Up.Simulation.Delete";
        }

        public static class MyPartnerPermissions
        {
            public const string Create = "Up.MyPartner.Create";
            public const string Edit = "Up.MyPartner.Edit";
            public const string GetAll = "Up.MyPartner.GetAll";
            public const string GetOne = "Up.MyPartner.GetOne";
            public const string GetAttachment = "Up.MyPartner.GetAttachment";
            public const string GetPending = "Up.MyPartner.GetPending";
            public const string GetAccepted = "Up.MyPartner.GetAccepted";
            public const string GetRejected = "Up.MyPartner.GetRejected";
            public const string Filter = "Up.MyPartner.Filter";
            public const string ProgressProject = "Up.MyPartner.ProgressProject";
        }

        public static class ProjectPurchasePermissions
        {
            public const string Create = "Up.ProjectPurchase.Create";
            public const string Edit = "Up.ProjectPurchase.Edit";
            public const string GetAll = "Up.ProjectPurchase.GetAll";
            public const string GetOne = "Up.ProjectPurchase.GetOne";
            public const string Delete = "Up.ProjectPurchase.Delete";
            public const string Export = "Up.ProjectPurchase.Export";
            public const string Import = "Up.ProjectPurchase.Import";
        }

        public static class QualificationPermissions
        {
            public const string Create = "Up.Qualification.Create";
            public const string Edit = "Up.Qualification.Edit";
            public const string GetAll = "Up.Qualification.GetAll";
            public const string GetOne = "Up.Qualification.GetOne";
            public const string Delete = "Up.Qualification.Delete";
            public const string Export = "Up.Qualification.Export";
            public const string Import = "Up.Qualification.Import";
        }

        public static class RefundPermissions
        {
            public const string Create = "Up.Refund.Create";
            public const string Approve = "Up.Refund.Approve";
            public const string GetAll = "Up.Refund.GetAll";
            public const string GetOne = "Up.Refund.GetOne";
            public const string Reject = "Up.Refund.Reject";
            public const string Export = "Up.Refund.Export";
        }

        public static class StudentCertificatePermissions
        {
            public const string Issue = "Up.StudentCertificate.Issue";
            public const string ShippingCertificate = "Up.StudentCertificate.ShippingCertificate";
            public const string GetAll = "Up.StudentCertificate.GetAll";
            public const string GetOne = "Up.StudentCertificate.GetOne";
            public const string Delete = "Up.StudentCertificate.Delete";
            public const string Export = "Up.StudentCertificate.Export";
            public const string GetEnumValues = "Up.StudentCertificate.GetEnumValues";
            public const string UpdateShippingStatus = "Up.StudentCertificate.UpdateShippingStatus";
            public const string GetAllShippingRequests = "Up.StudentCertificate.GetAllShippingRequests";
            public const string GetMyCertificates = "Up.StudentCertificate.GetMyCertificates";
        }

        public static class TicketPermissions
        {
            public const string Create = "Up.Ticket.Create";
            public const string Edit = "Up.Ticket.Edit";
            public const string GetAll = "Up.Ticket.GetAll";
            public const string GetOne = "Up.Ticket.GetOne";
            public const string GetByConsultation = "Up.Ticket.GetByConsultation";
            public const string CascadeDelete = "Up.Ticket.CascadeDelete";
            public const string Export = "Up.Ticket.Export";
            public const string Process = "Up.Ticket.Process";
            public const string GetByCounselor = "Up.Ticket.GetByCounselor";
            public const string GetOpenTickets = "Up.Ticket.GetOpenTickets";
            public const string CanAccessToTicket = "Up.Ticket.CanAccessToTicket";
        }

        public static class TicketMessagePermissions
        {
            public const string Create = "Up.TicketMessage.Create";
            public const string Edit = "Up.TicketMessage.Edit";
            public const string GetAll = "Up.TicketMessage.GetAll";
            public const string GetOne = "Up.TicketMessage.GetOne";
            public const string GetByTicke = "Up.TicketMessage.GetByTicke";
            public const string Delete = "Up.TicketMessage.Delete";
            public const string Export = "Up.TicketMessage.Export";
        }

        public static class TicketAttachmentPermissions
        {
            public const string Create = "Up.TicketAttachment.Create";
            public const string Edit = "Up.TicketAttachment.Edit";
            public const string GetAll = "Up.TicketAttachment.GetAll";
            public const string GetOne = "Up.TicketAttachment.GetOne";
            public const string Delete = "Up.TicketAttachment.Delete";
            public const string Export = "Up.TicketAttachment.Export";
            public const string Import = "Up.TicketAttachment.Import";
        }

        public static class MyTeamPermissions
        {
            public const string Create = "Up.MyTeam.Create";
            public const string ChangeStatus = "Up.MyTeam.ChangeStatus";
            public const string UpdateEmployee = "Up.MyTeam.UpdateEmployee";
            public const string UpdatePortfolio = "Up.MyTeam.UpdatePortfolio";
            public const string UpdatePortfolioAttachment = "Up.MyTeam.UpdatePortfolioAttachment";
            public const string GetAll = "Up.MyTeam.GetAll";
            public const string PendingEmployees = "Up.MyTeam.PendingEmployees";
            public const string AcceptedEmployees = "Up.MyTeam.AcceptedEmployees";
            public const string RejectedEmployees = "Up.MyTeam.RejectedEmployees";
            public const string Filtering = "Up.MyTeam.Filtering";
            public const string GetEmployeeById = "Up.MyTeam.GetEmployeeById";
            public const string GetPortfoliosForEmployee = "Up.MyTeam.GetPortfoliosForEmployee";
        }

        public static class UserPermissions
        {
            public const string Create = "Up.User.Create";
            public const string Complete = "Up.User.Complete";
            public const string Edit = "Up.User.Edit";
            public const string GetAll = "Up.User.GetAll";
            public const string GetOne = "Up.User.GetOne";
            public const string GetMe = "Up.User.GetMe";
            public const string Delete = "Up.User.Delete";
            public const string Export = "Up.User.Export";
            public const string GetEnumValues = "Up.User.GetEnumValues";
            public const string MyProfile = "Up.User.MyProfile";
        }

        public static class WheelAwardPermissions
        {
            public const string Create = "Up.WheelAward.Create";
            public const string Complete = "Up.WheelAward.Complete";
            public const string Edit = "Up.WheelAward.Edit";
            public const string GetAll = "Up.WheelAward.GetAll";
            public const string GetOne = "Up.WheelAward.GetOne";
            public const string Delete = "Up.WheelAward.Delete";
            public const string Export = "Up.WheelAward.Export";
            public const string Import = "Up.WheelAward.Import";
        }

        public static class WheelPlayerPermissions
        {
            public const string Spin = "Up.WheelPlayer.spin";
            public const string CanPlay = "Up.WheelPlayer.CanPlay";
            public const string TodaySpin = "Up.WheelPlayer.TodaySpin";
            public const string History = "Up.WheelPlayer.History";
            public const string ViewAllPlays = "Up.WheelPlayer.ViewAllPlays";
            public const string ManageDelivery = "Up.WheelPlayer.ManageDelivery";
            public const string ViewDeliveryStatus = "Up.WheelPlayer.ViewDeliveryStatus";
            public const string ManagePhysicalDelivery = "Up.WheelPlayer.ManagePhysicalDelivery";
            public const string ViewPhysicalDelivery = "Up.WheelPlayer.ViewPhysicalDelivery";
        }

        public static class LoyaltyPointPermissions
        {
            public const string Create = "Up.LoyaltyPoint.Create";
            public const string CreateBonus = "Up.LoyaltyPoint.CreateBonus";
            public const string Deduct = "Up.LoyaltyPoint.Deduct";
            public const string GetOne = "Up.LoyaltyPoint.GetOne";
            public const string Redeem = "Up.LoyaltyPoint.Redeem";
        }

        public static class SmsTemplatePermissions
        {
            public const string Create = "Up.SmsTemplate.Create";
            public const string Edit = "Up.SmsTemplate.Edit";
            public const string GetAll = "Up.SmsTemplate.GetAll";
            public const string GetOne = "Up.SmsTemplate.GetOne";
            public const string Delete = "Up.SmsTemplate.Delete";
        }

        public static class TagPermissions
        {
            public const string Create = "Up.Tag.Create";
            public const string Edit = "Up.Tag.Edit";
            public const string GetAll = "Up.Tag.GetAll";
            public const string GetOne = "Up.Tag.GetOne";
            public const string Delete = "Up.Tag.Delete";
        }

        public static class CourseTagsPermissions
        {
            public const string AssignToCourse = "Up.CourseTags.AssignToCourse";
            public const string GetByCourse = "Up.CourseTags.GetByCourse";
            public const string GetByTag = "Up.CourseTags.GetByTag";
            public const string RemoveFromCourse = "Up.CourseTags.RemoveFromCourse";
            public const string GetCoursesByTag = "Up.CourseTags.GetCoursesByTag";
        }

        public static class CourseFieldPermissions
        {
            public const string Create = "Up.CourseField.Create";
            public const string Edit = "Up.CourseField.Edit";
            public const string GetAll = "Up.CourseField.GetAll";
            public const string GetOne = "Up.CourseField.GetOne";
            public const string Delete = "Up.CourseField.Delete";
            public const string Export = "Up.CourseField.Export";
        }

        public static class AIQuizPermissions
        {
            public const string GenerateAIQuiz = "Up.AIQuiz.GenerateAIQuiz";
            public const string GenerateAIQuizFromFile = "Up.AIQuiz.GenerateAIQuizFromFile";
        }

        public static class PermissionOfRolePermissions
        {
            [AdminOnly]
            public const string Create = "Up.PermissionOfRolePermissions.Create";
            [AdminOnly]
            public const string All = "Up.PermissionOfRolePermissions.All";
            [AdminOnly]
            public const string GetByRole = "Up.PermissionOfRolePermissions.GetByRole";
            [AdminOnly]
            public const string GetByUser = "Up.PermissionOfRolePermissions.GetByUser";
            [AdminOnly]
            public const string Delete = "Up.PermissionOfRolePermissions.Delete";
            [AdminOnly]
            public const string CheckUserPermission = "Up.PermissionOfRolePermissions.CheckUserPermission";
        }

        public static class RolePermissions
        {
            [AdminOnly]
            public const string Create = "Up.RolePermissions.Create";
            [AdminOnly]
            public const string Remove = "Up.RolePermissions.Remove";
            [AdminOnly]
            public const string AssignByEmail = "Up.RolePermissions.AssignByEmail";
            [AdminOnly]
            public const string Assign = "Up.RolePermissions.Assign";
            [AdminOnly]
            public const string Default = "Up.RolePermissions.Default";
            [AdminOnly]
            public const string Update = "Up.RolePermissions.Update";
            [AdminOnly]
            public const string Exists = "Up.RolePermissions.Exists";
            [AdminOnly]
            public const string UsersInRole = "Up.RolePermissions.UsersInRole";
            [AdminOnly]
            public const string GetAll = "Up.RolePermissions.GetAll";
            [AdminOnly]
            public const string Delete = "Up.RolePermissions.Delete";
        }
    }
}
