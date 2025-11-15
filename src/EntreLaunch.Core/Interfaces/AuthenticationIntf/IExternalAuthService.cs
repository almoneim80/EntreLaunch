namespace EntreLaunch.Interfaces.AuthenticationIntf
{
    public interface IExternalAuthService
    {
        /// <summary>
        /// Generates the Google OAuth 2.0 authentication URL.
        /// </summary>
        string GenerateGoogleLoginUrl(List<string> requestedScopes);

        /// <summary>
        /// Processes the authorization code returned by the OAuth provider.
        /// </summary>
        Task<AuthResult> HandleGoogleAuthCallbackAsync(string code);
    }
}
