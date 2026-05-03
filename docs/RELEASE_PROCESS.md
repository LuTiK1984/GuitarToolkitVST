# Release Process

[Русская версия](#ru)

This document describes the preferred release flow for GuitarToolkit. Use it together with `RELEASE_CHECKLIST.md`.

GuitarToolkit ships as both a standalone Windows desktop app and a VST3 plugin, so every release should verify build output, user-facing documentation, and basic audio/DAW behavior.

## Release goals

A good release should be:

- reproducible;
- clearly versioned;
- tested in CI;
- manually smoke-tested where CI cannot help;
- documented in `CHANGELOG.md`;
- published with clear GitHub Release notes and assets.

## 1. Prepare the release branch

Create a dedicated branch for release preparation if the release includes more than a tiny metadata update.

Recommended branch name:

```text
release/v<version>
```

Examples:

```text
release/v1.6.0
release/v1.6.1
```

## 2. Update version references

Check and update version references in:

- `Directory.Build.props`;
- `GuitarToolkit.Plugin/GuitarToolkitPlugin.cs` if the VST3 plugin version is defined there;
- desktop window title or About page if the visible app version is shown in UI;
- README download asset names if they mention a specific version;
- `CHANGELOG.md`.

## 3. Update changelog

Move relevant items from `Unreleased` into a new version section:

```md
## [1.6.0] - YYYY-MM-DD
```

Suggested sections:

- Added;
- Changed;
- Fixed;
- Known issues;
- Verified.

Keep release notes user-focused. Avoid dumping internal commit noise unless it matters to users or contributors.

## 4. Build and test

Run:

```powershell
dotnet restore GuitarToolkit.sln
dotnet build GuitarToolkit.sln --configuration Debug
dotnet test GuitarToolkit.sln --configuration Debug
```

Expected:

- restore succeeds;
- build has 0 errors;
- tests pass;
- no unexpected warnings are introduced.

## 5. Build release packages

Run:

```powershell
powershell -ExecutionPolicy Bypass -File .\build-release.ps1 -Version <version> -Configuration Release
```

Expected release assets:

```text
GuitarToolkit_DESKTOP_v.<version>.zip
GuitarToolkit_VST3_v.<version>.zip
```

Check that VST3 package contains the full plugin folder and required runtime files, not only the `.vst3` file.

## 6. Manual Desktop smoke test

Check at minimum:

- app launches;
- About/version information is correct;
- Tuner tab opens;
- input device selection works;
- tuner receives input;
- metronome starts and stops;
- chord or scale playback works;
- Tabs page opens;
- logs folder is accessible if relevant.

## 7. Manual VST3 smoke test

Check at minimum in at least one DAW when possible:

- old plugin files are replaced or cleanly overwritten;
- DAW rescans VST3 plugins;
- GuitarToolkit appears in the plugin list;
- plugin loads on a track;
- editor opens;
- input assignment works if the DAW requires it;
- tuner input works;
- metronome/chord/scale playback works;
- Tabs page opens if supported;
- logs do not show new startup errors.

Recommended DAW targets over time:

- FL Studio;
- Reaper;
- Cubase;
- Ableton Live;
- Studio One;
- Bitwig Studio.

## 8. Create GitHub Release

Create a new GitHub Release with tag:

```text
v<version>
```

Upload:

- `GuitarToolkit_DESKTOP_v.<version>.zip`;
- `GuitarToolkit_VST3_v.<version>.zip`.

Release description should include:

- short release summary;
- highlights;
- installation notes;
- known issues;
- verification status;
- links to Quick Start and Discussions.

## 9. Suggested release note template

```md
# GuitarToolkit v<version>

## Highlights

- ...

## Downloads

- `GuitarToolkit_DESKTOP_v.<version>.zip` - standalone Windows desktop app.
- `GuitarToolkit_VST3_v.<version>.zip` - VST3 plugin package.

## Install notes

For VST3, copy the whole `GuitarToolkit` plugin folder to:

`C:\Program Files\Common Files\VST3\GuitarToolkit\`

Do not copy only the `.vst3` file.

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
```

## 10. Post-release checks

After publishing:

- download both ZIP files from the GitHub Release;
- confirm archive names are correct;
- confirm README links point to the current repository;
- confirm Discussions and Issues routes are still useful;
- check CI on `master`;
- open or update follow-up issues for known problems.

---

<a id="ru"></a>

# Процесс релиза

[English version](#release-process)

Этот документ описывает рекомендуемый процесс релиза GuitarToolkit. Используйте его вместе с `RELEASE_CHECKLIST.md`.

GuitarToolkit поставляется как standalone Windows desktop app и как VST3 plugin, поэтому каждый релиз должен проверять сборку, пользовательскую документацию и базовое audio/DAW-поведение.

## Цели релиза

Хороший релиз должен быть:

- воспроизводимым;
- с понятной версией;
- проверенным через CI;
- вручную проверенным там, где CI не помогает;
- описанным в `CHANGELOG.md`;
- опубликованным в GitHub Release с понятными notes и assets.

## 1. Подготовить release branch

Если релиз включает не только маленькое metadata-изменение, лучше создать отдельную ветку.

Рекомендуемое имя ветки:

```text
release/v<version>
```

Примеры:

```text
release/v1.6.0
release/v1.6.1
```

## 2. Обновить версии

Проверить и обновить версии в:

- `Directory.Build.props`;
- `GuitarToolkit.Plugin/GuitarToolkitPlugin.cs`, если версия VST3 указана там;
- desktop window title или About page, если версия видна в UI;
- README download asset names, если там указана конкретная версия;
- `CHANGELOG.md`.

## 3. Обновить changelog

Перенести нужные пункты из `Unreleased` в новый раздел версии:

```md
## [1.6.0] - YYYY-MM-DD
```

Рекомендуемые секции:

- Added;
- Changed;
- Fixed;
- Known issues;
- Verified.

Release notes должны быть понятны пользователям. Не нужно превращать их в шум из внутренних commit details.

## 4. Собрать и протестировать

Запустить:

```powershell
dotnet restore GuitarToolkit.sln
dotnet build GuitarToolkit.sln --configuration Debug
dotnet test GuitarToolkit.sln --configuration Debug
```

Ожидается:

- restore проходит;
- build без ошибок;
- tests проходят;
- новые неожиданные warnings не появились.

## 5. Собрать release packages

Запустить:

```powershell
powershell -ExecutionPolicy Bypass -File .\build-release.ps1 -Version <version> -Configuration Release
```

Ожидаемые release assets:

```text
GuitarToolkit_DESKTOP_v.<version>.zip
GuitarToolkit_VST3_v.<version>.zip
```

Проверьте, что VST3 package содержит всю папку плагина и runtime files, а не только `.vst3` файл.

## 6. Ручная Desktop smoke test

Минимально проверить:

- app запускается;
- About/version information корректна;
- Tuner tab открывается;
- input device selection работает;
- tuner получает input;
- metronome запускается и останавливается;
- chord или scale playback работает;
- Tabs page открывается;
- logs folder доступна, если актуально.

## 7. Ручная VST3 smoke test

По возможности проверить хотя бы в одной DAW:

- старые plugin files заменены или cleanly overwritten;
- DAW пересканировала VST3 plugins;
- GuitarToolkit появился в plugin list;
- plugin загружается на track;
- editor открывается;
- input assignment работает, если DAW этого требует;
- tuner input работает;
- metronome/chord/scale playback работает;
- Tabs page открывается, если поддерживается;
- logs не показывают новых startup errors.

DAW, которые стоит постепенно проверять:

- FL Studio;
- Reaper;
- Cubase;
- Ableton Live;
- Studio One;
- Bitwig Studio.

## 8. Создать GitHub Release

Создать GitHub Release с tag:

```text
v<version>
```

Загрузить:

- `GuitarToolkit_DESKTOP_v.<version>.zip`;
- `GuitarToolkit_VST3_v.<version>.zip`.

Описание релиза должно включать:

- короткое summary;
- highlights;
- install notes;
- known issues;
- verification status;
- ссылки на Quick Start и Discussions.

## 9. Шаблон release notes

```md
# GuitarToolkit v<version>

## Highlights

- ...

## Downloads

- `GuitarToolkit_DESKTOP_v.<version>.zip` - standalone Windows desktop app.
- `GuitarToolkit_VST3_v.<version>.zip` - VST3 plugin package.

## Install notes

Для VST3 скопируйте всю папку плагина `GuitarToolkit` сюда:

`C:\Program Files\Common Files\VST3\GuitarToolkit\`

Не копируйте только `.vst3` файл.

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
```

## 10. Проверка после релиза

После публикации:

- скачать оба ZIP из GitHub Release;
- проверить имена архивов;
- проверить, что README links ведут в текущий репозиторий;
- проверить, что Discussions и Issues routes полезны;
- проверить CI на `master`;
- открыть или обновить follow-up issues для известных проблем.
