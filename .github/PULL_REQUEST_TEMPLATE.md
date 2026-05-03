## Summary / Кратко

- 

## Verification / Проверка

- [ ] `dotnet build GuitarToolkit.sln --no-restore --configuration Debug`
- [ ] `dotnet test GuitarToolkit.sln --no-restore --configuration Debug`
- [ ] Manual Desktop smoke test, if UI/audio behavior changed / Ручная Desktop-проверка, если менялось UI или аудио
- [ ] Manual VST3/DAW smoke test, if plugin behavior changed / Ручная VST3/DAW-проверка, если менялся плагин

## Checklist / Чеклист

- [ ] Change is focused and scoped / Изменение сфокусировано и не расползается
- [ ] `CHANGELOG.md` updated for user-visible changes / `CHANGELOG.md` обновлён для пользовательских изменений
- [ ] Docs updated for setup, packaging, screenshots, or DAW behavior changes / Документация обновлена для установки, упаковки, скриншотов или поведения в DAW
- [ ] `GuitarToolkit.Core` remains free of WPF, NAudio, and AudioPlugSharp / `GuitarToolkit.Core` не зависит от WPF, NAudio и AudioPlugSharp
- [ ] Audio callback changes avoid blocking operations, file I/O, locks, and frequent logging / В audio callback нет блокировок, файлового I/O, locks и частого логирования

## DAW Notes / Заметки по DAW

DAW/version tested / Проверенная DAW и версия:

Relevant logs or screenshots / Логи или скриншоты:
