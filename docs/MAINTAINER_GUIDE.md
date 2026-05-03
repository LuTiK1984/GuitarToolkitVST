# Maintainer Guide

[Русская версия](#ru)

This guide describes how to maintain GuitarToolkit as an open-source project: how to handle Issues, Discussions, releases, labels, and routine repository hygiene.

## Maintainer goals

The maintainer should keep the project:

- useful for musicians;
- stable for Desktop and VST3 users;
- easy to install;
- easy to report problems to;
- safe for new contributors;
- clean and readable in GitHub.

## Issues vs Discussions

Use GitHub Discussions for open-ended conversations:

- setup questions;
- installation help;
- DAW compatibility reports;
- feature ideas;
- workflow feedback;
- community screenshots and demos.

Use GitHub Issues for actionable work:

- reproducible bugs;
- scoped features;
- documentation tasks;
- release tasks;
- cleanup tasks;
- confirmed DAW compatibility problems.

If a Discussion becomes actionable, create or link an Issue.

If an Issue is actually a support question, answer briefly and redirect to Discussions.

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

Recommended labels:

| Label | Use for |
| --- | --- |
| `bug` | Reproducible defects. |
| `enhancement` | Feature requests and improvements. |
| `documentation` | README/docs/release notes changes. |
| `task` | Maintenance or project-management tasks. |
| `good first issue` | Small, safe tasks for new contributors. |
| `help wanted` | Tasks where external help is useful. |
| `dependencies` | Dependency and tooling updates. |
| `desktop` | Desktop app behavior. |
| `vst3` | VST3 plugin behavior. |
| `daw` | DAW compatibility. |
| `audio` | Audio input/output, tuner, metronome, playback. |
| `tabs` | Guitar Pro/MusicXML/alphaTab behavior. |
| `ui` | Layout, themes, controls, visual behavior. |
| `needs-repro` | Missing reproduction steps. |
| `needs-testing` | Needs manual verification. |
| `blocked` | Cannot proceed until another task is done. |
| `wontfix` | Intentionally not planned. |

## Milestones

Use milestones for release-sized groups of work.

Examples:

```text
v1.6.0 - Repository polish, release hygiene, DAW compatibility tracking
v1.7.0 - UI polish and first-run improvements
v2.0.0 - Larger product milestone
```

A milestone should contain only work that is realistic for that release.

Do not put every idea into the next milestone. Keep the roadmap separate from committed release scope.

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

Confirmed results should update `docs/SUPPORTED_DAWS.md`.

## Closing issues

Close an Issue when:

- the fix was merged;
- the documentation was updated;
- the report is a duplicate;
- the report has no reproduction details after follow-up;
- the request is outside project scope;
- the discussion belongs in GitHub Discussions instead.

When closing, leave a short reason.

## Discussions routine

Recommended categories:

- Announcements;
- Q&A;
- Ideas;
- DAW Compatibility;
- Show and Tell;
- General.

Pin one welcome post that explains where to ask questions, where to report bugs, and what information is needed for VST3/DAW problems.

Use Discussions as a source of future Issues, not as an unmanaged chat dump.

## Release routine

Before a release:

1. Review open milestone Issues.
2. Update `CHANGELOG.md`.
3. Follow `docs/RELEASE_PROCESS.md`.
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

Prefer:

- clear answers;
- polite but firm boundaries;
- asking for reproduction details;
- closing stale or unsupported requests with a reason;
- keeping the project musician-first.

Avoid:

- promising urgent support;
- accepting vague bugs without reproduction steps;
- merging risky changes just because CI passed;
- letting Discussions become the only place where important decisions are recorded.

---

<a id="ru"></a>

# Руководство мейнтейнера

[English version](#maintainer-guide)

Это руководство описывает, как вести GuitarToolkit как open-source проект: как работать с Issues, Discussions, релизами, labels, milestones и чистотой репозитория.

## Цели мейнтейнера

Мейнтейнер должен сохранять проект:

- полезным для музыкантов;
- стабильным для Desktop и VST3 пользователей;
- простым в установке;
- удобным для сообщений о проблемах;
- безопасным для новых контрибьюторов;
- чистым и понятным на GitHub.

## Issues vs Discussions

Используйте GitHub Discussions для открытых обсуждений:

- вопросы настройки;
- помощь с установкой;
- отчёты о совместимости DAW;
- идеи функций;
- feedback по workflow;
- скриншоты и демо от сообщества.

Используйте GitHub Issues для задач, по которым можно действовать:

- воспроизводимые баги;
- ограниченные по scope функции;
- задачи по документации;
- релизные задачи;
- задачи чистки;
- подтверждённые проблемы совместимости DAW.

Если Discussion становится actionable, создайте или привяжите Issue.

Если Issue на самом деле является вопросом поддержки, кратко ответьте и направьте в Discussions.

## Triage workflow

Когда появляется новая Issue:

1. Проверить, можно ли по ней действовать.
2. Проверить, использован ли правильный template.
3. Попросить недостающие шаги воспроизведения, если нужно.
4. Добавить labels.
5. Назначить milestone, если задача относится к планируемому релизу.
6. Закрыть дубликаты со ссылкой на существующую Issue.
7. Перенести общие вопросы поддержки в Discussions.

## Рекомендуемые labels

| Label | Для чего |
| --- | --- |
| `bug` | Воспроизводимые дефекты. |
| `enhancement` | Feature requests и улучшения. |
| `documentation` | README/docs/release notes. |
| `task` | Maintenance или project-management задачи. |
| `good first issue` | Маленькие безопасные задачи для новых контрибьюторов. |
| `help wanted` | Задачи, где полезна внешняя помощь. |
| `dependencies` | Обновления зависимостей и tooling. |
| `desktop` | Поведение Desktop app. |
| `vst3` | Поведение VST3 plugin. |
| `daw` | Совместимость с DAW. |
| `audio` | Audio input/output, tuner, metronome, playback. |
| `tabs` | Guitar Pro/MusicXML/alphaTab behavior. |
| `ui` | Layout, themes, controls, visual behavior. |
| `needs-repro` | Не хватает шагов воспроизведения. |
| `needs-testing` | Нужна ручная проверка. |
| `blocked` | Нельзя продолжить до другой задачи. |
| `wontfix` | Осознанно не планируется. |

## Milestones

Используйте milestones для группы задач одного релиза.

Примеры:

```text
v1.6.0 - Repository polish, release hygiene, DAW compatibility tracking
v1.7.0 - UI polish and first-run improvements
v2.0.0 - Larger product milestone
```

Milestone должен содержать только то, что реально сделать в этом релизе.

Не складывайте все идеи в следующий milestone. Roadmap и committed release scope — разные вещи.

## Good first issues

Good first issues должны быть:

- маленькими;
- низкорисковыми;
- хорошо описанными;
- возможными без глубокого знания VST/audio;
- безопасными для review.

Хорошие примеры:

- улучшения документации;
- скриншоты;
- маленькие тесты в `GuitarToolkit.Tests`;
- исправления опечаток;
- сбор DAW compatibility reports;
- небольшие обновления README или Quick Start.

Не помечайте сложные audio, VST3, threading или DAW-host проблемы как good first issue.

## Работа с DAW compatibility reports

Для каждого DAW compatibility report собирайте:

- название и версию DAW;
- версию GuitarToolkit;
- версию Windows;
- audio device/interface;
- загружается ли plugin;
- открывается ли editor;
- работает ли tuner input;
- работает ли metronome/chord/scale playback;
- работает ли Tabs;
- logs или screenshots, если есть.

Подтверждённые результаты должны обновлять `docs/SUPPORTED_DAWS.md`.

## Закрытие issues

Закрывайте Issue, когда:

- fix merged;
- documentation updated;
- report является duplicate;
- нет reproduction details после follow-up;
- request вне scope проекта;
- обсуждение должно быть в GitHub Discussions.

При закрытии оставляйте короткую причину.

## Discussions routine

Рекомендуемые категории:

- Announcements;
- Q&A;
- Ideas;
- DAW Compatibility;
- Show and Tell;
- General.

Закрепите один welcome post, который объясняет, где задавать вопросы, где сообщать баги и какие данные нужны для VST3/DAW проблем.

Используйте Discussions как источник будущих Issues, а не как неуправляемый чат.

## Release routine

Перед релизом:

1. Проверить open milestone Issues.
2. Обновить `CHANGELOG.md`.
3. Следовать `docs/RELEASE_PROCESS.md`.
4. Запустить build и tests.
5. Провести manual Desktop smoke test.
6. По возможности провести manual VST3 smoke test.
7. Опубликовать GitHub Release с понятными notes и assets.
8. Создать follow-up Issues для известных проблем.

## Repository hygiene routine

Периодически проверяйте:

- release ZIPs не закоммичены;
- `bin/`, `obj/`, logs и test output не закоммичены;
- старая документация не дублируется;
- README links работают;
- Discussions и Issues templates всё ещё актуальны;
- `.gitignore` покрывает новые generated outputs;
- GitHub Releases содержат downloadable assets, вместо хранения архивов в репозитории.

## Тон мейнтейнера

Лучше:

- отвечать понятно;
- держать вежливые, но твёрдые границы;
- просить reproduction details;
- закрывать stale или unsupported requests с причиной;
- держать проект musician-first.

Избегайте:

- обещать срочную поддержку;
- принимать vague bugs без reproduction steps;
- мержить рискованные изменения только потому, что CI passed;
- оставлять важные решения только в Discussions без фиксации в Issues/docs.
