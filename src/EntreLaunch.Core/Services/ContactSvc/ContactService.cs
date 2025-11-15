namespace EntreLaunch.Services.ContactSvc
{
    public class ContactService : IContactService
    {
        private readonly IDomainService domainService;
        private readonly IEmailSchedulingService emailSchedulingService;
        private readonly IOptions<ApiSettingsConfig> apiSettingsConfig;
        private PgDbContext pgDbContext;
        private readonly ILogger<ContactService> _logger;
        public ContactService(
            PgDbContext pgDbContext,
            IDomainService domainService,
            IEmailSchedulingService emailSchedulingService,
            IOptions<ApiSettingsConfig> apiSettingsConfig,
            ILogger<ContactService> logger)
        {
            this.pgDbContext = pgDbContext;
            this.domainService = domainService;
            this.apiSettingsConfig = apiSettingsConfig;
            this.emailSchedulingService = emailSchedulingService;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<GeneralResult<Contact>> FindOrCreate(string email, string language, int timezone)
        {
            try
            {
                var customer = pgDbContext.Contacts!.FirstOrDefault(c => c.Email == email);
                if (customer == null)
                {
                    customer = new Contact
                    {
                        Email = email,
                    };
                }

                customer.Timezone = timezone;
                customer.Language = language;
                await SaveAsync(customer);
                return new GeneralResult<Contact>(true, "Contact found or created", customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to find or create contact");
                return new GeneralResult<Contact>(false, "Error accurred while finding or creating contact", null);
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult> Subscribe(Contact contact, string groEntreLaunchName)
        {
            try
            {
                var language = contact.Language ?? apiSettingsConfig.Value.DefaultLanguage;
                var emailSchedule = await emailSchedulingService.FindByGroEntreLaunchAndLanguage(groEntreLaunchName, language);
                if (emailSchedule.Data == null)
                {
                    _logger.LogError("Email schedule not found");
                    return new GeneralResult(false, "Email schedule not found", null);
                }

                await pgDbContext.ContactEmailSchedules.AddAsync(new ContactEmailSchedule
                {
                    Contact = contact,
                    Schedule = emailSchedule.Data,
                    CreatedAt = DateTime.UtcNow,
                });

                _logger.LogInformation("Contact subscribed");
                return new GeneralResult(true, "Contact subscribed", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to subscribe contact");
                return new GeneralResult(false, "Error accurred while subscribing contact", null);
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult> Unsubscribe(string email, string reason, string source, DateTime createdAt, string? ip)
        {
            try
            {
                var contact = (from u in pgDbContext.Contacts where u.Email == email select u).FirstOrDefault();
                if (contact != null)
                {
                    var unsubscribe = new Unsubscribe
                    {
                        ContactId = contact.Id,
                        Reason = reason,
                        CreatedByIp = ip,
                        Source = source,
                        CreatedAt = createdAt,
                    };

                    await pgDbContext.Unsubscribes!.AddAsync(unsubscribe);
                    contact.Unsubscribe = unsubscribe;
                    var schedules = pgDbContext.ContactEmailSchedules!
                        .Include(c => c.Schedule)
                        .Include(c => c.Contact)
                        .Where(s => s.Status == ScheduleStatus.Pending && s.ContactId == contact.Id)
                        .ToList();

                    foreach (var schedule in schedules)
                    {
                        schedule.Status = ScheduleStatus.Unsubscribed;
                    }

                    _logger.LogInformation("Contact unsubscribed");
                    return new GeneralResult(true, "Contact unsubscribed", null);
                }
                else
                {
                    _logger.LogError("Contact not found");
                    return new GeneralResult(false, "Contact not found", null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to unsubscribe contact");
                return new GeneralResult(false, "Error accurred while unsubscribing contact", null);
            }
        }

        /// <inheritdoc/>
        public async Task SaveAsync(Contact contact)
        {
            await EnrichWithDomainId(contact);
            EnrichWithAccountId(contact);

            if (contact.Id > 0)
            {
                pgDbContext.Contacts!.EntreLaunchdate(contact);
            }
            else
            {
                await pgDbContext.Contacts!.AddAsync(contact);
            }
        }

        /// <inheritdoc/>
        public async Task SaveRangeAsync(List<Contact> contacts)
        {
            await EnrichWithDomainIdAsync(contacts);
            EnrichWithAccountId(contacts);

            var sortedContacts = contacts.GroEntreLaunchBy(c => c.Id > 0);

            foreach (var groEntreLaunch in sortedContacts)
            {
                if (groEntreLaunch.Key)
                {
                    pgDbContext.EntreLaunchdateRange(groEntreLaunch.ToList());
                }
                else
                {
                    await pgDbContext.AddRangeAsync(groEntreLaunch.ToList());
                }
            }
        }

        /// <inheritdoc/>
        public void SetDBContext(PgDbContext pgDbContext)
        {
            this.pgDbContext = pgDbContext;
            domainService.SetDBContext(pgDbContext);
            emailSchedulingService.SetDBContext(pgDbContext);
        }

        /*****************************PRIVATE METHODS************************************/

        /// <summary>
        /// Enrich contact with domain id and domain.
        /// </summary>
        private async Task EnrichWithDomainId(Contact contact)
        {
            var domainName = domainService.GetDomainNameByEmail(contact.Email);

            var domainsQueryResult = await pgDbContext!.Domains!.FirstOrDefaultAsync(domain => domain.Name == domainName);

            if (domainsQueryResult != null)
            {
                contact.DomainId = domainsQueryResult.Id;
                contact.Domain = domainsQueryResult;
            }
            else
            {
                contact.Domain = new Domain()
                {
                    Name = domainName,
                    AccountStatus = AccountSyncStatus.NotInitialized,
                };

                await domainService.SaveAsync(contact.Domain);
            }
        }

        /// <summary>
        /// Enrich contacts with domain id and domain.
        /// </summary>
        private async Task EnrichWithDomainIdAsync(List<Contact> contacts)
        {
            var newDomains = new Dictionary<string, Domain>();

            var contactsWithDomain = from contact in contacts
                                     select new
                                     {
                                         Contact = contact,
                                         DomainName = domainService.GetDomainNameByEmail(contact.Email),
                                     };

            try
            {
                var contactsWithDomainInfo = (from contactWithDomain in contactsWithDomain
                                              join domain in pgDbContext.Domains! on contactWithDomain.DomainName equals domain.Name into domainTemp
                                              from domain in domainTemp.DefaultIfEmpty()
                                              select new
                                              {
                                                  contactWithDomain.Contact,
                                                  contactWithDomain.DomainName,
                                                  Domain = domain,
                                                  DomainId = domain?.Id ?? 0,
                                              }).ToList();

                foreach (var contactWithDomainInfo in contactsWithDomainInfo)
                {
                    if (contactWithDomainInfo.DomainId != 0)
                    {
                        contactWithDomainInfo.Contact.DomainId = contactWithDomainInfo.DomainId;
                        contactWithDomainInfo.Contact.Domain = contactWithDomainInfo.Domain;
                    }
                    else
                    {
                        var existingDomain = from newDomain in newDomains
                                             where newDomain.Key == contactWithDomainInfo.DomainName
                                             select newDomain;

                        if (!existingDomain.Any())
                        {
                            var domain = new Domain()
                            {
                                Name = contactWithDomainInfo.DomainName,
                                Source = contactWithDomainInfo.Contact.Email,
                                AccountStatus = AccountSyncStatus.NotIntended,
                            };

                            newDomains.Add(domain.Name, domain);
                            await domainService.SaveAsync(domain);
                            contactWithDomainInfo.Contact.Domain = domain;
                        }
                        else
                        {
                            contactWithDomainInfo.Contact.Domain = existingDomain.FirstOrDefault().Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "error");
                throw;
            }
        }

        /// <summary>
        /// Enrich contacts with account id and domain.
        /// </summary>
        private void EnrichWithAccountId(List<Contact> contacts)
        {
            foreach (var contact in contacts)
            {
                var domain = contact.Domain;
                if (domain != null)
                {
                    contact.AccountId = domain.AccountId;
                }
            }
        }

        /// <summary>
        /// Enrich contact with account id and domain.
        /// </summary>
        private void EnrichWithAccountId(Contact contact)
        {
            var domain = contact.Domain;
            if (domain != null)
            {
                contact.AccountId = domain.AccountId;
            }
        }
    }
}
