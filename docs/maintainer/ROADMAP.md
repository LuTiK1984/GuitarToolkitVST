# GuitarToolkit Roadmap

[Русская версия](#ru)

GuitarToolkit is a personal, idea-driven guitar toolkit for Windows, available as a standalone desktop app and a VST3 plugin. The goal is to grow it from a useful passion project into a polished, trustworthy tool for guitar practice, writing, and DAW workflows.

## Product Direction

- Keep the project musician-first: fast to open, clear to use, and useful during real practice or recording sessions.
- Treat Desktop and VST3 as first-class targets, but validate DAW behavior carefully before calling a feature stable.
- Prefer practical workflow improvements over feature count.
- Keep the open-source/passion-project spirit while gradually improving product polish.

## Near-Term Priorities

- Improve VST3 installation experience.
- Keep separate FL Studio, Reaper, and DAW compatibility docs.
- Improve diagnostics and logs discoverability.
- Polish Tabs behavior and reduce visible re-render jitter.
- Rebuild interface layout as a dedicated design pass.
- Improve first-run clarity.

## Medium-Term Ideas

- Installer or polished portable package.
- DAW-aware plugin behavior.
- Better tab workflow.
- Better progression workflow.
- Better interval training workflow.
- Better scale practice workflow.
- Better chord workflow.
- Better metronome workflow.
- Better tuner workflow.
- Better audio UX.
- Project trust: license clarity, third-party notices, release packaging, signing research.

## Bughunting Checklist

- Verify Tabs after resize/maximize/restore.
- Verify VST3 startup and editor opening in FL Studio after each release.
- Verify VST3 deployment with the full dependency set, including `runtimes`.
- Verify DAW behavior when assigning/changing recording input after plugin load.
- Test tab loading with GP3, GP4, GP5/GPX, and MusicXML files.
- Check that Desktop and VST3 do not diverge unexpectedly in shared UI behavior.

## Commercial Readiness Notes

Current state: strong niche open-source/passion project with product potential.

Before it feels commercial-grade:

- installation must be simpler and less scary;
- VST3 stability should be verified across multiple DAWs;
- logs/crash diagnostics should exist;
- license and third-party notices should be clear;
- release packages should feel intentional, not like raw build folders;
- Windows signing/installer strategy should be researched.

---

<a id="ru"></a>

# Roadmap GuitarToolkit

[English version](#guitartoolkit-roadmap)

GuitarToolkit - личный, идейный гитарный набор инструментов для Windows, доступный как standalone desktop-приложение и VST3-плагин. Цель проекта - вырасти из полезного passion project в аккуратный и надёжный инструмент для практики, сочинения и работы в DAW.

## Направление продукта

- Держать проект musician-first: быстро открыть, легко понять, удобно использовать во время реальной практики или записи.
- Считать Desktop и VST3 равноправными целями, но аккуратно проверять поведение в DAW перед тем, как называть функцию стабильной.
- Предпочитать практичные улучшения workflow простому наращиванию количества функций.
- Сохранять open-source/passion-project характер и постепенно повышать качество продукта.

## Ближайшие приоритеты

- Улучшить установку VST3.
- Поддерживать отдельные инструкции для FL Studio, Reaper и общего списка совместимости DAW.
- Развивать диагностику и логи.
- Полировать Tabs.
- Улучшать интерфейс как отдельный design pass.
- Улучшить первый запуск.

## Среднесрочные идеи

- Installer или более полированный portable package.
- DAW-aware поведение.
- Более сильный workflow табов, прогрессий, интервалов, гамм, аккордов, метронома и тюнера.
- Улучшение audio UX.
- Улучшение доверия к проекту: license, third-party notices, release packaging, signing research.

## Чеклист охоты за багами

- Проверять Tabs после resize/maximize/restore.
- Проверять VST3 startup и editor opening в FL Studio после каждого релиза.
- Проверять деплой полного dependency set, включая `runtimes`.
- Проверять назначение и смену recording input после загрузки плагина.
- Тестировать GP3, GP4, GP5/GPX и MusicXML.

## Коммерческая готовность

Текущее состояние: сильный нишевый open-source/passion project с продуктовым потенциалом.

Перед commercial-grade уровнем нужны более простая установка, проверенная VST3-стабильность в разных DAW, хорошая диагностика, понятные лицензии и third-party notices, более аккуратная релизная упаковка и исследование Windows signing/installer.
