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
                    if (!OcrEngine.IsLanguageSupported(language))
                         throw new InvalidOperationException($"OCR language '{languageCode}' is not supported/installed on this system.");
                }
                _ocrEngine = OcrEngine.TryCreateFromLanguage(language);
            }

            if (_ocrEngine == null)
                throw new InvalidOperationException("Failed to create OCR Engine.");

            // IMPROVEMENT v1.3.0: Image Pre-processing (Upscaling)
            // 1. Load stream into System.Drawing.Bitmap
            using var originalBitmap = new System.Drawing.Bitmap(imageStream);
            
            // 2. Define Scale Factor (2.0x is a sweet spot for performance/accuracy)
            double scaleFactor = 2.0; 
            int newWidth = (int)(originalBitmap.Width * scaleFactor);
            int newHeight = (int)(originalBitmap.Height * scaleFactor);

            // 3. Create upscaled bitmap
            using var upscaledBitmap = new System.Drawing.Bitmap(newWidth, newHeight);
            using (var graphics = System.Drawing.Graphics.FromImage(upscaledBitmap))
            {
                // High Quality settings for text preservation
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.DrawImage(originalBitmap, 0, 0, newWidth, newHeight);
            }

            // 4. Convert System.Drawing.Bitmap to SoftwareBitmap (UWP)
            // We need an intermediate stream for this conversion
            using var memoryStream = new MemoryStream();
            upscaledBitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            memoryStream.Position = 0;

            var decoder = await BitmapDecoder.CreateAsync(memoryStream.AsRandomAccessStream());
            using var softwareBitmap = await decoder.GetSoftwareBitmapAsync();

            try 
            {
                var ocrResult = await _ocrEngine.RecognizeAsync(softwareBitmap);
                return ocrResult.Text;
            }
            catch (Exception)
            {
                // Fallback (e.g., if image is too large) - unlikely for screen captures but good safety
                return string.Empty;
            }
        }
    }
}
