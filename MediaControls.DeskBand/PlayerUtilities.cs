using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Windows.ApplicationModel;
using Windows.Management.Deployment;
using Windows.System;

namespace MediaControls.DeskBand
{
    class PlayerUtilities
    {
        private static PlayerShortcut[] KnownPlayersPackage = new PlayerShortcut[]
        {
            new PlayerShortcut() { Link = "SpotifyAB.SpotifyMusic_zpdnekdrzrea0" , Type = PlayerShortcut.ShortcutType.WinRT }, // Spotify Store
            new PlayerShortcut() { Link = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), "Spotify.lnk") , Type = PlayerShortcut.ShortcutType.Win32 }, // Spotify Win32
            new PlayerShortcut() { Link = "SoundcloudLtd.SoundCloudforWindowsBeta_2xc63xn306dnw" , Type = PlayerShortcut.ShortcutType.WinRT }, // SoundCloud Store
            new PlayerShortcut() { Link = "Deezer.62021768415AF_q7m17pa7q8kj0" , Type = PlayerShortcut.ShortcutType.WinRT }, // Spotify Store
            new PlayerShortcut() { Link = "https://music.youtube.com/" , Type = PlayerShortcut.ShortcutType.Url, Name = "Youtube Music" }, // YouTube Music
        };

        public static List<string> PlayersSupportVolControl = new List<string>
        {
            "Spotify.exe",
            "vlc.exe"
        };

        public static IList<PlayerShortcut> GetPlayers()
        {
            PackageManager packageManager = new PackageManager();
            List<PlayerShortcut> players = new List<PlayerShortcut>();

            try
            {
                foreach (var player in KnownPlayersPackage)
                {
                    switch (player.Type)
                    {
                        case PlayerShortcut.ShortcutType.Win32:
                            if (File.Exists(player.Link))
                            {
                                player.Name = Path.GetFileNameWithoutExtension(player.Link);
                                player.Icon = ImageUtilities.GetShortcutLinkIcon(player.Link).ToImageSource();
                                players.Add(player);
                            }
                            break;

                        case PlayerShortcut.ShortcutType.WinRT:
                            var package = packageManager.FindPackagesForUser(string.Empty, player.Link).FirstOrDefault();
                            if (package != null)
                            {
                                player.Name = package.DisplayName;
                                player.Icon = package.Logo.ToImageSource();
                                players.Add(player);
                            }
                            break;

                        case PlayerShortcut.ShortcutType.Url:
                            player.Icon = new BitmapImage(new Uri(Path.Combine(player.Link, "favicon.ico")));
                            players.Add(player);
                            break;

                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + Environment.NewLine + e.ToString(), "Player utilities error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return players;
        }

        public static async Task<bool> StartPlayer(PlayerShortcut player)
        {
            PackageManager packageManager = new PackageManager();

            try
            {
                switch (player.Type)
                {
                    case PlayerShortcut.ShortcutType.WinRT:
                        var package = packageManager.FindPackagesForUser(string.Empty, player.Link).FirstOrDefault();
                        if (package != null)
                        {
                            var apps = await package.GetAppListEntriesAsync();
                            var firstApp = apps.FirstOrDefault();
                            return await firstApp.LaunchAsync();
                        }
                        break;

                    default:
                        Process.Start(player.Link);
                        break;

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + Environment.NewLine + e.ToString(), "Player utilities error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return false;
        }
    }
}
