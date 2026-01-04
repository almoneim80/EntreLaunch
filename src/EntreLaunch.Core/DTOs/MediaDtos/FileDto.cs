namespace EntreLaunch.DTOs.MediaDtos;

public class ValidatedFileResult
{
    public bool IsValid { get; }
    public string? ErrorMessage { get; }
    public string? UniqueName { get; }
    public string? MimeType { get; }
    public long? FileSize { get; }

    public ValidatedFileResult(
        bool isValid,
        string? errorMessage,
        string? uniqueName,
        string? mimeType,
        long? fileSize)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
        UniqueName = uniqueName;
        MimeType = mimeType;
        FileSize = fileSize;
    }
}
