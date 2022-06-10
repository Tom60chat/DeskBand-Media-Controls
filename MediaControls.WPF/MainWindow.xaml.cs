using System;
using System.Timers;
using System.Windows;
using System.Windows.Interop;
using Taskbar = System.Windows.Forms.Taskbar;

namespace MediaControls.WPF
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            UserControl.SetCorner(true);

            Top = SystemParameters.WorkArea.Height;
            Left = SystemParameters.WorkArea.Left + 168;

            timer = new Timer();
            taskBarHandle = Taskbar.Handle; //WindowManagement.FindWindow("Shell_TrayWnd", null);
            windowIsShowed = IsActive;

            //var newTaskbarHandle = WindowManagement.FindWindowEx(taskBarHandle, IntPtr.Zero, "Windows.UI.Composition.DesktopWindowContentBridge", null);
            //var barHandle = WindowManagement.FindWindowEx(taskBarHandle, IntPtr.Zero, "ReBarWindow32", null);
        }

        private readonly Timer timer;
        private IntPtr windowHandle;
        private IntPtr taskBarHandle;
        private bool windowIsShowed;

        private void Time_Elapsed(object sender, ElapsedEventArgs e)
        {
            SetOnTopOfTaskBar();
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            windowHandle = new WindowInteropHelper(this).Handle;

            timer.Interval = 50;
            timer.Elapsed += Time_Elapsed;
            timer.Start();

            //SetOnTopOfTaskBar();

            //FocusMe.Focus(this);
        }

        private void SetOnTopOfTaskBar()
        {
            var test = Taskbar.CurrentBounds;
            var test2 = Taskbar.DisplayBounds;
            var test3 = Taskbar.Position;
            var test4 = Taskbar.AutoHide;
            var hided = WindowManagement.IsWindowVisible(taskBarHandle);

            if (hided)
            {
                if (!windowIsShowed)
                {
                    Dispatcher.Invoke(() => Show());
                    windowIsShowed = true;
                }

                //var newTaskbarHandle = WindowManagement.FindWindowEx(taskBarHandle, IntPtr.Zero, "Windows.UI.Composition.DesktopWindowContentBridge", null);
                //WindowManagement.SetParent(taskBarHandle, barHandle);

                WindowManagement.SetWindowPos(
                    windowHandle, new IntPtr(-1),
                    0, 0, 0, 0,
                    WindowManagement.SWP_NOMOVE | WindowManagement.SWP_NOSIZE | WindowManagement.SWP_SHOWWINDOW);
            }
            else
            {
                if (windowIsShowed)
                {
                    Dispatcher.Invoke(() => Hide());
                    windowIsShowed = false;
                }
            }
        }
    }
}
