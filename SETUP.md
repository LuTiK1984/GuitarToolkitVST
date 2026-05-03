# Quick Start After Cloning

[Русская версия](#ru)

## Requirements

- Visual Studio 2022 with WPF/.NET desktop workload.
- .NET 8 SDK.
- Windows 10/11 x64.
- FL Studio, Reaper, or another VST3 DAW for plugin testing.

## Steps

### 1. Clone and Open

```powershell
git clone <url> GuitarToolkit
```

Open `GuitarToolkit.sln` in Visual Studio.

### 2. Build

- Platform: `x64`.
- Use `Ctrl+Shift+B` in Visual Studio, or run:

```powershell
dotnet build GuitarToolkit.sln --configuration Debug
```

### 3. Run Desktop

- Right-click `GuitarToolkit.Desktop`.
- Select `Set as Startup Project`.
- Press `F5`.

### 4. Install the VST Plugin

Run `deploy-vst.bat` as Administrator. It copies the plugin to:

```text
C:\Program Files\Common Files\VST3\GuitarToolkit\
```

Copy the full plugin folder. Do not copy only `GuitarToolkit.PluginBridge.vst3`; the plugin also needs dependencies and the `runtimes` folder.

### 5. Scan in a DAW

- FL Studio: `Options -> Manage plugins -> Find installed plugins`.
- Reaper: `Options -> Preferences -> Plug-ins -> VST -> Clear cache/re-scan`.

---

<a id="ru"></a>

# Быстрый старт после клонирования

[English version](#quick-start-after-cloning)

## Требования

- Visual Studio 2022 с workload для WPF/.NET desktop.
- .NET 8 SDK.
- Windows 10/11 x64.
- FL Studio, Reaper или другая DAW с поддержкой VST3 для проверки плагина.

## Шаги

### 1. Клонировать и открыть

```powershell
git clone <url> GuitarToolkit
```

Откройте `GuitarToolkit.sln` в Visual Studio.

### 2. Собрать

- Платформа: `x64`.
- В Visual Studio нажмите `Ctrl+Shift+B` или выполните:

```powershell
dotnet build GuitarToolkit.sln --configuration Debug
```

### 3. Запустить Desktop

- Нажмите правой кнопкой по `GuitarToolkit.Desktop`.
- Выберите `Set as Startup Project`.
- Нажмите `F5`.

### 4. Установить VST-плагин

Запустите `deploy-vst.bat` от имени администратора. Он скопирует плагин сюда:

```text
C:\Program Files\Common Files\VST3\GuitarToolkit\
```

Копируйте весь каталог плагина. Не копируйте только `GuitarToolkit.PluginBridge.vst3`: плагину также нужны зависимости и папка `runtimes`.

### 5. Просканировать в DAW

- FL Studio: `Options -> Manage plugins -> Find installed plugins`.
- Reaper: `Options -> Preferences -> Plug-ins -> VST -> Clear cache/re-scan`.
