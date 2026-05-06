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

## Long-Term Feature: ONNX-First Local Inspiration Engine

GuitarToolkit may grow a local music generation module for guitar-focused inspiration. The direction is **ONNX-first**: start with a real local model for symbolic music tokens, then move toward a small Transformer when the dataset, validation layer, and product workflow are ready.

This should not become a general-purpose chatbot or an LLM-driven application. The feature should stay narrow, offline-friendly, reproducible where possible, and useful for musicians during practice, writing, or DAW work.

### Product Goal

Build an `Inspiration Engine` that can generate and explain:

- chord progressions;
- simple melodic phrases;
- guitar riff ideas;
- scale suggestions for soloing;
- variations of an existing progression or melody;
- eventually MIDI output and playable guitar-oriented representations.

The module should feel like a native GuitarToolkit feature, not like an embedded AI chat window.

### Non-Goals

- Do not add a general LLM/chat interface as the core experience.
- Do not require paid APIs, tokens, subscriptions, or network access for the core feature.
- Do not generate final mixed audio as the first target.
- Do not promise full song composition.
- Do not let generated content bypass music-theory and guitar-playability validation.

### Model Strategy

The first ONNX model should be a **GRU/LSTM next-token model**, not a large Transformer.

Why GRU/LSTM first:

- simple architecture;
- lightweight runtime footprint;
- fast to train;
- easy to export to ONNX;
- strong fit for short symbolic sequences;
- less likely to overfit a small early dataset.

A small Transformer is the planned evolution path.

Why a small Transformer later:

- better long-context handling;
- more modern sequence modeling;
- more interesting variations once there is enough data;
- useful for longer melodies, riffs, and multi-section ideas.

Transformer risks:

- more implementation complexity;
- easier to overfit on a small dataset;
- ONNX export and runtime behavior may need more care;
- more testing is needed before shipping it inside Desktop and VST3.

### First ONNX Prototype

```text
Model: ProgressionNextTokenModel
Task: predict the next roman-numeral chord token
Input: style + mode + mood + previous roman-numeral tokens
Output: probability distribution for the next roman-numeral token
```

Example:

```text
Input: STYLE_METAL, MODE_MINOR, MOOD_DARK, i, VI
Output: VII 0.42, III 0.25, iv 0.18, V 0.10, bII 0.05
```

C# then uses temperature/top-k sampling and validation to assemble a progression:

```text
i - VI - VII - i
```

Recommended progression model size:

- vocabulary: roughly 100-300 tokens;
- embedding: 64-128;
- GRU/LSTM hidden size: 128-256;
- layers: 1-2;
- target size: below roughly 1-5 million parameters.

Recommended melody model size, later:

- vocabulary: roughly 300-1000 tokens;
- embedding: 128;
- hidden size: 256-512;
- layers: 2;
- target size: roughly 2-10 million parameters.

Hardware note:

- an RTX 3060 Ti with 8 GB VRAM is more than enough for these symbolic sequence models;
- the project is not training 4K image models, raw-audio generators, or large language models;
- the GPU limit only becomes serious for large Transformer experiments, huge MIDI corpora, raw audio generation, or multi-instrument composition models.

### Recommended Product UI

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

### Architecture Direction for C#/.NET

Keep the feature split into clear layers so the app can start with ONNX while preserving testable theory, validation, and export boundaries.

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
|   |-- ProgressionNextTokenModel
|   |-- OnnxProgressionModel
|   |-- OnnxModelMetadata
|   |-- TemperatureSampler
|   |-- TopKSampler
|   |-- MelodyGenerator
|   |-- RiffGenerator
|   `-- GuitarPlayabilityValidator
|
`-- Export
    `-- MidiExporter
```

Keep WPF UI code outside the generation core. ONNX inference, sampling, theory mapping, and validation should be testable from `GuitarToolkit.Tests` without Desktop, VST3, or UI dependencies.

### Suggested Implementation Path

#### Phase 1: ONNX Progression Next-Token Prototype

Start with a narrow local ML feature: a GRU/LSTM model that predicts the next roman-numeral chord token.

Required capabilities:

- define a compact symbolic token vocabulary;
- encode style, mode, mood, and previous roman-numeral tokens;
- train a GRU/LSTM model outside the app;
- export the trained model to `.onnx`;
- run local inference from C# behind an interface;
- sample the next token with temperature/top-k;
- assemble short progressions from repeated next-token predictions.

Acceptance criteria:

- no internet, paid API, token, or subscription is required;
- the model can generate a 4-bar progression from a seed prompt;
- the same seed and sampling settings can reproduce testable output;
- the app still works when the model file is absent or disabled;
- ONNX inference never runs from the audio callback path.

#### Phase 2: Theory Foundation and Validation Layer

Build enough theory support to validate every model output before the UI displays it.

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
- invalid model output is rejected or repaired before display;
- no WPF/NAudio/VST dependency is introduced into the core theory layer.

#### Phase 3: Style Profiles and Progression Assembly

Use style profiles as guardrails around the ONNX model rather than replacing the model with hard-coded generation.

Required capabilities:

- generate progressions by key/mode/style/mood/difficulty from model probabilities;
- produce roman numeral analysis;
- produce 3-5 alternative variations;
- explain why the progression works in short musician-friendly language;
- avoid obviously weak outputs by style profile;
- prefer guitar-friendly chords and voicings.

Example style behavior:

- minor metal: prefer `i`, `VI`, `VII`, `iv`, `V`, `bII`;
- neoclassical: prefer harmonic minor colors, `V7 -> i`, diminished passing chords;
- blues: prefer `I7`, `IV7`, `V7`, minor pentatonic/blues scale suggestions;
- ambient: prefer slower movement, suspended colors, add9/add11 where supported.

Acceptance criteria:

- the generator returns structured objects, not UI strings only;
- invalid combinations fail gracefully;
- at least 20 unit tests cover common generation and validation cases;
- model output is always post-processed through theory and playability checks.

#### Phase 4: Melody and Riff ONNX Models

Do not start the whole feature with melodies. Add melody and riff models only after progression generation is useful. Do not generate absolute notes first; generate scale degrees and durations, then map them to notes and guitar positions.

Recommended melody representation:

```text
1:1/8, b3:1/8, 4:1/8, 5:1/8, b7:1/4, 5:1/4
```

Then map it to the selected key, for example `E minor -> E, G, A, B, D, B`.

Acceptance criteria:

- easy difficulty avoids large jumps and fast dense passages;
- generated phrases stay inside the selected scale unless the style explicitly allows color notes;
- guitar validation can reject or simplify unplayable phrases;
- output can later be converted to MIDI.

#### Phase 5: Small Transformer Evolution

Add a small Transformer after the GRU/LSTM prototype, theory validation, and progression workflow are useful.

Recommended first Transformer task:

```text
Input: style + mode + mood + previous roman-numeral tokens
Output: probability distribution for the next roman-numeral token
```

Recommended constraints:

- keep the token vocabulary symbolic and small;
- start with progressions before melodies;
- keep model size small enough to ship with the app if licensing allows;
- compare quality against the GRU/LSTM baseline before replacing it;
- keep GRU/LSTM as a fallback if Transformer inference is unavailable.

Acceptance criteria:

- Transformer output passes the same theory and guitar-playability validation;
- inference can be disabled without breaking the feature;
- tests cover model selection, fallback behavior, and malformed model metadata;
- documentation explains model source, license, limitations, and training data.

#### Phase 6: MIDI Export and Preview-Friendly Output

Add export after generated musical structures are stable.

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

### Training Notes

- ONNX is the deployment format, not the primary training environment.
- Train using PyTorch or another ML framework, then export to ONNX.
- PyTorch CUDA builds for NVIDIA GPUs are appropriate for local training.
- Do not train or run a general text LLM.
- Prefer symbolic music tokens, not raw audio.
- Keep the dataset license-compatible with an open-source project.
- Keep generated outputs validated by the existing theory/playability engine.

### Suggested Interfaces

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
    public double Temperature { get; init; } = 0.85;
    public int TopK { get; init; } = 5;
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

### Agent Guidance

When implementing this feature:

- begin with the narrow ONNX progression next-token model, not a broad AI system;
- prefer GRU/LSTM for the first model;
- move toward a small Transformer after the GRU/LSTM baseline is useful;
- do not add chat UI as the main interface;
- keep generation deterministic under a provided seed;
- make every generated result pass through validation;
- keep Desktop and VST3 behavior safe and non-blocking;
- avoid expensive work on the audio callback path;
- document new model files and their licenses;
- add tests before UI polish;
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
- Улучшить диагностику и доступность логов.
- Полировать Tabs и уменьшать заметный re-render jitter.
- Улучшать интерфейс отдельным design pass.
- Улучшить первый запуск.

## Среднесрочные идеи

- Installer или более полированный portable package.
- DAW-aware поведение плагина.
- Более сильные workflow для табов, прогрессий, интервалов, гамм, аккордов, метронома и тюнера.
- Улучшение audio UX.
- Доверие к проекту: лицензии, third-party notices, релизная упаковка, исследование signing/installer.

## Долгосрочная фича: ONNX-first Local Inspiration Engine

В проект можно добавить локальный модуль музыкальной генерации для гитарного вдохновения. Новое направление - **ONNX-first**: начинаем с настоящей локальной модели для symbolic music tokens, затем двигаемся к маленькому Transformer, когда будут готовы датасет, validation layer и продуктовый workflow.

Это не должно превращаться в универсальный чат-бот или LLM-приложение. Фича должна быть узкой, по возможности офлайн, воспроизводимой и полезной музыканту во время практики, сочинения или работы в DAW.

### Цель фичи

Сделать `Inspiration Engine`, который умеет генерировать и объяснять:

- аккордовые прогрессии;
- простые мелодические фразы;
- идеи гитарных риффов;
- гаммы для соло;
- вариации существующей прогрессии или мелодии;
- в перспективе MIDI export и гитарно-ориентированное представление результата.

Модуль должен ощущаться как родная часть GuitarToolkit, а не как встроенное окно чата.

### Что не является целью

- Не добавлять универсальный LLM/chat-интерфейс как основу фичи.
- Не требовать платные API, токены, подписки или интернет для основной функции.
- Не начинать с генерации готового сведённого аудио.
- Не обещать полноценную композицию песни.
- Не показывать сгенерированный результат без music-theory и guitar-playability validation.

### Стратегия модели

Первая ONNX-модель должна быть **GRU/LSTM next-token model**, а не большой Transformer.

Почему GRU/LSTM сначала:

- простая архитектура;
- лёгкий runtime footprint;
- быстро обучается;
- легко экспортируется в ONNX;
- хорошо подходит для коротких symbolic sequences;
- меньше риск переобучения на раннем маленьком датасете.

Маленький Transformer остаётся плановым следующим этапом.

Почему Transformer позже:

- лучше ловит длинный контекст;
- современнее как sequence model;
- может давать более интересные варианты при хорошем датасете;
- полезен для длинных мелодий, риффов и multi-section ideas.

Риски Transformer:

- сложнее реализация;
- легче переобучить на маленьком датасете;
- ONNX export и runtime могут потребовать больше аккуратности;
- перед поставкой в Desktop/VST3 понадобится больше тестов.

### Первый ONNX-прототип

```text
Model: ProgressionNextTokenModel
Task: предсказать следующий roman-numeral chord token
Input: style + mode + mood + previous roman-numeral tokens
Output: probability distribution for the next roman-numeral token
```

Пример:

```text
Input: STYLE_METAL, MODE_MINOR, MOOD_DARK, i, VI
Output: VII 0.42, III 0.25, iv 0.18, V 0.10, bII 0.05
```

Потом C# использует temperature/top-k sampling и validation, чтобы собрать прогрессию:

```text
i - VI - VII - i
```

Размер модели для прогрессий:

- словарь: примерно 100-300 tokens;
- embedding: 64-128;
- GRU/LSTM hidden size: 128-256;
- layers: 1-2;
- целевой размер: примерно меньше 1-5 млн параметров.

Размер модели для мелодий, позже:

- словарь: примерно 300-1000 tokens;
- embedding: 128;
- hidden size: 256-512;
- layers: 2;
- целевой размер: примерно 2-10 млн параметров.

По железу:

- RTX 3060 Ti с 8 GB VRAM более чем достаточно для таких symbolic sequence models;
- проект не обучает 4K image models, raw-audio generators или large language models;
- ограничение GPU станет заметным только для больших Transformer-экспериментов, огромных MIDI-корпусов, raw audio generation или multi-instrument composition models.

### Интерфейс продукта

Лучше сделать структурированную вкладку, например `Inspiration`, `Composer` или `Idea Generator`.

Ввод пользователя:

- key/root note, например `E`, `A`, `C#`;
- mode/scale: major, natural minor, harmonic minor, dorian, phrygian, blues;
- style: rock, metal, neoclassical, blues, ambient;
- mood: dark, epic, sad, aggressive, calm;
- difficulty: easy, medium, hard;
- length: 4, 8 или 16 bars;
- output type: progression, melody, riff, variation.

Вывод:

- сгенерированные аккорды или ноты;
- roman numeral analysis;
- гамма для соло;
- короткое музыкальное объяснение;
- гитарная подсказка: power chords, palm muting, fretboard position;
- позже опциональный MIDI export.

### Архитектура C#/.NET

Фичу нужно держать слоями: ONNX inference отдельно, теория отдельно, validation отдельно, UI отдельно.

`GuitarToolkit.Core` может содержать:

- `MusicTheory`: notes, intervals, scales, chords, keys, roman numerals;
- `Generation`: request/result DTOs, ONNX progression model, samplers, style profiles, playability validator;
- `Export`: MIDI exporter.

WPF UI не должен попадать в generation core. ONNX inference, sampling, theory mapping и validation должны тестироваться из `GuitarToolkit.Tests` без Desktop, VST3 и UI-зависимостей.

### Путь реализации

1. **ONNX ProgressionNextTokenModel**
   GRU/LSTM модель предсказывает следующую ступень аккорда по style/mode/mood и предыдущим roman numerals.

2. **Theory foundation и validation**
   Ноты, гаммы, аккорды, ступени, транспонирование и проверка каждого результата модели перед показом.

3. **Style profiles и сборка прогрессий**
   Style profiles работают как guardrails вокруг модели: metal, neoclassical, blues, ambient и другие стили.

4. **Melody/riff ONNX models**
   Только после полезного генератора прогрессий. Мелодии лучше генерировать как scale degrees + durations, а затем переводить в ноты и позиции на грифе.

5. **Small Transformer**
   После GRU/LSTM baseline. Transformer сравнивается с GRU/LSTM, проходит ту же validation цепочку, а GRU/LSTM остаётся fallback.

6. **MIDI export**
   После стабилизации музыкальных структур. Сначала block chords для прогрессий, позже single-note lines для мелодий.

### Заметки по обучению

- ONNX - формат деплоя, а не основная среда обучения.
- Обучать модель лучше вне приложения, например в Python/PyTorch, затем экспортировать в ONNX.
- PyTorch CUDA builds подходят для локального обучения на NVIDIA GPU.
- Не обучать и не запускать универсальную text LLM.
- Использовать symbolic music tokens, а не raw audio.
- Датасет должен быть совместим с open-source лицензированием.
- Весь output модели проходит theory/playability validation.

### Инструкция для агента

При реализации:

- начинать с узкой ONNX-модели для next-token progression, а не с широкой AI system;
- предпочесть GRU/LSTM для первой модели;
- двигаться к маленькому Transformer после полезного GRU/LSTM baseline;
- не делать chat UI основным интерфейсом;
- сохранять deterministic generation при переданном seed;
- каждый generated result пропускать через validation;
- сохранять Desktop и VST3 безопасными и non-blocking;
- не делать тяжёлую работу на audio callback path;
- документировать model files, dataset source, license и limitations;
- сначала добавлять тесты, потом UI polish.

## Чеклист охоты за багами

- Проверять Tabs после resize/maximize/restore.
- Проверять VST3 startup и editor opening в FL Studio после каждого релиза.
- Проверять VST3 deployment с полным dependency set, включая `runtimes`.
- Проверять поведение DAW при назначении/смене recording input после загрузки плагина.
- Тестировать GP3, GP4, GP5/GPX и MusicXML.
- Проверять, что Desktop и VST3 не расходятся неожиданно в shared UI behavior.

## Коммерческая готовность

Текущее состояние: сильный нишевый open-source/passion project с продуктовым потенциалом.

Перед commercial-grade уровнем нужны:

- более простая установка;
- проверенная VST3-стабильность в разных DAW;
- хорошая диагностика и crash logs;
- понятные лицензии и third-party notices;
- аккуратная релизная упаковка;
- исследование Windows signing/installer.
