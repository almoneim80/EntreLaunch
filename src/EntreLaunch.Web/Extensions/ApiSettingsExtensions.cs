namespace EntreLaunch.Web.Extensions
{
    public static class ApiSettingsExtensions
    {
        public static void AddApiSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<LocalizationSettings>(configuration.GetSection("LocalizationSettings"));
            services.Configure<DefaultRolesConfig>(configuration.GetSection("DefaultRoles"));
            services.Configure<SubscriptionSettings>(configuration.GetSection("SubscriptionSettings"));
            services.Configure<CacheSettings>(configuration.GetSection("CacheSettings"));
            services.Configure<EmailSenderOptions>(configuration.GetSection("EmailSender"));
            services.Configure<OtpVerificationOptions>(configuration.GetSection("OtpVerification"));
            services.Configure<PayTabsOptions>(configuration.GetSection("PayTabsSettings"));
            services.Configure<JwtConfig>(configuration.GetSection("Jwt"));
            services.Configure<AccountDetailsApiConfig>(configuration.GetSection("AccountDetailsApi"));
            services.Configure<ApiSettingsConfig>(configuration.GetSection("ApiSettings"));
            services.Configure<EmailVerificationApiConfig>(configuration.GetSection("EmailVerificationApi"));
            services.Configure<GeolocationApiConfig>(configuration.GetSection("GeolocationApi"));
            services.Configure<FileConfig>(configuration.GetSection("File"));
            services.Configure<MediaConfig>(configuration.GetSection("Media"));
            services.Configure<CorsConfig>(configuration.GetSection("Cors"));
            services.Configure<FileUploadSettings>(configuration.GetSection("FileUploadSettings"));

            services.AddSingleton(resolver =>
                resolver.GetRequiredService<IOptions<DefaultRolesConfig>>().Value);
        }
    }
}
