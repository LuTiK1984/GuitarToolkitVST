# Supported DAWs

GuitarToolkit ships a VST3 plugin package. DAW compatibility depends on the DAW,
Windows, plugin scanning, and the full VST3 runtime folder being copied.

## Current status

| DAW | Status | Notes |
| --- | --- | --- |
| FL Studio | Primary manual target | Re-scan after deployment. Check input assignment after plugin load. |
| Reaper | Planned/manual target | Use the common VST3 folder and clear cache/rescan if the plugin does not appear. |
| Cubase | Untested | Should be compatible with VST3, but needs manual validation. |
| Ableton Live | Untested | Should be compatible with VST3, but needs manual validation. |
| Other VST3 hosts | Unknown | Reports are welcome through the DAW compatibility issue template. |

## Manual compatibility checklist

1. Close the DAW before replacing plugin files.
2. Install the full `GuitarToolkit` VST3 folder, not only the bridge `.vst3`.
3. Re-scan plugins.
4. Add GuitarToolkit to an audio track.
5. Assign or change the recording input after the plugin is loaded.
6. Check tuner input, metronome output, and chord/scale playback.
7. Open the Tabs page and verify editor stability if the DAW can host the UI.

Logs are written to:

```text
%AppData%\GuitarToolkit\logs
```
