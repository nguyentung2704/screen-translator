using System.IO;
using System.Threading.Tasks;

namespace ScreenTranslator.Core.Interfaces
{
    public interface IOcrService
    {
        Task<string> RecognizeTextAsync(Stream imageStream, string languageCode = "ja");
    }
}
