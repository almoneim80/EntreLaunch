namespace EntreLaunch.Infrastructure
{
    public static class Permissions
    {
        public static class AnswerPermissions
        {
            public const string Create = "EntreLaunch.Answer.Create";
            public const string Edit = "EntreLaunch.Answer.Edit";
            public const string ShowAll = "EntreLaunch.Answer.ShowAll";
            public const string ShowOne = "EntreLaunch.Answer.ShowOne";
            public const string Delete = "EntreLaunch.Answer.Delete";
            public const string Export = "EntreLaunch.Answer.Export";
            public const string Import = "EntreLaunch.Answer.Import";
        }

        public static class ExamPermissions
        {
            public const string Create = "EntreLaunch.Exam.Create";
            public const string Edit = "EntreLaunch.Exam.Edit";
            public const string ShowAll = "EntreLaunch.Exam.ShowAll";
            public const string ShowOne = "EntreLaunch.Exam.ShowOne";
            public const string GetAttempt = "EntreLaunch.Exam.GetAttempt";
            public const string CascadeDelete = "EntreLaunch.Exam.CascadeDelete";
            public const string Export = "EntreLaunch.Exam.Export";
            public const string CreateFull = "EntreLaunch.Exam.CreateFull";
            public const string Import = "EntreLaunch.Exam.Import";
            public const string GetEnumValues = "EntreLaunch.Exam.GetEnumValues";
            public const string CanRetake = "EntreLaunch.Exam.CanRetake";
            public const string Retake = "EntreLaunch.Exam.Retake";
        }

        public static class QuestionPermissions
        {
            public const string Create = "EntreLaunch.Question.Create";
            public const string Edit = "EntreLaunch.Question.Edit";
            public const string ShowAll = "EntreLaunch.Question.ShowAll";
            public const string ShowOne = "EntreLaunch.Question.ShowOne";
            public const string Delete = "EntreLaunch.Question.Delete";
            public const string Export = "EntreLaunch.Question.Export";
            public const string CreateFull = "EntreLaunch.Question.CreateFull";
        }

        public static class ExamResultPermissions
        {
            public const string Create = "EntreLaunch.ExamResult.Create";
            public const string Edit = "EntreLaunch.ExamResult.Edit";
            public const string ShowAll = "EntreLaunch.ExamResult.ShowAll";
            public const string ShowOne = "EntreLaunch.ExamResult.ShowOne";
            public const string Delete = "EntreLaunch.ExamResult.Delete";
            public const string Export = "EntreLaunch.ExamResult.Export";
        }

        public static class ClubPermissions
        {
            public const string Create = "EntreLaunch.CLub.Create";
            public const string SubscribeClub = "EntreLaunch.CLub.SubscribeClub";
            public const string SubscribeEvent = "EntreLaunch.CLub.SubscribeEvent";
            public const string RenewClubSubscription = "EntreLaunch.CLub.RenewClubSubscription";
            public const string Edit = "EntreLaunch.CLub.Edit";
            public const string ShowAll = "EntreLaunch.CLub.ShowAll";
            public const string ShowOne = "EntreLaunch.CLub.ShowOne";
            public const string GetEventSubscriber = "EntreLaunch.CLub.GetEventSubscriber";
            public const string SoftDelete = "EntreLaunch.CLub.SoftDelete";
            public const string Export = "EntreLaunch.CLub.Export";
            public const string CreateFull = "EntreLaunch.CLub.CreateFull";
            public const string ImportList = "EntreLaunch.CLub.ImportList";
            public const string ImportFile = "EntreLaunch.CLub.ImportFile";
            public const string GenerateTemplate = "EntreLaunch.CLub.GenerateTemplate";
            public const string GetUserSubscription = "EntreLaunch.CLub.GetUserSubscription";
            public const string UnsubscribeEvent = "EntreLaunch.CLub.UnsubscribeEvent";
        }

        public static class MyCommunityPermissions
        {
            public const string CreateTextPost = "EntreLaunch.MyCommunity.CreateTextPost";
            public const string CreatePostWithMedia = "EntreLaunch.MyCommunity.CreatePostWithMedia";
            public const string CreateMediaToPost = "EntreLaunch.MyCommunity.CreateMediaToPost";
            public const string CreateComment = "EntreLaunch.MyCommunity.CreateComment";
            public const string CreateLike = "EntreLaunch.MyCommunity.CreateLike";
            public const string CreateReport = "EntreLaunch.MyCommunity.CreateReport";
            public const string EntreLaunchdatePost = "EntreLaunch.MyCommunity.EntreLaunchdatePost";
            public const string EntreLaunchdateMedia = "EntreLaunch.MyCommunity.EntreLaunchdateMedia";
            public const string EntreLaunchdateComment = "EntreLaunch.MyCommunity.EntreLaunchdateComment";
            public const string GetAllPosts = "EntreLaunch.MyCommunity.GetAllPosts";
            public const string GetPostById = "EntreLaunch.MyCommunity.GetPostById";
            public const string GetPostLikeCount = "EntreLaunch.MyCommunity.GetPostLikeCount";
            public const string GetPostComments = "EntreLaunch.MyCommunity.GetPostComments";
            public const string GetPostReports = "EntreLaunch.MyCommunity.GetPostReports";
            public const string GetCommentReports = "EntreLaunch.MyCommunity.GetCommentReports";
            public const string GetPendingPosts = "EntreLaunch.MyCommunity.GetPendingPosts";
            public const string GetAcceptedPosts = "EntreLaunch.MyCommunity.GetAcceptedPosts";
            public const string GetRejectedPosts = "EntreLaunch.MyCommunity.GetRejectedPosts";
            public const string GetPendingReports = "EntreLaunch.MyCommunity.GetPendingReports";
            public const string GetAcceptedReports = "EntreLaunch.MyCommunity.GetAcceptedReports";
            public const string GetRejectedReports = "EntreLaunch.MyCommunity.GetRejectedReports";
            public const string ProcessPostStatus = "EntreLaunch.MyCommunity.ProcessPostStatus";
            public const string ProcessReportStatus = "EntreLaunch.MyCommunity.ProcessReportStatus";
            public const string ProcessCommentStatus = "EntreLaunch.MyCommunity.ProcessCommentStatus";
            public const string DeletePost = "EntreLaunch.MyCommunity.DeletePost";
            public const string DeleteComment = "EntreLaunch.MyCommunity.DeleteComment";
            public const string DeleteMedia = "EntreLaunch.MyCommunity.DeleteMedia";
            public const string DeletePostReport = "EntreLaunch.MyCommunity.DeletePostReport";
            public const string DeleteCommentReport = "EntreLaunch.MyCommunity.DeleteCommentReport";
        }

        public static class ConsultationPermissions
        {
            public const string BookingConsultation = "EntreLaunch.Consultation.BookingConsultation";
            public const string TextConsultation = "EntreLaunch.Consultation.TextConsultation";
            public const string ProgressStatus = "EntreLaunch.Consultation.ProgressStatus";
            public const string ShowAll = "EntreLaunch.Consultation.ShowAll";
            public const string ShowOne = "EntreLaunch.Consultation.ShowOne";
            public const string GetByType = "EntreLaunch.Consultation.GetByType";
            public const string Delete = "EntreLaunch.Consultation.Delete";
            public const string GetCounselorConsultations = "EntreLaunch.Consultation.GetCounselorConsultations";
            public const string GetEnumValues = "EntreLaunch.Consultation.GetEnumValues";
        }

        public static class CounselorPermissions
        {
            public const string Create = "EntreLaunch.Counselor.Create";
            public const string GetPending = "EntreLaunch.Counselor.GetPending";
            public const string ShowAll = "EntreLaunch.Counselor.ShowAll";
            public const string GetAccepted = "EntreLaunch.Counselor.GetAccepted";
            public const string GetRejected = "EntreLaunch.Counselor.GetRejected";
            public const string ProcessCounselorRequest = "EntreLaunch.Counselor.ProcessCounselorRequest";
            public const string CounselorBySpecialization = "EntreLaunch.Counselor.CounselorBySpecialization";
            public const string ShowCounselorCV = "EntreLaunch.Counselor.ShowCounselorCV";
        }

        public static class ConsultationTimePermissions
        {
            public const string Create = "EntreLaunch.ConsultationTime.Create";
            public const string Edit = "EntreLaunch.ConsultationTime.Edit";
            public const string ShowAll = "EntreLaunch.ConsultationTime.ShowAll";
            public const string ShowOne = "EntreLaunch.ConsultationTime.ShowOne";
            public const string CascadeDelete = "EntreLaunch.ConsultationTime.CascadeDelete";
            public const string Export = "EntreLaunch.ConsultationTime.Export";
            public const string Import = "EntreLaunch.ConsultationTime.Import";
        }

        public static class CoursePermissions
        {
            public const string Create = "EntreLaunch.Course.Create";
            public const string Edit = "EntreLaunch.Course.Edit";
            public const string ShowAll = "EntreLaunch.Course.ShowAll";
            public const string ShowOne = "EntreLaunch.Course.ShowOne";
            public const string CascadeDelete = "EntreLaunch.Course.CascadeDelete";
            public const string Export = "EntreLaunch.Course.Export";
            public const string CreateFull = "EntreLaunch.Course.CreateFull";
            public const string GetEnumValues = "EntreLaunch.Course.GetEnumValues";
            public const string Import = "EntreLaunch.Course.Import";
            public const string ChangeStatus = "EntreLaunch.Course.ChangeStatus";
            public const string GetByPaymentType = "EntreLaunch.Course.GetByPaymentType";
        }

        public static class CourseRatingPermissions
        {
            public const string Create = "EntreLaunch.CourseRating.Create";
            public const string Edit = "EntreLaunch.CourseRating.Edit";
            public const string ShowAll = "EntreLaunch.CourseRating.ShowAll";
            public const string ShowOne = "EntreLaunch.CourseRating.ShowOne";
            public const string Delete = "EntreLaunch.CourseRating.Delete";
            public const string Export = "EntreLaunch.CourseRating.Export";
            public const string GetRatingStats = "EntreLaunch.CourseRating.GetRatingStats";
            public const string GetAllByCourse = "EntreLaunch.CourseRating.GetAllByCourse";
            public const string GetSummary = "EntreLaunch.CourseRating.GetSummary";
            public const string GetByInstructor = "EntreLaunch.CourseRating.GetByInstructor";
            public const string Approve = "EntreLaunch.CourseRating.Approve";
            public const string Reject = "EntreLaunch.CourseRating.Reject";
            public const string GetByStatus = "EntreLaunch.CourseRating.GetByStatus";
        }

        public static class TrainingPathPermissions
        {
            public const string Create = "EntreLaunch.TrainingPath.Create";
            public const string Edit = "EntreLaunch.TrainingPath.Edit";
            public const string ShowAll = "EntreLaunch.TrainingPath.ShowAll";
            public const string ShowOne = "EntreLaunch.TrainingPath.ShowOne";
            public const string Delete = "EntreLaunch.TrainingPath.Delete";
            public const string Export = "EntreLaunch.TrainingPath.Export";
            public const string Import = "EntreLaunch.TrainingPath.Import";
        }

        public static class CourseInstructorPermissions
        {
            public const string Create = "EntreLaunch.CourseInstructor.Create";
            public const string Edit = "EntreLaunch.CourseInstructor.Edit";
            public const string ShowAll = "EntreLaunch.CourseInstructor.ShowAll";
            public const string ShowOne = "EntreLaunch.CourseInstructor.ShowOne";
            //public const string ShowByCourse = "EntreLaunch.CourseInstructor.ShowByCourse";
            public const string Delete = "EntreLaunch.CourseInstructor.Delete";
            public const string Export = "EntreLaunch.CourseInstructor.Export";
            public const string GetInstructorsByCourse = "EntreLaunch.CourseInstructor.GetInstructorsByCourse";
            public const string GetTrainerPerformance = "EntreLaunch.CourseInstructor.GetTrainerPerformance";
        }

        public static class CourseEnrollmentPermissions
        {
            public const string Create = "EntreLaunch.CourseEnrollment.Create";
            public const string Edit = "EntreLaunch.CourseEnrollment.Edit";
            public const string ShowAll = "EntreLaunch.CourseEnrollment.ShowAll";
            public const string ShowOne = "EntreLaunch.CourseEnrollment.ShowOne";
            public const string ShowByCourse = "EntreLaunch.CourseEnrollment.ShowByCourse";
            public const string Delete = "EntreLaunch.CourseEnrollment.Delete";
            public const string Export = "EntreLaunch.CourseEnrollment.Export";
            public const string Unenroll = "EntreLaunch.CourseEnrollment.Unenroll";
            public const string GetUserSubscriptions = "EntreLaunch.CourseEnrollment.GetUserSubscriptions";
        }

        public static class LessonPermissions
        {
            public const string Create = "EntreLaunch.Lesson.Create";
            public const string Edit = "EntreLaunch.Lesson.Edit";
            public const string ShowAll = "EntreLaunch.Lesson.ShowAll";
            public const string ShowOne = "EntreLaunch.Lesson.ShowOne";
            public const string CascadeDelete = "EntreLaunch.Lesson.CascadeDelete";
            public const string Export = "EntreLaunch.Lesson.Export";
            public const string CreateFull = "EntreLaunch.Lesson.CreateFull";
            public const string Import = "EntreLaunch.Lesson.Import";
            public const string Reorder = "EntreLaunch.Lesson.Reorder";
            public const string EntreLaunchdateProgress = "EntreLaunch.Lesson.EntreLaunchdateProgress";
            public const string GetProgress = "EntreLaunch.Lesson.GetProgress";
            public const string CalculateProgress = "EntreLaunch.Lesson.CalculateProgress";
            public const string GetByCourse = "EntreLaunch.Lesson.GetByCourse";
        }

        public static class LessonAttachmentPermissions
        {
            public const string Create = "EntreLaunch.LessonAttachment.Create";
            public const string Edit = "EntreLaunch.LessonAttachment.Edit";
            public const string ShowAll = "EntreLaunch.LessonAttachment.ShowAll";
            public const string ShowOne = "EntreLaunch.LessonAttachment.ShowOne";
            public const string Delete = "EntreLaunch.LessonAttachment.Delete";
            public const string Export = "EntreLaunch.LessonAttachment.Export";
            public const string Import = "EntreLaunch.LessonAttachment.Import";
            public const string OpenCounter = "EntreLaunch.LessonAttachment.OpenCounter";
            public const string GetStats = "EntreLaunch.LessonAttachment.GetStats";
            public const string ValidateFile = "EntreLaunch.LessonAttachment.ValidateFile";
        }

        public static class OpportunityPermissions
        {
            public const string Create = "EntreLaunch.InvestmentOpportunity.Create";
            public const string Edit = "EntreLaunch.InvestmentOpportunity.Edit";
            public const string ShowAll = "EntreLaunch.InvestmentOpportunity.ShowAll";
            public const string ShowOne = "EntreLaunch.InvestmentOpportunity.ShowOne";
            public const string Delete = "EntreLaunch.InvestmentOpportunity.Delete";
            public const string Export = "EntreLaunch.InvestmentOpportunity.Export";
            public const string Import = "EntreLaunch.InvestmentOpportunity.Import";
        }

        public static class MyOpportunityPermissions
        {
            public const string SenDOBportunityRequest = "EntreLaunch.OpportunityRequest.SenDOBportunityRequest";
            public const string ShowAll = "EntreLaunch.OpportunityRequest.ShowAll";
            public const string ShowPending = "EntreLaunch.OpportunityRequest.ShowPending";
            public const string ShowAccepted = "EntreLaunch.OpportunityRequest.ShowAccepted";
            public const string ShowRejected = "EntreLaunch.OpportunityRequest.ShowRejected";
            public const string ProgressRequest = "EntreLaunch.OpportunityRequest.ProgressRequest";
            public const string ShowOpportunities = "EntreLaunch.OpportunityRequest.ShowOpportunities";
            public const string Filter = "EntreLaunch.InvestmentOpportunity.Filter";
        }

        public static class LocalizationPermissions
        {
            public const string FirstTimeSetEntreLaunchOrDefault = "EntreLaunch.Localization.FirstTimeSetEntreLaunchOrDefault";
        }

        public static class NotificationPermissions
        {
            public const string Create = "EntreLaunch.Notification.Create";
            public const string Edit = "EntreLaunch.Notification.Edit";
            public const string ShowAll = "EntreLaunch.Notification.ShowAll";
            public const string ShowOne = "EntreLaunch.Notification.ShowOne";
            public const string Delete = "EntreLaunch.Notification.Delete";
            public const string Export = "EntreLaunch.Notification.Export";
            public const string GetEnumValues = "EntreLaunch.Notification.GetEnumValues";
        }

        public static class PaymentPermissions
        {
            public const string Create = "EntreLaunch.Payment.Create";
            public const string ShowAll = "EntreLaunch.Payment.ShowAll";
        }

        public static class PortfolioPermissions
        {
            public const string Create = "EntreLaunch.Portfolio.Create";
            public const string Edit = "EntreLaunch.Portfolio.Edit";
            public const string ShowAll = "EntreLaunch.Portfolio.ShowAll";
            public const string ShowOne = "EntreLaunch.Portfolio.ShowOne";
            public const string CascadeDelete = "EntreLaunch.Portfolio.CascadeDelete";
            public const string Export = "EntreLaunch.Portfolio.Export";
            public const string CreateFull = "EntreLaunch.Portfolio.CreateFull";
        }

        public static class PortfolioAttachmentPermissions
        {
            public const string Create = "EntreLaunch.PortfolioAttachment.Create";
            public const string Edit = "EntreLaunch.PortfolioAttachment.Edit";
            public const string ShowAll = "EntreLaunch.PortfolioAttachment.ShowAll";
            public const string ShowOne = "EntreLaunch.PortfolioAttachment.ShowOne";
            public const string Delete = "EntreLaunch.PortfolioAttachment.Delete";
            public const string Export = "EntreLaunch.PortfolioAttachment.Export";
            public const string Import = "EntreLaunch.PortfolioAttachment.Import";
        }

        public static class SimulationPermissions
        {
            public const string Create = "EntreLaunch.Simulation.Create";
            public const string EntreLaunchdate = "EntreLaunch.Simulation.EntreLaunchdate";
            public const string GetAll = "EntreLaunch.Simulation.GetAll";
            public const string GetOne = "EntreLaunch.Simulation.GetOne";
            public const string GetAds = "EntreLaunch.Simulation.GetAds";
            public const string GetByStatus = "EntreLaunch.Simulation.GetByStatus";
            public const string LikeCount = "EntreLaunch.Simulation.LikeCount";
            public const string Delete = "EntreLaunch.Simulation.Delete";
        }

        public static class MyPartnerPermissions
        {
            public const string Create = "EntreLaunch.MyPartner.Create";
            public const string Edit = "EntreLaunch.MyPartner.Edit";
            public const string ShowAll = "EntreLaunch.MyPartner.ShowAll";
            public const string ShowOne = "EntreLaunch.MyPartner.ShowOne";
            public const string ShowAttachment = "EntreLaunch.MyPartner.ShowAttachment";
            public const string ShowPending = "EntreLaunch.MyPartner.ShowPending";
            public const string ShowAccepted = "EntreLaunch.MyPartner.ShowAccepted";
            public const string ShowRejected = "EntreLaunch.MyPartner.ShowRejected";
            public const string Filter = "EntreLaunch.MyPartner.Filter";
            public const string ProgressProject = "EntreLaunch.MyPartner.ProgressProject";
        }

        public static class ProjectPurchasePermissions
        {
            public const string Create = "EntreLaunch.ProjectPurchase.Create";
            public const string Edit = "EntreLaunch.ProjectPurchase.Edit";
            public const string ShowAll = "EntreLaunch.ProjectPurchase.ShowAll";
            public const string ShowOne = "EntreLaunch.ProjectPurchase.ShowOne";
            public const string Delete = "EntreLaunch.ProjectPurchase.Delete";
            public const string Export = "EntreLaunch.ProjectPurchase.Export";
            public const string Import = "EntreLaunch.ProjectPurchase.Import";
        }

        public static class QualificationPermissions
        {
            public const string Create = "EntreLaunch.Qualification.Create";
            public const string Edit = "EntreLaunch.Qualification.Edit";
            public const string ShowAll = "EntreLaunch.Qualification.ShowAll";
            public const string ShowOne = "EntreLaunch.Qualification.ShowOne";
            public const string Delete = "EntreLaunch.Qualification.Delete";
            public const string Export = "EntreLaunch.Qualification.Export";
            public const string Import = "EntreLaunch.Qualification.Import";
        }

        public static class RefundPermissions
        {
            public const string Create = "EntreLaunch.Refund.Create";
            public const string Approve = "EntreLaunch.Refund.Approve";
            public const string ShowAll = "EntreLaunch.Refund.ShowAll";
            public const string ShowOne = "EntreLaunch.Refund.ShowOne";
            public const string Reject = "EntreLaunch.Refund.Reject";
            public const string Export = "EntreLaunch.Refund.Export";
        }

        public static class StudentCertificatePermissions
        {
            public const string Create = "EntreLaunch.StudentCertificate.Create";
            public const string Edit = "EntreLaunch.StudentCertificate.Edit";
            public const string ShowAll = "EntreLaunch.StudentCertificate.ShowAll";
            public const string ShowOne = "EntreLaunch.StudentCertificate.ShowOne";
            public const string Delete = "EntreLaunch.StudentCertificate.Delete";
            public const string Export = "EntreLaunch.StudentCertificate.Export";
            public const string GetEnumValues = "EntreLaunch.StudentCertificate.GetEnumValues";
        }

        public static class TicketPermissions
        {
            public const string Create = "EntreLaunch.Ticket.Create";
            public const string Edit = "EntreLaunch.Ticket.Edit";
            public const string ShowAll = "EntreLaunch.Ticket.ShowAll";
            public const string ShowOne = "EntreLaunch.Ticket.ShowOne";
            public const string ShowByConsultation = "EntreLaunch.Ticket.ShowByConsultation";
            public const string CascadeDelete = "EntreLaunch.Ticket.CascadeDelete";
            public const string Export = "EntreLaunch.Ticket.Export";
            public const string Process = "EntreLaunch.Ticket.Process";
        }

        public static class TicketMessagePermissions
        {
            public const string Create = "EntreLaunch.TicketMessage.Create";
            public const string Edit = "EntreLaunch.TicketMessage.Edit";
            public const string ShowAll = "EntreLaunch.TicketMessage.ShowAll";
            public const string ShowOne = "EntreLaunch.TicketMessage.ShowOne";
            public const string ShowByTicke = "EntreLaunch.TicketMessage.ShowByTicke";
            public const string Delete = "EntreLaunch.TicketMessage.Delete";
            public const string Export = "EntreLaunch.TicketMessage.Export";
        }

        public static class TicketAttachmentPermissions
        {
            public const string Create = "EntreLaunch.TicketAttachment.Create";
            public const string Edit = "EntreLaunch.TicketAttachment.Edit";
            public const string ShowAll = "EntreLaunch.TicketAttachment.ShowAll";
            public const string ShowOne = "EntreLaunch.TicketAttachment.ShowOne";
            public const string Delete = "EntreLaunch.TicketAttachment.Delete";
            public const string Export = "EntreLaunch.TicketAttachment.Export";
            public const string Import = "EntreLaunch.TicketAttachment.Import";
        }

        public static class MyTeamPermissions
        {
            public const string Create = "EntreLaunch.MyTeam.Create";
            public const string ChangeStatus = "EntreLaunch.MyTeam.ChangeStatus";
            public const string EntreLaunchdateEmployee = "EntreLaunch.MyTeam.EntreLaunchdateEmployee";
            public const string EntreLaunchdatePortfolio = "EntreLaunch.MyTeam.EntreLaunchdatePortfolio";
            public const string EntreLaunchdatePortfolioAttachment = "EntreLaunch.MyTeam.EntreLaunchdatePortfolioAttachment";
            public const string GetAll = "EntreLaunch.MyTeam.GetAll";
            public const string PendingEmployees = "EntreLaunch.MyTeam.PendingEmployees";
            public const string AcceptedEmployees = "EntreLaunch.MyTeam.AcceptedEmployees";
            public const string RejectedEmployees = "EntreLaunch.MyTeam.RejectedEmployees";
            public const string Filtering = "EntreLaunch.MyTeam.Filtering";
            public const string GetEmployeeById = "EntreLaunch.MyTeam.GetEmployeeById";
            public const string GetPortfoliosForEmployee = "EntreLaunch.MyTeam.GetPortfoliosForEmployee";
        }

        public static class UserPermissions
        {
            public const string Create = "EntreLaunch.User.Create";
            public const string Complete = "EntreLaunch.User.Complete";
            public const string Edit = "EntreLaunch.User.Edit";
            public const string ShowAll = "EntreLaunch.User.ShowAll";
            public const string ShowOne = "EntreLaunch.User.ShowOne";
            public const string ShowMe = "EntreLaunch.User.ShowMe";
            public const string Delete = "EntreLaunch.User.Delete";
            public const string Export = "EntreLaunch.User.Export";
            public const string GetEnumValues = "EntreLaunch.User.GetEnumValues";
        }

        public static class WheelAwardPermissions
        {
            public const string Create = "EntreLaunch.WheelAward.Create";
            public const string Complete = "EntreLaunch.WheelAward.Complete";
            public const string Edit = "EntreLaunch.WheelAward.Edit";
            public const string ShowAll = "EntreLaunch.WheelAward.ShowAll";
            public const string ShowOne = "EntreLaunch.WheelAward.ShowOne";
            public const string Delete = "EntreLaunch.WheelAward.Delete";
            public const string Export = "EntreLaunch.WheelAward.Export";
            public const string Import = "EntreLaunch.WheelAward.Import";
        }

        public static class WheelPlayerPermissions
        {
            public const string Create = "EntreLaunch.WheelAward.Create";
            public const string Complete = "EntreLaunch.WheelAward.Complete";
            public const string Edit = "EntreLaunch.WheelAward.Edit";
            public const string ShowAll = "EntreLaunch.WheelAward.ShowAll";
            public const string ShowOne = "EntreLaunch.WheelAward.ShowOne";
            public const string Delete = "EntreLaunch.WheelAward.Delete";
            public const string Export = "EntreLaunch.WheelAward.Export";
        }

        public static class LoyaltyPointPermissions
        {
            public const string Create = "EntreLaunch.LoyaltyPoint.Create";
            public const string CreateBonus = "EntreLaunch.LoyaltyPoint.CreateBonus";
            public const string Deduct = "EntreLaunch.LoyaltyPoint.Deduct";
            public const string ShowOne = "EntreLaunch.LoyaltyPoint.ShowOne";
            public const string Redeem = "EntreLaunch.LoyaltyPoint.Redeem";
        }

        public static class SmsTemplatePermissions
        {
            public const string Create = "EntreLaunch.SmsTemplate.Create";
            public const string Edit = "EntreLaunch.SmsTemplate.Edit";
            public const string ShowAll = "EntreLaunch.SmsTemplate.ShowAll";
            public const string ShowOne = "EntreLaunch.SmsTemplate.ShowOne";
            public const string Delete = "EntreLaunch.SmsTemplate.Delete";
        }

        public static class TagPermissions
        {
            public const string Create = "EntreLaunch.Tag.Create";
            public const string Edit = "EntreLaunch.Tag.Edit";
            public const string ShowAll = "EntreLaunch.Tag.ShowAll";
            public const string ShowOne = "EntreLaunch.Tag.ShowOne";
            public const string Delete = "EntreLaunch.Tag.Delete";
        }

        public static class CourseTagsPermissions
        {
            public const string AssignToCourse = "EntreLaunch.CourseTags.AssignToCourse";
            public const string GetByCourse = "EntreLaunch.CourseTags.GetByCourse";
            public const string GetByTag = "EntreLaunch.CourseTags.GetByTag";
            public const string RemoveFromCourse = "EntreLaunch.CourseTags.RemoveFromCourse";
        }

        public static class CourseFieldPermissions
        {
            public const string Create = "EntreLaunch.CourseField.Create";
            public const string Edit = "EntreLaunch.CourseField.Edit";
            public const string ShowAll = "EntreLaunch.CourseField.ShowAll";
            public const string ShowOne = "EntreLaunch.CourseField.ShowOne";
            public const string Delete = "EntreLaunch.CourseField.Delete";
            public const string Export = "EntreLaunch.CourseField.Export";
        }

        public static class AIQuizPermissions
        {
            public const string GenerateAIQuiz = "EntreLaunch.AIQuiz.GenerateAIQuiz";
            public const string GenerateAIQuizFromFile = "EntreLaunch.AIQuiz.GenerateAIQuizFromFile";
        }

        public static class PermissionOfRolePermissions
        {
            [AdminOnly]
            public const string Create = "EntreLaunch.PermissionOfRolePermissions.Create";
            [AdminOnly]
            public const string All = "EntreLaunch.PermissionOfRolePermissions.All";
            [AdminOnly]
            public const string ShowByRole = "EntreLaunch.PermissionOfRolePermissions.ShowByRole";
            [AdminOnly]
            public const string ShowByUser = "EntreLaunch.PermissionOfRolePermissions.ShowByUser";
            [AdminOnly]
            public const string Delete = "EntreLaunch.PermissionOfRolePermissions.Delete";
            [AdminOnly]
            public const string CheckUserPermission = "EntreLaunch.PermissionOfRolePermissions.CheckUserPermission";
        }

        public static class RolePermissions
        {
            [AdminOnly]
            public const string Create = "EntreLaunch.RolePermissions.Create";
            [AdminOnly]
            public const string Remove = "EntreLaunch.RolePermissions.Remove";
            [AdminOnly]
            public const string AssignByEmail = "EntreLaunch.RolePermissions.AssignByEmail";
            [AdminOnly]
            public const string Assign = "EntreLaunch.RolePermissions.Assign";
            [AdminOnly]
            public const string Default = "EntreLaunch.RolePermissions.Default";
            [AdminOnly]
            public const string EntreLaunchdate = "EntreLaunch.RolePermissions.EntreLaunchdate";
            [AdminOnly]
            public const string Exists = "EntreLaunch.RolePermissions.Exists";
            [AdminOnly]
            public const string UsersInRole = "EntreLaunch.RolePermissions.UsersInRole";
            [AdminOnly]
            public const string ShowAll = "EntreLaunch.RolePermissions.ShowAll";
            [AdminOnly]
            public const string Delete = "EntreLaunch.RolePermissions.Delete";
        }
    }
}
