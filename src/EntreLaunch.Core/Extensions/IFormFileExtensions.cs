using EntreLaunch.DTOs.MediaDtos;

namespace EntreLaunch.Extensions
{
    public static class IFormFileExtensions
    {
        public static ValidatedFileResult PrepareValidatedFile(
            this IFormFile file,
            MediaType expectedType,
            FileValidatorHelper validator)
        {
            var (isValid, errorMessage) = validator.Validate(file, expectedType);
            if (!isValid)
                return new ValidatedFileResult(false, errorMessage, null, null, null);

            var originalName = Path.GetFileNameWithoutExtension(file.FileName);
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var cleanName = originalName.Slugify();
            var uniqueId = Guid.NewGuid().ToString("N")[..8];
            var uniqueName = $"{cleanName}_{uniqueId}{extension}";

            return new ValidatedFileResult(true, null, uniqueName, file.ContentType, file.Length);
        }
    }
}
