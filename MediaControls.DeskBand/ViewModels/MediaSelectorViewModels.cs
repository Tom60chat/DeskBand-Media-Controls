using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Windows.UI.ViewManagement;

namespace MediaControls.DeskBand.ViewModels
{
    class MediaSelectorViewModels : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public MediaSelectorViewModels()
        {
            UISettings settings = new UISettings();
            var foreground = settings.GetColorValue(UIColorType.Foreground);
            var background = settings.GetColorValue(UIColorType.Background);
            Color myforeColor = Color.FromArgb(foreground.A, foreground.R, foreground.G, foreground.B);
            ForeBrush = new SolidColorBrush(myforeColor);

            Color mybackColor = Color.FromArgb(background.A, background.R, background.G, background.B);
            BackBrush = new SolidColorBrush(mybackColor);
        }

        private SolidColorBrush foreBrush = new SolidColorBrush(Colors.Black);
        public SolidColorBrush ForeBrush
        {
            get
            {
                return foreBrush;
            }
            set
            {
                foreBrush = value;
                NotifyPropertyChanged();
            }
        }


        private SolidColorBrush backBrush = new SolidColorBrush(Colors.White);
        public SolidColorBrush BackBrush
        {
            get
            {
                return backBrush;
            }
            set
            {
                backBrush = value;
                NotifyPropertyChanged();
            }
        }
    }
}
