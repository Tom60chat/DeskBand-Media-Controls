using System.Windows.Controls;

namespace MediaControls.DeskBand
{
    public static class DefinitionExtension
    {
        public static RowDefinition Clone(this RowDefinition rowDefinition) => new RowDefinition()
        {
            MaxHeight = rowDefinition.MaxHeight,
            Height = rowDefinition.Height,
            MinHeight = rowDefinition.MinHeight
        };

        public static ColumnDefinition Clone(this ColumnDefinition colDefinition) => new ColumnDefinition()
        {
            MaxWidth = colDefinition.MaxWidth,
            Width = colDefinition.Width,
            MinWidth = colDefinition.MinWidth
        };
    }
}
