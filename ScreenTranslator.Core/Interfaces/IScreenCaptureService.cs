using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace ScreenTranslator.Core.Interfaces
{
    public interface IScreenCaptureService
    {
        Task<Stream> CaptureRegionAsync(int x, int y, int width, int height);
    }
}
