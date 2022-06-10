using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Windows.Media.Control;
using TaskbarPosition = System.Windows.Forms.TaskbarPosition;

namespace MediaControls
{
    /// <summary>
    /// Logique d'interaction pour MediaSelectorWindow.xaml
    /// </summary>
    public partial class MediaSelectorWindow
    {
        public ObservableCollection<PlayerModel> PlayersItems { get; set; }
        public Point DeskBandPoint;
        private SessionManager sessionManager => SessionManager.Singleton;
        private CancellationTokenSource SessionsChangedTask_CancellationToken;

        public MediaSelectorWindow()
        {
            try
            {
                PlayersItems = new ObservableCollection<PlayerModel>(new List<PlayerModel>());
                InitializeComponent();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + Environment.NewLine + e.ToString(), "Media selector error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task Manager_SessionsChangedAsync(GlobalSystemMediaTransportControlsSessionManager sender, CancellationToken cancellationToken)
        {
            var height = Height;
            try
            {
                var sessions = sender.GetSessions();
                var players = new List<PlayerModel>();
                var currentSessionIndex = -1;
                int i = 0;

                foreach (var session in sessions)
                {
                    if (cancellationToken.IsCancellationRequested)
                        throw new TaskCanceledException();

                    var player = new PlayerModel
                    {
                        Name = session.SourceAppUserModelId,
                        Icon = null,
                        Title = string.Empty,
                        Cover = null,
                        Session = session
                    };

                    try
                    {
                        var processes = Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(session.SourceAppUserModelId));
                        if (processes.Length != 0)
                        {
                            var process = processes.First();
                            var main = process.MainModule;
                            var icon = System.Drawing.Icon.ExtractAssociatedIcon(main.FileName);

                            var file = FileVersionInfo.GetVersionInfo(main.FileName);

                            player.Name = file.ProductName;
                            player.Icon = ImageUtilities.ToImageSource(icon);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + Environment.NewLine + ex.ToString(), "Media selector error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    try
                    {
                        var properties = await session.TryGetMediaPropertiesAsync();
                        player.Title = properties.Title;
                        player.Cover = await properties.Thumbnail.ToImageSource();
                    }
                    catch { }

                    players.Add(player);

                    if (sessionManager.CurrentSession != null && session.SourceAppUserModelId == sessionManager.CurrentSession.SourceAppUserModelId)
                        currentSessionIndex = i;

                    i++;
                }

                if (cancellationToken.IsCancellationRequested)
                    throw new TaskCanceledException();

                PlayersItems.Clear();

                foreach (var player in players)
                    PlayersItems.Add(player);

                height = PlayersItems.Count * 44 + 4;
                height += 1 + 40; //separator.Height + uc_PlayersShortcut.Height;

                switch (UserControl1.Singleton.CurrentEdge)
                {
                    case TaskbarPosition.Bottom:
                        Top = Math.Abs(DeskBandPoint.Y) - height;
                        break;
                    case TaskbarPosition.Top:
                        Top = Math.Abs(DeskBandPoint.Y) + UserControl1.Singleton.ActualHeight;
                        break;

                    case TaskbarPosition.Right:
                    case TaskbarPosition.Left:
                        Top = Math.Abs(DeskBandPoint.Y);
                        break;
                }

                Height = height;
                lst_Player.SelectedItem = currentSessionIndex;
            }
            catch (TaskCanceledException) { }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + Environment.NewLine + e.ToString(), "Media selector error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                SessionsChangedTask_CancellationToken.Dispose();
                SessionsChangedTask_CancellationToken = null;
            }
        }

        private async void Manager_SessionsChanged(GlobalSystemMediaTransportControlsSessionManager sender, SessionsChangedEventArgs args)
        {
            try
            {
                if (SessionsChangedTask_CancellationToken != null)
                    SessionsChangedTask_CancellationToken.Cancel();

                while (SessionsChangedTask_CancellationToken != null)
                {
                    await Task.Delay(25);
                }

                SessionsChangedTask_CancellationToken = new CancellationTokenSource();

                await Dispatcher.Invoke(async () =>
                {
                    await Manager_SessionsChangedAsync(sender, SessionsChangedTask_CancellationToken.Token);
                });
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + Environment.NewLine + e.ToString(), "Media selector error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Make the menu update dynamically when the window is showed
            if ((bool)e.NewValue)
            {
                switch (UserControl1.Singleton.CurrentEdge)
                {
                    case TaskbarPosition.Top:
                    case TaskbarPosition.Bottom:
                        Left = Math.Abs(DeskBandPoint.X);
                        break;

                    case TaskbarPosition.Left:
                        Left = Math.Abs(DeskBandPoint.X) + UserControl1.Singleton.ActualWidth;
                        break;
                    case TaskbarPosition.Right:
                        Left = Math.Abs(DeskBandPoint.X) - ActualWidth;
                        break;
                }
                sessionManager.Manager.SessionsChanged += Manager_SessionsChanged;
                Manager_SessionsChanged(sessionManager.Manager, null);
            }
            else
            {
                sessionManager.Manager.SessionsChanged -= Manager_SessionsChanged;
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Hide();
        }

        private void lst_Player_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lst_Player.SelectedItem is PlayerModel player)
            {
                sessionManager.CurrentSession = player.Session;
                Hide();
            }
        }
    }
}
