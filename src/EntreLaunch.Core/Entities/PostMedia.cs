namespace EntreLaunch.Entities;

public class PostMedia : SharedData
{
#nullable disable
    public int PostId { get; set; }
    public virtual Post Post { get; set; } = null!;

    public string MediaType { get; set; }
    public string Url { get; set; }
}
