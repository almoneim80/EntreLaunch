using EntreLaunch.PayTabsHosted.API.Types;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System.Text;

namespace EntreLaunch.PayTabsHosted.API
{
    /// <summary>
    ///  Used to create digital signatures for orders sent to PayTabs. These signatures ensure data security and authenticity. 
    /// </summary>
    public class SignatureHelper : ISignatureHelper
    {
        private readonly ApiConfiguration payConfiguration;
        private readonly string lineSeparator = "\n";
        private readonly CanonicalBuilder canonicalBuilder;

        public SignatureHelper(ApiConfiguration payConfiguration, CanonicalBuilder canonicalBuilder)
        {
            this.payConfiguration = payConfiguration;
            this.canonicalBuilder = canonicalBuilder;
        }

        /// <summary>
        /// Creates a string that includes the information from the request in a 
        /// standardized (canonical) format.
        /// Purpose: Convert the request to Canonical Format to facilitate signing.
        /// </summary>
        /// <seealso cref="http://amazonpaycheckoutintegrationguide.s3.amazonaws.com/amazon-pay-api-v2/signing-requests.html"/>
        public string CreateCanonicalRequest(ApiRequest apiRequest, Dictionary<string, List<string>> preSignedHeaders)
        {
            string path = apiRequest.Path.AbsolutePath;
            StringBuilder canonicalRequestBuilder = new StringBuilder();

            // if a body was passed to the request, convert it into a JSON string
            string body = string.Empty;
            if (apiRequest.Body != null)
            {
                body = apiRequest.Body.ToJson();
            }

            canonicalRequestBuilder.Append(apiRequest.HttpMethod.ToString())
                                    .Append(lineSeparator)
                                    .Append(canonicalBuilder.GetCanonicalizedURI(path))
                                    .Append(lineSeparator)
                                    .Append(canonicalBuilder.GetCanonicalizedQueryString(apiRequest.QueryParameters))
                                    .Append(lineSeparator)
                                    .Append(canonicalBuilder.GetCanonicalizedHeaderString(preSignedHeaders))
                                    .Append(lineSeparator)
                                    .Append(canonicalBuilder.GetSignedHeadersString(preSignedHeaders))
                                    .Append(lineSeparator)
                                    .Append(canonicalBuilder.HashThenHexEncode(body));

            return canonicalRequestBuilder.ToString();
        }

        /// <summary>
        /// Generates the mandatory headers required in the request.
        /// Purpose: Create the required basic request headers such as:
        /// Content-Type.
        /// Region (Region).
        /// Date.
        /// Host.
        /// </summary>
        public Dictionary<string, List<string>> CreateDefaultHeaders(Uri uri)
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            List<string> acceptHeaderValue = new List<string>
             {
                 "application/json"
             };
            headers.Add("accept", acceptHeaderValue);

            List<string> contentHeaderValue = new List<string>
             {
                 "application/json"
             };
            headers.Add("content-type", contentHeaderValue);

            List<string> regionHeaderValue = new List<string>
             {
                // TODO: replace by method parameter to get rid of config dependency
                 payConfiguration.Region.ToShortform()
             };
            headers.Add(Constants.Headers.Region, regionHeaderValue);

            List<string> dateHeaderValue = new List<string>
             {
                 Util.GetFormattedTimestamp()
             };
            headers.Add(Constants.Headers.Date, dateHeaderValue);

            List<string> hostHeaderValue = new List<string>
             {
                 uri.Host
             };
            headers.Add(Constants.Headers.Host, hostHeaderValue);

            return headers;
        }

        /// <summary>
        /// Creates the string that is going to be signed.
        /// Purpose: Create text to be used for signing based on the Canonical Request.
        /// </summary>
        public string CreateStringToSign(string canonicalRequest)
        {
            string hashedCanonicalRequest = canonicalBuilder.HashThenHexEncode(canonicalRequest);

            StringBuilder stringToSignBuilder = new StringBuilder(payConfiguration.Algorithm.GetName());
            stringToSignBuilder.Append(lineSeparator)
                                .Append(hashedCanonicalRequest);

            return stringToSignBuilder.ToString();
        }

        /// <summary>
        /// Generates a signature for the string passed in.
        /// Purpose: Create the final signature using the private key.
        /// </summary>
        public string GenerateSignature(string stringToSign)
        {
            SecureRandom random = new SecureRandom();

            byte[] bytesToSign = Encoding.UTF8.GetBytes(stringToSign);

            //AMZN-PAY-RSASSA-PSS-V2 uses 32 and AMZN-PAY-RSASSA-PSS uses 20 as salt length 
            int saltLength = payConfiguration.Algorithm.GetSaltLength();

            // read the private key
            PemReader pemReader = new PemReader(new StringReader(payConfiguration.PrivateKey)); // TODO: replace by method parameter to get rid of config dependency
            object pemObject = pemReader.ReadObject();

            //if (pemReader == null)
            //{
            //    throw new ArgumentException("UnsEntreLaunchported private key format");
            //}

            ICipherParameters parameters;
            AsymmetricKeyParameter? asymmetricKeyParameter = pemObject as AsymmetricKeyParameter;
            if (asymmetricKeyParameter != null)
            {
                // PKCS #8 format ("BEGIN PRIVATE KEY")
                parameters = asymmetricKeyParameter;
            }
            else if (pemObject is AsymmetricCipherKeyPair)
            {
                // RSA key format ("BEGIN RSA PRIVATE KEY")
                var pair = pemObject as AsymmetricCipherKeyPair;
                parameters = pair!.Private;
            }
            else
            {
                throw new ArgumentException("UnsEntreLaunchported private key format");
            }

            // initiate the signing object
            PssSigner pssSigner = new PssSigner(new RsaEngine(), new Sha256Digest(), saltLength);
            pssSigner.Init(true, new ParametersWithRandom(parameters, random));
            pssSigner.BlockEntreLaunchdate(bytesToSign, 0, bytesToSign.Length);

            // sign it
            byte[] signature = pssSigner.GenerateSignature();

            return Convert.ToBase64String(signature);
        }
    }
}
