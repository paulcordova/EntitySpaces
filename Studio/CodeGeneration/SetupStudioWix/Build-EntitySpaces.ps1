# ==============================================================================
# Script de compilación y empaquetado - EntitySpaces Studio 2026
# ==============================================================================

# Definir rutas
$AppProjectDir = "D:\Work\src\04_proyectos\Internos\EntitySpaces\git-paul-EntitySpaces\Studio\CodeGeneration\StandAlone"
$InstallerProjectDir = "D:\Work\src\04_proyectos\Internos\EntitySpaces\git-paul-EntitySpaces\Studio\Installer_x64\EntitySpaces.Installer"
$PublishDir = "D:\Work\src\04_proyectos\Internos\EntitySpaces\publish_EntitySpaceStudio"

# 1. Limpiar la carpeta de publicación anterior
Write-Host "Limpiando carpeta de publicación..." -ForegroundColor Cyan
if (Test-Path $PublishDir) {
    Remove-Item -Path $PublishDir -Recurse -Force
}
New-Item -ItemType Directory -Path $PublishDir -Force | Out-Null

# 2. Publicar la aplicación principal
Write-Host "Publicando la aplicación en modo Release (x64)..." -ForegroundColor Cyan
Set-Location -Path $AppProjectDir

dotnet publish StandAlone.csproj `
    -c Release `
    -r win-x64 `
    --self-contained false `
    -p:PlatformTarget=x64 `
    -o $PublishDir

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Falló la publicación de la aplicación." -ForegroundColor Red
    exit $LASTEXITCODE
}

# 3. Compilar el proyecto de WixSharp (Instalador)
Write-Host "Generando el instalador MSI..." -ForegroundColor Cyan
Set-Location -Path $InstallerProjectDir

# Si estás usando Visual Studio, puedes usar dotnet build o msbuild
dotnet build EntitySpaces.Installer.csproj -c Release

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Falló la compilación del instalador." -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "¡Proceso finalizado con éxito!" -ForegroundColor Green
Write-Host "Revisa el log del instalador en: C:\Temp\wix_errors.txt" -ForegroundColor Yellow
Write-Host "Busca el MSI en: $InstallerProjectDir\bin\Release\net472" -ForegroundColor Yellow