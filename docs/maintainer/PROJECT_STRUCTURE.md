# Project Structure

[Русская версия](#ru)

This document defines the intended repository layout for GuitarToolkit.

## Root directory

The repository root should stay clean and should contain only project entry points, build files, and GitHub-recognized community files.

Recommended root files:

| Path | Purpose |
| --- | --- |
| `README.md` | Main project overview and public landing page. |
| `LICENSE` | Project license. |
| `CHANGELOG.md` | Canonical release history. |
| `CONTRIBUTING.md` | Contribution guide recognized by GitHub. |
| `CODE_OF_CONDUCT.md` | Community rules recognized by GitHub. |
| `SECURITY.md` | Security policy recognized by GitHub. |
| `SUPPORT.md` | Support routing recognized by GitHub. |
| `RELEASE_CHECKLIST.md` | Short release checklist. |
| `THIRD_PARTY_NOTICES.md` | Third-party dependency/license notes. |
| `ROADMAP.md` | Product direction. |
| `DISCUSSIONS.md` | GitHub Discussions setup guide. |
| `AGENTS.md` | Development notes for AI/coding agents and project boundaries. |
| `.gitignore` | Ignore rules for build outputs and local files. |
| `.editorconfig` | Shared editor formatting rules, if present. |
| `GuitarToolkit.sln` | Solution file. |
| `Directory.Build.props` | Shared .NET build/version metadata. |
| `build-release.ps1` | Release package build script. |
| `deploy-vst.bat` | Local VST3 deployment helper. |

Avoid adding random notes, reports, screenshots, archives, logs, or temporary files to the root directory.

## Source projects

| Path | Purpose |
| --- | --- |
| `GuitarToolkit.Core/` | DSP, music theory models, engines, settings, and shared non-UI logic. |
| `GuitarToolkit.UI/` | Shared WPF UI used by Desktop and VST3 targets. |
| `GuitarToolkit.Desktop/` | Standalone Windows desktop application and desktop-specific integration. |
| `GuitarToolkit.Plugin/` | VST3 plugin entry point and plugin-specific integration. |
| `GuitarToolkit.Tests/` | Unit tests for core behavior. |

Important rule: `GuitarToolkit.Core` should stay independent from WPF, NAudio, and AudioPlugSharp.

## Documentation layout

Documentation is split by audience.

```text
docs/
|-- README.md
|-- user/
|   |-- README.md
|   |-- QUICK_START.md
|   |-- SUPPORTED_DAWS.md
|   |-- FL_STUDIO.md
|   |-- REAPER.md
|   |-- SUPPORT.md
|   `-- KNOWN_TAB_IMPORT_ISSUES.md
|-- maintainer/
|   |-- README.md
|   |-- MAINTAINER_GUIDE.md
|   |-- PROJECT_STRUCTURE.md
|   |-- GITHUB_SETTINGS.md
|   |-- REPOSITORY_PRESENTATION.md
|   |-- BRANCHING_AND_PROTECTION.md
|   |-- LABELS_AND_MILESTONES.md
|   |-- DEPENDENCY_POLICY.md
|   `-- DISCUSSIONS.md
|-- releases/
|   |-- README.md
|   |-- RELEASE_PROCESS.md
|   |-- RELEASE_PAGE_GUIDE.md
|   `-- RELEASE_NOTES_v1.6.0_DRAFT.md
`-- images/
```

## GitHub community files

Use `.github/` for GitHub-specific configuration:

| Path | Purpose |
| --- | --- |
| `.github/workflows/` | GitHub Actions workflows. |
| `.github/ISSUE_TEMPLATE/` | Issue forms and issue routing. |
| `.github/DISCUSSION_TEMPLATE/` | Discussion category forms. |
| `.github/PULL_REQUEST_TEMPLATE.md` | Pull request checklist. |
| `.github/dependabot.yml` | Dependabot configuration. |
| `.github/CODEOWNERS` | Code owner rules. |

GitHub-recognized community docs may stay in the root for visibility and compatibility.

## Release assets

Release ZIP files should not be committed to the repository. Use GitHub Releases for downloadable assets:

- `GuitarToolkit_DESKTOP_v.<version>.zip`;
- `GuitarToolkit_VST3_v.<version>.zip`.

Local release outputs should be ignored by `.gitignore`.

## Sample files

Guitar Pro, MusicXML, and other tab files can be large or copyright-sensitive. Do not commit random user tabs.

If safe samples are needed later, use a clearly documented path such as:

```text
samples/
```

Only include files that are legally safe to redistribute.

## Do not commit

Do not commit:

- `bin/`;
- `obj/`;
- `.vs/`;
- `.idea/`;
- local logs;
- crash dumps;
- `TestResults/`;
- coverage outputs;
- release ZIP files;
- local report files;
- user tabs or copyrighted tab files;
- temporary screenshots and exports;
- secrets, tokens, keys, or private local paths.

## Cleanup rules

When cleaning the repository:

1. Do not move or delete code files unless the change is a dedicated refactor.
2. Do not remove VST3 runtime-related files without testing plugin loading.
3. Do not delete build scripts unless the release process is updated.
4. Prefer merging duplicate docs into one canonical document.
5. Update README links after moving documentation.
6. Keep `CHANGELOG.md` as the canonical release history.
7. Keep GitHub Releases as the place for old release-specific notes and downloadable assets.

---

<a id="ru"></a>

# Структура проекта

[English version](#project-structure)

Этот документ описывает целевую структуру репозитория GuitarToolkit.

## Корень репозитория

Корень должен оставаться чистым. В нём должны лежать только точки входа проекта, build-файлы и community-файлы, которые GitHub хорошо распознаёт.

Рекомендуемые файлы в корне:

| Путь | Назначение |
| --- | --- |
| `README.md` | Главная витрина проекта. |
| `LICENSE` | Лицензия. |
| `CHANGELOG.md` | Canonical release history. |
| `CONTRIBUTING.md` | Contribution guide, распознаётся GitHub. |
| `CODE_OF_CONDUCT.md` | Правила сообщества, распознаётся GitHub. |
| `SECURITY.md` | Security policy, распознаётся GitHub. |
| `SUPPORT.md` | Support routing, распознаётся GitHub. |
| `RELEASE_CHECKLIST.md` | Короткий чеклист релиза. |
| `THIRD_PARTY_NOTICES.md` | Сторонние зависимости и лицензии. |
| `ROADMAP.md` | Направление развития. |
| `DISCUSSIONS.md` | Настройка GitHub Discussions. |
| `AGENTS.md` | Заметки для AI/coding agents и границы проекта. |
| `.gitignore` | Ignore rules. |
| `.editorconfig` | Общие правила форматирования, если файл есть. |
| `GuitarToolkit.sln` | Solution file. |
| `Directory.Build.props` | Общие .NET build/version metadata. |
| `build-release.ps1` | Скрипт сборки release packages. |
| `deploy-vst.bat` | Helper для локального VST3 deploy. |

Не добавляйте в корень случайные заметки, отчёты, скриншоты, архивы, логи или временные файлы.

## Исходные проекты

| Путь | Назначение |
| --- | --- |
| `GuitarToolkit.Core/` | DSP, теория музыки, движки, настройки и общая non-UI логика. |
| `GuitarToolkit.UI/` | Общий WPF UI для Desktop и VST3. |
| `GuitarToolkit.Desktop/` | Standalone Windows desktop app. |
| `GuitarToolkit.Plugin/` | VST3 plugin entry point. |
| `GuitarToolkit.Tests/` | Unit tests для core behavior. |

Важное правило: `GuitarToolkit.Core` не должен зависеть от WPF, NAudio и AudioPlugSharp.

## Структура документации

Документация разделена по аудитории.

```text
docs/
|-- README.md
|-- user/
|-- maintainer/
|-- releases/
`-- images/
```

## GitHub community files

`.github/` используется для GitHub-specific конфигурации:

| Путь | Назначение |
| --- | --- |
| `.github/workflows/` | GitHub Actions workflows. |
| `.github/ISSUE_TEMPLATE/` | Issue forms и routing. |
| `.github/DISCUSSION_TEMPLATE/` | Discussion category forms. |
| `.github/PULL_REQUEST_TEMPLATE.md` | Pull request checklist. |
| `.github/dependabot.yml` | Dependabot configuration. |
| `.github/CODEOWNERS` | Code owner rules. |

GitHub-recognized community docs можно оставить в корне для совместимости и видимости.

## Release assets

Release ZIP-файлы не должны попадать в репозиторий. Используйте GitHub Releases для downloadable assets:

- `GuitarToolkit_DESKTOP_v.<version>.zip`;
- `GuitarToolkit_VST3_v.<version>.zip`.

Локальные release outputs должны игнорироваться через `.gitignore`.

## Sample files

Guitar Pro, MusicXML и другие tab files могут быть большими или copyright-sensitive. Не коммитьте случайные пользовательские табы.

## Не коммитить

Не коммитьте:

- `bin/`;
- `obj/`;
- `.vs/`;
- `.idea/`;
- logs;
- crash dumps;
- `TestResults/`;
- coverage outputs;
- release ZIP files;
- local reports;
- user tabs или copyrighted tab files;
- temporary screenshots and exports;
- secrets, tokens, keys или private local paths.

## Правила чистки

1. Не перемещайте и не удаляйте code files без dedicated refactor.
2. Не удаляйте VST3 runtime-related files без проверки plugin loading.
3. Не удаляйте build scripts, если release process не обновлён.
4. Дублирующиеся docs объединяйте в canonical document.
5. После перемещения docs обновляйте links.
6. `CHANGELOG.md` остаётся canonical release history.
7. GitHub Releases — место для old release-specific notes и downloadable assets.
