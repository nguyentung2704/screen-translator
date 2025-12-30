using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ScreenTranslator.Core.Interfaces;

namespace ScreenTranslator.Infrastructure.Services
{
    public class GeminiTranslationService : ITranslationService
    {
        private readonly HttpClient _httpClient;
        private string _apiKey = string.Empty;

        public GeminiTranslationService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        }

        public void SetApiKey(string apiKey)
        {
            _apiKey = apiKey;
        }

        public string GetApiKey() => _apiKey;

        public async Task<string> TranslateAsync(string text, string fromLanguage, string toLanguage)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            if (string.IsNullOrWhiteSpace(_apiKey)) return "[AI Error: Gemini API Key not set]";

            var url = $"https://generativelanguage.googleapis.com/v1/models/gemini-2.0-flash:generateContent?key={_apiKey}";

            var prompt = $"Translate the following text from {fromLanguage} to {toLanguage}. Output only the translated text, no explanations.\n\nText: {text}";

            var requestBody = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = prompt } } }
                }
            };

            int maxRetries = 3;
            int delayMs = 2000;

            for (int retry = 0; retry <= maxRetries; retry++)
            {
                try
                {
                    var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync(url, content);
                    
                    if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        if (retry < maxRetries)
                        {
                            await Task.Delay(delayMs * (retry + 1));
                            continue;
                        }
                        return "[AI Error: Too Many Requests. Please wait a minute and try again.]";
                    }

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorBody = await response.Content.ReadAsStringAsync();
                        return $"[AI Error: {response.StatusCode} - {errorBody}]";
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);
                    var root = doc.RootElement;
                    
                    var candidates = root.GetProperty("candidates");
                    if (candidates.GetArrayLength() > 0)
                    {
                        var firstCandidate = candidates[0];
                        var contentObj = firstCandidate.GetProperty("content");
                        var parts = contentObj.GetProperty("parts");
                        if (parts.GetArrayLength() > 0)
                        {
                            return parts[0].GetProperty("text").GetString()?.Trim() ?? string.Empty;
                        }
                    }
                    break; // Exit loop if successful but unexpected format (handled below)
                }
                catch (Exception ex)
                {
                    if (retry == maxRetries) return $"[AI Error: {ex.Message}]";
                    await Task.Delay(delayMs);
                }
            }

            return "[AI Error: Unexpected response format]";
        }
    }
}
