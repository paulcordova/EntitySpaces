
using System;
using System.IO;

namespace EntitySpaces
{
    public static class VersionConstants
    {
        // ================================================================
        // Shared version info (used by providers and Studio base)
        // ================================================================
        public const string Year = "2026";
        public const string ReleaseName = "ES2026";

        // ================================================================
        // Studio-specific version (can be incremented independently)
        // Format: Year.Month.Day.Build  (e.g., 2026.7.30.0)
        // ================================================================
        public const string StudioVersion = "2026.7.0030.0";

        // For backward compatibility, the general 'Version' constant
        // now points to StudioVersion. If you need a separate version
        // for providers, create a new constant like 'ProviderVersion'.
        public const string Version = StudioVersion;

        // Product names (Studio-specific)
        public const string ProductName = "EntitySpaces Studio " + Year;
        public const string ProductNameShort = "EntitySpacesStudio" + Year;

        // Registry path (still based on Year, not StudioVersion)
        public const string RegistryPath = @"Software\EntitySpaces " + Year;

        // Assembly attributes (no interpolation needed)
        public const string Copyright = "Copyright © EntitySpaces, LLC. 2005 - " + Year;
        public const string AssemblyDescription = "The EntitySpaces Studio Stand Alone Version";
        public const string AssemblyTitle = "EntitySpaces";
        public const string AssemblyProduct = "EntitySpacesArchitecture";
        public const string TemplateUIDescription = "The EntitySpaces " + Year + " Template User Interface";
    }

    public static class VersionInfo
    {
        // Reuse constants from VersionConstants
        public const string Year = VersionConstants.Year;
        public const string ReleaseName = VersionConstants.ReleaseName;
        public const string Version = VersionConstants.Version; // Now StudioVersion

        // Derived properties (computed at runtime, but use constants)
        public static string ProductName => VersionConstants.ProductName;
        public static string ProductNameShort => VersionConstants.ProductNameShort;
        public static string RegistryPath => VersionConstants.RegistryPath;
        public static string RegistryKey => VersionConstants.RegistryPath;

        public static string AppDataPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "EntitySpaces",
            ReleaseName
        );

        public static string InstallPathDefault => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            $"EntitySpaces {Year}"
        );
    }


}