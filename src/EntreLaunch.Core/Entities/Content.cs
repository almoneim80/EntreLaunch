namespace EntreLaunch.Entities;

[Table("content")]
[SupportsElastic]
[SupportsChangeLog]
[Index(nameof(Slug), IsUnique = true)]
public class Content : SharedData, ICommentable
{
    [Searchable]
    [Required]
    public string Title { get; set; } = string.Empty;

    [Searchable]
    [Required]
    public string Description { get; set; } = string.Empty;

    [Searchable]
    [Required]
    public string Body { get; set; } = string.Empty;

    public string CoverImageUrl { get; set; } = string.Empty;

    public string CoverImageAlt { get; set; } = string.Empty;

    [Required]
    public string Slug { get; set; } = string.Empty;

    [Required]
    public string Type { get; set; } = string.Empty;

    [Searchable]
    [Required]
    public string Author { get; set; } = string.Empty;

    [Searchable]
    [Required]
    public string Language { get; set; } = string.Empty;

    [Searchable]
    public string Category { get; set; } = string.Empty;

    [Searchable]
    public string[] Tags { get; set; } = Array.Empty<string>();

    public bool AllowComments { get; set; } = false;

    public DateTimeOffset? PublishedAt { get; set; } = DateTime.UtcNow;

    public static string GetCommentableType()
    {
        return "Content";
    }
}
