namespace EntreLaunch.Services
{
    public class ServerConfigurationManager : IServerConfigurationManager
    {
        /// <summary>
        /// Server configuration object for the current application.
        /// </summary>
        public ServerConfiguration Configuration { get; } = new ServerConfiguration();
    }
}
