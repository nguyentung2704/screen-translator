using System;
using System.Windows.Input;

namespace ScreenTranslator.Core.Interfaces
{
    public interface IHotkeyService
    {
        void Register(ModifierKeys modifiers, Key key, Action callback);
        void UnregisterAll();
    }
}
