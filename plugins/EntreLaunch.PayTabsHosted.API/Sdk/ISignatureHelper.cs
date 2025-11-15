using System;
using System.Collections.Generic;
using EntreLaunch.PayTabsHosted.API.Types;

namespace EntreLaunch.PayTabsHosted.API
{
    public interface ISignatureHelper
    {
        /// <summary>
        /// Creates a string that includes the information from the request in a 
        /// standardized (canonical) format.
        /// </summary>
        /// <seealso cref="http://amazonpaycheckoutintegrationguide.s3.amazonaws.com/amazon-pay-api-v2/signing-requests.html"/>
        string CreateCanonicalRequest(ApiRequest apiRequest, Dictionary<string, List<string>> preSignedHeaders);

        /// <summary>
        /// Generates the mandatory headers required in the request.
        /// </summary>
        Dictionary<string, List<string>> CreateDefaultHeaders(Uri uri);

        /// <summary>
        /// Creates the string that is going to be signed.
        /// </summary>
        string CreateStringToSign(string canonicalRequest);

        /// <summary>
        /// Generates a signature for the string passed in.
        /// </summary>
        string GenerateSignature(string stringToSign);
    }
}
