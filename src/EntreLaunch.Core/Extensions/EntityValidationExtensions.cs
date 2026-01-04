using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Extensions
{
    public static class EntityValidationExtensions
    {
        /// <summary>
        /// Checks if an entity exists and is not deleted.
        /// Returns a BadRequestObjectResult with a localized message if not found.
        /// </summary>
        public static async Task<IActionResult?> CheckIfEntityExistsAsync<T>(
            this int entityId,
            IExtendedBaseService baseService,
            Microsoft.Extensions.Logging.ILogger logger,
            ILocalizationManager localization,
            string? notFoundMessage = null) where T : SharedData
        {
            var exists = await baseService.IsEntityExistsAndNotDeletedAsync<T>(entityId);
            if (!exists.IsSuccess)
            {
                var message = notFoundMessage ?? localization.GetLocalizedString("EntityNotFound").Replace("{0}", typeof(T).Name);

                logger.LogWarning(message);
                var result = new GeneralResult<object>(false, message, null);
                return new BadRequestObjectResult(result);
            }

            return null;
        }
    }
}
