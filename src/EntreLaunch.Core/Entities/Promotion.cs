namespace EntreLaunch.Entities;

[Table("promotion")]
[SupportsElastic]
[SupportsChangeLog]
[Index(nameof(Code), IsUnique = true)]

public class Promotion : SharedData
{
    [Required]
    public string Code { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    public DateTimeOffset? StartDate { get; set; }

    public DateTimeOffset? EndDate { get; set; }
}
