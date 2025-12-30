using System;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;
using ScreenTranslator.Core.Interfaces;

namespace ScreenTranslator.Infrastructure.Services
{
    public class NativeHotkeyService : IHotkeyService
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private IntPtr _handle;
        private HwndSource? _source;
        private int _currentId;
        private Action? _callback;

        public void Initialize(IntPtr windowHandle)
        {
            _handle = windowHandle;
            _source = HwndSource.FromHwnd(_handle);
            _source?.AddHook(HwndHook);
        }

        public void Register(ModifierKeys modifiers, Key key, Action callback)
        {
            if (_handle == IntPtr.Zero) throw new InvalidOperationException("HotkeyService not initialized with window handle.");

            _currentId++;
            _callback = callback;
            
            // Convert WPF modifiers to Win32 modifiers
            // MOD_ALT = 0x1, MOD_CONTROL = 0x2, MOD_SHIFT = 0x4, MOD_WIN = 0x8
            uint fsModifiers = 0;
            if ((modifiers & ModifierKeys.Alt) != 0) fsModifiers |= 0x1;
            if ((modifiers & ModifierKeys.Control) != 0) fsModifiers |= 0x2;
            if ((modifiers & ModifierKeys.Shift) != 0) fsModifiers |= 0x4;
            if ((modifiers & ModifierKeys.Windows) != 0) fsModifiers |= 0x8;

            uint vk = (uint)KeyInterop.VirtualKeyFromKey(key);

            RegisterHotKey(_handle, _currentId, fsModifiers, vk);
        }

        public void Unregister(int id)
        {
            if (_handle != IntPtr.Zero)
            {
                UnregisterHotKey(_handle, id);
            }
        }

        public void UnregisterAll()
        {
            if (_handle != IntPtr.Zero)
            {
                for (int i = 1; i <= _currentId; i++)
                {
                    UnregisterHotKey(_handle, i);
                }
                _currentId = 0;
            }
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            if (msg == WM_HOTKEY)
            {
                if (wParam.ToInt32() == _currentId)
                {
                    _callback?.Invoke();
                    handled = true;
                }
            }
            return IntPtr.Zero;
        }
    }
}
