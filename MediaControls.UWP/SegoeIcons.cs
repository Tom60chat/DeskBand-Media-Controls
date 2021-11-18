namespace MediaControls.UWP
{
    public static class SegoeIcons
    {
        public static string Play => char.ConvertFromUtf32(0xE768);
        public static string Pause => char.ConvertFromUtf32(0xE769);
        public static string PlayPause => Play + Pause;
    }
}
