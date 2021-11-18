using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Media.Control;

namespace MediaControls.DeskBand
{
    public class SessionManager
    {

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int GetWindowText(IntPtr hWnd, [Out] StringBuilder lpString, int nMaxCount);

        #region Constructors
        private SessionManager(GlobalSystemMediaTransportControlsSessionManager manager)
        {
            Singleton = this;
            Manager = manager;

            manager.SessionsChanged += (sender, args) => CheckCurrentSession();
            manager.CurrentSessionChanged += (sender, args) => CheckCurrentSession();
            //manager.CurrentSessionChanged += (sender, args) => UpdateCurrentSession();
        }
        #endregion

        #region Variables
        /// <summary>
        /// Invoke when the current player has changed, can be null
        /// </summary>
        public event EventHandler<GlobalSystemMediaTransportControlsSession> CurrentSessionChanged;
        private GlobalSystemMediaTransportControlsSession currentSession;
        #endregion

        #region Properties
        public static SessionManager Singleton { get; private set; }
        public GlobalSystemMediaTransportControlsSessionManager Manager { get; private set; }
        public GlobalSystemMediaTransportControlsSession CurrentSession
        {
            get => currentSession;
            set
            {
                currentSession = value;
                if (CurrentSessionChanged != null)
                    CurrentSessionChanged(this, currentSession);
            }
        }
        #endregion

        #region Methods
        public static async Task<SessionManager> InitializeAsync()
        {
            GlobalSystemMediaTransportControlsSessionManager manager = null;

            while (manager == null)
            {
                try
                {
                    manager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
                }
                catch
                {
                    await Task.Delay(500);
                }
            }

            var sessionManager = new SessionManager(manager);

            sessionManager.UpdateCurrentSession();

            return sessionManager;
        }

        private async void CheckCurrentSession()
        {
            var sessions = Manager.GetSessions();

            // Check if the current player is still alive
            if (CurrentSession != null)
            {
                if (!sessions.ToList().Exists(x => x.SourceAppUserModelId == CurrentSession.SourceAppUserModelId))
                {
                    // If the process still exist wait in case is loading the next content
                    var processes = Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(CurrentSession.SourceAppUserModelId));
                    if (processes.Length != 0)
                        await Task.Delay(10000);

                    UpdateCurrentSession();
                }
            }
            else
            {
                UpdateCurrentSession();
            }
        }

        private void UpdateCurrentSession()
        {
            CurrentSession = Manager.GetCurrentSession();
            /*
            // Get the process which is probably related to the session
            var prop = await CurrentSession.TryGetMediaPropertiesAsync();
            var processes = Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(CurrentSession.SourceAppUserModelId));

            foreach(var process in processes)
            {
                process.main
                if (process.ProcessName.Contains(prop.Title))
                {
                    CurrentProcess = process;
                    CurrentProcess.Exited += (s, e) => CurrentProcess = null;
                    //break;
                }
            }
            // And don't update is the process is still active (Like a YouTube tab that is not closed or that it loads the next content)
            */
        }
        #endregion
    }
}
