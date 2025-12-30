using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using ScreenTranslator.Core.Interfaces;
using System.Runtime.InteropServices.WindowsRuntime;

namespace ScreenTranslator.Infrastructure.Services
{
    public class WindowsOcrService : IOcrService
    {
        private OcrEngine? _ocrEngine;

        public async Task<string> RecognizeTextAsync(Stream imageStream, string languageCode = "ja")
        {
            if (_ocrEngine == null || _ocrEngine.RecognizerLanguage.LanguageTag != languageCode)
            {
                var language = new Language(languageCode);
                if (!OcrEngine.IsLanguageSupported(language))
                {
                    // Fallback to first available or error
                    // For now, try to use installed language or throw
                    if (!OcrEngine.IsLanguageSupported(language))
                         throw new InvalidOperationException($"OCR language '{languageCode}' is not supported/installed on this system.");
                }
                _ocrEngine = OcrEngine.TryCreateFromLanguage(language);
            }

            if (_ocrEngine == null)
                throw new InvalidOperationException("Failed to create OCR Engine.");

            // Convert Stream to SoftwareBitmap
            using var randomAccessStream = imageStream.AsRandomAccessStream();
            var decoder = await BitmapDecoder.CreateAsync(randomAccessStream);
            using var softwareBitmap = await decoder.GetSoftwareBitmapAsync();

            var ocrResult = await _ocrEngine.RecognizeAsync(softwareBitmap);
            return ocrResult.Text;
        }
    }
}
