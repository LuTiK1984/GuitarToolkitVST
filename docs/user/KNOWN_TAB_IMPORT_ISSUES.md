# Known Tab Import Issues

[Русская версия](#ru)

This file tracks tablature files that fail before GuitarToolkit can render them because the alphaTab importer throws while parsing the source file.

## alphaTab GP4 Chord Import Crash

- File: `samples/tabs/bach_johann_sebastian-toccata_fugue_in_dm_bwv_565_metal_version.gp4`
- Observed: 2026-05-02
- Symptom: opening the file fails during import with `System.IndexOutOfRangeException`.
- Stack location: `AlphaTab.Importer.Gp3To5Importer.ReadChord(Beat beat)`.
- GuitarToolkit behavior: the failed file is logged, hidden from the current tab picker list for the session, and removed from recent/favorite lists so it is not retried repeatedly by accident.

Notes:

- The exception is thrown inside alphaTab before a `Score` is available, so GuitarToolkit cannot repair the parsed model after loading.
- Keep a compatible alternate export beside the file when possible, such as GP3, GP5/GPX, or MusicXML.
- Re-test this file after alphaTab package upgrades.

---

<a id="ru"></a>

# Известные проблемы импорта табов

[English version](#known-tab-import-issues)

Этот файл фиксирует табулатуры, которые падают до того, как GuitarToolkit может их отрисовать, потому что импортёр alphaTab выбрасывает исключение во время разбора исходного файла.

## Падение alphaTab при импорте аккордов GP4

- Файл: `samples/tabs/bach_johann_sebastian-toccata_fugue_in_dm_bwv_565_metal_version.gp4`
- Обнаружено: 2026-05-02
- Симптом: открытие файла падает во время импорта с `System.IndexOutOfRangeException`.
- Место в stack trace: `AlphaTab.Importer.Gp3To5Importer.ReadChord(Beat beat)`.
- Поведение GuitarToolkit: проблемный файл логируется, скрывается из текущего списка выбора табов на время сессии и удаляется из recent/favorite списков, чтобы он случайно не запускал падение снова.

Заметки:

- Исключение происходит внутри alphaTab до появления `Score`, поэтому GuitarToolkit не может исправить разобранную модель после загрузки.
- По возможности держите рядом совместимый альтернативный export: GP3, GP5/GPX или MusicXML.
- После обновлений alphaTab нужно повторно проверить этот файл.
