using EntreLaunch.PayTabsHosted.API.Types;
using Environment = EntreLaunch.PayTabsHosted.API.Types.Environment;

namespace EntreLaunch.PayTabsHosted.API
{
    /// <summary>
    /// Represents the basic settings of the PayTabs API, such as keys, region, currency, and algorithm.
    /// </summary>
#nullable disable
    public class ApiConfiguration
    {
        private string privateKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiConfiguration"/> class without Enviroment.
        /// </summary>
        /// <param name="profileId">Your merchant profile id , you can find the profile id on your PayTabs Merchant’s Dashboard- profile.</param>
        /// <param name="serverKey">You can find the Server key on your PayTabs Merchant’s Dashboard - Developers - Key management.</param>
        /// <param name="region">The region you registered in with PayTabs you must pass value from this array ['ARE','EGY','SAU','OMN','JOR','GLOBAL'].</param>
        /// <param name="currency">The currency you registered in with PayTabs account you must pass value from this array['AED', 'EGP', 'SAR', 'OMR', 'JOD', 'US'].</param>
        /// <param name="algorithm">The Amazon Signature algorithm, uses PayTabsSignatureAlgorithm.Default, if not specified.</param>
        public ApiConfiguration(int profileId, string serverKey, string clientKey, Region region, Currency currency, PayTabsSignatureAlgorithm algorithm = PayTabsSignatureAlgorithm.Default)
        {
            ProfileId = profileId;
            ServerKey = serverKey;
            ClientKey = clientKey;
            Region = region;
            Currency = currency;
            Algorithm = algorithm;
        }

        /// <summary>
        /// Gets or sets the identifier for the registered key pair Your merchant profile id , you can find the profile id on your PayTabs Merchant’s Dashboard- profile.
        /// </summary>
        public int ProfileId { get; set; }

        /// <summary>
        /// Gets or sets you can find the Server key on your PayTabs Merchant’s Dashboard - Developers - Key management.
        /// </summary>
        public string ServerKey { get; set; }

        /// <summary>
        /// Gets or sets you can find the Server key on your PayTabs Merchant’s Dashboard - Developers - Key management.
        /// </summary>
        public string ClientKey { get; set; }

        /// <summary>
        /// Gets or sets the region you registered in with PayTabs you must pass value from this array ['ARE','EGY','SAU','OMN','JOR','GLOBAL'].
        /// </summary>
        public Region Region { get; set; }

        /// <summary>
        /// Gets or sets the currency you registered in with PayTabs account you must pass value from this array['AED', 'EGP', 'SAR', 'OMR', 'JOD', 'US'].
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// Gets or sets the private key in form of a file path, or directly as a string.
        /// </summary>
        public string PrivateKey
        {
            get
            {
                return privateKey;
            }

            set
            {
                FileInfo fileInfo;
                try
                {
                    fileInfo = new FileInfo(value);
                } 
                catch(Exception)
                {
                    fileInfo = null;
                }
                if (fileInfo != null && fileInfo.Exists)
                {
                    privateKey = File.ReadAllText(value);
                }
                else
                {
                    privateKey = value;
                }
                if (!privateKey.StartsWith("-----"))
                {
                    if (fileInfo != null && fileInfo.Exists)
                    {
                        throw new ArgumentException("Provided file does not contain a private key in the expected format");
                    }
                    else
                    {
                        throw new FileNotFoundException("Provided private key file cannot be found", privateKey);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the PayTabs environment (live or sandbox).
        /// </summary>
        public Environment Environment { get; set; }

        /// <summary>
        /// Gets or sets the Amazon Signature algorithm, uses PayTabsSignatureAlgorithm.Default, if not specified.
        /// </summary>
        public PayTabsSignatureAlgorithm Algorithm { get; set; }

        /// <summary>
        /// Gets or sets specifies how often the API client will retry an API request in case of failure.
        /// </summary>
        public int MaxRetries { get; set; } = 3;
    }
}
