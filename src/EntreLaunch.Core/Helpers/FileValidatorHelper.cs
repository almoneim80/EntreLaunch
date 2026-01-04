using System.Text.RegularExpressions;

namespace EntreLaunch.Helpers;

public class FileValidatorHelper
{
    private readonly FileUploadSettings _settings;

    public FileValidatorHelper(IOptions<FileUploadSettings> options)
    {
        _settings = options.Value;
    }

    public (bool Success, string? ErrorMessage) Validate(IFormFile file, MediaType type)
    {
        if (file == null)
            return (false, "File is required.");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        var category = GetCategorySettings(type);
        if (category == null)
            return (false, $"Unsupported file type category: {type}.");

        if (!category.Extensions.Contains(extension))
            return (false, $"Invalid file type '{extension}' for {type}.");

        var maxSizeStr = category.MaxSizePerExtension.ContainsKey(extension)
            ? category.MaxSizePerExtension[extension]
            : category.MaxSizePerExtension.GetValueOrDefault("default", "10MB");

        if (!TryParseSize(maxSizeStr, out var maxSizeBytes))
            return (false, $"Invalid max size format in config: '{maxSizeStr}'.");

        if (file.Length > maxSizeBytes)
            return (false, $"File size exceeds limit of {maxSizeStr}.");

        return (true, null);
    }

    private FileCategorySetting? GetCategorySettings(MediaType type) => type switch
    {
        MediaType.Image => _settings.Images,
        MediaType.Video => _settings.Videos,
        MediaType.Document => _settings.Documents,
        _ => null
    };

    private static bool TryParseSize(string sizeStr, out long sizeInBytes)
    {
        sizeInBytes = 0;
        var match = Regex.Match(sizeStr.Trim(), @"(?i)^(\d+)(KB|MB|GB)$");

        if (!match.Success)
            return false;

        var value = long.Parse(match.Groups[1].Value);
        var unit = match.Groups[2].Value.ToUpper();

        sizeInBytes = unit switch
        {
            "KB" => value * 1024,
            "MB" => value * 1024 * 1024,
            "GB" => value * 1024 * 1024 * 1024,
            _ => 0
        };

        return sizeInBytes > 0;
    }
}
