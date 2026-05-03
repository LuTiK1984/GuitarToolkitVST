# FL Studio VST3 Setup

[Русская версия](#ru)

This page documents the recommended FL Studio setup flow for GuitarToolkit VST3.

## Install

1. Close FL Studio.
2. Extract `GuitarToolkit_VST3_v.<version>.zip`.
3. Copy the full extracted `GuitarToolkit` plugin folder to:

   ```text
   C:\Program Files\Common Files\VST3\GuitarToolkit\
   ```

4. Do not copy only `GuitarToolkit.PluginBridge.vst3`. The plugin also needs its DLL dependencies and the `runtimes` folder.

## Scan

1. Open FL Studio.
2. Go to `Options -> Manage plugins`.
3. Make sure the common VST3 folder is included in scan paths:

   ```text
   C:\Program Files\Common Files\VST3
   ```

4. Click `Find installed plugins`.
5. Add GuitarToolkit to an audio track.

## Smoke Test

1. Assign or change the recording input after GuitarToolkit is loaded.
2. Check tuner input.
3. Start and stop the metronome.
4. Play a chord and a scale.
5. Open the Tabs page and load a small Guitar Pro or MusicXML file if needed.

## Troubleshooting

- If the plugin does not appear, close FL Studio and re-scan after verifying the full VST3 folder was copied.
- If the UI opens but Tabs fail, check that the `runtimes` folder is present.
- If behavior is strange after replacing files, restart FL Studio and scan again.
- Logs are written to `%AppData%\GuitarToolkit\logs`.

---

<a id="ru"></a>

# Настройка VST3 в FL Studio

[English version](#fl-studio-vst3-setup)

Эта страница описывает рекомендуемый способ установки GuitarToolkit VST3 в FL Studio.

## Установка

1. Закройте FL Studio.
2. Распакуйте `GuitarToolkit_VST3_v.<version>.zip`.
3. Скопируйте весь распакованный каталог плагина `GuitarToolkit` сюда:

   ```text
   C:\Program Files\Common Files\VST3\GuitarToolkit\
   ```

4. Не копируйте только `GuitarToolkit.PluginBridge.vst3`. Плагину также нужны DLL-зависимости и папка `runtimes`.

## Сканирование

1. Откройте FL Studio.
2. Перейдите в `Options -> Manage plugins`.
3. Убедитесь, что общий путь VST3 добавлен в scan paths:

   ```text
   C:\Program Files\Common Files\VST3
   ```

4. Нажмите `Find installed plugins`.
5. Добавьте GuitarToolkit на audio track.

## Быстрая проверка

1. Назначьте или измените вход записи после загрузки GuitarToolkit.
2. Проверьте вход тюнера.
3. Запустите и остановите метроном.
4. Проиграйте аккорд и гамму.
5. Откройте вкладку Tabs и при необходимости загрузите небольшой файл Guitar Pro или MusicXML.

## Диагностика

- Если плагин не появился, закройте FL Studio и пересканируйте плагины после проверки, что скопирован весь каталог VST3.
- Если интерфейс открывается, но вкладка Tabs не работает, проверьте наличие папки `runtimes`.
- Если после замены файлов поведение странное, перезапустите FL Studio и выполните scan снова.
- Логи пишутся в `%AppData%\GuitarToolkit\logs`.
