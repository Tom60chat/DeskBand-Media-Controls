using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Management.Deployment;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace MediaControls.UWP
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class PachageFinder : Page
    {
        public PachageFinder()
        {
            this.InitializeComponent();
        }

        private void btn_Go_Click(object sender, RoutedEventArgs e)
        {
            PackageManager packageManager = new PackageManager();
            var packages = packageManager.FindPackagesForUser(string.Empty, fld_PackageFamilyName.Text);
            var packagesList = packages.ToList();

            txt_result.Text = packages.ToString();
        }
    }
}
