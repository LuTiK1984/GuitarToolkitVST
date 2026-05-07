# Dependency Update Policy

[Русская версия](#ru)

This project uses Dependabot to propose dependency and GitHub Actions updates. Every update must be reviewed before merging, because GuitarToolkit includes audio, UI, and VST3/DAW behavior that cannot be fully verified by unit tests alone.

## Update categories

| Category | Examples | Risk | Required checks |
| --- | --- | --- | --- |
| GitHub Actions | `actions/checkout`, `actions/setup-dotnet` | Low to medium | CI must pass. Check runner/version notes for major updates. |
| Test packages | `xunit`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk` | Low to medium | CI must pass. Confirm test discovery still works. |
| Desktop audio | `NAudio` | Medium to high | CI must pass. Manual Desktop audio smoke test is required. |
| VST/plugin bridge | `AudioPlugSharp`, VST bridge/runtime packages | High | CI must pass. Manual VST3/DAW smoke test is required. |
| Tabs/rendering | `alphaTab`, `AlphaSkia` | Medium to high | CI must pass. Manual Tabs loading/playback test is required. |
| Local ML inference | `Microsoft.ML.OnnxRuntime` | Medium | CI must pass. Manual Ideas tab generation smoke test is required. |
| UI/WPF-related packages | WPF helpers, UI dependencies | Medium | CI must pass. Manual Desktop and VST editor smoke test is recommended. |

## Merge rules

### Safe-ish updates

Minor or patch updates for test-only dependencies can usually be merged when:

- CI is green;
- the PR has no unusual migration notes;
- test discovery still works;
- the change affects only test project files.

### Major updates

Major updates require extra caution even when CI is green. Before merging, check:

- release notes for breaking changes;
- changed package behavior;
- whether the package is used at runtime;
- whether manual testing is needed.

### Audio and VST updates

Do not merge audio or VST-related dependency updates based only on CI.

Manual checks should include:

- Desktop app launches;
- input device selection works;
- tuner receives input;
- metronome produces sound;
- chord/scale playback works;
- VST3 plugin loads in at least one DAW;
- plugin editor opens;
- logs do not show new startup failures.

### Tabs updates

For alphaTab/AlphaSkia or tab-rendering updates, manually check:

- GP3/GP4/GP5/GPX or available sample files;
- MusicXML if available;
- track selection;
- play/pause/stop;
- speed/volume controls;
- resize/maximize/restore behavior.

## Suggested Dependabot workflow

1. Let Dependabot open one PR per dependency.
2. Wait for CI.
3. Rebase if the PR is outdated.
4. Merge low-risk test/documentation/CI updates first.
5. Handle audio, VST3, and tab-rendering updates separately.
6. Update `CHANGELOG.md` when a dependency update changes user-visible behavior or release packaging.
7. If a dependency update breaks build, tests, audio, tabs, or DAW behavior, close or postpone the PR and add a note explaining why.

## Manual smoke test checklist

### Desktop

- Launch `GuitarToolkit.Desktop`.
- Open Tuner.
- Select input device.
- Check detected signal/note behavior.
- Start and stop Metronome.
- Play a chord or scale.
- Open Tabs page if relevant.

### VST3

- Deploy the plugin or use the release output.
- Rescan plugins in a DAW.
- Add GuitarToolkit to a track.
- Open the editor.
- Assign/change input if required by the DAW.
- Check tuner, metronome, chord/scale playback, and Tabs page if relevant.

## When to postpone

Postpone or close a Dependabot PR when:

- CI fails and the fix is not obvious;
- release notes mention breaking changes that need code work;
- audio behavior changes and cannot be tested right now;
- VST3/DAW behavior changes and cannot be tested right now;
- multiple related packages should be upgraded together in a dedicated branch.

---

<a id="ru"></a>

# Политика обновления зависимостей

[English version](#dependency-update-policy)

В проекте используется Dependabot: он предлагает обновления зависимостей и GitHub Actions через pull request. Каждое обновление нужно проверять перед merge, потому что GuitarToolkit содержит аудио, UI и VST3/DAW-поведение, которое нельзя полностью проверить только unit-тестами.

## Категории обновлений

| Категория | Примеры | Риск | Что проверять |
| --- | --- | --- | --- |
| GitHub Actions | `actions/checkout`, `actions/setup-dotnet` | Низкий/средний | CI должен пройти. Для major updates читать runner/version notes. |
| Тестовые пакеты | `xunit`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk` | Низкий/средний | CI должен пройти. Проверить, что тесты обнаруживаются и запускаются. |
| Desktop-аудио | `NAudio` | Средний/высокий | CI должен пройти. Нужна ручная Desktop audio smoke test. |
| VST/plugin bridge | `AudioPlugSharp`, VST bridge/runtime packages | Высокий | CI должен пройти. Нужна ручная VST3/DAW smoke test. |
| Tabs/rendering | `alphaTab`, `AlphaSkia` | Средний/высокий | CI должен пройти. Нужна ручная проверка загрузки и playback табов. |
| Local ML inference | `Microsoft.ML.OnnxRuntime` | Средний | CI должен пройти. Нужна ручная проверка генерации во вкладке "Идеи". |
| UI/WPF-зависимости | WPF helpers, UI dependencies | Средний | CI должен пройти. Желательна ручная проверка Desktop и VST editor. |

## Правила merge

### Относительно безопасные обновления

Minor или patch updates для test-only зависимостей обычно можно мержить, если:

- CI зелёный;
- в PR нет подозрительных migration notes;
- test discovery работает;
- изменение затрагивает только test project files.

### Major updates

Major updates требуют осторожности даже при зелёном CI. Перед merge нужно проверить:

- release notes на breaking changes;
- изменилось ли поведение пакета;
- используется ли пакет в runtime;
- нужна ли ручная проверка.

### Audio и VST updates

Audio или VST-related обновления нельзя мержить только по зелёному CI.

Ручная проверка должна включать:

- Desktop app запускается;
- выбор input device работает;
- tuner получает input;
- metronome звучит;
- chord/scale playback работает;
- VST3 plugin загружается хотя бы в одной DAW;
- plugin editor открывается;
- в логах нет новых startup failures.

### Tabs updates

Для alphaTab/AlphaSkia или tab-rendering updates вручную проверить:

- GP3/GP4/GP5/GPX или доступные sample files;
- MusicXML, если есть;
- выбор дорожек;
- play/pause/stop;
- speed/volume controls;
- resize/maximize/restore behavior.

## Рекомендуемый workflow Dependabot

1. Dependabot открывает отдельный PR на каждую зависимость.
2. Ждём CI.
3. Делаем rebase, если PR устарел.
4. Сначала мержим low-risk updates: тесты, документация, CI.
5. Audio, VST3 и tab-rendering updates проверяем отдельно.
6. Обновляем `CHANGELOG.md`, если обновление меняет пользовательское поведение или release packaging.
7. Если обновление ломает build, tests, audio, tabs или DAW behavior — закрываем или откладываем PR с коротким объяснением.

## Чеклист ручной проверки

### Desktop

- Запустить `GuitarToolkit.Desktop`.
- Открыть Tuner.
- Выбрать input device.
- Проверить detected signal/note behavior.
- Запустить и остановить Metronome.
- Воспроизвести chord или scale.
- Открыть Tabs page, если актуально.

### VST3

- Задеплоить plugin или использовать release output.
- Пересканировать плагины в DAW.
- Добавить GuitarToolkit на track.
- Открыть editor.
- Назначить/сменить input, если DAW этого требует.
- Проверить tuner, metronome, chord/scale playback и Tabs page, если актуально.

## Когда откладывать

Dependabot PR лучше отложить или закрыть, если:

- CI падает, а исправление неочевидно;
- release notes говорят о breaking changes, требующих правок кода;
- меняется audio behavior, а сейчас нет возможности проверить звук;
- меняется VST3/DAW behavior, а сейчас нет возможности проверить DAW;
- несколько связанных пакетов лучше обновлять вместе в отдельной ветке.
