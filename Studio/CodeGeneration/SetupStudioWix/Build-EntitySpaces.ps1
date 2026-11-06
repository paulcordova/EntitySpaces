# ==============================================================================
# Build and Packaging Script - EntitySpaces Studio 2026
#
# Note: This script performs the full end-to-end process:
#       1. Publishes the main application (StandAlone) in Release mode (x64)
#          into SetupStudioWix\publish_EntitySpacesStudio.
#       2. Compiles the WixSharp installer project (EntitySpaces.Installer).
#       3. Executes the WixSharp installer project to generate the final MSI file
#          into SetupStudioWix\installer_EntitySpacesStudio.
#
# Logs: every log is written in the folder of this script
#       (BuildLog.txt = this script, wix_errors.txt = installer program).
# ==============================================================================

# All logs live next to this script
$LogDir           = $PSScriptRoot
$LogFile          = Join-Path -Path $LogDir -ChildPath "BuildLog.txt"
$InstallerLogFile = Join-Path -Path $LogDir -ChildPath "wix_errors.txt"

# Tell the installer program (Program.cs) where to write its own log
$env:ES_LOG_DIR = $LogDir

# Function to write to both console and log file
function Write-Log {
    param([string]$Message, [string]$Color = "White")
    Write-Host $Message -ForegroundColor $Color
    Add-Content -Path $LogFile -Value "$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss') - $Message"
}

# Initialize clean logs
foreach ($file in $LogFile, $InstallerLogFile) {
    if (Test-Path $file) { Remove-Item $file -Force }
}
Write-Log "============================================="
Write-Log "STARTING BUILD AND PACKAGING PROCESS"
Write-Log "============================================="

# Define paths
$AppProjectDir       = "D:\Work\src\04_proyectos\Internos\EntitySpaces\git-paul-EntitySpaces\Studio\CodeGeneration\StandAlone"
$InstallerProjectDir = "D:\Work\src\04_proyectos\Internos\EntitySpaces\git-paul-EntitySpaces\Studio\CodeGeneration\SetupStudioWix"

# Build output folders (inside the installer project folder, ignored by Git)
$PublishDir          = Join-Path -Path $InstallerProjectDir -ChildPath "publish_EntitySpacesStudio"
$InstallerOutputDir  = Join-Path -Path $InstallerProjectDir -ChildPath "installer_EntitySpacesStudio"

# Remember the current folder so it can be restored at the end
Push-Location

try {
    # 1. Clean the previous build output folders
    Write-Log "Cleaning publish and installer output folders..." "Cyan"
    foreach ($dir in $PublishDir, $InstallerOutputDir) {
        if (Test-Path $dir) {
            Remove-Item -Path $dir -Recurse -Force
        }
        New-Item -ItemType Directory -Path $dir -Force | Out-Null
    }

    # 2. Publish the main application
    Write-Log "Publishing application in Release mode (x64)..." "Cyan"
    Set-Location -Path $AppProjectDir

    $publishOutput = dotnet publish StandAlone.csproj -c Release -r win-x64 --self-contained false -p:PlatformTarget=x64 -o $PublishDir 2>&1
    $publishOutput | Out-String | Add-Content -Path $LogFile

    if ($LASTEXITCODE -ne 0) {
        throw "Application publication failed. Check the log for details."
    }

    # 3. Compile the WixSharp project (Installer)
    Write-Log "Compiling WixSharp installer project..." "Cyan"
    Set-Location -Path $InstallerProjectDir

    $buildOutput = dotnet build EntitySpaces.Installer.csproj -c Release 2>&1
    $buildOutput | Out-String | Add-Content -Path $LogFile

    if ($LASTEXITCODE -ne 0) {
        throw "Installer compilation failed. Check the log for details."
    }

    # 4. Execute the WixSharp installer project to generate the MSI
    Write-Log "Executing installer to generate MSI..." "Cyan"

    # Run the project in Release configuration. This triggers the Program.cs logic that builds the MSI
    # into $InstallerOutputDir and writes its log to $env:ES_LOG_DIR.
    $runOutput = dotnet run --project EntitySpaces.Installer.csproj -c Release 2>&1
    $runOutput | Out-String | Add-Content -Path $LogFile

    if ($LASTEXITCODE -ne 0) {
        throw "Installer execution failed. Check the log for details. (WixSharp or WiX error)"
    }

    # Look for the generated MSI (the file name includes the release version)
    $Msi = Get-ChildItem -Path $InstallerOutputDir -Filter "*.msi" -ErrorAction SilentlyContinue |
           Sort-Object LastWriteTime -Descending |
           Select-Object -First 1

    if ($Msi) {
        Write-Log "Process completed successfully!" "Green"
        Write-Log "MSI generated at: $($Msi.FullName)" "Yellow"
    } else {
        Write-Log "Warning: Execution finished but no MSI was found in: $InstallerOutputDir" "Yellow"
        Write-Log "Check $InstallerLogFile for WixSharp internal errors." "Yellow"
    }
}
catch {
    Write-Log "=============================================" "Red"
    Write-Log "FATAL ERROR: $($_.Exception.Message)" "Red"
    Write-Log "Check $LogFile and $InstallerLogFile for full details." "Red"
    Write-Log "=============================================" "Red"
}
finally {
    Pop-Location

    # Final pause to allow reading the console before it closes
    Write-Host "`nPress ENTER to close this window..." -ForegroundColor Magenta
    Read-Host
}