using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ManagmentDS
{
    public sealed class GlobalHotkey : IDisposable
    {
        private readonly int _id;
        private readonly IntPtr _handle;

        public event EventHandler HotkeyPressed;

        public GlobalHotkey(Keys key, KeyModifiers modifiers, Form form)
        {
            _id = GetHashCode();
            _handle = form.Handle;

            if (!RegisterHotKey(_handle, _id, (uint)modifiers, (uint)key))
                throw new InvalidOperationException("Nie można zarejestrować skrótu klawiszowego.");
        }

        public void ProcessMessage(ref Message m)
        {
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == _id)
                HotkeyPressed?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            UnregisterHotKey(_handle, _id);
        }

        private const int WM_HOTKEY = 0x0312;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(
            IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(
            IntPtr hWnd, int id);
    }

    [Flags]
    public enum KeyModifiers
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        Win = 8
    }
}
