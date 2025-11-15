using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace EntreLaunch.Plugin.AI.Service;

public class DifyApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public DifyApiClient(string apiKey)
    {
        _httpClient = new HttpClient();
        _apiKey = apiKey;
    }

    /// <summary>
    /// Generate a Quiz usign Dify API.
    /// </summary>
    public async Task<string> GenerateQuizAsync(string topic, string userId, int questionsNumber, int min, int max)
    {
        // Validate input
        if (min >= max)
        {
            return "Error: Minimum mark (minMark) must be less than Maximum mark (maxMark).";
        }

        // Configure the data structure (Body)
        var requestBody = new
        {
            inputs = new
            {
                topic_value = topic,
                Q_number = questionsNumber,
                minMark = min,
                maxMark = max
            },
            response_mode = "blocking",
            user = userId
        };

        // Convert the Body to JSON
        string requestJson = JsonSerializer.Serialize(requestBody);

        // Setting EntreLaunch the POST message and attaching the JSON to the content
        var requestMessage = new HttpRequestMessage(HttpMethod.Post,
            "https://api.dify.ai/v1/completion-messages")
        {
            Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
        };

        // Add authentication header
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        try
        {
            // Sending a request
            var response = await _httpClient.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return $"Error: {response.StatusCode}, Details: {errorContent}";
            }

            // Read the content of the reply
            var responseContent = await response.Content.ReadAsStringAsync();

            // Extract the “answer” field from the response
            using var doc = JsonDocument.Parse(responseContent);
            var root = doc.RootElement;

            // Check for the presence of the “answer” field
            if (root.TryGetProperty("answer", out var answerElement))
            {
                string rawAnswer = answerElement.GetString() ?? "";
                string cleanedAnswer = ExtractCleanJson(rawAnswer);

                // converting the cleaned text into a JSON object 
                try
                {
                    var jsonDoc = JsonDocument.Parse(cleanedAnswer);
                    return jsonDoc.RootElement.GetRawText();
                }
                catch (JsonException ex)
                {
                    return $"Error: Failed to parse the returned JSON. Details: {ex.Message}";
                }
            }
            else
            {
                return "Error: No 'answer' field found in the API response.";
            }
        }
        catch (Exception ex)
        {
            return $"Error: An exception occurred. Details: {ex.Message}";
        }
    }

    /// <summary>
    /// Extract the text between ``json`` and ```.
    /// </summary>
    private string ExtractCleanJson(string input)
    {
        // Locate the JSON content within the raw string
        int startIndex = input.IndexOf("```json") + 7; // Skip "```json"
        int endIndex = input.LastIndexOf("```");

        if (startIndex >= 7 && endIndex > startIndex)
        {
            return input.Substring(startIndex, endIndex - startIndex).Trim();
        }

        // If no valid JSON markers are found, return the original input
        return input;
    }

    /// <summary>
    /// Generate a Quiz usign Dify API from text.
    /// </summary>
    public async Task<string> GenerateQuizFromTextAsync(string content, string userId, int questionsNumber, int min, int max)
    {
        // Validate input
        if (min >= max)
        {
            return "Error: Minimum mark (minMark) must be less than Maximum mark (maxMark).";
        }
        if (min <= 0)
        {
            return "Error: Minimum mark (minMark) must be greater than or equal to 1.";
        }
        if (max <= 0)
        {
            return "Error: Maximum mark (maxMark) must be greater than or equal to 1.";
        }

        // Prepare request body
        var requestBody = new
        {
            inputs = new
            {
                topic_value = content,
                Q_number = questionsNumber,
                minMark = min,
                maxMark = max
            },
            response_mode = "blocking",
            user = userId
        };

        string requestJson = JsonSerializer.Serialize(requestBody);

        // Set EntreLaunch the POST request
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.dify.ai/v1/completion-messages")
        {
            Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
        };

        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        try
        {
            // Send the request
            var response = await _httpClient.SendAsync(requestMessage);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return $"Error: {response.StatusCode}, Details: {errorContent}";
            }

            // Read response content
            var responseContent = await response.Content.ReadAsStringAsync();

            // Parse the JSON response
            using var doc = JsonDocument.Parse(responseContent);
            var root = doc.RootElement;

            if (root.TryGetProperty("answer", out var answerElement))
            {
                string rawAnswer = answerElement.GetString() ?? "";
                string cleanedAnswer = ExtractCleanJson(rawAnswer);

                // Validate and return cleaned JSON
                try
                {
                    var jsonDoc = JsonDocument.Parse(cleanedAnswer);
                    return jsonDoc.RootElement.GetRawText(); // Return the formatted JSON
                }
                catch (JsonException ex)
                {
                    return $"Error: Failed to parse the returned JSON. Details: {ex.Message}";
                }
            }
            else
            {
                return "Error: No 'answer' field found in the API response.";
            }
        }
        catch (Exception ex)
        {
            return $"Error: An exception occurred. Details: {ex.Message}";
        }
    }

    /// <summary>
    /// (اختياري) طريقة لتصحيح الإجابات باستخدام dify.ai.
    /// يمكنك تطويرها لاحقًا بناءً على متطلباتك وتوثيق dify.ai.
    /// </summary>
    public async Task<string> CorrectAnswerAsync(string questionText, string userAnswer)
    {
        var requestUrl = "https://api.dify.ai/correct-answer";

        var payload = new
        {
            question = questionText,
            answer = userAnswer
        };

        var jsonPayload = JsonSerializer.Serialize(payload);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(requestUrl, content);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        return result;
    }
}
