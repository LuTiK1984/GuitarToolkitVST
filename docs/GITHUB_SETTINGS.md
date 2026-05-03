# GitHub Settings Guide

[Русская версия](#ru)

This document tracks repository settings that are configured through the GitHub UI rather than source files.

## About section

Recommended description:

```text
Open-source Windows guitar toolkit with tuner, metronome, music theory tools, tab viewer, and VST3 plugin support.
```

Recommended website:

```text
https://github.com/LuTiK1984/GuitarToolkit/releases
```

Recommended topics:

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

## Discussions

Recommended categories:

- Announcements;
- Q&A;
- Ideas;
- DAW Compatibility;
- Show and Tell;
- General;
- Polls.

Use `DISCUSSIONS.md` and `.github/DISCUSSION_TEMPLATE/` as the source of truth.

## Branch protection

Recommended path:

```text
Repository -> Settings -> Branches -> Add branch protection rule
```

Recommended rule:

```text
master
```

Recommended required check:

```text
CI / Build and test
```

Do not require checks that are not part of the project workflow.

## Expected CI checks

Expected checks from this repository:

| Check | Expected behavior |
| --- | --- |
| `CI / Build and test` | Runs on push and pull request. Should pass. |
| `CI / Release package check` | Runs only on manual `workflow_dispatch`. Skipped on normal push is expected. |

## External checks

If GitHub shows a check that is not defined in `.github/workflows/`, it usually comes from:

- an installed GitHub App;
- a repository ruleset;
- branch protection required checks;
- external CI/service integration;
- a previous app configuration.

Example:

```text
dynamic / submit-nuget
```

If this appears and the repository is not publishing NuGet packages, it is probably not needed for GuitarToolkit.

## How to investigate an unknown check

1. Open the failing check details.
2. Check whether the check belongs to GitHub Actions or an external app.
3. Search the repository for the check name.
4. Check installed GitHub Apps:

```text
Repository -> Settings -> GitHub Apps
```

5. Check Actions workflows:

```text
Repository -> Actions
```

6. Check branch protection/rulesets:

```text
Repository -> Settings -> Branches
Repository -> Settings -> Rules -> Rulesets
```

7. If the check is not relevant, remove it from required checks or disable the app/ruleset that creates it.

## What to require before merge

For this project, the main required check should be:

```text
CI / Build and test
```

Do not require `CI / Release package check` because it is a manual release verification job.

Do not require external checks like `submit-nuget` unless the project intentionally starts publishing NuGet packages.

---

<a id="ru"></a>

# Настройки GitHub

[English version](#github-settings-guide)

Этот документ фиксирует настройки репозитория, которые делаются через GitHub UI, а не через файлы в репозитории.

## About section

Рекомендуемое описание:

```text
Open-source Windows guitar toolkit with tuner, metronome, music theory tools, tab viewer, and VST3 plugin support.
```

Рекомендуемый website:

```text
https://github.com/LuTiK1984/GuitarToolkit/releases
```

Рекомендуемые topics:

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

## Discussions

Рекомендуемые категории:

- Announcements;
- Q&A;
- Ideas;
- DAW Compatibility;
- Show and Tell;
- General;
- Polls.

Основные источники правил: `DISCUSSIONS.md` и `.github/DISCUSSION_TEMPLATE/`.

## Branch protection

Путь настройки:

```text
Repository -> Settings -> Branches -> Add branch protection rule
```

Рекомендуемое правило:

```text
master
```

Рекомендуемый required check:

```text
CI / Build and test
```

Не требуйте checks, которые не являются частью workflow проекта.

## Ожидаемые CI checks

Ожидаемые checks от этого репозитория:

| Check | Ожидаемое поведение |
| --- | --- |
| `CI / Build and test` | Запускается на push и pull request. Должен проходить. |
| `CI / Release package check` | Запускается только вручную через `workflow_dispatch`. Skipped на обычном push — это нормально. |

## Внешние checks

Если GitHub показывает check, которого нет в `.github/workflows/`, обычно он приходит от:

- установленного GitHub App;
- repository ruleset;
- branch protection required checks;
- внешнего CI/service integration;
- старой настройки какого-то приложения.

Пример:

```text
dynamic / submit-nuget
```

Если такой check появляется, а проект не публикует NuGet packages, скорее всего он не нужен GuitarToolkit.

## Как проверить неизвестный check

1. Открыть Details у падающего check.
2. Проверить, относится ли он к GitHub Actions или внешнему app.
3. Поискать название check в репозитории.
4. Проверить GitHub Apps:

```text
Repository -> Settings -> GitHub Apps
```

5. Проверить Actions workflows:

```text
Repository -> Actions
```

6. Проверить branch protection/rulesets:

```text
Repository -> Settings -> Branches
Repository -> Settings -> Rules -> Rulesets
```

7. Если check не нужен, убрать его из required checks или отключить app/ruleset, который его создаёт.

## Что требовать перед merge

Для этого проекта основной required check:

```text
CI / Build and test
```

Не делайте required check из `CI / Release package check`, потому что это ручная release verification job.

Не требуйте внешние checks вроде `submit-nuget`, если проект намеренно не начинает публиковать NuGet packages.
