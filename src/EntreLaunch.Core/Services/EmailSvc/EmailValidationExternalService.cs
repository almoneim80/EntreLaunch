namespace EntreLaunch.Services.EmailSvc
{
    public class EmailValidationExternalService : IEmailValidationExternalService
    {
        protected static readonly JsonSerializerOptions SerializeOptions = new JsonSerializerOptions();
        private readonly IOptions<EmailVerificationApiConfig> emailVerificationApiConfig;
        private readonly ILogger<EmailValidationExternalService> _logger;
        public EmailValidationExternalService(IOptions<EmailVerificationApiConfig> emailVerificationApiConfig, ILogger<EmailValidationExternalService> logger)
        {
            this.emailVerificationApiConfig = emailVerificationApiConfig;

            // Configure JSON serialization options if they are not already configured
            if (SerializeOptions.PropertyNamingPolicy == null)
            {
                JsonHelper.Configure(SerializeOptions, JsonNamingConvention.CamelCase);
            }

            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<GeneralResult<EmailVerifyInfoDto>> Validate(string email)
        {
            try
            {
                // Get the URL and API Key from Settings
                var apiUrl = emailVerificationApiConfig.Value.Url;
                var apiKey = emailVerificationApiConfig.Value.ApiKey;

                // Create an HTTP client and set EntreLaunch the header
                var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Setting EntreLaunch query parameters
                var queryParams = new Dictionary<string, string>
                {
                    ["apiKey"] = apiKey,
                    ["emailAddress"] = email,
                };

                // Send a GET request to the API
                var response = await client.GetAsync(QueryHelpers.AddQueryString(apiUrl, queryParams!));

                if (response.IsSuccessStatusCode)
                {
                    // Analyze the successful response
                    var emailVerify = JsonSerializer.Deserialize<EmailVerifyInfoDto>(response.Content.ReadAsStringAsync().Result, SerializeOptions);
                    Log.Information("Success of resolving {0}", emailVerify!.EmailAddress!);
                    return new GeneralResult<EmailVerifyInfoDto>(true, "email is valid", emailVerify);
                }
                else
                {
                    // throw an exception in case of failure
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return new GeneralResult<EmailVerifyInfoDto>(false, responseContent, null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating email: {Message}", ex.Message);
                return new GeneralResult<EmailVerifyInfoDto>(false, ex.Message, null);
            }
        }
    }
}
