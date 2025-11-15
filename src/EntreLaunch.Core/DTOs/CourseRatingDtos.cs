using System.Text.Json.Serialization;

namespace EntreLaunch.DTOs;

public class CourseRatingCreateDto
{
    private int rating;
    public int CourseId { get; set; }

    [JsonIgnore]
    public string? UserId { get; set; }
    public int Rating
    {
        get => rating;
        set
        {
            if (value < 1 || value > 5)
            {
                _ = new GeneralResult<bool>(false, "Rating must be between 1 and 5.", false);
            }

            rating = value;
        }
    }

    public string? Review { get; set; }
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class CourseRatingEntreLaunchdateDto
{
    private int rating;
    public int CourseId { get; set; }
    [JsonIgnore]
    public string UserId { get; set; } = null!;
    public int Rating
    {
        get => rating;
        set
        {
            if (value < 1 || value > 5)
            {
                _ = new GeneralResult<bool>(false, "Rating must be between 1 and 5.", false);
            }

            rating = value;
        }
    }

    public string? Review { get; set; }
    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class CourseRatingDetailsDto : CourseRatingCreateDto
{
    public int Id { get; set; }
    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; }
}

public class CourseRatingExportDto
{
    public int Rating { get; }
    public int CourseId { get; set; }
    public Guid? UserId { get; set; }
    public string? Review { get; set; }
}
