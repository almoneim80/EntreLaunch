using EntreLaunch.DTOs.AuthenticationDtos;
using EntreLaunch.DTOs.BlogDtos;
using EntreLaunch.DTOs.ClubDtos;
using EntreLaunch.DTOs.ConsultationDtos;
using EntreLaunch.DTOs.EmailDtos;
using EntreLaunch.DTOs.ExamDtos;
using EntreLaunch.DTOs.LessonDtos;
using EntreLaunch.DTOs.MyCommunityDtos;
using EntreLaunch.DTOs.MyOpportunityDtos;
using EntreLaunch.DTOs.MyPartnerDtos;
using EntreLaunch.DTOs.MyTeamDtos;
using EntreLaunch.DTOs.PaymentDtos;
using EntreLaunch.DTOs.SimulationDtos;
using EntreLaunch.DTOs.SMSDtos;
using EntreLaunch.DTOs.TrainingDtos;
using EntreLaunch.DTOs.UserDtos;
using EntreLaunch.DTOs.WheelDtos;

namespace EntreLaunch.Configuration;

public class AutoMapperProfiles : AutoMapper.Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<bool?, bool>().ConvertUsing((src, dest) => src ?? dest);
        CreateMap<int?, int>().ConvertUsing((src, dest) => src ?? dest);
        CreateMap<decimal?, decimal>().ConvertUsing((src, dest) => src ?? dest);
        CreateMap<List<DnsRecord>?, List<DnsRecord>>().ConvertUsing((src, dest) => src ?? dest);
        CreateMap<Dictionary<string, string>?, Dictionary<string, string>>().ConvertUsing((src, dest) => src ?? dest);
        CreateMap<string?[], string?[]>().ConvertUsing((src, dest) => src ?? dest);
        CreateMap<CommentStatus?, CommentStatus>().ConvertUsing((src, dest) => src ?? dest);

        // GoogleUserInfoResponse -> ExternalRegisterDto
        CreateMap<GoogleUserInfoResponse, ExternalRegisterDto>()
            .ForMember(dest => dest.UserInfo, opt => opt.MapFrom(src => new UserInfo
            {
                Id = src.Id,
                Email = src.Email,
                Name = src.Name,
                ConfirmedEmail = src.VerifiedEmail
            }))
            .ForMember(dest => dest.Provider, opt => opt.MapFrom(_ => "Google"))
            .ForMember(dest => dest.ProviderKey, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ProviderDisplayName, opt => opt.MapFrom(_ => "Google Account"))
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<User, UserInfo>().ReverseMap();
        CreateMap<User, UserInfo>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<UserInfo, User>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        CreateMap<DateTimeOffset, DateTimeOffset>().ConvertUsing(new DateTimeOffsetToUtcConverter());
        CreateMap<DateTimeOffset?, DateTimeOffset?>().ConvertUsing(new DateTimeOffsetToUtcConverter());
        CreateMap<DateTimeOffset?, DateTimeOffset>().ConvertUsing(new DateTimeOffsetToUtcConverter());

        // EmailTemplate
        CreateMap<EmailTemplateCreateDto, EmailTemplate>().ReverseMap();
        CreateMap<EmailTemplateUpdateDto, EmailTemplate>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<EmailTemplate, EmailTemplateUpdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<EmailTemplate, EmailTemplateDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // EmailGroup
        CreateMap<EmailGroupCreateDto, EmailGroup>().ReverseMap();
        CreateMap<EmailGroupUpdateDto, EmailGroup>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<EmailGroup, EmailGroupUpdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<EmailGroup, EmailGroupDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Domain
        CreateMap<Domain, DomainCreateDto>().ReverseMap();
        CreateMap<Domain, DomainUpdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<DomainUpdateDto, Domain>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Domain, DomainDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<DomainImportDto, Domain>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Domain, EmailVerifyDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Activity log
        CreateMap<ActivityLog, ActivityLogDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // User
        CreateMap<User, UserCreateDto>().ReverseMap();
        CreateMap<User, UserCreateDto>().ReverseMap();
        CreateMap<User, UserUpdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<UserUpdateDto, User>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<User, UserDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<User, UserDetailsDto>();
        CreateMap<User, CompleteUserDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<CompleteUserDetailsDto, User>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // course path
        CreateMap<TrainingPath, TrainingPathCreateDto>().ReverseMap();
        CreateMap<TrainingPath, TrainingPathUpdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<TrainingPathUpdateDto, TrainingPath>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<TrainingPath, TrainingPathDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // course Feild
        CreateMap<CourseField, CourseFieldCreateDto>().ReverseMap();
        CreateMap<CourseField, CourseFieldUpdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<CourseFieldUpdateDto, CourseField>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<CourseField, CourseFieldDetailsDto>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt ?? DateTimeOffset.MinValue));
        // course 
        CreateMap<Course, OnlineCourseUpdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<OnlineCourseUpdateDto, Course>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        CreateMap<Course, SkillCourseUpdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<SkillCourseUpdateDto, Course>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        CreateMap<Course, PathCourseUpdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<PathCourseUpdateDto, Course>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // CourseInstructor 
        CreateMap<CourseInstructor, CourseInstructorCreateDto>().ReverseMap();
        CreateMap<CourseInstructor, CourseInstructorUpdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<CourseInstructorUpdateDto, CourseInstructor>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<CourseInstructor, CourseInstructorDetailsDto>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.CountryCode, opt => opt.MapFrom(src => src.User.CountryCode))
            .ForMember(dest => dest.Specialization, opt => opt.MapFrom(src => src.User.Specialization))
            .ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // CourseRating 
        CreateMap<CourseRating, CourseRatingCreateDto>().ReverseMap();
        CreateMap<CourseRating, CourseRatingUpdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<CourseRatingUpdateDto, CourseRating>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<CourseRating, CourseRatingDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<CourseRating, CourseRatingDetailsDto>()
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Name))
            .ForMember(dest => dest.RatingValue, opt => opt.MapFrom(src => src.Rating))
            .ForMember(dest => dest.ReviewerName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));

        // Exam 
        CreateMap<Exam, FullLessonExamDto>().ReverseMap();
        CreateMap<Exam, FullCourseExamDto>().ReverseMap();
        CreateMap<Exam, FullPathExamDto>().ReverseMap();
        CreateMap<Exam, UpdateLessonExamDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<UpdateLessonExamDto, Exam>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Exam, UpdateCourseExamDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<UpdateCourseExamDto, Exam>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Exam, UpdatePathExamDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<UpdatePathExamDto, Exam>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Question
        CreateMap<Question, QuestionCreateDto>().ReverseMap();
        CreateMap<Question, QuestionUpdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<QuestionUpdateDto, Question>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Question, QuestionDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Question, QuestionExportDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Lesson
        CreateMap<Lesson, LessonCreateDto>().ReverseMap();
        CreateMap<Lesson, LessonUpdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<LessonUpdateDto, Lesson>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Answer
        CreateMap<Answer, AnswerCreateDto>().ReverseMap();
        CreateMap<Answer, AnswerUpdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<AnswerUpdateDto, Answer>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Answer, AnswerDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Answer, AnswerExportDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<AnswerImportDto, Answer>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // LessonAttachment
        CreateMap<LessonAttachment, LessonAttachmentCreateDto>().ReverseMap();
        CreateMap<LessonAttachment, LessonAttachmentUpdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<LessonAttachmentUpdateDto, LessonAttachment>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<LessonAttachment, LessonAttachmentDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Result
        CreateMap<ExamResult, ExamResultCreateDto>().ReverseMap();
        CreateMap<ExamResult, ExamResultUpdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<ExamResultUpdateDto, ExamResult>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<ExamResult, ExamResultDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<ExamResult, ExamResultExportDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        CreateMap<EmailGroup, EmailGroupExportDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Consultation
        CreateMap<Consultation, OnlineConsultationCreateDto>().ReverseMap();
        CreateMap<Consultation, TextConsultationCreateDto>().ReverseMap();
        CreateMap<Consultation, ConsultationUpdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<ConsultationUpdateDto, Consultation>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Consultation, ConsultationDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // ConsultationTime
        CreateMap<ConsultationTime, ConsultationTimeCreateDto>().ReverseMap();
        CreateMap<ConsultationTime, ConsultationTimeDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<ConsultationTime, ConsultationTimeUpdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<ConsultationTimeUpdateDto, ConsultationTime>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<ConsultationTimeImportDto, ConsultationTime>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Counselor
        CreateMap<Counselor, CreateCounselorRequestDto>().ReverseMap();
        CreateMap<Counselor, CounselorRequestDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Ticket
        CreateMap<ConsultationTicket, TicketCreateDto>().ReverseMap();

        // TicketMessage
        CreateMap<ConsultationTicketMessage, TicketMessageCreateDto>().ReverseMap();
        CreateMap<ConsultationTicketMessage, TicketMessageUpdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<TicketMessageUpdateDto, ConsultationTicketMessage>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<ConsultationTicketMessage, TicketMessageDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // TicketAttachment
        CreateMap<ConsultationTicketAttachment, TicketAttachmentCreateDto>().ReverseMap();
        CreateMap<ConsultationTicketAttachment, TicketAttachmentDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Blog
        CreateMap<Blog, BlogCreateDto>().ReverseMap();

        // Post
        CreateMap<Post, PostWithMediaCreateDto>().ReverseMap();
        CreateMap<Post, TextPostCreateDto>().ReverseMap();
        CreateMap<Post, PostDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Media
        CreateMap<PostMedia, MediaCreateDto>().ReverseMap();
        CreateMap<PostMedia, MediaDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Post Like
        CreateMap<PostLike, LikeCreateDto>().ReverseMap();

        // Report
        CreateMap<CommunityReport, ReportCreateDto>().ReverseMap();
        CreateMap<CommunityReport, ReportDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // simulation
        CreateMap<Simulation, ProjectCreateDto>().ReverseMap();

        // Simulation Idea Strength
        CreateMap<SimulationIdeaPower, IdeaPowerCreateDto>().ReverseMap();

        // My Partner
        CreateMap<MyPartner, MyPartnerCreateDto>().ReverseMap();
        CreateMap<MyPartner, MyPartnerUpdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<MyPartnerUpdateDto, MyPartner>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<MyPartner, MyPartnerDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<MyPartnerAttachmentCreateDto, MyPartnerAttachment>();
        CreateMap<MyPartnerAttachment, ProjectAttachmentDetailsDto>();
        CreateMap<MyPartner, MyPartnerDetailsDto>().ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.ProjectAttachments)).ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<ProjectAttachmentUpdateDto, MyPartnerAttachment>();


        // Employee
        CreateMap<Employee, EmployeeCreateDto>().ReverseMap();
        CreateMap<Employee, EmployeeUpdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<EmployeeUpdateDto, Employee>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Employee, EmployeeDetailsDto>().ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Portfolios, opt => opt.MapFrom(src => src.Portfolios));

        // Employee Portfolio
        CreateMap<EmployeePortfolio, EmployeePortfolioCreateDto>().ReverseMap();
        CreateMap<EmployeePortfolio, EmployeePortfolioUpdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<EmployeePortfolioUpdateDto, EmployeePortfolio>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<EmployeePortfolio, EmployeePortfolioDetailsDto>()
            .ForMember(dest => dest.PortfolioAttachments, opt => opt.MapFrom(src => src.PortfolioAttachments));

        // Portfolio Attachment
        CreateMap<PortfolioAttachment, PortfolioAttachmentCreateDto>().ReverseMap();
        CreateMap<PortfolioAttachment, PortfolioAttachmentUpdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<PortfolioAttachmentUpdateDto, PortfolioAttachment>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<PortfolioAttachment, PortfolioAttachmentDetailsDto>();

        // My Opportunities
        CreateMap<Opportunity, OpportunityCreateDto>().ReverseMap();
        CreateMap<Opportunity, OpportunityUpdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<OpportunityUpdateDto, Opportunity>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Opportunity, OpportunityDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<OpportunityImportDto, Opportunity>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Opportunity, OpportunityExportDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Investment opportunities request
        CreateMap<OpportunityRequest, CreateOpportunityRequestDto>().ReverseMap();
        CreateMap<OpportunityRequest, OpportunityRequestDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // ClubEvent
        CreateMap<ClubEvent, ClubEventCreateDto>().ReverseMap();
        CreateMap<ClubEvent, ClubEventUpdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<ClubEventUpdateDto, ClubEvent>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<ClubEvent, ClubEventDetails>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<ClubEventImportDto, ClubEvent>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // WheelAward
        CreateMap<WheelAward, WheelAwardCreateDto>().ReverseMap();
        CreateMap<WheelAward, WheelAwardUpdateDto>()
            .ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<WheelAwardUpdateDto, WheelAward>()
            .ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<WheelAward, WheelAwardDetailsDto>()
            .ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<WheelAward, WheelAwardExportDto>()
            .ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // WheelPlayer
        CreateMap<WheelPlayer, WheelPlayerCreateDto>().ReverseMap();
        CreateMap<WheelPlayer, WheelPlayerDetailsDto>()
            .ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<WheelPlayer, WheelPlayerExportDto>()
            .ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Notification
        CreateMap<Notification, NotificationCreateDto>().ReverseMap();
        CreateMap<Notification, NotificationUpdateDto>()
            .ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<NotificationUpdateDto, Notification>()
            .ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Notification, NotificationDetailsDto>()
            .ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Notification, NotificationExportDto>()
            .ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Payment
        CreateMap<Payment, PaymentCreateDto>().ReverseMap();
        CreateMap<Payment, PaymentDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Refund
        CreateMap<Refund, RefundCreateDto>().ReverseMap();

        // sms template
        CreateMap<SmsTemplate, SmsTemplateCreateDto>().ReverseMap();
        CreateMap<SmsTemplate, SmsTemplateUpdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<SmsTemplateUpdateDto, SmsTemplate>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<SmsTemplate, SmsTemplateDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // for import muliple lessons with related content
        CreateMap<LessonWithRelatedContent, Lesson>()
                    .ForMember(dest => dest.Exams, opt => opt.MapFrom(src => src.Exercises))
                    .ForMember(dest => dest.LessonAttachments, opt => opt.MapFrom(src => src.Attachments));

        CreateMap<LessonExcerise, Exam>()
            .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.Questions))
            .ForMember(dest => dest.LessonId, opt => opt.Ignore());

        CreateMap<AttachmentOfLesson, LessonAttachment>()
            .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => Path.GetFileName(src.FileUrl ?? "file.pdf")))
            .ForMember(dest => dest.OpenCount, opt => opt.MapFrom(_ => 0));

        CreateMap<ExcersiseQuestions, Question>()
            .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Choices))
            .ForMember(dest => dest.ExamId, opt => opt.Ignore());

        CreateMap<QuestionChoise, Answer>()
            .ForMember(dest => dest.QuestionId, opt => opt.Ignore());
    }

    private static bool PropertyNeedsMapping(object source, object target, object sourceValue, object targetValue)
    {
        if (sourceValue is null or (object)"")
        {
            return false;
        }

        var defaultValue = sourceValue.GetType().IsValueType ? Activator.CreateInstance(sourceValue.GetType()) : null;
        return !sourceValue.Equals(defaultValue);
    }
}

public class DateTimeOffsetToUtcConverter :
    ITypeConverter<DateTimeOffset, DateTimeOffset>,
    ITypeConverter<DateTimeOffset?, DateTimeOffset?>,
    ITypeConverter<DateTimeOffset?, DateTimeOffset>
{
    public DateTimeOffset Convert(DateTimeOffset source, DateTimeOffset destination, ResolutionContext context)
    {
        return source.ToUniversalTime();
    }

    public DateTimeOffset? Convert(DateTimeOffset? source, DateTimeOffset? destination, ResolutionContext context)
    {
        if (source == null)
        {
            return destination;
        }

        return Convert(source.Value, destination ?? DateTimeOffset.MinValue, context);
    }

    public DateTimeOffset Convert(DateTimeOffset? source, DateTimeOffset destination, ResolutionContext context)
    {
        if (source == null)
        {
            return destination;
        }

        return Convert(source.Value, destination, context);
    }
}
