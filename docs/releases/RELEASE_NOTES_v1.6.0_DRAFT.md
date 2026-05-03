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

- `docs/user/QUICK_START.md` with bilingual Desktop and VST3 installation guidance.
- `docs/user/SUPPORT.md` with support routing and VST3 troubleshooting information.
- `docs/maintainer/` documentation for dependency policy, release process, project structure, GitHub settings, repository presentation, labels/milestones, and branch protection.
- `docs/releases/` documentation for release workflow and GitHub Releases page hygiene.
- GitHub Discussion forms and issue routing.
- `docs/README.md` as a centralized documentation index.

## Changed

- Reworked README as a cleaner user-facing project landing page.
- Updated documentation hierarchy into `docs/user`, `docs/maintainer`, and `docs/releases`.
- Updated `build-release.ps1` to package current user-facing documentation files.
- Improved GitHub Actions CI with manual release package checking and test result artifacts.

## Removed

- Removed obsolete setup/install docs and old duplicate release notes from the root/top-level documentation area.

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
