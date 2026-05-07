param(
    [string]$Checkpoint = "runs\progression_rich_ft\best_model.pt",
    [string]$Output = "runs\progression_rich_ft\ProgressionNextTokenModel.onnx",
    [string]$Python = "python"
)

$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $scriptDir
try {
    & $Python export_onnx.py --checkpoint $Checkpoint --output $Output

    $targetDir = Join-Path $env:APPDATA "GuitarToolkit\models"
    New-Item -ItemType Directory -Force -Path $targetDir | Out-Null

    $targetPath = Join-Path $targetDir "ProgressionNextTokenModel.onnx"
    Copy-Item -Force -Path $Output -Destination $targetPath

    Write-Host "installed=$targetPath"
}
finally {
    Pop-Location
}
