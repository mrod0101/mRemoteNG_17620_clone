using System;
using Microsoft.Win32;


namespace CustomActions
{
    public class DotnetInstalledChecker
    {
        private const string REGISTRY_PATH = @"SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App";
        private const string REGISTRY_PATH_WOW6432 = @"SOFTWARE\WOW6432Node\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App";
        private RegistryKey _activeRegistryPath;


        public DotnetInstalledChecker()
        {
            GetDotnetRegistryKeyPath();
        }

        public bool IsDotnet6Installed()
        {
            return _activeRegistryPath.GetValue("6.0.0") != null;
        }

        private void GetDotnetRegistryKeyPath()
        {
            GetDotnetKeyPath();
            GetDotnetKeyPath6432();
        }

        private void GetDotnetKeyPath()
        {
            try
            {
                _activeRegistryPath = Registry.LocalMachine.OpenSubKey(REGISTRY_PATH);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void GetDotnetKeyPath6432()
        {
            try
            {
                _activeRegistryPath = Registry.LocalMachine.OpenSubKey(REGISTRY_PATH_WOW6432);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}