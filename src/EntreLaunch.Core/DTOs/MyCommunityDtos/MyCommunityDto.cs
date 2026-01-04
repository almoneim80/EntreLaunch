namespace EntreLaunch.DTOs.MyCommunityDtos;

public class PostWithMediaCreateDto
{
#nullable disable
    [JsonIgnore]
    public string UserId { get; set; }
    public string Text { get; set; }
    public List<MediaCreateDto> Media { get; set; }

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateHelper.UtcNow;
}

public class TextPostCreateDto
{
#nullable disable
    [JsonIgnore]
    public string UserId { get; set; }
    public string Text { get; set; }

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateHelper.UtcNow;
}

public class PostDetailsDto
{
#nullable disable
    public int Id { get; set; }
    public string Text { get; set; }
    public PostUserData User { get; set; }
    public List<PostMediaDetailsDto> Media { get; set; }
    public int Likes { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
}

/*******Post Like********/
public class LikeCreateDto
{
    public int? PostId { get; set; }
    public int? CommentId { get; set; }

    [JsonIgnore]
    public string UserId { get; set; }

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateHelper.UtcNow;
}

/*******Media********/
public class MediaCreateDto
{
#nullable disable
    public string MediaType { get; set; }
    public string Url { get; set; }

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateHelper.UtcNow;
}

public class MediaDetailsDto
{
#nullable disable
    public int Id { get; set; }
    public Post Post { get; set; }
    public string Url { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
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

    [JsonIgnore]
    public int? CommentId { get; set; }
    public string Reason { get; set; }

    [JsonIgnore]
    public DateTimeOffset CreatedAt { get; set; } = DateHelper.UtcNow;
}

public class ReportDetailsDto
{
#nullable enable
    public ReportUserData? User { get; set; }
    public int? PostId { get; set; }
    public ReportParent? Parent { get; set; }
    public RequestStatus Status { get; set; }
    public string? Reason { get; set; }

    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}

/*******Othet data********/
public class PostUserData
{
#nullable disable
    public string UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Specialization { get; set; }
    public Country CountryCode { get; set; }
}

public class CommentUserData
{
#nullable disable
    public string UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

public class ReportUserData
{
#nullable disable
    public string UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}
