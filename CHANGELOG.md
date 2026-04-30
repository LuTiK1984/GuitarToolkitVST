# Changelog

All notable changes to GuitarToolkit are documented here.

## [Unreleased]

### Changed

- Started interface redesign with refreshed About and Tabs layouts.
- Tabs file selection now uses one explicit source selector and one file selector instead of parallel library/recent/favorites lists.
- Removed the non-working visual sync control from the Tabs toolbar.
- Updated Tabs checkbox and slider styling to better match the dark purple theme.

## [1.3.3] - 2026-04-30

### Added

- Basic application/plugin logging to `%AppData%\GuitarToolkit\logs`.
- About page with version, GitHub link, log folder shortcut, feature summary, and third-party library notes.
- Roadmap for product direction, near-term priorities, bughunting, and commercial-readiness work.
- Tabs page now remembers the last file, selected track, volume, speed, auto-follow, sync offset, and solo/mute state.
- Tabs page now restores the last playback cursor position for the remembered file.
- Tabs page now keeps a recent files list for faster reopening.
- Tabs page now supports a simple favorites list for frequently used tab files.
- Tabs page now supports a simple library folder scan for Guitar Pro and MusicXML files.
- Tabs library can now be refreshed without selecting the folder again.
- Added `THIRD_PARTY_NOTICES.md` with dependency/license notes.

### Changed

- VST install documentation now emphasizes copying the complete plugin folder, including dependencies and `runtimes`.
- README installation notes now mention diagnostics and the log folder.
- Desktop window minimum size now matches the startup size to protect the UI layout.
- Tabs toolbar spacing was improved when controls wrap to a second row.

## [1.3.2] - 2026-04-30

### Added

- Tabs page is now enabled in the VST3 plugin after FL Studio testing with the complete dependency set.
- README screenshots and feature descriptions now include the Tabs page.

### Changed

- VST deployment now copies the full plugin output folder, including transitive dependencies and runtime folders.
- README positioning was updated from an educational project to a personal passion project.
- Updated project, desktop, and VST plugin version to `1.3.2`.
- Updated release archive names for `v1.3.2`.

### Verified

- Build passes with 0 errors and 0 warnings.
- Test suite passes: 73/73.
- FL Studio loads the VST3 plugin with the Tabs page when the complete dependency set is deployed.

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
