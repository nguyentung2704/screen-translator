using System;
using System.Threading.Tasks;

namespace ScreenTranslator.Core.Interfaces
{
    public interface ISpeechService : IDisposable
    {
        event EventHandler<string> SpeechRecognized;
        Task StartContinuousRecognitionAsync(string languageCode = "ja-JP");
        Task StopContinuousRecognitionAsync();
    }
}
