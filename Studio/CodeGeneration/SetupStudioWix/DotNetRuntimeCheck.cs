using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// NOTE: this file intentionally does NOT import WixSharp or Microsoft.Win32.
// Both define a "RegistryHive" type and would be ambiguous. Registry types are fully qualified instead.

namespace EntitySpaces.Installer
{
    /// <summary>
    /// Detects the .NET Desktop Runtime (x64) installed on the machine.
    /// This code runs inside the MSI at install time (.NET Framework host).
    /// </summary>
    static class DotNetRuntimeCheck
    {
        // Each installed runtime adds a value named after its version (e.g. "10.0.1") under this key
        const string RegistryKeyPath =
            @"SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App";

        /// <summary>
        /// True when a release of the .NET Desktop Runtime x64 with the given major version is installed.
        /// Preview/RC builds are ignored. "detail" describes what was found, for the install log.
        /// </summary>
        public static bool IsInstalled(int requiredMajor, out string detail)
        {
            var found = GetVersionsFromRegistry()
                .Concat(GetVersionsFromDisk())
                .Distinct()
                .OrderBy(v => v)
                .ToList();

            detail = found.Count == 0
                ? "no .NET Desktop Runtime x64 releases found"
                : "found " + string.Join(", ", found.Select(v => v.ToString()));

            return found.Any(v => v.Major == requiredMajor);
        }

        /// <summary>Primary source: the registry written by the runtime installer (64-bit view).</summary>
        static List<Version> GetVersionsFromRegistry()
        {
            try
            {
                // Explicit 64-bit view: the installer host process may be 32-bit
                using (var hklm = Microsoft.Win32.RegistryKey.OpenBaseKey(
                    Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64))
                using (var key = hklm.OpenSubKey(RegistryKeyPath))
                {
                    if (key == null)
                        return new List<Version>();

                    return key.GetValueNames()
                        .Select(ParseRelease)
                        .Where(v => v != null)
                        .ToList();
                }
            }
            catch
            {
                return new List<Version>();
            }
        }

        /// <summary>Fallback: version folders under "%ProgramFiles%\dotnet\shared\Microsoft.WindowsDesktop.App".</summary>
        static List<Version> GetVersionsFromDisk()
        {
            try
            {
                // ProgramW6432 is the 64-bit Program Files even from a 32-bit process
                string programFiles = Environment.GetEnvironmentVariable("ProgramW6432")
                                      ?? Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

                string shared = Path.Combine(programFiles, "dotnet", "shared", "Microsoft.WindowsDesktop.App");

                if (!Directory.Exists(shared))
                    return new List<Version>();

                return Directory.GetDirectories(shared)
                    .Select(Path.GetFileName)
                    .Select(ParseRelease)
                    .Where(v => v != null)
                    .ToList();
            }
            catch
            {
                return new List<Version>();
            }
        }

        /// <summary>Parses "10.0.1"; returns null for previews ("10.0.0-rc.2...") or invalid names.</summary>
        static Version ParseRelease(string name)
        {
            if (string.IsNullOrEmpty(name) || name.Contains("-"))
                return null;

            Version version;
            return Version.TryParse(name, out version) ? version : null;
        }
    }
}
