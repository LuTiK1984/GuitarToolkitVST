# GuitarToolkit Roadmap

GuitarToolkit is a personal, idea-driven guitar toolkit for Windows, available as a standalone desktop app and a VST3 plugin. The goal is to grow it from a useful passion project into a polished, trustworthy tool for guitar practice, writing, and DAW workflows.

## Product Direction

- Keep the project musician-first: fast to open, clear to use, and useful during real practice or recording sessions.
- Treat Desktop and VST3 as first-class targets, but validate DAW behavior carefully before calling a feature stable.
- Prefer practical workflow improvements over feature count.
- Keep the open-source/passion-project spirit while gradually improving product polish.

## Near-Term Priorities

- Improve VST3 installation experience:
  - make it obvious that the whole release folder must be copied;
  - document FL Studio and Reaper setup separately;
  - keep `deploy-vst.bat` aligned with the full dependency set.
- Add basic diagnostics:
  - write app/plugin errors to `%AppData%\GuitarToolkit\logs`;
  - include DAW/plugin startup failures where possible;
  - add a simple way to find logs from the UI or README.
- Add an About/Version window:
  - app version;
  - GitHub link;
  - license information;
  - third-party notices for alphaTab, AudioPlugSharp, NAudio, and related libraries.
- Polish the Tabs page:
  - reduce visible re-render jitter after resize/maximize/restore;
  - save last tab file, selected track, volume, speed, and auto-follow state;
  - keep testing tab playback inside FL Studio.
- Rebuild the interface layout as a dedicated design pass:
  - introduce consistent spacing, sizing, and panel structure;
  - use the refined Tabs page as the control-size and toolbar-density reference before applying the style across the rest of the app;
  - reduce the current feeling of scattered controls;
  - make tabs, buttons, and mode controls clearly named and visually distinguishable;
  - add an interface language switch and prepare UI text for localization;
  - move input device selection into the Tuner tab instead of keeping it global on every page;
  - improve visual hierarchy across Desktop and VST without breaking workflows.
- Improve first-run clarity:
  - short quick-start guidance in README;
  - clearer “select input device first” flow;
  - simple screenshots for the most common workflows.

## Medium-Term Ideas

- Installer or polished portable package:
  - Desktop installer;
  - clearer VST3 folder layout;
  - possibly Inno Setup/MSI later.
- DAW-aware plugin behavior:
  - read host tempo if AudioPlugSharp exposes it reliably;
  - explore transport sync for metronome/tab playback;
  - avoid global hotkey conflicts inside DAW hosts.
- Better tab workflow:
  - polish the dedicated tab library folder UI;
  - add refresh/search/sort controls for library files;
  - per-track volume/mute/solo state;
  - export or print later if alphaTab supports it well.
- Better progression workflow:
  - grow the new preset dropdowns into a fuller preset manager with search, metadata, and safer saved-preset actions;
  - allow drag-and-drop reordering of chords in the current progression;
  - add duplicate and undo actions for fast progression editing;
  - support per-chord duration such as 1, 2, or 4 beats;
  - add playback patterns such as block chords, strum, and arpeggio;
  - export progressions as text and possibly MIDI;
  - decide whether saved presets should include key/mode or remain degree-only templates.
- Better interval-training workflow:
  - add ascending, descending, and harmonic interval modes;
  - add focused practice sets such as only seconds/thirds, only perfect intervals, or only trouble intervals;
  - track mistakes per interval and surface weak spots in the UI;
  - add optional guitar fretboard hints after answering;
  - support custom root-note range and timbre choices for closer guitar-practice feel;
  - add a slower review mode that explains the answer before auto-advance.
- Better scale-practice workflow:
  - add position presets for common fretboard boxes and CAGED shapes;
  - let users filter the displayed scale by fret range and string range;
  - add optional degree highlighting for tonic, thirds, fifths, sevenths, and avoid notes;
  - add exercises such as find the next note, name the degree, and play through a position;
  - compare two scales or modes on the same fretboard;
  - export or copy scale fingering diagrams for practice notes.
- Better chord workflow:
  - add quick chord search by name and type;
  - turn favorites into a fuller saved-chords list with rename/remove actions;
  - compare multiple voicings side by side;
  - add optional fingering hints for common shapes;
  - add transpose-shape controls for moving a voicing across the fretboard;
  - suggest related substitutions such as sus, add9, seventh, and relative minor/major options;
  - add a chord-change trainer with timed practice and accuracy tracking.
- Better metronome workflow:
  - add count-in before playback or practice takes;
  - support multiple click sounds and accent patterns;
  - add tap-tempo averaging feedback and reset;
  - add tempo-ramp practice for gradual speed increases;
  - add saved tempo presets by song or exercise;
  - explore host-tempo sync for the VST when AudioPlugSharp exposes reliable transport data.
- Improve audio UX:
  - safer volume defaults;
  - smoother tab playback volume changes;
  - optional metronome/count-in integration.
- Project trust:
  - clear license;
  - third-party notices;
  - code signing research for future Windows releases.

## Bughunting Checklist

- Tabs page visibly re-renders several times while adapting after resize/maximize/restore.
- Verify VST3 startup and editor opening in FL Studio after each release.
- Verify VST3 deployment with the full dependency set, including `runtimes`.
- Verify DAW behavior when assigning/changing recording input after plugin load.
- Test tab loading with GP3, GP4, GP5/GPX, and MusicXML files.
- Check scroll/auto-follow behavior on small windows, maximized windows, and restored windows.
- Check that Desktop and VST3 do not diverge unexpectedly in shared UI behavior.
- Restoring the saved tab cursor tick does not currently move alphaTab from the beginning after app restart.

## Commercial Readiness Notes

Current state: strong niche open-source/passion project with product potential.

Before it feels commercial-grade:

- installation must be simpler and less scary;
- VST3 stability should be verified across multiple DAWs;
- logs/crash diagnostics should exist;
- license and third-party notices should be clear;
- release packages should feel intentional, not like raw build folders;
- Windows signing/installer strategy should be researched.
