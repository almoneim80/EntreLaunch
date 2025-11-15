using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Migrations;
namespace EntreLaunch.Data
{
    public class PgDbContext : IdentityDbContext<User>
    {
        public readonly IConfiguration Configuration;
        private readonly IHttpContextHelper? httpContextHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="PgDbContext"/> class.
        /// Constructor with no parameters and manual configuration building is required for the case when you would like to use PgDbContext as a base class for a new context (let's say in a plugin).
        /// </summary>
        protected PgDbContext()
        {
            try
            {
                Console.WriteLine("Initializing PgDbContext...");
                Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", false)
                    .AddEnvironmentVariables().AddUserSecrets(typeof(PgDbContext).Assembly).Build();
                Console.WriteLine("PgDbContext initialized");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to create PgDbContext. Error: {0}, Stack Trace: {1}", ex.Message, ex.StackTrace);
                throw;
            }
        }

        public PgDbContext(DbContextOptions<PgDbContext> options, IConfiguration configuration, IHttpContextHelper httpContextHelper)
            : base(options)
        {
            Configuration = configuration;
            this.httpContextHelper = httpContextHelper;
        }

        public bool IsImportRequest { get; set; }

        // ************** Tables **************
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public virtual DbSet<Content> Content { get; set; } = null!;

        public virtual DbSet<Contact> Contacts { get; set; } = null!;

        public virtual DbSet<TaskExecutionLog> TaskExecutionLogs { get; set; } = null!;

        public virtual DbSet<Media> Media { get; set; } = null!;

        public virtual DbSet<Entities.File> Files { get; set; } = null!;

        public virtual DbSet<EmailGroup> EmailGroups { get; set; } = null!;

        public virtual DbSet<EmailSchedule> EmailSchedules { get; set; } = null!;

        public virtual DbSet<EmailTemplate> EmailTemplates { get; set; } = null!;

        public virtual DbSet<ContactEmailSchedule> ContactEmailSchedules { get; set; } = null!;

        public virtual DbSet<EmailLog> EmailLogs { get; set; } = null!;

        public virtual DbSet<IpDetails> IpDetails { get; set; } = null!;

        public virtual DbSet<ChangeLog> ChangeLogs { get; set; } = null!;

        public virtual DbSet<ChangeLogTaskLog> ChangeLogTaskLogs { get; set; } = null!;

        public virtual DbSet<Link> Links { get; set; } = null!;

        public virtual DbSet<LinkLog> LinkLogs { get; set; } = null!;

        public virtual DbSet<Domain> Domains { get; set; } = null!;

        public virtual DbSet<Account> Accounts { get; set; } = null!;

        public virtual DbSet<Unsubscribe> Unsubscribes { get; set; } = null!;

        public virtual DbSet<Promotion> Promotions { get; set; } = null!;

        public virtual DbSet<MailServer> MailServers { get; set; } = null!;

        public virtual DbSet<Course> Courses { get; set; } = null!;

        public virtual DbSet<CourseField> CourseFields { get; set; } = null!;

        public virtual DbSet<CourseEnrollment> CourseEnrollments { get; set; } = null!;

        public virtual DbSet<CourseInstructor> CourseInstructors { get; set; } = null!;

        public virtual DbSet<TrainingPath> TrainingPaths { get; set; } = null!;

        public virtual DbSet<CourseRating> CourseRatings { get; set; } = null!;

        public virtual DbSet<Exam> Exams { get; set; } = null!;

        public virtual DbSet<Lesson> Lessons { get; set; } = null!;

        public virtual DbSet<StudentProgress> StudentProgresses { get; set; } = null!;

        public virtual DbSet<LessonAttachment> LessonAttachments { get; set; } = null!;

        public virtual DbSet<Question> Questions { get; set; } = null!;

        public virtual DbSet<Answer> Answers { get; set; } = null!;

        public virtual DbSet<ExamResult> ExamResults { get; set; } = null!;

        public virtual DbSet<StudentCertificate> StudentCertificates { get; set; } = null!;

        public virtual DbSet<Progress> Progresses { get; set; } = null!;

        // Consultation Tables
        public virtual DbSet<Consultation> Consultations { get; set; } = null!;
        public virtual DbSet<Counselor> Counselors { get; set; } = null!;

        public virtual DbSet<ConsultationTime> ConsultationTimes { get; set; } = null!;

        public virtual DbSet<ConsultationTicket> ConsultationTickets { get; set; } = null!;

        public virtual DbSet<ConsultationTicketMessage> ConsultationTicketMessages { get; set; } = null!;

        public virtual DbSet<ConsultationTicketAttachment> ConsultationTicketAttachments { get; set; } = null!;

        // My Community Tables
        public virtual DbSet<Post> Posts { get; set; } = null!;

        public virtual DbSet<PostMedia> PostMedias { get; set; } = null!;

        public virtual DbSet<PostComment> PostComments { get; set; } = null!;

        public virtual DbSet<PostLike> PostLikes { get; set; } = null!;

        public virtual DbSet<CommunityReport> CommunityReports { get; set; } = null!;

        // Simulation Tables
        public virtual DbSet<Simulation> Simulations { get; set; } = null!;

        public virtual DbSet<SimulationBusinessPlan> SimulationBusinessPlans { get; set; } = null!;

        public virtual DbSet<SimulationIdeaPower> SimulationIdeaPowers { get; set; } = null!;

        public virtual DbSet<SimulationFeasibilityStudy> SimulationFeasibilityStudies { get; set; } = null!;

        public virtual DbSet<SimulationPurchase> SimulationPurchases { get; set; } = null!;

        public virtual DbSet<SimulationMarketing> SimulationMarketings { get; set; } = null!;

        public virtual DbSet<SimulationCampaign> SimulationCampaigns { get; set; } = null!;

        public virtual DbSet<SimulationAdvertisement> SimulationAdvertisements { get; set; } = null!;
        public virtual DbSet<SimulationAdLike> SimulationAdLikes { get; set; } = null!;
        public virtual DbSet<Guest> Guests { get; set; } = null!;

        // user project tables
        public virtual DbSet<MyPartner> MyPartners { get; set; } = null!;

        public virtual DbSet<MyPartnerAttachment> MyPartnerAttachments { get; set; } = null!;

        // My Team Tables
        public virtual DbSet<Employee> Employees { get; set; } = null!;

        public virtual DbSet<EmployeePortfolio> EmployeePortfolios { get; set; } = null!;

        public virtual DbSet<PortfolioAttachment> PortfolioAttachments { get; set; } = null!;

        // Opportunities table
        public virtual DbSet<Opportunity> Opportunities { get; set; } = null!;
        public virtual DbSet<OpportunityRequest> OpportunityRequests { get; set; } = null!;

        // Club tables
        public virtual DbSet<ClubEvent> ClubEvents { get; set; } = null!;
        public virtual DbSet<ClubSubscriber> ClubSubscribers { get; set; } = null!;

        // Wheel Game tables
        public virtual DbSet<WheelPlayer> WheelPlayers { get; set; } = null!;
        public virtual DbSet<WheelAward> WheelAwards { get; set; } = null!;

        // Notification 
        public virtual DbSet<Notification> Notifications { get; set; } = null!;

        // Pamyment tables
        public virtual DbSet<Payment> Payments { get; set; } = null!;
        public virtual DbSet<Refund> Refunds { get; set; } = null!;
        public virtual DbSet<PaymentGateway> PaymentGateways { get; set; } = null!;
        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; } = null!;
        public virtual DbSet<PaymentTransaction> PaymentTransactions { get; set; } = null!;
        public virtual DbSet<LoyaltyPoint> LoyaltyPoints { get; set; } = null!;

        public DbSet<ImapAccount> ImapAccounts { get; set; } = null!;
        public DbSet<ImapAccountFolder> ImapAccountFolders { get; set; } = null!;

        // sms tables
        public DbSet<SmsLog> SmsLogs { get; set; } = null!;
        public DbSet<SmsTemplate> SmsTemplates { get; set; } = null!;

        //tag tables
        public DbSet<Tag> Tags { get; set; } = null!;
        public DbSet<CourseTag> CourseTags { get; set; } = null!;

        // ************ End Tables *******************

        /// <summary>
        /// Save changes.
        /// </summary>
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            var result = 0;
            var changes = new Dictionary<EntityEntry, ChangeLog>();

            var entries = ChangeTracker
               .Entries()
               .Where(e => e.Entity is BaseEntityWithId && (
                       e.State == EntityState.Added
                       || e.State == EntityState.Modified
                       || e.State == EntityState.Deleted));

            var currentUserId = await httpContextHelper!.GetCurrentUserIdAsync();
            var userIpAddress = httpContextHelper!.IpAddressV4;
            var userAgent = httpContextHelper!.UserAgent;

            if (entries.Any())
            {
                foreach (var entityEntry in entries)
                {
                    if (entityEntry.State == EntityState.Added)
                    {
                        var createdAtEntity = entityEntry.Entity as IHasCreatedAt;

                        if (createdAtEntity is not null)
                        {
                            createdAtEntity.CreatedAt = createdAtEntity.CreatedAt == DateTime.MinValue ? DateTime.UtcNow : GetDateWithKind(createdAtEntity.CreatedAt);
                        }

                        var createdByEntity = entityEntry.Entity as IHasCreatedBy;

                        if (createdByEntity is not null)
                        {
                            createdByEntity.CreatedById = currentUserId;
                            createdByEntity.CreatedByIp = string.IsNullOrEmpty(createdByEntity.CreatedByIp) ? userIpAddress : createdByEntity.CreatedByIp;
                            createdByEntity.CreatedByUserAgent = string.IsNullOrEmpty(createdByEntity.CreatedByUserAgent) ? userAgent : createdByEntity.CreatedByUserAgent;
                        }
                    }

                    if (entityEntry.State == EntityState.Modified)
                    {
                        var updatedAtEntity = entityEntry.Entity as IHasUpdatedAt;

                        if (updatedAtEntity is not null)
                        {
                            updatedAtEntity.UpdatedAt = IsImportRequest && updatedAtEntity.UpdatedAt.HasValue
                                ? updatedAtEntity.UpdatedAt.Value.ToUniversalTime()
                                : DateTime.UtcNow;
                        }

                        var updatedByEntity = entityEntry.Entity as IHasUpdatedBy;

                        if (updatedByEntity is not null)
                        {
                            updatedByEntity.UpdatedById = currentUserId;
                            updatedByEntity.UpdatedByIp = IsImportRequest && !string.IsNullOrEmpty(updatedByEntity.UpdatedByIp) ? updatedByEntity.UpdatedByIp : userIpAddress;
                            updatedByEntity.UpdatedByUserAgent = IsImportRequest && !string.IsNullOrEmpty(updatedByEntity.UpdatedByUserAgent) ? updatedByEntity.UpdatedByUserAgent : userAgent;
                        }
                    }

                    var entityType = entityEntry.Entity.GetType();

                    if (entityType!.GetCustomAttributes<SupportsChangeLogAttribute>().Any())
                    {
                        // save entity state as it is before SaveChanges call
                        changes[entityEntry] = new ChangeLog
                        {
                            ObjectType = entityEntry.Entity.GetType().Name,
                            EntityState = entityEntry.State,
                            CreatedAt = DateTime.UtcNow,
                        };
                    }
                }
            }

            if (changes.Count > 0)
            {
                // save original records and obtain ids (to preserve ids in change_log)
                result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

                foreach (var change in changes)
                {
                    // save object id which we only recieve after SaveChanges (for new records)
                    change.Value.ObjectId = ((BaseEntityWithId)change.Key.Entity).Id;
                    change.Value.Data = JsonHelper.Serialize(change.Key.Entity);
                }

                ChangeLogs!.AddRange(changes.Values);
            }

            return result + await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        /// <summary>
        /// Configuring PgDbContext.
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                Console.WriteLine("Configuring PgDbContext...");

                var postgresConfig = Configuration.GetSection("Postgres").Get<PostgresConfig>();

                if (postgresConfig == null)
                {
                    throw new MissingConfigurationException("Postgres configuration is mandatory.");
                }

                optionsBuilder.UseNpgsql(
                    postgresConfig.ConnectionString,
                    b => b.MigrationsHistoryTable("_migrations")
                          .MigrationsAssembly("Up.Web"))
                    .UseSnakeCaseNamingConvention()
                    .ReplaceService<IMigrationsSqlGenerator, CustomSqlServerMigrationsSqlGenerator>();

                Console.WriteLine("PgDbContext successfully configured");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to configure PgDbContext. Error: {0}, Stack Trace: {1}", ex.Message, ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Configuring PgDbContext.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // all deleted records should be hidden
            //builder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);

            // Override default AspNet Identity table names
            builder.Entity<User>(entity => { entity.ToTable(name: "users"); });
            builder.Entity<IdentityRole>(entity => { entity.ToTable(name: "roles"); });
            builder.Entity<IdentityUserRole<string>>(entity => { entity.ToTable("user_roles"); });
            builder.Entity<IdentityUserClaim<string>>(entity => { entity.ToTable("user_claims"); });
            builder.Entity<IdentityUserLogin<string>>(entity => { entity.ToTable("user_logins"); });
            builder.Entity<IdentityUserToken<string>>(entity => { entity.ToTable("user_tokens"); });
            builder.Entity<IdentityRoleClaim<string>>(entity => { entity.ToTable("role_claims"); });

            // Community Post Indexes
            builder.Entity<Post>().HasIndex(p => p.UserId);
            builder.Entity<PostMedia>().HasIndex(m => m.PostId);
            builder.Entity<PostComment>().HasIndex(c => new { c.PostId, c.ParentCommentId });

            // ProjectSimulation
            builder.Entity<Simulation>().HasIndex(p => p.UserId);
            builder.Entity<Simulation>().HasIndex(p => p.ProjectStatus);
            builder.Entity<Simulation>().HasIndex(p => p.IsDeleted).HasFilter("is_deleted = false");

            // ProjectBusinessPlan
            builder.Entity<SimulationBusinessPlan>().Property(b => b.BusinessPartners);
            builder.Entity<SimulationBusinessPlan>().Property(b => b.ProjectActivities);
            builder.Entity<SimulationBusinessPlan>().Property(b => b.ValueProposition);
            builder.Entity<SimulationBusinessPlan>().Property(b => b.CustomerRelationships);
            builder.Entity<SimulationBusinessPlan>().Property(b => b.CustomerSegments);
            builder.Entity<SimulationBusinessPlan>().Property(b => b.RequiredResources);
            builder.Entity<SimulationBusinessPlan>().Property(b => b.DistributionChannels);
            builder.Entity<SimulationBusinessPlan>().Property(b => b.RevenueStreams);
            builder.Entity<SimulationBusinessPlan>().Property(b => b.CostStructure);

            builder.Entity<SimulationBusinessPlan>().HasIndex(p => p.SimulationId);
            builder.Entity<Simulation>().HasIndex(p => p.IsDeleted).HasFilter("is_deleted = false");

            // StrengthCategory
            builder.Entity<SimulationIdeaPower>().HasIndex(p => p.SimulationId);
            builder.Entity<SimulationIdeaPower>().HasIndex(p => p.CategoryType);
            builder.Entity<Simulation>().HasIndex(p => p.IsDeleted).HasFilter("is_deleted = false");

            // ProjectFeasibilityStudy
            builder.Entity<SimulationFeasibilityStudy>().HasIndex(p => p.SimulationId);
            builder.Entity<SimulationFeasibilityStudy>().HasIndex(p => p.IsDeleted).HasFilter("is_deleted = false");

            // ProjectPurchase
            builder.Entity<SimulationPurchase>().HasIndex(p => p.SimulationId);
            builder.Entity<SimulationPurchase>().HasIndex(p => p.IsDeleted).HasFilter("is_deleted = false");

            // ProjectMarketing
            builder.Entity<SimulationMarketing>().HasIndex(p => p.SimulationId);
            builder.Entity<SimulationMarketing>().HasIndex(p => p.ProductName);
            builder.Entity<SimulationMarketing>().HasIndex(p => p.IsDeleted).HasFilter("is_deleted = false");

            // ProjectMarketingCampaign
            builder.Entity<SimulationCampaign>().HasIndex(p => p.SimulationId);
            builder.Entity<SimulationCampaign>().HasIndex(p => p.Name);
            builder.Entity<SimulationCampaign>().HasIndex(p => p.IsDeleted).HasFilter("is_deleted = false");

            // ProjectProductAd
            builder.Entity<SimulationAdvertisement>().HasIndex(p => p.MarketingId);
            builder.Entity<SimulationAdvertisement>().HasIndex(p => p.IsDeleted).HasFilter("is_deleted = false");

            // Project
            builder.Entity<MyPartner>().HasIndex(p => p.Activity);
            builder.Entity<MyPartner>().HasIndex(p => p.City);
            builder.Entity<MyPartner>().HasIndex(p => p.CapitalFrom);
            builder.Entity<MyPartner>().HasIndex(p => p.CapitalTo);
            builder.Entity<MyPartner>().Property(b => b.AcceptRequirements);
            builder.Entity<MyPartner>().HasIndex(p => p.IsDeleted).HasFilter("is_deleted = false");

            // ProjectAttachment
            builder.Entity<MyPartnerAttachment>().HasIndex(p => p.ProjectId);
            builder.Entity<MyPartnerAttachment>().HasIndex(p => p.IsDeleted).HasFilter("is_deleted = false");

            // TeamEmployee
            builder.Entity<Employee>().HasIndex(p => p.WorkField);
            builder.Entity<Employee>().HasIndex(p => p.JobTitle);
            builder.Entity<Employee>().HasIndex(p => p.IsDeleted).HasFilter("is_deleted = false");

            // Portfolio
            builder.Entity<EmployeePortfolio>().HasIndex(p => p.ProjectTitle);
            builder.Entity<EmployeePortfolio>().HasIndex(p => p.IsDeleted).HasFilter("is_deleted = false");

            // PortfolioAttachment
            builder.Entity<PortfolioAttachment>().HasIndex(p => p.PortfolioId);
            builder.Entity<PortfolioAttachment>().HasIndex(p => p.IsDeleted).HasFilter("is_deleted = false");

            // Opportunity
            builder.Entity<Opportunity>().HasIndex(p => p.Sector);
            builder.Entity<Opportunity>().HasIndex(p => p.Costs);
            builder.Entity<Opportunity>().HasIndex(p => p.BrandCountry);
            builder.Entity<Opportunity>().HasIndex(p => p.IsDeleted).HasFilter("is_deleted = false");

            // ClubEvent
            builder.Entity<ClubEvent>().HasIndex(p => p.City);
            builder.Entity<ClubEvent>().HasIndex(p => p.Name);
            builder.Entity<ClubEvent>().HasIndex(p => p.IsDeleted).HasFilter("is_deleted = false");

            // ClubEventSubscriber
            builder.Entity<ClubSubscriber>().HasIndex(p => p.UserId);
            builder.Entity<ClubSubscriber>().HasIndex(p => p.EventId);
            builder.Entity<ClubSubscriber>().HasIndex(p => p.UserId);
            builder.Entity<ClubSubscriber>().HasIndex(p => p.IsDeleted).HasFilter("is_deleted = false");

            // WheelAward
            builder.Entity<WheelAward>().HasIndex(p => p.Name);
            builder.Entity<WheelAward>().HasIndex(p => p.Probability);
            builder.Entity<WheelAward>().HasIndex(p => p.IsDeleted).HasFilter("is_deleted = false");

            // WheelAward
            builder.Entity<WheelPlayer>().HasIndex(p => p.IsFree);
            builder.Entity<WheelPlayer>().HasIndex(p => p.IsDeleted).HasFilter("is_deleted = false");

            // Notification
            builder.Entity<Notification>().HasIndex(n => n.ReciverId);
            builder.Entity<Notification>().HasIndex(n => n.SenderId);
            builder.Entity<Notification>().HasIndex(n => n.IsRead);
            builder.Entity<Notification>().HasIndex(p => p.IsDeleted).HasFilter("is_deleted = false");

            // payment
            builder.Entity<Payment>().HasIndex(p => new { p.PaymentPurpose, p.TargetType, p.TargetId });
            builder.Entity<Payment>().HasIndex(p => p.UserId);
            builder.Entity<Payment>().HasIndex(p => p.Status);
            builder.Entity<Payment>().HasIndex(p => p.IsDeleted).HasFilter("is_deleted = false");

            // Refund
            builder.Entity<Refund>().HasIndex(r => r.PaymentId);
            builder.Entity<Refund>().HasIndex(r => r.Status);
            builder.Entity<Refund>().HasIndex(p => p.IsDeleted).HasFilter("is_deleted = false");

            // ************ Relationships ************

            builder.Entity<Contact>()
            .HasOne(c => c.Unsubscribe)
            .WithOne(u => u.Contact)
            .HasForeignKey<Contact>(c => c.UnsubscribeId)
            .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Unsubscribe>()
                .HasOne(u => u.Contact)
                .WithOne(c => c.Unsubscribe)
                .HasForeignKey<Unsubscribe>(u => u.ContactId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ClubSubscriber>()
                .HasOne(s => s.Event)
                .WithMany(e => e.ClubEventSubscribers)
                .HasForeignKey(s => s.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ClubSubscriber>()
                .HasOne(s => s.User)
                .WithMany(e => e.ClubEventSubscribers)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // CommunityPost relations
            builder.Entity<Post>()
                .HasOne(c => c.User)
                .WithMany(u => u.CommunityPosts)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Post>()
                .HasMany(c => c.PostComments)
                .WithOne(pc => pc.Post)
                .HasForeignKey(pc => pc.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Post>()
                .HasMany(c => c.PostLikes)
                .WithOne(pl => pl.Post)
                .HasForeignKey(pl => pl.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Post>()
                .HasMany(c => c.PostMedias)
                .WithOne(pm => pm.Post)
                .HasForeignKey(pm => pm.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Post>()
                .HasMany(c => c.CommunityReports)
                .WithOne(pr => pr.Post)
                .HasForeignKey(pr => pr.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            // PostComment relations
            builder.Entity<PostComment>()
                .HasOne(pc => pc.User)
                .WithMany(u => u.PostComments)
                .HasForeignKey(pc => pc.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PostComment>()
                .HasOne(pc => pc.Post)
                .WithMany(p => p.PostComments)
                .HasForeignKey(pc => pc.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PostComment>()
                .HasMany(pc => pc.CommunityReports)
                .WithOne(pr => pr.Comment)
                .HasForeignKey(pr => pr.CommentId)
                .OnDelete(DeleteBehavior.Restrict);

            // PostLike relations
            builder.Entity<PostLike>()
                .HasOne(pl => pl.User)
                .WithMany(u => u.PostLikes)
                .HasForeignKey(pl => pl.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PostLike>()
                .HasOne(pl => pl.Post)
                .WithMany(p => p.PostLikes)
                .HasForeignKey(pl => pl.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            // PostMedia relations
            builder.Entity<PostMedia>()
                .HasOne(pm => pm.Post)
                .WithMany(p => p.PostMedias)
                .HasForeignKey(pm => pm.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            // PostReport relations
            builder.Entity<CommunityReport>()
                .HasOne(pr => pr.User)
                .WithMany(u => u.CommunityReports)
                .HasForeignKey(pr => pr.userId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CommunityReport>()
                .HasOne(pr => pr.Post)
                .WithMany(p => p.CommunityReports)
                .HasForeignKey(pr => pr.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CommunityReport>()
                .HasOne(pr => pr.Comment)
                .WithMany()
                .HasForeignKey(pr => pr.CommentId)
                .OnDelete(DeleteBehavior.Restrict);

            // User relations

            // modelBuilder.Entity<User>()
            //    .HasKey(u => u.Id);

            // modelBuilder.Entity<User>()
            //    .Property(u => u.FullName)
            //    .HasMaxLength(200);

            // modelBuilder.Entity<User>()
            //    .Property(u => u.CountryCode)
            //    .IsRequired();

            builder.Entity<User>()
                .HasMany(u => u.WheelPlayers)
                .WithOne(wp => wp.Player)
                .HasForeignKey(wp => wp.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<User>()
                .HasMany(u => u.ProjectSimulations)
                .WithOne(ps => ps.User)
                .HasForeignKey(ps => ps.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // WheelPlayer relations
            builder.Entity<WheelPlayer>()
                .HasOne(wp => wp.Player)
                .WithMany(u => u.WheelPlayers)
                .HasForeignKey(wp => wp.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<WheelPlayer>()
                .HasOne(wp => wp.Award)
                .WithMany(wa => wa.WheelPlayers)
                .HasForeignKey(wp => wp.AwardId)
                .OnDelete(DeleteBehavior.Restrict);

            // WheelAward relations
            builder.Entity<WheelAward>()
                .HasMany(wa => wa.WheelPlayers)
                .WithOne(wp => wp.Award)
                .HasForeignKey(wp => wp.AwardId)
                .OnDelete(DeleteBehavior.Restrict);

            // ProjectSimulation Relations
            builder.Entity<Simulation>()
                .HasMany(ps => ps.SimulationIdeaPowers)
                .WithOne(pis => pis.Simulation)
                .HasForeignKey(pis => pis.SimulationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Simulation>()
                .HasMany(ps => ps.SimulationBusinessPlans)
                .WithOne(pbp => pbp.Simulation)
                .HasForeignKey(pbp => pbp.SimulationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Simulation>()
                .HasMany(ps => ps.SimulationPurchases)
                .WithOne(pp => pp.Simulation)
                .HasForeignKey(pp => pp.SimulationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Simulation>()
                .HasMany(ps => ps.SimulationMarketings)
                .WithOne(pm => pm.Simulation)
                .HasForeignKey(pm => pm.SimulationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Simulation>()
                .HasMany(ps => ps.SimulationCampaigns)
                .WithOne(pmc => pmc.Simulation)
                .HasForeignKey(pmc => pmc.SimulationId)
                .OnDelete(DeleteBehavior.Restrict);

            // ProjectIdeaStrength Relations
            builder.Entity<SimulationIdeaPower>()
                .HasOne(pis => pis.Simulation)
                .WithMany(ps => ps.SimulationIdeaPowers)
                .HasForeignKey(pis => pis.SimulationId)
                .OnDelete(DeleteBehavior.Restrict);

            // ProjectBusinessPlan Relations
            builder.Entity<SimulationBusinessPlan>()
                .HasOne(pbp => pbp.Simulation)
                .WithMany(pfs => pfs.SimulationBusinessPlans)
                .HasForeignKey(pfs => pfs.SimulationId)
                .OnDelete(DeleteBehavior.Restrict);

            // ProjectMarketing Relations
            builder.Entity<SimulationMarketing>()
                .HasOne(pm => pm.Simulation)
                .WithMany(ppa => ppa.SimulationMarketings)
                .HasForeignKey(ppa => ppa.SimulationId)
                .OnDelete(DeleteBehavior.Restrict);

            // ProjectProductAd Relations
            builder.Entity<SimulationAdvertisement>()
                .HasOne(pm => pm.Marketing)
                .WithMany(pm => pm.Advertisements)
                .HasForeignKey(ppa => ppa.MarketingId)
                .OnDelete(DeleteBehavior.Restrict);

            // Project Relationship
            builder.Entity<MyPartner>()
                .HasOne(p => p.User)
                .WithMany(u => u.Projects)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ProjectAttachment Relationship
            builder.Entity<MyPartnerAttachment>()
                .HasOne(pa => pa.Project)
                .WithMany(p => p.ProjectAttachments)
                .HasForeignKey(pa => pa.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // TeamEmployee Relationship
            builder.Entity<Employee>()
                .HasOne(te => te.User)
                .WithMany(u => u.TeamEmployees)
                .HasForeignKey(te => te.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Portfolio Relationship
            builder.Entity<EmployeePortfolio>()
                .HasOne(p => p.Employee)
                .WithMany(te => te.Portfolios)
                .HasForeignKey(p => p.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // PortfolioAttachment Relationship
            builder.Entity<PortfolioAttachment>()
                .HasOne(pa => pa.Portfolio)
                .WithMany(p => p.PortfolioAttachments)
                .HasForeignKey(pa => pa.PortfolioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Consultation
            // Counselor ←→ User
            builder.Entity<Counselor>()
                .HasOne(c => c.User)
                .WithOne(u => u.Counselor)
                .HasForeignKey<Counselor>(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Consultation ←→ Counselor
            builder.Entity<Consultation>()
                .HasOne(c => c.Counselor)
                .WithMany(co => co.Consultations)
                .HasForeignKey(c => c.CounselorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Consultation ←→ User (Client)
            builder.Entity<Consultation>()
                .HasOne(c => c.Client)
                .WithMany(u => u.ConsultationsAsClient)
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Consultation ←→ ConsultationTime
            builder.Entity<Consultation>()
                .HasOne(c => c.ConsultationTime)
                .WithMany(ct => ct.BookedConsultations)
                .HasForeignKey(c => c.ConsultationTimeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Consultation ←→ ConsultationTicket
            builder.Entity<Consultation>()
                .HasOne(c => c.Ticket)
                .WithOne(t => t.Consultation)
                .HasForeignKey<ConsultationTicket>(t => t.ConsultationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ConsultationTicket>()
                .HasIndex(t => t.ConsultationId)
                .IsUnique(true);

            // ConsultationTime ←→ Counselor
            builder.Entity<ConsultationTime>()
                .HasOne(ct => ct.Counselor)
                .WithMany(co => co.ConsultationTimes)
                .HasForeignKey(ct => ct.CounselorId)
                .OnDelete(DeleteBehavior.Restrict);

            // ConsultationTicket ←→ Counselor (Creator)
            builder.Entity<ConsultationTicket>()
                .HasOne(t => t.Creator)
                .WithMany(co => co.Tickets)
                .HasForeignKey(t => t.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            // TicketMessage ←→ ConsultationTicket
            builder.Entity<ConsultationTicketMessage>()
                .HasOne(tm => tm.Ticket)
                .WithMany(t => t.TicketMessages)
                .HasForeignKey(tm => tm.TicketId)
                .OnDelete(DeleteBehavior.Restrict);

            // TicketMessage ←→ User (Sender)
            builder.Entity<ConsultationTicketMessage>()
                .HasOne(tm => tm.Sender)
                .WithMany(u => u.TicketMessages)
                .HasForeignKey(tm => tm.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            // TicketAttachment ←→ ConsultationTicket
            builder.Entity<ConsultationTicketAttachment>()
                .HasOne(ta => ta.Ticket)
                .WithMany(t => t.TicketAttachments)
                .HasForeignKey(ta => ta.TicketId)
                .OnDelete(DeleteBehavior.Restrict);

            // TicketAttachment ←→ User (Sender)
            builder.Entity<ConsultationTicketAttachment>()
                .HasOne(ta => ta.Sender)
                .WithMany(u => u.TicketAttachments)
                .HasForeignKey(ta => ta.SenderId)
                .OnDelete(DeleteBehavior.Restrict);


            // Notification relationships
            builder.Entity<Notification>()
                .HasOne(n => n.Sender)
                .WithMany(u => u.NotificationsSenders)
                .HasForeignKey(n => n.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Notification>()
                .HasOne(n => n.Reciver)
                .WithMany(u => u.NotificationsRecivers)
                .HasForeignKey(n => n.ReciverId)
                .OnDelete(DeleteBehavior.Restrict);

            // TrainingPath and Course: One-to-Many Relationship
            builder.Entity<Course>()
                .HasOne(c => c.TrainingPath)
                .WithMany(tp => tp.Courses)
                .HasForeignKey(c => c.PathId)
                .OnDelete(DeleteBehavior.Restrict);

            // TrainingPath and CourseField: One-to-Many Relationship
            builder.Entity<Course>()
                .HasOne(c => c.CourseField)
                .WithMany(tp => tp.Courses)
                .HasForeignKey(c => c.FieldId)
                .OnDelete(DeleteBehavior.Restrict);

            // CourseRating relationships
            builder.Entity<CourseRating>()
                .HasOne(cr => cr.Course)
                .WithMany(c => c.CourseRatings)
                .HasForeignKey(cr => cr.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CourseRating>()
                .HasOne(cr => cr.User)
                .WithMany(u => u.CourseRatings)
                .HasForeignKey(cr => cr.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // CourseInstructor relationships
            builder.Entity<CourseInstructor>()
                .HasOne(ci => ci.Course)
                .WithMany(c => c.CourseInstructors)
                .HasForeignKey(ci => ci.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CourseInstructor>()
                .HasOne(ci => ci.User)
                .WithMany(u => u.CourseInstructors)
                .HasForeignKey(ci => ci.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // LessonAttachment relationships
            builder.Entity<LessonAttachment>()
                .HasOne(la => la.Lesson)
                .WithMany(l => l.LessonAttachments)
                .HasForeignKey(la => la.LessonId)
                .OnDelete(DeleteBehavior.Restrict);

            // ExamResult relationships
            builder.Entity<ExamResult>()
                .HasOne(er => er.Exam)
                .WithMany(e => e.ExamResults)
                .HasForeignKey(er => er.ExamId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ExamResult>()
                .HasOne(er => er.User)
                .WithMany(u => u.ExamResults)
                .HasForeignKey(er => er.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ----- Payment relationships ---------
            builder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PaymentTransaction>()
               .HasOne(pt => pt.Payment)
               .WithMany(p => p.PaymentTransactions)
               .HasForeignKey(pt => pt.PaymentId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Refund>()
               .HasOne(r => r.Payment)
               .WithMany(p => p.Refunds)
               .HasForeignKey(r => r.PaymentId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PaymentTransaction>()
               .HasOne(pt => pt.PaymentMethod)
               .WithMany(pg => pg.PaymentTransactions)
               .HasForeignKey(pt => pt.PaymentMethodId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<LoyaltyPoint>()
                   .HasOne(lpt => lpt.User)
                   .WithMany(u => u.LoyaltyPoints)
                   .HasForeignKey(lpt => lpt.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<LoyaltyPoint>()
                   .HasOne(lpt => lpt.Payment)
                   .WithMany(p => p.LoyaltyPoints)
                   .HasForeignKey(lpt => lpt.PaymentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CourseTag>()
                .HasOne(ct => ct.Course)
                .WithMany(c => c.CourseTags)
                .HasForeignKey(ct => ct.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CourseTag>()
                .HasOne(ct => ct.Tag)
                .WithMany(t => t.CourseTags)
                .HasForeignKey(ct => ct.TagId)
                .OnDelete(DeleteBehavior.Cascade);

            // The relationship between StudentProgress and User
            builder.Entity<StudentProgress>()
                .HasOne(sp => sp.User)
                .WithMany(u => u.StudentProgresses)
                .HasForeignKey(sp => sp.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // The relationship between StudentProgress and Course
            builder.Entity<StudentProgress>()
                .HasOne(sp => sp.Course)
                .WithMany(c => c.StudentProgresses)
                .HasForeignKey(sp => sp.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // The relationship between StudentProgress and Lesson
            builder.Entity<StudentProgress>()
                .HasOne(sp => sp.Lesson)
                .WithOne() // ont to one relationship
                .HasForeignKey<StudentProgress>(sp => sp.LastLessonId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        /// <summary>
        /// GetDateWithKind.
        /// </summary>
        private DateTime GetDateWithKind(DateTime date)
        {
            if (date.Kind == DateTimeKind.Unspecified /*|| date.Kind == DateTimeKind.Local*/)
            {
                return DateTime.SpecifyKind(date, DateTimeKind.Utc);
            }

            return date;
        }
    }
}
