using System.Collections.Generic;
using System.Windows.Media;
using Windows.Media.Control;

namespace MediaControls.DeskBand
{
    public class Player
    {
        /// <summary>
        /// Name of te media player
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Icon of the media player
        /// </summary>
        public ImageSource Icon { get; set; }
        /// <summary>
        /// Album title of the current media session
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Album cover of the current media session
        /// </summary>
        public ImageSource Cover { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Player player &&
                   Name == player.Name;
        }

        public override int GetHashCode()
        {
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        }

        public override string ToString() => Name;
    }
}
