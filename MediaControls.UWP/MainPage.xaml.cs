using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Control;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MediaControls.UWP
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private GlobalSystemMediaTransportControlsSessionManager manager;
        private GlobalSystemMediaTransportControlsSession currentSession;
        private GlobalSystemMediaTransportControlsSessionPlaybackInfo lastPlayBackInfo;

        public MainPage()
        {
            this.InitializeComponent();

                txt_Title.Text = "No player";
                txt_AlbumArtist.Text = string.Empty;

            Init();
        }

        private async Task Init()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                btn_PlayPause.IsEnabled = btn_Next.IsEnabled = btn_Previous.IsEnabled = (bool)(btn_PlayPause.Tag = btn_Next.Tag = btn_Previous.Tag = false));

            var manager = await GetManager();

            Manager_CurrentSessionChanged(manager, null);

            manager.SessionsChanged += Manager_SessionsChanged;
        }

        private async Task<GlobalSystemMediaTransportControlsSessionManager> GetManager()
        {
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
            return manager;
        }

        private GlobalSystemMediaTransportControlsSession GetCurrentMusicPlaybackSession()
        {
            var currentSession = manager.GetCurrentSession();
            //MessageBox.Show(currentSession.GetPlaybackInfo().PlaybackType.ToString());
            if (currentSession != null && currentSession.GetPlaybackInfo().PlaybackType == Windows.Media.MediaPlaybackType.Music)
                return currentSession;

            var sessions = manager.GetSessions().ToList();
            if (sessions.Count != 0)
            {
                var musicSessions = sessions.FindAll(x => x.GetPlaybackInfo().PlaybackType == Windows.Media.MediaPlaybackType.Music);
                if (musicSessions.Count != 0)
                    return musicSessions.FirstOrDefault();
                else
                    return sessions.FirstOrDefault();
            }
            else
                return null;
        }

        private void Manager_SessionsChanged(GlobalSystemMediaTransportControlsSessionManager sender, SessionsChangedEventArgs args)
        {
            Manager_CurrentSessionChanged(sender, null);
        }

        private void Manager_CurrentSessionChanged(GlobalSystemMediaTransportControlsSessionManager sender, CurrentSessionChangedEventArgs args)
        {
            if (currentSession != null)
            {
                currentSession.MediaPropertiesChanged -= Current_MediaPropertiesChanged;
                currentSession.PlaybackInfoChanged -= Current_PlaybackInfoChanged;
            }

            currentSession = GetCurrentMusicPlaybackSession();

            if (currentSession != null)
            {
                currentSession.MediaPropertiesChanged += Current_MediaPropertiesChanged;
                currentSession.PlaybackInfoChanged += Current_PlaybackInfoChanged;

                Current_MediaPropertiesChanged(currentSession, null);
                Current_PlaybackInfoChanged(currentSession, null);
            }
        }

        private async void Current_MediaPropertiesChanged(GlobalSystemMediaTransportControlsSession sender, MediaPropertiesChangedEventArgs args)
        {
            try
            {
                var properties = await sender.TryGetMediaPropertiesAsync();
                var info = sender.GetPlaybackInfo();

                if (info.PlaybackType != Windows.Media.MediaPlaybackType.Music)
                    return;

                if (properties != null)
                {
                    // UWP Thumbnail Stream to Bitmap
                    if (properties.Thumbnail != null)
                    {
                        var read = await properties.Thumbnail.OpenReadAsync();

                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            BitmapImage cover = new BitmapImage();
                            cover.SetSource(read);
                            Img_AlbumCover.Source = cover;
                        });
                    }

                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        txt_Title.Text = properties.Title;
                        txt_AlbumArtist.Text = properties.AlbumArtist;

                        Grd_Main.Visibility = Visibility.Visible;
                    });
                }
                else
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        Img_AlbumCover.Source = null;
                        txt_Title.Text = "No info";
                        txt_AlbumArtist.Text = string.Empty;

                        Grd_Main.Visibility = Visibility.Visible;
                    });
                }
            }
            catch (Exception e)
            {
                if (e.HResult == unchecked((int)0x800706BA))
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        Img_AlbumCover.Source = null;
                        txt_Title.Text = "No player";
                        txt_AlbumArtist.Text = string.Empty;

                        Grd_Main.Visibility = Visibility.Visible;
                    });
                }
            }
        }

        private void Current_PlaybackInfoChanged(GlobalSystemMediaTransportControlsSession sender, PlaybackInfoChangedEventArgs args)
        {
            var info = sender.GetPlaybackInfo();

            if (info.PlaybackType == Windows.Media.MediaPlaybackType.Music)
            {
                if (lastPlayBackInfo != null && info.PlaybackType != lastPlayBackInfo.PlaybackType)
                    Current_MediaPropertiesChanged(sender, null);

                _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (info.Controls.IsPlayEnabled) // Play
                        {
                        btn_PlayPause.IsEnabled = (bool)(btn_PlayPause.Tag = true);
                        btn_PlayPause.Content = SegoeIcons.Play;
                    }
                    else if (info.Controls.IsPauseEnabled) // Pause
                        {
                        btn_PlayPause.IsEnabled = (bool)(btn_PlayPause.Tag = true);
                        btn_PlayPause.Content = SegoeIcons.Pause;
                    }
                    else if (info.Controls.IsPlayPauseToggleEnabled) // Unknown
                        {
                        btn_PlayPause.IsEnabled = (bool)(btn_PlayPause.Tag = true);
                        btn_PlayPause.Content = SegoeIcons.PlayPause;
                    }
                    else
                    {
                        btn_PlayPause.IsEnabled = (bool)(btn_PlayPause.Tag = true);
                        btn_PlayPause.Content = SegoeIcons.PlayPause;
                    }

                    btn_Next.Tag = btn_Next.IsEnabled = info.Controls.IsNextEnabled;
                    btn_Previous.Tag = btn_Previous.IsEnabled = info.Controls.IsPreviousEnabled;
                });
            }

            lastPlayBackInfo = info;
        }

        private async void btn_Previous_Click(object sender, RoutedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => btn_Previous.IsEnabled = false);

            await currentSession.TrySkipPreviousAsync();

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => btn_Previous.IsEnabled = btn_Previous.Tag is bool defaultEnable ? defaultEnable : true);
        }

        private async void btn_PlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (currentSession == null) return;
            var info = currentSession.GetPlaybackInfo();

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => btn_PlayPause.IsEnabled = false);

            if (info.Controls.IsPlayEnabled) // Play
            {
                if (await currentSession.TryPlayAsync())
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => btn_PlayPause.Content = SegoeIcons.Play);
            }
            else if (info.Controls.IsPauseEnabled) // Pause
            {
                if (await currentSession.TryPauseAsync())
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => btn_PlayPause.Content = SegoeIcons.Pause);
            }
            else if (info.Controls.IsPlayPauseToggleEnabled) // Unknown
            {
                if (await currentSession.TryTogglePlayPauseAsync())
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => btn_PlayPause.Content = SegoeIcons.PlayPause);
            }

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => btn_PlayPause.IsEnabled = btn_PlayPause.Tag is bool defaultEnable ? defaultEnable : true);
        }

        private async void btn_Next_Click(object sender, RoutedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => btn_Next.IsEnabled = false);

            await currentSession.TrySkipNextAsync();

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => btn_Next.IsEnabled = btn_Next.Tag is bool defaultEnable ? defaultEnable : true);
        }

        private void Page_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => StkPnl_Controls.Visibility = Visibility.Visible);
        }

        private void Page_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => StkPnl_Controls.Visibility = Visibility.Collapsed);
        }
    }
}
