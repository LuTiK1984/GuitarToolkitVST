# Installing GuitarToolkit

## Desktop Version

1. Download `GuitarToolkit_DESKTOP_v.1.2.0.zip` from GitHub Releases.
2. Extract the archive to any folder.
3. Run `GuitarToolkit.Desktop.exe`.
4. Select an input device in the top panel: microphone, line input, or audio interface.

## VST3 Plugin

1. Download `GuitarToolkit_VST3_v.1.2.0.zip` from GitHub Releases.
2. Close your DAW if it is running.
3. Extract the archive contents to:

```text
C:\Program Files\Common Files\VST3\GuitarToolkit\
```

4. Confirm Administrator access if Windows asks for permission.
5. Open your DAW and rescan VST3 plugins.
6. Add GuitarToolkit as an effect on an audio track.

## If Your DAW Does Not See the Plugin

- Check that `GuitarToolkit.PluginBridge.vst3` is in the VST3 folder.
- Check that `GuitarToolkit.Plugin.dll`, `GuitarToolkit.Core.dll`, `GuitarToolkit.UI.dll`, `AudioPlugSharp.dll`, `AudioPlugSharpWPF.dll`, `Ijwhost.dll`, and `GuitarToolkit.PluginBridge.runtimeconfig.json` are next to it.
- Fully close and reopen your DAW.
- Rescan plugins.

## Requirements

- Windows 10/11 x64.
- .NET 8 runtime.
- For VST3: a DAW with VST3 support.
