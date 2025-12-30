using System.Windows.Input;

namespace ScreenTranslator.Core.Models
{
    public class AppSettings
    {
        public string GeminiApiKey { get; set; } = string.Empty;
        public ModifierKeys HotkeyModifiers { get; set; } = ModifierKeys.Control | ModifierKeys.Shift;
        public Key HotkeyKey { get; set; } = Key.D;
        public string SourceLanguage { get; set; } = "ja";
        public string TargetLanguage { get; set; } = "vi";
        public string TranslatorProvider { get; set; } = "Standard";
    }
}
