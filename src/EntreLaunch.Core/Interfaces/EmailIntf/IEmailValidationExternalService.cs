namespace EntreLaunch.Interfaces.EmailIntf
{
    public interface IEmailValidationExternalService
    {
        /// <summary>
        /// check email domain.
        /// </summary>
        Task<GeneralResult<EmailVerifyInfoDto>> Validate(string email);
    }
}
