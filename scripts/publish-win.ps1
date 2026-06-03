# Сборка portable exe для Windows x64
$ErrorActionPreference = "Stop"
$root = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path)
$project = Join-Path $root "src\PdfCounter\PdfCounter.csproj"
$outDir = Join-Path $root "publish"

dotnet publish $project `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -o $outDir

Write-Host "Готово: $outDir\PdfCounter.exe" -ForegroundColor Green
