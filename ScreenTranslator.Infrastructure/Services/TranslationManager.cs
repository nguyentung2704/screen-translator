using System.Threading.Tasks;
using ScreenTranslator.Core.Interfaces;

namespace ScreenTranslator.Infrastructure.Services
{
    public enum TranslatorProvider
    {
        Standard,
        AI
    }

    public class TranslationManager : ITranslationService
    {
        private readonly GoogleTranslationService _googleService;
        private readonly GeminiTranslationService _geminiService;
        
        public TranslatorProvider CurrentProvider { get; set; } = TranslatorProvider.Standard;

        public TranslationManager(GoogleTranslationService googleService, GeminiTranslationService geminiService)
        {
            _googleService = googleService;
            _geminiService = geminiService;
        }

        public void SetGeminiApiKey(string key)
        {
            _geminiService.SetApiKey(key);
        }

        public string GetApiKey() => _geminiService.GetApiKey();

        public Task<string> TranslateAsync(string text, string fromLanguage, string toLanguage)
        {
            if (CurrentProvider == TranslatorProvider.AI)
            {
                return _geminiService.TranslateAsync(text, fromLanguage, toLanguage);
            }
            return _googleService.TranslateAsync(text, fromLanguage, toLanguage);
        }
    }
}
