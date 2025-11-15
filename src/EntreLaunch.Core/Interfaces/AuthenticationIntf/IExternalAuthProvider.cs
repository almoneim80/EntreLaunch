namespace EntreLaunch.Interfaces.AuthenticationIntf;

public interface IExternalAuthProvider
{
    Task<GeneralResult> AuthenticateAsync(string token);
}
