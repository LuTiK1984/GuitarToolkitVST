# Reaper VST3 Setup

This page documents the recommended Reaper setup flow for GuitarToolkit VST3.

## Install

1. Close Reaper.
2. Extract `GuitarToolkit_VST3_v.<version>.zip`.
3. Copy the full extracted `GuitarToolkit` plugin folder to:

   ```text
   C:\Program Files\Common Files\VST3\GuitarToolkit\
   ```

4. Do not copy only `GuitarToolkit.PluginBridge.vst3`. The plugin also needs
   its DLL dependencies and the `runtimes` folder.

## Scan

1. Open Reaper.
2. Go to `Options -> Preferences -> Plug-ins -> VST`.
3. Make sure this path is present:

   ```text
   C:\Program Files\Common Files\VST3
   ```

4. Click `Clear cache/re-scan` if GuitarToolkit does not appear.
5. Add GuitarToolkit to a track as an FX plugin.

## Smoke test

1. Assign or change the track input after GuitarToolkit is loaded.
2. Arm the track if needed for input monitoring.
3. Check tuner input.
4. Start and stop the metronome.
5. Play a chord and a scale.
6. Open the Tabs page and load a small Guitar Pro or MusicXML file if needed.

## Troubleshooting

- If Reaper does not find the plugin, verify the VST3 scan path and run
  `Clear cache/re-scan`.
- If the editor fails to open after updating, restart Reaper.
- If Tabs fail, check that the `runtimes` folder is present.
- Logs are written to `%AppData%\GuitarToolkit\logs`.
