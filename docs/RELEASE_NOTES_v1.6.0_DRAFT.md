# GuitarToolkit v1.6.0 Draft Release Notes

[Русская версия](#ru)

This is a draft release note for the next repository-polish release. Final release notes should be copied into the GitHub Release page and summarized in `CHANGELOG.md` when the version is published.

## Summary

GuitarToolkit v1.6.0 focuses on open-source readiness, repository cleanup, release hygiene, CI improvements, and community workflow. The application code and user-facing music features are not the main focus of this release.

## Highlights

- Renamed and consolidated the main repository as `GuitarToolkit`.
- Added and refined bilingual GitHub Discussions setup.
- Added support, quick-start, dependency, release, maintainer, project-structure, branch-protection, GitHub-settings, repository-presentation, and labels/milestones documentation.
- Cleaned duplicate setup/install/release-note files from the repository.
- Expanded `.gitignore` to prevent accidental commits of build outputs, logs, test results, release archives, local sample tabs, and temporary files.
- Improved CI with manual release package checking.
- Updated release packaging to include current documentation instead of removed install docs.
- Added issue/discussion routing and maintainer workflow documentation.

## Added

- `SUPPORT.md` with bilingual support routing and VST3 troubleshooting information.
- `DISCUSSIONS.md` with recommended GitHub Discussions categories and welcome-post text.
- `.github/DISCUSSION_TEMPLATE/` forms for Q&A, Ideas, DAW Compatibility, Announcements, Show and Tell, and General discussions.
- `.github/ISSUE_TEMPLATE/config.yml` with contact links to Discussions, Support, and Security.
- `.github/ISSUE_TEMPLATE/task.yml` for maintenance, documentation, release, refactoring, and project-management tasks.
- `docs/QUICK_START.md` with bilingual Desktop and VST3 installation guidance.
- `docs/DEPENDENCY_POLICY.md` with Dependabot and risky-dependency review rules.
- `docs/RELEASE_PROCESS.md` with full release workflow and GitHub Release template.
- `docs/PROJECT_STRUCTURE.md` with repository layout rules and cleanup guidance.
- `docs/MAINTAINER_GUIDE.md` with issue triage, Discussions, labels, milestones, and maintainer routines.
- `docs/REPOSITORY_PRESENTATION.md` with About text, topics, pinned post text, and release description templates.
- `docs/LABELS_AND_MILESTONES.md` with recommended labels, colors, scopes, and milestone strategy.
- `docs/BRANCHING_AND_PROTECTION.md` with branch workflow and `master` protection recommendations.
- `docs/GITHUB_SETTINGS.md` with repository UI settings, expected checks, and external-check troubleshooting.

## Changed

- Reworked README community/support navigation.
- Added links from README to the new support, release, dependency, maintainer, project-structure, branching, GitHub-settings, and presentation docs.
- Updated `CONTRIBUTING.md` with dependency update policy and project-structure guidance.
- Updated `RELEASE_CHECKLIST.md` to link the full release process and include dependency/docs checks.
- Updated `build-release.ps1` to package current documentation files:
  - `README.md`;
  - `LICENSE`;
  - `CHANGELOG.md`;
  - `THIRD_PARTY_NOTICES.md`;
  - `docs/QUICK_START.md`;
  - `docs/SUPPORTED_DAWS.md`;
  - `docs/FL_STUDIO.md`;
  - `docs/REAPER.md`;
  - `KNOWN_TAB_IMPORT_ISSUES.md`.
- Improved GitHub Actions CI with:
  - `workflow_dispatch` manual runs;
  - concurrency cancellation;
  - `dotnet --info` output;
  - test result artifact upload;
  - manual release package check job;
  - release ZIP artifact upload for manual package checks.

## Removed

- Removed obsolete `SETUP.md` because setup guidance is now covered by README and `docs/QUICK_START.md`.
- Removed duplicate installation docs:
  - `docs/INSTALL_EN.md`;
  - `docs/INSTALL_RU.md`.
- Removed old root-level duplicate release-note files:
  - `RELEASE_NOTES_v1.1.0.md`;
  - `RELEASE_NOTES_v1.2.0.md`;
  - `RELEASE_NOTES_v1.3.0.md`;
  - `RELEASE_NOTES_v1.3.1.md`;
  - `RELEASE_NOTES_v1.3.2.md`;
  - `RELEASE_NOTES_v1.3.3.md`.

## Fixed

- Fixed release packaging after documentation cleanup by replacing references to removed install docs with current documentation files.
- Documented how to disable or ignore GitHub's irrelevant `Automatic Dependency Submission (NuGet)` / `dynamic / submit-nuget` check when it fails on Windows-targeted WPF projects.

## Verification

Before publishing this release, verify:

- `CI / Build and test` passes.
- Manual `CI / Release package check` succeeds.
- `build-release.ps1` creates both ZIP files.
- Desktop ZIP contains current docs.
- VST3 ZIP contains current docs and full plugin output.
- README links work.
- GitHub Discussions categories and forms work.
- Required branch checks do not include irrelevant external checks such as `dynamic / submit-nuget`.

## Known notes

- `CI / Release package check` is expected to be skipped on normal pushes. It is a manual release verification job.
- `Automatic Dependency Submission (NuGet)` may fail if enabled in GitHub Settings because the project targets Windows/WPF. Disable it if it is not needed.
- This release is mostly repository and community infrastructure work, not a major app-feature release.

---

<a id="ru"></a>

# Черновик release notes GuitarToolkit v1.6.0

[English version](#guitartoolkit-v160-draft-release-notes)

Это черновик release notes для следующего polish-релиза. Финальный текст нужно перенести в GitHub Release и кратко отразить в `CHANGELOG.md` при публикации версии.

## Кратко

GuitarToolkit v1.6.0 сфокусирован на open-source готовности, чистке репозитория, release hygiene, CI improvements и community workflow. Код приложения и музыкальные функции не являются главным фокусом этого релиза.

## Главное

- Основной репозиторий переименован и закреплён как `GuitarToolkit`.
- Добавлена и улучшена bilingual-настройка GitHub Discussions.
- Добавлена документация по support, quick start, dependencies, releases, maintainer workflow, структуре проекта, branch protection, GitHub settings, repository presentation и labels/milestones.
- Удалены duplicate setup/install/release-note файлы.
- Расширен `.gitignore`, чтобы не коммитить build outputs, logs, test results, release archives, local sample tabs и temporary files.
- Улучшен CI с ручной проверкой release packages.
- Обновлён release packaging: теперь в архивы попадает актуальная документация вместо удалённых install docs.
- Добавлены issue/discussion routing и maintainer workflow docs.

## Добавлено

- `SUPPORT.md` с bilingual support routing и VST3 troubleshooting.
- `DISCUSSIONS.md` с категориями GitHub Discussions и welcome-post текстом.
- `.github/DISCUSSION_TEMPLATE/` формы для Q&A, Ideas, DAW Compatibility, Announcements, Show and Tell и General.
- `.github/ISSUE_TEMPLATE/config.yml` с contact links на Discussions, Support и Security.
- `.github/ISSUE_TEMPLATE/task.yml` для maintenance, documentation, release, refactoring и project-management задач.
- `docs/QUICK_START.md` с bilingual установкой Desktop и VST3.
- `docs/DEPENDENCY_POLICY.md` с правилами проверки Dependabot и рискованных dependency updates.
- `docs/RELEASE_PROCESS.md` с release workflow и GitHub Release template.
- `docs/PROJECT_STRUCTURE.md` с правилами структуры репозитория.
- `docs/MAINTAINER_GUIDE.md` с triage, Discussions, labels, milestones и routine мейнтейнера.
- `docs/REPOSITORY_PRESENTATION.md` с About text, topics, pinned post text и release description templates.
- `docs/LABELS_AND_MILESTONES.md` с labels, colors, scopes и milestone strategy.
- `docs/BRANCHING_AND_PROTECTION.md` с branch workflow и рекомендациями защиты `master`.
- `docs/GITHUB_SETTINGS.md` с repository UI settings, expected checks и external-check troubleshooting.

## Изменено

- Улучшена навигация README по community/support docs.
- В README добавлены ссылки на новые support, release, dependency, maintainer, project-structure, branching, GitHub-settings и presentation docs.
- `CONTRIBUTING.md` дополнен dependency policy и project-structure guidance.
- `RELEASE_CHECKLIST.md` теперь ссылается на full release process и включает dependency/docs checks.
- `build-release.ps1` теперь кладёт в release packages актуальные docs:
  - `README.md`;
  - `LICENSE`;
  - `CHANGELOG.md`;
  - `THIRD_PARTY_NOTICES.md`;
  - `docs/QUICK_START.md`;
  - `docs/SUPPORTED_DAWS.md`;
  - `docs/FL_STUDIO.md`;
  - `docs/REAPER.md`;
  - `KNOWN_TAB_IMPORT_ISSUES.md`.
- GitHub Actions CI улучшен:
  - manual `workflow_dispatch` runs;
  - concurrency cancellation;
  - `dotnet --info` output;
  - test result artifact upload;
  - manual release package check job;
  - release ZIP artifact upload для manual package checks.

## Удалено

- Удалён устаревший `SETUP.md`, потому что setup теперь описан в README и `docs/QUICK_START.md`.
- Удалены duplicate install docs:
  - `docs/INSTALL_EN.md`;
  - `docs/INSTALL_RU.md`.
- Удалены старые duplicate release-note files из корня:
  - `RELEASE_NOTES_v1.1.0.md`;
  - `RELEASE_NOTES_v1.2.0.md`;
  - `RELEASE_NOTES_v1.3.0.md`;
  - `RELEASE_NOTES_v1.3.1.md`;
  - `RELEASE_NOTES_v1.3.2.md`;
  - `RELEASE_NOTES_v1.3.3.md`.

## Исправлено

- Исправлен release packaging после чистки документации: удалённые install docs заменены актуальными documentation files.
- Задокументировано, как отключить или игнорировать нерелевантный GitHub check `Automatic Dependency Submission (NuGet)` / `dynamic / submit-nuget`, если он падает на Windows-targeted WPF projects.

## Проверка перед публикацией

Перед публикацией релиза проверить:

- `CI / Build and test` проходит.
- Manual `CI / Release package check` проходит.
- `build-release.ps1` создаёт оба ZIP-файла.
- Desktop ZIP содержит актуальные docs.
- VST3 ZIP содержит актуальные docs и полный plugin output.
- README links работают.
- GitHub Discussions categories/forms работают.
- Required branch checks не содержат нерелевантные external checks вроде `dynamic / submit-nuget`.

## Заметки

- `CI / Release package check` должен быть skipped на обычных push. Это ручная release verification job.
- `Automatic Dependency Submission (NuGet)` может падать, если включён в GitHub Settings, потому что проект таргетит Windows/WPF. Его можно отключить, если он не нужен.
- Это в основном repository/community infrastructure release, а не крупный feature release приложения.
