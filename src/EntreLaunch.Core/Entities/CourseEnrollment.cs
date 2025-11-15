namespace EntreLaunch.Entities;

public class CourseEnrollment : SharedData
{
    public int CourseId { get; set; }
    public virtual Course Course { get; set; } = null!;

    public string UserId { get; set; } = null!;
    public virtual User User { get; set; } = null!;

    public bool IsCompleted { get; set; } = false;
    public DateTimeOffset? EnrolledAt { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual ICollection<StudentCertificate>? StudentCertificates { get; set; }
}
