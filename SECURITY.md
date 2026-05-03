# Security Policy

[Русская версия](#ru)

GuitarToolkit is a local Windows desktop/VST3 tool. It does not provide a network service and does not intentionally process secrets, but security and crash reports are still welcome.

## Supported Versions

Only the latest public release is actively supported for security fixes.

| Version | Supported |
| --- | --- |
| Latest release | Yes |
| Older releases | Best effort |

## Reporting a Vulnerability

If the report contains sensitive details, use GitHub private vulnerability reporting if it is enabled for the repository. If private reporting is not available, open a GitHub issue with a high-level description and avoid posting exploit details publicly.

Please include:

- GuitarToolkit version.
- Desktop or VST3 target.
- Windows version.
- Steps to reproduce.
- Logs from `%AppData%\GuitarToolkit\logs` if relevant.

The maintainer will review the report, ask for details if needed, and publish a fix or mitigation when the issue is confirmed.

---

<a id="ru"></a>

# Политика безопасности

[English version](#security-policy)

GuitarToolkit - локальный Windows-инструмент в формате desktop-приложения и VST3-плагина. Проект не предоставляет сетевой сервис и не должен обрабатывать секреты, но сообщения о проблемах безопасности и падениях всё равно важны.

## Поддерживаемые версии

Активные исправления безопасности выпускаются только для последнего публичного релиза.

| Версия | Поддержка |
| --- | --- |
| Последний релиз | Да |
| Старые релизы | По возможности |

## Как сообщить об уязвимости

Если отчёт содержит чувствительные детали, используйте GitHub private vulnerability reporting, если он включён в репозитории. Если приватный канал недоступен, создайте GitHub issue с общим описанием и не публикуйте подробности эксплуатации публично.

Укажите:

- Версию GuitarToolkit.
- Цель: Desktop или VST3.
- Версию Windows.
- Шаги воспроизведения.
- Логи из `%AppData%\GuitarToolkit\logs`, если они важны.

Мейнтейнер проверит отчёт, попросит детали при необходимости и выпустит исправление или mitigation после подтверждения проблемы.
