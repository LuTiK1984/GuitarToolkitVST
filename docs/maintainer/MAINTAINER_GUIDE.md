# Maintainer Guide

[Русская версия](#ru)

This guide describes how to maintain GuitarToolkit as an open-source project.

## Maintainer goals

Keep the project:

- useful for musicians;
- stable for Desktop and VST3 users;
- easy to install;
- easy to report problems to;
- safe for new contributors;
- clean and readable in GitHub.

## Issues vs Discussions

Use GitHub Discussions for:

- setup questions;
- installation help;
- DAW compatibility reports;
- feature ideas;
- workflow feedback;
- screenshots and demos.

Use GitHub Issues for:

- reproducible bugs;
- scoped features;
- documentation tasks;
- release tasks;
- cleanup tasks;
- confirmed DAW compatibility problems.

If a Discussion becomes actionable, create or link an Issue. If an Issue is actually a support question, answer briefly and redirect to Discussions.

## Triage workflow

When a new Issue appears:

1. Check whether it is actionable.
2. Check whether the correct template was used.
3. Ask for missing reproduction details if needed.
4. Add labels.
5. Assign a milestone if it belongs to a planned release.
6. Close duplicates with a link to the existing Issue.
7. Move general support questions to Discussions.

## Suggested labels

Use the detailed label guide: [Labels and Milestones](LABELS_AND_MILESTONES.md).

Common labels:

- `bug`;
- `enhancement`;
- `documentation`;
- `task`;
- `dependencies`;
- `desktop`;
- `vst3`;
- `daw`;
- `audio`;
- `tabs`;
- `ui`;
- `needs-repro`;
- `needs-testing`;
- `good first issue`;
- `help wanted`.

## Milestones

Use milestones for realistic release scope, not for every idea.

Examples:

```text
v1.6.0 - Repository polish, release hygiene, DAW compatibility tracking
v1.7.0 - UI polish and first-run improvements
v2.0.0 - Larger product milestone
```

## Good first issues

Good first issues should be:

- small;
- low-risk;
- well-described;
- possible without deep VST/audio knowledge;
- safe to review.

Good examples:

- documentation improvements;
- screenshots;
- small tests in `GuitarToolkit.Tests`;
- typo fixes;
- DAW compatibility report collection;
- small README or Quick Start updates.

Avoid marking complex audio, VST3, threading, or DAW-host issues as good first issues.

## Handling DAW compatibility reports

For every DAW compatibility report, collect:

- DAW name and version;
- GuitarToolkit version;
- Windows version;
- audio device/interface;
- whether the plugin loads;
- whether the editor opens;
- whether tuner input works;
- whether metronome/chord/scale playback works;
- whether Tabs works;
- logs or screenshots if available.

Confirmed results should update [Supported DAWs](../user/SUPPORTED_DAWS.md).

## Release routine

Before a release:

1. Review open milestone Issues.
2. Update `CHANGELOG.md`.
3. Follow [Release Process](../releases/RELEASE_PROCESS.md).
4. Run build and tests.
5. Run manual Desktop smoke test.
6. Run manual VST3 smoke test when possible.
7. Publish GitHub Release with clear notes and assets.
8. Create follow-up Issues for known problems.

## Repository hygiene routine

Periodically check:

- no release ZIPs are committed;
- no `bin/`, `obj/`, logs, or test output are committed;
- old documentation is not duplicated;
- README links still work;
- Discussions and Issues templates still make sense;
- `.gitignore` covers new generated outputs;
- GitHub Releases contain the downloadable assets instead of storing them in the repository.

## Maintainer tone

Prefer clear answers, polite but firm boundaries, reproduction details, and musician-first priorities.

Avoid promising urgent support, accepting vague bugs without reproduction steps, merging risky changes only because CI passed, or leaving important decisions only in Discussions without docs/issues.

---

<a id="ru"></a>

# Руководство мейнтейнера

[English version](#maintainer-guide)

Этот документ описывает, как вести GuitarToolkit как open-source проект.

## Цели мейнтейнера

Сохранять проект:

- полезным для музыкантов;
- стабильным для Desktop и VST3 пользователей;
- простым в установке;
- удобным для сообщений о проблемах;
- безопасным для новых контрибьюторов;
- чистым и понятным на GitHub.

## Issues vs Discussions

Discussions — для вопросов, установки, DAW reports, идей, feedback и демо.

Issues — для воспроизводимых багов, scoped features, документации, релизов, clean-up и подтверждённых проблем совместимости.

Если Discussion становится actionable — создайте Issue. Если Issue является вопросом поддержки — ответьте и направьте в Discussions.

## Triage workflow

1. Проверить, actionable ли issue.
2. Проверить template.
3. Попросить missing repro details.
4. Добавить labels.
5. Назначить milestone.
6. Закрыть duplicates.
7. Перенести support questions в Discussions.

## Labels и milestones

Подробно: [Labels and Milestones](LABELS_AND_MILESTONES.md).

## Good first issues

Хорошие первые задачи — маленькие, безопасные и понятные: docs, screenshots, typo fixes, small tests, DAW reports.

Не помечайте сложные audio/VST3/threading/DAW-host issues как `good first issue`.

## DAW compatibility reports

Собирайте DAW/version, GuitarToolkit version, Windows version, audio device, plugin load/editor/input/playback/Tabs status, logs/screenshots.

Подтверждённые результаты обновляют [Supported DAWs](../user/SUPPORTED_DAWS.md).

## Release routine

Перед релизом: проверить milestone, обновить changelog, пройти [Release Process](../releases/RELEASE_PROCESS.md), build/test, Desktop smoke test, VST3 smoke test, GitHub Release, follow-up issues.

## Repository hygiene

Периодически проверяйте, что нет release ZIPs, `bin/`, `obj/`, logs, test outputs, дублирующих docs и битых README links.

## Тон мейнтейнера

Отвечайте понятно, просите reproduction details, держите вежливые границы и сохраняйте musician-first направление.
