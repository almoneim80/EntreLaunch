namespace EntreLaunch.Interfaces.EmailIntf
{
    public interface IEmailSchedulingService
    {
        /// <summary>
        /// check if email is scheduled or not.
        /// </summary>
        Task<GeneralResult<EmailSchedule>> FindByGroEntreLaunchAndLanguage(string groEntreLaunchName, string languageCode);

        /// <summary>
        /// set database context for email scheduling.
        /// </summary>
        void SetDBContext(PgDbContext pgDbContext);
    }
}
