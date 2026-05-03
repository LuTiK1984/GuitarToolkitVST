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

A good issue usually has:

- one type label;
- one to three area labels;
- zero or one status label;
- optional contributor label.

Use status labels only while they are true. Remove them when the issue is unblocked, reproduced, clarified, or tested.

## Good first issue rules

Use `good first issue` only when the task is small, clear, low-risk, possible without deep audio/VST3/DAW knowledge, and easy to review.

Good examples:

- docs improvement;
- screenshot update;
- typo fix;
- small unit test;
- adding a DAW compatibility report to docs.

Avoid marking complex audio callback, VST3 loading, threading, DAW-host edge cases, large UI redesigns, or runtime dependency updates as `good first issue`.

## Milestone workflow

Use milestones for real release scope, not for every idea.

Recommended active milestones:

```text
v1.6.0 - Repository polish, release hygiene, DAW compatibility tracking
v1.7.0 - First-run experience, screenshots, and installation polish
v2.0.0 - Larger product milestone, if needed later
```

## Suggested v1.6.0 scope

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

## Suggested v1.7.0 scope

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

---

<a id="ru"></a>

# Labels и Milestones

[English version](#labels-and-milestones)

Этот документ задаёт рекомендуемые GitHub labels и milestone workflow для GuitarToolkit.

## Цели labels

Labels должны помогать быстро понять тип задачи, затронутую область, готовность к работе, необходимость проверки и пригодность для новичков.

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

Используйте `desktop`, `vst3`, `daw`, `audio`, `tabs`, `tuner`, `metronome`, `chords`, `scales`, `progressions`, `ui`, `ci`, `release`.

### Status labels

Используйте `needs-repro`, `needs-info`, `needs-testing`, `blocked`, `duplicate`, `wontfix`, `not planned`.

### Contributor labels

Используйте `good first issue` и `help wanted` только когда задача действительно подходит.

## Правила labels

Обычно достаточно одного type label, одного-трёх area labels, нуля или одного status label и optional contributor label.

## Good first issue

Хорошие первые задачи: docs, screenshots, typo fixes, small tests, DAW reports. Не помечайте сложные audio/VST3/threading/DAW issues как good first issue.

## Milestones

Рекомендуемые milestones:

```text
v1.6.0 - Repository polish, release hygiene, DAW compatibility tracking
v1.7.0 - First-run experience, screenshots, and installation polish
v2.0.0 - Larger product milestone, if needed later
```

Milestones должны отражать реальный release scope, а не весь roadmap.
