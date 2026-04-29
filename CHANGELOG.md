# Changelog

All notable changes to GuitarToolkit are documented here.

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
