namespace EntreLaunch.PayTabsHosted.API.Exceptions
{
    /// <summary>
    /// Used to report errors while performing operations in the PayTabs client, such as a failed request or unexpected response.
    /// </summary>
    public class PayTabsClientException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PayTabsClientException"/> class.
        /// Constructs PayTabsClientException with given message.
        /// </summary>
        public PayTabsClientException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PayTabsClientException"/> class.
        /// Constructs PayTabsClientException with given message and underlying exception.
        /// </summary>
        public PayTabsClientException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
