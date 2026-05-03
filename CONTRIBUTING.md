# Contributing to GuitarToolkit

[Русская версия](#ru)

Thanks for helping improve GuitarToolkit. This project is a Windows guitar toolkit shipped as both a standalone WPF app and a VST3 plugin, so stability, focused changes, and clear reproduction steps matter a lot.

## Project Layout

- `GuitarToolkit.Core` contains DSP, theory models, and services. Keep it free of WPF, NAudio, and AudioPlugSharp dependencies.
- `GuitarToolkit.UI` contains shared WPF controls used by both Desktop and VST3.
- `GuitarToolkit.Desktop` is the standalone Windows app and uses NAudio.
- `GuitarToolkit.Plugin` is the VST3 entry point and uses AudioPlugSharp.
- `GuitarToolkit.Tests` contains xUnit tests for Core behavior.

For detailed file-placement rules, see the [project structure guide](docs/PROJECT_STRUCTURE.md).

## Development Setup

Requirements:

- Windows 10/11 x64.
- .NET 8 SDK.
- Visual Studio 2022 or another editor that can build WPF projects.

Restore, build, and test:

```powershell
dotnet restore GuitarToolkit.sln
dotnet build GuitarToolkit.sln --no-restore --configuration Debug
dotnet test GuitarToolkit.sln --no-restore --configuration Debug
```

For VST-related changes, build and test as `x64`.

## Pull Request Checklist

- Keep the change focused and scoped.
- Update `CHANGELOG.md` for user-visible behavior changes.
- Update README or docs when setup, release packaging, screenshots, or DAW behavior changes.
- Put new long-form documentation under `docs/` unless it belongs in a standard root-level community file.
- Run the Debug build and test commands above when code changes.
- For VST changes, manually test in a DAW when possible.
- Do not add blocking operations, file I/O, locks, or frequent logging in the audio callback.

## Dependency Updates

Dependency updates are reviewed with extra care because GuitarToolkit includes audio, UI, and VST3 behavior that CI cannot fully verify.

Use the [dependency update policy](docs/DEPENDENCY_POLICY.md) when reviewing Dependabot PRs or manually updating NuGet/GitHub Actions dependencies.

In short:

- test-only updates usually need green CI;
- GitHub Actions major updates need green CI and runner/version note review;
- audio, VST3, and tab-rendering updates need manual smoke testing in addition to CI.

## DAW and VST Changes

DAW hosts can be sensitive to plugin metadata, port layout, editor startup, and runtime files. For VST changes, please include:

- DAW name and version.
- Windows version.
- Audio interface or input device when relevant.
- Whether the plugin was freshly scanned after deployment.
- Logs from `%AppData%\GuitarToolkit\logs` if available.

Manual VST smoke test:

1. Add GuitarToolkit to a track.
2. Assign or change the recording input after the plugin is loaded.
3. Check tuner input, metronome output, and chord/scale playback.
4. Re-scan or restart the DAW after running `deploy-vst.bat`.

## Issue Reports

Use the issue templates when possible. For bugs, include expected behavior, actual behavior, steps to reproduce, release version or commit, Desktop/VST3 target, logs, screenshots, or sample files when they help.

For Guitar Pro or MusicXML import issues, mention the file format and whether the file can be shared publicly.

---

<a id="ru"></a>

# Участие в разработке GuitarToolkit

[English version](#contributing-to-guitartoolkit)

Спасибо за помощь проекту. GuitarToolkit - это Windows-инструмент для гитаристов, который выходит и как standalone WPF-приложение, и как VST3-плагин. Поэтому особенно важны стабильность, узкие по смыслу изменения и понятные шаги воспроизведения.

## Структура проекта

- `GuitarToolkit.Core` содержит DSP, теорию музыки, модели и сервисы. В нём не должно быть зависимостей от WPF, NAudio и AudioPlugSharp.
- `GuitarToolkit.UI` содержит общие WPF-контролы для Desktop и VST3.
- `GuitarToolkit.Desktop` - standalone-приложение для Windows, использует NAudio.
- `GuitarToolkit.Plugin` - точка входа VST3-плагина, использует AudioPlugSharp.
- `GuitarToolkit.Tests` - xUnit-тесты поведения Core.

Подробные правила размещения файлов описаны в [project structure guide](docs/PROJECT_STRUCTURE.md).

## Подготовка окружения

Требования:

- Windows 10/11 x64.
- .NET 8 SDK.
- Visual Studio 2022 или редактор, который умеет собирать WPF-проекты.

Восстановить зависимости, собрать и протестировать:

```powershell
dotnet restore GuitarToolkit.sln
dotnet build GuitarToolkit.sln --no-restore --configuration Debug
dotnet test GuitarToolkit.sln --no-restore --configuration Debug
```

Для изменений, связанных с VST, собирайте и проверяйте проект как `x64`.

## Чеклист pull request

- Держите изменение сфокусированным.
- Обновляйте `CHANGELOG.md`, если меняется поведение для пользователя.
- Обновляйте README или документацию, если меняется установка, релизная упаковка, скриншоты или поведение в DAW.
- Новую длинную документацию кладите в `docs/`, если это не стандартный root-level community file.
- Запускайте Debug build и тесты после изменений кода.
- Для VST-изменений по возможности проверяйте плагин в DAW.
- Не добавляйте блокирующие операции, файловый ввод/вывод, locks или частое логирование в audio callback.

## Обновления зависимостей

Обновления зависимостей проверяются особенно аккуратно, потому что GuitarToolkit содержит audio, UI и VST3-поведение, которое CI не может полностью проверить.

Используйте [политику обновления зависимостей](docs/DEPENDENCY_POLICY.md) при проверке Dependabot PR или ручном обновлении NuGet/GitHub Actions dependencies.

Кратко:

- test-only updates обычно требуют зелёный CI;
- GitHub Actions major updates требуют зелёный CI и просмотр runner/version notes;
- audio, VST3 и tab-rendering updates требуют ручной smoke test дополнительно к CI.

## Изменения DAW и VST

DAW-хосты чувствительны к метаданным плагина, раскладке портов, запуску редактора и runtime-файлам. Для VST-изменений укажите:

- Название и версию DAW.
- Версию Windows.
- Аудиоинтерфейс или входное устройство, если важно.
- Был ли плагин заново просканирован после деплоя.
- Логи из `%AppData%\GuitarToolkit\logs`, если они есть.

Минимальная ручная проверка VST:

1. Добавить GuitarToolkit на дорожку.
2. Назначить или изменить вход записи после загрузки плагина.
3. Проверить вход тюнера, звук метронома и воспроизведение аккордов/гамм.
4. После `deploy-vst.bat` пересканировать плагины или перезапустить DAW.

## Сообщения об ошибках

По возможности используйте issue templates. Для багов укажите ожидаемое поведение, фактическое поведение, шаги воспроизведения, версию релиза или commit, цель Desktop/VST3, логи, скриншоты или тестовые файлы.

Для проблем импорта Guitar Pro или MusicXML укажите формат файла и можно ли приложить файл публично.
