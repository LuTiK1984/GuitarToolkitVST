# GuitarToolkit v1.3.0

## Release Assets

- `GuitarToolkit_VST3_v.1.3.0.zip` - VST3 plugin build for DAW hosts.
- `GuitarToolkit_DESKTOP_v.1.3.0.zip` - standalone Windows desktop build.

## Highlights

- Added a new Tabs page prototype powered by alphaTab.
- Added Guitar Pro and MusicXML file loading for desktop tab viewing.
- Added track selection with selected-track solo and mute modes.
- Added tab playback controls: play/pause, stop, volume, and flexible playback speed.
- Added Tabs-page keyboard shortcuts: Space for play/pause and Esc for stop.
- Added automatic notation following during playback.
- Improved tab layout behavior for smaller windows and resize/maximize/restore scenarios.
- Removed the prototype notice panel from the Tabs page to give notation more room.
- Updated project, VST plugin, and desktop UI version to `1.3.0`.

## Known Follow-Up

- While the Tabs page adapts notation size after window resize/maximize/restore, alphaTab can visibly re-render a few times before settling.

## Included Documentation

- English and Russian installation guides are included in release archives.
- `CHANGELOG.md` documents the `1.3.0` release.

## Verification

- Build: passed with 0 errors and 0 warnings.
- Tests: 73/73 passed.

## Requirements

- Windows 10/11 x64.
- .NET 8 runtime.
- For VST3: a DAW host with VST3 support.
