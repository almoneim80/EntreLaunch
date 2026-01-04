namespace EntreLaunch.Entities;

public class PostLike : SharedData
{
    public int? PostId { get; set; }
    public virtual Post? Post { get; set; } = null!;

    public int? CommentId { get; set; }
    public virtual PostComment? Comment { get; set; } = null!;

    public string UserId { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    public bool? IsActive { get; set; }
}
