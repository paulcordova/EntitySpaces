# EntitySpaces Studio 2026 — SetupStudioWix

Installer project for **EntitySpaces Studio 2026**.  
This folder contains the WiX/WixSharp source code that builds the MSI, the
branding assets, the build script and the build output folders.

Everything that is **source** lives in this folder and is tracked by Git.  
Everything that is **build output** lives in dedicated subfolders
(`publish_EntitySpacesStudio\` and `installer_EntitySpacesStudio\`) and is
ignored by Git.

---

## Table of contents

1. [Folder layout](#1-folder-layout)
2. [What each item is](#2-what-each-item-is)
3. [End-to-end build pipeline](#3-end-to-end-build-pipeline)
4. [Paths contract between Program.cs and the script](#4-paths-contract-between-programcs-and-the-script)
5. [How to build](#5-how-to-build)
6. [Versioning and ProductCode](#6-versioning-and-productcode)
7. [What the MSI installs](#7-what-the-msi-installs)
8. [Branding and UI](#8-branding-and-ui)
9. [Prerequisite check (.NET 10)](#9-prerequisite-check-net-10)
10. [Logs](#10-logs)
11. [Troubleshooting](#11-troubleshooting)
12. [What must never be committed](#12-what-must-never-be-committed)

---

## 1. Folder layout

```
SetupStudioWix\
│
├── EntitySpaces.Installer.csproj        # installer project (source, Git)
├── Program.cs                           # WixSharp build logic (source, Git)
├── Build-EntitySpaces.ps1               # end-to-end build script (source, Git)
├── .gitignore                           # excludes every build output
├── README.md                            # this file
│
├── Images\                              # branding assets (source, Git)
│   ├── EntitySpaces.ico                 # ico for "Programs and Features"
│   ├── Banner.bmp                       # 493x58, top banner of the dialogs
│   ├── Welcome_156x312.bmp              # 156x312, left-side image of the dialogs
│   └── License.rtf                      # license text of the license page
│
├── publish_EntitySpacesStudio\          # BUILD OUTPUT (gitignored)
│   └── EntitySpaces.exe, *.dll, *.json, *.config ...
│
├── installer_EntitySpacesStudio\        # BUILD OUTPUT (gitignored)
│   ├── EntitySpaces-YYYY.MM.NN-x64.msi
│   ├── EntitySpaces-YYYY.MM.NN-x64.wxs
│   ├── WixSharp.CA.dll
│   ├── WixSharp.UI.CA.dll
│   ├── EmbeddedUI.config
│   └── CustomAction.config
│
├── bin\                                 # .NET build output (gitignored)
├── obj\                                 # .NET build output (gitignored)
│
├── BuildLog.txt                         # script log (gitignored)
└── wix_errors.txt                       # installer program log (gitignored)
```

---

## 2. What each item is

| Item | Type | Role |
|------|------|------|
| `EntitySpaces.Installer.csproj` | Source (Git) | .NET Framework project that compiles `Program.cs` into `EntitySpaces.Installer.exe`. References WixSharp + WiX Toolset. |
| `Program.cs` | Source (Git) | The WixSharp build logic. Defines every folder, file, shortcut, registry key and UI page of the MSI. Also runs the `.NET 10` prerequisite check at install time. |
| `Build-EntitySpaces.ps1` | Source (Git) | PowerShell script that runs the whole pipeline: clean → `dotnet publish` → `dotnet build` → `dotnet run` → verify MSI. |
| `Images\` | Source (Git) | Branding: icon, banner, side image and license text used by the installer UI. |
| `publish_EntitySpacesStudio\` | Build output | Filled by `dotnet publish` from the `StandAlone` project. **Read** by `Program.cs` to package the app into the MSI. |
| `installer_EntitySpacesStudio\` | Build output | Filled by `Program.cs` (WixSharp). Contains the final `.msi` plus every WixSharp helper file. |
| `bin\`, `obj\` | Build output | Standard .NET compilation artifacts of the installer project itself. |
| `BuildLog.txt` | Build output | Timestamped log written by `Build-EntitySpaces.ps1`. |
| `wix_errors.txt` | Build output | Full log of the installer program (`Program.cs`), written into `ES_LOG_DIR`. |

> **Note:** `publish_EntitySpacesStudio` and `installer_EntitySpacesStudio` are
> cleaned and recreated on every build. Do not store anything there that you
> want to keep. Put permanent documentation in this folder (`SetupStudioWix\`).

---

## 3. End-to-end build pipeline

The script `Build-EntitySpaces.ps1` runs these steps in order:

### Step 1 — Clean

```
Delete  SetupStudioWix\publish_EntitySpacesStudio\
Delete  SetupStudioWix\installer_EntitySpacesStudio\
Recreate both folders (empty)
```

### Step 2 — Publish the application

```powershell
cd ...\StandAlone
dotnet publish StandAlone.csproj -c Release -r win-x64 --self-contained false `
    -p:PlatformTarget=x64 -o ...\SetupStudioWix\publish_EntitySpacesStudio
```

Result: `publish_EntitySpacesStudio\` contains the full application
(`EntitySpaces.exe`, DLLs, JSON, config files).

### Step 3 — Compile the installer project

```powershell
cd ...\SetupStudioWix
dotnet build EntitySpaces.Installer.csproj -c Release
```

Result: `EntitySpaces.Installer.exe` in `bin\Release\net48\`.

### Step 4 — Run the installer project (this builds the MSI)

```powershell
dotnet run --project EntitySpaces.Installer.csproj -c Release
```

What `Program.cs` does:

1. Reads `publish_EntitySpacesStudio\` (application binaries).
2. Reads templates, custom base classes, documentation, `esLanguages.xml`, `esUserData.xml`.
3. Builds the MSI into `installer_EntitySpacesStudio\`.
4. Writes its full log to `wix_errors.txt` (via `ES_LOG_DIR`).

### Step 5 — Verify

The script looks for `*.msi` in `installer_EntitySpacesStudio\` and prints
the path on success. If no MSI is found, the script prints a warning and
points to `wix_errors.txt`.

---

## 4. Paths contract between `Program.cs` and the script

The two components **must agree** on these paths. If they diverge, the MSI is
built **without** the application binaries (empty installer).

| Path | Defined in `Build-EntitySpaces.ps1` | Defined in `Program.cs` |
|------|-------------------------------------|--------------------------|
| Publish folder | `$PublishDir = Join-Path $InstallerProjectDir "publish_EntitySpacesStudio"` | `SourcePaths.Publish = Path.Combine(InstallerDir, "publish_EntitySpacesStudio")` |
| Installer output folder | `$InstallerOutputDir = Join-Path $InstallerProjectDir "installer_EntitySpacesStudio"` | `SourcePaths.OutputDir = Path.Combine(InstallerDir, "installer_EntitySpacesStudio")` |
| Log folder | `$env:ES_LOG_DIR = $LogDir` (script folder) | `Environment.GetEnvironmentVariable("ES_LOG_DIR")` |

**Spelling matters:** `publish_EntitySpacesStudio` (with `Wix` and a trailing
`t`). An old version used `publish_EntitySpaceStudio` (without `Wix`, without
the final `t`) which caused:

```
NO FILES   ...\publish_EntitySpaceStudio\*.exe
NO FILES   ...\publish_EntitySpaceStudio\*.dll
```

and an MSI with no application inside. If you ever see those lines again,
search the whole repository for `publish_EntitySpaceStudio` and fix the typo.

---

## 5. How to build

### Interactive (recommended)

Right-click `Build-EntitySpaces.ps1` → **Run with PowerShell**.

Or from a PowerShell window:

```powershell
cd D:\Work\src\04_proyectos\Internos\EntitySpaces\git-paul-EntitySpaces\Studio\CodeGeneration\SetupStudioWix
.\Build-EntitySpaces.ps1
```

The script pauses at the end (`Press ENTER to close this window...`) so the
console stays open and you can read the result.

### Prerequisites

- Windows 10/11 x64
- .NET SDK 10 (for `StandAlone`)
- .NET Framework 4.8 developer pack (for `EntitySpaces.Installer`)
- WiX Toolset v7 installed at `C:\Program Files\WiX Toolset v7.0\`
- NuGet package `WixToolset.Dtf.CustomAction` 7.0.1 (restored automatically)

### Manual step-by-step (without the script)

```powershell
# 1. Publish
dotnet publish ...\StandAlone\StandAlone.csproj -c Release -r win-x64 `
    --self-contained false -p:PlatformTarget=x64 `
    -o ...\SetupStudioWix\publish_EntitySpacesStudio

# 2. Build the installer project
dotnet build ...\SetupStudioWix\EntitySpaces.Installer.csproj -c Release

# 3. Build the MSI
$env:ES_LOG_DIR = "...\SetupStudioWix"
dotnet run --project ...\SetupStudioWix\EntitySpaces.Installer.csproj -c Release
```

---

## 6. Versioning and ProductCode

### Release version (human-readable)

Format: `YYYY.MM.NN`

- `YYYY` = year
- `MM` = month
- `NN` = release number within that month (`00`, `01`, `02`, …)

Default in `Program.cs`:

```csharp
const string DefaultReleaseVersion = "2026.11.00";
```

Can be overridden from the command line (useful for CI):

```powershell
EntitySpaces.Installer.exe 2026.11.01
```

### MSI ProductVersion

MSI limits: major/minor ≤ 255, build ≤ 65535.  
`ToMsiVersion()` converts `2026.11.00` → `26.11.0` (year − 2000, month,
release). This stays valid and monotonically increasing until the year 2255.

### ProductCode

Derived deterministically from `UpgradeCode + releaseVersion`:

- Same version → same ProductCode (idempotent rebuilds).
- New version → new ProductCode (required for major upgrades).

### UpgradeCode

Never change it:

```
D521D279-3D08-4C91-B500-2A8B5A045C60
```

Changing it breaks the upgrade chain (old versions will not be replaced).

---

## 7. What the MSI installs

### Install folder

```
%ProgramFiles64Folder%\EntitySpaces\EntitySpaces 2026\
├── CodeGeneration\
│   ├── Bin\
│   │   ├── EntitySpaces.exe
│   │   ├── *.dll, *.json, *.config     (from publish_EntitySpacesStudio)
│   │   └── UIAddIns\
│   │       └── EntitySpaces.TemplateUI.dll
│   ├── CustomBase\
│   │   ├── C#\
│   │   └── VB\
│   ├── Templates\
│   │   ├── C#\           (.est + Custom, Generated, MultiProvider, ProxyStub, RIA, WCFServices, Web)
│   │   ├── VB\           (same structure)
│   │   ├── JavaScript\
│   │   └── StoredProcedures\
│   │       └── DotNetNuke\
│   └── esLanguages.xml
└── Documentation\
    └── *.pdf
```

### ProgramData

```
%CommonAppDataFolder%\EntitySpaces\ES2026\
└── esUserData.xml     (NeverOverwrite = true)
```

### Shortcuts

- Desktop: `EntitySpaces 2026` → `[INSTALLDIR]\CodeGeneration\Bin\EntitySpaces.exe`
- Start menu: `EntitySpaces 2026` → same target

### Registry

| Hive | Key | Values |
|------|-----|--------|
| HKCU | `Software\EntitySpaces 2026` | `Install_Dir`, `Project_1` … `Project_10` (empty placeholders) |
| HKLM | `Software\EntitySpaces` | `Install_Dir`, `Version` |

---

## 8. Branding and UI

All images live in `Images\` (source, Git-tracked). They are validated at
build time; if one is missing, `Program.cs` fails with
`SOURCE PATHS NOT FOUND`.

| File | Used as | Requirement |
|------|---------|-------------|
| `EntitySpaces.ico` | Product icon in *Programs and Features* | `.ico` with multiple sizes |
| `Banner.bmp` | Top banner of every dialog except the first | 493×58 px |
| `Welcome_156x312.bmp` | Left-side image of welcome/exit dialogs | ~156×312 px ratio (validated by `ManagedUI`) |
| `License.rtf` | License text of the license page | RTF |

### Dialog sequence (install)

1. Welcome
2. License
3. Setup type
4. Features
5. Install directory
6. Progress
7. Exit

### Dialog sequence (modify / repair / remove)

1. Maintenance type
2. Features
3. Progress
4. Exit

---

## 9. Prerequisite check (.NET 10)

At install time (not build time), `Program.cs` wires a `Load` handler
(`OnLoad`) that runs before the UI:

1. Skips the check if the product is already installed (`Installed` property
   set → repair / modify / uninstall).
2. Calls `DotNetRuntimeCheck.IsInstalled(10, out detail)`.
3. If missing:
   - **Interactive** (`UILevel > 2`): shows a `MessageBox` with the download
     URL and optionally opens it. Returns `ActionResult.UserExit`.
   - **Silent** (`UILevel ≤ 2`): returns `ActionResult.Failure` (non-zero
     exit code for automation).

Download URL:

```
https://dotnet.microsoft.com/download/dotnet/10.0
```

Constant to change if the required major version ever bumps:

```csharp
const int RequiredDotNetMajor = 10;
```

---

## 10. Logs

Two logs are produced per build, both in this folder:

| File | Written by | Content |
|------|-----------|---------|
| `BuildLog.txt` | `Build-EntitySpaces.ps1` | Timestamped PowerShell steps, `dotnet publish` / `build` / `run` output, final result |
| `wix_errors.txt` | `Program.cs` (via `ES_LOG_DIR`) | Full WixSharp log: source verification, MSI build, WiX compiler errors |

Both files are truncated at the start of each build.

To change the log folder, set `ES_LOG_DIR` before running the installer
program. If unset, `Program.cs` writes to `installer_EntitySpacesStudio\`.

### Anatomy of `wix_errors.txt`

```
Start: <date>
Release version: 2026.11.00 (MSI ProductVersion 26.11.0)

=== SOURCE PATHS (read) ===
OK   ...\publish_EntitySpacesStudio
OK   ...\Studio
...

=== TARGET PATHS (installed by the MSI) ===
  Install     : %ProgramFiles64Folder%\EntitySpaces\EntitySpaces 2026
  ...

=== SOURCE FILE VERIFICATION ===
OK (N)  ...\*.exe        ← must be > 0
OK (N)  ...\*.dll        ← must be > 0
...

=== BRANDING IMAGES ===
OK   ...\Images\EntitySpaces.ico
...

WARNING: folders that exist but contain no matching files (N):
  ...                   ← investigate these

Building MSI...
...
Installer built successfully!
MSI: ...\installer_EntitySpacesStudio\EntitySpaces-2026.11.00-x64.msi
```

---

## 11. Troubleshooting

### Symptom: MSI is generated but the install folder is empty

**Cause:** `Program.cs` is reading the wrong publish folder.  
**Check:** in `wix_errors.txt`, look for:

```
NO FILES   ...\publish_EntitySpaceStudio\*.exe
```

If present, the typo `publish_EntitySpaceStudio` (no `Wix`, no trailing `t`)
still exists somewhere. Search the repo and fix it.

### Symptom: "MSI was not found at the expected path"

**Cause:** the script and `Program.cs` disagree on the output folder.  
**Check:** both must be `installer_EntitySpacesStudio`. Verify:

- `SourcePaths.OutputDir` in `Program.cs`
- `$InstallerOutputDir` in `Build-EntitySpaces.ps1`
- `project.OutDir` assignment inside `ConfigureProject(...)`
- `Environment.CurrentDirectory = SourcePaths.OutputDir;` in `Main()`

### Symptom: WiX compilation fails (WIX0091, WIX0001, …)

**Cause:** malformed generated `.wxs` or a source file with a path that WiX
cannot resolve.  
**Check:** open `wix_errors.txt`, find the `WIXxxxx` code, and inspect the
generated `EntitySpaces-YYYY.MM.NN-x64.wxs` in `installer_EntitySpacesStudio\`
(`PreserveTempFiles = true` keeps it).

### Symptom: dialogs do not show my branding

**Cause:** image dimensions do not match the expected sizes.  
**Check:** `Banner.bmp` must be 493×58, `Welcome_156x312.bmp` must be ~156×312.
`ManagedUI` validates the ratio and will fail the build otherwise.

### Symptom: "source path(s) are missing" during build

**Cause:** one of the folders in `SourcePaths` does not exist (e.g. a template
folder was renamed or a documentation PDF was moved).  
**Check:** the log lists every missing path under
`SOURCE PATHS NOT FOUND (N)`.

### Symptom: build runs but exits with code 1

The script and `Program.cs` both set `Environment.ExitCode = 1` on fatal
errors so CI fails correctly. Always open `wix_errors.txt` first: the
`FATAL ERROR` block contains the full exception.

---

## 12. What must never be committed

`.gitignore` already excludes everything below. If you see any of these
files in `git status`, something is wrong with `.gitignore`.

```
bin/
obj/
*.user
*.wxs
*.wixobj
*.wixpdb
*.msi
WixSharp*.dll
*.CA.dll
CustomAction.config
EmbeddedUI.config
wix_errors.txt
BuildLog.txt
publish_EntitySpacesStudio/
installer_EntitySpacesStudio/
```

### Only these belong in Git

```
EntitySpaces.Installer.csproj
Program.cs
Build-EntitySpaces.ps1
.gitignore
README.md
Images\*
```

---

## Appendix — key symbols in `Program.cs`

For quick navigation when editing:

| Symbol | Purpose |
|--------|---------|
| `SourcePaths` | Every input folder/file the installer reads |
| `TargetPaths` | Every install location and registry key |
| `UpgradeCode` | Fixed GUID; **never change** |
| `DefaultReleaseVersion` | Fallback `YYYY.MM.NN` |
| `ResolveReleaseVersion()` | Reads `argv[1]` or falls back to default |
| `ToMsiVersion()` | `YYYY.MM.NN` → MSI `Version` |
| `ProductCodeFor()` | Deterministic ProductCode per release |
| `Main()` | Entry point; sets output folder, builds MSI |
| `CreateProject()` | Builds the folder tree, shortcuts and registry |
| `ConfigureProject()` | Platform, version, upgrade, `OutDir`, `Load` |
| `ConfigureManagedUI()` | Dialog sequence |
| `ConfigureBranding()` | Icon, banner, side image, license |
| `BuildInstallDir()` | `%ProgramFiles64Folder%\EntitySpaces\EntitySpaces 2026` |
| `BuildProgramDataDir()` | `%CommonAppDataFolder%\EntitySpaces\ES2026` |
| `BuildShortcutDir()` | Desktop and Start menu shortcuts |
| `BuildUserRegistry()` | HKCU values |
| `BuildMachineRegistry()` | HKLM values |
| `OnLoad()` | `.NET 10` prerequisite check at install time |
| `CollectFiles()` | Glob + verify + log |
| `PrintVerificationSummary()` | Fails the build on missing sources |
| `InitLog()` | Writes to `ES_LOG_DIR\wix_errors.txt` |
| `TeeWriter` | Duplicates console output into the log file |

---
