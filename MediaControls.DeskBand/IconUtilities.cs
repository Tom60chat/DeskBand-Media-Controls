using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.Storage.Streams;

namespace MediaControls.DeskBand
{
    internal static class ImageUtilities
    {
        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        public static ImageSource ToImageSource(this Icon icon)
        {
            Bitmap bitmap = icon.ToBitmap();
            return bitmap.ToImageSource();
        }

        public static ImageSource ToImageSource(this Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();

            ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            if (!DeleteObject(hBitmap))
            {
                throw new Win32Exception();
            }

            return wpfBitmap;
        }
        public static async Task<ImageSource> ToImageSource(this IRandomAccessStreamReference thumbnail)
        {
            if (thumbnail == null) return null;

            var read = await thumbnail.OpenReadAsync();
            var imageData = new byte[read.Size];
            var cover = new BitmapImage();
            var ms = new MemoryStream(imageData);

            await read.ReadAsync(imageData.AsBuffer(), Convert.ToUInt32(imageData.Length), InputStreamOptions.None);

            cover.BeginInit();
            cover.StreamSource = ms;
            cover.EndInit();

            return cover;
        }

        public static ImageSource ToImageSource(this Uri uri) => new BitmapImage(uri);


        public static Icon GetShortcutLinkIcon(string lnkPath)
        {
            var shl = new Shell32.Shell();         // Move this to class scope
            lnkPath = Path.GetFullPath(lnkPath);
            var dir = shl.NameSpace(Path.GetDirectoryName(lnkPath));
            var itm = dir.Items().Item(Path.GetFileName(lnkPath));
            var lnk = (Shell32.ShellLinkObject)itm.GetLink;
            string strIcon;
            lnk.GetIconLocation(out strIcon);
            return Icon.ExtractAssociatedIcon(strIcon);
        }
    }
}
