using System;
using System.Windows.Forms;
using Microsoft.Win32;

namespace PowerSwitcher.Utils
{
    public sealed class RegistryHelper : IDisposable
    {
        private readonly RegistryKey rKey;

        public RegistryHelper()
        {
            rKey = Registry.CurrentUser.OpenSubKey(AppVariables.RegistryKeyPath, true);
        }

        public void Dispose()
        {
            if (rKey != null) rKey.Dispose();
        }

        public bool IsRegisteredOnStartup
        {
            get => (rKey.GetValue(AppVariables.ApplicationName) != null) ? true : false;
        }

        public void RegisterOnStartup()
        {
            rKey.SetValue(AppVariables.ApplicationName, Application.ExecutablePath);
        }

        public void UnRegisterOnStartup()
        {
            rKey.DeleteValue(AppVariables.ApplicationName);
        }
    }
}
