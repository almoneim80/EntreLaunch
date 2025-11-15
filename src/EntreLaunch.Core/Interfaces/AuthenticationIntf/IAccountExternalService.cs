namespace EntreLaunch.Interfaces.AuthenticationIntf
{
    public interface IAccountExternalService
    {
        Task<AccountDetailsInfo?> GetAccountDetails(string domain);
    }
}
