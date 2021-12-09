using System;
using System.IO;
using System.Linq;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Win32;

namespace CustomActions
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult IsMinimumRdpVersionInstalled(Session session)
        {
            var acceptedRdpKbVariables = new[] { session["RDP80_KB"], session["RDP81_KB"] };
            var returnVariable = "MINIMUM_RDP_VERSION_INSTALLED";
            var kbInstalledChecker = new KbInstalledChecker(session);
            kbInstalledChecker.Execute(acceptedRdpKbVariables, returnVariable);
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult IsRdpDtlsUpdateInstalled(Session session)
        {
            var kb = session["RDP_DTLS_KB"];
            var returnVar = "RDP_DTLS_UPDATE_INSTALLED";
            var kbInstalledChecker = new KbInstalledChecker(session);
            kbInstalledChecker.Execute(kb, returnVar);
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult IsLegacyVersionInstalled(Session session)
        {
            session.Log("Begin IsLegacyVersionInstalled");
            var uninstaller = new UninstallNsisVersions();
            if (uninstaller.IsLegacymRemoteNgInstalled())
            {
                session["LEGACYVERSIONINSTALLED"] = "1";
            }
            else
            {
                session["LEGACYVERSIONINSTALLED"] = "0";
            }

            session.Log("End IsLegacyVersionInstalled");
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult UninstallLegacyVersion(Session session)
        {
            session.Log("Begin UninstallLegacyVersion");
            var uninstaller = new UninstallNsisVersions();
            uninstaller.GetLegacyUninstallString();
            uninstaller.UninstallLegacyVersion(true);
            session.Log("End UninstallLegacyVersion");
            return ActionResult.Success;
        }


        [CustomAction]
        public static ActionResult IsMinimumDotNetVersionInstalled(Session session)
        {
            session.Log("Begin IsMinimumDotNetVersionInstalled");


            using (var root = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                using (var key = root.OpenSubKey(@"SOFTWARE\WOW6432Node\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App", false))
                {
                    foreach (var subkey in key.GetSubKeyNames())
                    {
                        if (subkey.StartsWith("6.0"))
                            session["MINIMUM_DOTNET_VERSION_INSTALLED"] = "1";
                        else
                            session["MINIMUM_DOTNET_VERSION_INSTALLED"] = "0";
                    }
                };
            };

            session.Log("End IsMinimumDotNetVersionInstalled");

            return ActionResult.Success;
        }

    }
}