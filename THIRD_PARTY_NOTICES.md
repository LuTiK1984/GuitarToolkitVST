# Third-Party Notices

[Русская версия](#ru)

GuitarToolkit uses several open-source libraries and runtime components. This file is a practical project notice, not a substitute for the original license texts published by each project.

## Runtime Dependencies

| Component | Version | Purpose | License | Project |
| --- | --- | --- | --- | --- |
| alphaTab / AlphaTab.Windows | 1.8.2 | Guitar Pro and MusicXML rendering/playback | MPL-2.0 | https://github.com/CoderLine/alphaTab |
| AudioPlugSharp / AudioPlugSharpWPF | 0.7.9 | VST3 plugin bridge and WPF hosting | MIT | https://github.com/mikeoliphant/AudioPlugSharp |
| Microsoft.ML.OnnxRuntime | 1.20.1 | Local ONNX model inference for the Inspiration Engine | MIT | https://github.com/microsoft/onnxruntime |
| NAudio | 2.2.1 | Desktop audio input/output | MIT | https://github.com/naudio/NAudio |
| .NET / WPF | 8.0 | Application runtime and desktop UI | Microsoft/.NET licenses | https://dotnet.microsoft.com/ |

## Development/Test Dependencies

| Component | Version | Purpose | License | Project |
| --- | --- | --- | --- | --- |
| xUnit | 2.6.2 | Unit tests | Apache-2.0 | https://github.com/xunit/xunit |
| Microsoft.NET.Test.Sdk | 17.8.0 | Test execution | Microsoft/.NET licenses | https://github.com/microsoft/vstest |

## Notes

- The VST3 format and VST trademark belong to Steinberg Media Technologies GmbH.
- Release packages may include transitive dependencies copied by the .NET SDK or NuGet packages.
- Before any commercial distribution, verify the bundled dependency list and include all license texts required by those projects.

---

<a id="ru"></a>

# Уведомления о сторонних компонентах

[English version](#third-party-notices)

GuitarToolkit использует несколько open-source библиотек и runtime-компонентов. Этот файл является практическим уведомлением проекта и не заменяет оригинальные тексты лицензий, опубликованные каждым проектом.

## Runtime-зависимости

| Компонент | Версия | Назначение | Лицензия | Проект |
| --- | --- | --- | --- | --- |
| alphaTab / AlphaTab.Windows | 1.8.2 | Рендеринг и воспроизведение Guitar Pro / MusicXML | MPL-2.0 | https://github.com/CoderLine/alphaTab |
| AudioPlugSharp / AudioPlugSharpWPF | 0.7.9 | VST3 bridge и WPF hosting | MIT | https://github.com/mikeoliphant/AudioPlugSharp |
| Microsoft.ML.OnnxRuntime | 1.20.1 | Локальный ONNX inference для Inspiration Engine | MIT | https://github.com/microsoft/onnxruntime |
| NAudio | 2.2.1 | Desktop audio input/output | MIT | https://github.com/naudio/NAudio |
| .NET / WPF | 8.0 | Runtime приложения и desktop UI | Microsoft/.NET licenses | https://dotnet.microsoft.com/ |

## Зависимости разработки и тестов

| Компонент | Версия | Назначение | Лицензия | Проект |
| --- | --- | --- | --- | --- |
| xUnit | 2.6.2 | Unit tests | Apache-2.0 | https://github.com/xunit/xunit |
| Microsoft.NET.Test.Sdk | 17.8.0 | Запуск тестов | Microsoft/.NET licenses | https://github.com/microsoft/vstest |

## Заметки

- Формат VST3 и товарный знак VST принадлежат Steinberg Media Technologies GmbH.
- Релизные пакеты могут включать транзитивные зависимости, скопированные .NET SDK или NuGet packages.
- Перед коммерческим распространением нужно отдельно проверить полный список bundled-зависимостей и добавить все тексты лицензий, которые требуют эти проекты.
