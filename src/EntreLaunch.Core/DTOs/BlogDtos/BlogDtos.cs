namespace EntreLaunch.DTOs.BlogDtos;

public class BlogCreateDto
{
#nullable disable
    [JsonIgnore]
    public string UserId { get; set; }
    public string Title { get; set; }
    public string Details { get; set; }
    public string Media { get; set; }

    [JsonIgnore]
    public BlogStatus Status { get; set; } = BlogStatus.Pending;
    [JsonIgnore]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
}

public class BlogDetailsDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Details { get; set; }
    public string Media { get; set; }
    public BlogStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public BlogWriterBto Writer { get; set; }
}

public class BlogWriterBto
{
    public string Name { get; set; }
    public string Avatar { get; set; }
    public string Email { get; set; }
}
