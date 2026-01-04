namespace EntreLaunch.DTOs;

public class CourseRatingCreateDto
{
    public int CourseId { get; set; }

    [JsonIgnore]
    public string? UserId { get; set; }
    public int Rating { get; set; }

    public string? Review { get; set; }
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateHelper.UtcNow;
}

public class CourseRatingUpdateDto
{
    public int CourseId { get; set; }
    [JsonIgnore]
    public string UserId { get; set; } = null!;
    public int Rating { get; set; }

    public string? Review { get; set; }
    [JsonIgnore]
    public DateTimeOffset? UpdatedAt { get; set; } = DateHelper.UtcNow;
}

public class CourseRatingDetailsDto
{
#nullable disable
    public int Id { get; set; }
    public string CourseName { get; set; }
    public int RatingValue { get; set; }
    public string Review { get; set; }
    public string ReviewerName { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
}

public class CourseRatingExportDto
{
    public int Rating { get; }
    public int CourseId { get; set; }
    public Guid? UserId { get; set; }
    public string Review { get; set; }
}
