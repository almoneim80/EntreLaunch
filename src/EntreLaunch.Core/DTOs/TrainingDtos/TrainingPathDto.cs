namespace EntreLaunch.DTOs.TrainingDtos;

public class TrainingPathCreateDto
{
#nullable disable
    public string Name { get; set; }
    public decimal? Price { get; set; }
    public string Description { get; set; }
    public bool CertificateExists { get; set; }
    public int CertificateValidityInDays { get; set; }
    public int MaxEnrollment { get; set; }
    public bool IsFree { get; set; }

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateHelper.UtcNow;
}

public class TrainingPathUpdateDto
{
#nullable enable
    public string? Name { get; set; }
    public decimal? Price { get; set; }
    public string? Description { get; set; }
    public bool? CertificateExists { get; set; }
    public int? CertificateValidityInDays { get; set; }
    public int? MaxEnrollment { get; set; }
    public bool? IsFree { get; set; }

    [JsonIgnore]
    public DateTimeOffset? UpdatedAt { get; set; } = DateHelper.UtcNow;
}

public class TrainingPathExportDto
{
    public string? Name { get; set; }

    public int? CoursesNumber { get; set; }

    public decimal? Price { get; set; }

    public string? Description { get; set; }
}

public class TrainingPathDetailsDto : TrainingPathCreateDto
{
    public int Id { get; set; }
    [JsonIgnore]
    public DateTimeOffset? UpdatedAt { get; set; }
}

// Training Path with full Data
public class TrainingPathFullDetailsDto
{
#nullable disable
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal? Price { get; set; }
    public string Description { get; set; }
    public bool CertificateExists { get; set; }
    public int CertificateValidityInDays { get; set; }
    public int MaxEnrollment { get; set; }
    public bool IsFree { get; set; }

    public List<PathCourseDetailsDto> PathCourses { get; set; }
}

public class TrainingPathSubscripersDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTimeOffset SubscribedAt { get; set; }
}
