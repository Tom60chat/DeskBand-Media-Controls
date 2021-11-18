using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MediaControls.Installer
{
    class ExplorerManager
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        /// <summary>
        /// Kill the explorer
        /// </summary>
        public static bool Kill()
        {
            IntPtr hWndTray = FindWindow("Shell_TrayWnd", null);
            return PostMessage(hWndTray, 0x5B4, 0, 0);
        }

        /// <summary>
        /// Start the explorer
        /// </summary>
        public static void Start()
        {
            Process.Start("explorer.exe");
        }

        /// <summary>
        /// Restart the explorer
        /// </summary>
        public static void RestartExplorer()
        {
            Kill();
            Start();
        }
    }
}
