namespace EntreLaunch.PayTabsHosted.API.Exceptions
{
    /// <summary>
    /// Used to report errors related to the API Configuration, such as missing or incorrect settings.
    /// </summary>
    public class ApiConfigurationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiConfigurationException"/> class.
        /// Constructs ApiConfigurationException with given message.
        /// </summary>
        public ApiConfigurationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiConfigurationException"/> class.
        /// Constructs ApiConfigurationException with given message and underlying exception.
        /// </summary>
        public ApiConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
