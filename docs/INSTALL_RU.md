# Установка GuitarToolkit

## Desktop-версия

1. Скачайте архив `GuitarToolkit_DESKTOP_v.1.3.3.zip` из GitHub Releases.
2. Распакуйте архив в удобную папку.
3. Запустите `GuitarToolkit.Desktop.exe`.
4. В верхней панели выберите устройство ввода: микрофон, линейный вход или аудиоинтерфейс.

## VST3-плагин

1. Скачайте архив `GuitarToolkit_VST3_v.1.3.3.zip` из GitHub Releases.
2. Закройте DAW, если она открыта.
3. Распакуйте архив в одну папку `GuitarToolkit`.
4. Скопируйте всю папку `GuitarToolkit` в:

```text
C:\Program Files\Common Files\VST3\GuitarToolkit\
```

Не копируйте только файл `.vst3`. Плагину также нужны DLL-файлы и папка `runtimes` из архива.

5. Если Windows требует права администратора, подтвердите копирование от имени администратора.
6. Откройте DAW и выполните повторное сканирование VST3-плагинов.
7. Добавьте GuitarToolkit как эффект на аудиотрек.

## FL Studio

1. Откройте `Options -> Manage plugins`.
2. Проверьте, что `C:\Program Files\Common Files\VST3` есть в путях поиска плагинов.
3. Нажмите `Find installed plugins`.
4. Добавьте GuitarToolkit в микшер как эффект.
5. Если файлы заменялись вручную, полностью перезапустите FL Studio перед пересканированием.

## Reaper

1. Откройте `Options -> Preferences -> Plug-ins -> VST`.
2. Проверьте, что `C:\Program Files\Common Files\VST3` есть в VST-путях.
3. Нажмите `Re-scan`.
4. Добавьте GuitarToolkit как FX-плагин на аудиотрек.

## Если DAW не видит плагин

- Проверьте, что файл `GuitarToolkit.PluginBridge.vst3` находится в папке VST3.
- Проверьте, что скопирована вся папка релиза, включая `GuitarToolkit.Plugin.dll`, `GuitarToolkit.UI.dll`, `AlphaTab.dll`, `AlphaTab.Windows.dll`, `AlphaSkia.dll`, `AlphaSkia.Native.Windows.dll`, `AudioPlugSharp*.dll`, `NAudio*.dll`, `Ijwhost.dll`, `GuitarToolkit.PluginBridge.runtimeconfig.json` и папку `runtimes`.
- Полностью закройте DAW и откройте её заново.
- Выполните повторное сканирование плагинов.
- Если плагин открывается, но затем падает, проверьте логи в `%AppData%\GuitarToolkit\logs`.

## Требования

- Windows 10/11 x64.
- .NET 8 runtime.
- Для VST3-версии: DAW с поддержкой VST3.
