using EntreLaunch.DTOs.CertificateDtos;
using EntreLaunch.DTOs.SimulationDtos;

namespace EntreLaunch.DTOs.UserDtos;

public class UserCreateDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public double? NationalId { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
}

public class CompleteUserDetailsDto
{
    public string? AvatarUrl { get; set; }
    public DateTimeOffset DOB { get; set; }
    public string? Description { get; set; }
    public string? Specialization { get; set; }
    public Country? CountryCode { get; set; }
}

public class UserUpdateDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public double? NationalId { get; set; }
    public string? PhoneNumber { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTimeOffset DOB { get; set; }
    public string? Description { get; set; }
    public string? Specialization { get; set; }
    public Country? CountryCode { get; set; }
}

public class UserDetailsDto
{
    public string? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public double? NationalId { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTimeOffset? DOB { get; set; }
    public string? Description { get; set; }
    public string? Specialization { get; set; }
    public Country? CountryCode { get; set; }
}

public class OtpVerificationDto
{
    public string UserId { get; set; } = string.Empty;
    public string OtpCode { get; set; } = string.Empty;
}

public class OtpResendDto
{
    public string UserId { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}

// ALL USER DATA
public class UserFullProfileDto
{
#nullable disable
    // base user data
    public BaseUserData BaseData { get; set; }

    // courses data
    public List<CompletedPathData> completedPaths { get; set; }
    public List<IncompletePathData> IncompletePaths { get; set; }

    // courses data
    public List<CompletedCourseData> CompletedCourses { get; set; }
    public List<IncompleteCourseData> IncompleteCourses { get; set; }

    // certificate Data
    public List<CertificateDetailsDto> Certificates { get; set; }

    // consultations data if user has Enterpreneur role
    public List<ClientConsultationData> ClientConsultations { get; set; }

    //  consultations data if user has Counselor role
    public List<CounselorConsultations> CounselorConsultations { get; set; }
    public List<CounselorConsultationTimeData> ConsultationTimes { get; set; }

    // opportunities
    public List<InvestmentOpportunityData> InvestmentOpportunities { get; set; }

    // financing
    public List<FinancingOpportunityData> FinancingOpportunities { get; set; }

    // partners
    public List<MyPartnerData> MyPartners { get; set; }

    // teams
    //public List<MyTeamData> MyTeams { get; set; }

    // simulated projects
    public List<SimulationDetails> SimulatedProjects { get; set; }

    // joined club events
    public List<JoinedClubEventData> JoinedClubEvents { get; set; } = new();

    // progress
    public List<StudentProgressData> StudentProgress { get; set; }

    public List<UserBlog> Blogs { get; set; }

    public int loyaltyPoints { get; set; }

    public List<WheelGameHistory> wheelGameHistories { get; set; }

    public List<UserSubscriptionDto> UserSubscriptions { get; set; }
    public List<UserPurchaseDto> UserPurchases { get; set; }
}

public class BaseUserData
{
#nullable disable
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string AvatarUrl { get; set; }
    public DateTimeOffset DOB { get; set; }
    public bool IsActive { get; set; }
    public string Country { get; set; }
    public double NationalId { get; set; }
    public string Specialization { get; set; }
    public string Description { get; set; }
}

public class CompletedPathData
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public DateTimeOffset CompletionDate { get; set; }
}

public class IncompletePathData
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public DateTimeOffset CompletionDate { get; set; }
}

public class CompletedCourseData
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string PathName { get; set; } = null;
    public string FieldName { get; set; }
    public decimal Price { get; set; } = 0;
    public string StudyWay { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public DateTimeOffset CompletionDate { get; set; }
    public bool IsFree { get; set; }
    public CourseType? CourseType { get; set; }
}

public class IncompleteCourseData
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string PathName { get; set; } = null;
    public string FieldName { get; set; }
    public decimal Price { get; set; } = 0;
    public string StudyWay { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public DateTimeOffset CompletionDate { get; set; }
    public bool IsFree { get; set; }
    public CourseType? CourseType { get; set; }
}

public class StudentProgressData
{
    public string PathName { get; set; }
    public string CourseName { get; set; }
    public int LastLessonId { get; set; }
    public bool IsCompleted { get; set; }
    public double CompletionPercentage { get; set; }
}

public class ClientConsultationData
{
    public string CounselorName { get; set; }
    public DateTimeOffset? ConsultationDate { get; set; }
    public ConsultationType Type { get; set; }
    public ConsultationStatus Status { get; set; }
    public string Description { get; set; }
}

public class CounselorConsultations
{
    public string ClientName { get; set; }
    public DateTimeOffset? ConsultationDate { get; set; }
    public ConsultationType Type { get; set; }
    public ConsultationStatus Status { get; set; }
    public string Description { get; set; }
}

public class CounselorConsultationTimeData
{
    public DateTimeOffset? ConsultationDate { get; set; }
    public bool IsBooked { get; set; }
}

public class InvestmentOpportunityData
{
    public string City { get; set; }
    public double ShareCapital { get; set; }
    public decimal LoanRatio { get; set; }
    public int ManagementExperince { get; set; }
    public bool HaveFranchiseProjects { get; set; }
    public int FranchiseExperince { get; set; }
    public bool FeasibillityStudyBring { get; set; }
    public OpportunityRequestStatus Status { get; set; }

    public string CompanyName { get; set; }
    public string Description { get; set; }
    public string Sector { get; set; }
    public decimal Costs { get; set; }
    public int ContractDurationInDay { get; set; }
    public List<string> AcceptRequirements { get; set; }
    public Country? BrandCountry { get; set; }
}

public class FinancingOpportunityData
{
    public string City { get; set; }
    public double ShareCapital { get; set; }
    public decimal LoanRatio { get; set; }
    public int ManagementExperince { get; set; }
    public bool HaveFranchiseProjects { get; set; }
    public int FranchiseExperince { get; set; }
    public bool FeasibillityStudyBring { get; set; }
    public OpportunityRequestStatus Status { get; set; }

    public string CompanyName { get; set; }
    public string Description { get; set; }
    public string Sector { get; set; }
    public decimal Costs { get; set; }
    public int ContractDurationInDay { get; set; }
    public List<string> AcceptRequirements { get; set; }
    public Country? BrandCountry { get; set; }
}

public class MyPartnerData
{
    public string Activity { get; set; }
    public string City { get; set; }
    public string Sector { get; set; }
    public decimal Cost { get; set; }
    public string Idea { get; set; }
    public List<string> AcceptRequirements { get; set; }
    public decimal CapitalFrom { get; set; }
    public decimal CapitalTo { get; set; }
    public string Contact { get; set; }
    public MyPartnerStatus Status { get; set; }
}

public class MyTeamData
{
    public string WorkField { get; set; }
    public string JobTitle { get; set; }
    public string EmployeeDefinition { get; set; }
    public List<string> Skills { get; set; }
    public EmployeeStaus Status { get; set; }

    public string ProjectTitle { get; set; }
    public decimal CostFrom { get; set; }
    public decimal CostTo { get; set; }
    public string About { get; set; }
    public List<string> attachments { get; set; }
}

public class JoinedClubEventData
{
    public string Name { get; set; }
    public string City { get; set; }
    public string Description { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
}

public class UserBlog
{
    public string Title { get; set; }
    public string Details { get; set; }
    public string Media { get; set; }
    public BlogStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class WheelGameHistory
{
    public DateTimeOffset PlayedAt { get; set; }
    public string AwardName { get; set; }
    public AwardType AwardType { get; set; }
    public bool IsFree { get; set; }
}

public class UserSubscriptionDto
{
    public int Id { get; set; }
    public SubscriptionType Type { get; set; }
    public string ReferenceName { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public SubscriptionStatus Status { get; set; }
    public decimal Price { get; set; }
}

public class UserPurchaseDto
{
    public int Id { get; set; }
    public PurchaseItemType ItemType { get; set; }
    public string ReferenceName { get; set; } = string.Empty;
    public DateTimeOffset PurchaseDate { get; set; }
    public decimal Price { get; set; }
    public bool IsRefunded { get; set; }
}
