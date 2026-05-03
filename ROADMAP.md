# GuitarToolkit Roadmap

[Русская версия](#ru)

GuitarToolkit is a personal, idea-driven guitar toolkit for Windows, available as a standalone desktop app and a VST3 plugin. The goal is to grow it from a useful passion project into a polished, trustworthy tool for guitar practice, writing, and DAW workflows.

## Product Direction

- Keep the project musician-first: fast to open, clear to use, and useful during real practice or recording sessions.
- Treat Desktop and VST3 as first-class targets, but validate DAW behavior carefully before calling a feature stable.
- Prefer practical workflow improvements over feature count.
- Keep the open-source/passion-project spirit while gradually improving product polish.

## Near-Term Priorities

- Improve VST3 installation experience:
  - make it obvious that the whole release folder must be copied;
  - document FL Studio and Reaper setup separately;
  - keep `deploy-vst.bat` aligned with the full dependency set.
- Add basic diagnostics:
  - write app/plugin errors to `%AppData%\GuitarToolkit\logs`;
  - include DAW/plugin startup failures where possible;
  - add a simple way to find logs from the UI or README.
- Add an About/Version window:
  - app version;
  - GitHub link;
  - license information;
  - third-party notices for alphaTab, AudioPlugSharp, NAudio, and related libraries.
- Polish the Tabs page:
  - reduce visible re-render jitter after resize/maximize/restore;
  - save last tab file, selected track, volume, speed, and auto-follow state;
  - keep testing tab playback inside FL Studio.
- Rebuild the interface layout as a dedicated design pass:
  - introduce consistent spacing, sizing, and panel structure;
  - use the refined Tabs page as the control-size and toolbar-density reference before applying the style across the rest of the app;
  - reduce the current feeling of scattered controls;
  - make tabs, buttons, and mode controls clearly named and visually distinguishable;
  - add an interface language switch and prepare UI text for localization;
  - move input device selection into the Tuner tab instead of keeping it global on every page;
  - replace the first theme-mapping pass with proper shared theme resources across all WPF views;
  - expand Settings into the home for language, theme, audio, tab library paths, diagnostics, and reset actions;
  - improve visual hierarchy across Desktop and VST without breaking workflows.
- Improve first-run clarity:
  - short quick-start guidance in README;
  - clearer “select input device first” flow;
  - simple screenshots for the most common workflows.

## Medium-Term Ideas

- Installer or polished portable package:
  - Desktop installer;
  - clearer VST3 folder layout;
  - possibly Inno Setup/MSI later.
- DAW-aware plugin behavior:
  - read host tempo if AudioPlugSharp exposes it reliably;
  - explore transport sync for metronome/tab playback;
  - avoid global hotkey conflicts inside DAW hosts.
- Better tab workflow:
  - polish the dedicated tab library folder UI;
  - add refresh/search/sort controls for library files;
  - per-track volume/mute/solo state;
  - export or print later if alphaTab supports it well.
- Better progression workflow:
  - grow the new preset dropdowns into a fuller preset manager with search, metadata, and safer saved-preset actions;
  - allow drag-and-drop reordering of chords in the current progression;
  - add duplicate and undo actions for fast progression editing;
  - support per-chord duration such as 1, 2, or 4 beats;
  - add playback patterns such as block chords, strum, and arpeggio;
  - export progressions as text and possibly MIDI;
  - decide whether saved presets should include key/mode or remain degree-only templates.
- Better interval-training workflow:
  - add ascending, descending, and harmonic interval modes;
  - add focused practice sets such as only seconds/thirds, only perfect intervals, or only trouble intervals;
  - track mistakes per interval and surface weak spots in the UI;
  - add optional guitar fretboard hints after answering;
  - support custom root-note range and timbre choices for closer guitar-practice feel;
  - add a slower review mode that explains the answer before auto-advance.
- Better scale-practice workflow:
  - add position presets for common fretboard boxes and CAGED shapes;
  - let users filter the displayed scale by fret range and string range;
  - add optional degree highlighting for tonic, thirds, fifths, sevenths, and avoid notes;
  - add exercises such as find the next note, name the degree, and play through a position;
  - compare two scales or modes on the same fretboard;
  - export or copy scale fingering diagrams for practice notes.
- Better chord workflow:
  - add quick chord search by name and type;
  - turn favorites into a fuller saved-chords list with rename/remove actions;
  - compare multiple voicings side by side;
  - add optional fingering hints for common shapes;
  - add transpose-shape controls for moving a voicing across the fretboard;
  - suggest related substitutions such as sus, add9, seventh, and relative minor/major options;
  - add a chord-change trainer with timed practice and accuracy tracking.
- Better metronome workflow:
  - add count-in before playback or practice takes;
  - support multiple click sounds and accent patterns;
  - add tap-tempo averaging feedback and reset;
  - add tempo-ramp practice for gradual speed increases;
  - add saved tempo presets by song or exercise;
  - explore host-tempo sync for the VST when AudioPlugSharp exposes reliable transport data.
- Better tuner workflow:
  - add a strobe-style fine tuning view next to the current cents needle;
  - add chromatic and string-focused modes so tuning can either follow any note or lock to the selected guitar strings;
  - add an input calibration/noise gate control for noisy interfaces and laptops;
  - show pitch stability over the last few detections so the user can see whether the note is settling;
  - add quick A4 reference presets such as 432, 440, and 442 Hz;
  - add optional alternate temperament offsets later if the core tuner remains stable enough.
- Improve audio UX:
  - safer volume defaults;
  - smoother tab playback volume changes;
  - optional metronome/count-in integration.
- Project trust:
  - clear license;
  - third-party notices;
  - code signing research for future Windows releases.

## Bughunting Checklist

- Tabs page visibly re-renders several times while adapting after resize/maximize/restore.
- Verify VST3 startup and editor opening in FL Studio after each release.
- Verify VST3 deployment with the full dependency set, including `runtimes`.
- Verify DAW behavior when assigning/changing recording input after plugin load.
- Test tab loading with GP3, GP4, GP5/GPX, and MusicXML files.
- Check scroll/auto-follow behavior on small windows, maximized windows, and restored windows.
- Check that Desktop and VST3 do not diverge unexpectedly in shared UI behavior.
- Restoring the saved tab cursor tick does not currently move alphaTab from the beginning after app restart.

## Commercial Readiness Notes

Current state: strong niche open-source/passion project with product potential.

Before it feels commercial-grade:

- installation must be simpler and less scary;
- VST3 stability should be verified across multiple DAWs;
- logs/crash diagnostics should exist;
- license and third-party notices should be clear;
- release packages should feel intentional, not like raw build folders;
- Windows signing/installer strategy should be researched.

---

<a id="ru"></a>

# Roadmap GuitarToolkit

[English version](#guitartoolkit-roadmap)

GuitarToolkit - личный, идейный гитарный набор инструментов для Windows, доступный как standalone desktop-приложение и VST3-плагин. Цель проекта - вырасти из полезного passion project в аккуратный и надёжный инструмент для практики, сочинения и работы в DAW.

## Направление продукта

- Держать проект musician-first: быстро открыть, легко понять, удобно использовать во время реальной практики или записи.
- Считать Desktop и VST3 равноправными целями, но аккуратно проверять поведение в DAW перед тем, как называть функцию стабильной.
- Предпочитать практичные улучшения workflow простому наращиванию количества функций.
- Сохранять open-source/passion-project характер и постепенно повышать качество продукта.

## Ближайшие приоритеты

- Улучшить установку VST3 и сделать очевидным, что копировать нужно весь каталог плагина.
- Поддерживать отдельные инструкции для FL Studio, Reaper и общего списка совместимости DAW.
- Развивать диагностику: логи в `%AppData%\GuitarToolkit\logs`, понятные подсказки из UI и README.
- Полировать Tabs: меньше визуальных перерисовок после resize/maximize/restore, сохранение последнего файла, дорожки, громкости, скорости и auto-follow.
- Развивать Settings как центр языка, темы, аудио, путей библиотеки табов, диагностики и reset-действий.
- Готовить интерфейс к локализации и будущим модулям без ломки текущих вкладок.

## Среднесрочные идеи

- Installer или более полированный portable package.
- DAW-aware поведение: host tempo, transport sync, меньше конфликтов hotkeys внутри DAW.
- Более сильный workflow табов: поиск, сортировка, refresh библиотеки, per-track volume/mute/solo.
- Более сильный workflow прогрессий: менеджер пресетов, drag-and-drop, undo, длительности аккордов, strum/arpeggio, export.
- Расширение interval trainer, scale practice, chord workflow, metronome и tuner в сторону реальной ежедневной практики.

## Чеклист охоты за багами

- Проверять Tabs после resize/maximize/restore.
- Проверять VST3 startup и editor opening в FL Studio после каждого релиза.
- Проверять деплой полного dependency set, включая `runtimes`.
- Проверять назначение и смену recording input после загрузки плагина.
- Тестировать GP3, GP4, GP5/GPX и MusicXML.
- Следить, чтобы Desktop и VST3 не расходились в shared UI behavior.

## Коммерческая готовность

Текущее состояние: сильный нишевый open-source/passion project с продуктовым потенциалом.

Перед commercial-grade уровнем нужны более простая установка, проверенная VST3-стабильность в разных DAW, хорошая диагностика, понятные лицензии и third-party notices, более аккуратная релизная упаковка и исследование Windows signing/installer.
