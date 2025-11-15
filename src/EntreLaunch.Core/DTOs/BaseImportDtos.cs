using CsvHelper.Configuration.Attributes;

namespace EntreLaunch.DTOs;

public class BaseImportDtoWithIdAndSource
{
    [Optional]
    public int? Id { get; set; }

    [Optional]
    public string? Source { get; set; }
}

public class BaseImportDtoWithDates : BaseImportDtoWithIdAndSource
{
    [Optional]
    public DateTimeOffset? CreatedAt { get; set; }

    [Optional]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; }
}

public class BaseImportDto : BaseImportDtoWithDates
{
    [Optional]
    public string? CreatedByIp { get; set; }

    [Optional]
    public string? CreatedById { get; set; }

    [Optional]
    public string? CreatedByUserAgent { get; set; }

    [Optional]
    public string? EntreLaunchdatedByIp { get; set; }

    [Optional]
    public string? EntreLaunchdatedById { get; set; }

    [Optional]
    public string? EntreLaunchdatedByUserAgent { get; set; }
}
