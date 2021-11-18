using MediaControls.Installer.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace MediaControls.Installer
{
    public class Installer
    {
        public readonly static string InstallPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "DeskBand Media Controls");

        public static DirectoryInfo InstallDirectory => new DirectoryInfo(InstallPath);

        public static Dictionary<string, byte[]> DLLs = new Dictionary<string, byte[]>()
        { 
            {"MediaControls.DeskBand.dll", Resources.MediaControls_DLL},
            {"Octokit.dll", Resources.Octokit_DLL},
        };

        // https://stackoverflow.com/questions/54151880/using-c-sharp-to-check-if-a-file-is-in-use
        public static bool IsFileBusy
        {
            get
            {
                try
                {
                    foreach (var DLL in DLLs)
                    {
                        using Stream stream = new FileStream(Path.Combine(InstallPath, DLL.Key), FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                        stream.Close();
                        stream.Dispose();
                    }
                    return false;
                }
                catch (IOException)
                {
                    // File is in use
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static bool CanUpdate()
        {
            try
            {
                var DLL = DLLs.Keys.First();
                FileVersionInfo CurrentDll = FileVersionInfo.GetVersionInfo(Path.Combine(InstallPath, DLL));

                //Setup the versions
                if (Version.TryParse(CurrentDll.FileVersion, out Version CurrentDLLversion))
                {
                    Version thisVersion = Assembly.GetExecutingAssembly().GetName().Version;
                    int versionComparison = CurrentDLLversion.CompareTo(thisVersion);

                    return versionComparison < 0;
                }
            }
            catch { }

            return false;
        }

        public static bool Install()
        {
            var installDirectory = InstallDirectory;

            try
            {
                // Create directory if he doesn't exist
                if (!installDirectory.Exists)
                    installDirectory.Create();

                foreach(var DLL in DLLs)
                    File.WriteAllBytes(Path.Combine(InstallPath, DLL.Key), DLL.Value);

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        public static bool Uninstall()
        {
            var installDirectory = InstallDirectory;

            try
            {
                // Delete the file and delete the folder if empty (We don't want to delete user file)
                if (installDirectory.Exists)
                {

                    foreach (var DLL in DLLs)
                    {
                        var dllFile = new FileInfo(Path.Combine(InstallPath, DLL.Key));
                        if (dllFile.Exists)
                            dllFile.Delete();
                    }

                    // https://stackoverflow.com/questions/755574/how-to-quickly-check-if-folder-is-empty-net
                    if (!Directory.EnumerateFileSystemEntries(installDirectory.FullName).Any())
                        installDirectory.Delete();
                }

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine(e.ToString());
                return false;
            }
        }
    }
}
