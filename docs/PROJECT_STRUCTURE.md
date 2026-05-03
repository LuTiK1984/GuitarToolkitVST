# Project Structure

[Русская версия](#ru)

This document explains where project files should live and what should not be committed. The goal is to keep GuitarToolkit easy to navigate for users, contributors, and maintainers.

## Root directory

The repository root should stay clean. Keep only files that are expected at the top level of a GitHub project or are required by the build.

Recommended root files:

| Path | Purpose |
| --- | --- |
| `README.md` | Main project overview and entry point. |
| `LICENSE` | Project license. |
| `CHANGELOG.md` | Release history and user-visible changes. |
| `CONTRIBUTING.md` | Contribution rules and development workflow. |
| `CODE_OF_CONDUCT.md` | Community behavior rules. |
| `SECURITY.md` | Security reporting policy. |
| `SUPPORT.md` | Support routing and help policy. |
| `RELEASE_CHECKLIST.md` | Short release checklist. |
| `THIRD_PARTY_NOTICES.md` | Third-party dependency/license notes. |
| `ROADMAP.md` | Product direction and planned work. |
| `KNOWN_TAB_IMPORT_ISSUES.md` | Known tab import limitations. |
| `DISCUSSIONS.md` | GitHub Discussions setup and guidance. |
| `.gitignore` | Ignore rules for build outputs and local files. |
| `.editorconfig` | Shared editor formatting rules, if present. |
| `GuitarToolkit.sln` | Solution file. |
| `Directory.Build.props` | Shared .NET build/version metadata. |
| `build-release.ps1` | Release package build script. |
| `deploy-vst.bat` | Local VST3 deployment helper. |

Avoid adding random notes, reports, screenshots, archives, logs, or temporary files to the root directory.

## Source projects

Main source directories:

| Path | Purpose |
| --- | --- |
| `GuitarToolkit.Core/` | DSP, music theory models, engines, settings, and shared non-UI logic. |
| `GuitarToolkit.UI/` | Shared WPF UI used by Desktop and VST3 targets. |
| `GuitarToolkit.Desktop/` | Standalone Windows desktop application and desktop-specific integration. |
| `GuitarToolkit.Plugin/` | VST3 plugin entry point and plugin-specific integration. |
| `GuitarToolkit.Tests/` | Unit tests for core behavior. |

Important rule: `GuitarToolkit.Core` should stay independent from WPF, NAudio, and AudioPlugSharp.

## Documentation

Use `docs/` for longer documentation that does not need to be in the root.

Recommended docs:

| Path | Purpose |
| --- | --- |
| `docs/QUICK_START.md` | Desktop and VST3 installation walkthrough. |
| `docs/DEPENDENCY_POLICY.md` | Dependabot and dependency update rules. |
| `docs/RELEASE_PROCESS.md` | Full release process and release note template. |
| `docs/PROJECT_STRUCTURE.md` | This file. |
| `docs/FL_STUDIO.md` | FL Studio setup notes. |
| `docs/REAPER.md` | Reaper setup notes. |
| `docs/SUPPORTED_DAWS.md` | DAW compatibility tracking. |
| `docs/images/` | README and documentation screenshots. |

When adding new documentation:

- put long guides in `docs/`;
- link them from README only when they are useful to most users;
- avoid duplicate English/Russian files when a single bilingual document is clearer;
- remove or merge obsolete docs instead of keeping old copies.

## GitHub community files

Use `.github/` for GitHub-specific configuration.

Recommended structure:

| Path | Purpose |
| --- | --- |
| `.github/workflows/` | GitHub Actions workflows. |
| `.github/ISSUE_TEMPLATE/` | Issue forms and issue routing. |
| `.github/DISCUSSION_TEMPLATE/` | Discussion category forms. |
| `.github/PULL_REQUEST_TEMPLATE.md` | Pull request checklist. |
| `.github/dependabot.yml` | Dependabot configuration. |
| `.github/CODEOWNERS` | Code owner rules. |

Do not put general user documentation inside `.github/` unless GitHub requires it there.

## Screenshots and media

Use:

```text
docs/images/
```

For:

- README screenshots;
- documentation screenshots;
- UI examples;
- DAW setup screenshots.

Avoid committing large raw media, temporary exports, or duplicated screenshots.

## Release assets

Release ZIP files should not be committed to the repository.

Use GitHub Releases for:

- `GuitarToolkit_DESKTOP_v.<version>.zip`;
- `GuitarToolkit_VST3_v.<version>.zip`;
- release notes;
- release-specific downloadable assets.

Local release outputs should be ignored by `.gitignore`.

## Sample files

Guitar Pro, MusicXML, and other tab files can be large or copyright-sensitive. Do not commit random user tabs.

If test samples are needed later, use a clearly documented path such as:

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
- secrets, tokens, keys, or local machine paths that should not be public.

## Cleanup rules

When cleaning the repository:

1. Do not move or delete code files unless the change is a dedicated refactor.
2. Do not remove VST3 runtime-related files without testing plugin loading.
3. Do not delete build scripts unless the release process is updated.
4. Prefer merging duplicate docs into one canonical document.
5. Update README links after moving documentation.
6. Keep `CHANGELOG.md` as the canonical release history.
7. Keep GitHub Release pages as the place for old release-specific notes and downloadable assets.

---

<a id="ru"></a>

# Структура проекта

[English version](#project-structure)

Этот документ объясняет, где должны лежать файлы проекта и что не нужно коммитить. Цель — сохранить GuitarToolkit понятным для пользователей, контрибьюторов и мейнтейнеров.

## Корень репозитория

Корень репозитория должен оставаться чистым. В нём стоит держать только файлы, которые обычно ожидаются на верхнем уровне GitHub-проекта или нужны для сборки.

Рекомендуемые файлы в корне:

| Путь | Назначение |
| --- | --- |
| `README.md` | Главная витрина проекта. |
| `LICENSE` | Лицензия проекта. |
| `CHANGELOG.md` | История релизов и пользовательских изменений. |
| `CONTRIBUTING.md` | Правила участия и workflow разработки. |
| `CODE_OF_CONDUCT.md` | Правила общения в сообществе. |
| `SECURITY.md` | Политика сообщений о проблемах безопасности. |
| `SUPPORT.md` | Куда писать за помощью. |
| `RELEASE_CHECKLIST.md` | Короткий чеклист релиза. |
| `THIRD_PARTY_NOTICES.md` | Заметки о сторонних зависимостях и лицензиях. |
| `ROADMAP.md` | Направление развития проекта. |
| `KNOWN_TAB_IMPORT_ISSUES.md` | Известные ограничения импорта табов. |
| `DISCUSSIONS.md` | Настройка и правила GitHub Discussions. |
| `.gitignore` | Правила игнорирования build/local файлов. |
| `.editorconfig` | Общие правила форматирования редактора, если файл есть. |
| `GuitarToolkit.sln` | Solution file. |
| `Directory.Build.props` | Общие .NET build/version metadata. |
| `build-release.ps1` | Скрипт сборки release packages. |
| `deploy-vst.bat` | Локальный helper для деплоя VST3. |

Не добавляйте в корень случайные заметки, отчёты, скриншоты, архивы, логи или временные файлы.

## Исходные проекты

Основные директории кода:

| Путь | Назначение |
| --- | --- |
| `GuitarToolkit.Core/` | DSP, теория музыки, движки, настройки и общая non-UI логика. |
| `GuitarToolkit.UI/` | Общий WPF UI для Desktop и VST3. |
| `GuitarToolkit.Desktop/` | Standalone Windows desktop app и desktop-specific интеграция. |
| `GuitarToolkit.Plugin/` | VST3 entry point и plugin-specific интеграция. |
| `GuitarToolkit.Tests/` | Unit tests для core behavior. |

Важное правило: `GuitarToolkit.Core` не должен зависеть от WPF, NAudio и AudioPlugSharp.

## Документация

Используйте `docs/` для длинной документации, которой не обязательно лежать в корне.

Рекомендуемые docs:

| Путь | Назначение |
| --- | --- |
| `docs/QUICK_START.md` | Установка Desktop и VST3. |
| `docs/DEPENDENCY_POLICY.md` | Правила Dependabot и обновления зависимостей. |
| `docs/RELEASE_PROCESS.md` | Полный процесс релиза и шаблон release notes. |
| `docs/PROJECT_STRUCTURE.md` | Этот файл. |
| `docs/FL_STUDIO.md` | Настройка FL Studio. |
| `docs/REAPER.md` | Настройка Reaper. |
| `docs/SUPPORTED_DAWS.md` | Трекинг совместимости DAW. |
| `docs/images/` | Скриншоты для README и документации. |

При добавлении новой документации:

- длинные инструкции кладите в `docs/`;
- добавляйте ссылки в README только на то, что полезно большинству пользователей;
- избегайте отдельных English/Russian дублей, если один bilingual-документ понятнее;
- удаляйте или объединяйте устаревшие docs вместо хранения старых копий.

## GitHub community files

Используйте `.github/` для GitHub-specific конфигурации.

Рекомендуемая структура:

| Путь | Назначение |
| --- | --- |
| `.github/workflows/` | GitHub Actions workflows. |
| `.github/ISSUE_TEMPLATE/` | Issue forms и issue routing. |
| `.github/DISCUSSION_TEMPLATE/` | Discussion category forms. |
| `.github/PULL_REQUEST_TEMPLATE.md` | Pull request checklist. |
| `.github/dependabot.yml` | Dependabot configuration. |
| `.github/CODEOWNERS` | Code owner rules. |

Не кладите обычную пользовательскую документацию в `.github/`, если GitHub не требует этот путь.

## Скриншоты и медиа

Используйте:

```text
docs/images/
```

Для:

- README screenshots;
- screenshots в документации;
- UI examples;
- DAW setup screenshots.

Не коммитьте большие raw media, временные exports или дублирующиеся screenshots.

## Release assets

Release ZIP-файлы не должны попадать в репозиторий.

Используйте GitHub Releases для:

- `GuitarToolkit_DESKTOP_v.<version>.zip`;
- `GuitarToolkit_VST3_v.<version>.zip`;
- release notes;
- release-specific downloadable assets.

Локальные release outputs должны игнорироваться через `.gitignore`.

## Sample files

Guitar Pro, MusicXML и другие tab files могут быть большими или copyright-sensitive. Не коммитьте случайные пользовательские табы.

Если позже понадобятся test samples, используйте явно описанный путь, например:

```text
samples/
```

Добавляйте только файлы, которые можно легально распространять.

## Не коммитить

Не коммитьте:

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
- user tabs или copyrighted tab files;
- temporary screenshots and exports;
- secrets, tokens, keys или local machine paths, которые не должны быть публичными.

## Правила чистки

При чистке репозитория:

1. Не перемещайте и не удаляйте code files, если это не отдельный dedicated refactor.
2. Не удаляйте VST3 runtime-related files без проверки загрузки плагина.
3. Не удаляйте build scripts, если release process не обновлён.
4. Лучше объединять duplicate docs в один canonical document.
5. После перемещения документации обновляйте README links.
6. `CHANGELOG.md` остаётся canonical release history.
7. GitHub Releases остаются местом для старых release-specific notes и downloadable assets.
