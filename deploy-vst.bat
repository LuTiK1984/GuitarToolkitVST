@echo off
setlocal

echo === GuitarToolkit VST3 Deploy ===
echo.

set "SOURCE=GuitarToolkit.Plugin\bin\x64\Debug\net8.0-windows"
set "DEST=C:\Program Files\Common Files\VST3\GuitarToolkit"
set "MISSING=0"

echo Source: %SOURCE%
echo Target: %DEST%
echo.

if not exist "%SOURCE%" (
    echo ERROR: Build output folder was not found.
    echo Build the solution first: dotnet build GuitarToolkit.sln --configuration Debug
    pause
    exit /b 1
)

call :require "GuitarToolkit.Plugin.dll"
call :require "GuitarToolkit.Core.dll"
call :require "GuitarToolkit.UI.dll"
call :require "AudioPlugSharp.dll"
call :require "AudioPlugSharpWPF.dll"
call :require "AlphaSkia.dll"
call :require "AlphaSkia.Native.Windows.dll"
call :require "AlphaTab.dll"
call :require "AlphaTab.Windows.dll"
call :require "GuitarToolkit.PluginBridge.vst3"
call :require "GuitarToolkit.PluginBridge.runtimeconfig.json"
call :require "Ijwhost.dll"
call :require "runtimes"

if "%MISSING%"=="1" (
    echo.
    echo ERROR: Required files are missing. Build the plugin project and check NuGet bridge files.
    pause
    exit /b 1
)

if not exist "%DEST%" (
    echo Creating target folder...
    mkdir "%DEST%"
    if errorlevel 1 (
        echo ERROR: Cannot create target folder. Run this script as Administrator.
        pause
        exit /b 1
    )
)

echo Copying files...
xcopy "%SOURCE%\*" "%DEST%\" /E /I /Y >nul
if errorlevel 1 goto copy_failed

echo.
echo Done. Restart or rescan plugins in your DAW.
pause
exit /b 0

:require
if not exist "%SOURCE%\%~1" (
    echo Missing: %~1
    set "MISSING=1"
) else (
    echo Found:   %~1
)
exit /b 0

:copy_failed
echo.
echo ERROR: Deploy failed.
echo Close your DAW if it is running, then run this script as Administrator.
pause
exit /b 1
