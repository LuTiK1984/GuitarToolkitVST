param(
    [string]$Version = "1.5.0",
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$artifacts = Join-Path $root "artifacts"
$releaseDir = Join-Path $artifacts "release"
$packageDir = Join-Path $artifacts "package"
$desktopPackage = Join-Path $packageDir "desktop"
$pluginPackage = Join-Path $packageDir "vst3"

$desktopOut = Join-Path $root "GuitarToolkit.Desktop\bin\x64\$Configuration\net8.0-windows"
$pluginOut = Join-Path $root "GuitarToolkit.Plugin\bin\x64\$Configuration\net8.0-windows"

$desktopZip = Join-Path $releaseDir "GuitarToolkit_DESKTOP_v.$Version.zip"
$pluginZip = Join-Path $releaseDir "GuitarToolkit_VST3_v.$Version.zip"

$releaseDocs = @(
    "README.md",
    "LICENSE",
    "CHANGELOG.md",
    "THIRD_PARTY_NOTICES.md",
    "docs\README.md",
    "docs\user\QUICK_START.md",
    "docs\user\SUPPORTED_DAWS.md",
    "docs\user\FL_STUDIO.md",
    "docs\user\REAPER.md",
    "docs\user\KNOWN_TAB_IMPORT_ISSUES.md"
)

Write-Host "== GuitarToolkit release build v$Version =="
Write-Host "Configuration: $Configuration"
Write-Host

if (Test-Path $releaseDir) {
    Remove-Item -LiteralPath $releaseDir -Recurse -Force
}
if (Test-Path $packageDir) {
    Remove-Item -LiteralPath $packageDir -Recurse -Force
}
New-Item -ItemType Directory -Path $releaseDir | Out-Null
New-Item -ItemType Directory -Path $desktopPackage | Out-Null
New-Item -ItemType Directory -Path $pluginPackage | Out-Null

Write-Host "Building solution..."
dotnet build (Join-Path $root "GuitarToolkit.sln") --configuration $Configuration --no-restore

Write-Host
Write-Host "Running tests..."
dotnet test (Join-Path $root "GuitarToolkit.sln") --configuration $Configuration --no-build

function Assert-File {
    param([string]$Path)

    if (-not (Test-Path -LiteralPath $Path)) {
        throw "Required file was not found: $Path"
    }
}

function Copy-ReleaseDocs {
    param([string]$Destination)

    foreach ($relativePath in $releaseDocs) {
        $source = Join-Path $root $relativePath
        if (Test-Path -LiteralPath $source) {
            if ($relativePath -like "docs\*") {
                $target = Join-Path $Destination $relativePath
                $targetDir = Split-Path -Parent $target
                New-Item -ItemType Directory -Path $targetDir -Force | Out-Null
                Copy-Item -LiteralPath $source -Destination $target -Force
            }
            else {
                Copy-Item -LiteralPath $source -Destination $Destination -Force
            }
        }
        else {
            Write-Warning "Optional release document not found: $relativePath"
        }
    }
}

Write-Host
Write-Host "Checking desktop output..."
Assert-File (Join-Path $desktopOut "GuitarToolkit.Desktop.exe")
Assert-File (Join-Path $desktopOut "GuitarToolkit.Core.dll")
Assert-File (Join-Path $desktopOut "GuitarToolkit.UI.dll")

Write-Host "Checking VST3 output..."
Assert-File (Join-Path $pluginOut "GuitarToolkit.Plugin.dll")
Assert-File (Join-Path $pluginOut "GuitarToolkit.Core.dll")
Assert-File (Join-Path $pluginOut "GuitarToolkit.UI.dll")
Assert-File (Join-Path $pluginOut "AudioPlugSharp.dll")
Assert-File (Join-Path $pluginOut "AudioPlugSharpWPF.dll")
Assert-File (Join-Path $pluginOut "GuitarToolkit.PluginBridge.vst3")
Assert-File (Join-Path $pluginOut "GuitarToolkit.PluginBridge.runtimeconfig.json")
Assert-File (Join-Path $pluginOut "Ijwhost.dll")

Write-Host
Write-Host "Creating desktop archive..."
Copy-Item -Path (Join-Path $desktopOut "*") -Destination $desktopPackage -Recurse -Force
Copy-ReleaseDocs -Destination $desktopPackage
Compress-Archive -Path (Join-Path $desktopPackage "*") -DestinationPath $desktopZip -Force

Write-Host "Creating VST3 archive..."
Copy-Item -Path (Join-Path $pluginOut "*") -Destination $pluginPackage -Recurse -Force
Copy-ReleaseDocs -Destination $pluginPackage
Compress-Archive -Path (Join-Path $pluginPackage "*") -DestinationPath $pluginZip -Force

Write-Host
Write-Host "Release artifacts:"
Get-Item $desktopZip, $pluginZip | Format-Table Name, Length, LastWriteTime

Write-Host
Write-Host "Done."
