# Installing GuitarToolkit

## Desktop Version

1. Download `GuitarToolkit_DESKTOP_v.1.3.2.zip` from GitHub Releases.
2. Extract the archive to any folder.
3. Run `GuitarToolkit.Desktop.exe`.
4. Select an input device in the top panel: microphone, line input, or audio interface.

## VST3 Plugin

1. Download `GuitarToolkit_VST3_v.1.3.2.zip` from GitHub Releases.
2. Close your DAW if it is running.
3. Extract the archive into one folder named `GuitarToolkit`.
4. Copy the whole `GuitarToolkit` folder to:

```text
C:\Program Files\Common Files\VST3\GuitarToolkit\
```

Do not copy only the `.vst3` file. The plugin also needs the DLL files and the `runtimes` folder from the archive.

5. Confirm Administrator access if Windows asks for permission.
6. Open your DAW and rescan VST3 plugins.
7. Add GuitarToolkit as an effect on an audio track.

## FL Studio

1. Open `Options -> Manage plugins`.
2. Make sure `C:\Program Files\Common Files\VST3` is in the plugin search paths.
3. Click `Find installed plugins`.
4. Add GuitarToolkit to the mixer as an effect plugin.
5. If you replaced files manually, restart FL Studio before rescanning.

## Reaper

1. Open `Options -> Preferences -> Plug-ins -> VST`.
2. Make sure `C:\Program Files\Common Files\VST3` is in the VST paths.
3. Click `Re-scan`.
4. Add GuitarToolkit as an FX plugin on an audio track.

## If Your DAW Does Not See the Plugin

- Check that `GuitarToolkit.PluginBridge.vst3` is in the VST3 folder.
- Check that the whole release folder was copied, including `GuitarToolkit.Plugin.dll`, `GuitarToolkit.UI.dll`, `AlphaTab.dll`, `AlphaTab.Windows.dll`, `AlphaSkia.dll`, `AlphaSkia.Native.Windows.dll`, `AudioPlugSharp*.dll`, `NAudio*.dll`, `Ijwhost.dll`, `GuitarToolkit.PluginBridge.runtimeconfig.json`, and the `runtimes` folder.
- Fully close and reopen your DAW.
- Rescan plugins.
- Check logs in `%AppData%\GuitarToolkit\logs` if the plugin opens and then fails.

## Requirements

- Windows 10/11 x64.
- .NET 8 runtime.
- For VST3: a DAW with VST3 support.
