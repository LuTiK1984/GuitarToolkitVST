@echo off
echo === GuitarToolkit VST3 Deploy ===
echo.

set SOURCE=GuitarToolkit.Plugin\bin\x64\Debug\net8.0-windows
set DEST=C:\Program Files\Common Files\VST3\GuitarToolkit

if not exist "%SOURCE%\GuitarToolkit.Plugin.dll" (
    echo ОШИБКА: Сначала собери проект в Visual Studio!
    echo Файл %SOURCE%\GuitarToolkit.Plugin.dll не найден.
    pause
    exit /b 1
)

if not exist "%SOURCE%\GuitarToolkit.PluginBridge.vst3" (
    echo ОШИБКА: Бридж-файл не найден. Проверь post-build в Plugin.csproj
    pause
    exit /b 1
)

echo Копирую в: %DEST%
echo.

if not exist "%DEST%" mkdir "%DEST%"

copy /Y "%SOURCE%\GuitarToolkit.Plugin.dll"                "%DEST%\" >nul
copy /Y "%SOURCE%\GuitarToolkit.Core.dll"                  "%DEST%\" >nul
copy /Y "%SOURCE%\GuitarToolkit.UI.dll"                    "%DEST%\" >nul
copy /Y "%SOURCE%\AudioPlugSharp.dll"                      "%DEST%\" >nul
copy /Y "%SOURCE%\AudioPlugSharpWPF.dll"                   "%DEST%\" >nul
copy /Y "%SOURCE%\GuitarToolkit.PluginBridge.vst3"         "%DEST%\" >nul
copy /Y "%SOURCE%\GuitarToolkit.PluginBridge.runtimeconfig.json" "%DEST%\" >nul
copy /Y "%SOURCE%\Ijwhost.dll"                             "%DEST%\" >nul

echo.
echo Готово! Перезапусти FL Studio и сделай Rescan.
pause
