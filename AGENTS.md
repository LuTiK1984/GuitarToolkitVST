# GuitarToolkit Development Notes

## Project Shape

- `GuitarToolkit.Core` contains DSP, theory models, and services. Keep it free of WPF, NAudio, and AudioPlugSharp dependencies.
- `GuitarToolkit.UI` contains shared WPF controls used by both the desktop app and the VST plugin.
- `GuitarToolkit.Desktop` is the standalone Windows app and uses NAudio for input/output.
- `GuitarToolkit.Plugin` is the VST3 entry point and uses AudioPlugSharp/AudioPlugSharpWPF.
- `GuitarToolkit.Tests` contains xUnit tests for Core behavior.

## VST Notes

- Build and test VST-related changes as `x64`.
- The NuGet-sourced bridge/runtime files in the repository root are intentional and are used for VST deployment:
  - `GuitarToolkit.PluginBridge.vst3`
  - `GuitarToolkit.PluginBridge.runtimeconfig.json`
  - `AudioPlugSharpWPF.dll`
  - `Ijwhost.dll`
- Avoid changing `GuitarToolkitPlugin.Process()` unless necessary. DAW hosts can be sensitive when audio ports are reassigned while a plugin is loaded.
- Do not use blocking operations, file I/O, locks, or frequent logging in the audio callback.

## Verification

Run these after code changes:

```powershell
dotnet build GuitarToolkit.sln --no-restore --configuration Debug
dotnet test GuitarToolkit.sln --no-restore --configuration Debug
```

For VST changes, also test manually in a DAW:

1. Add GuitarToolkit to a track.
2. Assign or change the recording input after the plugin is loaded.
3. Check tuner input, metronome output, and chord/scale playback.
4. Re-scan or restart the DAW after running `deploy-vst.bat`.
