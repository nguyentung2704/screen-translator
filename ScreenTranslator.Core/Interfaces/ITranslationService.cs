using System.Threading.Tasks;

namespace ScreenTranslator.Core.Interfaces
{
    public interface ITranslationService
    {
        Task<string> TranslateAsync(string text, string fromLanguage, string toLanguage);
    }
}
