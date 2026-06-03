# Создание релиза: обновляет VERSION, коммит, тег vX.Y.Z, push
param(
    [Parameter(Mandatory = $true)]
    [string]$Version
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path)
$versionFile = Join-Path $root "VERSION"
$tag = "v$Version"

if ($Version -notmatch '^\d+\.\d+\.\d+') {
    throw "Версия должна быть SemVer: 1.0.0"
}

Set-Content -Path $versionFile -Value $Version -NoNewline
Write-Host "VERSION -> $Version"

Push-Location $root
try {
    git add VERSION
    git commit -m "Release $tag"
    git tag $tag
    git push origin main
    git push origin $tag
    Write-Host "Запущена сборка и релиз: $tag" -ForegroundColor Green
    Write-Host "https://github.com/bdeenyy/pdf-counter/actions"
}
finally {
    Pop-Location
}
