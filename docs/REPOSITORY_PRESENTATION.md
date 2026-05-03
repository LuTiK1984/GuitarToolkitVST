# Repository Presentation

[Русская версия](#ru)

This document contains ready-to-use text for the GitHub repository About section, topics, pinned posts, and release descriptions.

## GitHub About description

Recommended short description:

```text
Open-source Windows guitar toolkit with tuner, metronome, music theory tools, tab viewer, and VST3 plugin support.
```

Alternative shorter version:

```text
Open-source Windows guitar toolkit and VST3 plugin for practice, theory, tabs, and DAW workflows.
```

## GitHub About website

Recommended link:

```text
https://github.com/LuTiK1984/GuitarToolkit/releases
```

Use the Releases page as the website link until the project has a dedicated website or documentation page.

## Recommended topics

Use these GitHub topics:

```text
guitar
music
music-theory
tuner
metronome
guitar-tabs
vst3
daw
wpf
dotnet
csharp
windows
open-source
desktop-app
audio
```

Optional topics if GitHub accepts them and they stay relevant:

```text
reaper
fl-studio
guitar-pro
musicxml
alphatab
```

## Social preview idea

If a custom social preview image is added later, it should show:

- GuitarToolkit name;
- dark purple UI style;
- guitar/music-tooling identity;
- small labels: Tuner, Metronome, Chords, Scales, Tabs, VST3.

Suggested text:

```text
GuitarToolkit
Open-source guitar toolkit for Windows and VST3
```

## Pinned repository announcement

Use this in Discussions -> Announcements or as a pinned welcome discussion:

```md
# Welcome to GuitarToolkit

GuitarToolkit is an open-source Windows guitar toolkit for practice, writing, and DAW workflows.

It includes:

- real-time tuner;
- metronome;
- chord library;
- scales and fretboard view;
- interval trainer;
- progression builder;
- circle of fifths;
- Guitar Pro / MusicXML tab viewer;
- standalone desktop app;
- VST3 plugin package.

Downloads are available in GitHub Releases:

https://github.com/LuTiK1984/GuitarToolkit/releases

For installation help, see the Quick Start guide:

https://github.com/LuTiK1984/GuitarToolkit/blob/master/docs/QUICK_START.md

Use Discussions for setup questions, ideas, DAW compatibility reports, and general feedback.
Use Issues for reproducible bugs and clearly scoped development tasks.
```

## Short project pitch

Use this in posts, project descriptions, or release summaries:

```text
GuitarToolkit is an open-source Windows toolkit for guitar practice and DAW work. It combines a tuner, metronome, chord and scale tools, interval training, progression building, circle of fifths, and tab viewing in one standalone app and VST3 plugin.
```

## Release description template

Use this for GitHub Releases:

```md
# GuitarToolkit v<version>

## Highlights

- ...
- ...
- ...

## Downloads

- `GuitarToolkit_DESKTOP_v.<version>.zip` - standalone Windows desktop app.
- `GuitarToolkit_VST3_v.<version>.zip` - VST3 plugin package for DAW hosts.

## Installation notes

For Desktop, extract the ZIP and run the desktop executable.

For VST3, copy the whole `GuitarToolkit` plugin folder to:

```text
C:\Program Files\Common Files\VST3\GuitarToolkit\
```

Do not copy only the `.vst3` file. The plugin needs its DLL dependencies and `runtimes` folder.

## Verification

- Build: passed.
- Tests: passed.
- Desktop smoke test: passed / not checked.
- VST3 smoke test: passed / not checked.

## Known issues

- ...

## Links

- Quick Start: https://github.com/LuTiK1984/GuitarToolkit/blob/master/docs/QUICK_START.md
- Discussions: https://github.com/LuTiK1984/GuitarToolkit/discussions
- Supported DAWs: https://github.com/LuTiK1984/GuitarToolkit/blob/master/docs/SUPPORTED_DAWS.md
```

## Suggested release summary for repository-polish release

Use this if the next release is mostly documentation, repository cleanup, and community setup:

```md
# GuitarToolkit v1.6.0

This release focuses on project polish, documentation, release hygiene, and open-source community setup.

## Highlights

- Added GitHub Discussions setup and bilingual discussion forms.
- Added Support, Quick Start, Dependency Policy, Release Process, Project Structure, and Maintainer Guide documentation.
- Improved README navigation and repository links after the project rename.
- Cleaned duplicate setup/install/release-notes files from the repository root.
- Expanded `.gitignore` to prevent committing build outputs, logs, test results, release archives, and local files.
- Added issue routing and task templates for better project maintenance.

## Downloads

- `GuitarToolkit_DESKTOP_v1.6.0.zip` - standalone Windows desktop app.
- `GuitarToolkit_VST3_v1.6.0.zip` - VST3 plugin package.

## Installation notes

For VST3, copy the whole `GuitarToolkit` plugin folder to:

`C:\Program Files\Common Files\VST3\GuitarToolkit\`

Do not copy only the `.vst3` file.

## Links

- Quick Start: https://github.com/LuTiK1984/GuitarToolkit/blob/master/docs/QUICK_START.md
- Discussions: https://github.com/LuTiK1984/GuitarToolkit/discussions
```

---

<a id="ru"></a>

# Витрина репозитория

[English version](#repository-presentation)

Этот документ содержит готовые тексты для GitHub About section, topics, закреплённых постов и описаний релизов.

## Описание для GitHub About

Рекомендуемое короткое описание:

```text
Open-source Windows guitar toolkit with tuner, metronome, music theory tools, tab viewer, and VST3 plugin support.
```

Более короткий вариант:

```text
Open-source Windows guitar toolkit and VST3 plugin for practice, theory, tabs, and DAW workflows.
```

## Website для GitHub About

Рекомендуемая ссылка:

```text
https://github.com/LuTiK1984/GuitarToolkit/releases
```

Пока у проекта нет отдельного сайта, лучше использовать страницу Releases.

## Рекомендуемые topics

Используйте эти GitHub topics:

```text
guitar
music
music-theory
tuner
metronome
guitar-tabs
vst3
daw
wpf
dotnet
csharp
windows
open-source
desktop-app
audio
```

Опциональные topics, если GitHub их принимает и они остаются актуальны:

```text
reaper
fl-studio
guitar-pro
musicxml
alphatab
```

## Идея social preview

Если позже добавлять custom social preview image, на ней стоит показать:

- название GuitarToolkit;
- тёмно-фиолетовый UI style;
- guitar/music-tooling identity;
- небольшие подписи: Tuner, Metronome, Chords, Scales, Tabs, VST3.

Текст:

```text
GuitarToolkit
Open-source guitar toolkit for Windows and VST3
```

## Закреплённый announcement

Используйте это в Discussions -> Announcements или как закреплённый welcome discussion:

```md
# Welcome to GuitarToolkit

GuitarToolkit is an open-source Windows guitar toolkit for practice, writing, and DAW workflows.

It includes:

- real-time tuner;
- metronome;
- chord library;
- scales and fretboard view;
- interval trainer;
- progression builder;
- circle of fifths;
- Guitar Pro / MusicXML tab viewer;
- standalone desktop app;
- VST3 plugin package.

Downloads are available in GitHub Releases:

https://github.com/LuTiK1984/GuitarToolkit/releases

For installation help, see the Quick Start guide:

https://github.com/LuTiK1984/GuitarToolkit/blob/master/docs/QUICK_START.md

Use Discussions for setup questions, ideas, DAW compatibility reports, and general feedback.
Use Issues for reproducible bugs and clearly scoped development tasks.
```

## Короткое описание проекта

Можно использовать в постах, описаниях проекта или release summaries:

```text
GuitarToolkit is an open-source Windows toolkit for guitar practice and DAW work. It combines a tuner, metronome, chord and scale tools, interval training, progression building, circle of fifths, and tab viewing in one standalone app and VST3 plugin.
```

## Шаблон описания релиза

Используйте для GitHub Releases:

```md
# GuitarToolkit v<version>

## Highlights

- ...
- ...
- ...

## Downloads

- `GuitarToolkit_DESKTOP_v.<version>.zip` - standalone Windows desktop app.
- `GuitarToolkit_VST3_v.<version>.zip` - VST3 plugin package for DAW hosts.

## Installation notes

For Desktop, extract the ZIP and run the desktop executable.

For VST3, copy the whole `GuitarToolkit` plugin folder to:

```text
C:\Program Files\Common Files\VST3\GuitarToolkit\
```

Do not copy only the `.vst3` file. The plugin needs its DLL dependencies and `runtimes` folder.

## Verification

- Build: passed.
- Tests: passed.
- Desktop smoke test: passed / not checked.
- VST3 smoke test: passed / not checked.

## Known issues

- ...

## Links

- Quick Start: https://github.com/LuTiK1984/GuitarToolkit/blob/master/docs/QUICK_START.md
- Discussions: https://github.com/LuTiK1984/GuitarToolkit/discussions
- Supported DAWs: https://github.com/LuTiK1984/GuitarToolkit/blob/master/docs/SUPPORTED_DAWS.md
```

## Release summary для polish-релиза

Если следующий релиз в основном про документацию, чистку репозитория и community setup:

```md
# GuitarToolkit v1.6.0

This release focuses on project polish, documentation, release hygiene, and open-source community setup.

## Highlights

- Added GitHub Discussions setup and bilingual discussion forms.
- Added Support, Quick Start, Dependency Policy, Release Process, Project Structure, and Maintainer Guide documentation.
- Improved README navigation and repository links after the project rename.
- Cleaned duplicate setup/install/release-notes files from the repository root.
- Expanded `.gitignore` to prevent committing build outputs, logs, test results, release archives, and local files.
- Added issue routing and task templates for better project maintenance.

## Downloads

- `GuitarToolkit_DESKTOP_v1.6.0.zip` - standalone Windows desktop app.
- `GuitarToolkit_VST3_v1.6.0.zip` - VST3 plugin package.

## Installation notes

For VST3, copy the whole `GuitarToolkit` plugin folder to:

`C:\Program Files\Common Files\VST3\GuitarToolkit\`

Do not copy only the `.vst3` file.

## Links

- Quick Start: https://github.com/LuTiK1984/GuitarToolkit/blob/master/docs/QUICK_START.md
- Discussions: https://github.com/LuTiK1984/GuitarToolkit/discussions
```
