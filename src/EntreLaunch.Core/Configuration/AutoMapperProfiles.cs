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

        // Content
        CreateMap<ContentCreateDto, Content>().ReverseMap();
        CreateMap<ContentEntreLaunchdateDto, Content>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Content, ContentEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Content, ContentDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<ContentImportDto, Content>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // EmailTemplate
        CreateMap<EmailTemplateCreateDto, EmailTemplate>().ReverseMap();
        CreateMap<EmailTemplateEntreLaunchdateDto, EmailTemplate>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<EmailTemplate, EmailTemplateEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<EmailTemplate, EmailTemplateDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Contact
        CreateMap<ContactCreateDto, Contact>().ReverseMap();
        CreateMap<ContactEntreLaunchdateDto, Contact>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Contact, ContactEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Contact, ContactDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<ContactImportDto, Contact>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Promotion
        CreateMap<PromotionCreateDto, Promotion>().ReverseMap();
        CreateMap<PromotionEntreLaunchdateDto, Promotion>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Promotion, PromotionEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Promotion, PromotionDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Promotion, PromotionExportDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // EmailGroup
        CreateMap<EmailGroupCreateDto, EmailGroup>().ReverseMap();
        CreateMap<EmailGroEntreLaunchEntreLaunchdateDto, EmailGroup>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<EmailGroup, EmailGroEntreLaunchEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<EmailGroup, EmailGroEntreLaunchDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Link
        CreateMap<Link, LinkCreateDto>().ReverseMap();
        CreateMap<Link, LinkEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<LinkEntreLaunchdateDto, Link>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Link, LinkDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<LinkImportDto, Link>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Domain
        CreateMap<Domain, DomainCreateDto>().ReverseMap();
        CreateMap<Domain, DomainEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<DomainEntreLaunchdateDto, Domain>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Domain, DomainDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<DomainImportDto, Domain>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Domain, EmailVerifyDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Account
        CreateMap<Account, AccountCreateDto>().ReverseMap();
        CreateMap<Account, AccountEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<AccountEntreLaunchdateDto, Account>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Account, AccountDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Account, AccountExportDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<AccountImportDto, Account>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<AccountDetailsInfo, Account>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Activity log
        CreateMap<ActivityLog, ActivityLogDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Unsubscribe, UnsubscribeDto>().ReverseMap();
        CreateMap<Unsubscribe, UnsubscribeDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<UnsubscribeImportDto, Unsubscribe>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Unsubscribe, UnsubscribeExportDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // User
        CreateMap<User, UserCreateDto>().ReverseMap();
        CreateMap<User, UserCreateDto>().ReverseMap();
        CreateMap<User, UserEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<UserEntreLaunchdateDto, User>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<User, UserDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<User, UserDetailsDto>();
        CreateMap<User, CompleteUserDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<CompleteUserDetailsDto, User>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // course path
        CreateMap<TrainingPath, TrainingPathCreateDto>().ReverseMap();
        CreateMap<TrainingPath, TrainingPathEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<TrainingPathEntreLaunchdateDto, TrainingPath>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<TrainingPath, TrainingPathDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<TrainingPathImportDto, TrainingPath>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<TrainingPath, TrainingPathExportDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // course Feild
        CreateMap<CourseField, CourseFieldCreateDto>().ReverseMap();
        CreateMap<CourseField, CourseFieldEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<CourseFieldEntreLaunchdateDto, CourseField>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<CourseField, CourseFieldDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<CourseField, CourseFieldExportDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // course 
        CreateMap<Course, CourseCreateDto>().ReverseMap();
        CreateMap<Course, CourseEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<CourseEntreLaunchdateDto, Course>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Course, CourseDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Course, CourseExportDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<CourseImportDto, Course>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // CourseEnrollment 
        CreateMap<CourseEnrollment, CourseEnrollmentCreateDto>().ReverseMap();
        CreateMap<CourseEnrollment, CourseEnrollmentEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<CourseEnrollmentEntreLaunchdateDto, CourseEnrollment>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<CourseEnrollment, CourseEnrollmentDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<CourseEnrollment, CourseEnrollmentExportDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // CourseInstructor 
        CreateMap<CourseInstructor, CourseInstructorCreateDto>().ReverseMap();
        CreateMap<CourseInstructor, CourseInstructorEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<CourseInstructorEntreLaunchdateDto, CourseInstructor>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<CourseInstructor, CourseInstructorDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<CourseInstructor, CourseInstructorExportDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // CourseRating 
        CreateMap<CourseRating, CourseRatingCreateDto>().ReverseMap();
        CreateMap<CourseRating, CourseRatingEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<CourseRatingEntreLaunchdateDto, CourseRating>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<CourseRating, CourseRatingDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Exam 
        CreateMap<Exam, ExamCreateDto>().ReverseMap();
        CreateMap<Exam, ExamEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<ExamEntreLaunchdateDto, Exam>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Exam, ExamDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Exam, ExamExportDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<ExamImportDto, Exam>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Question
        CreateMap<Question, QuestionCreateDto>().ReverseMap();
        CreateMap<Question, QuestionEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<QuestionEntreLaunchdateDto, Question>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Question, QuestionDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Question, QuestionExportDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Lesson
        CreateMap<Lesson, LessonCreateDto>().ReverseMap();
        CreateMap<Lesson, LessonEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<LessonEntreLaunchdateDto, Lesson>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Lesson, LessonDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Lesson, LessonExportDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<LessonImportDto, Lesson>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Answer
        CreateMap<Answer, AnswerCreateDto>().ReverseMap();
        CreateMap<Answer, AnswerEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<AnswerEntreLaunchdateDto, Answer>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Answer, AnswerDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Answer, AnswerExportDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<AnswerImportDto, Answer>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // LessonAttachment
        CreateMap<LessonAttachment, LessonAttachmentCreateDto>().ReverseMap();
        CreateMap<LessonAttachment, LessonAttachmentEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<LessonAttachmentEntreLaunchdateDto, LessonAttachment>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<LessonAttachment, LessonAttachmentDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<LessonAttachmentImportDto, LessonAttachment>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Progress
        CreateMap<Progress, ProgressCreateDto>().ReverseMap();
        CreateMap<Progress, ProgressDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Result
        CreateMap<ExamResult, ExamResultCreateDto>().ReverseMap();
        CreateMap<ExamResult, ExamResultEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<ExamResultEntreLaunchdateDto, ExamResult>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<ExamResult, ExamResultDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<ExamResult, ExamResultExportDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        CreateMap<EmailGroup, EmailGroEntreLaunchExportDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // StudentCertificate
        CreateMap<StudentCertificate, StudentCertificateCreateDto>().ReverseMap();
        CreateMap<StudentCertificate, StudentCertificateEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<StudentCertificateEntreLaunchdateDto, StudentCertificate>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<StudentCertificate, StudentCertificateDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Consultation
        CreateMap<Consultation, ConsultationCreateDto>().ReverseMap();
        CreateMap<Consultation, ConsultationEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<ConsultationEntreLaunchdateDto, Consultation>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Consultation, ConsultationDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // ConsultationTime
        CreateMap<ConsultationTime, ConsultationTimeCreateDto>().ReverseMap();
        CreateMap<ConsultationTime, ConsultationTimeDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<ConsultationTime, ConsultationTimeEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<ConsultationTimeEntreLaunchdateDto, ConsultationTime>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<ConsultationTimeImportDto, ConsultationTime>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Counselor
        CreateMap<Counselor, CreateCounselorRequestDto>().ReverseMap();
        CreateMap<Counselor, CounselorRequestDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Ticket
        CreateMap<ConsultationTicket, TicketCreateDto>().ReverseMap();
        CreateMap<ConsultationTicket, TicketDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // TicketMessage
        CreateMap<ConsultationTicketMessage, TicketMessageCreateDto>().ReverseMap();
        CreateMap<ConsultationTicketMessage, TicketMessageEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<TicketMessageEntreLaunchdateDto, ConsultationTicketMessage>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<ConsultationTicketMessage, TicketMessageDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // TicketAttachment
        CreateMap<ConsultationTicketAttachment, TicketAttachmentCreateDto>().ReverseMap();
        CreateMap<ConsultationTicketAttachment, TicketAttachmentDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Post
        CreateMap<Post, PostWithMediaCreateDto>().ReverseMap();
        CreateMap<Post, TextPostCreateDto>().ReverseMap();
        CreateMap<Post, PostEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<PostEntreLaunchdateDto, Post>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Post, PostDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Comment
        CreateMap<PostComment, CommentCreateDto>().ReverseMap();
        CreateMap<PostComment, CommentEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<CommentEntreLaunchdateDto, PostComment>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<PostComment, CommentDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Media
        CreateMap<PostMedia, MediaCreateDto>().ReverseMap();
        CreateMap<PostMedia, MediaEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<MediaEntreLaunchdateDto, PostMedia>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
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
        CreateMap<MyPartner, MyPartnerEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<MyPartnerEntreLaunchdateDto, MyPartner>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<MyPartner, MyPartnerDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<MyPartnerAttachmentCreateDto, MyPartnerAttachment>();
        CreateMap<MyPartnerAttachment, ProjectAttachmentDetailsDto>();
        CreateMap<MyPartner, MyPartnerDetailsDto>().ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.ProjectAttachments)).ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<ProjectAttachmentEntreLaunchdateDto, MyPartnerAttachment>();


        // Employee
        CreateMap<Employee, EmployeeCreateDto>().ReverseMap();
        CreateMap<Employee, EmployeeEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<EmployeeEntreLaunchdateDto, Employee>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Employee, EmployeeDetailsDto>().ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Portfolios, opt => opt.MapFrom(src => src.Portfolios));

        // Employee Portfolio
        CreateMap<EmployeePortfolio, EmployeePortfolioCreateDto>().ReverseMap();
        CreateMap<EmployeePortfolio, EmployeePortfolioEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<EmployeePortfolioEntreLaunchdateDto, EmployeePortfolio>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<EmployeePortfolio, EmployeePortfolioDetailsDto>()
            .ForMember(dest => dest.PortfolioAttachments, opt => opt.MapFrom(src => src.PortfolioAttachments));

        // Portfolio Attachment
        CreateMap<PortfolioAttachment, PortfolioAttachmentCreateDto>().ReverseMap();
        CreateMap<PortfolioAttachment, PortfolioAttachmentEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<PortfolioAttachmentEntreLaunchdateDto, PortfolioAttachment>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<PortfolioAttachment, PortfolioAttachmentDetailsDto>();

        // My Opportunities
        CreateMap<Opportunity, OpportunityCreateDto>().ReverseMap();
        CreateMap<Opportunity, OpportunityEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<OpportunityEntreLaunchdateDto, Opportunity>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Opportunity, OpportunityDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Opportunity, OpportunityExportDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<OpportunityImportDto, Opportunity>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Investment opportunities request
        CreateMap<OpportunityRequest, CreateOpportunityRequestDto>().ReverseMap();
        CreateMap<OpportunityRequest, OpportunityRequestDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // ClubEvent
        CreateMap<ClubEvent, ClubEventCreateDto>().ReverseMap();
        CreateMap<ClubEvent, ClubEventEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<ClubEventEntreLaunchdateDto, ClubEvent>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<ClubEvent, ClubEventDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<ClubEvent, ClubEventExportDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<ClubEvent, ClubEventExportDto>();
        CreateMap<ClubEventImportDto, ClubEvent>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // WheelAward
        CreateMap<WheelAward, WheelAwardCreateDto>().ReverseMap();
        CreateMap<WheelAward, WheelAwardEntreLaunchdateDto>()
            .ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<WheelAwardEntreLaunchdateDto, WheelAward>()
            .ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<WheelAward, WheelAwardDetailsDto>()
            .ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<WheelAward, WheelAwardExportDto>()
            .ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<WheelAwardImportDto, WheelAward>()
            .ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // WheelPlayer
        CreateMap<WheelPlayer, WheelPlayerCreateDto>().ReverseMap();
        CreateMap<WheelPlayer, WheelPlayerEntreLaunchdateDto>()
            .ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<WheelPlayerEntreLaunchdateDto, WheelPlayer>()
            .ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<WheelPlayer, WheelPlayerDetailsDto>()
            .ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<WheelPlayer, WheelPlayerExportDto>()
            .ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Notification
        CreateMap<Notification, NotificationCreateDto>().ReverseMap();
        CreateMap<Notification, NotificationEntreLaunchdateDto>()
            .ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<NotificationEntreLaunchdateDto, Notification>()
            .ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Notification, NotificationDetailsDto>()
            .ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Notification, NotificationExportDto>()
            .ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Payment
        CreateMap<Payment, PaymentCreateDto>().ReverseMap();
        CreateMap<Payment, PaymentEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<PaymentEntreLaunchdateDto, Payment>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Payment, PaymentDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // Refund
        CreateMap<Refund, RefundCreateDto>().ReverseMap();
        CreateMap<Refund, RefundEntreLaunchdateDto>()
            .ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<RefundEntreLaunchdateDto, Refund>()
            .ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Refund, RefundDetailsDto>()
            .ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<Refund, RefundExportDto>()
            .ForAllMembers(m => m.Condition(PropertyNeedsMapping));

        // sms template
        CreateMap<SmsTemplate, SmsTemplateCreateDto>().ReverseMap();
        CreateMap<SmsTemplate, SmsTemplateEntreLaunchdateDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<SmsTemplateEntreLaunchdateDto, SmsTemplate>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<SmsTemplate, SmsTemplateDetailsDto>().ForAllMembers(m => m.Condition(PropertyNeedsMapping));
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
