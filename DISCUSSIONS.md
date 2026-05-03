# GitHub Discussions Guide

[Русская версия](#ru)

Use GitHub Discussions for open-ended project conversations, setup questions, DAW compatibility reports, ideas, and community feedback. Use GitHub Issues only for confirmed bugs or clearly scoped development tasks.

## Recommended Categories

| Category | Slug | Purpose |
| --- | --- | --- |
| Announcements | `announcements` | Release notes, maintainer updates, important project news. |
| Q&A | `q-a` | Installation help, setup questions, build questions, usage questions. |
| Ideas | `ideas` | Feature ideas, workflow improvements, module suggestions. |
| DAW Compatibility | `daw-compatibility` | Reports about VST3 behavior in FL Studio, Reaper, Cubase, Ableton Live, and other DAWs. |
| Show and Tell | `show-and-tell` | Screenshots, workflows, practice setups, demos, and community examples. |
| General | `general` | Anything useful that does not fit the categories above. |

## Suggested Setup

1. Open repository settings.
2. Go to **Settings -> Features**.
3. Enable **Discussions**.
4. Open the **Discussions** tab.
5. Create or verify the categories listed above.
6. Make sure the category slugs match the YAML files in `.github/DISCUSSION_TEMPLATE/`.
7. Pin the welcome post from the section below.

## Suggested Pinned Welcome Post

```md
# Welcome to GuitarToolkit Discussions

GuitarToolkit is an open-source Windows guitar toolkit available as a standalone desktop app and a VST3 plugin.

Use this space for:

- setup and installation questions;
- VST3 and DAW compatibility reports;
- feature ideas and workflow feedback;
- screenshots, demos, and usage examples;
- general project discussion.

Please use GitHub Issues only for reproducible bugs or clearly scoped development tasks.

When asking for help with the VST3 plugin, include your DAW name and version, Windows version, GuitarToolkit version, audio setup, and steps you tested.
```

## Maintainer Notes

- Convert repeated questions into README or docs improvements.
- Move confirmed bugs from Discussions into Issues.
- Label beginner-friendly tasks as `good first issue` after a discussion becomes actionable.
- Keep Announcements focused and low-noise.
- Use DAW compatibility reports to update `docs/SUPPORTED_DAWS.md`.

---

<a id="ru"></a>

# Руководство по GitHub Discussions

[English version](#github-discussions-guide)

Используйте GitHub Discussions для открытых обсуждений проекта, вопросов по установке, отчётов о совместимости с DAW, идей и обратной связи. GitHub Issues лучше оставить для подтверждённых багов и чётко сформулированных задач разработки.

## Рекомендуемые категории

| Категория | Slug | Назначение |
| --- | --- | --- |
| Announcements | `announcements` | Релизы, новости от мейнтейнера, важные объявления проекта. |
| Q&A | `q-a` | Помощь с установкой, сборкой, настройкой и использованием. |
| Ideas | `ideas` | Идеи функций, улучшения workflow, предложения по новым модулям. |
| DAW Compatibility | `daw-compatibility` | Отчёты о поведении VST3 в FL Studio, Reaper, Cubase, Ableton Live и других DAW. |
| Show and Tell | `show-and-tell` | Скриншоты, рабочие процессы, демонстрации и примеры использования. |
| General | `general` | Всё полезное, что не подходит под категории выше. |

## Настройка

1. Откройте настройки репозитория.
2. Перейдите в **Settings -> Features**.
3. Включите **Discussions**.
4. Откройте вкладку **Discussions**.
5. Создайте или проверьте категории из таблицы выше.
6. Убедитесь, что slug категорий совпадает с YAML-файлами в `.github/DISCUSSION_TEMPLATE/`.
7. Закрепите приветственный пост из раздела ниже.

## Текст для закреплённого приветственного поста

```md
# Welcome to GuitarToolkit Discussions

GuitarToolkit — open-source набор инструментов для гитаристов под Windows, доступный как standalone desktop app и как VST3 plugin.

Здесь можно обсуждать:

- вопросы установки и настройки;
- совместимость VST3 с DAW;
- идеи функций и улучшения workflow;
- скриншоты, демо и примеры использования;
- общие вопросы по проекту.

Пожалуйста, используйте GitHub Issues только для воспроизводимых багов и чётких задач разработки.

Если вопрос связан с VST3, укажите DAW и её версию, версию Windows, версию GuitarToolkit, аудионастройки и проверенные шаги.
```

## Заметки для мейнтейнера

- Повторяющиеся вопросы переносить в README или отдельную документацию.
- Подтверждённые баги переносить из Discussions в Issues.
- Простые задачи после обсуждения помечать как `good first issue`.
- Announcements держать чистыми и без лишнего шума.
- Отчёты о DAW использовать для обновления `docs/SUPPORTED_DAWS.md`.
