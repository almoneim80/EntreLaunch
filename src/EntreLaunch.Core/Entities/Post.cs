using EntreLaunch.Interfaces.BaseIntf;

namespace EntreLaunch.Entities;

[SupportsElastic]
[SupportsChangeLog]
[Table("posts")]
public class Post : SharedData, IBaseEntity
{
    public string UserId { get; set; } = null!;
    public virtual User User { get; set; } = null!;

    [Searchable]
    public string? Text { get; set; }
    public RequestStatus Status { get; set; }

    [Searchable]
    public virtual ICollection<PostComment>? PostComments { get; set; }
    public virtual ICollection<PostLike>? PostLikes { get; set; }
    public virtual ICollection<PostMedia>? PostMedias { get; set; }
    public virtual ICollection<CommunityReport>? CommunityReports { get; set; }
}
