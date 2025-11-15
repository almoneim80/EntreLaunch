namespace EntreLaunch.Services.EmailSvc
{
    public class EmailVerifyService : IEmailVerifyService
    {
        private readonly PgDbContext pgContext;
        private readonly IDomainService domainService;
        private readonly IEmailValidationExternalService emailValidationExternalService;
        private readonly ILogger<EmailVerifyService> _logger;
        public EmailVerifyService(PgDbContext pgDbContext, IDomainService domainService, IEmailValidationExternalService emailValidationExternalService, ILogger<EmailVerifyService> logger)
        {
            pgContext = pgDbContext;
            this.domainService = domainService;
            this.emailValidationExternalService = emailValidationExternalService;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<GeneralResult<Domain>> Verify(string email)
        {
            try
            {
                // Extracting a domain name from an email
                var domainName = domainService.GetDomainNameByEmail(email);

                // Searching for a domain in the database
                var domain = await (from d in pgContext.Domains
                                    where d.Name == domainName
                                    select d).FirstOrDefaultAsync();

                if (domain != null && domain.DnsCheck is true)
                {
                    return new GeneralResult<Domain>(true, "Domain verified successfully", domain);
                }
                else
                {
                    // If the domain doesn't exist, create it
                    if (domain is null)
                    {
                        domain = new Domain() { Name = domainName, Source = email };
                        await domainService.SaveAsync(domain);
                    }

                    await domainService.Verify(domain!);
                    await VerifyDomain(email, domain);
                    await pgContext.SaveChangesAsync();

                    _logger.LogInformation("Domain verified successfully: {Domain}", email);
                    return new GeneralResult<Domain>(true, "Domain verified successfully", domain);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying domain: {Domain}", email);
                return new GeneralResult<Domain>(false, "Error verifying domain", null);
            }
        }


        /// <summary>
        /// Verify domain.
        /// </summary>
        private async Task VerifyDomain(string email, Domain domain)
        {
            var emailVerify = await emailValidationExternalService.Validate(email);
            if(emailVerify.Data != null)
            {
                domain.CatchAll = emailVerify.Data.CatchAllCheck;
            }
        }
    }
}
