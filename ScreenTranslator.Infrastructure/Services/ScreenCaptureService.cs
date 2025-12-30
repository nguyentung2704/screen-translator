using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using ScreenTranslator.Core.Interfaces;

namespace ScreenTranslator.Infrastructure.Services
{
    public class ScreenCaptureService : IScreenCaptureService
    {
        public Task<Stream> CaptureRegionAsync(int x, int y, int width, int height)
        {
            return Task.Run(() =>
            {
                // Ensure valid bounds
                if (width <= 0 || height <= 0) return Stream.Null;

                using var bitmap = new Bitmap(width, height);
                using (var g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(x, y, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
                }

                var ms = new MemoryStream();
                bitmap.Save(ms, ImageFormat.Png);
                ms.Position = 0;
                return (Stream)ms;
            });
        }
    }
}
