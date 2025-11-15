#pragma warning disable SYSLIB0014
using EntreLaunch.PayTabsHosted.API.AuthorizationToken;
using EntreLaunch.PayTabsHosted.API.DeliveryTracker;
using EntreLaunch.PayTabsHosted.API.Exceptions;
using EntreLaunch.PayTabsHosted.API.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using System.Net;
using System.Text;

namespace EntreLaunch.PayTabsHosted.API
{
    public abstract class Client 
    {
        protected ApiUrlBuilder apiUrlBuilder;
        protected ApiConfiguration payConfiguration;
        protected virtual ISignatureHelper SignatureHelper { get; private set; }
        protected CanonicalBuilder canonicalBuilder;
        protected Dictionary<string, List<string>> queryParametersMap = new Dictionary<string, List<string>>();

        protected Client(ApiConfiguration payConfiguration)
        {
            apiUrlBuilder = new ApiUrlBuilder(payConfiguration);

            this.payConfiguration = payConfiguration;
            canonicalBuilder = new CanonicalBuilder();
            SignatureHelper = new SignatureHelper(payConfiguration, canonicalBuilder);
        }

        //**************************************** Internal processing of requests ****************************************
        //**************************************** Internal processing of requests ****************************************

        /// <summary>
        /// 4
        /// Sends the API requests and processes the result by filling the PayTabsResponse object.
        /// </summary>
        protected virtual T ProcessRequest<T>(ApiRequest apiRequest)
            where T : PayTabsResponse, new()
        {
            var responseObject = new T();

            long startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            try
            {
                int retries = 0;

                while (retries <= payConfiguration.MaxRetries)
                {
                    using (HttpWebResponse httpWebResponse = SendRequestNoSigned(apiRequest))
                    using (StreamReader reader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8))
                    {
                        // if API call failed with a service error, try again
                        if (Constants.ServiceErrors.Values.Contains((int)httpWebResponse.StatusCode))
                        {
                            retries++;
                            int delay = Util.GetExponentialWaitTime(retries);
                            System.Threading.Thread.Sleep(delay);
                        }
                        // otherwise parse the result (even if it returned with 400 or any other "expected" error)
                        else
                        {
                            // if we have a response, deserialize it first as deserialization will overwrite any earlier assigned fields 
                            string response = reader.ReadToEnd();
                            if (!string.IsNullOrEmpty(response))
                            {
                                var dateConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyyMMddTHHmmssZ" };

                                JObject jsonResponse = JObject.Parse(response);
                                if (jsonResponse.ContainsKey("shipping_details"))
                                {
                                    JArray? shippingAddressList = jsonResponse["shipping_details"] as JArray;
                                    if (shippingAddressList != null)
                                    {
                                        for (int i = 0; i < shippingAddressList.Count; i++)
                                        {
                                            shippingAddressList[i] = JObject.Parse(shippingAddressList[i].ToString());
                                        }
                                    }
                                    response = jsonResponse.ToString();
                                }
                                responseObject = JsonConvert.DeserializeObject<T>(response, dateConverter);
                            }

                            // get the headers of the response objects
                            if(responseObject != null)
                            {
                                responseObject.Headers = new Dictionary<string, string>();


                                foreach (string key in httpWebResponse.Headers)
                                {
                                    responseObject.Headers.Add(key, httpWebResponse.Headers.Get(key) ?? string.Empty);

                                    if (key.Equals(Constants.Headers.RequestId, StringComparison.OrdinalIgnoreCase))
                                    {
                                        responseObject.RequestId = httpWebResponse.Headers[Constants.Headers.RequestId] ?? string.Empty;
                                    }
                                }
                            }

                            responseObject!.Code = (int)httpWebResponse.StatusCode;
                            responseObject.RawResponse = response;
                            responseObject.Url = apiRequest.Path;
                            responseObject.Method = apiRequest.HttpMethod;
                            responseObject.RawRequest = apiRequest.Body?.ToJson() ?? string.Empty;
                            responseObject.Retries = retries;

                            break;
                        }
                    }
                }

                responseObject.Duration = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - startTime;
            }
            catch (Exception ex)
            {
                throw new PayTabsClientException(ex.Message, ex);
            }

            return responseObject;
        }

        /// <summary>
        /// 5
        /// Sends the API requests and processes the result by filling the PayTabsResponse object.
        /// </summary>
        protected virtual T ProcessRequest<T>(ApiRequest apiRequest, Dictionary<string, string> postSignedHeaders)
            where T : PayTabsResponse, new()
        {
            var responseObject = new T();

            long startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            try
            {
                int retries = 0;

                while (retries <= payConfiguration.MaxRetries)
                {
                    using (HttpWebResponse httpWebResponse = SendRequest(apiRequest, postSignedHeaders))
                    using (StreamReader reader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8))
                    {
                        // if API call failed with a service error, try again
                        if (Constants.ServiceErrors.Values.Contains((int)httpWebResponse.StatusCode))
                        {
                            retries++;
                            int delay = Util.GetExponentialWaitTime(retries);
                            System.Threading.Thread.Sleep(delay);
                        }
                        // otherwise parse the result (even if it returned with 400 or any other "expected" error)
                        else
                        {
                            // if we have a response, deserialize it first as deserialization will overwrite any earlier assigned fields 
                            string response = reader.ReadToEnd();
                            if (!string.IsNullOrEmpty(response))
                            {
                                var dateConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyyMMddTHHmmssZ" };

                                JObject jsonResponse = JObject.Parse(response);
                                if (jsonResponse.ContainsKey("shipping_details"))
                                {
                                    JArray? shippingAddressList = jsonResponse["shipping_details"] as JArray;
                                    if (shippingAddressList != null)
                                    {
                                        for (int i = 0; i < shippingAddressList.Count; i++)
                                        {
                                            shippingAddressList[i] = JObject.Parse(shippingAddressList[i].ToString());
                                        }
                                    }
                                    response = jsonResponse.ToString();
                                }
                                responseObject = JsonConvert.DeserializeObject<T>(response, dateConverter);
                            }

                            // get the headers of the response objects
                            if (responseObject != null)
                            {
                                responseObject.Headers = new Dictionary<string, string>();
                                foreach (string key in httpWebResponse.Headers)
                                {
                                    responseObject.Headers.Add(key, httpWebResponse.Headers.Get(key) ?? string.Empty);

                                    if (key.Equals(Constants.Headers.RequestId, StringComparison.OrdinalIgnoreCase))
                                    {
                                        responseObject.RequestId = httpWebResponse.Headers.Get(Constants.Headers.RequestId) ?? string.Empty;
                                    }
                                }
                            }

                            responseObject!.Code = (int)httpWebResponse.StatusCode;
                            responseObject.RawResponse = response;
                            responseObject.Url = apiRequest.Path;
                            responseObject.Method = apiRequest.HttpMethod;
                            responseObject.RawRequest = apiRequest.Body?.ToJson() ?? string.Empty;
                            responseObject.Retries = retries;

                            break;
                        }
                    }
                }

                responseObject.Duration = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - startTime;
            }
            catch (Exception ex)
            {
                throw new PayTabsClientException(ex.Message, ex);
            }

            return responseObject;
        }

        /// <summary>
        /// 6
        /// Helper method to execute the request.
        /// </summary>
        protected virtual HttpWebResponse SendRequest(ApiRequest apiRequest, Dictionary<string, string> postSignedHeaders)
        {
            string path = apiRequest.Path.ToString();

            // add the query parameters to the URL, if there are any
            if (apiRequest.QueryParameters != null && apiRequest.QueryParameters.Count > 0)
            {
                var localCanonicalBuilder = new CanonicalBuilder();
                path += "?" + localCanonicalBuilder.GetCanonicalizedQueryString(apiRequest.QueryParameters);
            }

            // TODO: move setting of SecurityProtocol into constructor

            // ensure the right minimum TLS version is being used
            if (Util.IsObsoleteSecurityProtocol(ServicePointManager.SecurityProtocol))
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            }

            // TODO: consider switching to HttpClient class for web requests

            // create the web request
            HttpWebRequest? request = WebRequest.Create(path) as HttpWebRequest;
            if (request == null)
            {
                throw new PayTabsClientException("Http Request is empty");
            }

            foreach (KeyValuePair<string, string> header in postSignedHeaders)
            {
                if (WebHeaderCollection.IsRestricted(header.Key))
                {
                    switch (header.Key)
                    {
                        case "accept":
                            request.Accept = header.Value;
                            break;
                        case "content-type":
                            request.ContentType = header.Value;
                            break;
                        case "user-agent":
                            request.UserAgent = header.Value;
                            break;
                        default:
                            throw new PayTabsClientException("unknown header" + " " + header.Key);
                    }
                }
                else
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }
            request.Method = apiRequest.HttpMethod.ToString();

            if (apiRequest.HttpMethod != PayHttpMethod.GET)
            {
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(apiRequest.Body.ToJson());
                    streamWriter.Flush();
                }
            }

            HttpWebResponse? httpResponse;
            try
            {
                httpResponse = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException we)
            {
                httpResponse = we.Response as HttpWebResponse;

                if (httpResponse == null)
                {
                    throw new PayTabsClientException("Http Response is empty " + we);
                }
            }

            if (httpResponse == null)
            {
                throw new PayTabsClientException("Http Response is empty");
            }

            return httpResponse;
        }

        /// <summary>
        /// 7
        /// Helper method to execute the request.
        /// </summary>
        protected virtual HttpWebResponse SendRequestNoSigned(ApiRequest apiRequest)
        {
            string path = apiRequest.Path.ToString();

            // add the query parameters to the URL, if there are any
            if (apiRequest.QueryParameters != null && apiRequest.QueryParameters.Count > 0)
            {
                var localCanonicalBuilder = new CanonicalBuilder();
                path += "?" + localCanonicalBuilder.GetCanonicalizedQueryString(apiRequest.QueryParameters);
            }

            // TODO: move setting of SecurityProtocol into constructor

            // ensure the right minimum TLS version is being used
            if (Util.IsObsoleteSecurityProtocol(ServicePointManager.SecurityProtocol))
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            }

            // TODO: consider switching to HttpClient class for web requests

            // create the web request
            HttpWebRequest? request = WebRequest.Create(path) as HttpWebRequest;
            if(request == null)
            {
                throw new PayTabsClientException("Http Request is empty");
            }

            request.Method = apiRequest.HttpMethod.ToString();

            if (apiRequest.HttpMethod != PayHttpMethod.GET)
            {
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(apiRequest.Body.ToJson());
                    streamWriter.Flush();
                }
            }

            HttpWebResponse? httpResponse;
            try
            {
                httpResponse = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException we)
            {
                httpResponse = we.Response as HttpWebResponse;

                if (httpResponse == null)
                {
                    throw new PayTabsClientException("Http Response is empty " + we);
                }
            }

            if (httpResponse == null)
            {
                throw new PayTabsClientException("Http Response is empty");
            }

            return httpResponse;
        }


        //**************************************** Technical assistance ****************************************
        //**************************************** Technical assistance ****************************************

        /// <summary>
        /// 8
        /// Builds the authorization header.
        /// </summary>
        protected string BuildAuthorizationHeader(Dictionary<string, List<string>> preSignedHeaders, string signature)
        {
            StringBuilder authorizationBuilder = new StringBuilder(payConfiguration.Algorithm.GetName())
                    .Append(" client-key=").Append(payConfiguration.ClientKey).Append(", ").Append("SignedHeaders=")
                    .Append(canonicalBuilder.GetSignedHeadersString(preSignedHeaders))
                    .Append(", Signature=").Append(signature);

            return authorizationBuilder.ToString();
        }

        /// <summary>
        /// 9
        /// Builds the authorization header.
        /// </summary>
        protected string BuildAuthorizationHeader()
        {
            StringBuilder authorizationBuilder = new StringBuilder(payConfiguration.Algorithm.GetName())
                    .Append(payConfiguration.ServerKey);

            return authorizationBuilder.ToString();
        }


        /// <summary>
        /// 10
        /// Builds the authorization header.
        /// </summary>
        protected string BuildAuthorizationSignedHeader(Dictionary<string, List<string>> preSignedHeaders, string signature)
        {
            StringBuilder authorizationBuilder = new StringBuilder(payConfiguration.Algorithm.GetName())
                    .Append(" client-key=").Append(payConfiguration.ClientKey).Append(", ")
                    .Append(", signature=").Append(signature);

            return authorizationBuilder.ToString();
        }


        //****************************************  Delivery Tracking ****************************************
        //****************************************  Delivery Tracking ****************************************

        /// <summary>
        /// 11
        /// Sends delivery tracking information that will trigger a Alexa Delivery Notification when the item is shipped or about to be delivered.
        /// Shipment tracking information such as tracking number and shipping company is sent to PayTabs.
        /// </summary>
        public DeliveryTrackerResponse SendDeliveryTrackingInformation(DeliveryTrackerRequest deliveryTrackersRequest, Dictionary<string, string>? headers = null)
        {
            if (headers == null)
            {
                headers = new Dictionary<string, string>();
            }

            var apiUrl = apiUrlBuilder.BuildFullApiPath(Constants.ApiServices.Default, Constants.Resources.DeliveryTracker);
            var apiRequest = new ApiRequest(apiUrl, PayHttpMethod.POST, deliveryTrackersRequest, headers);

            var result = CallAPISigned<DeliveryTrackerResponse>(apiRequest);

            return result;
        }


        //**************************************** Submit General Requests ****************************************
        //**************************************** Submit General Requests ****************************************

        /// <summary>
        /// 2
        /// API to process the request and return the signed headers.
        /// send a request to PayTabs without an additional signature.
        /// </summary>
        public T CallAPI<T>(ApiRequest apiRequest) where T : PayTabsResponse, new()
        {
            if (apiRequest.Headers == null)
            {
                apiRequest.Headers = new Dictionary<string, string>();
            }

            if (apiRequest.HttpMethod == PayHttpMethod.POST && !apiRequest.Headers.ContainsKey(Constants.Headers.AuthorizationKey))
            {
                apiRequest.Headers.Add(Constants.Headers.AuthorizationKey, payConfiguration.ServerKey);
            }

            return ProcessRequest<T>(apiRequest);
        }

        /// <summary>
        /// 1
        /// API to process the request and return the signed headers.
        /// send a request to PayTabs with a digital signature to ensure security.
        /// </summary>
        public T CallAPISigned<T>(ApiRequest apiRequest) where T : PayTabsResponse, new()
        {
            if (apiRequest.Headers == null)
            {
                apiRequest.Headers = new Dictionary<string, string>();
            }

            // for POST calls, add an authorization key if it hasn't been provided yet
            if (apiRequest.HttpMethod == PayHttpMethod.POST && !apiRequest.Headers.ContainsKey(Constants.Headers.AuthorizationKey))
            {
                // remove dashes from GUID as these aren't sEntreLaunchported characters for the idempotency Key
                apiRequest.Headers.Add(Constants.Headers.ClientKey, payConfiguration.ClientKey);
            }

            Dictionary<string, string> postSignedHeaders = SignRequest(apiRequest);

            return ProcessRequest<T>(apiRequest, postSignedHeaders);
        }

        /// <summary>
        /// 3
        /// Signs the request provided and returns the signed headers map.
        /// This method add a digital signature to the request using SignatureHelper.
        /// </summary>
        internal Dictionary<string, string> SignRequest(ApiRequest request)
        {
            Dictionary<string, List<string>> preSignedHeaders = SignatureHelper.CreateDefaultHeaders(request.Path);

            if (request.Headers.Count > 0)
            {
                foreach (KeyValuePair<string, string> header in request.Headers)
                {
                    preSignedHeaders[header.Key.ToLower()] = new List<string>
                     {
                         header.Value
                     };
                }
            }

            string canonicalRequest = SignatureHelper.CreateCanonicalRequest(request, preSignedHeaders);
            string stringToSign = SignatureHelper.CreateStringToSign(canonicalRequest);
            string signature = SignatureHelper.GenerateSignature(stringToSign);

            Dictionary<string, string> postSignedHeadersMap = new Dictionary<string, string>();

            foreach (string key in preSignedHeaders.Keys)
            {
                postSignedHeadersMap.Add(key.ToLower(), preSignedHeaders[key][0]);
            }

            string authorizationHeader = BuildAuthorizationSignedHeader(preSignedHeaders, signature);
            postSignedHeadersMap.Add("authorization", authorizationHeader);

            string userAgent = Util.BuildUserAgentHeader();
            postSignedHeadersMap.Add("user-agent", userAgent);

            return postSignedHeadersMap;
        }


        //**************************************** Managing Tokens ****************************************
        //**************************************** Managing Tokens ****************************************

        /// <summary>
        /// 12
        /// Retrieves a delegated authorization token used in order to make API calls on behalf of a merchant.
        /// Used to obtain an authorization token that can be used to connect to PayTabs servers on behalf of the merchant.
        /// </summary>
        /// <returns>HS256 encoded JWT Token that will be used to make V2 API calls on behalf of the merchant.</returns>
        public AuthorizationTokenResponse GetAuthorizationToken(AuthorizationTokenRequest request, Dictionary<string, string>? headers = null)
        {
            if (headers == null)
            {
                headers = new Dictionary<string, string>();
            }

            var apiUrl = apiUrlBuilder.BuildFullApiPath(Constants.ApiServices.Default, Constants.Resources.Request);
            var apiRequest = new ApiRequest(apiUrl, PayHttpMethod.POST, request, headers);

            var result = CallAPISigned<AuthorizationTokenResponse>(apiRequest);

            return result;
        }

        /// <summary>
        /// 13
        /// Retrieves a delegated authorization token used in order to make API calls on behalf of a merchant.
        /// Similar to the previous function but is used for a token related to invoice management.
        /// </summary>
        public AuthorizationTokenResponse GetInvoiceAuthorizationToken(AuthorizationTokenRequest body, Dictionary<string, string>? headers = null)
        {
            if (headers == null)
            {
                headers = new Dictionary<string, string>();
            }

            var apiUrl = apiUrlBuilder.BuildFullApiPath(Constants.ApiServices.Default, Constants.Resources.Invoice);
            var apiRequest = new ApiRequest(apiUrl, PayHttpMethod.POST, body, headers);

            var result = CallAPISigned<AuthorizationTokenResponse>(apiRequest);

            return result;
        }

        /// <summary>
        /// 14
        /// Retrieves a token used to make API calls on behalf of the merchant.
        ///  Used to query the status of an existing token (such as checking its validity or information).
        /// </summary>
        public QueryTokenResponse TokenQuery(QueryTokenRequest request, Dictionary<string, string>? headers = null)
        {
            if (headers == null)
            {
                headers = new Dictionary<string, string>();
            }

            var apiUrl = apiUrlBuilder.BuildFullApiPath(Constants.ApiServices.Default, Constants.Resources.Token);
            var apiRequest = new ApiRequest(apiUrl, PayHttpMethod.GET, request, headers);
            apiRequest.Headers = headers;

            var result = CallAPI<QueryTokenResponse>(apiRequest);

            return result;
        }

        /// <summary>
        /// 15
        ///  Used to cancel or delete an existing token.
        /// </summary>
        public RevokeTokenResponse RevokeToken(RevokeTokenRequest request, Dictionary<string, string>? headers = null)
        {
            if (headers == null)
            {
                headers = new Dictionary<string, string>();
            }

            var apiUrl = apiUrlBuilder.BuildFullApiPath(Constants.ApiServices.Default, Constants.Resources.Token, "", Constants.Methods.Delete);
            var apiRequest = new ApiRequest(apiUrl, PayHttpMethod.POST, request, headers!);

            var result = CallAPI<RevokeTokenResponse>(apiRequest);

            return result;
        }

        /// <summary>
        /// 16
        /// Initiating Payment Hosted Page.
        /// </summary>
        //public InitiatingPaymentHostedPageResponse InitiatingPaymentHostedPage(InitiatingHostedPaymentPageRequest body, Dictionary<string, string> headers = null)
        //{
        //    var apiUrl = apiUrlBuilder.BuildFullApiPath(Constants.ApiServices.Default);
        //    var apiRequest = new ApiRequest(apiUrl, PayHttpMethod.POST, body, headers);

        //    var result = CallAPI<InitiatingPaymentHostedPageResponse>(apiRequest);

        //    return result;
        //}
    }
}
#pragma warning restore SYSLIB0014
