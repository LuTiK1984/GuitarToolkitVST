# Changelog

All notable changes to GuitarToolkit are documented here.

## [1.3.1] - 2026-04-30

### Fixed

- VST3 release package now includes transitive alphaTab/AlphaSkia dependencies required by the shared UI assembly.
- The VST3 plugin no longer instantiates the Tabs page inside DAW hosts; the alphaTab-based Tabs page remains desktop-only for now.

### Changed

- Updated project, desktop, and VST plugin version to `1.3.1`.
- Updated release archive names for `v1.3.1`.

### Verified

- Build passes with 0 errors and 0 warnings.
- Test suite passes: 73/73.
- VST3 Release output contains `AlphaTab.dll`, `AlphaTab.Windows.dll`, `AlphaSkia.dll`, and `AlphaSkia.Native.Windows.dll`.

## [1.3.0] - 2026-04-30

### Added

- New Tabs page prototype powered by alphaTab.
- Guitar Pro and MusicXML loading for desktop tab viewing.
- Track selection, selected-track solo/mute, play/pause/stop controls, volume control, and flexible playback speed input.
- Keyboard shortcuts on the active Tabs page: Space for play/pause and Esc for stop.
- Automatic cursor following for long tablatures.

### Changed

- Improved Tabs page layout for smaller windows.
- Removed prototype notice panel from the Tabs page to give more space to notation.
- Added adaptive alphaTab rendering for window resize, maximize, and restore.
- Updated project, desktop, and VST plugin version to `1.3.0`.
- Updated release archive names for `v1.3.0`.

### Known Follow-Up

- The Tabs page can visibly re-render a few times while adapting notation size after window resize/maximize/restore.

### Verified

- Build passes with 0 errors and 0 warnings.
- Test suite passes: 73/73.

## [1.2.0] - 2026-04-29

### Added

- Application icon for the standalone desktop executable and main window.
- GitHub README badges.
- README screenshots for tuner, chord library, and fretboard/scales.
- Release checklist for repeatable publishing.
- GitHub issue templates for bug reports, DAW compatibility reports, and feature requests.

### Changed

- Updated project, desktop, and VST plugin version to `1.2.0`.
- Updated installation docs and release archive names for `v1.2.0`.

### Verified

- Build passes with 0 errors and 0 warnings.
- Test suite passes: 73/73.

## [1.1.0] - 2026-04-29

### Added

- Bilingual GitHub README with English and Russian project descriptions.
- `AGENTS.md` with development notes for VST, verification, and project boundaries.
- `Directory.Build.props` with shared project metadata and version `1.1.0`.
- Additional `MetronomeEngine` unit tests.
- Release notes for `v1.1.0`.

### Changed

- Updated VST plugin version to `1.1.0`.
- Updated desktop window title to `GuitarToolkit v1.1.0`.
- Improved `deploy-vst.bat` with required-file checks and clearer error messages.
- User settings load/save failures now write diagnostic messages to debug output.

### Verified

- Build passes with 0 errors and 0 warnings.
- Test suite passes: 73/73.
- VST plugin no longer crashes in the tested DAW scenario where recording inputs are assigned after the plugin is loaded.

## [1.0.0] - 2026-04-20

### Added

- Initial GuitarToolkit release.
- VST3 plugin via AudioPlugSharp.
- Standalone WPF desktop application via NAudio.
- Shared WPF UI library.
- Core DSP and theory library with tuner, metronome, chord library, fretboard scales, interval trainer, progression builder, and circle of fifths.
- xUnit test project for pitch detection and core music theory models.
