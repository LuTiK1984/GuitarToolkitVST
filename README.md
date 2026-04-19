# GuitarToolkit — VST3 плагин на C# / .NET 8

## Структура проекта

```
GuitarToolkit.sln
├── GuitarToolkit.Core/        ← Ядро: DSP, модели, сервисы (без зависимостей)
│   ├── DSP/
│   │   ├── Fft.cs             — FFT (Cooley–Tukey), замена NAudio.Dsp
│   │   ├── PitchDetector.cs   — определение частоты (FFT + HPS)
│   │   └── ClickGenerator.cs  — генерация щелчков метронома
│   ├── Models/
│   │   ├── NoteUtils.cs       — частота ↔ нота, центы
│   │   └── Tunings.cs         — справочник строёв
│   └── Services/
│       ├── TunerEngine.cs     — движок тюнера (ring buffer, сглаживание)
│       └── MetronomeEngine.cs — движок метронома (sample-accurate)
│
└── GuitarToolkit.Plugin/      ← VST3-плагин + WPF интерфейс
    ├── GuitarToolkitPlugin.cs — главный класс плагина (AudioPluginWPF)
    └── UI/
        ├── PluginView.xaml    — главная панель с вкладками
        ├── TunerView.xaml     — вкладка тюнера
        └── MetronomeView.xaml — вкладка метронома
```

---

## Предварительные требования

1. **Visual Studio 2022** (17.8+)
2. **.NET 8 SDK** (обычно ставится вместе с VS)
3. **Компонент «Поддержка C++/CLI»** — нужен для AudioPlugSharp:
   - Visual Studio Installer → Изменить → Отдельные компоненты
   - Найти «Поддержка C++/CLI» (или "C++/CLI support for v143 build tools")
   - Установить
4. **FL Studio** (или другой DAW с поддержкой VST3)

---

## Пошаговая инструкция

### Шаг 1. Открыть решение

Открой `GuitarToolkit.sln` в Visual Studio.

Если .sln не открывается, создай решение вручную:
1. File → New → Blank Solution → имя "GuitarToolkit"
2. ПКМ на решении → Add → Existing Project → выбери `GuitarToolkit.Core.csproj`
3. ПКМ на решении → Add → Existing Project → выбери `GuitarToolkit.Plugin.csproj`

### Шаг 2. Восстановить NuGet-пакеты

ПКМ на решении → Restore NuGet Packages.

Должны скачаться:
- `AudioPlugSharp` (0.7.9)
- `AudioPlugSharpWPF` (0.7.9)

### Шаг 3. Выбрать платформу

В верхнем тулбаре VS:
- Платформа решения: **x64** (не Any CPU!)
- Конфигурация: Debug

### Шаг 4. Собрать

Ctrl+Shift+B (Build Solution).

Результат сборки будет в:
`GuitarToolkit.Plugin\bin\x64\Debug\net8.0-windows\`

Там должны появиться:
- `GuitarToolkit.Plugin.dll` — сам плагин
- `GuitarToolkit.PluginBridge.vst3` — бридж для DAW
- `GuitarToolkit.PluginBridge.runtimeconfig.json`
- `Ijwhost.dll`
- `AudioPlugSharp.dll`, `AudioPlugSharpWPF.dll`
- `GuitarToolkit.Core.dll`

### Шаг 5. Установить плагин в FL Studio

Скопируй **всю папку** `net8.0-windows` (или её содержимое) в:

```
C:\Program Files\Common Files\VST3\GuitarToolkit\
```

Или в любую другую папку, которая указана в FL Studio как путь к VST-плагинам.

### Шаг 6. Загрузить в FL Studio

1. Открой FL Studio
2. Options → File Settings → Manage plugins → Find plugins (Rescan)
3. Найди "GuitarToolkit" в списке эффектов (Effects)
4. Создай аудиотрек, подключи гитару/микрофон ко входу
5. Добавь GuitarToolkit как эффект на этот трек

---

## Отладка

Чтобы ставить breakpoints в своём C#-коде:
1. Собери проект в Debug
2. Скопируй файлы в папку VST3
3. Открой FL Studio
4. В VS: Debug → Attach to Process → найди `FL64.exe` → Attach
5. Загрузи плагин в FL Studio — breakpoints должны сработать

---

## Известные моменты

- Если FL Studio не видит плагин — проверь, что скопированы ВСЕ файлы,
  включая Ijwhost.dll и .runtimeconfig.json
- Если плагин крашит DAW — проверь, что установлен компонент C++/CLI
- Метод `Host.ProcessAllEvents()` в плагине — если не компилируется,
  попробуй `Host.ProcessEvents()` (зависит от версии API)
- Sample rate берётся от хоста в Initialize(), если значение нестандартное,
  движки корректно адаптируются

---

## Дальнейшая разработка

- [ ] Справочник аккордов (вкладка «Аккорды»)
- [ ] Визуализатор грифа и гамм (вкладка «Гаммы»)
- [ ] Проверка и доработка тюнера (чувствительность к микрофону)
- [ ] (Опционально) Standalone-версия — добавить третий проект
      GuitarToolkit.Desktop с NAudio для захвата звука
