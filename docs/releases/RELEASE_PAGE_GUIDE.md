# GitHub Releases Page Guide

[Русская версия](#ru)

This guide describes how to keep the GitHub Releases page clean and useful for GuitarToolkit users.

For many users, the Releases page is the real download page. It should be simple, clear, and trustworthy.

## Required assets

Every public release should include:

```text
GuitarToolkit_DESKTOP_v.<version>.zip
GuitarToolkit_VST3_v.<version>.zip
```

Do not rely only on GitHub's automatic source archives.

## Asset naming

Use consistent names:

```text
GuitarToolkit_DESKTOP_v.1.6.0.zip
GuitarToolkit_VST3_v.1.6.0.zip
```

Avoid vague names such as `release.zip`, `plugin.zip`, `build.zip`, `final.zip`, or `final2.zip`.

## Release title

Recommended format:

```text
GuitarToolkit v1.6.0
```

For pre-releases:

```text
GuitarToolkit v1.6.0-beta.1
```

## Release description structure

Recommended structure:

```md
# GuitarToolkit v<version>

## Highlights

- ...

## Downloads

- `GuitarToolkit_DESKTOP_v.<version>.zip` - standalone Windows desktop app.
- `GuitarToolkit_VST3_v.<version>.zip` - VST3 plugin package for DAW hosts.

## Installation notes

For Desktop, extract the ZIP and run the desktop executable.

For VST3, copy the whole `GuitarToolkit` plugin folder to:

`C:\Program Files\Common Files\VST3\GuitarToolkit\`

Do not copy only the `.vst3` file. The plugin needs its DLL dependencies and the `runtimes` folder.

## Verification

- Build: passed.
- Tests: passed.
- Desktop smoke test: passed / not checked.
- VST3 smoke test: passed / not checked.

## Known issues

- ...

## Links

- Quick Start: https://github.com/LuTiK1984/GuitarToolkit/blob/master/docs/user/QUICK_START.md
- Discussions: https://github.com/LuTiK1984/GuitarToolkit/discussions
- Supported DAWs: https://github.com/LuTiK1984/GuitarToolkit/blob/master/docs/user/SUPPORTED_DAWS.md
```

## Latest release checklist

The latest release should have:

- clear title;
- useful description;
- Desktop ZIP;
- VST3 ZIP;
- install notes;
- known issues;
- verification notes;
- links to Quick Start and Discussions.

## Cleanup routine for old releases

Do not delete old releases without a reason. Instead:

- make sure old releases have correct assets;
- mark broken releases as pre-release or add a warning if needed;
- keep the latest release polished;
- remove accidentally uploaded local artifacts if they are not useful.

---

<a id="ru"></a>

# Руководство по странице GitHub Releases

[English version](#github-releases-page-guide)

Этот документ описывает, как держать страницу GitHub Releases чистой и полезной для пользователей GuitarToolkit.

Для многих пользователей Releases — фактическая страница загрузки. Она должна быть простой, понятной и вызывающей доверие.

## Обязательные assets

Каждый публичный релиз должен включать:

```text
GuitarToolkit_DESKTOP_v.<version>.zip
GuitarToolkit_VST3_v.<version>.zip
```

Не полагайтесь только на source archives, которые GitHub создаёт автоматически.

## Имена assets

Используйте одинаковый формат:

```text
GuitarToolkit_DESKTOP_v.1.6.0.zip
GuitarToolkit_VST3_v.1.6.0.zip
```

## Название релиза

```text
GuitarToolkit v1.6.0
```

## Описание релиза

Релиз должен содержать highlights, downloads, installation notes, verification, known issues и links.

Главное предупреждение для VST3:

```text
Copy the whole GuitarToolkit plugin folder. Do not copy only the .vst3 file.
```

## Чеклист latest release

- понятное название;
- Desktop ZIP;
- VST3 ZIP;
- install notes;
- known issues;
- links to Quick Start and Discussions.
