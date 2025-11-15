using System;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace EntreLaunch.PayTabsHosted.API
{
    public class Util
    {
        /// <summary>
        /// Generates a url encoded string from the given string.
        /// Purpose: Convert texts to URL Encoding format so that they are suitable for sending in API links.
        /// Retains allowed characters such as (a-z, A-Z, 0-9, -_.~).
        /// Converts other characters to encrypted characters such as %20 for spaces.
        /// </summary>
        public static string UrlEncode(string data, bool path)
        {
            if (data == null)
            {
                return "";
            }

            if (path)
            {
                data = Regex.Replace(data, @"/+", "/");
            }

            StringBuilder encoded = new StringBuilder();
            string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~" + (path ? "/" : "");

            foreach (char symbol in Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(data)))
            {
                if (unreservedChars.IndexOf(symbol) != -1)
                {
                    encoded.Append(symbol);
                }
                else
                {
                    encoded.Append("%" + string.Format("{0:X2}", (int)symbol));
                }
            }

            return encoded.ToString();
        }

        /// <summary>
        /// Formats date as ISO 8601 timestamp.
        /// Purpose: Create a timestamp in ISO 8601 format.
        /// How it works: Returns the current time in yyyyyMMddTHHmmssZ format.
        /// </summary>
        public static string GetFormattedTimestamp()
        {
            return DateTime.UtcNow.ToString("yyyyMMdd\\THHmmss\\Z",
                                        CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Generates the next wait interval, in milliseconds, using an exponential
        /// backoff algorithm.
        /// Purpose: Calculate an ascending wait time when retrying.
        /// The wait time doubles with each new attempt using the formula 2^retryCount * 1000.
        /// </summary>
        public static int GetExponentialWaitTime(int retryCount)
        {
            return (int)Math.Pow(2, retryCount) * 1000;
        }

        /// <summary>
        /// Generates the user agent header.
        /// Purpose: Create a value for the User-Agent header to identify the application sending the request.
        /// Add the SDK name (paytabs-getway-api-sdk-dotnet).
        /// Adds the version (1.0.0.0.0).
        /// Adds the current operating system.
        /// </summary>
        public static string BuildUserAgentHeader()
        {
            string osVersion = System.Environment.OSVersion.ToString();

            StringBuilder userAgentBuilder = new StringBuilder(Constants.SdkName).Append("/")
                .Append(Constants.SdkVersion)
                .Append("(").Append(osVersion).Append(")");

            return userAgentBuilder.ToString();
        }

        //https://swimburger.net/blog/dotnet/configure-servicepointmanager-securityprotocol-through-appsettings

        /// <summary>
        /// Checks if the passed set of SecurityProtocolType is using only outdated protocols.
        /// Purpose: To check if the security protocols used are outdated.
        /// </summary>
        /// <param name="securityProtocolTypes">A set of security protocol types.</param>
        /// <returns>True if using outdated security protocol type versions, false otherwise.</returns>
        /// <remarks>
        /// This method will effectively ensure that as a minimum TLS version 1.1 is being used for API calls.
        /// Please note that the provdided parameter doesn't contain a single protocol type only, but may 
        /// contain a set, e.g. "SSL3.0 | TLS1.0".
        /// </remarks>
        public static bool IsObsoleteSecurityProtocol(SecurityProtocolType securityProtocolTypes)
        {
            // check if there is an outdated protocol being used
            if (securityProtocolTypes.HasFlag(SecurityProtocolType.Tls) && !securityProtocolTypes.HasFlag(SecurityProtocolType.Tls11) && !securityProtocolTypes.HasFlag(SecurityProtocolType.Tls12))
            {
                return true;
            }

            return false;
        }
    }
}
