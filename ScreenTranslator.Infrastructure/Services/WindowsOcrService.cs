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

        public async Task<string> RecognizeTextAsync(Stream imageStream, string languageCode = "ja", bool enablePreprocessing = false)
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

            // IMPROVEMENT v1.4.13: Binarization (Thresholding)
            if (enablePreprocessing)
            {
                ApplyBinarization(upscaledBitmap);
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

        private void ApplyBinarization(System.Drawing.Bitmap bitmap)
        {
            // Simple Thresholding: Convert to Grayscale -> Threshold
            // Using LockBits for performance
            var rect = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var data = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try
            {
                int bytes = Math.Abs(data.Stride) * bitmap.Height;
                byte[] rgbValues = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(data.Scan0, rgbValues, 0, bytes);

                // Threshold value (128 is standard, 160 works well for text on game backgrounds)
                // Let's use a slightly aggressive threshold to catch faint text
                byte threshold = 160; 

                for (int i = 0; i < rgbValues.Length; i += 4)
                {
                    // Format32bppArgb: Blue, Green, Red, Alpha
                    byte b = rgbValues[i];
                    byte g = rgbValues[i + 1];
                    byte r = rgbValues[i + 2];
                    
                    // Grayscale formula (standard luminance)
                    byte gray = (byte)(0.299 * r + 0.587 * g + 0.114 * b);

                    // Threshold
                    byte binary = (gray < threshold) ? (byte)0 : (byte)255; // Black text on White bg assumption? 
                    // Actually Windows OCR likes dark text on light bg or vice versa. 
                    // High contrast is key.

                    rgbValues[i] = binary;
                    rgbValues[i + 1] = binary;
                    rgbValues[i + 2] = binary;
                    // Leave alpha (i+3) alone or set to 255
                    rgbValues[i + 3] = 255;
                }

                System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, data.Scan0, bytes);
            }
            finally
            {
                bitmap.UnlockBits(data);
            }
        }
    }
}
