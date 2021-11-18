using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace MediaControls.Installer
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly Guid DllGuid = new Guid("77D175B4-0A80-4581-A28E-D17150AAE027");

        public MainWindow()
        {
            // Check version
            if ((Environment.OSVersion.Version.Major == 10 && Environment.OSVersion.Version.Build < 17763) ||
                Environment.OSVersion.Version.Major < 10)
            {
                MessageBox.Show("This app only works on Windows 10 version 1809 (build 17763) or newer", "App not compatible", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            InitializeComponent();
            UpdateUi();

            btn_install.Visibility = Visibility.Visible;
            txt_Progress.Visibility = Visibility.Collapsed;
        }

        private void UpdateUi()
        {
            if (DeskbandRegister.IsRegistered(DllGuid))
            {
                btn_install.Content = Properties.Resources.Uninstall;
                btnUpdate.Visibility = Installer.CanUpdate() ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                btn_install.Content = Properties.Resources.Install;
                btnUpdate.Visibility = Visibility.Collapsed;
            }
        }

        private async void btn_install_Click(object sender, RoutedEventArgs e)
        {
            UpdateUi();

            taskbar.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Indeterminate;
            btn_install.Visibility = Visibility.Collapsed;
            txt_Progress.Visibility = Visibility.Visible;
            txt_Progress.Text = "...";

            await Task.Run(() => {
                if (DeskbandRegister.IsRegistered(DllGuid))
                {
                    if (Uninstall())
                        MessageBox.Show(Properties.Resources.UninstallationSuccess, Properties.Resources.Uninstall);
                }
                else
                {
                    if (Install())
                        MessageBox.Show(Properties.Resources.InstallationSuccess, Properties.Resources.Install);
                }
            });

            taskbar.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
            btn_install.Visibility = Visibility.Visible;
            txt_Progress.Visibility = Visibility.Collapsed;

            UpdateUi();
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            taskbar.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Indeterminate;
            stkPnlButtons.Visibility = Visibility.Collapsed;
            txt_Progress.Visibility = Visibility.Visible;

            await Task.Run(() =>
            {
                if (Uninstall(false))
                {
                    if (Install())
                        MessageBox.Show(Properties.Resources.InstallationSuccess, Properties.Resources.Install);


                    if (!Installer.IsFileBusy)
                        ExplorerManager.Start();
                }
            });

            taskbar.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
            stkPnlButtons.Visibility = Visibility.Visible;
            txt_Progress.Visibility = Visibility.Collapsed;

            UpdateUi();
        }

        private bool Install()
        {
            bool success = false;
            Dispatcher.Invoke(() => txt_Progress.Text = Properties.Resources.Installing___);

            if (Installer.Install())
            {
                var domain = AppDomain.CreateDomain(nameof(DeskbandRegister), AppDomain.CurrentDomain.Evidence, AppDomain.CurrentDomain.SetupInformation /* new AppDomainSetup { ApplicationBase = Path.GetDirectoryName(typeof(DeskbandRegister).Assembly.Location) }*/);
                try
                {
                    var DLL = Installer.DLLs.Keys.First();
                    var DllFileName = Path.Combine(Installer.InstallPath, DLL);

                    var loader = (DeskbandRegister)domain.CreateInstanceAndUnwrap(typeof(DeskbandRegister).Assembly.FullName, typeof(DeskbandRegister).FullName);

                    if (loader.Register(DllFileName))
                        success = true;
                }
                finally
                {
                    AppDomain.Unload(domain);
                }
            }

            return success;
        }

        private bool Uninstall(bool RestartExplorer = true)
        {
            var fileBusy = Installer.IsFileBusy;
            bool Unregistered = false;
            var DLL = Installer.DLLs.Keys.First();
            var DllFileName = Path.Combine(Installer.InstallPath, DLL);

            Dispatcher.Invoke(() => txt_Progress.Text = Properties.Resources.Uninstalling___);

            if (fileBusy)
            {
                var reply = MessageBox.Show(Properties.Resources.CloseExplorerQuestion + Environment.NewLine + Properties.Resources.ContinueQuestion, Properties.Resources.UninstallationPrerequisites,
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (reply != MessageBoxResult.Yes)
                    return false;
            }

            if (File.Exists(DllFileName)) // If the dll has been install
            {
                // Unregister the intalled DLL
                var domain = AppDomain.CreateDomain(nameof(DeskbandRegister), AppDomain.CurrentDomain.Evidence, new AppDomainSetup { ApplicationBase = Path.GetDirectoryName(typeof(DeskbandRegister).Assembly.Location) });
                try
                {
                    var loader = (DeskbandRegister)domain.CreateInstanceAndUnwrap(typeof(DeskbandRegister).Assembly.FullName, typeof(DeskbandRegister).FullName);
                    Unregistered = loader.Unregister(DllFileName);
                }
                finally
                {
                    AppDomain.Unload(domain);
                }
            }
            else
            {
                // Unregister the embeded DLL
                var asm = Assembly.Load(Properties.Resources.MediaControls_DLL);
                Unregistered = DeskbandRegister.Unregister(asm);
            }


            if (Unregistered)
            {
                // We need to kill the explorer and wait to lets the explorer close to free the dll
                if (fileBusy)
                    ExplorerManager.Kill();
                else
                    RestartExplorer = false;

                Thread.Sleep(5000);
                var uninstalled = Installer.Uninstall();
                if (RestartExplorer)
                    ExplorerManager.Start();

                if (uninstalled)
                    return true;
            }

            return false;
        }
    }
}
