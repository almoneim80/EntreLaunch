using EntreLaunch.Interfaces.BaseIntf;

namespace EntreLaunch.Entities;

public class PostComment : SharedData, IBaseEntity
{
    public int PostId { get; set; }
    public virtual Post Post { get; set; } = null!;

    public string UserId { get; set; } = null!;
    public virtual User User { get; set; } = null!;

    public int? ParentCommentId { get; set; }
    public string? ParentName { get; set; }
    public string? Content { get; set; }
    public CommentStatus? Status { get; set; }

    public virtual ICollection<PostComment>? PostComments { get; set; }
    public virtual ICollection<PostLike>? PostLikes { get; set; }
    public virtual ICollection<CommunityReport>? CommunityReports { get; set; }
}
