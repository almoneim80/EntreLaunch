namespace EntreLaunch.DTOs;

public class StudentCertificateCreateDto
{
    public int ExamId { get; set; }
    public string? UserId { get; set; }
}

public class StudentCertificateEntreLaunchdateDto
{
    public string? ShippingAddress { get; set; }
}

public class StudentCertificateDetailsDto : StudentCertificateCreateDto
{
    public int Id { get; set; }
}

public class StudentCertificateExportDto
{
    public string? ShippingAddress { get; set; }
}

public class StudentCertificateDto
{
    public int Id { get; set; }
    public int EnrollmentId { get; set; }
    public int CourseId { get; set; }
    public string? Url { get; set; }
    public DateTimeOffset? IssuedAt { get; set; }
    public DeliveryMethod DeliveryMethod { get; set; }
    public string? ShippingStatus { get; set; }
    public string? ShippingAddress { get; set; }
}

