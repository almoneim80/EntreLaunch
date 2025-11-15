namespace EntreLaunch.DTOs;

public class PostWithMediaCreateDto
{
    [JsonIgnore]
    public string? UserId { get; set; }
    public string? Text { get; set; }
    public List<MediaCreateDto>? Media { get; set; }

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class TextPostCreateDto
{
    [JsonIgnore]
    public string? UserId { get; set; }
    public string? Text { get; set; }

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class PostEntreLaunchdateDto
{
#nullable disable
    [JsonIgnore]
    public string UserId { get; set; }
    public string Text { get; set; }

    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class PostDetailsDto
{
#nullable disable
    public int Id { get; set; }
    public string Text { get; set; }
    public PostUserData User { get; set; }
    public List<PostMediaDetailsDto> Media { get; set; }
    public List<CommentDetailsDto> Comments { get; set; }
    public int Likes { get; set; }
    public DateTimeOffset? CreatedAt { get; set; } 
}

/*******Comment********/
public class CommentCreateDto
{
#nullable disable
    public int PostId { get; set; }
    [JsonIgnore]
    public string UserId { get; set; } = null!;
    public int? ParentCommentId { get; set; }
    public string Content { get; set; }

    [JsonIgnore]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class CommentEntreLaunchdateDto
{
#nullable enable
    [JsonIgnore]
    public string? UserId { get; set; } = null!;
    public string? Content { get; set; }

    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class CommentDetailsDto
{
#nullable disable
    public int ParentCommentId { get; set; }
    public string ParentName { get; set; }
    public CommentUserData User { get; set; }
    public string Content { get; set; }
    public CommentStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

/*******Post Like********/
public class LikeCreateDto
{
    public int? PostId { get; set; }
    public int? CommentId { get; set; }

    [JsonIgnore]
    public string UserId { get; set; }

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

/*******Media********/
public class MediaCreateDto
{
#nullable disable
    public string MediaType { get; set; }
    public string Url { get; set; }

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class MediaEntreLaunchdateDto
{
#nullable enable
    public string? Url { get; set; }

    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class MediaDetailsDto
{
#nullable disable
    public int Id { get; set; }
    public Post Post { get; set; }
    public string Url { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? EntreLaunchdatedAt { get; set; }
}

public class PostMediaDetailsDto
{
#nullable disable
    public string Url { get; set; }
}

/*******Report********/
public class ReportCreateDto
{
#nullable disable
    [JsonIgnore]
    public string UserId { get; set; } = null!;
    public int? PostId { get; set; }
    public int? CommentId { get; set; }
    public string Reason { get; set; }

    [JsonIgnore]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class ReportDetailsDto
{
#nullable enable
    public ReportUserData? User { get; set; }
    public int? PostId { get; set; }
    public int? CommentId { get; set; }
    public ReportParent? Parent { get; set; }
    public RequestStatus Status { get; set; }
    public string? Reason { get; set; }

    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? EntreLaunchdatedAt { get; set; }
}

/*******Othet data********/
public class PostUserData
{
#nullable disable
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Specialization { get; set; }
    public Country CountryCode { get; set; }
}

public class CommentUserData
{
#nullable disable
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

public class ReportUserData
{
#nullable disable
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}
