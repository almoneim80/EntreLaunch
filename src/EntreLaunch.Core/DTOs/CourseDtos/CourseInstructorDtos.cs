namespace EntreLaunch.DTOs;

public class CourseInstructorCreateDto
{
    public int CourseId { get; set; }
    public string UserId { get; set; } = null!;
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateHelper.UtcNow;
}

public class CourseInstructorUpdateDto
{
    public int CourseId { get; set; }
    public string UserId { get; set; } = null!;
    [JsonIgnore]
    public DateTimeOffset? UpdatedAt { get; set; } = DateHelper.UtcNow;
}

public class CourseInstructorDetailsDto
{
#nullable disable
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public Country CountryCode { get; set; }
    public string Specialization { get; set; }
}
