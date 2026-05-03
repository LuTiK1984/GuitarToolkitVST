# GuitarToolkit Quick Start

[Русская версия](#ru)

This guide helps you install and launch GuitarToolkit quickly. GuitarToolkit is available as a standalone Windows desktop app and as a VST3 plugin for DAW hosts.

## 1. Choose the package

Open the latest GitHub Release and download the package you need:

| Package | Use for |
| --- | --- |
| `GuitarToolkit_DESKTOP_v.<version>.zip` | Standalone Windows desktop app |
| `GuitarToolkit_VST3_v.<version>.zip` | VST3 plugin for DAW hosts |

## 2. Desktop app

1. Download `GuitarToolkit_DESKTOP_v.<version>.zip`.
2. Extract the ZIP file to a folder you control, for example `C:\Apps\GuitarToolkit\`.
3. Run the desktop executable from the extracted folder.
4. Open the Tuner tab and select your input device.
5. Check the Metronome, Chords, Scales, Progressions, Circle of Fifths, and Tabs pages.

## 3. VST3 plugin

1. Download `GuitarToolkit_VST3_v.<version>.zip`.
2. Extract the ZIP file.
3. Copy the whole `GuitarToolkit` plugin folder to:

```text
C:\Program Files\Common Files\VST3\GuitarToolkit\
```

Do not copy only `GuitarToolkit.PluginBridge.vst3`. The plugin needs its DLL dependencies and the `runtimes` folder.

4. Open your DAW.
5. Rescan VST3 plugins.
6. Add GuitarToolkit to a track.
7. Open the plugin editor.
8. Assign an input device or track input if your DAW requires it.
9. Test the tuner input, metronome playback, chord/scale playback, and Tabs page.

## 4. Troubleshooting

If GuitarToolkit does not start or the VST3 plugin does not load:

- make sure you downloaded the latest release;
- make sure the full VST3 folder was copied;
- rescan plugins in your DAW;
- restart the DAW after installation;
- check that your DAW supports VST3 plugins;
- check the logs folder.

Logs are stored here:

```text
%AppData%\GuitarToolkit\logs
```

## 5. Asking for help

For installation and setup questions, use GitHub Discussions -> Q&A.

For VST3 behavior in a specific DAW, use GitHub Discussions -> DAW Compatibility.

For reproducible bugs, use GitHub Issues -> Bug report.

When asking about VST3, include:

- GuitarToolkit version;
- DAW name and version;
- Windows version;
- audio interface or input device;
- steps you tested;
- logs or screenshots, if available.

---

<a id="ru"></a>

# Быстрый старт GuitarToolkit

[English version](#guitartoolkit-quick-start)

Это руководство помогает быстро установить и запустить GuitarToolkit. Проект доступен как standalone desktop-приложение для Windows и как VST3-плагин для DAW.

## 1. Выберите пакет

Откройте последний GitHub Release и скачайте нужный пакет:

| Пакет | Для чего |
| --- | --- |
| `GuitarToolkit_DESKTOP_v.<version>.zip` | Standalone-приложение для Windows |
| `GuitarToolkit_VST3_v.<version>.zip` | VST3-плагин для DAW |

## 2. Desktop-приложение

1. Скачайте `GuitarToolkit_DESKTOP_v.<version>.zip`.
2. Распакуйте ZIP в удобную папку, например `C:\Apps\GuitarToolkit\`.
3. Запустите desktop executable из распакованной папки.
4. Откройте вкладку Tuner и выберите входное устройство.
5. Проверьте Metronome, Chords, Scales, Progressions, Circle of Fifths и Tabs.

## 3. VST3-плагин

1. Скачайте `GuitarToolkit_VST3_v.<version>.zip`.
2. Распакуйте ZIP.
3. Скопируйте всю папку плагина `GuitarToolkit` сюда:

```text
C:\Program Files\Common Files\VST3\GuitarToolkit\
```

Не копируйте только `GuitarToolkit.PluginBridge.vst3`. Плагину нужны DLL-зависимости и папка `runtimes`.

4. Откройте DAW.
5. Пересканируйте VST3-плагины.
6. Добавьте GuitarToolkit на дорожку.
7. Откройте окно плагина.
8. Назначьте входное устройство или вход дорожки, если этого требует DAW.
9. Проверьте вход тюнера, звук метронома, воспроизведение аккордов/гамм и вкладку Tabs.

## 4. Если что-то не работает

Если GuitarToolkit не запускается или VST3-плагин не загружается:

- убедитесь, что скачан последний релиз;
- убедитесь, что скопирована вся папка VST3;
- пересканируйте плагины в DAW;
- перезапустите DAW после установки;
- проверьте, что DAW поддерживает VST3;
- проверьте папку логов.

Логи находятся здесь:

```text
%AppData%\GuitarToolkit\logs
```

## 5. Где просить помощь

Вопросы по установке и настройке: GitHub Discussions -> Q&A.

Отчёты о поведении VST3 в конкретной DAW: GitHub Discussions -> DAW Compatibility.

Воспроизводимые баги: GitHub Issues -> Bug report.

Если вопрос связан с VST3, укажите:

- версию GuitarToolkit;
- название и версию DAW;
- версию Windows;
- аудиоинтерфейс или входное устройство;
- проверенные шаги;
- логи или скриншоты, если есть.
