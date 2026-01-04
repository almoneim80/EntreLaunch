namespace EntreLaunch.Entities;

public class MyPartnerAttachment : SharedData
{
    public int ProjectId { get; set; }
    public virtual MyPartner Project { get; set; } = null!;

    public string? Url { get; set; }
}
