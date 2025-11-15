namespace EntreLaunch.Entities;

public class CourseRating : SharedData
{
    public int CourseId { get; set; }
    public virtual Course Course { get; set; } = null!;

    public string UserId { get; set; } = null!;
    public virtual User User { get; set; } = null!;

    public int Rating { get; set; } = 0;
    public string? Review { get; set; }

    public RatingStatus Status { get; set; } = RatingStatus.Pending;
    public string? ReviewNote { get; set; }
}
