using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WixSharp;
using WixSharp.CommonTasks;

namespace EntitySpaces.Installer
{
    public class Program
    {
        // =====================================================================
        //  1) SOURCE PATHS (where the installer READS files from)
        // =====================================================================
        static class SourcePaths
        {
            public const string ProjectRoot = @"D:\Work\src\04_proyectos\Internos\EntitySpaces";

            // Publish output of the application (net10 x64)
            public static readonly string Publish = Path.Combine(ProjectRoot, "publish_EntitySpaceStudio");
            public static readonly string[] PublishPatterns = { "*.exe", "*.dll", "*.json", "*.config" };

            // Repository
            public static readonly string Studio = Path.Combine(ProjectRoot, @"git-paul-EntitySpaces\Studio");
            public static readonly string CodeGeneration = Path.Combine(Studio, "CodeGeneration");
            public static readonly string Setup = Path.Combine(CodeGeneration, "SetupStudio");

            // Documentation
            public static readonly string Documentation = Path.Combine(Studio, "Documentation");
            public const string DocumentationPattern = "*.pdf";

            // Loose files
            public static readonly string EsLanguagesXml = Path.Combine(Setup, @"_ConfigFiles\esLanguages.xml");
            public static readonly string EsSettingsXml = Path.Combine(Setup, @"_ProgramData\esSettings.xml");
            public static readonly string EsUserDataXml = Path.Combine(Setup, @"_ProgramData\esUserData.xml");

            // CodeGeneration subfolders
            public static readonly string BinUIAddIns = Path.Combine(CodeGeneration, @"Bin\UIAddIns");
            public static readonly string CustomBase = Path.Combine(CodeGeneration, "CustomBase");

            public static readonly string Templates = Path.Combine(CodeGeneration, "Templates");
            public static readonly string TemplatesJavaScript = Path.Combine(Templates, "JavaScript");
            public static readonly string TemplatesStoredProcedures = Path.Combine(Templates, "StoredProcedures");
            public static readonly string TemplatesDotNetNuke = Path.Combine(TemplatesStoredProcedures, "DotNetNuke");

            // Installer branding images (kept inside the installer project, folder "Images")
            public static readonly string Images = Path.Combine(Studio, @"CodeGeneration\SetupStudioWix\Images");
            public static readonly string AppIcon = Path.Combine(Images, "EntitySpaces.ico");   // Programs and Features
            public static readonly string BannerImage = Path.Combine(Images, "Banner.bmp");     // 493x58, top of dialogs
            public static readonly string SideImage = Path.Combine(Images, "Welcome_156x312.bmp"); // 156x312, left side of the dialogs
            public static readonly string LicenseFile = Path.Combine(Images, "License.rtf");        // text of the license page (RTF)

            // Languages and subfolders repeated in CustomBase and Templates
            public static readonly string[] Languages = { "C#", "VB" };
            public static readonly string[] TemplateSubfolders =
                { "Custom", "Generated", "MultiProvider", "ProxyStub", "RIA", "WCFServices", "Web" };
        }

        // =====================================================================
        //  2) TARGET PATHS AND NAMES (what the MSI INSTALLS on the user's machine)
        // =====================================================================
        static class TargetPaths
        {
            public const string AppName = "EntitySpaces 2026";
            public const string AppExe = "EntitySpaces.exe";

            // Exact root path expected by the application
            public const string InstallFolder = @"%ProgramFiles64Folder%\EntitySpaces\EntitySpaces 2026";
            // WixSharp does not recognize %ProgramData%; the right name is CommonAppDataFolder
            public const string ProgramDataFolder = @"%CommonAppDataFolder%\EntitySpaces\ES2026";
            public const string DesktopFolder = @"%Desktop%";
            public const string StartMenuFolder = @"%ProgramMenuFolder%\EntitySpaces 2026";

            // Subfolders inside the install folder
            public const string SubCodeGeneration = "CodeGeneration";
            public const string SubBin = "Bin";
            public const string SubUIAddIns = "UIAddIns";
            public const string SubCustomBase = "CustomBase";
            public const string SubTemplates = "Templates";
            public const string SubJavaScript = "JavaScript";
            public const string SubStoredProcedures = "StoredProcedures";
            public const string SubDotNetNuke = "DotNetNuke";
            public const string SubDocumentation = "Documentation";

            // Registry
            public const string RegistryHkcu = @"Software\EntitySpaces 2026";
            public const string RegistryHklm = @"Software\EntitySpaces";
            public const int RegistryProjectSlots = 10; // Project_1 ... Project_10 placeholders in HKCU

            // Shortcut target (built from the constants above)
            public const string ShortcutTarget =
                "[INSTALLDIR]" + SubCodeGeneration + @"\" + SubBin + @"\" + AppExe;
        }

        // =====================================================================
        //  3) VERSIONING
        // =====================================================================

        // Release version shown to people: YYYY.MM.NN
        //   YYYY = year, MM = month, NN = release number within that month.
        // First release for .NET 10 x64: 2026.11.00. For another release in the same month use 2026.11.01, etc.
        // It can also be passed as the first command-line argument (handy for CI):
        //   EntitySpaces.Installer.exe 2026.11.01
        const string DefaultReleaseVersion = "2026.11.00";

        // UpgradeCode: must NEVER change between versions.
        static readonly Guid UpgradeCode = new Guid("D521D279-3D08-4C91-B500-2A8B5A045C60");

        /// <summary>Release version from the command line if present, otherwise the default above.</summary>
        static string ResolveReleaseVersion()
        {
            string[] args = Environment.GetCommandLineArgs(); // [0] is the exe path
            return args.Length > 1 && !string.IsNullOrWhiteSpace(args[1])
                ? args[1].Trim()
                : DefaultReleaseVersion;
        }

        /// <summary>
        /// Converts "YYYY.MM.NN" into a valid MSI ProductVersion.
        /// MSI limits: major and minor up to 255, build up to 65535, so 2026.11.00 becomes 26.11.0
        /// (year - 2000, month, release number). Versions keep increasing correctly until the year 2255.
        /// </summary>
        static Version ToMsiVersion(string releaseVersion)
        {
            string[] parts = (releaseVersion ?? "").Split('.');
            int year, month, release;

            if (parts.Length != 3
                || !int.TryParse(parts[0], out year)
                || !int.TryParse(parts[1], out month)
                || !int.TryParse(parts[2], out release)
                || year < 2000 || year > 2255
                || month < 1 || month > 12
                || release < 0 || release > 65535)
            {
                throw new ArgumentException(
                    "Invalid release version '" + releaseVersion + "'. Expected YYYY.MM.NN (for example 2026.11.00).");
            }

            return new Version(year - 2000, month, release);
        }

        /// <summary>
        /// ProductCode derived from the UpgradeCode and the release version: the same version always
        /// produces the same ProductCode, and every new version produces a different one
        /// (required for major upgrades).
        /// </summary>
        static Guid ProductCodeFor(string releaseVersion)
        {
            using (var sha1 = System.Security.Cryptography.SHA1.Create())
            {
                byte[] hash = sha1.ComputeHash(
                    Encoding.UTF8.GetBytes(UpgradeCode.ToString("D") + "|" + releaseVersion));

                byte[] bytes = new byte[16];
                Array.Copy(hash, bytes, 16);

                // Name-based UUID layout (version 5)
                bytes[6] = (byte)((bytes[6] & 0x0F) | 0x50);
                bytes[8] = (byte)((bytes[8] & 0x3F) | 0x80);

                return new Guid(bytes);
            }
        }

        // =====================================================================
        //  4) OTHER SETTINGS
        // =====================================================================
        const string LogPath = @"C:\Temp\wix_errors.txt";

        // Shown as "Publisher" in Programs and Features. TODO: confirm the exact company/team name.
        const string Publisher = "EntitySpaces";
        const string TemplateUiDllName = "EntitySpaces.TemplateUI.dll";

        // .NET prerequisite (checked when the installer starts, see OnLoad)
        const int RequiredDotNetMajor = 10;
        const string DotNetDownloadUrl = "https://dotnet.microsoft.com/download/dotnet/10.0";
        const string DotNetMissingMessage =
            "EntitySpaces 2026 requires the .NET 10 Desktop Runtime (x64), which was not found on this computer.";

        static StreamWriter _logWriter;

        // Source-folder verification results (filled by CollectFiles)
        static readonly List<string> MissingPaths = new List<string>();
        static readonly List<string> EmptyMatches = new List<string>();

        // =====================================================================
        //  Entry point
        // =====================================================================
        static void Main()
        {
            InitLog();

            try
            {
                // Required by WiX v7 (OSMF license) before building
                WixTools.AcceptEulaFor = "wix7";

                Console.WriteLine("Start: " + DateTime.Now);
                Compiler.VerboseOutput = true;

                string releaseVersion = ResolveReleaseVersion();
                Console.WriteLine("Release version: " + releaseVersion +
                                  " (MSI ProductVersion " + ToMsiVersion(releaseVersion) + ")");

                PrintPaths();

                Console.WriteLine();
                Console.WriteLine("=== SOURCE FILE VERIFICATION ===");

                var project = CreateProject(releaseVersion);
                ConfigureProject(project, releaseVersion);
                ConfigureManagedUI(project);
                ConfigureBranding(project);

                // Throws if any source folder or image is missing
                PrintVerificationSummary();

                Console.WriteLine("Building MSI...");
                string msi = project.BuildMsi();

                if (string.IsNullOrEmpty(msi) || !System.IO.File.Exists(msi))
                    throw new Exception("The WiX engine failed to build the MSI. Check the WIX0091 errors in the log.");

                Console.WriteLine("Installer built successfully!");
                Console.WriteLine("MSI: " + msi);
            }
            catch (Exception ex)
            {
                Console.WriteLine("=========================================");
                Console.WriteLine("FATAL ERROR WHILE BUILDING THE MSI!");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("=========================================");

                // Non-zero exit code so MSBuild / CI detects the failure
                Environment.ExitCode = 1;
            }
            finally
            {
                CloseLog();
            }
        } // end Main

        // =====================================================================
        //  Project construction
        // =====================================================================

        /// <summary>Creates the project with every folder, file, shortcut and registry key.</summary>
        static ManagedProject CreateProject(string releaseVersion)
        {
            return new ManagedProject(TargetPaths.AppName,
                BuildInstallDir(),
                BuildProgramDataDir(),
                BuildShortcutDir(TargetPaths.DesktopFolder),
                BuildShortcutDir(TargetPaths.StartMenuFolder),
                BuildUserRegistry(),
                BuildMachineRegistry(releaseVersion));
        }

        /// <summary>Installer settings: platform, scope, version, identity and upgrade behavior.</summary>
        static void ConfigureProject(ManagedProject project, string releaseVersion)
        {
            project.Platform = Platform.x64;
            project.Scope = InstallScope.perMachine;

            // Version and identity
            project.Version = ToMsiVersion(releaseVersion);
            project.GUID = ProductCodeFor(releaseVersion);
            project.UpgradeCode = UpgradeCode;

            // Output file name, e.g. EntitySpaces-2026.11.00-x64.msi
            project.OutFileName = "EntitySpaces-" + releaseVersion + "-x64";

            // Newer versions replace older ones. The old product is removed AFTER the new files are
            // installed, so files shared by both versions (such as the user's ProgramData XML files)
            // are not deleted and re-created.
            project.MajorUpgrade = new MajorUpgrade
            {
                Schedule = UpgradeSchedule.afterInstallExecute,
                DowngradeErrorMessage = "A newer version of [ProductName] is already installed."
            };

            // Keeps the generated .wxs files for debugging (development only)
            project.PreserveTempFiles = true;

            // .NET 10 Desktop Runtime check, executed when the setup starts
            project.Load += OnLoad;
        }

        /// <summary>Wires the custom dialogs into the managed UI.</summary>
        static void ConfigureManagedUI(ManagedProject project)
        {
            project.ManagedUI = new ManagedUI();

            project.ManagedUI.InstallDialogs
                .Add<Dialogs.WelcomeDialog>()
                .Add<Dialogs.LicenceDialog>()
                .Add<Dialogs.SetupTypeDialog>()
                .Add<Dialogs.FeaturesDialog>()
                .Add<Dialogs.InstallDirDialog>()
                .Add<Dialogs.ProgressDialog>()
                .Add<Dialogs.ExitDialog>();

            project.ManagedUI.ModifyDialogs
                .Add<Dialogs.MaintenanceTypeDialog>()
                .Add<Dialogs.FeaturesDialog>()
                .Add<Dialogs.ProgressDialog>()
                .Add<Dialogs.ExitDialog>();
        }

        /// <summary>Icon and publisher shown in Programs and Features, plus the images of the setup dialogs.</summary>
        static void ConfigureBranding(ManagedProject project)
        {
            Console.WriteLine();
            Console.WriteLine("=== BRANDING IMAGES ===");
            RequireFile(SourcePaths.AppIcon);
            RequireFile(SourcePaths.BannerImage);
            RequireFile(SourcePaths.SideImage);
            RequireFile(SourcePaths.LicenseFile);

            // "Programs and Features": icon (.ico with several sizes) and publisher
            project.ControlPanelInfo.ProductIcon = SourcePaths.AppIcon;
            project.ControlPanelInfo.Manufacturer = Publisher;

            // Setup dialogs: banner on top of every page except the first, and the image docked on the
            // left side of the welcome/exit dialogs. ManagedUI validates that this one has a ~156x312 ratio.
            project.BannerImage = SourcePaths.BannerImage;
            project.BackgroundImage = SourcePaths.SideImage;

            // License agreement shown on the second page (replaces the WixSharp default text)
            project.LicenceFile = SourcePaths.LicenseFile;
        }

        /// <summary>Logs the status of a single required file and records it if it is missing.</summary>
        static void RequireFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                Console.WriteLine("OK            " + path);
            }
            else
            {
                Console.WriteLine("MISSING       " + path);
                MissingPaths.Add(path);
            }
        }

        // =====================================================================
        //  Folder builders
        // =====================================================================

        /// <summary>Install root: CodeGeneration + Documentation.</summary>
        static Dir BuildInstallDir()
        {
            return new Dir(TargetPaths.InstallFolder,
                BuildCodeGenerationDir(),
                BuildDocumentationDir());
        }

        /// <summary>CodeGeneration: Bin, CustomBase, Templates and esLanguages.xml.</summary>
        static Dir BuildCodeGenerationDir()
        {
            var items = new List<WixEntity>
            {
                BuildBinDir(),
                BuildCustomBaseDir(),
                BuildTemplatesDir()
            };
            items.AddRange(CollectFiles(SourcePaths.EsLanguagesXml));

            return new Dir(new Id(TargetPaths.SubCodeGeneration), items.ToArray());
        }

        /// <summary>Bin: all published binaries plus the UIAddIns subfolder.</summary>
        static Dir BuildBinDir()
        {
            var items = SourcePaths.PublishPatterns
                .SelectMany(pattern => CollectFiles(Path.Combine(SourcePaths.Publish, pattern)))
                .ToList();

            items.Add(BuildUIAddInsDir());

            return new Dir(new Id(TargetPaths.SubBin), items.ToArray());
        }

        /// <summary>
        /// UIAddIns: TemplateUI.dll from the publish output, plus whatever the repository
        /// folder contains. The repository folder is optional and skipped if missing.
        /// </summary>
        static Dir BuildUIAddInsDir()
        {
            var items = new List<WixEntity>();

            string templateUiDll = Path.Combine(SourcePaths.Publish, TemplateUiDllName);
            if (System.IO.File.Exists(templateUiDll))
                items.Add(new WixSharp.File(templateUiDll));

            if (Directory.Exists(SourcePaths.BinUIAddIns))
            {
                items.AddRange(GetSourceFiles(SourcePaths.BinUIAddIns, "*.*")
                    .Where(f => !Path.GetFileName(f).Equals(TemplateUiDllName, StringComparison.OrdinalIgnoreCase))
                    .Select(f => (WixEntity)new WixSharp.File(f)));
            }

            return new Dir(new Id(TargetPaths.SubUIAddIns), items.ToArray());
        }

        /// <summary>CustomBase\{C#, VB}</summary>
        static Dir BuildCustomBaseDir()
        {
            var items = SourcePaths.Languages
                .Select(lang => (WixEntity)DirWithFiles(lang, Path.Combine(SourcePaths.CustomBase, lang)))
                .ToArray();

            return new Dir(new Id(TargetPaths.SubCustomBase), items);
        }

        /// <summary>Templates: one folder per language plus JavaScript and StoredProcedures.</summary>
        static Dir BuildTemplatesDir()
        {
            var items = new List<WixEntity>();

            items.AddRange(SourcePaths.Languages.Select(lang => (WixEntity)BuildLanguageTemplatesDir(lang)));
            items.Add(DirWithFiles(TargetPaths.SubJavaScript, SourcePaths.TemplatesJavaScript));
            items.Add(DirWithFiles(TargetPaths.SubStoredProcedures, SourcePaths.TemplatesStoredProcedures,
                DirWithFiles(TargetPaths.SubDotNetNuke, SourcePaths.TemplatesDotNetNuke)));

            return new Dir(new Id(TargetPaths.SubTemplates), items.ToArray());
        }

        static Dir BuildDocumentationDir()
        {
            return new Dir(new Id(TargetPaths.SubDocumentation),
                CollectFiles(Path.Combine(SourcePaths.Documentation, SourcePaths.DocumentationPattern)));
        }

        /// <summary>
        /// ProgramData: user settings files. They are flagged NeverOverwrite so a reinstall
        /// does not replace files the user already has.
        /// </summary>
        static Dir BuildProgramDataDir()
        {
            var items = new List<WixEntity>();

            foreach (string xml in new[] { SourcePaths.EsSettingsXml, SourcePaths.EsUserDataXml })
            {
                if (System.IO.File.Exists(xml))
                    items.Add(new WixSharp.File(xml) { NeverOverwrite = true });
                else
                    Console.WriteLine("SKIPPED (not found)  " + xml);
            }

            return new Dir(new Id(TargetPaths.ProgramDataFolder), items.ToArray());
        }

        /// <summary>Folder that holds the application shortcut (desktop or start menu).</summary>
        static Dir BuildShortcutDir(string folder)
        {
            return new Dir(folder,
                new ExeFileShortcut(TargetPaths.AppName, TargetPaths.ShortcutTarget, "")
                {
                    // Directory ID of the install folder (no brackets)
                    WorkingDirectory = "INSTALLDIR"
                });
        }

        // =====================================================================
        //  Registry
        //  Keys must be created BEFORE the project and passed to its constructor;
        //  a RegKey that is never added to the project is silently ignored.
        // =====================================================================

        /// <summary>HKCU: install dir plus empty Project_1..Project_N placeholders.</summary>
        static RegKey BuildUserRegistry()
        {
            var values = new List<RegValue>
            {
                new RegValue(new Id("Reg_Install_Dir"), "Install_Dir", "[INSTALLDIR]")
            };

            for (int i = 1; i <= TargetPaths.RegistryProjectSlots; i++)
                values.Add(new RegValue(new Id($"Reg_Project_{i}"), $"Project_{i}", ""));

            return new RegKey(
                (Feature)null,
                RegistryHive.CurrentUser,
                TargetPaths.RegistryHkcu,
                values.ToArray());
        }

        /// <summary>HKLM: install dir and release version (machine-wide, readable by every user).</summary>
        static RegKey BuildMachineRegistry(string releaseVersion)
        {
            return new RegKey(
                (Feature)null,
                RegistryHive.LocalMachine,
                TargetPaths.RegistryHklm,
                new RegValue(new Id("Reg_HKLM_Install_Dir"), "Install_Dir", "[INSTALLDIR]"),
                new RegValue(new Id("Reg_HKLM_Version"), "Version", releaseVersion));
        }

        // =====================================================================
        //  Prerequisites
        // =====================================================================

        /// <summary>
        /// Runs when the setup starts (interactive and silent). Aborts the installation if the
        /// .NET Desktop Runtime required by the application is not installed.
        /// NOTE: this runs inside the MSI at install time, not while building the MSI.
        /// </summary>
        static void OnLoad(SetupEventArgs e)
        {
            // Only check on a first install; skip repair, modify and uninstall
            if (!string.IsNullOrEmpty(e.Session["Installed"]))
                return;

            string detail;
            if (DotNetRuntimeCheck.IsInstalled(RequiredDotNetMajor, out detail))
            {
                e.Session.Log("DotNet check OK: {0}", detail);
                return;
            }

            e.Session.Log("DotNet check FAILED: {0}", detail);

            bool userNotified = ShowDotNetMissingMessage(e);

            // UserExit when the user has already seen the message, Failure for silent installs
            e.Result = userNotified
                ? WixToolset.Dtf.WindowsInstaller.ActionResult.UserExit
                : WixToolset.Dtf.WindowsInstaller.ActionResult.Failure;
        }

        /// <summary>
        /// Shows the "runtime missing" message and offers to open the download page.
        /// Returns false when no message could be shown (silent install or non-interactive process).
        /// </summary>
        static bool ShowDotNetMissingMessage(SetupEventArgs e)
        {
            int uiLevel;
            int.TryParse(e.Session["UILevel"], out uiLevel);

            // UILevel 2 = no UI. Also skip when running in a non-interactive process (MSI service).
            if (uiLevel <= 2 || !Environment.UserInteractive)
                return false;

            try
            {
                var answer = System.Windows.Forms.MessageBox.Show(
                    DotNetMissingMessage + "\n\n" + DotNetDownloadUrl + "\n\nOpen the download page now?",
                    TargetPaths.AppName,
                    System.Windows.Forms.MessageBoxButtons.YesNo,
                    System.Windows.Forms.MessageBoxIcon.Error);

                if (answer == System.Windows.Forms.DialogResult.Yes)
                    System.Diagnostics.Process.Start(DotNetDownloadUrl);

                return true;
            }
            catch (Exception ex)
            {
                e.Session.Log("Could not show the .NET message: {0}", ex.Message);
                return false;
            }
        }

        // =====================================================================
        //  Structure helpers
        // =====================================================================

        /// <summary>Target folder whose content is the files of a source folder (plus optional subfolders).</summary>
        static Dir DirWithFiles(string targetName, string sourceFolder, params WixEntity[] subfolders)
        {
            var items = new List<WixEntity>();
            items.AddRange(CollectFiles(Path.Combine(sourceFolder, "*.*")));
            items.AddRange(subfolders);
            return new Dir(new Id(targetName), items.ToArray());
        }

        /// <summary>Templates\{language}: the .est files in its root plus the Custom, Generated, ... subfolders.</summary>
        static Dir BuildLanguageTemplatesDir(string language)
        {
            string languageRoot = Path.Combine(SourcePaths.Templates, language);

            var items = new List<WixEntity>();

            // Files located directly in the language folder
            // (e.g. "Generated - Classes Master (C#).est")
            items.AddRange(CollectFiles(Path.Combine(languageRoot, "*.est")));

            // Subfolders (Custom, Generated, MultiProvider, ...)
            items.AddRange(SourcePaths.TemplateSubfolders
                .Select(s => (WixEntity)DirWithFiles(s, Path.Combine(languageRoot, s))));

            return new Dir(new Id(language), items.ToArray());
        }

        // =====================================================================
        //  Source verification
        // =====================================================================

        /// <summary>
        /// Returns the files matching "folder\pattern" as WixSharp files and logs the status of the path.
        /// Missing folders and empty matches are recorded for PrintVerificationSummary.
        /// </summary>
        static WixEntity[] CollectFiles(string path)
        {
            // Only the folder is normalized (wildcards must not go through GetFullPath)
            string dir = Path.GetFullPath(Path.GetDirectoryName(path));
            string pattern = Path.GetFileName(path);
            string full = Path.Combine(dir, pattern);

            if (!Directory.Exists(dir))
            {
                Console.WriteLine("MISSING       " + full);
                MissingPaths.Add(dir);
                return new WixEntity[0];
            }

            var files = GetSourceFiles(dir, pattern);

            if (files.Length == 0)
            {
                Console.WriteLine("NO FILES      " + full);
                EmptyMatches.Add(full);
                return new WixEntity[0];
            }

            Console.WriteLine("OK (" + files.Length + ")  " + full);

            return files.Select(f => (WixEntity)new WixSharp.File(f)).ToArray();
        }

        /// <summary>Top-level files of a folder that match a pattern, skipping Subversion metadata.</summary>
        static string[] GetSourceFiles(string dir, string pattern)
        {
            return Directory.GetFiles(dir, pattern, SearchOption.TopDirectoryOnly)
                .Where(f => !IsSvnPath(f))
                .ToArray();
        }

        static bool IsSvnPath(string path)
        {
            return path.Contains(@"\.svn\") || path.Contains("/.svn/");
        }

        /// <summary>Prints warnings for empty matches and fails if any source folder is missing.</summary>
        static void PrintVerificationSummary()
        {
            Console.WriteLine();

            var empty = EmptyMatches.Distinct().ToList();
            if (empty.Count > 0)
            {
                Console.WriteLine("WARNING: folders that exist but contain no matching files (" + empty.Count + "):");
                foreach (var p in empty)
                    Console.WriteLine("  " + p);
            }

            var missing = MissingPaths.Distinct().ToList();
            if (missing.Count > 0)
            {
                Console.WriteLine("SOURCE PATHS NOT FOUND (" + missing.Count + "):");
                foreach (var p in missing)
                    Console.WriteLine("  " + p);

                throw new DirectoryNotFoundException(
                    missing.Count + " source path(s) are missing. Check the log.");
            }
        }

        static void PrintPaths()
        {
            Console.WriteLine();
            Console.WriteLine("=== SOURCE PATHS (read) ===");
            foreach (var p in new[] { SourcePaths.Publish, SourcePaths.Studio, SourcePaths.CodeGeneration, SourcePaths.Setup, SourcePaths.Documentation })
                Console.WriteLine((Directory.Exists(p) ? "OK         " : "MISSING    ") + p);

            Console.WriteLine();
            Console.WriteLine("=== TARGET PATHS (installed by the MSI) ===");
            Console.WriteLine("  Install     : " + TargetPaths.InstallFolder);
            Console.WriteLine("  ProgramData : " + TargetPaths.ProgramDataFolder);
            Console.WriteLine("  Desktop     : " + TargetPaths.DesktopFolder);
            Console.WriteLine("  Start menu  : " + TargetPaths.StartMenuFolder);
            Console.WriteLine("  HKCU        : " + TargetPaths.RegistryHkcu);
            Console.WriteLine("  HKLM        : " + TargetPaths.RegistryHklm);
        }

        // =====================================================================
        //  Log
        // =====================================================================

        /// <summary>Mirrors everything written to the console into the log file.</summary>
        static void InitLog()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(LogPath));
                _logWriter = new StreamWriter(LogPath, false, Encoding.UTF8) { AutoFlush = true };
                Console.SetOut(new TeeWriter(Console.Out, _logWriter));
                Console.SetError(new TeeWriter(Console.Error, _logWriter));
            }
            catch (Exception ex)
            {
                // If the log cannot be created we keep going with the console only
                Console.WriteLine("Could not create the log file at " + LogPath + ": " + ex.Message);
            }
        }

        static void CloseLog()
        {
            try { _logWriter?.Flush(); _logWriter?.Dispose(); } catch { }
        }
    } // end Program

    /// <summary>Duplicates console output into a file.</summary>
    class TeeWriter : TextWriter
    {
        readonly TextWriter _console;
        readonly TextWriter _file;

        public TeeWriter(TextWriter console, TextWriter file)
        {
            _console = console;
            _file = file;
        }

        public override Encoding Encoding => _console.Encoding;

        public override void Write(char value)
        {
            _console.Write(value);
            _file.Write(value);
        }

        public override void Write(string value)
        {
            _console.Write(value);
            _file.Write(value);
        }

        public override void Flush()
        {
            _console.Flush();
            _file.Flush();
        }
    }
}
