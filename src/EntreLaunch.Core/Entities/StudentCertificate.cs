namespace EntreLaunch.Entities;

public class StudentCertificate : SharedData
{
    public int EnrollmentId { get; set; }
    public virtual CourseEnrollment Enrollment { get; set; } = null!;

    public int CourseId { get; set; }
    public virtual Course Course { get; set; } = null!;

    public string? Url { get; set; }
    public DateTimeOffset? IssuedAt { get; set; }
    public DeliveryMethod DeliveryMethod { get; set; } 
    public string? ShippingStatus { get; set; }
    public string? ShippingAddress { get; set; }

    public string CertificateId { get; set; } = Guid.NewGuid().ToString();
    public string? BarcodeUrl { get; set; }

    public DateTimeOffset? ExpirationDate { get; set; }
}
