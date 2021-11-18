using CSDeskBand;
using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace MediaControls.DeskBand
{
    [ComVisible(true)]
    [Guid("77D175B4-0A80-4581-A28E-D17150AAE027")]
    [CSDeskBandRegistration(Name = "Media Controls", ShowDeskBand = true)]
    public class Deskband : CSDeskBandWpf
    {
        public Deskband()
        {
            Options.Title = Properties.Resources.Name;

            Options.HorizontalSize = new Size(180, 40);
            Options.MinHorizontalSize = new Size(150, 0);

            Options.MinVerticalSize = new Size(0, 150);
            Options.VerticalSize = new Size(40, 180);

            Options.HeightCanChange = true;
            Options.IsFixed = false;
            Options.PropertyChanged += Options_PropertyChanged;

            TaskbarInfo.TaskbarEdgeChanged += TaskbarInfo_TaskbarEdgeChanged;
            TaskbarInfo.TaskbarOrientationChanged += TaskbarInfo_TaskbarOrientationChanged;
            TaskbarInfo.TaskbarSizeChanged += TaskbarInfo_TaskbarSizeChanged;
        }

        private void TaskbarInfo_TaskbarSizeChanged(object sender, TaskbarSizeChangedEventArgs e)
        {
        }

        private void TaskbarInfo_TaskbarOrientationChanged(object sender, TaskbarOrientationChangedEventArgs e)
        {
        }

        private void TaskbarInfo_TaskbarEdgeChanged(object sender, TaskbarEdgeChangedEventArgs e)
        {
            if (UserControl1.Singleton != null)
            {
                UserControl1.Singleton.CurrentEdge = e.Edge;
            }
        }

        private void Options_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

        }

        protected override UIElement UIElement => new UserControl1(); // Return the main wpf control
    }
}
