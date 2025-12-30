using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ScreenTranslator.Core.Interfaces;

namespace ScreenTranslator.Infrastructure.Services
{
    public class GoogleTranslationService : ITranslationService
    {
        private readonly HttpClient _httpClient;
        // Note: In a real app, use a proper API Key or a more robust client. 
        // This uses the basic public endpoint structure which might be rate limited.
        private const string BaseUrl = "https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}";

        public GoogleTranslationService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        }

        public async Task<string> TranslateAsync(string text, string fromLanguage, string toLanguage)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            try 
            {
                var url = string.Format(BaseUrl, fromLanguage, toLanguage, Uri.EscapeDataString(text));
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return $"[Translation Error: {response.StatusCode}]";
                }

                var json = await response.Content.ReadAsStringAsync();
                
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
                {
                    var firstBlock = root[0];
                    if (firstBlock.ValueKind == JsonValueKind.Array)
                    {
                        var sb = new System.Text.StringBuilder();
                        foreach (var segment in firstBlock.EnumerateArray())
                        {
                            if (segment.ValueKind == JsonValueKind.Array && segment.GetArrayLength() > 0)
                            {
                                sb.Append(segment[0].GetString());
                            }
                        }
                        var result = sb.ToString();
                        return string.IsNullOrWhiteSpace(result) ? "[Error: Empty translation result]" : result;
                    }
                }
            }
            catch (Exception ex)
            {
                return $"[Translation Error: {ex.Message}]";
            }

            return "[Error: Unexpected translation format]";
        }
    }
}
