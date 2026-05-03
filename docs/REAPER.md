# Reaper VST3 Setup

[Русская версия](#ru)

This page documents the recommended Reaper setup flow for GuitarToolkit VST3.

## Install

1. Close Reaper.
2. Extract `GuitarToolkit_VST3_v.<version>.zip`.
3. Copy the full extracted `GuitarToolkit` plugin folder to:

   ```text
   C:\Program Files\Common Files\VST3\GuitarToolkit\
   ```

4. Do not copy only `GuitarToolkit.PluginBridge.vst3`. The plugin also needs its DLL dependencies and the `runtimes` folder.

## Scan

1. Open Reaper.
2. Go to `Options -> Preferences -> Plug-ins -> VST`.
3. Make sure this path is present:

   ```text
   C:\Program Files\Common Files\VST3
   ```

4. Click `Clear cache/re-scan` if GuitarToolkit does not appear.
5. Add GuitarToolkit to a track as an FX plugin.

## Smoke Test

1. Assign or change the track input after GuitarToolkit is loaded.
2. Arm the track if needed for input monitoring.
3. Check tuner input.
4. Start and stop the metronome.
5. Play a chord and a scale.
6. Open the Tabs page and load a small Guitar Pro or MusicXML file if needed.

## Troubleshooting

- If Reaper does not find the plugin, verify the VST3 scan path and run `Clear cache/re-scan`.
- If the editor fails to open after updating, restart Reaper.
- If Tabs fail, check that the `runtimes` folder is present.
- Logs are written to `%AppData%\GuitarToolkit\logs`.

---

<a id="ru"></a>

# Настройка VST3 в Reaper

[English version](#reaper-vst3-setup)

Эта страница описывает рекомендуемый способ установки GuitarToolkit VST3 в Reaper.

## Установка

1. Закройте Reaper.
2. Распакуйте `GuitarToolkit_VST3_v.<version>.zip`.
3. Скопируйте весь распакованный каталог плагина `GuitarToolkit` сюда:

   ```text
   C:\Program Files\Common Files\VST3\GuitarToolkit\
   ```

4. Не копируйте только `GuitarToolkit.PluginBridge.vst3`. Плагину также нужны DLL-зависимости и папка `runtimes`.

## Сканирование

1. Откройте Reaper.
2. Перейдите в `Options -> Preferences -> Plug-ins -> VST`.
3. Убедитесь, что путь добавлен:

   ```text
   C:\Program Files\Common Files\VST3
   ```

4. Если GuitarToolkit не появляется, нажмите `Clear cache/re-scan`.
5. Добавьте GuitarToolkit на дорожку как FX-плагин.

## Быстрая проверка

1. Назначьте или измените вход дорожки после загрузки GuitarToolkit.
2. При необходимости arm track для input monitoring.
3. Проверьте вход тюнера.
4. Запустите и остановите метроном.
5. Проиграйте аккорд и гамму.
6. Откройте вкладку Tabs и при необходимости загрузите небольшой файл Guitar Pro или MusicXML.

## Диагностика

- Если Reaper не находит плагин, проверьте VST3 scan path и выполните `Clear cache/re-scan`.
- Если после обновления не открывается editor, перезапустите Reaper.
- Если Tabs не работает, проверьте наличие папки `runtimes`.
- Логи пишутся в `%AppData%\GuitarToolkit\logs`.
