namespace EntreLaunch.Helpers
{
    public static class DateHelper
    {
        /// <summary>
        /// Gets the current UTC time.
        /// </summary>
        public static DateTimeOffset UtcNow => DateTimeOffset.UtcNow.ToUniversalTime();

        /// <summary>
        /// Gets returns current UTC date as DateTimeOffset (00:00:00 time).
        /// </summary>
        public static DateTimeOffset UtcToday => DateTimeOffset.UtcNow.Date.ToUniversalTime();

        /// <summary>
        /// Gets returns tomorrow's UTC start (00:00:00).
        /// </summary>
        public static DateTimeOffset UtcTomorrow => DateTimeOffset.UtcNow.Date.AddDays(1).ToUniversalTime();

        /// <summary>
        /// Normalizes any DateTimeOffset to UTC with zero offset.
        /// </summary>
        public static DateTimeOffset NormalizeToUtc(DateTimeOffset value)
        {
            return value.ToUniversalTime();
        }

        /// <summary>
        /// Normalizes nullable DateTimeOffset to UTC with fallback.
        /// </summary>
        public static DateTimeOffset NormalizeToUtc(DateTimeOffset? value, DateTimeOffset fallback)
        {
            return value?.ToUniversalTime() ?? fallback;
        }
    }
}
