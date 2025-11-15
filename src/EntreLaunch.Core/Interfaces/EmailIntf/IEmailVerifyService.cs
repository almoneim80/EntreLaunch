namespace EntreLaunch.Interfaces.EmailIntf
{
    public interface IEmailVerifyService
    {
        /// <summary>
        /// Verifies the email.
        /// </summary>
        Task<GeneralResult<Domain>> Verify(string email);
    }
}
