namespace EntreLaunch.Entities;

[SupportsElastic]
[SupportsChangeLog]
[Table("my_partner")]
public class MyPartner : SharedData
{
    public string UserId { get; set; } = null!;
    public virtual User User { get; set; } = null!;

    [Searchable]
    public string? Activity { get; set; }

    [Searchable]
    public string? City { get; set; }

    [Searchable]
    public string? Sector { get; set; }

    [Searchable]
    public decimal? Cost { get; set; }

    [Searchable]
    public string? Idea { get; set; }

    [Searchable]
    public List<string>? AcceptRequirements { get; set; }

    [Searchable]
    public decimal? CapitalFrom { get; set; }

    [Searchable]
    public decimal CapitalTo { get; set; }

    [Searchable]
    public string? Contact { get; set; }

    public MyPartnerStatus Status { get; set; } 

    [Searchable]
    public virtual ICollection<MyPartnerAttachment>? ProjectAttachments { get; set; }
}
