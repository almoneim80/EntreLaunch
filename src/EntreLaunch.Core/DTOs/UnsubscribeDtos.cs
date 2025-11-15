using CsvHelper.Configuration.Attributes;

namespace EntreLaunch.DTOs;

public class UnsubscribeDto
{
    public int ContactId { get; set; }

    public string Reason { get; set; } = string.Empty;

    public string Source { get; set; } = string.Empty;
}

public class UnsubscribeDetailsDto : UnsubscribeDto
{
    public int Id { get; set; }

    public DateTimeOffset? CreatedAt { get; set; }
}

public class UnsubscribeImportDto : BaseImportDtoWithIdAndSource
{
    private string contactEmail = string.Empty;

    public string Reason { get; set; } = string.Empty;

    public int ContactId { get; set; }

    [Optional]
    [SurrogateForeignKey(typeof(Contact), "Email", "ContactId")]
    public string ContactEmail
    {
        get
        {
            return contactEmail;
        }

        set
        {
            contactEmail = value.ToLower();
        }
    }

    [Optional]
    public DateTimeOffset? CreatedAt { get; set; }
}

public class UnsubscribeExportDto
{
    public int ContactId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
}
