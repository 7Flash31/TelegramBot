using System;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Text.Json;
using Newtonsoft.Json;
class GPTBot
{
    public static async Task<string> SendCompletionRequest(string question)
    {
        var prompt = new
        {
            modelUri = "gpt://b1gt4nscm4mtcv853jvr/yandexgpt-lite",
            completionOptions = new
            {
                stream = false,
                temperature = 0.6,
                maxTokens = "2000"
            },
            messages = new[]
            {
                new { role = "user", text = $"{question}" },
            }
        };

        var url = "https://llm.api.cloud.yandex.net/foundationModels/v1/completion";
        var apiKey = "AQVNyn0pq-53tMTeBI7FQk1clrHQmz7e6nhBacY_";

        using(var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Api-Key", apiKey);

            var content = new StringContent(JsonConvert.SerializeObject(prompt), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);

            if(response.IsSuccessStatusCode)
            {
                var resultJson = await response.Content.ReadAsStringAsync();
                var resultDocument = JsonDocument.Parse(resultJson);
                var textProperty = resultDocument.RootElement.GetProperty("result")
                    .GetProperty("alternatives")[0]
                    .GetProperty("message")
                    .GetProperty("text");

                return textProperty.GetString();
            }
            else
            {
                return $"Error: {response.StatusCode}";
            }
        }
    }
}

class Result
{
    [JsonPropertyName("alternatives")]
    public Alternatives Alternatives { get; set; } = new();

    [JsonPropertyName("usage")]
    public Usage Usage { get; set; } = new();

}

class Alternatives
{
    [JsonPropertyName("message")]
    public Message Message { get; set; } = new();

    [JsonPropertyName("status")]
    public string Status { get; set; } = "";
}

class Usage
{
    [JsonPropertyName("inputTextTokens")]
    public string InputTextTokens { get; set; } = "";

    [JsonPropertyName("completionTokens")]
    public string CompletionTokens { get; set; } = "";

    [JsonPropertyName("totalTokens")]
    public string TotalTokens { get; set; } = "";
}

class Message
{
    [JsonPropertyName("role")]
    public string Role { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; }
}

