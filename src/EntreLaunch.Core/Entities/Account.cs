using System;
using System.Text.Json.Serialization;
using EntreLaunch.Geography;

namespace EntreLaunch.Entities;

[Table("account")]
[SupportsElastic]
[SupportsChangeLog]
[Index(nameof(Name), IsUnique = true)]
public class Account : SharedData, ICommentable
{
    [Searchable]
    public string Name { get; set; } = string.Empty;

    [Searchable]
    public string? CityName { get; set; }

    [Searchable]
    public string? State { get; set; }

    [Searchable]
    public Continent? ContinentCode { get; set; }

    [Searchable]
    public Country? CountryCode { get; set; }

    [Searchable]
    public string? SiteUrl { get; set; }

    public string? LogoUrl { get; set; }

    [Searchable]
    public string? EmployeesRange { get; set; }

    [Searchable]
    public double? Revenue { get; set; }

    [Searchable]
    public string[]? Tags { get; set; }

    [Searchable]
    public Dictionary<string, string>? SocialMedia { get; set; }

    [Searchable]
    public string? Data { get; set; }

    [JsonIgnore]
    public virtual ICollection<Contact>? Contacts { get; set; }

    [JsonIgnore]
    public virtual ICollection<Domain>? Domains { get; set; }

    public static string GetCommentableType()
    {
        return "Account";
    }
}
