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
$trainerPackage = Join-Path $packageDir "ml-trainer"

$desktopOut = Join-Path $root "GuitarToolkit.Desktop\bin\x64\$Configuration\net8.0-windows"
$pluginOut = Join-Path $root "GuitarToolkit.Plugin\bin\x64\$Configuration\net8.0-windows"
$trainerOut = Join-Path $root "tools\ml\GuitarToolkit.MLTrainer\bin\x64\$Configuration\net8.0-windows"

$desktopZip = Join-Path $releaseDir "GuitarToolkit_DESKTOP_v.$Version.zip"
$pluginZip = Join-Path $releaseDir "GuitarToolkit_VST3_v.$Version.zip"
$trainerZip = Join-Path $releaseDir "GuitarToolkit_ML_TRAINER_v.$Version.zip"

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

$mlTrainerToolFiles = @(
    "evaluate_checkpoint.py",
    "eval_prompts.jsonl",
    "eval_prompts_full.jsonl",
    "export_onnx.py",
    "generate_synthetic_dataset.py",
    "inspect_checkpoint.py",
    "model.py",
    "README.md",
    "requirements.txt",
    "sample_dataset.jsonl",
    "train.py",
    "validate_dataset.py",
    "vocab.json"
)

$melodyTransformerToolFiles = @(
    "export_onnx.py",
    "generate_synthetic_dataset.py",
    "inspect_checkpoint.py",
    "model.py",
    "README.md",
    "requirements.txt",
    "sample_dataset.jsonl",
    "train.py",
    "validate_dataset.py",
    "vocab.json"
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
New-Item -ItemType Directory -Path $trainerPackage | Out-Null

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

Write-Host "Checking ML Trainer output..."
Assert-File (Join-Path $trainerOut "GuitarToolkit.MLTrainer.exe")

Write-Host
Write-Host "Creating desktop archive..."
Copy-Item -Path (Join-Path $desktopOut "*") -Destination $desktopPackage -Recurse -Force
Copy-ReleaseDocs -Destination $desktopPackage
Compress-Archive -Path (Join-Path $desktopPackage "*") -DestinationPath $desktopZip -Force

Write-Host "Creating VST3 archive..."
Copy-Item -Path (Join-Path $pluginOut "*") -Destination $pluginPackage -Recurse -Force
Copy-ReleaseDocs -Destination $pluginPackage
Compress-Archive -Path (Join-Path $pluginPackage "*") -DestinationPath $pluginZip -Force

Write-Host "Creating ML Trainer archive..."
Copy-Item -Path (Join-Path $trainerOut "*") -Destination $trainerPackage -Recurse -Force
Copy-Item -LiteralPath (Join-Path $root "tools\ml\GuitarToolkit.MLTrainer\README.md") -Destination $trainerPackage -Force
$trainerToolsPackage = Join-Path $trainerPackage "progression_next_token"
New-Item -ItemType Directory -Path $trainerToolsPackage -Force | Out-Null
foreach ($fileName in $mlTrainerToolFiles) {
    $source = Join-Path $root "tools\ml\progression_next_token\$fileName"
    if (Test-Path -LiteralPath $source) {
        Copy-Item -LiteralPath $source -Destination $trainerToolsPackage -Force
    }
    else {
        Write-Warning "Optional ML Trainer tool file not found: $fileName"
    }
}
$melodyToolsPackage = Join-Path $trainerPackage "melody_phrase_transformer"
New-Item -ItemType Directory -Path $melodyToolsPackage -Force | Out-Null
foreach ($fileName in $melodyTransformerToolFiles) {
    $source = Join-Path $root "tools\ml\melody_phrase_transformer\$fileName"
    if (Test-Path -LiteralPath $source) {
        Copy-Item -LiteralPath $source -Destination $melodyToolsPackage -Force
    }
    else {
        Write-Warning "Optional Melody Transformer tool file not found: $fileName"
    }
}
Copy-ReleaseDocs -Destination $trainerPackage
Compress-Archive -Path (Join-Path $trainerPackage "*") -DestinationPath $trainerZip -Force

Write-Host
Write-Host "Release artifacts:"
Get-Item $desktopZip, $pluginZip, $trainerZip | Format-Table Name, Length, LastWriteTime

Write-Host
Write-Host "Done."
