using EntreLaunch.Services;

namespace EntreLaunch.Enums;
public enum RequestStatus
{
    Pending = 0,
    Accepted = 1,
    Rejected = 2
}

public enum CommentStatus
{
    Good = 0,
    Spam = 1,
    Dangerous = 2,
}

public enum ReportParent
{
    Post = 0,
    Comment = 1
}
