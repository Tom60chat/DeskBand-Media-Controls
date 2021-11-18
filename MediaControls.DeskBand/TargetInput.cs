using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MediaControls.DeskBand
{
    class TargetInput
    {
        public const uint WM_KEYDOWN = 0x0100;
        public const uint WM_CHAR = 0x0102;
        public const uint WM_KEYUP = 0x0101;
        // https://docs.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes
        public const long VK_RETURN = 0x0D;
        public const long VK_CONTROL = 0x11;
        public const long VK_UP = 0x26;
        public const long VK_DOWN = 0x28;

        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        public static bool SendInput(IntPtr handle, uint action, long key) => PostMessage(handle, action, new IntPtr(key), new IntPtr(0));
    }
}
