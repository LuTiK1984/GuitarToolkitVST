# Contributing to GuitarToolkit

Thanks for helping improve GuitarToolkit. This project is a Windows guitar
toolkit shipped as both a standalone WPF app and a VST3 plugin, so stability and
clear reproduction steps matter a lot.

## Project layout

- `GuitarToolkit.Core` contains DSP, theory models, and services. Keep it free
  of WPF, NAudio, and AudioPlugSharp dependencies.
- `GuitarToolkit.UI` contains shared WPF controls used by both Desktop and VST3.
- `GuitarToolkit.Desktop` is the standalone Windows app and uses NAudio.
- `GuitarToolkit.Plugin` is the VST3 entry point and uses AudioPlugSharp.
- `GuitarToolkit.Tests` contains xUnit tests for Core behavior.

## Development setup

Requirements:

- Windows 10/11 x64.
- .NET 8 SDK.
- Visual Studio 2022 or another editor that can build WPF projects.

Restore, build, and test:

```powershell
dotnet restore GuitarToolkit.sln
dotnet build GuitarToolkit.sln --no-restore --configuration Debug
dotnet test GuitarToolkit.sln --no-restore --configuration Debug
```

For VST-related changes, build and test as `x64`.

## Pull request checklist

Before opening a pull request:

- Keep the change focused.
- Update `CHANGELOG.md` for user-visible behavior changes.
- Update README or docs when setup, release packaging, or DAW behavior changes.
- Run the Debug build and test commands above.
- For VST changes, manually test in a DAW when possible.
- Do not add blocking operations, file I/O, locks, or frequent logging in the
  audio callback.

## DAW and VST changes

DAW hosts can be sensitive to plugin metadata, port layout, editor startup, and
runtime files. For VST changes, please include:

- DAW name and version.
- Windows version.
- Audio interface or input device when relevant.
- Whether the plugin was freshly scanned after deployment.
- Logs from `%AppData%\GuitarToolkit\logs` if available.

Manual VST smoke test:

1. Add GuitarToolkit to a track.
2. Assign or change the recording input after the plugin is loaded.
3. Check tuner input, metronome output, and chord/scale playback.
4. Re-scan or restart the DAW after running `deploy-vst.bat`.

## Issue reports

Use the issue templates when possible. For bugs, include:

- Expected behavior.
- Actual behavior.
- Steps to reproduce.
- Release version or commit.
- Desktop or VST3 target.
- Logs, screenshots, or sample files when they help.

For Guitar Pro or MusicXML import issues, mention the file format and whether
the file can be shared publicly.
