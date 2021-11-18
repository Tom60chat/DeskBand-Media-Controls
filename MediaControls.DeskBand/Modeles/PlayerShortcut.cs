using System.Windows.Media;

namespace MediaControls.DeskBand
{
    public class PlayerShortcut
    {
        #region Enumerators
        public enum ShortcutType
        {
            Win32,
            WinRT,
            Url,
        }
        #endregion

        #region Variables
        public string Link;
        public ShortcutType Type;
        #endregion

        #region Properties
        public string Name { get; set; }
        public ImageSource Icon { get; set; }
        #endregion
    }
}
