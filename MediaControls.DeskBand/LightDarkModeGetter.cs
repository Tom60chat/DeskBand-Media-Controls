using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Windows.UI.ViewManagement;

namespace MediaControls.DeskBand
{
    public static class LightDarkModeGetter
    {
        public static SolidColorBrush Foreground
        {
            get
            {
                UISettings settings = new UISettings();
                var foreground = settings.GetColorValue(UIColorType.Foreground);
                Color myforeColor = Color.FromArgb(foreground.A, foreground.R, foreground.G, foreground.B);
                return new SolidColorBrush(myforeColor);
            }
        }

        public static SolidColorBrush Background
        {
            get
            {
                UISettings settings = new UISettings();
                var background = settings.GetColorValue(UIColorType.Background);

                Color mybackColor = Color.FromArgb(background.A, background.R, background.G, background.B);
                return new SolidColorBrush(mybackColor);
            }
        }

        public static SolidColorBrush TranslucentForeground
        {
            get
            {
                UISettings settings = new UISettings();
                var foreground = settings.GetColorValue(UIColorType.Foreground);
                Color myforeColor = Color.FromArgb(40, foreground.R, foreground.G, foreground.B);
                return new SolidColorBrush(myforeColor);
            }
        }
    }
}
