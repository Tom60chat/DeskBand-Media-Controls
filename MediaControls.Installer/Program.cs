using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows;

namespace MediaControls.Installer
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
            App.Main();
        }

        /// <summary>
        /// Hooks to assembly resolver and tries to load assembly (.dll)
        /// from executable resources it CLR can't find it locally.
        ///
        /// Used for embedding assemblies onto executables.
        ///
        /// See: http://www.digitallycreated.net/Blog/61/combining-multiple-assemblies-into-a-single-exe-for-a-wpf-application
        /// </summary>
        private static Assembly OnResolveAssembly(object sender, ResolveEventArgs args)
        {
            //MessageBox.Show("Resolving " + args.Name);

            var executingAssembly = Assembly.GetExecutingAssembly();
            var assemblyName = new AssemblyName(args.Name);

            var path = assemblyName.Name + ".dll";
            if (!assemblyName.CultureInfo.Equals(CultureInfo.InvariantCulture))
            {
                path = $"{assemblyName.CultureInfo.TwoLetterISOLanguageName}\\{path}";
            }

            using (var stream = executingAssembly.GetManifestResourceStream(path))
            {
                if (stream == null)
                {
                    //MessageBox.Show("Null stream");
                    return null;
                }

                var assemblyRawBytes = new byte[stream.Length];
                stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);
                return Assembly.Load(assemblyRawBytes);
            }
        }
    }
}