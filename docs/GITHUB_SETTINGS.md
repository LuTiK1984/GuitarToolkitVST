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

## Automatic Dependency Submission

GitHub may show a workflow named:

```text
Automatic Dependency Submission (NuGet)
```

with a job/check like:

```text
dynamic / submit-nuget
```

This is not defined in `.github/workflows/ci.yml`. It is created by GitHub's Automatic Dependency Submission feature for the Dependency Graph.

For this project, it may fail because GuitarToolkit targets Windows/WPF and the automatic NuGet dependency submission job can run on a non-Windows environment.

If the project does not need automatic NuGet dependency submission, disable it in the GitHub UI:

```text
Repository -> Settings -> Security and quality / Advanced Security -> Dependency graph -> Automatic dependency submission -> Disabled
```

Depending on GitHub UI wording, this setting may also appear under:

```text
Repository -> Settings -> Code security and analysis
```

This should not disable the normal dependency graph or Dependabot alerts unless those settings are changed separately.

## External checks

If GitHub shows a check that is not defined in `.github/workflows/`, it usually comes from:

- an installed GitHub App;
- a repository ruleset;
- branch protection required checks;
- external CI/service integration;
- a previous app configuration;
- GitHub Advanced Security / Dependency Graph features.

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

7. Check Advanced Security / Code security settings:

```text
Repository -> Settings -> Security and quality / Advanced Security
Repository -> Settings -> Code security and analysis
```

8. If the check is not relevant, remove it from required checks or disable the app/ruleset/security feature that creates it.

## What to require before merge

For this project, the main required check should be:

```text
CI / Build and test
```

Do not require `CI / Release package check` because it is a manual release verification job.

Do not require external checks like `submit-nuget` unless the project intentionally starts publishing NuGet packages or relying on automatic dependency submission.

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

## Automatic Dependency Submission

GitHub может показывать workflow:

```text
Automatic Dependency Submission (NuGet)
```

с job/check:

```text
dynamic / submit-nuget
```

Этот workflow не задан в `.github/workflows/ci.yml`. Его создаёт функция GitHub Automatic Dependency Submission для Dependency Graph.

В этом проекте он может падать, потому что GuitarToolkit таргетит Windows/WPF, а automatic NuGet dependency submission может запускаться в non-Windows environment.

Если проекту не нужна automatic NuGet dependency submission, отключите её в GitHub UI:

```text
Repository -> Settings -> Security and quality / Advanced Security -> Dependency graph -> Automatic dependency submission -> Disabled
```

В зависимости от версии GitHub UI настройка может находиться здесь:

```text
Repository -> Settings -> Code security and analysis
```

Это не должно отключить обычный dependency graph или Dependabot alerts, если не менять их отдельно.

## Внешние checks

Если GitHub показывает check, которого нет в `.github/workflows/`, обычно он приходит от:

- установленного GitHub App;
- repository ruleset;
- branch protection required checks;
- внешнего CI/service integration;
- старой настройки какого-то приложения;
- GitHub Advanced Security / Dependency Graph features.

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

7. Проверить Advanced Security / Code security settings:

```text
Repository -> Settings -> Security and quality / Advanced Security
Repository -> Settings -> Code security and analysis
```

8. Если check не нужен, убрать его из required checks или отключить app/ruleset/security feature, который его создаёт.

## Что требовать перед merge

Для этого проекта основной required check:

```text
CI / Build and test
```

Не делайте required check из `CI / Release package check`, потому что это ручная release verification job.

Не требуйте внешние checks вроде `submit-nuget`, если проект намеренно не начинает публиковать NuGet packages или полагаться на automatic dependency submission.
