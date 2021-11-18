using MediaControls.DeskBand;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace MediaControls.Test
{
    /// <summary>
    /// Logique d'interaction pour TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        private UserControl1 userControl;

        public TestWindow(UserControl1 userControl)
        {
            this.userControl = userControl;
            InitializeComponent();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem item)
            {
                if (item.Content.ToString() == "Top")
                {
                    userControl.CurrentEdge = CSDeskBand.Edge.Top;
                }
                else if (item.Content.ToString() == "Bottom")
                {
                    userControl.CurrentEdge = CSDeskBand.Edge.Bottom;
                }
                else if (item.Content.ToString() == "Left")
                {
                    userControl.CurrentEdge = CSDeskBand.Edge.Left;
                }
                else if (item.Content.ToString() == "Right")
                {
                    userControl.CurrentEdge = CSDeskBand.Edge.Right;
                }
            }
        }
    }
}
