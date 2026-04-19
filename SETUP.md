# Быстрый старт после клонирования

## Требования
- Visual Studio 2022 (с компонентом C++/CLI support)
- .NET 8 SDK
- FL Studio (для тестирования VST)

## Шаги

### 1. Клонировать и открыть
```
git clone <url> GuitarToolkit
```
Открыть `GuitarToolkit.sln` в Visual Studio.

### 2. Собрать
- Платформа: **x64**
- Ctrl+Shift+B

### 3. Запустить Desktop
- ПКМ на `GuitarToolkit.Desktop` → Set as Startup Project
- F5

### 4. Установить VST-плагин
Запусти `deploy-vst.bat` от **имени администратора** (ПКМ → Запуск от имени администратора).
Он скопирует плагин в `C:\Program Files\Common Files\VST3\GuitarToolkit\`.

Если скрипт ругается на отсутствие бридж-файла:
1. Найди `AudioPlugSharpVst.vst3` в `%USERPROFILE%\.nuget\packages\audioplugsharpvst3\0.7.9\contentFiles\vst\`
2. Скопируй его и `Ijwhost.dll` в папку сборки плагина (`GuitarToolkit.Plugin\bin\x64\Debug\net8.0-windows\`)
3. Переименуй `AudioPlugSharpVst.vst3` → `GuitarToolkit.PluginBridge.vst3`
4. Запусти `deploy-vst.bat` снова

### 5. FL Studio
- Options → File Settings → Manage plugins → Find plugins
- GuitarToolkit появится в списке эффектов
