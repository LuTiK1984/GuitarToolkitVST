# Supported DAWs

[Русская версия](#ru)

GuitarToolkit ships a VST3 plugin package. DAW compatibility depends on the DAW, Windows, plugin scanning, and the full VST3 runtime folder being copied.

## Current Status

| DAW | Status | Notes |
| --- | --- | --- |
| FL Studio | Primary manual target | Re-scan after deployment. Check input assignment after plugin load. |
| Reaper | Planned/manual target | Use the common VST3 folder and clear cache/re-scan if the plugin does not appear. |
| Cubase | Untested | Should be compatible with VST3, but needs manual validation. |
| Ableton Live | Untested | Should be compatible with VST3, but needs manual validation. |
| Other VST3 hosts | Unknown | Reports are welcome through the DAW compatibility issue template. |

## Manual Compatibility Checklist

1. Close the DAW before replacing plugin files.
2. Install the full `GuitarToolkit` VST3 folder, not only the bridge `.vst3`.
3. Re-scan plugins.
4. Add GuitarToolkit to an audio track.
5. Assign or change the recording input after the plugin is loaded.
6. Check tuner input, metronome output, and chord/scale playback.
7. Open the Tabs page and verify editor stability if the DAW can host the UI.

Logs are written to:

```text
%AppData%\GuitarToolkit\logs
```

---

<a id="ru"></a>

# Поддерживаемые DAW

[English version](#supported-daws)

GuitarToolkit поставляется как VST3-плагин. Совместимость зависит от конкретной DAW, Windows, сканирования плагинов и того, был ли скопирован полный каталог VST3 runtime.

## Текущий статус

| DAW | Статус | Заметки |
| --- | --- | --- |
| FL Studio | Основная ручная цель | После деплоя нужно пересканировать плагины. Проверять назначение входа после загрузки плагина. |
| Reaper | Плановая/ручная цель | Использовать общий VST3-каталог и `Clear cache/re-scan`, если плагин не появился. |
| Cubase | Не проверено | Должен быть совместим с VST3, но нужна ручная проверка. |
| Ableton Live | Не проверено | Должен быть совместим с VST3, но нужна ручная проверка. |
| Другие VST3-хосты | Неизвестно | Отчёты приветствуются через DAW compatibility issue template. |

## Ручной чеклист совместимости

1. Закрыть DAW перед заменой файлов плагина.
2. Установить полный каталог VST3 `GuitarToolkit`, а не только bridge `.vst3`.
3. Пересканировать плагины.
4. Добавить GuitarToolkit на audio track.
5. Назначить или изменить вход записи после загрузки плагина.
6. Проверить вход тюнера, звук метронома и воспроизведение аккордов/гамм.
7. Открыть вкладку Tabs и проверить стабильность editor, если DAW корректно хостит UI.

Логи пишутся сюда:

```text
%AppData%\GuitarToolkit\logs
```
