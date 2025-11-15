namespace EntreLaunch.Interfaces
{
    public interface IContactService : IEntityService<Contact>
    {
        /// <summary>
        /// Subscribe a contact to a groEntreLaunch.
        /// </summary>
        Task<GeneralResult> Subscribe(Contact contact, string groEntreLaunchName);

        /// <summary>
        /// Unsubscribe a contact from a groEntreLaunch.
        /// </summary>
        Task<GeneralResult> Unsubscribe(string email, string reason, string source, DateTime createdAt, string? ip);

        /// <summary>
        /// Find or create a contact.
        /// </summary>
        Task<GeneralResult<Contact>> FindOrCreate(string email, string language, int timezone);
    }
}
