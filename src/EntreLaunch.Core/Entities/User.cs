namespace EntreLaunch.Entities;
[SupportsElastic]
[SupportsChangeLog]
[Table("users")]
public class User : IdentityUser, ISharedData
{
    public string? AvatarUrl { get; set; } 
    
    public DateTimeOffset? DOB { get; set; }

    public DateTimeOffset? LastTimeLoggedIn { get; set; }

    [Searchable]
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public double? NationalId { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true; 

    [Searchable]
    public string? Specialization { get; set; }

    [Searchable]
    public Country CountryCode { get; set; }

    [NotMapped] 
    public override string? UserName { get => Email!; }

    [JsonIgnore]
    public DateTimeOffset? DeletedAt { get; set; }

    [JsonIgnore]
    public DateTimeOffset? SoftDeleteExpiration { get; set; }

    [JsonIgnore]
    public bool IsDeleted { get; set; } = false;

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; }

    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; }

    public string? AdditionalData { get; set; }

    // navigational properties
    public virtual ICollection<ClubSubscriber>? ClubEventSubscribers { get; set; }
    public virtual ICollection<Post>? CommunityPosts { get; set; }
    public virtual ICollection<PostComment>? PostComments { get; set; }
    public virtual ICollection<PostLike>? PostLikes { get; set; }
    public virtual ICollection<CommunityReport>? CommunityReports { get; set; }
    public virtual ICollection<WheelPlayer>? WheelPlayers { get; set; }
    public virtual ICollection<Simulation>? ProjectSimulations { get; set; }
    public virtual ICollection<MyPartner>? Projects { get; set; }
    public virtual ICollection<Employee>? TeamEmployees { get; set; }

    // consultations
    public virtual Counselor? Counselor { get; set; }
    public virtual ICollection<Consultation>? ConsultationsAsClient { get; set; }
    public virtual ICollection<ConsultationTicketMessage>? TicketMessages { get; set; }
    public virtual ICollection<ConsultationTicketAttachment>? TicketAttachments { get; set; }

    public virtual ICollection<Notification>? NotificationsSenders { get; set; }
    public virtual ICollection<Notification>? NotificationsRecivers { get; set; }
    public virtual ICollection<CourseRating>? CourseRatings { get; set; }
    public virtual ICollection<CourseInstructor>? CourseInstructors { get; set; }
    public virtual ICollection<CourseEnrollment>? CourseEnrollments { get; set; }
    public virtual ICollection<ExamResult>? ExamResults { get; set; }
    public virtual ICollection<StudentProgress>? StudentProgresses { get; set; }

    // refresh tokens
    public virtual ICollection<RefreshToken>? RefreshTokens { get; set; }


    // payments
    public virtual ICollection<Payment>? Payments { get; set; }
    public virtual ICollection<LoyaltyPoint>? LoyaltyPoints { get; set; }
}
