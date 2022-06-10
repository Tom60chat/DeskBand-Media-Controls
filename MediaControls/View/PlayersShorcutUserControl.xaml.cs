using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MediaControls
{
    /// <summary>
    /// Logique d'interaction pour PlayersShorcutUserControl.xaml
    /// </summary>
    public partial class PlayersShorcutUserControl : UserControl
    {
        public ObservableCollection<PlayerShortcut> PlayerShortcutItems { get; set; }
        public StackPanel LstStkPanel { get; private set; }

        public PlayersShorcutUserControl()
        {
            try
            {
                var players = PlayerUtilities.GetPlayers();
                PlayerShortcutItems = new ObservableCollection<PlayerShortcut>(players);

                InitializeComponent();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + Environment.NewLine + e.ToString(), "Players shortcut error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void lst_Player_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lst_Player.SelectedItem is PlayerShortcut player)
            {
                await PlayerUtilities.StartPlayer(player);
            }
            lst_Player.SelectedIndex = -1;
        }

        private void lstStkPanel_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is StackPanel lstStkPanel)
                LstStkPanel = lstStkPanel;
        }

        /*public void HideActivePlayer()
        {
            if (SessionManager.Singleton == null || SessionManager.Singleton.Manager == null) return;

            var manager = SessionManager.Singleton.Manager;

            var session = SessionManager.Singleton.Manager.GetSessions()
        }*/
    }
}
