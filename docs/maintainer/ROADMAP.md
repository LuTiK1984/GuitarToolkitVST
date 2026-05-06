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

## Long-Term Feature: Local Inspiration Engine

The project may include a local music generation module for guitar-focused inspiration. This should not become a general-purpose chatbot or an LLM-driven application. The feature should stay narrow, offline-friendly, deterministic where possible, and useful for musicians during practice, writing, or DAW work.

### Product goal

Build an `Inspiration Engine` that can generate and explain:

- chord progressions;
- simple melodic phrases;
- guitar riff ideas;
- scale suggestions for soloing;
- variations of an existing progression or melody;
- eventually MIDI output and playable guitar-oriented representations.

The module should feel like a native GuitarToolkit feature, not like an embedded AI chat window.

### Non-goals

- Do not add a general LLM/chat interface as the core experience.
- Do not require paid APIs, tokens, subscriptions, or network access for the core feature.
- Do not generate final mixed audio as the first target.
- Do not promise full song composition.
- Do not let generated content bypass music-theory and guitar-playability validation.

### Recommended product UI

Prefer a structured tool tab, for example `Inspiration`, `Composer`, or `Idea Generator`.

Expected user inputs:

- key/root note, for example `E`, `A`, `C#`;
- mode/scale, for example major, natural minor, harmonic minor, dorian, phrygian, blues;
- style, for example rock, metal, neoclassical, blues, ambient;
- mood, for example dark, epic, sad, aggressive, calm;
- difficulty, for example easy, medium, hard;
- length, for example 4, 8, or 16 bars;
- output type, for example progression, melody, riff, variation.

Expected output:

- generated chords or notes;
- roman numeral analysis;
- suggested scale for soloing;
- short musical explanation;
- guitar-oriented hint, for example power chords, palm muting, fretboard position;
- optional export target such as MIDI in a later milestone.

### Architecture direction for C#/.NET

Keep the feature split into clear layers so the app can start simple and later add ML without rewriting the whole module.

Suggested projects/namespaces:

```text
GuitarToolkit.Core
|-- MusicTheory
|   |-- Note
|   |-- Interval
|   |-- Scale
|   |-- Chord
|   |-- Key
|   `-- RomanNumeral
|
|-- Generation
|   |-- GenerationRequest
|   |-- GeneratedProgression
|   |-- GeneratedMelody
|   |-- StyleProfile
|   |-- ProgressionGenerator
|   |-- MelodyGenerator
|   |-- RiffGenerator
|   |-- MarkovProgressionModel
|   |-- MarkovMelodyModel
|   `-- GuitarPlayabilityValidator
|
`-- Export
    `-- MidiExporter
```

Keep WPF UI code outside the generation core. The generation engine should be testable from `GuitarToolkit.Tests` without Desktop, VST3, or UI dependencies.

### Suggested implementation path

#### Phase 1: Music theory foundation

Implement a small, well-tested theory layer before ML.

Required capabilities:

- represent notes and accidentals;
- build common scales from interval formulas;
- build triads and basic seventh chords from scale degrees;
- map roman numerals to real chords in a selected key;
- transpose generated structures between keys;
- expose simple DTOs that the UI can display.

Acceptance criteria:

- tests cover major, natural minor, harmonic minor, pentatonic, and blues basics;
- tests cover at least `E minor`, `A minor`, `C major`, and one sharp/flat key;
- no WPF/NAudio/VST dependency is introduced into the core theory layer.

#### Phase 2: Rule-based progression generator

Start with deterministic and weighted rules. This gives immediate product value without ML.

Required capabilities:

- generate progressions by key/mode/style/mood/difficulty;
- produce roman numeral analysis;
- produce 3-5 alternative variations;
- explain why the progression works in short musician-friendly language;
- avoid obviously weak outputs by style profile.

Example style behavior:

- minor metal: prefer `i`, `VI`, `VII`, `iv`, `V`, `bII`;
- neoclassical: prefer harmonic minor colors, `V7 -> i`, diminished passing chords;
- blues: prefer `I7`, `IV7`, `V7`, minor pentatonic/blues scale suggestions;
- ambient: prefer slower movement, suspended colors, add9/add11 where supported.

Acceptance criteria:

- the generator returns structured objects, not UI strings only;
- generation can be seeded for reproducible tests;
- invalid combinations fail gracefully;
- at least 20 unit tests cover common generation cases.

#### Phase 3: Lightweight local probabilistic model

Add a Markov/n-gram model before any neural model. This keeps the feature local, free, transparent, and easy to tune.

Recommended model format:

```json
{
  "minor_metal_dark": {
    "i": {
      "VI": 0.45,
      "iv": 0.25,
      "VII": 0.20,
      "bII": 0.10
    },
    "VI": {
      "III": 0.35,
      "VII": 0.45,
      "iv": 0.20
    },
    "VII": {
      "i": 0.60,
      "VI": 0.25,
      "iv": 0.15
    }
  }
}
```

Required capabilities:

- load style-specific transition tables from JSON;
- generate roman-numeral progressions from weighted transitions;
- combine model output with theory validation;
- allow simple tuning without retraining a neural network.

Acceptance criteria:

- model files are small and human-reviewable;
- malformed model files are handled safely;
- generated progressions are always validated before display;
- tests cover weighted selection using deterministic random seeds.

#### Phase 4: Melody and riff generation

Do not generate absolute notes first. Generate scale degrees and durations, then map them to notes and guitar positions.

Recommended melody representation:

```text
1:1/8, b3:1/8, 4:1/8, 5:1/8, b7:1/4, 5:1/4
```

Then map it to the selected key, for example `E minor -> E, G, A, B, D, B`.

Required capabilities:

- generate melody tokens as scale degrees plus durations;
- keep phrases within a configurable range;
- prefer playable guitar movement for selected difficulty;
- optionally map notes to string/fret positions;
- support rests and repeated notes;
- keep rhythm simple in the first version.

Acceptance criteria:

- easy difficulty avoids large jumps and fast dense passages;
- generated phrases stay inside the selected scale unless the style explicitly allows color notes;
- guitar validation can reject or simplify unplayable phrases;
- output can later be converted to MIDI.

#### Phase 5: MIDI export and preview-friendly output

Add export only after the generated musical structures are stable.

Recommended direction:

- consider a .NET MIDI library such as DryWetMIDI for writing Standard MIDI Files;
- keep MIDI export separated from generation logic;
- export progressions as block chords first;
- export melodies as single-note lines later;
- keep tempo, bar length, and duration mapping explicit.

Acceptance criteria:

- generated MIDI opens in common DAWs;
- tests verify that export does not crash for empty/invalid results;
- MIDI export is optional and does not affect VST3 stability.

#### Phase 6: Optional ONNX neural model

Only add ONNX after the theory engine, rule-based generation, and Markov model are useful. ONNX should be used for local inference only. Train the model outside the app, for example in Python/PyTorch, then export to `.onnx` for C# runtime usage.

Recommended first ONNX task:

```text
Input: style + mode + mood + previous roman-numeral tokens
Output: probability distribution for the next roman-numeral token
```

Example:

```text
Input: STYLE_METAL, MODE_MINOR, MOOD_DARK, i, VI
Output: VII 0.42, III 0.25, iv 0.18, V 0.10, bII 0.05
```

Recommended model size:

- vocabulary: roughly 100-300 tokens for progressions;
- embedding: 64-128;
- GRU/LSTM hidden size: 128-256;
- layers: 1-2;
- target size: small enough to ship with the app if licensing allows.

Training notes:

- ONNX is the deployment format, not the primary training environment;
- train using PyTorch or another ML framework, then export to ONNX;
- do not train or run a general text LLM;
- prefer symbolic music tokens, not raw audio;
- keep the dataset license-compatible with an open-source project;
- keep generated outputs validated by the existing theory/playability engine.

Acceptance criteria:

- the app still works when the ONNX model is absent or disabled;
- ONNX inference is wrapped behind an interface, for example `IProgressionModel`;
- all model output passes through theory validation;
- the model does not require internet access;
- the model does not introduce paid or rate-limited dependencies;
- documentation explains model source, license, and limitations.

### Suggested interfaces

```csharp
public interface IMusicGenerator<TRequest, TResult>
{
    TResult Generate(TRequest request);
}
```

```csharp
public sealed class GenerationRequest
{
    public string RootNote { get; init; } = "E";
    public string Mode { get; init; } = "NaturalMinor";
    public string Style { get; init; } = "Metal";
    public string Mood { get; init; } = "Dark";
    public int Bars { get; init; } = 4;
    public int Difficulty { get; init; } = 2;
    public int? Seed { get; init; }
}
```

```csharp
public sealed class GeneratedProgression
{
    public IReadOnlyList<string> Chords { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> RomanNumerals { get; init; } = Array.Empty<string>();
    public string SuggestedScale { get; init; } = string.Empty;
    public string Explanation { get; init; } = string.Empty;
    public string GuitarHint { get; init; } = string.Empty;
}
```

### Agent guidance

When implementing this feature:

- start with small testable core classes;
- do not begin by adding a neural network;
- do not add chat UI as the main interface;
- keep generation deterministic under a provided seed;
- make every generated result pass through validation;
- keep Desktop and VST3 behavior safe and non-blocking;
- avoid expensive work on the audio callback path;
- document new model files and their licenses;
- add tests before adding UI polish;
- prefer a working narrow feature over a broad unfinished AI system.

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

## Долгосрочная фича: локальный Inspiration Engine

В проект можно добавить локальный модуль музыкальной генерации для гитарного вдохновения. Это не должно превращаться в универсальный чат-бот или LLM-приложение. Фича должна быть узкой, по возможности офлайн, предсказуемой и полезной музыканту во время практики, сочинения или работы в DAW.

### Цель фичи

Сделать `Inspiration Engine`, который умеет генерировать и объяснять:

- аккордовые прогрессии;
- простые мелодические фразы;
- идеи гитарных риффов;
- гаммы для соло;
- вариации существующей прогрессии или мелодии;
- в перспективе MIDI-экспорт и гитарно-ориентированное представление результата.

Модуль должен ощущаться как родная часть GuitarToolkit, а не как встроенное окно чата с ИИ.

### Что не является целью

- Не добавлять универсальный LLM/chat-интерфейс как основу фичи.
- Не требовать платных API, токенов, подписок или интернета для основной функции.
- Не начинать с генерации готового сведённого аудио.
- Не обещать полноценное сочинение целой песни.
- Не показывать сгенерированный результат без проверки теорией музыки и гитарной играбельностью.

### Рекомендуемый UI

Лучше сделать отдельную структурированную вкладку, например `Inspiration`, `Composer` или `Idea Generator`.

Ожидаемые входные параметры:

- тональность/тоника, например `E`, `A`, `C#`;
- лад/гамма, например major, natural minor, harmonic minor, dorian, phrygian, blues;
- стиль, например rock, metal, neoclassical, blues, ambient;
- настроение, например dark, epic, sad, aggressive, calm;
- сложность, например easy, medium, hard;
- длина, например 4, 8 или 16 тактов;
- тип результата, например progression, melody, riff, variation.

Ожидаемый вывод:

- сгенерированные аккорды или ноты;
- римские ступени;
- предложенная гамма для соло;
- короткое музыкальное объяснение;
- гитарная подсказка, например power chords, palm muting, позиция на грифе;
- опциональный экспорт, например MIDI на более позднем этапе.

### Архитектура для C#/.NET

Фичу нужно разделить на понятные слои, чтобы начать просто и позже добавить ML без переписывания всего модуля.

Рекомендуемые проекты/namespace:

```text
GuitarToolkit.Core
|-- MusicTheory
|   |-- Note
|   |-- Interval
|   |-- Scale
|   |-- Chord
|   |-- Key
|   `-- RomanNumeral
|
|-- Generation
|   |-- GenerationRequest
|   |-- GeneratedProgression
|   |-- GeneratedMelody
|   |-- StyleProfile
|   |-- ProgressionGenerator
|   |-- MelodyGenerator
|   |-- RiffGenerator
|   |-- MarkovProgressionModel
|   |-- MarkovMelodyModel
|   `-- GuitarPlayabilityValidator
|
`-- Export
    `-- MidiExporter
```

WPF UI должен оставаться вне ядра генерации. Движок генерации должен тестироваться из `GuitarToolkit.Tests` без зависимостей от Desktop, VST3 и UI.

### Рекомендуемый путь реализации

#### Фаза 1: фундамент теории музыки

Перед ML сначала нужен небольшой и хорошо покрытый тестами слой теории музыки.

Что должно быть реализовано:

- представление нот и знаков альтерации;
- построение популярных гамм по интервальным формулам;
- построение трезвучий и базовых септаккордов по ступеням;
- перевод римских ступеней в реальные аккорды выбранной тональности;
- транспонирование сгенерированных структур между тональностями;
- простые DTO, которые удобно отображать в UI.

Критерии готовности:

- тестами покрыты major, natural minor, harmonic minor, pentatonic и blues basics;
- тестами покрыты хотя бы `E minor`, `A minor`, `C major` и одна тональность с диезами/бемолями;
- в core theory layer не добавлены зависимости от WPF, NAudio или VST.

#### Фаза 2: rule-based генератор прогрессий

Начинать стоит с детерминированных и весовых правил. Это быстро даст полезную фичу без ML.

Что должно быть реализовано:

- генерация прогрессий по тональности/ладу/стилю/настроению/сложности;
- вывод римских ступеней;
- 3-5 альтернативных вариаций;
- короткое объяснение, почему прогрессия работает;
- фильтрация слабых или неуместных результатов через style profile.

Примеры стилевого поведения:

- minor metal: чаще использовать `i`, `VI`, `VII`, `iv`, `V`, `bII`;
- neoclassical: чаще использовать harmonic minor, `V7 -> i`, diminished passing chords;
- blues: чаще использовать `I7`, `IV7`, `V7`, minor pentatonic/blues scale suggestions;
- ambient: более медленное движение, suspended colors, add9/add11 там, где это поддержано.

Критерии готовности:

- генератор возвращает структурированные объекты, а не только готовые UI-строки;
- генерация может быть seed-based для воспроизводимых тестов;
- невалидные комбинации обрабатываются безопасно;
- есть минимум 20 unit-тестов на основные сценарии генерации.

#### Фаза 3: лёгкая локальная вероятностная модель

Перед нейросетью добавить Markov/n-gram модель. Это сохраняет фичу локальной, бесплатной, прозрачной и простой для настройки.

Рекомендуемый формат модели:

```json
{
  "minor_metal_dark": {
    "i": {
      "VI": 0.45,
      "iv": 0.25,
      "VII": 0.20,
      "bII": 0.10
    },
    "VI": {
      "III": 0.35,
      "VII": 0.45,
      "iv": 0.20
    },
    "VII": {
      "i": 0.60,
      "VI": 0.25,
      "iv": 0.15
    }
  }
}
```

Что должно быть реализовано:

- загрузка style-specific transition tables из JSON;
- генерация прогрессий в римских ступенях через weighted transitions;
- объединение выхода модели с theory validation;
- возможность простой ручной настройки без переобучения нейросети.

Критерии готовности:

- файлы моделей маленькие и пригодные для ручного review;
- повреждённые/невалидные model files безопасно обрабатываются;
- сгенерированные прогрессии всегда валидируются перед показом;
- weighted selection покрыт тестами с deterministic random seed.

#### Фаза 4: генерация мелодий и риффов

Не генерировать сразу абсолютные ноты. Лучше генерировать ступени гаммы и длительности, затем переводить их в ноты и позиции на грифе.

Рекомендуемое представление мелодии:

```text
1:1/8, b3:1/8, 4:1/8, 5:1/8, b7:1/4, 5:1/4
```

Потом это переводится в выбранную тональность, например `E minor -> E, G, A, B, D, B`.

Что должно быть реализовано:

- генерация melody tokens как scale degrees plus durations;
- удержание фраз в заданном диапазоне;
- предпочтение удобного гитарного движения с учётом сложности;
- опциональный перевод нот в string/fret positions;
- поддержка пауз и повторяющихся нот;
- простой ритм в первой версии.

Критерии готовности:

- easy difficulty избегает больших скачков и слишком плотных быстрых пассажей;
- фразы остаются внутри выбранной гаммы, если стиль явно не разрешает color notes;
- guitar validation умеет отклонять или упрощать неудобные фразы;
- результат можно в будущем перевести в MIDI.

#### Фаза 5: MIDI export и preview-friendly output

Экспорт добавлять только после того, как музыкальные структуры стали стабильными.

Рекомендуемое направление:

- рассмотреть .NET MIDI-библиотеку вроде DryWetMIDI для записи Standard MIDI Files;
- держать MIDI export отдельно от generation logic;
- сначала экспортировать прогрессии как block chords;
- позже экспортировать мелодии как single-note lines;
- явно хранить tempo, bar length и duration mapping.

Критерии готовности:

- сгенерированный MIDI открывается в распространённых DAW;
- тесты проверяют, что экспорт не падает на пустых/невалидных результатах;
- MIDI export опционален и не влияет на VST3 stability.

#### Фаза 6: опциональная ONNX-модель

ONNX добавлять только после того, как theory engine, rule-based generation и Markov model уже полезны. ONNX использовать только для локального inference. Обучать модель вне приложения, например в Python/PyTorch, затем экспортировать в `.onnx` для C# runtime.

Рекомендуемая первая задача для ONNX:

```text
Input: style + mode + mood + previous roman-numeral tokens
Output: probability distribution for the next roman-numeral token
```

Пример:

```text
Input: STYLE_METAL, MODE_MINOR, MOOD_DARK, i, VI
Output: VII 0.42, III 0.25, iv 0.18, V 0.10, bII 0.05
```

Рекомендуемый размер модели:

- vocabulary: примерно 100-300 tokens для прогрессий;
- embedding: 64-128;
- GRU/LSTM hidden size: 128-256;
- layers: 1-2;
- target size: достаточно маленький, чтобы модель можно было поставлять вместе с приложением, если лицензия это позволяет.

Заметки по обучению:

- ONNX - формат деплоя, а не основная среда обучения;
- обучать через PyTorch или другой ML framework, затем экспортировать в ONNX;
- не обучать и не запускать универсальную text LLM;
- использовать symbolic music tokens, а не raw audio;
- следить, чтобы датасет был совместим с open-source лицензированием;
- весь выход модели всё равно пропускать через existing theory/playability engine.

Критерии готовности:

- приложение продолжает работать, если ONNX-модель отсутствует или выключена;
- ONNX inference спрятан за интерфейсом, например `IProgressionModel`;
- любой выход модели проходит theory validation;
- модель не требует интернета;
- модель не добавляет платных или rate-limited зависимостей;
- документация объясняет источник модели, лицензию и ограничения.

### Рекомендуемые интерфейсы

```csharp
public interface IMusicGenerator<TRequest, TResult>
{
    TResult Generate(TRequest request);
}
```

```csharp
public sealed class GenerationRequest
{
    public string RootNote { get; init; } = "E";
    public string Mode { get; init; } = "NaturalMinor";
    public string Style { get; init; } = "Metal";
    public string Mood { get; init; } = "Dark";
    public int Bars { get; init; } = 4;
    public int Difficulty { get; init; } = 2;
    public int? Seed { get; init; }
}
```

```csharp
public sealed class GeneratedProgression
{
    public IReadOnlyList<string> Chords { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> RomanNumerals { get; init; } = Array.Empty<string>();
    public string SuggestedScale { get; init; } = string.Empty;
    public string Explanation { get; init; } = string.Empty;
    public string GuitarHint { get; init; } = string.Empty;
}
```

### Инструкция для агента

При реализации этой фичи:

- начинать с маленьких testable core classes;
- не начинать сразу с нейросети;
- не добавлять chat UI как основной интерфейс;
- делать генерацию deterministic при переданном seed;
- каждый generated result пропускать через validation;
- сохранять Desktop и VST3 безопасными и non-blocking;
- избегать тяжёлой работы на audio callback path;
- документировать новые model files и их лицензии;
- сначала добавлять тесты, потом UI polish;
- предпочитать рабочую узкую фичу широкому недоделанному AI system.

## Чеклист охоты за багами

- Проверять Tabs после resize/maximize/restore.
- Проверять VST3 startup и editor opening в FL Studio после каждого релиза.
- Проверять деплой полного dependency set, включая `runtimes`.
- Проверять назначение и смену recording input после загрузки плагина.
- Тестировать GP3, GP4, GP5/GPX и MusicXML.

## Коммерческая готовность

Текущее состояние: сильный нишевый open-source/passion project с продуктовым потенциалом.

Перед commercial-grade уровнем нужны более простая установка, проверенная VST3-стабильность в разных DAW, хорошая диагностика, понятные лицензии и third-party notices, более аккуратная релизная упаковка и исследование Windows signing/installer.
