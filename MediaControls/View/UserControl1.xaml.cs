using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Windows.Media.Control;
using TaskbarPosition = System.Windows.Forms.TaskbarPosition;
using Taskbar = System.Windows.Forms.Taskbar;

namespace MediaControls
{
    /// <summary>
    /// Logique d'interaction pour UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        private SessionManager sessionManager;
        private GlobalSystemMediaTransportControlsSession currentSession;
        private GlobalSystemMediaTransportControlsSessionPlaybackInfo lastPlayBackInfo;
        private MediaSelectorWindow mediaSelector;
        private TaskbarPosition currentEdge = TaskbarPosition.Bottom;
        //private bool mediaSelectorOpen;
        public bool ImgLoading { get => true; }
        public static UserControl1 Singleton { get; private set; }
        public TaskbarPosition CurrentEdge
        {
            get => currentEdge;
            set {
                currentEdge = value;
                UpdateMediaSelectorButton();
                UpdateOrientation();
                UserControl_SizeChanged(this, EventArgs.Empty as SizeChangedEventArgs);
            }
        }

        public UserControl1()
        {
            Singleton = this;
            
            try
            {
                InitializeComponent();
                SetCorner(false);
                NoPlayer();
                _ = Init();
                _ = CheckUpdate();

                CurrentEdge = Taskbar.Position;
                wrapPnlControls.Opacity = 0;
                bnt_MediaSelector.Opacity = 0;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + Environment.NewLine + e.ToString(), "Init error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SetCorner(bool corner)
        {
            if (corner)
            {
                AlbumCoverImage_Border.CornerRadius = new CornerRadius(4);

                var style = Resources["CustomButton"] as Style;
                var newStyle = new Style(typeof(Button), style);
                newStyle.Setters.Add(new Setter(Border.CornerRadiusProperty, 4));
                bnt_MediaSelector.Style = style;
            }
            else
            {
                AlbumCoverImage_Border.CornerRadius = default;
                bnt_MediaSelector.Style = Resources["CustomButton"] as Style;
            }
        }

        private async Task CheckUpdate()
        {
            try
            {
                var gitHub = new GitHubHelper("Tom60chat", "DeskBand-Media-Controls");

                if (await gitHub.CheckNewerVersion()) //  Check if a update is available.
                {
                    Dispatcher.Invoke(() =>
                    {
                        btn_Update.Visibility = Visibility.Visible;
                        btn_Update.Click += (s, e) => gitHub.Update();
                    });
                }
            }
            catch (Exception e)
            {
#if DEBUG
                MessageBox.Show(e.Message + Environment.NewLine + e.ToString(), "Update error", MessageBoxButton.OK, MessageBoxImage.Error);
#endif
            }
        }

        private async Task Init()
        {
            try
            {
                sessionManager = await SessionManager.InitializeAsync();
                sessionManager.CurrentSessionChanged += SessionManager_CurrentSessionChanged; ;

                SessionManager_CurrentSessionChanged(this, sessionManager.CurrentSession);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + Environment.NewLine + e.ToString(), "Init error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateOrientation()
        {
            Grd_Main.ColumnDefinitions.Clear();
            Grd_Main.RowDefinitions.Clear();

            switch (CurrentEdge)
            {
                // Horizontal
                case TaskbarPosition.Bottom:
                case TaskbarPosition.Top:
                    // Definition
                    /*reactGridProperties.Height = 40;
                    reactGridProperties.Width = double.NaN;
                    reactGridProperties.MinHeight = 0;
                    reactGridProperties.MinWidth = 150;*/
                    Grd_Main.ColumnDefinitions.Add(clmDef1.Clone());
                    Grd_Main.ColumnDefinitions.Add(clmDef2.Clone());
                    Grd_Main.ColumnDefinitions.Add(clmDef3.Clone());

                    // Oritentation
                    //wrapPnlControls.Orientation = Orientation.Horizontal;

                    if (uc_PlayersShortcut.LstStkPanel != null)
                    {
                        uc_PlayersShortcut.LstStkPanel.Orientation = Orientation.Horizontal;
                    }

                    // Allignement
                    wrapPnlControls.HorizontalAlignment = HorizontalAlignment.Left;
                    wrapPnlControls.VerticalAlignment = VerticalAlignment.Center;
                    gridProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    gridProperties.VerticalAlignment = VerticalAlignment.Center;
                    /*viewBoxProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    viewBoxProperties.VerticalAlignment = VerticalAlignment.Center;*/
                    txt_Title.TextAlignment = txt_AlbumArtist.TextAlignment = TextAlignment.Left;

                    // Size
                    //txt_Title.FontSize = 14;
                    //txt_Title.LineHeight = 14;
                    //txt_AlbumArtist.FontSize = 10;

                    //btn_Previous.FontSize = btn_PlayPause.FontSize = btn_Next.FontSize = btn_VolDown.FontSize = btn_VolUp.FontSize = btn_Update.FontSize = 16;

                    break;

                // Vertical
                case TaskbarPosition.Right:
                case TaskbarPosition.Left:
                    // Definition
                    /*reactGridProperties.Height = double.NaN;
                    reactGridProperties.Width = 40;
                    reactGridProperties.MinHeight = 100;
                    reactGridProperties.MinWidth = 0;*/
                    Grd_Main.RowDefinitions.Add(rowDef1.Clone());
                    Grd_Main.RowDefinitions.Add(rowDef2.Clone());
                    Grd_Main.RowDefinitions.Add(rowDef3.Clone());

                    // Oritentation
                    //wrapPnlControls.Orientation = Orientation.Vertical;

                    if (uc_PlayersShortcut.LstStkPanel != null)
                    {
                        uc_PlayersShortcut.LstStkPanel.Orientation = Orientation.Vertical;
                    }
                    // Allignement
                    wrapPnlControls.HorizontalAlignment = HorizontalAlignment.Center;
                    wrapPnlControls.VerticalAlignment = VerticalAlignment.Top;
                    gridProperties.HorizontalAlignment = HorizontalAlignment.Center;
                    gridProperties.VerticalAlignment = VerticalAlignment.Top;
                    /*viewBoxProperties.HorizontalAlignment = HorizontalAlignment.Center; /* currentEdge == Edge.Right ? HorizontalAlignment.Right : HorizontalAlignment.Left; *//*
                    viewBoxProperties.VerticalAlignment = VerticalAlignment.Top;*/
                    txt_Title.TextAlignment = txt_AlbumArtist.TextAlignment = TextAlignment.Center; /*currentEdge == Edge.Right ? TextAlignment.Right : TextAlignment.Left;*/

                    // Size
                    //txt_Title.FontSize = 7;
                    //txt_Title.LineHeight = 7;
                    //txt_AlbumArtist.FontSize = 5;

                    //btn_Previous.FontSize = btn_PlayPause.FontSize = btn_Next.FontSize = btn_VolDown.FontSize = btn_VolUp.FontSize = btn_Update.FontSize = 12;
                    break;
            }
        }

        private void NoPlayer()
        {
            // Hide controls

            Dispatcher.Invoke(() =>
            {
                //Img_AlbumCover.Visibility = Visibility.Collapsed;
                Img_AlbumCover.ImageSource = null;
                bnt_MediaSelector.Visibility = Visibility.Collapsed;
                gridProperties.Visibility = Visibility.Collapsed;
                wrapPnlControls.Visibility = Visibility.Collapsed;
                uc_PlayersShortcut.Visibility = Visibility.Visible;
            });

            if (mediaSelector != null)
                mediaSelector.Hide();
        }

        private void NoInfo()
        {
            Dispatcher.Invoke(() =>
            {
                ShowPropertiesFade.Storyboard.SpeedRatio = 0.0000001; // Litle hack
                ShowControlsFade.Storyboard.Begin();
                /*if (ShowControlsFade.Storyboard.Duration.HasTimeSpan)
                    await Task.Delay(ShowControlsFade.Storyboard.Duration.TimeSpan);
                else
                    await Task.Delay(150);
                ShowPropertiesFade.Storyboard.Begin();*/
                /*txt_Title.Visibility = Visibility.Visible;
                txt_Title.Text = "...";*/
            });
        }

        private void SessionManager_CurrentSessionChanged(object sender, GlobalSystemMediaTransportControlsSession session)
        {
            try
            {
                if (currentSession != null)
                {
                    currentSession.MediaPropertiesChanged -= Current_MediaPropertiesChanged;
                    currentSession.PlaybackInfoChanged -= Current_PlaybackInfoChanged;
                }

                currentSession = session;

                if (currentSession != null)
                {
                    currentSession.MediaPropertiesChanged += Current_MediaPropertiesChanged;
                    currentSession.PlaybackInfoChanged += Current_PlaybackInfoChanged;


                    Current_MediaPropertiesChanged(currentSession, null);
                    Current_PlaybackInfoChanged(currentSession, null);
                }
                else
                {
                    Dispatcher.Invoke(() => NoPlayer());
                }

                Dispatcher.Invoke(() => btn_VolUp.Visibility = btn_VolDown.Visibility =
                    false ?
                    //PlayerUtilities.PlayersSupportVolControl.Contains(currentSession.SourceAppUserModelId) ?
                        Visibility.Visible : Visibility.Collapsed);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + Environment.NewLine + e.ToString(), "SessionManager - CurrentSessionChanged error", MessageBoxButton.OK, MessageBoxImage.Error);
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


                Dispatcher.Invoke(() =>
                {
                    ShowPropertiesFade.Storyboard.SpeedRatio = 1;
                    if (!IsMouseOver)
                        ShowPropertiesFade.Storyboard.Begin();

                    // Restore controls
                    bnt_MediaSelector.Visibility = Visibility.Visible;
                    gridProperties.Visibility = Visibility.Visible;
                    wrapPnlControls.Visibility = Visibility.Visible;
                    uc_PlayersShortcut.Visibility = Visibility.Collapsed;
                });

                if (properties != null)
                {
                    // Update texts
                    Dispatcher.Invoke(() =>
                    {
                        txt_Title.Text = properties.Title;
                        txt_Title.Visibility = string.IsNullOrEmpty(properties.Title) ?
                            Visibility.Collapsed : Visibility.Visible;

                        txt_AlbumArtist.Text = properties.Artist;
                        txt_AlbumArtist.Visibility = string.IsNullOrEmpty(properties.Artist) ?
                            Visibility.Collapsed : Visibility.Visible;

                        if (string.IsNullOrEmpty(properties.Title) && string.IsNullOrEmpty(properties.Artist))
                            NoInfo();

                        Grd_Main.Visibility = Visibility.Visible;
                    });

                    // Update thumbnail
                    try
                    {
                        // UWP Thumbnail Stream to Bitmap
                        if (properties.Thumbnail != null)
                        {
                            await Dispatcher.Invoke(async () =>
                            {
                                var cover = await properties.Thumbnail.ToImageSource();

                                if (currentSession == null) return; // Avoid latency

                                if (currentSession.SourceAppUserModelId == "Spotify.exe")
                                {
                                    // Create a CroppedBitmap based off of a xaml defined resource.
                                    cover = new CroppedBitmap(
                                       cover as BitmapImage,
                                       new Int32Rect(30, 0, 240, 233));       //select region rect            
                                }

                                Img_AlbumCover.ImageSource = cover; //set image source to cropped
                                //Img_AlbumCover.Visibility = Visibility.Visible;
                            });
                        }
                        else
                            Img_AlbumCover.ImageSource = null;
                            //Img_AlbumCover.Visibility = Visibility.Hidden;
                    }
                    catch (Exception e)
                    {
                        Img_AlbumCover.ImageSource = null;
                        //Img_AlbumCover.Visibility = Visibility.Hidden;
                        MessageBox.Show(e.Message + Environment.NewLine + e.ToString(), "Cover error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                    Dispatcher.Invoke(() => NoInfo());
            }
            catch (Exception)
            {
                /*if (e.HResult == unchecked((int)0x800706BA))
                    Dispatcher.Invoke(() => NoPlayer() );*/
            }
        }

        private void Current_PlaybackInfoChanged(GlobalSystemMediaTransportControlsSession sender, PlaybackInfoChangedEventArgs args)
        {
            var info = sender.GetPlaybackInfo();

            if (info == null) return;

            if (info.PlaybackType == Windows.Media.MediaPlaybackType.Music)
            {
                if (lastPlayBackInfo != null && info.PlaybackType != lastPlayBackInfo.PlaybackType)
                    Current_MediaPropertiesChanged(sender, null);

                Dispatcher.Invoke(() =>
                {
                    if (info.Controls.IsPlayEnabled) // Play
                    {
                        btn_PlayPause.IsEnabled = true;
                        btn_PlayPause.Content = SegoeIcons.Play;
                    }
                    else if (info.Controls.IsPauseEnabled) // Pause
                    {
                        btn_PlayPause.IsEnabled = true;
                        btn_PlayPause.Content = SegoeIcons.Pause;
                    }
                    else if (info.Controls.IsPlayPauseToggleEnabled) // Unknown
                    {
                        btn_PlayPause.IsEnabled = true;
                        btn_PlayPause.Content = SegoeIcons.PlayPause;
                    }
                    else
                    {
                        btn_PlayPause.IsEnabled = false;
                        btn_PlayPause.Content = SegoeIcons.PlayPause;
                    }

                    btn_Next.IsEnabled = info.Controls.IsNextEnabled;
                    btn_Previous.IsEnabled = info.Controls.IsPreviousEnabled;
                });
            }

            lastPlayBackInfo = info;
        }

        private async void btn_Previous_Click(object sender, RoutedEventArgs e)
        {
            if (currentSession == null) return;

            Dispatcher.Invoke(() => btn_Previous.IsEnabled = false );

            await currentSession.TrySkipPreviousAsync();

            Dispatcher.Invoke(() => btn_Previous.IsEnabled = true);
        }

        private async void btn_PlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (currentSession == null) return;

            var info = currentSession.GetPlaybackInfo();

            Dispatcher.Invoke(() => btn_PlayPause.IsEnabled = false);

            if (info.Controls.IsPlayEnabled) // Play
                await currentSession.TryPlayAsync();
            else if (info.Controls.IsPauseEnabled) // Pause
                await currentSession.TryPauseAsync();
            else if (info.Controls.IsPlayPauseToggleEnabled) // Unknown
            {
                await currentSession.TryTogglePlayPauseAsync();
                Dispatcher.Invoke(() => btn_PlayPause.IsEnabled = true);
            }
        }

        private async void btn_Next_Click(object sender, RoutedEventArgs e)
        {
            if (currentSession == null) return;

            Dispatcher.Invoke(() => btn_Next.IsEnabled = false );

            await currentSession.TrySkipNextAsync();

            Dispatcher.Invoke(() => btn_Next.IsEnabled = true);
        }

        private void btn_VolUp_Click(object sender, RoutedEventArgs e)
        {
            if (currentSession == null) return;

            var processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(currentSession.SourceAppUserModelId));

            foreach(var process in processes)
            {
                TargetInput.SendInput(process.Handle, TargetInput.WM_KEYDOWN, TargetInput.VK_CONTROL);
                TargetInput.SendInput(process.Handle, TargetInput.WM_KEYDOWN, TargetInput.VK_UP);
                TargetInput.SendInput(process.Handle, TargetInput.WM_KEYUP, TargetInput.VK_UP);
                TargetInput.SendInput(process.Handle, TargetInput.WM_KEYUP, TargetInput.VK_CONTROL);
            }
        }

        private void btn_VolDown_Click(object sender, RoutedEventArgs e)
        {
            if (currentSession == null) return;

            var processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(currentSession.SourceAppUserModelId));

            foreach (var process in processes)
            {
                TargetInput.SendInput(process.Handle, TargetInput.WM_KEYDOWN, TargetInput.VK_CONTROL);
                TargetInput.SendInput(process.Handle, TargetInput.WM_KEYDOWN, TargetInput.VK_DOWN);
                TargetInput.SendInput(process.Handle, TargetInput.WM_KEYUP, TargetInput.VK_DOWN);
                TargetInput.SendInput(process.Handle, TargetInput.WM_KEYUP, TargetInput.VK_CONTROL);
            }
        }

        private void bnt_MediaSelector_Click(object sender, RoutedEventArgs e)
        {
            /*if (mediaSelectorOpen)
            {
                mediaSelectorOpen = false;
                return;
            }*/

            if (mediaSelector == null)
            {
                mediaSelector = new MediaSelectorWindow();
                mediaSelector.IsVisibleChanged += (s, ev) => UpdateMediaSelectorButton();
            }

            if (mediaSelector.IsVisible)
                mediaSelector.Hide();
            else
            {
                var point = PointFromScreen(new Point());
                mediaSelector.DeskBandPoint = point; // Lets the window know where is he
                switch (CurrentEdge)
                {
                    case TaskbarPosition.Bottom:
                    case TaskbarPosition.Top:
                        mediaSelector.Width = ActualWidth;
                        break;

                    case TaskbarPosition.Left:
                    case TaskbarPosition.Right:
                        mediaSelector.Width = 180;
                        break;

                }
                mediaSelector.Show();
                //mediaSelectorOpen = true;
            }
        }

        private void UpdateMediaSelectorButton()
        {
            bnt_MediaSelector.Content =
                mediaSelector != null && mediaSelector.IsVisible ?
                CurrentEdge switch
                {
                    TaskbarPosition.Left => SegoeIcons.ChevronLeft,
                    TaskbarPosition.Top => SegoeIcons.ChevronUp,
                    TaskbarPosition.Right => SegoeIcons.ChevronRight,
                    TaskbarPosition.Bottom => SegoeIcons.ChevronDown,
                    _ => SegoeIcons.ChevronDown,
                } :
                CurrentEdge switch
                {
                    TaskbarPosition.Left => SegoeIcons.ChevronRight,
                    TaskbarPosition.Top => SegoeIcons.ChevronDown,
                    TaskbarPosition.Right => SegoeIcons.ChevronLeft,
                    TaskbarPosition.Bottom => SegoeIcons.ChevronUp,
                    _ => SegoeIcons.ChevronDown,
                };
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double margin = 0;

            switch (CurrentEdge)
            {
                case TaskbarPosition.Bottom:
                case TaskbarPosition.Top:
                    Grd_Main.ColumnDefinitions[0].Width = new GridLength(ActualHeight);
                    /*bnt_MediaSelector.Height = double.NaN;
                    bnt_MediaSelector.Width = ActualHeight;*/
                    /*wrapPnlControls.ItemHeight = AtualHeight;
                    wrapPnlControls.ItemWidth = 24;*/
                    break;

                case TaskbarPosition.Left:
                case TaskbarPosition.Right:
                    Grd_Main.RowDefinitions[0].Height = new GridLength(ActualWidth);
                    margin = Grd_Main.RowDefinitions[0].Height.Value + Grd_Main.RowDefinitions[1].Height.Value;
                    /* bnt_MediaSelector.Height = ActualWidth;
                     bnt_MediaSelector.Width = double.NaN;*/
                    /*wrapPnlControls.ItemHeight = 24;
                    wrapPnlControls.ItemWidth = ActualWidth;*/
                    break;
            }

            /*if (txt_AlbumArtist.ActualHeight > ActualHeight)
            {
                txt_Title.Height = double.NaN;
                txt_AlbumArtist.Height = 0;
            }
            else
            {*/

            txt_Title.MaxHeight = Math.Max((int)((ActualHeight - 13 - margin) / 13) * 13, 0);
            txt_AlbumArtist.MaxHeight = Math.Max((int)((ActualHeight - txt_Title.ActualHeight - margin) / 13) * 13, 13);
            //test.MaxHeight = (int)((ActualHeight - txt_AlbumArtist.Height) / 14) * 14;
            if (txt_Title.MaxHeight == 0)
            {
                txt_Title.MaxHeight = 13;
                rowAlbumArtist.Height = new GridLength(0);
            }
            else
                rowAlbumArtist.Height = new GridLength();
            //}

            /*double width = ActualWidth;
            double height = ActualHeight;

            if (double.IsNaN(ActualWidth) || ActualWidth == 0)
                width = 40;
            if (double.IsNaN(ActualHeight) || ActualHeight == 0)
                height = 150;

            switch (CurrentEdge)
            {
                case Edge.Bottom:
                case Edge.Top:
                    reactGridProperties.MaxWidth = reactGridProperties.Height * width / height;
                    reactGridProperties.MaxHeight = double.PositiveInfinity;
                    break;

                case Edge.Left:
                case Edge.Right:
                    reactGridProperties.MaxWidth = double.PositiveInfinity;
                    reactGridProperties.MaxHeight = reactGridProperties.Width * height / width;
                    break;
            }*/
        }
    }
}
