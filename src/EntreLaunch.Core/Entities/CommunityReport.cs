namespace EntreLaunch.Entities;

public class CommunityReport : SharedData
{
    public string userId { get; set; } = null!;
    public virtual User User { get; set; } = null!;

    public int? PostId { get; set; }
    public virtual Post? Post { get; set; }

    public int? CommentId { get; set; }
    public virtual PostComment? Comment { get; set; } 

    public string? Reason { get; set; }
    public ReportParent? Parent { get; set; }
    public RequestStatus Status { get; set; }
}
