# Branching and Protection

[Русская версия](#ru)

This document describes the recommended branch workflow and GitHub branch protection settings for GuitarToolkit.

## Goals

Branch protection should help keep `master` stable while still allowing fast personal development.

The main goals are:

- avoid accidental broken commits on `master`;
- make pull requests the default workflow for non-trivial changes;
- require CI before merging risky changes;
- keep releases reproducible;
- make dependency and VST/audio changes easier to review.

## Branches

Recommended branches:

| Branch type | Example | Purpose |
| --- | --- | --- |
| Main branch | `master` | Stable project state and release base. |
| Feature branch | `feature/tabs-search` | New features. |
| Fix branch | `fix/vst3-startup-crash` | Bug fixes. |
| Docs branch | `docs/quick-start-screenshots` | Documentation-only changes. |
| Release branch | `release/v1.6.0` | Release preparation. |
| Dependency branch | `deps/update-naudio` | Manual dependency updates. |

Dependabot will also create its own branches automatically.

## Recommended workflow

For small documentation-only changes, direct commits can be acceptable while the project is solo-maintained.

For code, build, release, audio, VST3, or dependency changes, prefer:

1. Create a branch.
2. Make focused changes.
3. Open a pull request.
4. Wait for CI.
5. Run manual checks if needed.
6. Merge after review.

## Pull request expectations

A pull request should include:

- clear summary;
- verification steps;
- screenshots when UI changes;
- DAW/version notes when VST3 behavior changes;
- changelog/docs updates when user-facing behavior changes.

Use `.github/PULL_REQUEST_TEMPLATE.md` as the source of truth.

## Branch protection recommendation

Recommended protection rule for `master`:

```text
Branch name pattern: master
```

Enable:

- Require a pull request before merging.
- Require status checks to pass before merging.
- Require branches to be up to date before merging.
- Require conversation resolution before merging.
- Do not allow force pushes.
- Do not allow deletions.

Optional while the project is solo-maintained:

- Allow administrators to bypass if urgent.
- Require approvals: 0 or 1 depending on whether there are external contributors.

Recommended required status check:

```text
CI / build-and-test
```

The exact status check name may differ depending on how GitHub displays the workflow job.

## Suggested merge strategy

Recommended defaults:

- Use squash merge for most PRs.
- Use regular merge only when preserving branch history matters.
- Avoid rebase merge for release branches unless you intentionally want a linear history.

Suggested commit style:

```text
docs: update quick start guide
ci: update workflow configuration
test: add scale helper tests
fix: handle tuner input edge case
feat: add first-run setup hint
chore: clean repository docs
```

This is a recommendation, not a strict conventional-commits requirement.

## Risk levels

### Low-risk PRs

Usually safe after green CI:

- typo fixes;
- README/docs improvements;
- screenshot updates;
- issue/discussion template updates;
- small test additions.

### Medium-risk PRs

Need closer review:

- build script changes;
- CI changes;
- release package changes;
- dependency updates for test tooling;
- UI layout changes.

### High-risk PRs

Require manual smoke testing:

- audio behavior;
- tuner input detection;
- metronome output;
- VST3 plugin loading;
- DAW editor behavior;
- alphaTab/Tab viewer runtime behavior;
- NAudio or AudioPlugSharp updates.

## Manual setup steps

GitHub UI path:

```text
Repository -> Settings -> Branches -> Add branch protection rule
```

Recommended rule:

```text
Branch name pattern: master
```

Then enable the settings listed in this document.

## When not to merge

Do not merge if:

- CI fails;
- release package output is unclear;
- VST3 runtime files changed without testing;
- audio dependency changed without manual Desktop check;
- DAW-specific behavior changed without at least one host smoke test;
- the PR mixes unrelated changes.

---

<a id="ru"></a>

# Ветки и защита branch

[English version](#branching-and-protection)

Этот документ описывает рекомендуемый workflow веток и настройки GitHub Branch Protection для GuitarToolkit.

## Цели

Branch protection помогает держать `master` стабильным и при этом не мешать быстрой личной разработке.

Основные цели:

- не допускать случайные broken commits в `master`;
- сделать pull requests основным workflow для нетривиальных изменений;
- требовать CI перед merge рискованных изменений;
- сохранить воспроизводимость релизов;
- упростить review зависимостей, VST и audio changes.

## Ветки

Рекомендуемые ветки:

| Тип ветки | Пример | Назначение |
| --- | --- | --- |
| Main branch | `master` | Стабильное состояние проекта и база релизов. |
| Feature branch | `feature/tabs-search` | Новые функции. |
| Fix branch | `fix/vst3-startup-crash` | Исправления багов. |
| Docs branch | `docs/quick-start-screenshots` | Изменения только документации. |
| Release branch | `release/v1.6.0` | Подготовка релиза. |
| Dependency branch | `deps/update-naudio` | Ручные обновления зависимостей. |

Dependabot также автоматически создаёт свои ветки.

## Рекомендуемый workflow

Для маленьких documentation-only изменений прямые коммиты допустимы, пока проект ведётся одним мейнтейнером.

Для code, build, release, audio, VST3 или dependency changes лучше:

1. Создать branch.
2. Сделать focused changes.
3. Открыть pull request.
4. Дождаться CI.
5. Провести manual checks, если нужно.
6. Merge после review.

## Ожидания от pull request

Pull request должен включать:

- понятный summary;
- verification steps;
- screenshots при UI changes;
- DAW/version notes при изменениях VST3 behavior;
- changelog/docs updates, если меняется user-facing behavior.

`.github/PULL_REQUEST_TEMPLATE.md` — основной источник чеклиста.

## Рекомендованная защита branch

Рекомендуемое правило для `master`:

```text
Branch name pattern: master
```

Включить:

- Require a pull request before merging.
- Require status checks to pass before merging.
- Require branches to be up to date before merging.
- Require conversation resolution before merging.
- Do not allow force pushes.
- Do not allow deletions.

Опционально, пока проект ведётся одним человеком:

- Allow administrators to bypass if urgent.
- Require approvals: 0 или 1 в зависимости от наличия внешних contributors.

Рекомендуемый required status check:

```text
CI / build-and-test
```

Точное имя status check может отличаться в GitHub UI.

## Стратегия merge

Рекомендуемые defaults:

- Squash merge для большинства PR.
- Regular merge только если важно сохранить историю branch.
- Rebase merge лучше не использовать для release branches, если только специально не нужна линейная история.

Рекомендуемый commit style:

```text
docs: update quick start guide
ci: update workflow configuration
test: add scale helper tests
fix: handle tuner input edge case
feat: add first-run setup hint
chore: clean repository docs
```

Это рекомендация, а не жёсткое требование conventional commits.

## Уровни риска

### Low-risk PRs

Обычно можно принимать после зелёного CI:

- typo fixes;
- README/docs improvements;
- screenshot updates;
- issue/discussion template updates;
- small test additions.

### Medium-risk PRs

Нужен более внимательный review:

- build script changes;
- CI changes;
- release package changes;
- dependency updates for test tooling;
- UI layout changes.

### High-risk PRs

Нужны manual smoke tests:

- audio behavior;
- tuner input detection;
- metronome output;
- VST3 plugin loading;
- DAW editor behavior;
- alphaTab/Tab viewer runtime behavior;
- NAudio или AudioPlugSharp updates.

## Ручная настройка

Путь в GitHub UI:

```text
Repository -> Settings -> Branches -> Add branch protection rule
```

Рекомендуемое правило:

```text
Branch name pattern: master
```

Дальше включить настройки из этого документа.

## Когда не мержить

Не мержить, если:

- CI fails;
- release package output unclear;
- VST3 runtime files changed without testing;
- audio dependency changed without manual Desktop check;
- DAW-specific behavior changed without at least one host smoke test;
- PR mixes unrelated changes.
