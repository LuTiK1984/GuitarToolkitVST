# Установка GuitarToolkit

## Desktop-версия

1. Скачайте архив `GuitarToolkit_DESKTOP_v.1.2.0.zip` из GitHub Releases.
2. Распакуйте архив в удобную папку.
3. Запустите `GuitarToolkit.Desktop.exe`.
4. В верхней панели выберите устройство ввода: микрофон, линейный вход или аудиоинтерфейс.

## VST3-плагин

1. Скачайте архив `GuitarToolkit_VST3_v.1.2.0.zip` из GitHub Releases.
2. Закройте DAW, если она открыта.
3. Распакуйте содержимое архива в папку:

```text
C:\Program Files\Common Files\VST3\GuitarToolkit\
```

4. Если Windows требует права администратора, подтвердите копирование от имени администратора.
5. Откройте DAW и выполните повторное сканирование VST3-плагинов.
6. Добавьте GuitarToolkit как эффект на аудиотрек.

## Если DAW не видит плагин

- Проверьте, что файл `GuitarToolkit.PluginBridge.vst3` находится в папке VST3.
- Проверьте, что рядом лежат `GuitarToolkit.Plugin.dll`, `GuitarToolkit.Core.dll`, `GuitarToolkit.UI.dll`, `AudioPlugSharp.dll`, `AudioPlugSharpWPF.dll`, `Ijwhost.dll` и `GuitarToolkit.PluginBridge.runtimeconfig.json`.
- Полностью закройте DAW и откройте её заново.
- Выполните повторное сканирование плагинов.

## Требования

- Windows 10/11 x64.
- .NET 8 runtime.
- Для VST3-версии: DAW с поддержкой VST3.
