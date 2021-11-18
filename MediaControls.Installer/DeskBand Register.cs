using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

namespace MediaControls.Installer
{
    public class DeskbandRegister : MarshalByRefObject
    {
        public bool Register(string dllPath)
        {
            var asm = Assembly.LoadFile(dllPath);
            return Register(asm);
        }

        public bool Unregister(string dllPath)
        {
            var asm = Assembly.LoadFile(dllPath);
            return Unregister(asm);
        }

        public static bool Register(Assembly asm)
        {
            var regAsm = new RegistrationServices();

            try
            {
                return regAsm.RegisterAssembly(asm, AssemblyRegistrationFlags.SetCodeBase);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Properties.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        public static bool Unregister(Assembly asm)
        {
            var regAsm = new RegistrationServices();

            try
            {
                return regAsm.UnregisterAssembly(asm);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Properties.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        /*public static string GetRegisteredDllPath(Guid guid)
        {
            var key = Registry.ClassesRoot.OpenSubKey(@"CLSID\{" + guid.ToString().ToUpper() + "}");

            return string.Empty;
        }*/

        /// <summary>
        /// Check if the DLL is regitered.
        /// </summary>
        /// <param name="dllName">DLL file name.</param>
        /// <returns>If the DLL is regitered.</returns>
        public static bool IsRegistered(string dllName) => Registry.ClassesRoot.OpenSubKey(Path.GetFileNameWithoutExtension(dllName)) != null;

        /// <summary>
        /// Check if the DLL is regsitered.
        /// If the key is not found uncheck "Prefered 32bits" in build option.
        /// </summary>
        /// <param name="guid">GUID of the dll.</param>
        /// <returns>If the DLL is regitered.</returns>
        public static bool IsRegistered(Guid guid) => Registry.ClassesRoot.OpenSubKey(@"CLSID\{" + guid.ToString().ToUpper() + "}") != null;
    }
}
