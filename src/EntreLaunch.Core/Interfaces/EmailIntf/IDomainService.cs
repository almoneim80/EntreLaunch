namespace EntreLaunch.Interfaces.EmailIntf
{
    public interface IDomainService : IEntityService<Domain>
    {
        /// <summary>
        /// Verify domain name using external service.
        /// </summary>
        public Task Verify(Domain domain);

        /// <summary>
        /// Get domain name from email address.
        /// </summary>
        public string GetDomainNameByEmail(string email);

        /// <summary>
        /// Verify domain name using external service.
        /// </summary>
        Task<GeneralResult<DomainDetailsDto>> VerifyDomainAsync(string name, bool force);
    }
}
