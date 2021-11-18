using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Control;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace MediaControls.UWP
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class SessionMonitoring : Page
    {
        GlobalSystemMediaTransportControlsSessionManager manager;

        public SessionMonitoring()
        {
            InitializeComponent();

            Init();
        }

        async void Init()
        {
            manager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
            manager.CurrentSessionChanged += Manager_CurrentSessionChanged;
            manager.SessionsChanged += Manager_SessionsChanged;
        }

        private async void Manager_SessionsChanged(GlobalSystemMediaTransportControlsSessionManager sender, SessionsChangedEventArgs args)
        {
            var sessions = sender.GetSessions();
            var currentSession = sender.GetCurrentSession();

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Console.Text += $"New sessions: {sessions.Count}" + Environment.NewLine;

                foreach (var session in sessions)
                {
                    Console.Text += $"Session: {session.SourceAppUserModelId}" + Environment.NewLine;
                }

                Console.Text += sessions.ToList().Exists(x => x.SourceAppUserModelId == currentSession.SourceAppUserModelId) + Environment.NewLine;

                Console.Text += Environment.NewLine;
            });
        }

        private async void Manager_CurrentSessionChanged(GlobalSystemMediaTransportControlsSessionManager sender, CurrentSessionChangedEventArgs args)
        {
            var currentSession = sender.GetCurrentSession();

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                Console.Text += $"New session: {currentSession.SourceAppUserModelId}" + Environment.NewLine;

                var prop = await currentSession.TryGetMediaPropertiesAsync();

                Console.Text += $"AlbumArtist: {prop.AlbumArtist}, AlbumTitle: {prop.AlbumTitle}, AlbumTrackCount: {prop.AlbumTrackCount}, Artist: {prop.Artist}" +
                $", Genres: {prop.Genres}, PlaybackType: {prop.PlaybackType}, Subtitle: {prop.Subtitle}, Title: {prop.Title}, TrackNumber: {prop.TrackNumber}" + Environment.NewLine;
                Console.Text += Environment.NewLine;
            });
        }
    }
}
