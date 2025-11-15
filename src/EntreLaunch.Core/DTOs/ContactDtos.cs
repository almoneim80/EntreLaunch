using CsvHelper.Configuration.Attributes;
using EntreLaunch.Geography;

namespace EntreLaunch.DTOs;

public abstract class BaseContactDto
{
    public string? Prefix { get; set; }

    public string? FirstName { get; set; }

    public string? MiddleName { get; set; }

    public string? LastName { get; set; }

    public DateTimeOffset? Birthday { get; set; }

    public Continent? ContinentCode { get; set; }

    public Country? CountryCode { get; set; }

    public string? CityName { get; set; }

    public string? Address1 { get; set; }

    public string? Address2 { get; set; }

    public string? JobTitle { get; set; }

    public string? CompanyName { get; set; }

    public string? Department { get; set; }

    public string? State { get; set; }

    public string? Zip { get; set; }

    public string? Phone { get; set; }

    public int? Timezone { get; set; }

    public string? Language { get; set; }

    public Dictionary<string, string>? SocialMedia { get; set; }

    [SwaggerHide]
    public int? UnsubscribeId { get; set; }

    public string? Source { get; set; }
}

public class ContactCreateDto : BaseContactDto
{
    private string email = string.Empty;

    [Required]
    [EmailAddress]
    public string Email
    {
        get
        {
            return email;
        }

        set
        {
            email = value.ToLower();
        }
    }
}

public class ContactEntreLaunchdateDto : BaseContactDto
{
    private string? email;

    [EmailAddress]
    public string? Email
    {
        get
        {
            return email;
        }

        set
        {
            email = value == null ? null : value.ToLower();
        }
    }
}

public class ContactDetailsDto : ContactCreateDto
{
    public int Id { get; set; }

    public string AvatarUrl { get; set; } = string.Empty;

    public DateTimeOffset? CreatedAt { get; set; }

    public DateTimeOffset? EntreLaunchdatedAt { get; set; }

    public int DomainId { get; set; }

    public int AccountId { get; set; }

    [CsvHelper.Configuration.Attributes.Ignore]
    public DomainDetailsDto? Domain { get; set; }

    [CsvHelper.Configuration.Attributes.Ignore]
    public AccountDetailsDto? Account { get; set; }
}

public class ContactImportDto : BaseImportDto
{
    private string? email;

    [Optional]
    [EmailAddress]
    [SwaggerUnique]
    public string? Email
    {
        get
        {
            return email;
        }

        set
        {
            email = value == null ? null : value.ToLower();
        }
    }

    [Optional]
    public string? Prefix { get; set; }

    [Optional]
    public string? FirstName { get; set; }

    [Optional]
    public string? MiddleName { get; set; }

    [Optional]
    public string? LastName { get; set; }

    [Optional]
    public DateTimeOffset? Birthday { get; set; }

    [Optional]
    public Continent? ContinentCode { get; set; }

    [Optional]
    public Country? CountryCode { get; set; }

    [Optional]
    public string? CityName { get; set; }

    [Optional]
    public string? Address1 { get; set; }

    [Optional]
    public string? Address2 { get; set; }

    [Optional]
    public string? JobTitle { get; set; }

    [Optional]
    public string? CompanyName { get; set; }

    [Optional]
    public string? Department { get; set; }

    [Optional]
    public string? State { get; set; }

    [Optional]
    public string? Zip { get; set; }

    [Optional]
    public string? Phone { get; set; }

    [Optional]
    public int? Timezone { get; set; }

    [Optional]
    public string? Language { get; set; }

    [Optional]
    public Dictionary<string, string>? SocialMedia { get; set; }

    [Optional]
    public int? UnsubscribeId { get; set; }

    [Optional]
    public int? AccountId { get; set; }

    [Optional]
    [SurrogateForeignKey(typeof(Account), "Name", "AccountId")]
    public string? AccountName { get; set; }

    [Optional]
    public int? DomainId { get; set; }
}
