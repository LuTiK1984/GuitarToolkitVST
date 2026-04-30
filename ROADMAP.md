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
  - recent files;
  - favorites/library folder;
  - per-track volume/mute/solo state;
  - export or print later if alphaTab supports it well.
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

## Commercial Readiness Notes

Current state: strong niche open-source/passion project with product potential.

Before it feels commercial-grade:

- installation must be simpler and less scary;
- VST3 stability should be verified across multiple DAWs;
- logs/crash diagnostics should exist;
- license and third-party notices should be clear;
- release packages should feel intentional, not like raw build folders;
- Windows signing/installer strategy should be researched.
