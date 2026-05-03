# Labels and Milestones

[Русская версия](#ru)

This document defines the recommended GitHub labels and milestone workflow for GuitarToolkit.

## Label goals

Labels should make it easy to understand:

- what kind of work an issue is;
- which project area it affects;
- whether it needs more information;
- whether it is safe for newcomers;
- whether it belongs to Desktop, VST3, DAW, audio, UI, tabs, or documentation.

## Recommended labels

### Type labels

| Label | Color | Description |
| --- | --- | --- |
| `bug` | `#d73a4a` | Something is broken or behaves incorrectly. |
| `enhancement` | `#a2eeef` | New feature or improvement. |
| `documentation` | `#0075ca` | README, docs, screenshots, release notes, or guides. |
| `task` | `#ededed` | Maintenance, cleanup, release, or project-management task. |
| `dependencies` | `#0366d6` | Dependency, package, or GitHub Actions update. |

### Area labels

| Label | Color | Description |
| --- | --- | --- |
| `desktop` | `#5319e7` | Standalone desktop app. |
| `vst3` | `#5319e7` | VST3 plugin target. |
| `daw` | `#5319e7` | DAW compatibility or host-specific behavior. |
| `audio` | `#fbca04` | Audio input/output, tuner, metronome, playback. |
| `tabs` | `#fbca04` | Guitar Pro, MusicXML, alphaTab, tab viewer behavior. |
| `tuner` | `#c5def5` | Tuner-specific behavior. |
| `metronome` | `#c5def5` | Metronome-specific behavior. |
| `chords` | `#c5def5` | Chord library or chord playback. |
| `scales` | `#c5def5` | Scales, fretboard, modes, theory display. |
| `progressions` | `#c5def5` | Progression builder and presets. |
| `ui` | `#c5def5` | Layout, theme, controls, visual polish. |
| `ci` | `#0e8a16` | GitHub Actions, build, test pipeline. |
| `release` | `#0e8a16` | Release preparation, packaging, versioning. |

### Status labels

| Label | Color | Description |
| --- | --- | --- |
| `needs-repro` | `#fef2c0` | Needs reproduction steps or clearer evidence. |
| `needs-info` | `#fef2c0` | Needs more details from reporter. |
| `needs-testing` | `#fef2c0` | Needs manual verification. |
| `blocked` | `#b60205` | Blocked by another issue, decision, or missing test access. |
| `duplicate` | `#cfd3d7` | Duplicate of another issue. |
| `wontfix` | `#ffffff` | Intentionally not planned. |
| `not planned` | `#ffffff` | Outside current project scope or not planned now. |

### Contributor labels

| Label | Color | Description |
| --- | --- | --- |
| `good first issue` | `#7057ff` | Good for newcomers. Small, clear, low-risk. |
| `help wanted` | `#008672` | External help is welcome. |

## Labeling rules

Use at least one type label:

- `bug`;
- `enhancement`;
- `documentation`;
- `task`;
- `dependencies`.

Use area labels when the issue touches a specific module or target.

Use status labels only while the status is true. Remove them when the issue is unblocked, reproduced, clarified, or tested.

Do not over-label. A good issue usually has:

- one type label;
- one to three area labels;
- zero or one status label;
- optional contributor label.

## Good first issue rules

Use `good first issue` only when the task is:

- small;
- clear;
- low-risk;
- possible without deep audio/VST3/DAW knowledge;
- easy to review.

Good examples:

- docs improvement;
- screenshot update;
- typo fix;
- small unit test;
- adding a DAW compatibility report to docs.

Avoid marking these as `good first issue`:

- audio callback changes;
- VST3 plugin loading problems;
- threading issues;
- DAW-host edge cases;
- large UI redesigns;
- dependency updates that affect runtime behavior.

## Milestone workflow

Use milestones for real release scope, not for every idea.

Recommended active milestones:

```text
v1.6.0 - Repository polish, release hygiene, DAW compatibility tracking
v1.7.0 - First-run experience, screenshots, and installation polish
v2.0.0 - Larger product milestone, if needed later
```

## Suggested v1.6.0 scope

Use `v1.6.0` for repository and project-management polish:

- README polish;
- Discussions setup;
- support/docs cleanup;
- release process docs;
- dependency policy;
- maintainer guide;
- project structure guide;
- repository presentation setup;
- labels and milestones;
- DAW compatibility tracking foundation.

Suggested issues:

- first-run guidance;
- DAW compatibility reports;
- release notes polish;
- repository screenshots;
- labels/milestones setup.

## Suggested v1.7.0 scope

Use `v1.7.0` for user-facing install and first-run improvements:

- better installation screenshots;
- better VST3 install flow;
- clearer logs access;
- About/version polish;
- first-run hints;
- DAW docs improvements;
- common troubleshooting screenshots.

## Milestone hygiene

Before each release:

1. Review all issues in the milestone.
2. Move unfinished non-blocking work to the next milestone.
3. Close duplicate or stale issues.
4. Make sure release notes reflect completed work.
5. Do not release with unclear critical issues hidden in the milestone.

## Manual GitHub setup steps

To configure labels manually:

1. Open the repository on GitHub.
2. Go to **Issues**.
3. Open **Labels**.
4. Edit existing labels or create new ones using the tables above.
5. Remove labels that do not match the project workflow if they create noise.

To configure milestones manually:

1. Open **Issues**.
2. Open **Milestones**.
3. Create `v1.6.0` and `v1.7.0`.
4. Add realistic descriptions.
5. Assign only issues that actually belong to that release.

---

<a id="ru"></a>

# Labels и Milestones

[English version](#labels-and-milestones)

Этот документ задаёт рекомендуемые GitHub labels и milestone workflow для GuitarToolkit.

## Цели labels

Labels должны помогать быстро понять:

- какой это тип работы;
- какую область проекта затрагивает задача;
- нужна ли дополнительная информация;
- подходит ли задача новичку;
- относится ли она к Desktop, VST3, DAW, audio, UI, tabs или документации.

## Рекомендуемые labels

### Type labels

| Label | Color | Description |
| --- | --- | --- |
| `bug` | `#d73a4a` | Что-то сломано или работает неправильно. |
| `enhancement` | `#a2eeef` | Новая функция или улучшение. |
| `documentation` | `#0075ca` | README, docs, screenshots, release notes или guides. |
| `task` | `#ededed` | Maintenance, cleanup, release или project-management task. |
| `dependencies` | `#0366d6` | Dependency, package или GitHub Actions update. |

### Area labels

| Label | Color | Description |
| --- | --- | --- |
| `desktop` | `#5319e7` | Standalone desktop app. |
| `vst3` | `#5319e7` | VST3 plugin target. |
| `daw` | `#5319e7` | DAW compatibility или host-specific behavior. |
| `audio` | `#fbca04` | Audio input/output, tuner, metronome, playback. |
| `tabs` | `#fbca04` | Guitar Pro, MusicXML, alphaTab, tab viewer behavior. |
| `tuner` | `#c5def5` | Tuner-specific behavior. |
| `metronome` | `#c5def5` | Metronome-specific behavior. |
| `chords` | `#c5def5` | Chord library или chord playback. |
| `scales` | `#c5def5` | Scales, fretboard, modes, theory display. |
| `progressions` | `#c5def5` | Progression builder and presets. |
| `ui` | `#c5def5` | Layout, theme, controls, visual polish. |
| `ci` | `#0e8a16` | GitHub Actions, build, test pipeline. |
| `release` | `#0e8a16` | Release preparation, packaging, versioning. |

### Status labels

| Label | Color | Description |
| --- | --- | --- |
| `needs-repro` | `#fef2c0` | Нужны steps to reproduce или доказательства. |
| `needs-info` | `#fef2c0` | Нужны дополнительные детали от автора. |
| `needs-testing` | `#fef2c0` | Нужна ручная проверка. |
| `blocked` | `#b60205` | Заблокировано другой задачей, решением или отсутствием доступа для теста. |
| `duplicate` | `#cfd3d7` | Дубликат другой issue. |
| `wontfix` | `#ffffff` | Осознанно не планируется. |
| `not planned` | `#ffffff` | Вне текущего scope или пока не планируется. |

### Contributor labels

| Label | Color | Description |
| --- | --- | --- |
| `good first issue` | `#7057ff` | Хорошо для новичков: маленькая, понятная, низкорисковая задача. |
| `help wanted` | `#008672` | Внешняя помощь приветствуется. |

## Правила labels

Используйте хотя бы один type label:

- `bug`;
- `enhancement`;
- `documentation`;
- `task`;
- `dependencies`.

Area labels добавляйте, когда issue затрагивает конкретный модуль или target.

Status labels используйте только пока статус актуален. Убирайте их, когда issue разблокирована, воспроизведена, уточнена или протестирована.

Не перегружайте issue labels. Обычно достаточно:

- один type label;
- от одного до трёх area labels;
- ноль или один status label;
- optional contributor label.

## Правила good first issue

Используйте `good first issue` только если задача:

- маленькая;
- понятная;
- низкорисковая;
- возможна без глубокого знания audio/VST3/DAW;
- легко review-ится.

Хорошие примеры:

- улучшение docs;
- обновление screenshots;
- исправление typo;
- маленький unit test;
- добавление DAW compatibility report в docs.

Не помечайте как `good first issue`:

- audio callback changes;
- проблемы загрузки VST3 plugin;
- threading issues;
- DAW-host edge cases;
- большие UI redesigns;
- dependency updates, влияющие на runtime behavior.

## Milestone workflow

Используйте milestones для реального release scope, а не для каждой идеи.

Рекомендуемые активные milestones:

```text
v1.6.0 - Repository polish, release hygiene, DAW compatibility tracking
v1.7.0 - First-run experience, screenshots, and installation polish
v2.0.0 - Larger product milestone, if needed later
```

## Suggested v1.6.0 scope

Используйте `v1.6.0` для repository и project-management polish:

- README polish;
- Discussions setup;
- support/docs cleanup;
- release process docs;
- dependency policy;
- maintainer guide;
- project structure guide;
- repository presentation setup;
- labels and milestones;
- DAW compatibility tracking foundation.

Suggested issues:

- first-run guidance;
- DAW compatibility reports;
- release notes polish;
- repository screenshots;
- labels/milestones setup.

## Suggested v1.7.0 scope

Используйте `v1.7.0` для user-facing install и first-run improvements:

- better installation screenshots;
- better VST3 install flow;
- clearer logs access;
- About/version polish;
- first-run hints;
- DAW docs improvements;
- common troubleshooting screenshots.

## Milestone hygiene

Перед каждым релизом:

1. Проверить все issues в milestone.
2. Перенести незавершённую non-blocking работу в следующий milestone.
3. Закрыть duplicate или stale issues.
4. Убедиться, что release notes отражают completed work.
5. Не выпускать релиз, если в milestone спрятаны unclear critical issues.

## Ручная настройка GitHub

Чтобы настроить labels вручную:

1. Откройте repository на GitHub.
2. Перейдите в **Issues**.
3. Откройте **Labels**.
4. Отредактируйте существующие labels или создайте новые по таблицам выше.
5. Удалите labels, которые создают шум и не подходят workflow проекта.

Чтобы настроить milestones вручную:

1. Откройте **Issues**.
2. Откройте **Milestones**.
3. Создайте `v1.6.0` и `v1.7.0`.
4. Добавьте понятные descriptions.
5. Назначайте только те issues, которые реально относятся к релизу.
