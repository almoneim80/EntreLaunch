using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.CertificateDtos;
using EntreLaunch.DTOs.PaymentDtos;
using EntreLaunch.DTOs.SimulationDtos;
using EntreLaunch.DTOs.TrainingDtos;
using EntreLaunch.DTOs.UserDtos;
using EntreLaunch.Interfaces.PurchaseIntf;
using EntreLaunch.Interfaces.SubscriptionIntf;
namespace EntreLaunch.Services.AuthenticationSvc
{
    public class UserProfileService(
        PgDbContext dbContext,
        ILogger<UserProfileService> logger,
        ILocalizationManager localizationManager,
        ISubscriptionService subscriptionService,
        IPurchaseService purchaseService,
        ILoyaltyPointsService loyaltyPointsService) : IUserProfileService
    {
        private readonly PgDbContext _dbContext = dbContext;
        private readonly ILogger<UserProfileService> _logger = logger;
        private readonly ILocalizationManager _localizationManager = localizationManager;
        private readonly DateTimeOffset now = DateTimeOffset.UtcNow;

        public async Task<GeneralResult<UserFullProfileDto>> GetFullProfileAsync(string userId)
        {
            try
            {
                var defaultPagination = new PaginationParams { Page = 1, PageSize = 50 };
                var cancellationToken = CancellationToken.None;

                var user = await _dbContext.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

                if (user == null)
                {
                    return new GeneralResult<UserFullProfileDto>(false, _localizationManager.GetLocalizedString("UserNotFound"), null);
                }

                var fullProfile = new UserFullProfileDto
                {
                    BaseData = new BaseUserData
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email!,
                        PhoneNumber = user.PhoneNumber!,
                        AvatarUrl = user.AvatarUrl!,
                        DOB = user.DOB ?? now,
                        IsActive = user.IsActive,
                        Country = user.CountryCode.ToString(),
                        NationalId = user.NationalId ?? 0,
                        Specialization = user.Specialization!,
                        Description = user.Description!
                    },

                    completedPaths = await GetCompletedPathsAsync(userId),
                    IncompletePaths = await GetIncompletePathsAsync(userId),

                    CompletedCourses = await GetCompletedCoursesAsync(userId),
                    IncompleteCourses = await GetIncompleteCoursesAsync(userId),

                    Certificates = await GetCertificatesAsync(userId),

                    ClientConsultations = await GetClientConsultationsAsync(userId),
                    CounselorConsultations = await GetCounselorConsultationsAsync(userId),
                    ConsultationTimes = await GetCounselorConsultationTimesAsync(userId),

                    InvestmentOpportunities = await GetInvestmentOpportunitiesAsync(userId),
                    FinancingOpportunities = await GetFinancingOpportunitiesAsync(userId),

                    MyPartners = await GetMyPartnersAsync(userId),
                    //MyTeams = await GetMyTeamsAsync(userId),

                    SimulatedProjects = await GetSimulatedProjectsAsync(userId),

                    StudentProgress = await GetStudentProgressAsync(userId),

                    Blogs = await GetBlogsAsync(userId),

                    loyaltyPoints = await loyaltyPointsService.GetUserPointsAsync(userId),

                    wheelGameHistories = await GetWheelGameHistory(userId),

                    UserSubscriptions = await GetUserSubscriptionsAsync(userId, defaultPagination, cancellationToken),
                    UserPurchases = await GetUserPurchasesAsync(userId)
                };

                return new GeneralResult<UserFullProfileDto>(true, _localizationManager.GetLocalizedString("FullProfileRetrieved"), fullProfile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching user full profile for User {UserId}.", userId);
                return new GeneralResult<UserFullProfileDto>(false, _localizationManager.GetLocalizedString("ErrorFullUserData"), null);
            }
        }

        #region Helper Methods
        private async Task<List<CompletedPathData>> GetCompletedPathsAsync(string userId)
        {
            return await _dbContext.Progresses
                .Where(e => e.UserId == userId && e.IsCompleted && !e.IsDeleted)
                .Select(e => new CompletedPathData
                {
                    Name = e.Path != null ? e.Path.Name! : string.Empty,
                    Description = e.Path != null ? e.Path.Description! : string.Empty,
                    Price = e.Path != null ? e.Path.Price : 0,
                    CompletionDate = e.CreatedAt ?? now
                }).ToListAsync();
        }

        private async Task<List<IncompletePathData>> GetIncompletePathsAsync(string userId)
        {
            return await _dbContext.Progresses
                .Where(e => e.UserId == userId && !e.IsCompleted && !e.IsDeleted)
                .Select(e => new IncompletePathData
                {
                    Name = e.Path != null ? e.Path.Name! : string.Empty,
                    Description = e.Path != null ? e.Path.Description! : string.Empty,
                    Price = e.Path != null ? e.Path.Price : 0,
                    CompletionDate = e.CreatedAt ?? now
                }).ToListAsync();
        }

        private async Task<List<CompletedCourseData>> GetCompletedCoursesAsync(string userId)
        {
            return await _dbContext.StudentProgresses
                .Where(p => p.UserId == userId && p.IsCompleted && !p.IsDeleted && p.CourseId != null)
                .Select(p => new CompletedCourseData 
                {
                    Name = p.Course!.Name,
                    Description = p.Course.Description!,
                    PathName = p.Course.TrainingPath != null ? p.Course.TrainingPath.Name : string.Empty,
                    FieldName = p.Course.CourseField != null ? p.Course.CourseField.Name : string.Empty,
                    Price = p.Course.Price ?? 0,
                    StudyWay = p.Course.StudyWay ?? string.Empty,
                    StartDate = p.Course.StartDate ?? now,
                    EndDate = p.Course.EndDate ?? now,
                    CompletionDate = p.Course.CompletionDate ?? now,
                    IsFree = p.Course.IsFree,
                    CourseType = p.Course.Type ?? CourseType.Unknown,
                }).ToListAsync();
        }

        private async Task<List<IncompleteCourseData>> GetIncompleteCoursesAsync(string userId)
        {
            return await _dbContext.StudentProgresses
                .Where(p => p.UserId == userId && !p.IsCompleted && !p.IsDeleted && p.CourseId != null)
                .Select(p => new IncompleteCourseData
                {
                    Name = p.Course!.Name!,
                    Description = p.Course.Description!,
                    PathName = p.Course.TrainingPath != null ? p.Course.TrainingPath.Name : string.Empty,
                    FieldName = p.Course.CourseField != null ? p.Course.CourseField.Name : string.Empty,
                    Price = p.Course.Price ?? 0,
                    StudyWay = p.Course.StudyWay ?? string.Empty,
                    StartDate = p.Course.StartDate ?? now,
                    EndDate = p.Course.EndDate ?? now,
                    CompletionDate = p.Course.CompletionDate ?? now,
                    IsFree = p.Course.IsFree,
                    CourseType = p.Course.Type ?? CourseType.Unknown,
                }).ToListAsync();
        }

        private async Task<List<CertificateDetailsDto>> GetCertificatesAsync(string userId)
        {
            return await _dbContext.Certificates
                .Include(c => c.Course)
                .Include(c => c.Path)
                .Include(c => c.User)
                .Where(c => c.UserId == userId && !c.IsDeleted)
                .Select(c => new CertificateDetailsDto
                {
                    Id = c.Id,
                    CertificateFor = c.CertificateType == StudentCertificateType.Path
                        ? c.Path != null ? c.Path.Name : null
                        : c.Course != null ? c.Course.Name : null,
                    IssuedAt = c.IssuedAt ?? DateTimeOffset.UtcNow,
                    CertificateId = c.CertificateId,
                    ExpirationDate = c.ExpirationDate ?? DateTimeOffset.MinValue,
                    ShippingStatus = c.ShippingStatus,
                    ShippingAddress = c.ShippingAddress,
                    Student = c.User != null ? new StudentData
                    {
                        FirstName = c.User.FirstName,
                        LastName = c.User.LastName,
                        Email = c.User.Email,
                        NationalId = c.User.NationalId ?? 0,
                        PhoneNumber = c.User.PhoneNumber,
                        Specialization = c.User.Specialization
                    }
                    : null
                }).ToListAsync();
        }

        private async Task<List<ClientConsultationData>> GetClientConsultationsAsync(string userId)
        {
            return await _dbContext.Consultations
                .Where(c => c.ClientId == userId && !c.IsDeleted)
                .Select(c => new ClientConsultationData
                {
                    CounselorName = c.Counselor.User.FirstName + " " + c.Counselor.User.LastName,
                    ConsultationDate = c.ConsultationTime != null ? c.ConsultationTime.DateTimeSlot : null,
                    Type = c.Type,
                    Status = c.Status,
                    Description = c.Description!
                }).ToListAsync();
        }

        private async Task<List<CounselorConsultations>> GetCounselorConsultationsAsync(string userId)
        {
            return await _dbContext.Consultations
                .Where(c => c.Counselor.UserId == userId && !c.IsDeleted)
                .Select(c => new CounselorConsultations
                {
                    ClientName = c.Client.FirstName + " " + c.Client.LastName,
                    ConsultationDate = c.ConsultationTime != null ? c.ConsultationTime.DateTimeSlot : null,
                    Type = c.Type,
                    Status = c.Status,
                    Description = c.Description!
                }).ToListAsync();
        }

        private async Task<List<CounselorConsultationTimeData>> GetCounselorConsultationTimesAsync(string userId)
        {
            return await _dbContext.ConsultationTimes
                .Where(t => t.Counselor.UserId == userId && !t.IsDeleted)
                .Select(t => new CounselorConsultationTimeData
                {
                    ConsultationDate = t.DateTimeSlot,
                    IsBooked = t.IsBooked
                }).ToListAsync();
        }

        private async Task<List<InvestmentOpportunityData>> GetInvestmentOpportunitiesAsync(string userId)
        {
            return await _dbContext.OpportunityRequests
                .Where(o => o.UserId == userId && o.Type == OpportunityType.Investment && !o.IsDeleted)
                .Select(o => new InvestmentOpportunityData
                {
                    CompanyName = o.Opportunity.CompanyName,
                    City = o.City,
                    ShareCapital = o.ShareCapital ?? 0,
                    LoanRatio = o.LoanRatio ?? 0,
                    ManagementExperince = o.ManagementExperince,
                    HaveFranchiseProjects = o.HaveFranchiseProjects,
                    FranchiseExperince = o.FranchiseExperince,
                    FeasibillityStudyBring = o.FeasibillityStudyBring,
                    Status = o.Status,
                    Description = o.Opportunity.Description!,
                    Sector = o.Opportunity.Sector!,
                    Costs = o.Opportunity.Costs ?? 0,
                    ContractDurationInDay = o.Opportunity.ContractDurationInDay ?? 0,
                    AcceptRequirements = o.Opportunity.AcceptRequirements,
                    BrandCountry = o.Opportunity.BrandCountry
                }).ToListAsync();
        }

        private async Task<List<FinancingOpportunityData>> GetFinancingOpportunitiesAsync(string userId)
        {
            return await _dbContext.OpportunityRequests
                .Where(o => o.UserId == userId && o.Type == OpportunityType.Financing && !o.IsDeleted)
                .Select(o => new FinancingOpportunityData
                {
                    CompanyName = o.Opportunity.CompanyName,
                    City = o.City,
                    ShareCapital = o.ShareCapital ?? 0,
                    LoanRatio = o.LoanRatio ?? 0,
                    ManagementExperince = o.ManagementExperince,
                    HaveFranchiseProjects = o.HaveFranchiseProjects,
                    FranchiseExperince = o.FranchiseExperince,
                    FeasibillityStudyBring = o.FeasibillityStudyBring,
                    Status = o.Status,
                    Description = o.Opportunity.Description!,
                    Sector = o.Opportunity.Sector!,
                    Costs = o.Opportunity.Costs ?? 0,
                    ContractDurationInDay = o.Opportunity.ContractDurationInDay ?? 0,
                    AcceptRequirements = o.Opportunity.AcceptRequirements,
                    BrandCountry = o.Opportunity.BrandCountry
                }).ToListAsync();
        }

        private async Task<List<MyPartnerData>> GetMyPartnersAsync(string userId)
        {
            return await _dbContext.MyPartners
                .Where(p => p.UserId == userId && !p.IsDeleted)
                .Select(p => new MyPartnerData
                {
                    Activity = p.Activity!,
                    City = p.City!,
                    Sector = p.Sector!,
                    Cost = p.Cost ?? 0,
                    Idea = p.Idea!,
                    AcceptRequirements = p.AcceptRequirements,
                    CapitalFrom = p.CapitalFrom ?? 0,
                    CapitalTo = p.CapitalTo,
                    Contact = p.Contact!,
                    Status = p.Status
                }).ToListAsync();
        }

        private async Task<List<SimulationDetails>> GetSimulatedProjectsAsync(string userId)
        {
            return await _dbContext.Simulations
                .Where(s => s.UserId == userId && !s.IsDeleted)
                .Select(s => new SimulationDetails
                {
                    Id = s.Id,
                    ProjectField = s.ProjectField,
                    ProjectType = s.ProjectType,
                    ProjectStatus = s.ProjectStatus,
                    IdeaPowerhValue = s.IdeaPowerhValue ?? 0,
                    TotalCampaignValue = s.TotalCampaignValue ?? 0,
                }).ToListAsync();
        }

        private async Task<List<StudentProgressData>> GetStudentProgressAsync(string userId)
        {
            return await _dbContext.StudentProgresses
                .Where(p => p.UserId == userId && !p.IsDeleted)
                .Select(p => new StudentProgressData
                {
                    PathName = p.Path != null ? p.Path.Name : string.Empty,
                    CourseName = p.Course != null ? p.Course.Name : string.Empty,
                    LastLessonId = p.LastLessonId ?? 0,
                    IsCompleted = p.IsCompleted,
                    CompletionPercentage = p.CompletionPercentage
                }).ToListAsync();
        }

        private async Task<List<UserBlog>> GetBlogsAsync(string userId)
        {
            return await _dbContext.Blogs
                .Where(p => p.UserId == userId && !p.IsDeleted && p.Status == BlogStatus.Accepted)
                .Select(p => new UserBlog
                {
                    Title = p.Title,
                    Details = p.Details,
                    Media = p.Media,
                    Status = p.Status,
                    CreatedAt = p.CreatedAt ?? now,
                }).ToListAsync();
        }

        private async Task<List<WheelGameHistory>> GetWheelGameHistory(string userId)
        {
            return await _dbContext.WheelPlayers
                .Where(x => x.PlayerId == userId && !x.IsDeleted)
                .Include(x => x.Award)
                .Select(x => new WheelGameHistory
                {
                    PlayedAt = x.PlayedAt ?? now,
                    AwardName = x.Award != null ? x.Award.Name : "",
                    AwardType = x.Award != null ? x.Award.Type : AwardType.Unknown,
                    IsFree = x.IsFree
                }).ToListAsync();
        }

        private async Task<List<UserSubscriptionDto>> GetUserSubscriptionsAsync(string userId, PaginationParams pagination, CancellationToken cancellationToken)
        {
            var result = await subscriptionService.GetUserSubscriptionsAsync(userId, pagination, cancellationToken);

            if (!result.IsSuccess || result.Data == null)
                return [];

            return result.Data.Items.Select(sub => new UserSubscriptionDto
            {
                Id = sub.Id,
                Type = sub.Type,
                ReferenceName = sub.ReferenceName ?? string.Empty,
                StartDate = sub.StartDate,
                EndDate = sub.EndDate,
                Status = sub.Status,
                Price = sub.Price
            }).ToList();
        }

        private async Task<List<UserPurchaseDto>> GetUserPurchasesAsync(string userId)
        {
            var result = await purchaseService.GetUserPurchasesAsync(userId);

            if (!result.IsSuccess || result.Data == null)
                return [];

            var referenceNames = await GetReferenceNamesAsync(result.Data);

            return result.Data.Select(p => new UserPurchaseDto
            {
                Id = p.Id,
                ItemType = p.ItemType,
                ReferenceName = referenceNames.TryGetValue((p.ItemType, p.ReferenceId), out var name) ? name : "—",
                PurchaseDate = p.CreatedAt,
                Price = p.Price,
                IsRefunded = p.IsRefunded
            }).ToList();
        }

        private async Task<Dictionary<(PurchaseItemType, int), string>> GetReferenceNamesAsync(List<PurchaseDetailsDto> purchases)
        {
            var result = new Dictionary<(PurchaseItemType, int), string>();

            var skillCourseIds = purchases
                .Where(p => p.ItemType == PurchaseItemType.SkillsLibCourse)
                .Select(p => p.ReferenceId)
                .Distinct()
                .ToList();

            if (skillCourseIds.Any())
            {
                var names = await _dbContext.Courses
                    .Where(c => skillCourseIds.Contains(c.Id) && !c.IsDeleted)
                    .ToDictionaryAsync(c => c.Id, c => c.Name ?? "");

                foreach (var kv in names)
                    result[(PurchaseItemType.SkillsLibCourse, kv.Key)] = kv.Value;
            }

            return result;
        }
        #endregion
    }
}
