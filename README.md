# 🎸 GuitarToolkit

Набор инструментов для гитариста — VST3-плагин и standalone-приложение на C# / .NET 8.

Четыре модуля в одном пакете: тюнер, метроном, справочник аккордов, визуализатор гамм. Единая кодовая база, две платформы.

## Возможности

**Тюнер** — определение высоты звука в реальном времени (FFT + Harmonic Product Spectrum), отображение ноты, частоты и отклонения в центах. 8 гитарных строёв, настраиваемый эталон A4, регулировка усиления входа.

**Метроном** — 30–300 BPM, 2–8 долей в такте, Tap Tempo, визуальные индикаторы. Sample-accurate генерация щелчков без таймеров.

**Справочник аккордов** — 12 тоник × 9 типов (мажор, минор, 7, maj7, m7, sus2, sus4, dim, aug). Диаграмма аппликатуры на грифе, воспроизведение аккорда (аддитивный синтез).

**Визуализатор гамм** — 13 гамм и ладов на грифе (15 ладов). Переключение между именами нот и ступенями. Выделение тоники.

## Архитектура

```
GuitarToolkit.sln
│
├── GuitarToolkit.Core/           Ядро: DSP, модели, сервисы (0 зависимостей)
│   ├── DSP/
│   │   ├── Fft.cs                  FFT Cooley–Tukey (radix-2, in-place)
│   │   ├── PitchDetector.cs        Определение частоты (FFT + HPS + интерполяция)
│   │   ├── ClickGenerator.cs       Генерация щелчков метронома
│   │   └── ChordPlayer.cs          Синтез звучания аккордов
│   ├── Models/
│   │   ├── NoteUtils.cs            Частота ↔ нота, центы, MIDI
│   │   ├── Tunings.cs              Справочник гитарных строёв
│   │   ├── ChordDefinition.cs      Модель аккорда (аппликатура)
│   │   ├── ChordLibrary.cs         Библиотека аккордов (E/A-формы + открытые)
│   │   ├── ScaleDefinition.cs      Модель гаммы
│   │   └── ScaleLibrary.cs         Библиотека гамм и ладов
│   └── Services/
│       ├── TunerEngine.cs          Движок тюнера (ring buffer, сглаживание)
│       └── MetronomeEngine.cs      Движок метронома (sample-accurate)
│
├── GuitarToolkit.UI/             WPF-компоненты (общие для обеих платформ)
│   ├── IAudioHost.cs               Интерфейс IAudioPlayback
│   ├── ToolkitHostView.xaml        Главная панель с вкладками и стилями
│   ├── TunerView.xaml              Вкладка тюнера
│   ├── MetronomeView.xaml          Вкладка метронома
│   ├── ChordView.xaml              Вкладка аккордов
│   └── FretboardView.xaml          Вкладка гамм
│
├── GuitarToolkit.Plugin/         VST3-плагин (AudioPlugSharp)
│   └── GuitarToolkitPlugin.cs      Точка входа, реализация IAudioPlayback
│
└── GuitarToolkit.Desktop/        Standalone-приложение (NAudio)
    ├── AudioBridge.cs               NAudio-обёртка, реализация IAudioPlayback
    └── MainWindow.xaml              Окно с выбором устройства ввода
```

## Требования

- **Visual Studio 2022** (17.8+)
- **.NET 8 SDK**
- **Компонент C++/CLI** — нужен для AudioPlugSharp:
  Visual Studio Installer → Изменить → Отдельные компоненты → «Поддержка C++/CLI» (C++/CLI support for v143 build tools)
- **DAW с поддержкой VST3** для тестирования плагина (FL Studio, Reaper и т.д.)

## Быстрый старт

### Сборка

```
git clone <url> GuitarToolkit
```

Открыть `GuitarToolkit.sln` в Visual Studio. Платформа: **x64**. Собрать: **Ctrl+Shift+B**.

### Запуск Desktop

ПКМ на `GuitarToolkit.Desktop` → Set as Startup Project → **F5**.

Выбрать устройство ввода (микрофон / линейный вход) в выпадающем списке сверху.

### Установка VST3-плагина

Запустить `deploy-vst.bat` **от имени администратора**. Скрипт скопирует плагин в `C:\Program Files\Common Files\VST3\GuitarToolkit\`.

Если бридж-файлы не собрались автоматически:

1. Найти `AudioPlugSharpVst.vst3` и `Ijwhost.dll` в `%USERPROFILE%\.nuget\packages\audioplugsharpvst3\0.7.9\contentFiles\vst\`
2. Скопировать в `GuitarToolkit.Plugin\bin\x64\Debug\net8.0-windows\`
3. Переименовать `AudioPlugSharpVst.vst3` → `GuitarToolkit.PluginBridge.vst3`
4. Создать файл `GuitarToolkit.PluginBridge.runtimeconfig.json`:
```json
{
  "runtimeOptions": {
    "tfm": "net8.0",
    "frameworks": [
      { "name": "Microsoft.NETCore.App", "version": "8.0.0" },
      { "name": "Microsoft.WindowsDesktop.App", "version": "8.0.0" }
    ]
  }
}
```
5. Запустить `deploy-vst.bat`

В DAW: пересканировать плагины, найти **GuitarToolkit** в списке эффектов.

## Отладка VST3

1. Собрать в Debug, задеплоить через `deploy-vst.bat`
2. Открыть DAW
3. В VS: Debug → Attach to Process → `FL64.exe` (или `reaper.exe`) → Attach
4. Загрузить плагин — breakpoints сработают

## Стек

| Компонент | Технология |
|-----------|-----------|
| Язык | C# |
| Платформа | .NET 8, x64 |
| UI | WPF |
| VST3 бридж | AudioPlugSharp 0.7.9 |
| Аудио (Desktop) | NAudio 2.2.1 |
| DSP | Собственная реализация (FFT, HPS) |

## Лицензия

Учебный проект. VST3 — товарный знак Steinberg Media Technologies.
