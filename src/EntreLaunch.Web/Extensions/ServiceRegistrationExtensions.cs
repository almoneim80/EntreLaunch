using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using EntreLaunch.DTOs.LessonDtos;
using EntreLaunch.Interfaces.ConsultationsIntf;
using EntreLaunch.Services.BlogSvc;
using EntreLaunch.Services.ClubSvc;
using EntreLaunch.Services.FortuneWheelSvc;
using EntreLaunch.Services.MediaSvc;
using EntreLaunch.Services.PurchaseSvc;
using EntreLaunch.Services.StaticContentSvc;
using EntreLaunch.Services.SubscriptionSvc;

namespace EntreLaunch.Web.Extensions
{
    public static class ServiceRegistrationExtensions
    {
        public static void AddProjectServices(this IServiceCollection services, IConfiguration config)
        {
            // Scoped
            services.AddScoped<IPaymentGateway, PaytabsPaymentGateway>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IRefundService, RefundService>();
            services.AddScoped<ILoyaltyPointsService, LoyaltyPointsService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<IRatingsService, RatingsService>();
            services.AddScoped<IStudentProgress, StudentProgressService>();
            services.AddScoped<IMyOpportunityService, MyOpportunityService>();
            services.AddScoped<IOpportunityFilteringService, OpportunityFilteringService>();
            services.AddScoped<IOpportunityQueryService, OpportunityQueryService>();
            services.AddScoped<IOpportunityRequestService, OpportunityRequestService>();
            services.AddScoped<IMyFinancingService, MyFinancingService>();
            services.AddScoped<IMyPartnerService, MyPartnerService>();
            services.AddScoped<IMyPartnerFilteringService, MyPartnerFilteringService>();
            services.AddScoped<IMyPartnerProjectService, MyPartnerProjectService>();
            services.AddScoped<IMyPartnerAttachmentService, MyPartnerAttachmentService>();
            services.AddScoped<IMyTeamService, MyTeamService>();
            services.AddScoped<IConsultationBookingService, ConsultationBookingService>();
            services.AddScoped<ICounselorService, CounselorService>();
            services.AddScoped<ITicketService, TicketService>();
            services.AddScoped<IMyCommunityService, MyCommunityService>();
            services.AddScoped<ISimulationService, SimulationService>();
            services.AddScoped<IClubService, ClubService>();
            services.AddScoped<ICertificateService, CertificateService>();
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddScoped<IMultipleImportService<Lesson, LessonWithRelatedContent>, MultipleImportService<Lesson, LessonWithRelatedContent>>();
            services.AddScoped<IBlogService, BlogService>();
            services.AddScoped<IWheelPlayerService, WheelPlayerService>();
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddScoped<IPurchaseService, PurchaseService>();
            services.AddScoped<IStaticContentService, StaticContentService>();
            services.AddScoped<IEmailVerificationService, EmailVerificationService>();
            services.AddScoped<IEmailVerifyService, EmailVerifyService>();
            services.AddScoped<IEmailVerificationExtension, EmailVerificationExtensionService>();
            services.AddScoped<IAttachmentService, AttachmentService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<ISkillCourseService, SkillCourseService>();
            services.AddScoped<IOnlineCourseService, OnlineCourseService>();
            services.AddScoped<IPathCourseService, PathCourseService>();
            services.AddScoped<ICourseInstructorService, CourseInstructorService>();
            services.AddScoped<ILessonService, LessonService>();
            services.AddScoped<ITrainingPathService, TrainingPathService>();
            services.AddScoped<IExamService, ExamService>();
            services.AddScoped<IExtendedBaseService, ExtendedBaseService>();
            services.AddScoped<CascadeDeleteService>();
            services.AddScoped(typeof(BaseService<,,,>), typeof(BaseService<,,,>));
            services.AddScoped(typeof(IImportService<,>), typeof(ImportService<,>));
            services.AddScoped(typeof(BaseServiceWithoutUpdate<,,>), typeof(BaseServiceWithoutUpdate<,,>));
            services.AddScoped<IVariablesService, VariablesService>();
            services.AddScoped<IEmailValidationExternalService, EmailValidationExternalService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IExportService, ExportService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IExternalAuthService, GoogleAuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ICourseFieldService, CourseFieldService>();
            services.AddScoped<IUrlHelper>(factory =>
            {
                var actionContext = factory.GetRequiredService<IActionContextAccessor>().ActionContext!;
                return new UrlHelper(actionContext);
            });

            // Transient
            services.AddTransient<IMxVerifyService, MxVerifyService>();
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<IDomainService, DomainService>();
            services.AddTransient<IContactService, ContactService>();
            services.AddTransient(typeof(QueryProviderFactory<>), typeof(QueryProviderFactory<>));
            services.AddTransient(typeof(ESOnlyQueryProviderFactory<>), typeof(ESOnlyQueryProviderFactory<>));
            services.AddTransient<IEmailSchedulingService, EmailSchedulingService>();

            // Singleton
            services.AddSingleton<IpDetailsService, IpDetailsService>();
            services.AddSingleton<IHttpContextHelper, HttpContextHelper>();
            services.AddSingleton<ILockService, LockService>();
            services.AddSingleton<TaskStatusService, TaskStatusService>();
            services.AddSingleton<ActivityLogService, ActivityLogService>();
            services.AddSingleton<IEmailService, EmailService>();
            services.AddSingleton<ICacheService, MemoryCacheService>();
            //services.AddSingleton<IFileStorageService, AwsS3Service>();
            services.AddSingleton<IFileStorageService, AzureBlobStorageService>();
            services.AddSingleton<ILocalizationManager, LocalizationManager>();
            services.AddSingleton<IServerConfigurationManager, ServerConfigurationManager>();
            services.AddSingleton<EsDbContext>();
            services.AddSingleton<FileValidatorHelper>();
        }
    }
}
